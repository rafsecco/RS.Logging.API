FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./RS.Logstream.API/RS.Logstream.API.csproj", "src/RS.Logstream.API/"]
COPY ["./RS.Core/RS.Core.csproj", "src/RS.Core/"]
COPY ["./RS.Logstream.Domain/RS.Logstream.Domain.csproj", "src/RS.Logstream.Domain/"]
RUN dotnet restore "src/RS.Logstream.API/RS.Logstream.API.csproj"
COPY . .
WORKDIR "/src/RS.Logstream"
RUN dotnet build "RS.Logstream.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RS.Logstream.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RS.Logstream.API.dll"]
