# RS.Logging
Projeto de estudo de minimal API
Uma API para fazer o log geral e log de processos

## Descrição e configuração do docker para testar o projeto
O script irá criar dois containers, um para o banco de dados (mariadb) e outro para a API (C#)

No prompt de comando navegue até a pasta ".\docker" do projeto e execute os comandos abaixo:
```
docker-compose -p rs_logging build
docker-compose -p rs_logging up -d
```

Para testar importe a collection do postman disponibilizada

## Padronização dos commits

Seguir o padrão de [Conventional Commits](https://www.conventionalcommits.org/):
```
feat:     nova funcionalidade
fix:      correção de bug
chore:    tarefas de manutenção / config
refactor: refatoração sem mudança de comportamento
docs:     documentação
```

## Futuras implementações
- [X] Adicionar log geral
- [X] Adicionar log de processos
- [X] Criar o projeto de testes
- [ ] Refatorar
- [ ] Segurança
- [ ] Multi-Tenant
