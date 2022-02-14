# RS.Logging
Uma API para fazer o log dos seus projetos

## Descrição e configuração do docker para testar o projeto
O script irá criar dois containers, um para o banco de dados (mariadb) e outro para a API (C#)

No prompt de comando navegue até a pasta ".\docker" do projeto e execute os comandos abaixo:
```
docker-compose -p rs_log_solution build
docker-compose -p rs_log_solution up -d
```

Para testar importe a collection do postman disponibilizada

## Futuras implementações
- [ ] Criar o projeto de testes
- [ ] Refatorar
- [ ] Segurança
- [ ] Multi-Tenant
