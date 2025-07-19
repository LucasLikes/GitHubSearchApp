
# GitHub Search App

Aplicação para busca de repositórios públicos no GitHub, marcação de favoritos e organização dos resultados por relevância.

---

## Descrição

Este projeto consiste em um backend ASP.NET Core que integra com a API pública do GitHub para pesquisar repositórios públicos, permitir que o usuário marque favoritos (em memória, sem persistência) e listar os repositórios ordenados por uma lógica de relevância personalizada.

---

## Funcionalidades

- Buscar repositórios públicos no GitHub por nome ou parte do nome.
- Favoritar e desfavoritar repositórios em tempo de execução (sem banco de dados).
- Listar repositórios favoritos.
- Ordenar os resultados pela relevância (estrelas, forks e watchers).
- API documentada com Swagger/OpenAPI.

---

## Estrutura do Projeto

- **API:** ASP.NET Core Web API com controllers REST.
- **Application:** Serviços e lógica de negócio.
- **Domain:** Entidades e interfaces do domínio.
- **Infrastructure:** Implementação da integração com a API do GitHub.

---

## Como executar

1. Clonar o repositório.
2. Restaurar pacotes NuGet (`dotnet restore`).
3. Executar o projeto (`dotnet run`).
4. Abrir o Swagger em `https://localhost:<porta>/swagger` para testar as APIs.

---

## Endpoints principais

- `GET /api/Repos/me?nome={nome}` - Buscar repositórios no GitHub pelo nome.
- `GET /api/Favoritos` - Listar repositórios favoritos.
- `POST /api/Favoritos` - Adicionar repositório aos favoritos.
- `DELETE /api/Favoritos/{id}` - Remover repositório dos favoritos.

---

## Lógica de relevância

A relevância dos repositórios é calculada pela fórmula:

```csharp
Estrelas * 2 + Forks + Watchers
```

Esta fórmula valoriza mais os repositórios com maior número de estrelas, seguida pelos forks e watchers, refletindo a popularidade e engajamento do projeto.

---

## Testes

O projeto inclui testes unitários para a lógica de relevância e serviços.

---

## Imagens

### Tela de Busca no Swagger
![Placeholder para print do Swagger](./images/swagger-screenshot.png)

### Exemplo de Resultado da Busca
![Placeholder para print do resultado](./images/busca-resultado.png)

### Frontend (Angular)
![Placeholder para print do frontend](./images/frontend-screenshot.png)

---

## Tecnologias usadas

- .NET 9
- ASP.NET Core Web API
- HttpClientFactory
- Swagger/OpenAPI
- Testes Unitários (xUnit)

---

## Próximos passos

- Persistência dos favoritos em banco de dados.
- Autenticação e autorização.
- Frontend mais completo e estilizado.
- AUTH 
- Logs e Monitoramento

---

## Autor

Lucas Gabriel Likes

