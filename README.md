# Desafio API RESTful - Secretaria FIAP

[cite_start]Esta é uma API RESTful desenvolvida em .NET 8 como solução para o Desafio .NET da FIAP[cite: 1]. [cite_start]A API gerencia alunos, turmas e matrículas [cite: 4][cite_start], implementando autenticação JWT e seguindo padrões de arquitetura DDD e Clean Code[cite: 60].

## Requisitos Atendidos

Este projeto atende a todos os requisitos funcionais (RF) e não funcionais (RNF) solicitados:
- [cite_start]CRUD completo para Alunos e Turmas[cite: 9, 11].
- [cite_start]Sistema de Matrícula com validação de duplicidade (RF05) [cite: 32] [cite_start]e visualização de alunos por turma[cite: 13].
- [cite_start]Autenticação JWT com Roles e Políticas (Admin/User) (RF10)[cite: 44].
- [cite_start]Listagens paginadas (10 por padrão) e ordenadas alfabeticamente (RF01)[cite: 24].
- [cite_start]Contagem de alunos por turma na listagem (RF02)[cite: 26].
- [cite_start]Busca de alunos por nome (RF09)[cite: 42].
- [cite_start]Validações robustas (RF03, RF04, RF06, RF07), incluindo CPF/Email únicos, senhas fortes e regras de negócio[cite: 28, 30, 34, 37].
- [cite_start]Senhas armazenadas com hash seguro (RF08)[cite: 40].
- [cite_start]Documentação Swagger (RNF03)  [cite_start]e uso correto de verbos HTTP (RNF01)[cite: 48].

## Tecnologias Utilizadas

- [cite_start]**Framework**: .NET 8 [cite: 56]
- [cite_start]**Arquitetura**: Abordagem baseada em DDD (Domain, Application, Infrastructure, API) [cite: 60]
- [cite_start]**Persistência**: Entity Framework Core 8 [cite: 58]
- [cite_start]**Banco de Dados**: SQL Server [cite: 57]
- [cite_start]**Autenticação**: ASP.NET Core Identity + JWT Bearer [cite: 44]
- **Validação**: FluentValidation
- [cite_start]**Testes**: xUnit, Moq, FluentAssertions (BN01) 
- [cite_start]**Padrões**: Repository Pattern, Service Pattern, Injeção de Dependência, SRP[cite: 60].

---

## Como Instalar e Executar

Siga os passos abaixo para executar a aplicação localmente.

### 1. Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Um servidor SQL Server (Express, Developer ou LocalDB) acessível. O LocalDB é o padrão.

### 2. Configuração da Conexão

1.  Clone este repositório.
2.  Abra o arquivo `src/API/appsettings.json`.
3.  Garanta que a seção `ConnectionStrings` está configurada para seu banco. O código enviado utiliza a seguinte configuração para o LocalDB (usuário `sa`, senha `14112008`):

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=FiapApiRestifulDB;User ID=sa;Password=14112008;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True"
    },
    ```
    > **AVISO DE SEGURANÇA:** O arquivo acima contém uma senha. Em um projeto de produção, essa string de conexão deve ser gerenciada via User Secrets ou Azure Key Vault, e nunca deve ser enviada para um repositório Git.

4.  Na mesma seção, configure o `Jwt`. O `"Key"` deve ser preenchido com a chave secreta configurada no seu User Secrets (conforme fizemos durante o desenvolvimento).

    ```json
    "Jwt": {
      "Key": "COLOQUE_AQUI_A_SUA_CHAVE_SECRETA_DO_USER_SECRETS",
      "Issuer": "https://localhost:7096",
      "Audience": "https://localhost:7096"
    },
    ```

### 3. Executando as Migrações

Com a string de conexão configurada, aplique as migrações para criar o banco de dados e todas as tabelas (incluindo o índice único do CPF).

Abra um terminal na pasta raiz do projeto (onde está o `.sln`) e execute:

```bash
dotnet ef database update --startup-project src/API


### 4. Executando a Aplicação

Após a criação do banco, rode a API:

```bash
dotnet run --project src/API

A aplicação será iniciada e o terminal mostrará os endereços de escuta, incluindo https://localhost:7096.

Usuário Administrador Inicial:
Na primeira vez que a aplicação rodar, ela executará o SeedData automaticamente, criando as roles "Admin" e "User" e o usuário administrador padrão:

Usuário: admin@exemplo.com

Senha: Admin@123

### 5. Documentação (Swagger)

A API é iniciada automaticamente com o Swagger. Acesse a URL principal da aplicação (definida no 

launchSettings.json) para ver a documentação completa da API e testar os endpoints:

https://localhost:7096/swagger

Para testar endpoints protegidos:

Use o endpoint POST /api/auth/login com o usuário admin.

Copie o token JWT da resposta.

Clique no botão "Authorize" no topo do Swagger e cole o token no formato Bearer SEU_TOKEN_AQUI.

Testando a Aplicação
Para rodar a suíte completa de testes unitários (BN01), execute o seguinte comando na pasta raiz do projeto:

```bash

dotnet test