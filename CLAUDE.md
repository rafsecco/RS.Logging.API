# Prompt para gerar este projeto

Este arquivo não documenta o RS.Logstream — ele contém o prompt que, entregue a um agente,
regeneraria o projeto do zero com a mesma arquitetura, convenções e funcionalidades. Para
documentação de uso normal, veja [README.md](README.md); para comandos de EF Core, veja
[MIGRATIONS.md](MIGRATIONS.md).

---

Crie uma Web API em .NET 10 (minimal API, sem controllers) chamada **RS.Logstream**, cujo
objetivo é centralizar o log técnico — geral, de processos de negócio, e de chamadas HTTP de
saída — de várias aplicações clientes, para diagnóstico de exceções, erros HTTP, tempo de
resposta, uso de endpoints, falhas externas e rastreamento entre serviços.

## Requisito não funcional crítico

Nunca grave logs de forma síncrona e direta no banco dentro do request HTTP. Use uma fila em
memória + `BackgroundService` para desacoplar a escrita do banco da requisição, de forma que
uma falha ou lentidão do banco de logs jamais derrube ou atrase a aplicação que está enviando
logs.

## Arquitetura

Organize a solução em camadas com dependency flow API → Domain ← Infra (Domain não depende de
infraestrutura):

- **`RS.Core`** — biblioteca compartilhada: `BaseEntity` (Id `ulong` + CreatedAt) e
  `PagedModel<T>` genérico (TotalResults/PageNumber/PageSize/List — disponível para paginação
  com metadados, mesmo que os endpoints atuais retornem arrays simples).
- **`RS.Logstream.Domain`** — entidades e interfaces de repositório, sem EF Core. Organize por
  feature: `Log/`, `LogProcess/`, `ApiCall/`, cada uma com sua interface em `Contracts/`.
- **`RS.Logstream.Infra`** — implementação dos repositórios com EF Core, mappings
  (`IEntityTypeConfiguration` por entidade, recebendo `IDbColumnTypes` via construtor),
  `DbContext`, migrations e abstrações de provider de banco.
- **`RS.Logstream.API`** — minimal API: endpoints, DI (`Configurations/`, um arquivo por
  responsabilidade), Swagger, CORS, autenticação, ingestão assíncrona e retenção.
- **`RS.Logstream.Tests`** (projeto separado, ex. `src/Tests/`) — testes de repositório/domínio
  e testes de integração HTTP.

## Entidades

Todas herdam de `BaseEntity` (Id `ulong`, CreatedAt) e usam construtor com parâmetros
prefixados `p` (ex. `pTenantId`) + construtor protegido sem parâmetros para o EF Core:

- **Log** — LogLevel, Message (obrigatório), StackTrace?, TenantId?, CorrelationId?, TraceId?
- **LogProcess** — ProcessId (`uint`, identificador de negócio do processo), Name?, lista de
  `LogProcessDetail`, TenantId?, CorrelationId?, TraceId?. Método `GetStatus()` calcula um
  `ProcessStatus` (Success=0/Warning=1/Error=2/Critical=3) a partir do pior `LogLevel` entre os
  details (nenhum detail = Success).
- **LogProcessDetail** — LogProcessId (FK), LogLevel, Message, StackTrace?, CorrelationId?,
  TraceId? (herda TenantId do `LogProcess` pai — não tem campo próprio).
- **ApiCallLog** — Url, HttpMethod, IsSuccess, RequestBody?, RequestHeaders?,
  ResponseStatusCode?, ResponseBody?, DurationMs?, ErrorMessage?, TenantId?, CorrelationId?,
  TraceId?.

## Funcionalidades obrigatórias

1. **Ingestão assíncrona**: `POST /logs`, `POST /log-process/detail`, `POST /api-call-log` e os
   endpoints batch enfileiram o item em um `Channel` in-memory (`ILogIngestionQueue`) e
   retornam `202 Accepted`. Um `BackgroundService` (`LogIngestionWorker`) consome a fila, abre
   um scope de DI e grava no banco. Exceção: `POST /log-process` é síncrono e retorna `200 OK`
   com o Id gerado (é a FK necessária para os `POST /log-process/detail` seguintes).
2. **Multi-tenant** via header `X-Tenant-Id` (string livre, sem tabela de cadastro). Grava no
   registro quando enviado no POST; filtra `GET`/`search`/`audit` quando enviado. Opcional e
   retrocompatível (sem header = sem filtro).
3. **CorrelationId / TraceId** via headers `X-Correlation-Id` / `X-Trace-Id`, gravados em todas
   as entidades e filtráveis nos endpoints de busca via `cid`/`tid`.
4. **Batch ingestion**: `POST /logs/batch` e `POST /log-process/detail/batch` recebem uma lista
   e enfileiram cada item individualmente, retornando `202 Accepted`.
5. **Busca full-text**: parâmetro `q` em `/logs/search` e `/log-process/search`. Abstraia via
   `IFullTextSearchProvider`: implementação MariaDB usa
   `EF.Functions.Match(..., MySqlMatchSearchMode.NaturalLanguage)` sobre índices `FULLTEXT`;
   SQL Server e Postgres usam fallback com `Contains()` (mesmo comportamento do filtro
   `msg`/`st` por substring).
6. **Retenção por tenant**: `LogRetentionWorker` (`BackgroundService`) expurga periodicamente
   `Log` e `LogProcess` mais antigos que um cutoff configurável por tenant via
   `appsettings.json` (`Retention:DefaultDays`, `Retention:PolicyByTenant`,
   `Retention:IntervalHours`). `LogProcessDetail` é removido em cascata
   (`ON DELETE CASCADE`). Falha de expurgo de um tenant não deve derrubar o worker nem a API.
7. **Log de chamadas de API externas** (`ApiCallLog`): endpoint dedicado para registrar
   chamadas HTTP de saída feitas por outros serviços, com contexto completo (request/response
   body, status code, duração, erro) para diagnosticar falhas de integração.
8. **Suporte a múltiplos bancos de dados**: `DatabaseProvider`
   (`"MariaDB" | "SqlServer" | "Postgres"`) selecionado via `appsettings.json`, cada um com sua
   connection string. Abstraia as diferenças de tipo de coluna via `IDbColumnTypes`
   (BigInt/SmallInt/IntUnsigned/LongText/VarChar/NowSql) injetado nos mappings EF Core, e a
   diferença de full-text via `IFullTextSearchProvider`. Migrations em pastas separadas por
   provider.
9. **Autenticação JWT Bearer** obrigatória em todos os endpoints (`.RequireAuthorization()` em
   cada grupo). Dois modos por ambiente: OIDC (Production — valida via `Authority`/discovery)
   ou chave simétrica HS256 local (Development/Staging — via `Auth:Secret`). `Auth:Secret` deve
   ser proibido em Production (fail-fast); se nem `Authority` nem `Secret` estiverem
   configurados, a API não deve subir.
10. **HTTP logging**: registrar no output da aplicação método, path, query string, status code
    e duração de cada requisição recebida.
11. **CORS**: `AllowAnyOrigin` em Development; origens explícitas configuráveis via
    `Cors:AllowedOrigins` em `appsettings.Production.json`.
12. **Swagger**: habilitado em Development e Staging, com suporte a colar um Bearer token
    (botão Authorize).
13. **Auditoria de processos**: `GET /log-process/audit` e `GET /log-process/audit/{id}`
    retornam os processos com o status agregado (`GetStatus()`) e a lista de details.

## Convenções

### Rotas
- `/logs` — logs gerais
- `/log-process` — processos e detalhes
- `/log-process/audit` — auditoria com status agregado
- `/api-call-log` — log de chamadas HTTP de saída
- Cada grupo de endpoints em um arquivo próprio dentro de `Endpoints/`.

### Query params (nomes curtos)
`pn`=pageNumber, `ps`=pageSize, `ds`=dateStart, `de`=dateEnd, `ll`=logLevel, `msg`=message,
`st`=stackTrace, `q`=full-text, `cid`=correlationId, `tid`=traceId, `ok`=isSuccess,
`sc`=statusCode, `url`=URL contains (exclusivos de `/api-call-log/search`). Nota: em
`/log-process/audit`, `st` é reaproveitado com semântica diferente — filtra por
`ProcessStatus` (int), não por stackTrace; mantenha essa ambiguidade como está, é intencional
por escopo (parâmetros nunca coexistem no mesmo endpoint).

### Banco de dados (notação húngara)
Prefixos de coluna: `id_` (PK/FK), `ds_` (texto), `dt_` (data/hora), `nr_` (número/métrica),
`ie_` (enum), `fl_` (flag/bool), `vl_` (monetário), `qt_` (quantidade). Prefixos de objeto:
`TB_` (tabela), `VW_` (view), `idx_`/`ft_`/`uk_`/`fk_` (índices), `sp_`/`fn_`
(procedures/functions).

### Commits
Conventional Commits: `feat:`, `fix:`, `chore:`, `refactor:`, `docs:`.

## Docker

Um `docker-compose.<provider>.yml` por banco (MariaDB/SqlServer/Postgres — cada um com o
serviço de banco + build da API) combinado via `-f` com um `docker-compose.<ambiente>.yml`
(dev/staging/prod — cada um só com porta/env/restart policy da API). Credenciais em
`docker/.env` (gitignored), com `.env.example` versionado como template. Migrations aplicadas
automaticamente no startup (`Database.Migrate()`); seed de dados de teste só em Development.

## Testes

Projeto de testes separado, xUnit:
- **Repositório/domínio** contra EF Core InMemory, sem subir a API — nomenclatura
  `Metodo_Cenario_ResultadoEsperado`, `// Arrange`/`// Act`/`// Assert`,
  `#region Success`/`#region Error`, `Assert.*` puro (sem Moq/FluentAssertions).
- **Integração HTTP** via `WebApplicationFactory<Program>` (exige `public partial class
  Program {}` no `Program.cs` e um guard para pular `Database.Migrate()`/seed em ambiente
  `Testing`, já que o provider InMemory não suporta migrations). Substitua o `DbContext` por
  InMemory via `ConfigureTestServices` — lembre de remover tanto `DbContextOptions<TContext>`
  quanto `IDbContextOptionsConfiguration<TContext>` (o EF Core 9+ registra ambos; remover só o
  primeiro causa `InvalidOperationException` de múltiplos providers). Use `UseSetting(...)` (não
  `ConfigureAppConfiguration`/`AddInMemoryCollection`) para sobrescrever chaves que já têm valor
  não-vazio no `appsettings.json` base (ex. `Auth:Authority`), pois `UseSetting` garante
  precedência sobre as demais fontes de configuração. Gere um JWT HS256 local para autenticar
  as requisições de teste, e um helper de polling para aguardar a persistência assíncrona antes
  de assertar via GET. Isole dados entre testes que compartilham a mesma instância InMemory
  usando um `X-Tenant-Id` (`Guid`) próprio por teste.
