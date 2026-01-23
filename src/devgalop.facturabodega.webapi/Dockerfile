FROM mcr.microsoft.com/dotnet/aspnet:9.0.12-alpine3.22-amd64 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0.310-alpine3.22-amd64 AS build
WORKDIR /src

COPY . /src/

RUN dotnet restore "./devgalop.facturabodega.webapi.csproj"

COPY . .
WORKDIR /src/.

RUN dotnet build "devgalop.facturabodega.webapi.csproj" -c Release -o /app/build

FROM build AS publish

RUN dotnet publish "devgalop.facturabodega.webapi.csproj" -c Release -o /app/publish
# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "devgalop.facturabodega.webapi.dll"]