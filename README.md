# RS.Logging

Projeto de minimal API (.NET 10) para fazer o log geral e log de processos de outras aplicações.

Objetivo: entender problemas técnicos (exceções, erros HTTP, tempo de resposta, uso de
endpoints, falhas externas, métricas e rastreamento entre serviços).

## Subindo o projeto com Docker

O `docker-compose.yml` cria dois containers: um para o banco de dados (MariaDB) e outro
para a API (.NET).

Na raiz do projeto, execute:
```bash
docker compose -f docker/docker-compose.yml up --build
```

- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger (apenas em Development)

Para testar os endpoints, importe a collection do Postman disponível em `postman/`.

## Ingestão de logs

`CreateLog` e `CreateLogProcessDetail` não gravam no banco de forma síncrona: o
registro é colocado em uma fila em memória (`System.Threading.Channels`) e processado
por um `BackgroundService`, retornando `202 Accepted` imediatamente. Assim, uma falha
ou indisponibilidade do banco não derruba a API que está enviando logs.

`CreateLogProcess` continua síncrono e retorna `200 OK` com o `Id` gerado, que é o
identificador necessário (FK) para os `CreateLogProcessDetail` subsequentes.

## Multi-tenant

A API identifica a aplicação de origem através do header HTTP `X-Tenant-Id`. É uma
string livre, sem necessidade de cadastro prévio — qualquer valor é aceito como
identificador de tenant.

- **Gravação**: `POST /CreateLog/`, `POST /CreateLogProcess/` e `POST /CreateLogBatch/`
  leem o header `X-Tenant-Id` e gravam o valor no registro (`TenantId`, campo opcional
  em `Log` e `LogProcess`). `LogProcessDetail` não possui `TenantId` próprio — ele é
  resolvido a partir do `LogProcess` pai (via `cd_Process`).
- **Leitura**: todos os endpoints `GET` (`/GetAll/`, `/GetById/{id}`, `/Search/`,
  `/GetAllLogProcess/`, `/GetLogProcessById/{id}/`, `/LogProcessSearch/`,
  `/LogProcessAudit/`, `/LogProcessAudit/{id}`) também leem `X-Tenant-Id` e, **se o
  header for enviado**, filtram os resultados para aquele tenant.
- **Retrocompatibilidade**: o header é opcional. Se não for enviado, nenhum filtro de
  tenant é aplicado e o comportamento é o mesmo de antes (todos os registros são
  retornados).
- Tenants sem `X-Tenant-Id` (valor `null`) também podem ser filtrados por retenção
  através da política padrão — veja [Retention por aplicação](#retention-por-aplicação).

## CorrelationId / TraceId

Para rastrear uma requisição entre serviços, a API aceita os headers
`X-Correlation-Id` e `X-Trace-Id`:

- **Gravação**: `POST /CreateLog/`, `POST /CreateLogProcess/`, `POST /CreateLogProcessDetail/`
  e os endpoints de batch leem esses headers e gravam os valores (`CorrelationId` /
  `TraceId`, campos opcionais) em `Log`, `LogProcess` e `LogProcessDetail`.
- **Busca**: `GET /Search/` e `GET /LogProcessSearch/` aceitam os query params `cid`
  (CorrelationId) e `tid` (TraceId) para filtrar pelos valores gravados.

## Batch ingestion

Para enviar vários registros de uma vez, sem uma requisição por item:

- `POST /CreateLogBatch/` — recebe `List<LogsViewModel>` no corpo. Os headers
  `X-Tenant-Id`, `X-Correlation-Id` e `X-Trace-Id` são aplicados a **todos** os itens
  do lote (um lote representa uma única origem/contexto de trace). Cada item é
  enfileirado individualmente na fila de ingestão e a resposta é `202 Accepted`.
- `POST /CreateLogProcessDetailBatch/` — recebe `List<LogProcessDetailsViewModel>` no
  corpo. Os headers `X-Correlation-Id` e `X-Trace-Id` são aplicados a todos os itens;
  `X-Tenant-Id` não se aplica (o tenant é o do `LogProcess` pai). Retorna
  `202 Accepted`.

## Busca full-text

Além dos filtros por substring (`msg`/`st`, que usam `LIKE`), `GET /Search/` e
`GET /LogProcessSearch/` aceitam o parâmetro `q` para busca full-text usando os
índices `FULLTEXT` do MariaDB sobre `Message` e `StackTrace`
(`EF.Functions.Match(..., MySqlMatchSearchMode.NaturalLanguage)`).

- O modo `NaturalLanguage` aplica as regras padrão do MariaDB: palavras muito curtas
  (menor que `innodb_ft_min_token_size`, padrão 3 caracteres) e *stopwords* comuns são
  ignoradas na busca.
- `q` é adicional aos filtros `msg`/`st` existentes — pode ser usado isoladamente ou
  combinado com os demais filtros de `/Search/` e `/LogProcessSearch/`.

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
- [ ] Refatorar
- [ ] Segurança (autenticação/autorização)
- [ ] Compressão
- [ ] Webhook
- [ ] Dashboard
- [ ] Export para Elastic/OpenSearch
- [ ] Dead-letter queue
