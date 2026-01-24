FROM mcr.microsoft.com/dotnet/aspnet:9.0.12-alpine3.22-amd64 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0.310-alpine3.22-amd64 AS build
WORKDIR /src

# Copia solo el archivo del proyecto a la ruta de trabajo
COPY src/devgalop.facturabodega.webapi/devgalop.facturabodega.webapi.csproj ./

# Restaura las dependencias del proyecto
RUN dotnet restore "./devgalop.facturabodega.webapi.csproj"

# Copia el resto del código fuente
COPY src/devgalop.facturabodega.webapi/ .

# Establece el directorio de trabajo
WORKDIR /src/.

# Compila el proyecto en modo Release
RUN dotnet build "devgalop.facturabodega.webapi.csproj" -c Release -o /app/build

FROM build AS publish

# Publica la aplicación
RUN dotnet publish "devgalop.facturabodega.webapi.csproj" -c Release -o /app/publish


FROM base AS final
# Establece el directorio de trabajo /app
WORKDIR /app
# Copia los archivos publicados desde la etapa de publicación
COPY --from=publish /app/publish .

# Define el punto de entrada para la aplicación
ENTRYPOINT ["dotnet", "devgalop.facturabodega.webapi.dll"]