# Sistema de Facturación y Gestión de bodegas

Este es un proyecto de estudio, que busca afianzar conceptos y tecnologías aprendidas. Se trata de un sistema básico de facturación, administración de usuarios, control y gestión de bodegas

---

## Arquitectura

El sistema está pensado para ser un **Monolito Modular**. El sistema se desarrolla en .NET con un estilo de *arquitectura vertical*.

### Contexto

![diagrama_contexto](/resources/C4_sistema_facturacion-Contexto.drawio.png)

### Contenedores

![diagrama_contenedores](/resources/C4_sistema_facturacion-Contenedores.drawio.png)

### Componentes

![diagrama_componentes](/resources/C4_sistema_facturacion-Componentes.drawio.png)

---

## Tecnologías

Las tecnologías aplicadas en este proyecto son:

- [.Net 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Entity Framework
- PostgreSQL
- MongoDB
- [Docker](https://www.docker.com/get-started/)

### ¿Cómo instalar PostgreSQL utilizando docker?

Para instalar una imagen de Postgres con la ayuda de *Docker* es necesario seguir los siguientes pasos:

- Se debe hacer pull a la imagen de postgres con la ayuda del siguiente comando

```bash
#Si desea la versión latest 
docker pull postgres

#Si desea instalar cualquier otra versión modifique VERSION_TAG por la versión elegida
docker pull postgres:VERSION_TAG
```

- Una vez finalice la descarga de la imagen, puede verificar el estado de la imagen con la ayuda del siguiente comando:

```bash
docker images
```

- Para ejecutar la base de datos deberá correr el comando:

```bash
# Este comando ejecuta la imagen de postgres previamente instalada en docker
# --name: Esta parámetro permite darle un nombre (tag) al contenedor
# -e POSTGRES_PASSWORD: Este parámetro crea la contraseña para la base de datos
# -e POSTGRES_USER: Este parámetro crea el usuario para la base de datos
# -e POSTGRES_DB: Este parámetro crea la base de datos
# -p: son los puertos por los que se expone la base de datos PuertoHost:PuertoContenedor
# -v: crea el volumen para almacenar los datos localmente
# -d: ejecución en modo detach de la imagen

docker run --name postgres-db \
-e POSTGRES_PASSWORD=mysecretpassword \
-e POSTGRES_USER=myuser \
-e POSTGRES_DB=myowndatabase \
-p 5432:5432 \
-v postgres-data:/var/lib/postgresql/data \
-d postgres
```

- Una vez esté corriendo el contenedor, puede realizar la validación con el comando:

```bash
docker ps 
```

- Para interactuar con la base de datos en SQL Shell puede utilizar:

```bash
docker exec -it postgres-db psql -U myuser -d myowndatabase
```

- Si prefiere hacer uso de un GUI para interactuar con la base de datos, puede utilizar **pgAdmin**. Para instalar la imagen debe correr el siguiente comando:

```bash
docker pull dpage/pgadmin4
```

- Una vez instalada la imagen, puede ejecutar un contenedor de la siguiente manera:

```bash
# Crea un contenedor usando la imagen dpage/pgAdmin4
# --name: Este parámetro le asigna un nombre al contenedor
# -p: Este parámetro mapea los puertos por los que se va a exponer la aplicacion PuertoLocal:PuertoContenedor
# -e PGADMIN_DEFAULT_EMAIL: correo con el cual se realiza el login
# -e PGADMIN_DEFAULT_PASSWORD: contraseña para el acceso a pgAdmin
# -d: ejecución en modo detach

docker run --name pgadmin -p 5050:80 -e PGADMIN_DEFAULT_EMAIL=user@domain.com -e PGADMIN_DEFAULT_PASSWORD=mysecretpassword -d dpage/pgadmin4
```

- Cuando este corriendo el contenedor, podrá acceder a *localhost:5050* para tener el control de la base de datos.

### ¿Cómo instalar EntityFramework CLI?

- Lo primero que debes hacer es validar si tienes instaldo el componente de CLI para Entity Framework

```bash
dotnet ef
```

- Si la ejecución de ese comando te genera algún tipo de error, debes ejecutar lo siguiente:

```bash
# Si quieres instalar de forma global para todos los proyectos (Recomendado)
dotnet tool install --global dotnet-ef

# Si quieres solo instalar la herramienta en el proyecto local
dotnet tool install dotnet-ef
```

---

## Objetivos

### General

El objetivo de este proyecto es aplicar los conceptos aprendidos en temas como:

- API Security
- API Documentation
- Unit Test
- Vertical Slice Architecture
- PostgreSQL
- MongoDB
- Manejo de contenedores

Todo esto utilizando las tecnologías mencionadas anteriormente.

### Especificos

Los objetivos especificos pautados para el desarrollo de este proyecto son:

- Crear un backend seguro utilizando JWT Tokens y autorización por roles.
- Generar APIs bien documentadas haciendo uso de OpenAPI y documentación interna del código.
- Aplicar pruebas unitarias y un pipeline de despliegue donde se ejecuten de manera automática con la ayuda de Github Actions.
- Fortalecer los conceptos aprendidos en arquitectura vertical aplicandolos en el desarrollo backend.
- Fortalecer conceptos aprendidos para bases de datos PostgreSQL y MongoDB mediante un buen diseño de bases de datos
