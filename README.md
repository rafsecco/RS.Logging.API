# RS.Logging

> Versão atual: **2.0.0** — veja o [CHANGELOG](CHANGELOG.md) para o histórico de mudanças.

Projeto de minimal API (.NET 10) para fazer o log geral e log de processos de várias aplicações.

Objetivo: entender problemas técnicos (exceções, erros HTTP, tempo de resposta, uso de
endpoints, falhas externas, métricas e rastreamento entre serviços).

## Subindo o projeto com Docker

Os compose files ficam em `docker/`. Cada banco de dados tem seu próprio arquivo base,
combinado com um override de ambiente (`dev` ou `prod`):

```bash
# MariaDB — DEV (Swagger habilitado, banco exposto na 3306, seed de dados de teste)
docker compose -f docker/docker-compose.mariadb.yml -f docker/docker-compose.dev.yml up --build

# MariaDB — PRD
docker compose -f docker/docker-compose.mariadb.yml -f docker/docker-compose.prod.yml up --build

# SQL Server — DEV
docker compose -f docker/docker-compose.sqlserver.yml -f docker/docker-compose.dev.yml up --build

# PostgreSQL — DEV
docker compose -p rs-logstream -f docker/docker-compose.postgres.yml -f docker/docker-compose.dev.yml up -d --build
```

As credenciais ficam em `docker/.env` (não versionado). Na primeira subida, as migrations
são aplicadas automaticamente e, em DEV, dados de teste são inseridos via seed.

- API (DEV): http://localhost:5000
- Swagger (DEV): http://localhost:5000/swagger

Para testar os endpoints, importe a collection do Postman disponível em `postman/`.

## Ingestão de logs

`POST /logs` e `POST /log-process/detail` não gravam no banco de forma síncrona: o
registro é colocado em uma fila em memória (`System.Threading.Channels`) e processado
por um `BackgroundService`, retornando `202 Accepted` imediatamente. Assim, uma falha
ou indisponibilidade do banco não derruba a API que está enviando logs.

`POST /log-process` continua síncrono e retorna `200 OK` com o `Id` gerado, que é o
identificador necessário (FK) para os `POST /log-process/detail` subsequentes.

## Multi-tenant

A API identifica a aplicação de origem através do header HTTP `X-Tenant-Id`. É uma
string livre, sem necessidade de cadastro prévio — qualquer valor é aceito como
identificador de tenant.

- **Gravação**: `POST /logs`, `POST /log-process`, `POST /logs/batch` e
  `POST /api-call-log` leem o header `X-Tenant-Id` e gravam o valor no registro
  (`TenantId`, campo opcional em `Log`, `LogProcess` e `ApiCallLog`).
  `LogProcessDetail` não possui `TenantId` próprio — ele é resolvido a partir do
  `LogProcess` pai.
- **Leitura**: todos os endpoints `GET` também leem `X-Tenant-Id` e, **se o header for
  enviado**, filtram os resultados para aquele tenant.
- **Retrocompatibilidade**: o header é opcional. Se não for enviado, nenhum filtro de
  tenant é aplicado e o comportamento é o mesmo de antes (todos os registros são
  retornados).
- Tenants sem `X-Tenant-Id` (valor `null`) também podem ser filtrados por retenção
  através da política padrão — veja [Retention por aplicação](#retention-por-aplicação).

## CorrelationId / TraceId

Para rastrear uma requisição entre serviços, a API aceita os headers
`X-Correlation-Id` e `X-Trace-Id`:

- **Gravação**: `POST /logs`, `POST /log-process`, `POST /log-process/detail`,
  `POST /api-call-log` e os endpoints de batch leem esses headers e gravam os valores
  (`CorrelationId` / `TraceId`, campos opcionais) em todas as entidades.
- **Busca**: os endpoints `/search` aceitam os query params `cid` (CorrelationId) e
  `tid` (TraceId) para filtrar pelos valores gravados.

## Batch ingestion

Para enviar vários registros de uma vez, sem uma requisição por item:

- `POST /logs/batch` — recebe `List<LogsViewModel>` no corpo. Os headers
  `X-Tenant-Id`, `X-Correlation-Id` e `X-Trace-Id` são aplicados a **todos** os itens
  do lote. Cada item é enfileirado individualmente e a resposta é `202 Accepted`.
- `POST /log-process/detail/batch` — recebe `List<LogProcessDetailsViewModel>` no
  corpo. Os headers `X-Correlation-Id` e `X-Trace-Id` são aplicados a todos os itens;
  `X-Tenant-Id` não se aplica (o tenant é o do `LogProcess` pai). Retorna
  `202 Accepted`.

## Busca full-text

Além dos filtros por substring (`msg`/`st`, que usam `LIKE`), `GET /logs/search` e
`GET /log-process/search` aceitam o parâmetro `q` para busca full-text sobre `Message`
e `StackTrace`.

O comportamento de `q` depende do provider configurado:
- **MariaDB**: usa índices `FULLTEXT` nativos com
  `EF.Functions.Match(..., MySqlMatchSearchMode.NaturalLanguage)`. O modo
  `NaturalLanguage` aplica as regras padrão do MariaDB: palavras muito curtas (menor que
  `innodb_ft_min_token_size`, padrão 3 caracteres) e *stopwords* comuns são ignoradas.
- **SQL Server / Postgres**: fallback com `Contains()` (mesmo comportamento do
  parâmetro `msg`).

`q` é adicional aos filtros `msg`/`st` existentes — pode ser usado isoladamente ou
combinado com os demais filtros.

## Log de chamadas de API

Para registrar chamadas HTTP que um serviço faz para APIs externas, use o endpoint
`POST /api-call-log`. O registro captura o contexto completo da chamada para facilitar
o diagnóstico de falhas de integração.

**Campos principais:**

| Campo | Tipo | Descrição |
|---|---|---|
| `url` | string | URL chamada |
| `httpMethod` | string | GET, POST, PUT... |
| `isSuccess` | bool | sucesso ou falha |
| `requestBody` | string? | body enviado |
| `requestHeaders` | string? | headers relevantes (texto livre ou JSON) |
| `responseStatusCode` | int? | 200, 404, 500... `null` se sem resposta |
| `responseBody` | string? | body recebido (`null` se timeout/exception) |
| `durationMs` | long? | tempo da chamada em ms |
| `errorMessage` | string? | mensagem de exceção, se houver |

Os headers `X-Tenant-Id`, `X-Correlation-Id` e `X-Trace-Id` também são aceitos para
correlacionar o registro com os demais logs do fluxo.

**Endpoints disponíveis:**
- `POST /api-call-log` — registra uma chamada (`202 Accepted`, via fila de ingestão)
- `GET /api-call-log` — lista paginada (`pn`, `ps`, `X-Tenant-Id`)
- `GET /api-call-log/{id}` — por id
- `GET /api-call-log/search` — filtros: `ds`/`de` (data), `ok` (true/false — isSuccess),
  `sc` (status code), `url` (substring), `cid`, `tid`, `X-Tenant-Id`

## Autenticação

Todos os endpoints exigem um JWT válido no header `Authorization: Bearer <token>`. O
token deve ser emitido pela Auth API configurada em `appsettings.json`:

```json
"Auth": {
  "Authority": "https://auth-api.internal",
  "Audience": "rs-logging",
  "Issuer": "auth-api",
  "RequireHttpsMetadata": true
}
```

O Swagger UI (`/swagger`) exibe o botão **Authorize** para colar o token e testar os
endpoints diretamente.

### Testando sem a Auth API (ambiente de desenvolvimento)

Em Development (local e Docker), a API usa chave simétrica configurada em
`appsettings.Development.json` — sem necessidade de um servidor de autenticação externo.

**Gerar um token de teste — PowerShell (sem instalação):**

```powershell
$secret  = "rs-logging-dev-secret-minimo-32-chars!"
$header  = [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes('{"alg":"HS256","typ":"JWT"}')) -replace '=+$' -replace '\+','-' -replace '/','_'
$exp     = [DateTimeOffset]::UtcNow.AddHours(1).ToUnixTimeSeconds()
$payload = [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes("{`"iss`":`"auth-api`",`"aud`":`"rs-logging`",`"exp`":$exp}")) -replace '=+$' -replace '\+','-' -replace '/','_'
$hmac    = [Security.Cryptography.HMACSHA256]::new([Text.Encoding]::UTF8.GetBytes($secret))
$sig     = [Convert]::ToBase64String($hmac.ComputeHash([Text.Encoding]::ASCII.GetBytes("$header.$payload"))) -replace '=+$' -replace '\+','-' -replace '/','_'
"$header.$payload.$sig"
```

**Alternativa visual — jwt.io:**

Acesse [jwt.io](https://jwt.io), selecione algoritmo `HS256` e preencha:
- `iss`: `auth-api`
- `aud`: `rs-logging`
- `exp`: timestamp Unix futuro (ex: `9999999999`)
- Secret: `rs-logging-dev-secret-minimo-32-chars!`

Cole o token gerado no campo **Authorize** do Swagger ou no header das requisições:

```
Authorization: Bearer <token gerado>
```

Quando a Auth API estiver pronta, basta setar `Auth:Authority` com a URL real e remover
`Auth:Secret` do `appsettings.Development.json`.

## Suporte a múltiplos bancos de dados

O provider de banco é selecionado via `appsettings.json`:

```json
"DatabaseProvider": "MariaDB"
```

Valores aceitos: `"MariaDB"` | `"SqlServer"` | `"Postgres"`.

Cada provider tem sua própria connection string:
```json
"ConnectionStrings": {
  "ConnMariaDB":  "Server=localhost; Port=3306; Database=RS.Logging; ...",
  "ConnSqlServer": "Server=localhost,1433; Database=RS.Logging; ...",
  "ConnPostgres":  "Host=localhost; Port=5432; Database=rs_logging; ..."
}
```

**Diferenças por provider:**

| Recurso | MariaDB | SQL Server | Postgres |
|---|---|---|---|
| Tipo de ID | `BIGINT UNSIGNED` | `BIGINT` | `BIGINT` |
| Texto longo | `LONGTEXT` | `NVARCHAR(MAX)` | `TEXT` |
| Default datetime | `NOW()` | `GETDATE()` | `NOW()` |
| Busca full-text (`q`) | FULLTEXT nativo | LIKE (fallback) | LIKE (fallback) |

As migrations ficam em pastas separadas por provider (`Migrations/MariaDB/`,
`Migrations/SqlServer/`, `Migrations/Postgres/`). Sempre gere e aplique migrations
com o `DatabaseProvider` correto configurado.

## Auditoria de processos

`GET /log-process/audit` e `GET /log-process/audit/{id}` retornam os processos com
**status agregado** calculado a partir do pior nível de log entre os detalhes:

| Status | Condição |
|---|---|
| `Success` (0) | Nenhum detalhe, ou todos `Information`/`Debug` |
| `Warning` (1) | Ao menos um detalhe `Warning` |
| `Error` (2) | Ao menos um detalhe `Error` |
| `Critical` (3) | Ao menos um detalhe `Critical` |

Ambos os endpoints aceitam `X-Tenant-Id` para filtrar por aplicação.

## Retention por aplicação

A retenção de logs é configurada em `appsettings.json`, sem necessidade de uma tabela
de cadastro:

```json
"Retention": {
  "DefaultDays": 90,
  "IntervalHours": 24,
  "PolicyByTenant": {
    "appA": 30,
    "appB": 180
  }
}
```

- `DefaultDays`: quantidade de dias que um registro é mantido por padrão.
- `PolicyByTenant`: dicionário `TenantId -> dias`. Tenants listados aqui usam esse
  valor em vez do `DefaultDays`. Tenants ausentes (incluindo registros sem
  `X-Tenant-Id`) usam `DefaultDays`.
- `IntervalHours`: intervalo, em horas, entre execuções do expurgo.

O `LogRetentionWorker` (`BackgroundService`) executa periodicamente, calcula o
`cutoff` (`DateTime.Now - dias`) por tenant e remove de `TB_Log` e `TB_LogProcess` os
registros mais antigos que o `cutoff`. Os registros correspondentes em
`TB_LogProcessDetail` são removidos automaticamente via `ON DELETE CASCADE`. Falhas no
expurgo de um tenant são logadas e não interrompem o worker nem a API.

## Padronização dos commits

Seguir o padrão de [Conventional Commits](https://www.conventionalcommits.org/):
```
feat:     nova funcionalidade
fix:      correção de bug
chore:    tarefas de manutenção / config
refactor: refatoração sem mudança de comportamento
docs:     documentação
```

## Roadmap

- [X] Log geral
- [X] Log de processos
- [X] Projeto de testes
- [X] Auditoria de processos (status agregado por processo)
- [X] Upgrade para .NET 10
- [X] Ingestão assíncrona (fila/background) para não gravar log direto no banco da API
- [X] CORS de produção configurável via `appsettings.Production.json`
- [X] CorrelationId / TraceId
- [X] Multi-tenant
- [X] Busca full-text
- [X] Batch ingestion
- [X] Retention por aplicação
- [X] Reorganização de endpoints (rotas REST, arquivos por grupo)
- [X] Log de chamadas de API (`ApiCallLog`)
- [X] Suporte a múltiplos bancos de dados (MariaDB, SQL Server, Postgres)
- [X] Segurança (autenticação JWT Bearer via Auth API externa)
- [ ] Compressão
- [ ] Webhook
- [ ] Dashboard
- [ ] Export para Elastic/OpenSearch
- [ ] Dead-letter queue
