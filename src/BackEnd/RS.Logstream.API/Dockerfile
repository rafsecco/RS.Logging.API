FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["RS.Logstream.API/RS.Logstream.API.csproj",      "RS.Logstream.API/"]
COPY ["RS.Core/RS.Core.csproj",                         "RS.Core/"]
COPY ["RS.Logstream.Domain/RS.Logstream.Domain.csproj", "RS.Logstream.Domain/"]
COPY ["RS.Logstream.Infra/RS.Logstream.Infra.csproj",   "RS.Logstream.Infra/"]
RUN dotnet restore "RS.Logstream.API/RS.Logstream.API.csproj"
COPY . .
RUN dotnet publish "RS.Logstream.API/RS.Logstream.API.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RS.Logstream.API.dll"]
