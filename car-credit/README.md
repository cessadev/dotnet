# Car Credit API

Backend REST que simula la gestión de créditos para adquisición de vehículos de carga, construido con **.NET 8**, **Entity Framework Core**, **Dapper** y **SQL Server**, siguiendo **Clean Architecture**.

Proyecto de práctica orientado a demostrar conocimientos prácticos de arquitectura, modelado de dominio financiero, acceso a datos híbrido (ORM + SQL crudo) y buenas prácticas de diseño de API REST.

---

## Stack técnico

| Categoría | Tecnología |
|---|---|
| Framework | .NET 8 / ASP.NET Core Web API |
| ORM | Entity Framework Core 8 (migraciones, Fluent API) |
| Acceso a datos crudo | Dapper (reportes agregados, Stored Procedures) |
| Base de datos | SQL Server 2022 |
| Documentación interactiva | Swagger / OpenAPI |
| Contenedores | Docker Compose |

---

## Arquitectura

El proyecto sigue **Clean Architecture** con separación estricta por capas, organizadas por carpetas dentro de un único proyecto:

```
├── Domain/
│   ├── Entities/        → POCOs puros, sin dependencias externas
│   └── Enums/
├── Application/
│   ├── Interfaces/      → Puertos de entrada (Services) y salida (Repositories)
│   ├── Services/        → Casos de uso, reglas de negocio
│   ├── DTOs/
│   │   ├── Requests/
│   │   ├── Responses/
│   │   └── Queries/     → Proyecciones de solo lectura (reportes)
│   └── Converters/       → Serialización JSON personalizada
├── Infrastructure/
│   └── Persistence/
│       ├── AppDbContext.cs
│       ├── Configuration/   → Fluent API (precisión decimal, conversiones)
│       └── Repositories/    → Implementación de los puertos de salida
└── Controllers/
```

**Regla de dependencia:** las capas externas dependen de las internas, nunca al revés. `Domain` no conoce a nadie; `Application` conoce `Domain`; `Infrastructure` y `Controllers` conocen `Application`, nunca entre sí directamente.

El acceso a datos combina dos estrategias según el caso de uso:
- **EF Core** para operaciones CRUD y transaccionales (crear préstamo + cuotas de forma atómica mediante propiedades de navegación).
- **Dapper + Stored Procedure** para reportes agregados (`usp_GetOverdueInstallments`), priorizando rendimiento en consultas de solo lectura con múltiples `JOIN`.

---

## Modelo de dominio

5 entidades centrales:

- **Customer** — cliente identificado por tipo y número de documento (`CC`, `CE`, `TI`, `PA`, `PPT`, `NIT`)
- **Vehicle** — vehículo de carga, con valor comercial de mercado
- **Loan** — crédito vinculado a un cliente y un vehículo, identificado por una referencia legible (no por su ID interno)
- **Installment** — cuota individual de un préstamo, con su propia referencia de pago
- **Payment** — registro de la transacción real contra una cuota

**Decisiones de diseño destacadas:**

- **Identificadores de negocio en la API pública**: los endpoints y DTOs exponen `DocumentNumber`, `Identifier`, `Reference` y `PaymentReference` — nunca los IDs autoincrementales internos de la base de datos. Los IDs quedan reservados para las relaciones de EF Core y las consultas SQL/Dapper.
- **Precisión financiera**: todos los campos monetarios usan `decimal(18,2)` configurado explícitamente vía Fluent API — nunca `double`, para evitar errores de redondeo en operaciones financieras.
- **Redondeo de cuotas sin pérdida**: el cálculo de cada cuota trunca hacia abajo y acumula el residuo en la última cuota, garantizando que la suma exacta de las cuotas sea igual al monto del préstamo.
- **Reglas de negocio explícitas**: el monto de un préstamo no puede exceder el valor comercial del vehículo; un préstamo o cliente con cuotas pendientes no puede eliminarse; los pagos deben coincidir exactamente con el valor de la cuota (modelo estricto, sin pagos parciales ni sobrepagos).
- **Contratos JSON legibles**: los enums se serializan como texto (`"CC"`, `"Toyota"`, `"Months12"`) en vez de números, mediante converters personalizados y `JsonStringEnumConverter`.

---

## Cómo levantar el proyecto

### Prerrequisitos
- Docker y Docker Compose

### Pasos

```bash
# 1. Levantar los contenedores (SQL Server + entorno de desarrollo .NET)
docker compose up -d

# 2. Entrar al contenedor de desarrollo
docker compose exec devenv bash

# 3. Aplicar las migraciones (crea el esquema y el Stored Procedure)
dotnet ef database update

# 4. Correr la API
dotnet run --urls http://0.0.0.0:5099
```

La API queda disponible en `http://localhost:5099`, con Swagger UI en la raíz (`/`).

### Reiniciar la base de datos desde cero

```bash
docker compose down -v   # elimina también el volumen de datos
docker compose up -d
dotnet ef database update
```

---

## Probar la API

El archivo [`car-credit.http`](./car-credit.http) contiene todos los endpoints documentados y listos para ejecutar con la extensión [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) de VS Code.

Flujo típico de prueba:
1. Crear un `Customer`
2. Crear un `Vehicle`
3. Crear un `Loan` referenciando ambos — genera automáticamente sus `Installment`
4. Consultar las cuotas generadas (`GET /api/installment/loan/{reference}`)
5. Registrar el pago de una cuota (`PATCH /api/installment/{paymentReference}/pay`)

---

## Endpoints principales

| Recurso | Método | Ruta |
|---|---|---|
| Customer | `GET` | `/api/customer` |
| Customer | `GET` | `/api/customer/{documentNumber}` |
| Customer | `POST` | `/api/customer` |
| Customer | `DELETE` | `/api/customer/{documentNumber}` |
| Vehicle | `GET` | `/api/vehicle` |
| Vehicle | `GET` | `/api/vehicle/{identifier}` |
| Vehicle | `POST` | `/api/vehicle` |
| Loan | `GET` | `/api/loan` |
| Loan | `GET` | `/api/loan/{reference}` |
| Loan | `POST` | `/api/loan` |
| Loan | `DELETE` | `/api/loan/{reference}` |
| Installment | `GET` | `/api/installment/loan/{loanReference}` |
| Installment | `GET` | `/api/installment/loan/{loanReference}/summary` |
| Installment | `GET` | `/api/installment/overdue` |
| Installment | `GET` | `/api/installment/loan/{loanReference}/overdue` |
| Installment | `PATCH` | `/api/installment/{paymentReference}/pay` |

---

## Autor

César Urbiña