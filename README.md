# RS.Log.API
Uma API para fazer o log dos seus projetos


<!-- Docker
	docker-compose -p rs_log up -d
 -->


<!-- Configuracao do EntityFramework Core
	Comandos para criar o banco de dados 
	Ir para a pasta raiz do projeto \src\RS.Log.API\
	dotnet ef migrations add CreateDB -o .\Database\Migrations\ 
	dotnet ef migrations script -o ..\..\sql\TenantDataBase.sql
	dotnet ef database update --connection "Server = localhost; Port = 3306; Database = RS.Log.Tenant-1; Uid = root; Pwd = MyDB@123;"
	dotnet ef database update --connection "Server = localhost; Port = 3306; Database = RS.Log.Tenant-2; Uid = root; Pwd = MyDB@123;"


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