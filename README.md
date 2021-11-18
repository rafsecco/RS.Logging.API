# RS.Log.API
Uma API para fazer o log dos seus projetos

## Descrição e configuração do docker para testar o projeto
O script irá criar uma imagem e um container para o banco de dados MariaDB.
Contendo 2 databases e usuários para os projetos A e B.
Inserindo alguns dados para testes.

No prompt de comando navegue até a pasta ".\SQL" do projeto e execute os comandos docker abaixo:
```
docker build --tag mariadb_multitenant-image .
docker container run --name RS.Log.DB -p 3306:3306 -e bind-address=0.0.0.0 -d mariadb_multitenant-image
```

## Adicionando um novo Database
Para adicionar um novo Database navegue para pasta ".\src\RS.Log.API" e execute o comando abaixo:
```
dotnet ef database update --connection "Server = localhost; Port = 3306; Database = [_NomeDataBase_]; Uid = root; Pwd = MyDB@123;"
```

Depois para criar o usuario para ter acesso apenas a este database

```
docker exec -ti RS.Log.DB bash

mysql --user=root --password=MyDB@123

CREATE USER 'ProjectC'@localhost IDENTIFIED BY 'ProjectC@123';
GRANT USAGE ON *.* TO 'ProjectC'@'%' IDENTIFIED BY 'ProjectC@123';
GRANT ALL privileges ON `RS.Log.ProjectC`.* TO 'ProjectC'@'%';
FLUSH PRIVILEGES;
```

Ai adicione essa nova conexão no arquivo appsettings


<!-- Docker
	docker-compose -p rs_log up -d
 -->


<!-- Configuracao do EntityFramework Core
	Comandos para criar o banco de dados 
	Ir para a pasta raiz do projeto \src\RS.Log.API\
	dotnet ef migrations add CreateDB -o .\Database\Migrations\ 
	dotnet ef migrations script -o ..\..\sql\TenantDataBase.sql
	dotnet ef database update --connection "Server = localhost; Port = 3306; Database = RS.Log.ProjectA; Uid = root; Pwd = MyDB@123;"
	dotnet ef migrations remove
-->


<!-- Configuração do COLLATION
	SQL Server: SQL_Latin1_General_CP1_CI_AI
		SQL_Latin1_General: Regras basicas de agrupamento utilizada pelo windows
		CP1:	Windows 12-5-2  codificacao ANSI
		CI:		Nao diferencia maiúsculas e minúsculas	| CS: Diferencia maiúsculas e minúsculas
		AI:		Não diferencia Acentuação				| AS: Diferencia Acentuação

	MySql: latin1_swedish_ci 
		Assim a primeira opção (com distinção) é utf8_swedish_ci,
		ALTER DATABASE `sua_base` CHARSET = Latin1 COLLATE = utf8_swedish_ci;
		e a segunda (sem distinção) utf8_general_ci. 
-->


<!-- Links Úteis
	https://docs.microsoft.com/pt-br/sql/relational-databases/collations/collation-and-unicode-support?view=sql-server-ver15#Collation_Defn
 -->

 <!-- MARKDOWN
	Cabeçalhos	=>	# = h1 | ## = h2 | ### = h3 ...
	Negrito		=>	**texto** ou __texto__
	Italico		=>	*texto* ou _texto_
	Listas
		Não Ordenadas	=>	* texto
		Ordenadas		=>	1. texto
								1. texto
								2. texto
							2. texto
	Imagens		=>	![Texto Alt](link imagem)
	Links		=>	[Texto do link](link)
	Codigo Fonte=>	``` codigo ```
	Task List	=>	- [x] texto	|	- [] texto
-->