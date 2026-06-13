# RS.Logging

Projeto de minimal API (.NET 10) para fazer o log geral e log de processos de outras aplicações.

Objetivo: entender problemas técnicos (exceções, erros HTTP, tempo de resposta, uso de
endpoints, falhas externas, métricas e rastreamento entre serviços).

## Subindo o projeto com Docker

O `docker-compose.yml` cria dois containers: um para o banco de dados (MariaDB) e outro
para a API (.NET).

Na raiz do projeto, execute:
```bash
docker compose -f docker/docker-compose.yml up --build
```

- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger (apenas em Development)

Para testar os endpoints, importe a collection do Postman disponível em `postman/`.

## Padronização dos commits

Seguir o padrão de [Conventional Commits](https://www.conventionalcommits.org/):
```
feat:     nova funcionalidade
fix:      correção de bug
chore:    tarefas de manutenção / config
refactor: refatoração sem mudança de comportamento
docs:     documentação
```

## Roadmap

- [X] Log geral
- [X] Log de processos
- [X] Projeto de testes
- [X] Auditoria de processos (status agregado por processo)
- [X] Upgrade para .NET 10
- [ ] Refatorar
- [ ] Segurança (autenticação/autorização)
- [ ] Ingestão assíncrona (fila/background) para não gravar log direto no banco da API
- [ ] CorrelationId / TraceId
- [ ] Multi-tenant
- [ ] Busca full-text
- [ ] Batch ingestion
- [ ] Compressão
- [ ] Retention por aplicação
- [ ] Webhook
- [ ] Dashboard
- [ ] Export para Elastic/OpenSearch
- [ ] Dead-letter queue
