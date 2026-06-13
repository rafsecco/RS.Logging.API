# RS.Logging

Projeto de minimal API
Uma API para fazer o log geral e log de processos

Objetivo: entender problemas técnicos.

Exemplos:

exceções
erros HTTP
tempo de resposta
uso de endpoints
falhas externas
métricas
rastreamento entre serviços

Recursos que agregariam muito valor
- Multi-tenant
- CorrelationId
- TraceId
- Busca full-text
- Batch ingestion
- Compressão
- Retention por aplicação
- Webhook
- Dashboard
- Export para Elastic/OpenSearch
- Dead-letter queue

E um detalhe que costuma fazer diferença: não registrar log diretamente no banco da API principal. Use fila/memória/background para não transformar erro de log em indisponibilidade do sistema.

## Estrutura

```
src/
  BackEnd/
    RS.Core/                  # BaseEntity, PagedModel
    RS.Logging.Domain/        # Entidades, interfaces de repositório
    RS.Logging.Infra/         # EF Core, repositórios, migrations
    RS.Logging.API/           # Endpoints, DI, Swagger, CORS
docker/
  docker-compose.yml          # MariaDB + API
  mariadb/db.env              # Credenciais do banco
postman/                      # Coleção para testes manuais
```

## Comandos

### Backend
```bash
# Subir banco de dados
docker compose -f docker/docker-compose.yml up maria-db -d

# Rodar API (Development)
dotnet run --project src/BackEnd/RS.Logging.API

# Aplicar migrations
dotnet ef database update --project src/BackEnd/RS.Logging.Infra --startup-project src/BackEnd/RS.Logging.API

# Criar nova migration
dotnet ef migrations add <NomeMigration> --project src/BackEnd/RS.Logging.Infra --startup-project src/BackEnd/RS.Logging.API
```

### Docker (stack completa)
```bash
docker compose -f docker/docker-compose.yml up --build
# API: http://localhost:5000
# Swagger: http://localhost:5000/swagger (apenas Development)
```

## Arquitetura

Camadas no backend (dependency flow: API → Domain ← Infra):
- **Domain** define contratos (`ILogRepository`, `ILogProcessRepository`) — sem dependência de infraestrutura
- **Infra** implementa os contratos com EF Core + Pomelo
- **API** injeta via DI usando extension methods em `Configurations/`

Cada configuração fica em seu próprio arquivo:
- `ApiConfig.cs` — JSON options, controllers, middleware pipeline
- `DbContextConfig.cs` — connection string, retry policy (3x / 6s)
- `CorsConfig.cs` — AllowAnyOrigin em Development, localhost:7000 em Production
- `SwaggerConfig.cs` — habilitado apenas em Development

## Convenções

### Commits
Seguir Conventional Commits:

```
feat:     nova funcionalidade
fix:      correção de bug
chore:    tarefas de manutenção / config
refactor: refatoração sem mudança de comportamento
docs:     documentação
```

### Parâmetros de query (API)
Nomes curtos por convenção:
- `pn` = pageNumber, `ps` = pageSize
- `ds` = dateStart, `de` = dateEnd
- `ll` = logLevel, `msg` = message, `st` = stackTrace

### Entidades
IDs: `ulong` (BIGINT UNSIGNED auto-increment). Sempre herdar de `BaseEntity` (Id + CreatedAt).

## Pendências

- Autenticação/autorização (scaffold comentado em `ApiConfig.cs`)
- Testes automatizados (infraestrutura existe, não implementada)
- CORS de produção hardcoded em `localhost:7000` — externalizar para `appsettings.Production.json`
