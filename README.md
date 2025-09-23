# Desafio API RESTful - Secretaria FIAP

Esta é uma API RESTful desenvolvida em .NET 9 como solução para o Desafio .NET da FIAP. A API gerencia alunos, turmas e matrículas, implementando autenticação JWT e seguindo padrões de arquitetura DDD e Clean Code.

## Requisitos Atendidos

Este projeto atende a todos os requisitos funcionais (RF) e não funcionais (RNF) solicitados:
- CRUD completo para Alunos, Turmas e Matricula.
- Sistema de Matrícula com validação de duplicidade (RF05) e visualização de alunos por turma.
- Autenticação JWT com Política de Acesso **exclusiva para Administradores** (atendendo 100% ao RF10).
- Listagens paginadas (10 por padrão) e ordenadas alfabeticamente (RF01).
- Contagem de alunos por turma na listagem (RF02).
- Busca de alunos por nome (RF09).
- Validações robustas (RF03, RF04, RF06, RF07), incluindo CPF/Email únicos, senhas fortes e regras de negócio.
- Senhas armazenadas com hash seguro (RF08).
- Documentação Swagger (RNF03)  e uso correto de verbos HTTP (RNF01).
- Modelagem de banco com chaves (GUID) e restrições de índice único (RNF02).

## Tecnologias Utilizadas

- **Framework**: .NET 9 (atendendo ao requisito .NET Core >= 6) 
- **Arquitetura**: Abordagem baseada em DDD (Domain-Driven Design)
- **Persistência**: Entity Framework Core 9
- **Banco de Dados**: SQL Server
- **Autenticação**: ASP.NET Core Identity + JWT Bearer
- **Validação**: FluentValidation
- **Testes (Bônus BN01)**: xUnit, Moq, FluentAssertions 
- **Padrões**: Repository Pattern, Service Pattern, Injeção de Dependência, Clean Code.

---

## Como Instalar e Executar

Existem duas maneiras de executar este projeto. A maneira com Docker é a mais recomendada por ser mais simples.

### Opção 1: Executando com Docker (Recomendado)

Esta abordagem inicia a API e um banco de dados SQL Server com um único comando, conforme sugerido no desafio.

#### Pré-requisitos
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e em execução.

#### Configuração
1.  Clone este repositório.
2.  Abra o arquivo `docker-compose.yml` na raiz do projeto.
3.  Defina uma senha forte para o banco de dados em `SA_PASSWORD` no serviço `sql-server-fiap`.
4.  Coloque a **mesma senha** na `ConnectionStrings__DefaultConnection` do serviço `api-fiap`.
5.  Defina uma `Jwt__Key` segura no mesmo local.

#### Execução
Abra um terminal na raiz do projeto e execute o comando único:

```bash
docker compose up --build



O Docker irá construir a imagem da API, iniciar os contêineres, aplicar as migrações e criar o usuário Admin automaticamente.
Siga os passos abaixo para executar a aplicação localmente.



### Opção 2: Executando localmente

### 1. Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server LocalDB (geralmente instalado com o Visual Studio 2022).

### 2. Configuração da Conexão

1.  Clone este repositório.
2.  Abra o arquivo `src/API/appsettings.json`.
3.  Garanta que a seção `ConnectionStrings` esteja configurada para usar a Autenticação do Windows (Trusted_Connection), que funcionará em qualquer máquina com LocalDB:

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=FiapApiRestifulDB;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True"
    },
    ```

4.  Na mesma seção, configure o `Jwt`. O `"Key"` deve ser preenchido com uma chave secreta segura (para um teste real, a chave deve ser fornecida via User Secrets ou Variáveis de Ambiente, mas para este desafio, informe uma chave longa aqui ou no User Secrets).
5.  obs: Minha chave foi inserida no User Secrets.

    ```json
    "Jwt": {
      "Key": "",
      "Issuer": "https://localhost:7096",
      "Audience": "https://localhost:7096"
    },
    ```



### Execução:

### Aplique as Migrações:

Abra um terminal na pasta raiz do projeto e execute:

```bash
dotnet ef database update --startup-project src/API


### Rode a Aplicação:


```bash
dotnet run --project src/API


### Acessando a API (Para Ambas as Opções)

Usuário Administrador Inicial:

Na primeira vez que a aplicação rodar, o SeedData criará o usuário administrador padrão:

Usuário: admin@exemplo.com

Senha: Admin@123


### Documentação (Swagger):

Se rodando via Docker: acesse http://localhost:8080/swagger

Se rodando localmente: acesse https://localhost:7096/swagger



### Para testar os endpoints protegidos:

Use o endpoint POST /api/auth/login com o usuário admin.

Copie o token JWT da resposta.

Clique no botão "Authorize" no topo do Swagger e cole o token no formato Bearer SEU_TOKEN_AQUI.



### Testando a Aplicação:

Para rodar a suíte completa de testes unitários, execute o seguinte comando na pasta raiz do projeto:


```bash
dotnet test