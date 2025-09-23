# Estágio 1: Build (usando o SDK completo)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia todos os arquivos .csproj e o .sln
# Esta forma é mais robusta para encontrar todos os projetos
COPY *.sln .
COPY src/API/*.csproj src/API/
COPY src/Core.Application/*.csproj src/Core.Application/
COPY src/Core.Domain/*.csproj src/Core.Domain/
COPY src/Infrastructure/*.csproj src/Infrastructure/
# Nós ainda não copiamos os testes, e tudo bem.

# Restaura as dependências APENAS do projeto de API
# Isso restaura a API e os projetos dos quais ela depende (Application, Domain, etc.)
# mas IGNORA os projetos de teste, corrigindo o seu erro.
RUN dotnet restore "src/API/API.csproj"

# Copia todo o resto do código-fonte
COPY . .

# Publica a aplicação
WORKDIR "/src/src/API"
RUN dotnet publish "API.csproj" -c Release -o /app/publish

# Estágio 2: Final (usando o runtime leve)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "API.dll"]