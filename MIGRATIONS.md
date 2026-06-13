# Migrations

Comandos do EF Core para gerenciar as migrations do projeto. As migrations residem em
`src/BackEnd/RS.Logging.Infra/Migrations` (projeto `RS.Logging.Infra`, startup project `RS.Logging.API`).

> Pré-requisito: ferramenta `dotnet-ef` instalada (`dotnet tool install --global dotnet-ef` ou
> `dotnet tool update --global dotnet-ef`).

## Criar uma nova migration

```bash
dotnet ef migrations add <NomeMigration> --project src/BackEnd/RS.Logging.Infra --startup-project src/BackEnd/RS.Logging.API --output-dir Migrations
```

## Aplicar migrations no banco

```bash
dotnet ef database update --project src/BackEnd/RS.Logging.Infra --startup-project src/BackEnd/RS.Logging.API
```

Aplicar até uma migration específica (útil para reverter parcialmente):

```bash
dotnet ef database update <NomeMigration> --project src/BackEnd/RS.Logging.Infra --startup-project src/BackEnd/RS.Logging.API
```

Reverter todas as migrations (volta ao banco vazio):

```bash
dotnet ef database update 0 --project src/BackEnd/RS.Logging.Infra --startup-project src/BackEnd/RS.Logging.API
```

## Remover a última migration

Remove a última migration criada (somente se ainda não aplicada ao banco):

```bash
dotnet ef migrations remove --project src/BackEnd/RS.Logging.Infra --startup-project src/BackEnd/RS.Logging.API
```

## Listar migrations

```bash
dotnet ef migrations list --project src/BackEnd/RS.Logging.Infra --startup-project src/BackEnd/RS.Logging.API
```

## Gerar script SQL

Gera o SQL de todas as migrations pendentes (idempotente, seguro para rodar em produção):

```bash
dotnet ef migrations script --project src/BackEnd/RS.Logging.Infra --startup-project src/BackEnd/RS.Logging.API --idempotent --output migration.sql
```

Gerar script entre duas migrations específicas:

```bash
dotnet ef migrations script <MigrationOrigem> <MigrationDestino> --project src/BackEnd/RS.Logging.Infra --startup-project src/BackEnd/RS.Logging.API --output migration.sql
```

## Subir o banco antes de aplicar migrations

```bash
docker compose -f docker/docker-compose.yml up maria-db -d
```
