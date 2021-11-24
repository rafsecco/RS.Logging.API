# RS.Log.API
Uma API para fazer o log dos seus projetos

## Descrição e configuração do docker para testar o projeto
O script irá criar dois containers, um para o banco de dados (mariadb) e outro para a API (C#)

No prompt de comando navegue até a pasta ".\docker" do projeto e execute os comandos abaixo:
```
docker-compose -p rs_log_solution build
docker-compose -p rs_log_solution up -d
```

Para testar digite o endereço abaixo no seu browser ou importe a collection do postman disponibilizada
http://localhost:5000/swagger/

## Futuras implementações
[] Criar o projeto de testes
[] Refatorar
[] Segurança
