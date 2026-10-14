const fs = require("fs");
const path = require("path");

const contrato = JSON.parse(
  fs.readFileSync(path.join(__dirname, "contrato-ejemplo.json"), "utf8")
);

describe("contrato del catálogo", () => {
  test("el producto expone los campos en español", () => {
    expect(Object.keys(contrato.producto)).toEqual([
      "productoId",
      "nombre",
      "descripcion",
      "precioUnitario",
      "cantidadExistencias",
      "fechaCreacionUtc",
      "fechaActualizacionUtc",
    ]);
  });

  test("el listado paginado expone elementos y totales", () => {
    expect(Object.keys(contrato.pagina)).toEqual([
      "elementos",
      "numeroPagina",
      "tamanoPagina",
      "totalRegistros",
    ]);
  });

  test("el ajuste de existencias pide el cambio de cantidad", () => {
    expect(contrato.ajuste).toEqual({ cambioCantidad: -2 });
  });
});
