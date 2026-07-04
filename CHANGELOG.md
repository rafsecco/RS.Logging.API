# Changelog

All notable changes to this project will be documented in this file. See [standard-version](https://github.com/conventional-changelog/standard-version) for commit guidelines.

## [Unreleased]

### Tests

- **Testes de integração HTTP** — nova suíte em `src/Tests/RS.Logstream.Tests/Integration/`
  usando `WebApplicationFactory<Program>` para exercitar os 4 grupos de endpoints
  (`/logs`, `/log-process`, `/log-process/audit`, `/api-call-log`) fim a fim: autenticação
  JWT (401 sem token), paginação, filtros de busca, e o fluxo assíncrono de ingestão
  (`202 Accepted` + polling até a gravação em background)
- `CustomWebApplicationFactory` roda em ambiente `Testing`, com chave JWT simétrica de teste
  e `RSLogstreamDbContext` substituído por provider `InMemory` — sem dependência de banco ou
  Docker reais
- `Program.cs` ganhou um marker `public partial class Program {}` e um guard para pular
  `Database.Migrate()`/seed em ambiente `Testing` (necessário para o `WebApplicationFactory`
  funcionar com o provider `InMemory`, que não suporta migrations)

## [2.0.0](https://github.com/rafsecco/RS.Logstream/compare/v1.0.3...v2.0.0) (2026-06-23)

### ⚠ Breaking Changes

- Todos os endpoints foram renomeados para seguir convenção REST:
  - `/GetAll/` → `GET /logs`
  - `/GetById/{id}` → `GET /logs/{id}`
  - `/Search/` → `GET /logs/search`
  - `/CreateLog/` → `POST /logs`
  - `/CreateLogBatch/` → `POST /logs/batch`
  - `/GetAllLogProcess/` → `GET /log-process`
  - `/GetLogProcessById/{id}/` → `GET /log-process/{id}`
  - `/LogProcessSearch/` → `GET /log-process/search`
  - `/CreateLogProcess/` → `POST /log-process`
  - `/CreateLogProcessDetail/` → `POST /log-process/detail`
  - `/CreateLogProcessDetailBatch/` → `POST /log-process/detail/batch`
  - `/LogProcessAudit/` → `GET /log-process/audit`
  - `/LogProcessAudit/{id}` → `GET /log-process/audit/{id}`

### Features

- **Autenticação JWT Bearer** — todos os endpoints exigem token emitido pela Auth API externa; validação local via JWKS/Authority sem chamada de rede por requisição
- **Multi-tenant** — identificação via header `X-Tenant-Id` (string livre, sem cadastro); filtro opt-in nos endpoints de leitura; retrocompatível (sem header = sem filtro)
- **CorrelationId / TraceId** — propagação via headers `X-Correlation-Id` e `X-Trace-Id` gravados em `Log`, `LogProcess`, `LogProcessDetail` e `ApiCallLog`; filtráveis via `cid`/`tid`
- **Batch ingestion** — `POST /logs/batch` e `POST /log-process/detail/batch` enfileiram múltiplos itens na fila de ingestão e retornam `202 Accepted`
- **Busca full-text** — parâmetro `q` em `/logs/search` e `/log-process/search`; FULLTEXT nativo no MariaDB, fallback `LIKE` no SQL Server e Postgres
- **Retention por tenant** — `LogRetentionWorker` expurga logs periodicamente com política por tenant via `appsettings.json` (`Retention:DefaultDays`, `Retention:PolicyByTenant`, `Retention:IntervalHours`)
- **Log de chamadas de API** — nova entidade `ApiCallLog` com endpoints `GET`/`POST /api-call-log`; registra URL, método, request/response body, status code, duração e erro de chamadas HTTP de saída
- **Suporte a múltiplos bancos** — `DatabaseProvider` em `appsettings.json` seleciona MariaDB, SQL Server ou Postgres; tipos de coluna e full-text abstraídos via `IDbColumnTypes` e `IFullTextSearchProvider`
- **HTTP logging** — `UseHttpLogging` registra método, path, status code e duração de cada requisição recebida
- **Ingestão assíncrona** — `LogIngestionQueue` (Channel in-memory) + `LogIngestionWorker` (BackgroundService) desacopla a escrita do banco da requisição HTTP

### Refactoring

- Endpoints divididos em arquivos separados por grupo (`Endpoints/LogEndpoints.cs`, `LogProcessEndpoints.cs`, `AuditEndpoints.cs`, `ApiCallLogEndpoints.cs`)
- Mappings EF Core recebem `IDbColumnTypes` via construtor — tipos de coluna resolvidos por provider sem condicional em runtime
- Migrations reorganizadas por provider (`Migrations/MariaDB/`, `Migrations/SqlServer/`, `Migrations/Postgres/`)
- Upgrade para .NET 10 + EF Core 9

### Tests

- 64 testes unitários (InMemory): `LogRepositoryTests`, `LogProcessRepositoryTests`, `ApiCallLogRepositoryTests`, `LogProcessGetStatusTests`
- Cobertura de filtros por tenant, correlationId, traceId, isSuccess, statusCode, URL substring e date range

### [1.0.3](https://github.com/rafsecco/RS.Logstream/compare/v1.0.2...v1.0.3) (2025-02-04)

### [1.0.2](https://github.com/rafsecco/RS.Logstream/compare/v1.0.1...v1.0.2) (2025-02-04)

### 1.0.1 (2025-02-04)
