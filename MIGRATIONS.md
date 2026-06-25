# Migrations

Comandos do EF Core para gerenciar as migrations do projeto. As migrations residem em
`src/BackEnd/RS.Logstream.Infra/Migrations` (projeto `RS.Logstream.Infra`, startup project `RS.Logstream`).

> Pré-requisito: ferramenta `dotnet-ef` instalada (`dotnet tool install --global dotnet-ef` ou
> `dotnet tool update --global dotnet-ef`).

## Criar uma nova migration

```bash
dotnet ef migrations add <NomeMigration> --project src/BackEnd/RS.Logstream.Infra --startup-project src/BackEnd/RS.Logstream.API --output-dir Migrations
```

## Aplicar migrations no banco

```bash
dotnet ef database update --project src/BackEnd/RS.Logstream.Infra --startup-project src/BackEnd/RS.Logstream.API
```

Aplicar até uma migration específica (útil para reverter parcialmente):

```bash
dotnet ef database update <NomeMigration> --project src/BackEnd/RS.Logstream.Infra --startup-project src/BackEnd/RS.Logstream.API
```

Reverter todas as migrations (volta ao banco vazio):

```bash
dotnet ef database update 0 --project src/BackEnd/RS.Logstream.Infra --startup-project src/BackEnd/RS.Logstream.API
```

## Remover a última migration

Remove a última migration criada (somente se ainda não aplicada ao banco):

```bash
dotnet ef migrations remove --project src/BackEnd/RS.Logstream.Infra --startup-project src/BackEnd/RS.Logstream.API
```

## Listar migrations

```bash
dotnet ef migrations list --project src/BackEnd/RS.Logstream.Infra --startup-project src/BackEnd/RS.Logstream.API
```

## Gerar script SQL

Gera o SQL de todas as migrations pendentes (idempotente, seguro para rodar em produção):

```bash
dotnet ef migrations script --project src/BackEnd/RS.Logstream.Infra --startup-project src/BackEnd/RS.Logstream.API --idempotent --output migration.sql
```

Gerar script entre duas migrations específicas:

```bash
dotnet ef migrations script <MigrationOrigem> <MigrationDestino> --project src/BackEnd/RS.Logstream.Infra --startup-project src/BackEnd/RS.Logstream.API --output migration.sql
```

## Subir o banco antes de aplicar migrations

```bash
docker compose -f docker/docker-compose.mariadb.yml up maria-db -d
```
