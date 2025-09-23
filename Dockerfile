
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.sln .
COPY src/API/*.csproj src/API/
COPY src/Core.Application/*.csproj src/Core.Application/
COPY src/Core.Domain/*.csproj src/Core.Domain/
COPY src/Infrastructure/*.csproj src/Infrastructure/

RUN dotnet restore "src/API/API.csproj"

COPY . .

WORKDIR "/src/src/API"
RUN dotnet publish "API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "API.dll"]