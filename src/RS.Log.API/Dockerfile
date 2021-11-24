FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["./RS.Log.API/RS.Log.API.csproj", "RS.Log.API/"]
RUN dotnet restore "RS.Log.API/RS.Log.API.csproj"
COPY . .
WORKDIR "RS.Log.API"
RUN dotnet build "RS.Log.API.csproj" -c Development -o /app/build
#RUN dotnet build "RS.Log.API.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "RS.Log.API.csproj" -c Development -o /app/publish
#RUN dotnet publish "RS.Log.API.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RS.Log.API.dll"]
