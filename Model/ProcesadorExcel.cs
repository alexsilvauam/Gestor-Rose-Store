// ProcesadorExcel.cs
using System;
using System.Collections.Generic;
using System.IO; // Necesario para excepciones de archivo y Path
using System.Linq; // Útil para buscar columnas
using System.Threading.Tasks;
using ClosedXML.Excel; // ¡La librería para Excel!
// ClosedXML usa su propia estructura XLColor, no siempre necesita System.Drawing

namespace Gestor_Rose_Store.Models // O el namespace que uses para el Modelo
{
    public class ProcesadorExcel
    {
        // --- CONSTANTES AJUSTADAS ---
        private const string COLUMNA_CODIGO_RESUMEN = "Referencia";
        private const string COLUMNA_CODIGO_VENTAS = "Cod";
        private const string COLUMNA_ESTADO_VENTAS = "Status";
        private const string VALOR_VENDIDO = "VENDIDO"; // Ahora en mayúsculas
        private const string HOJA_RESUMEN = "Hoja1";
        private const string HOJA_VENTAS = "INVENT C$";
        // Color para resaltar la fila (puedes cambiarlo si prefieres otro)
        private static readonly XLColor COLOR_RESALTADO = XLColor.Yellow;
        // --- FIN DE CONSTANTES ---

        // Método principal para procesar los archivos
        public async Task<int> ProcesarArchivosAsync(string rutaArchivoResumen, string rutaArchivoVentasPropias)
        {
            int filasActualizadas = 0;
            var codigosVendidos = new HashSet<string>();

            // --- 1. Leer Archivo Resumen ---
            try
            {
                await Task.Run(() =>
                {
                    using var workbookResumen = new XLWorkbook(rutaArchivoResumen);
                    IXLWorksheet? worksheetResumen = ObtenerHoja(workbookResumen, HOJA_RESUMEN, "resumen");

                    int colCodigoResumenIdx = EncontrarIndiceColumna(worksheetResumen, COLUMNA_CODIGO_RESUMEN, "resumen");

                    foreach (var row in worksheetResumen.RowsUsed().Skip(1)) // Omitir cabecera
                    {
                        var cellValue = row.Cell(colCodigoResumenIdx).GetValue<string>();
                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            codigosVendidos.Add(cellValue.Trim());
                        }
                    }
                }); // Fin Task.Run resumen

                if (codigosVendidos.Count == 0)
                {
                    Console.WriteLine("Advertencia: No se leyeron códigos del archivo resumen.");
                    // Considera si lanzar una excepción aquí es apropiado para tu flujo
                    // throw new InvalidOperationException("No se encontraron códigos válidos en el archivo resumen.");
                }
            }
            catch (FileNotFoundException) { throw new FileNotFoundException($"No se encontró el archivo resumen en: {rutaArchivoResumen}"); }
            catch (IOException ex) { throw new IOException($"Error al leer el archivo resumen (¿está abierto en Excel?): {rutaArchivoResumen}. Detalles: {ex.Message}", ex); }
            catch (Exception ex) { throw new Exception($"Error inesperado al procesar el archivo resumen: {ex.Message}", ex); }


            // --- 2. Leer y Actualizar Archivo de Ventas ---
            string rutaArchivoSalida = GenerarNombreArchivoSalida(rutaArchivoVentasPropias);
            try
            {
                await Task.Run(() => {
                    using var workbookVentas = new XLWorkbook(rutaArchivoVentasPropias);
                    IXLWorksheet? worksheetVentas = ObtenerHoja(workbookVentas, HOJA_VENTAS, "ventas");

                    int colCodigoVentasIdx = EncontrarIndiceColumna(worksheetVentas, COLUMNA_CODIGO_VENTAS, "ventas");
                    int colEstadoVentasIdx = EncontrarIndiceColumna(worksheetVentas, COLUMNA_ESTADO_VENTAS, "ventas (estado)");

                    bool seHicieronCambios = false; // Bandera para saber si guardar

                    foreach (var row in worksheetVentas.RowsUsed().Skip(1)) // Omitir cabecera
                    {
                        var celdaCodigo = row.Cell(colCodigoVentasIdx);
                        string codigoActual = celdaCodigo.GetValue<string>();

                        if (!string.IsNullOrWhiteSpace(codigoActual) && codigosVendidos.Contains(codigoActual.Trim()))
                        {
                            var celdaEstado = row.Cell(colEstadoVentasIdx);

                            // Actualizar solo si no está ya marcado o si queremos remarcar siempre
                            if (celdaEstado.GetValue<string>().Trim().ToUpperInvariant() != VALOR_VENDIDO)
                            {
                                celdaEstado.Value = VALOR_VENDIDO;

                                // --- Aplicar formato a la fila ---
                                try
                                {
                                    // Determina el rango de celdas usadas en esta fila específica
                                    var firstCell = row.FirstCellUsed();
                                    var lastCell = row.LastCellUsed();

                                    if (firstCell != null && lastCell != null) // Asegurarse que la fila no esté completamente vacía
                                    {
                                        var firstColUsed = firstCell.Address.ColumnNumber;
                                        var lastColUsed = lastCell.Address.ColumnNumber;
                                        // Aplicar color de fondo a toda la fila usada
                                        row.Cells(firstColUsed, lastColUsed).Style.Fill.BackgroundColor = COLOR_RESALTADO;
                                        // Podrías añadir más formato si quieres:
                                        // row.Range(firstColUsed, lastColUsed).Style.Font.Bold = true;
                                    }
                                }
                                catch (Exception formatEx)
                                {
                                    // No detener todo por un error de formato, solo registrarlo
                                    Console.WriteLine($"Advertencia: No se pudo aplicar formato a la fila {row.RowNumber()}. Error: {formatEx.Message}");
                                }
                                // --- Fin aplicar formato ---

                                filasActualizadas++;
                                seHicieronCambios = true; // Marcamos que hubo al menos un cambio
                            }
                            else
                            {
                                // Opcional: Si ya estaba vendido, ¿quieres re-aplicar el formato por si acaso?
                                // Podrías poner aquí la lógica de formato si quieres asegurarte
                                // que todas las filas vendidas (incluso las previamente marcadas) tengan el formato.
                            }
                        }
                    }

                    // --- 4. Guardar Cambios ---
                    if (seHicieronCambios) // Guardar solo si se modificó algo
                    {
                        workbookVentas.SaveAs(rutaArchivoSalida);
                    }
                    else
                    {
                        Console.WriteLine("No se realizaron nuevos cambios en el archivo de ventas.");
                        // Si no hubo cambios, no se sobrescribe ni se crea archivo nuevo.
                        // El Presenter informará que no hubo actualizaciones.
                    }

                }); // Fin Task.Run ventas
            }
            catch (FileNotFoundException) { throw new FileNotFoundException($"No se encontró el archivo de ventas en: {rutaArchivoVentasPropias}"); }
            catch (IOException ex) { throw new IOException($"Error al leer/escribir el archivo de ventas (¿está abierto en Excel?): {rutaArchivoVentasPropias}. Detalles: {ex.Message}", ex); }
            catch (System.Security.SecurityException ex) { throw new IOException($"No se pudo guardar el archivo en '{rutaArchivoSalida}'. Verifica los permisos de escritura. Detalles: {ex.Message}", ex); }
            catch (Exception ex) { throw new Exception($"Error inesperado al procesar el archivo de ventas: {ex.Message}", ex); }

            // --- 5. Devolver Resultado ---
            return filasActualizadas;
        }

        // --- Métodos Auxiliares ---

        // Obtiene la hoja de trabajo de forma segura
        private IXLWorksheet ObtenerHoja(XLWorkbook workbook, object hojaIdentificador, string nombreArchivoParaError)
        {
            IXLWorksheet? worksheet;
            if (hojaIdentificador is int index)
            {
                if (workbook.Worksheets.Count < index || index <= 0)
                    throw new InvalidOperationException($"El archivo {nombreArchivoParaError} no tiene una hoja en el índice {index}.");
                worksheet = workbook.Worksheet(index);
            }
            else if (hojaIdentificador is string name)
            {
                if (!workbook.Worksheets.TryGetWorksheet(name, out worksheet))
                    throw new InvalidOperationException($"No se encontró la hoja '{name}' en el archivo {nombreArchivoParaError}.");
            }
            else
            {
                throw new InvalidOperationException($"Configuración de hoja inválida para el archivo {nombreArchivoParaError}.");
            }
            return worksheet;
        }

        // Encuentra el índice de una columna por nombre o letra (simple)
        private int EncontrarIndiceColumna(IXLWorksheet worksheet, string nombreColumna, string archivoParaError)
        {
            // Intenta encontrar por el texto en la primera fila (cabecera) - Case Insensitive
            var headerCell = worksheet.FirstRowUsed()?.CellsUsed(c => c.Value.ToString().Trim().Equals(nombreColumna, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (headerCell != null)
            {
                return headerCell.Address.ColumnNumber;
            }

            // Si no se encontró por nombre y es una letra, intenta por letra
            if (nombreColumna.Length == 1 && char.IsLetter(nombreColumna[0]))
            {
                try
                {
                    // Intenta usar la letra directamente (ClosedXML puede manejarlo)
                    // Verificamos si la columna existe usándola
                    worksheet.Column(nombreColumna);
                    // Si no lanza excepción, obtenemos el número
                    return XLHelper.GetColumnNumberFromLetter(nombreColumna.ToUpper());
                }
                catch
                {
                    // Ignorar error si la columna no existe por letra
                }
            }

            // Si no se encontró de ninguna forma
            throw new InvalidOperationException($"No se encontró la columna '{nombreColumna}' en la hoja '{worksheet.Name}' del archivo {archivoParaError}. Verifica el nombre o la letra.");
        }


        // Genera el nombre para el archivo de salida (¡Debe ser public!)
        public string GenerarNombreArchivoSalida(string rutaOriginal)
        {
            string? directorio = Path.GetDirectoryName(rutaOriginal);
            string nombreSinExtension = Path.GetFileNameWithoutExtension(rutaOriginal);
            string extension = Path.GetExtension(rutaOriginal);
            directorio ??= "";
            return Path.Combine(directorio, $"{nombreSinExtension}_Actualizado{extension}");
        }
    }
}