#FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./RS.Logging.API/RS.Logging.API.csproj", "src/RS.Logging.API/"]
COPY ["./RS.Core/RS.Core.csproj", "src/RS.Core/"]
RUN dotnet restore "src/RS.Logging.API/RS.Logging.API.csproj"
COPY . .
WORKDIR "/src/RS.Logging.API"
RUN dotnet build "RS.Logging.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RS.Logging.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RS.Logging.API.dll"]
