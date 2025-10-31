# Censudex — Clients Service (microservice)

Servicio micro (REST) que forma parte de la plataforma Censudex y es consumido por la [**API Gateway**](https://github.com/Taller-2-Arq-de-Sistemas/censudex-api-gateway) y otros servicios (ej. [Auth](https://github.com/Taller-2-Arq-de-Sistemas/censudex-auth) y [Órdenes](https://github.com/Taller-2-Arq-de-Sistemas/censudex-orders)). Gestiona el CRUD básico de clientes: creación, lectura, actualización y desactivación (soft delete). Está implementado en **C# (.NET 9)** con **EF Core + Npgsql (PostgreSQL)** y sigue una separación sencilla tipo *Clean-ish* (Controllers → Services/Repositories → Data/EF).

---

# Arquitectura y patrones

* **Arquitectura general:** microservice REST, diseñado para ser pequeño, desacoplado y desplegable independientemente.
* **Capas / organización** (estructura del proyecto):

  * `Presentation` — Controllers / DTOs (API surface).
  * `Repositories` — Acceso a datos (EF Core) y contratos (`IClientRepository`).
  * `Data` — `ApplicationDBContext`, configuraciones EF, seeder y migraciones.
  * `Models` — Entidades/POCOs (Client).
  * `Mappers` / `Helpers` — Mapeo DTO ↔ Model, validaciones.
* **Patrón de diseño principal:** Controllers + Repository pattern.

  * Controllers reciben la petición, validan DTOs y delegan a repositorios.
  * Repositorio encapsula consultas EF / operaciones de persistencia.
* **Otros componentes relevantes:**

  * **DTOs** para entradas y salidas (CreateUserRequest, ViewUserResponse).
  * **Mapper** estático para convertir entre DTO y modelo.
  * **Seeder**: inserta datos iniciales *solo si la DB está vacía* (se ejecuta en startup).
  * **.env** con `DotNetEnv` para cargar variables de entorno en `Program.cs`.

---

# Requisitos previos

* .NET SDK (v9 compatible) instalado.
* PostgreSQL (local o remoto). Si usas Render / Railway: recuerda permitir IPs o ejecutar migraciones desde un entorno con acceso.
* `dotnet-ef` (CLI) — opcional pero recomendado:

  ```bash
  dotnet tool install --global dotnet-ef
  ```
* Paquetes del proyecto: `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design`, `DotNetEnv`, `BCrypt.Net-Next`.

---

# Pasos para ejecutar el proyecto (develop)

> **Copia y pega** los comandos en tu terminal (desde la raíz del proyecto).

1. **Clona / entra al proyecto**

   ```bash
   git clone https://github.com/Taller-2-Arq-de-Sistemas/censudex-clients
   cd censudex-clients
   ```

2. **Copiar ejemplo de .env**

   ```bash
   cp .env.example .env
   ```

   Luego edita `.env` y utiliza tu connection_string de tu base de datos en el formato a continuación.

   ```
   DB_CONNECTION_STRING=Host="yourdbhost";Database="yourdbname";Username="yourdbusername";Password="yourdbpassword";Port="yourdbport";
   ```

3. **Instala herramientas .NET Entity Framework**

   ```bash
   dotnet tool install --global dotnet-ef
   ```

4. **Restaurar paquetes NuGet**

   ```bash
   dotnet restore
   ```

5. **Inicia el proyecto:**
   Al correr el siguiente comando se aplican las migraciones y ocurre un `seeding` si la tabla `Clients` está vacía.

   ```bash
   dotnet run
   ```

6. **Probar endpoints**

   * Swagger (dev): `http://localhost:5240/swagger`.
   * Usar Postman con las URLs listadas en la sección Endpoints o con la colección Postman de este repositorio.

---

# Endpoints (5)

Base URL de ejemplo local: `http://localhost:5240`

1. `POST /clients` — Crear cliente

   * URL: `POST http://localhost:5240/clients`
   * Body (JSON):

     ```json
     {
       "firstName": "Alice",
       "lastName": "Wonderland",
       "email": "alice@censudex.cl",
       "username": "alice123",
       "birthDate": "1995-08-15",
       "address": "123 Fantasy St",
       "phoneNumber": "+56912345678",
       "password": "StrongPass1!"
     }
     ```
   * Respuestas: `201 Created` con DTO del cliente (sin contraseña); `400` validaciones; `409` conflicto email/username.

2. `GET /clients` — Listar clientes (filtros, paginado, orden)

   * URL: `GET http://localhost:5240/clients`
   * Query params soportados:

     * `name` (full o parcial, busca `FirstName + " " + LastNames`)
     * `email` (exact)
     * `username` (parcial)
     * `isActive` (true/false)
     * `sortBy` (FirstName, LastNames, Email, Username, CreatedAt)
     * `isDescending` (true/false)
     * `pageNumber`, `pageSize`
   * Respuesta: objeto con `Items`, `TotalCount`, `TotalPages`.

3. `GET /clients/{id}` — Obtener cliente por ID

   * URL: `GET http://localhost:5240/clients/{id}`
   * Respuestas: `200` con `ViewUserResponse` o `404` si no existe.

4. `PATCH /clients/{id}` — Actualizar cliente (replace/partial as implemented)

   * URL: `PATCH http://localhost:5240/clients/{id}`
   * Body (JSON) — usa el mismo DTO que creación; password opcional.

     ```json
     {
       "firstName": "AliceUpdated",
       "lastName": "WonderlandUpdated",
       "email": "aliceupdated@censudex.cl",
       "username": "aliceUpdated123",
       "birthDate": "1995-08-15",
       "address": "456 Fantasy St",
       "phoneNumber": "+56987654321",
       "password": "NewStrongPass1!"
     }
     ```
   * Respuestas: `204 No Content`, `400` validaciones, `404` no encontrado.

5. `PATCH /clients/delete/{id}` — Soft delete (desactivar)

   * URL: `PATCH http://localhost:5240/clients/delete/{id}`
   * Acción: `IsActive = false`.
   * Respuestas: `204 No Content`, `404` si no existe.

---

# Qué contiene este repositorio

* `Program.cs` — arranque, carga `.env`, registra ApplicationDBContext y repositorio, aplica migraciones y hace seed condicional.
* `ApplicationDBContext` — EF Core context + configuración.
* `Models/Client.cs` — entidad cliente.
* `Dtos/` — CreateUserRequest, ViewUserResponse, etc.
* `Repositories/` — `IClientRepository`, `ClientRepository`.
* `Controllers/ClientsController.cs` — endpoints REST.
* `Mappers/ClientMapper.cs` — mapeo DTO ↔ Model.
* `Data/Seeder.cs` — seeder runtime (solo si tabla vacía).

---
