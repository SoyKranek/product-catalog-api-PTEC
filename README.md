# Catálogo de productos

API REST para crear productos, consultarlos y mover su stock sin dejarlo en negativo. La usan varias plataformas al mismo tiempo, así que el descuento de existencias se resuelve en un solo `UPDATE` de PostgreSQL.

## Cómo está armada

Cuatro proyectos, las dependencias van hacia el dominio:

- `ProductCatalog.Domain`: `Producto` y las reglas de precio y existencias.
- `ProductCatalog.Application`: un manejador por caso de uso y `IRepositorioProducto`.
- `ProductCatalog.Infrastructure`: Dapper, el script de la tabla y el `UPDATE` condicional.
- `ProductCatalog.Api`: controlador, FluentValidation, Problem Details y Swagger.

Elegí .NET 8 porque es el LTS. Dapper porque el SQL queda explícito, igual que en los sistemas que mantengo con SQL Server y Oracle. PostgreSQL porque se puede levantar con Docker y publicar sin pagar una instancia de SQL Server. El ajuste de stock no lee y luego escribe: la base solo actualiza la fila si `stock_quantity + cambio >= 0`. Si otra petición se adelantó, el manejador reintenta una vez y, si sigue sin poder aplicar el cambio por carrera, responde 409.

## Requisitos

- .NET 8 SDK
- Docker Desktop
- Node.js 20, solo si vas a correr las pruebas Jest del contrato

## Variables

| Variable | Uso |
| --- | --- |
| `ConnectionStrings__ProductCatalog` | Cadena de PostgreSQL. En Render puede llegar como URL `postgres://`. |
| `ASPNETCORE_ENVIRONMENT` | `Development` en local, `Production` en Render. |

Ejemplo en `.env.example`. La clave local del compose es solo para desarrollo.

## Correr en local

```bash
docker compose up --build
```

Swagger: `http://localhost:8080/swagger`  
Salud: `http://localhost:8080/health`

Sin Docker, con PostgreSQL ya creado:

```bash
dotnet run --project src/ProductCatalog.Api
```

La tabla `products` se crea al arrancar.

## Pruebas

```bash
dotnet test
cd tests/contrato
npm install
npm test
```

## Contrato

Base: `/api/products`. JSON en camelCase.

Crear:

```bash
curl -X POST http://localhost:8080/api/products ^
  -H "Content-Type: application/json" ^
  -d "{\"nombre\":\"Café molido\",\"descripcion\":\"Bolsa de 500 gramos\",\"precioUnitario\":18500.50,\"existenciasIniciales\":10}"
```

Responde **201** y el encabezado `Location`.

Consultar uno: `GET /api/products/{productoId}` → **200** o **404**.

Listar: `GET /api/products?numeroPagina=1&tamanoPagina=20` → `elementos`, `numeroPagina`, `tamanoPagina`, `totalRegistros`.

Actualizar ficha (el stock no se toca):

```bash
curl -X PUT http://localhost:8080/api/products/{productoId} ^
  -H "Content-Type: application/json" ^
  -d "{\"nombre\":\"Café molido\",\"descripcion\":\"Bolsa de 250 gramos\",\"precioUnitario\":12000}"
```

Eliminar: `DELETE /api/products/{productoId}` → **204** o **404**.

Ajustar existencias. Positivo suma, negativo resta. Cero o resultado negativo → **400**. Producto inexistente → **404**. Carrera no resuelta → **409**.

```bash
curl -X PATCH http://localhost:8080/api/products/{productoId}/stock ^
  -H "Content-Type: application/json" ^
  -d "{\"cambioCantidad\":-2}"
```

Producto de respuesta: `productoId`, `nombre`, `descripcion`, `precioUnitario`, `cantidadExistencias`, `fechaCreacionUtc`, `fechaActualizacionUtc`.

Los errores usan `type`, `title`, `status` y `detail`.

## Despliegue

Imagen del `Dockerfile`, puerto **8080**. En Render el blueprint es `render.yaml`: servicio web gratis y PostgreSQL. El plan gratis puede dormir el servicio; la primera petición después de un rato tarda más.

URL pública: pendiente de publicar en Render.
