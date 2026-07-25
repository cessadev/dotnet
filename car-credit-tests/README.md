# Car Credit Tests

Suite de pruebas unitarias para [`car-credit`](../car-credit), construida con **xUnit** y **Moq**, enfocada en validar las reglas de negocio de la capa `Application/Services` de forma aislada, sin dependencias reales de base de datos.

Proyecto de práctica orientado a demostrar la capacidad de escribir pruebas unitarias significativas — que verifiquen comportamiento y reglas de negocio, no solo cobertura de líneas — sobre una arquitectura desacoplada mediante interfaces.

---

## Stack técnico

| Categoría | Tecnología |
|---|---|
| Framework de pruebas | xUnit 2.5 |
| Mocking | Moq 4.20 |
| Cobertura | coverlet.collector |
| Runner | Microsoft.NET.Test.Sdk / `dotnet test` |

---

## Por qué es posible probar `Application` de forma aislada

Este proyecto de pruebas solo puede existir gracias a las decisiones de arquitectura tomadas en `car-credit`:

- Los `Service` (`LoanService`, `InstallmentService`, `CustomerService`) dependen únicamente de **interfaces** (`ILoanRepository`, `ICustomerRepository`, `IVehicleRepository`, `IInstallmentRepository`), nunca de `AppDbContext` directamente.
- Gracias a esto, cada repositorio puede sustituirse por un **mock** con Moq, permitiendo probar la lógica de negocio del `Service` sin necesidad de una base de datos real, un contenedor de Docker corriendo, ni datos de prueba persistidos.
- Esta es la razón práctica detrás de la regla de dependencia de Clean Architecture: no es solo teoría de diseño, es lo que hace posible este proyecto de pruebas.

La referencia al proyecto principal se declara como `ProjectReference` en el `.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="..\car-credit\car-credit.csproj" />
</ItemGroup>
```

---

## Estructura

```
car-credit-tests/
└── Application/
    └── Services/
        ├── CreateLoanTests.cs      → LoanService.Create
        ├── DeleteLoanTests.cs      → LoanService.Delete
        └── RegisterPaymentTests.cs → InstallmentService.RegisterPayment
```

La estructura de carpetas refleja exactamente la del proyecto probado (`car-credit/Application/Services/`), para que sea inmediato ubicar qué archivo de test corresponde a qué clase de producción.

---

## Patrón de prueba: Arrange / Act / Assert

Todas las pruebas siguen la misma estructura de tres pasos, con los mocks de los repositorios configurados explícitamente en cada caso:

```csharp
// Arrange — se configuran los mocks y sus respuestas simuladas
var mockLoanRepository = new Mock<ILoanRepository>();
mockLoanRepository.Setup(r => r.GetByReference(reference)).ReturnsAsync(existingLoan);

// Act — se ejecuta el método real bajo prueba
var result = await service.Delete(reference);

// Assert — se verifica el resultado y que los mocks se hayan llamado (o no) como se esperaba
Assert.True(result);
mockLoanRepository.Verify(r => r.Remove(existingLoan), Times.Once);
```

El uso de `mockRepository.Verify(..., Times.Never)` en los casos de error es intencional: no basta con comprobar que se lanzó la excepción esperada, también se verifica que **ninguna escritura contra el repositorio ocurrió** — por ejemplo, que un préstamo con cliente inexistente nunca llega a llamar `Add` ni `SaveChanges`.

---

## Cobertura actual

### `LoanService.Create`

| Escenario | Qué verifica |
|---|---|
| Cliente inexistente | Lanza `KeyNotFoundException`; no se persiste nada |
| Vehículo inexistente | Lanza `KeyNotFoundException`; no se persiste nada |
| Monto excede el valor comercial del vehículo | Lanza `InvalidOperationException`; no se persiste nada |
| Solicitud válida — redondeo | La suma exacta de las cuotas es igual al monto del préstamo; el residuo se acumula en la última cuota |
| Solicitud válida — plazos (`Theory`) | Genera el número correcto de cuotas para cada valor de `EInstallmentsTerm` (6 a 48 meses) |
| Solicitud válida — referencias | La referencia del préstamo tiene el formato y longitud esperados; cada cuota genera su `PaymentReference` correctamente enlazado |
| Solicitud válida — fechas | Las fechas de vencimiento de las cuotas son estrictamente crecientes |

### `LoanService.Delete`

| Escenario | Qué verifica |
|---|---|
| Préstamo inexistente | Retorna `false`; nunca se consulta si hay cuotas pendientes ni se elimina nada |
| Cuotas pendientes | Lanza `InvalidOperationException`; nunca se elimina el préstamo |
| Todas las cuotas pagadas | Elimina el préstamo; se llama `Remove` y `SaveChanges` exactamente una vez |

### `InstallmentService.RegisterPayment`

| Escenario | Qué verifica |
|---|---|
| Cuota inexistente (`PaymentReference` inválido) | Retorna `null`; nunca se registra un pago |
| Pago válido | Se crea el `Payment` con los datos correctos; la cuota queda marcada como pagada, con `AmountPaid` y `DatePayment` actualizados |
| Monto no coincide con el valor de la cuota | Lanza `InvalidOperationException`; el estado de la cuota permanece sin cambios (modelo estricto, sin pagos parciales) |

---

## Cómo ejecutar las pruebas

### Desde la raíz de la solución

```bash
dotnet test
```

### Con reporte de cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Un archivo o un método específico

```bash
dotnet test --filter "FullyQualifiedName~CreateLoanTests"
dotnet test --filter "FullyQualifiedName~Create_AmountExceedsVehicleMarketValue_ThrowsInvalidOperationException"
```

### Dentro del entorno Docker del proyecto

```bash
docker compose exec devenv bash
dotnet test
```

---

## Convenciones de nomenclatura

Los métodos de prueba siguen el formato `Metodo_Escenario_ResultadoEsperado`:

```
Create_CustomerDoesNotExist_ThrowsKeyNotFoundException
Delete_LoanAllInstallmentsPaid_DeletesLoan
RegisterPayment_PaymentAmountDoesNotMatchInstallment_ThrowsInvalidOperationException
```

Esto permite identificar de inmediato, solo por el nombre del test —sin abrir el archivo—, qué método se prueba, bajo qué condición, y qué se espera que ocurra.

---

## Autor

César Urbiña