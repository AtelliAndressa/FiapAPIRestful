# Desafio API RESTful - Secretaria FIAP

Esta é uma API RESTful desenvolvida em .NET 8 como solução para o Desafio .NET da FIAP. A API gerencia alunos, turmas e matrículas, implementando autenticação JWT e seguindo padrões de arquitetura DDD e Clean Code.

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

Siga os passos abaixo para executar a aplicação localmente.

### 1. Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
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

### 3. Executando as Migrações

Com a string de conexão configurada, aplique as migrações para criar o banco de dados e todas as tabelas (incluindo o índice único do CPF).

Abra um terminal na pasta raiz do projeto (onde está o `.sln`) e execute:

```bash
dotnet ef database update --startup-project src/API
