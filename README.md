# MicroserviciosDemo
Proyecto .NET 8 con microservicios y Docker
=======
# MicroserviciosDemo (.NET 8 + Docker)

Este proyecto es una demostración de arquitectura de **microservicios** utilizando **.NET 8** y **Docker**.  
Incluye dos servicios independientes:

- **CatalogService**: expone un API REST para consultar productos.
- **OrderService**: expone un API REST para gestionar pedidos.

---

## 🏗️ Arquitectura

El proyecto sigue una arquitectura de microservicios simple:

+-------------------+
|   API Gateway     |   (futuro: Ocelot)
+---------+---------+
|
-----------------------------------------
|                                       |
+-------------------+                 +-------------------+
|   CatalogService  |                 |   OrderService    |
|  (Productos)      |                 |  (Pedidos)        |
+---------+---------+                 +---------+---------+
|                                     |
|                                     |
http://localhost:5001/api/products    http://localhost:5002/api/orders

Código

Cada servicio es independiente, tiene su propio `Dockerfile` y se orquesta con `docker-compose`.

---

## 🚀 Tecnologías utilizadas
- .NET 8 (Web API)
- Docker y Docker Compose
- Arquitectura de microservicios
- GitHub para control de versiones

---

## 📂 Estructura del proyecto
MicroserviciosDemo/
├── CatalogService/
│    ├── Controllers/
│    │    └── ProductsController.cs
│    ├── Dockerfile
│    └── CatalogService.csproj
├── OrderService/
│    ├── Controllers/
│    │    └── OrdersController.cs
│    ├── Dockerfile
│    └── OrderService.csproj
└── docker-compose.yml

Código

---

## ▶️ Cómo ejecutar

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/magaliamedina/MicroserviciosDemo.git
   cd MicroserviciosDemo
Levantar los servicios con Docker Compose:

bash
docker-compose up --build
Acceder a los endpoints:

Catálogo: http://localhost:5001/api/products

Órdenes: http://localhost:5002/api/orders
