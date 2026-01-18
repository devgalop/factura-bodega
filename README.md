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

- .Net 9
- PostgreSQL
- MongoDB
- Docker

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
