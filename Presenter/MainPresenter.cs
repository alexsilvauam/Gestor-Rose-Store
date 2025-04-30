// MainPresenter.cs
using System;
using System.Collections.Generic; // Para el HashSet
using System.Threading.Tasks; // Para async/await
using System.Windows.Forms; // Para el diálogo de selección de archivos
using Gestor_Rose_Store.Models;
using Gestor_Rose_Store.View; // Para la interfaz IMainView

namespace Gestor_Rose_Store.Presenter
{
    public class MainPresenter
    {
        private readonly IMainView _view;
        
        private readonly ProcesadorExcel _model;
        
        private string? _rutaArchivoResumen = null;
        private string? _rutaArchivoVentasPropias = null;

        public MainPresenter(IMainView view)
        {
            _view = view;
            // _model = new ProcesadorExcel(); // <- Crearemos ProcesadorExcel luego

            // Suscribimos los métodos del Presenter a los eventos de la View
            _view.SeleccionarArchivoResumenClick += OnSeleccionarArchivoResumen;
            _view.SeleccionarArchivoVentasPropiasClick += OnSeleccionarArchivoVentasPropias;
            _view.ProcesarArchivosClick += async (s, e) => await OnProcesarArchivosAsync(); // Hacemos el handler async

            // Estado inicial de la UI controlado por el Presenter
            ActualizarEstadoBotonProcesar();
            _view.StatusText = "Listo. Selecciona los archivos.";
            _model = new ProcesadorExcel();
        }

        // --- Manejadores de Eventos de la Vista ---

        private void OnSeleccionarArchivoResumen(object? sender, EventArgs e)
        {
            string? rutaSeleccionada = _view.PedirRutaArchivoExcel("Selecciona el archivo de Resumen de Ventas Colectivo");
            if (!string.IsNullOrEmpty(rutaSeleccionada))
            {
                _rutaArchivoResumen = rutaSeleccionada;
                _view.RutaArchivoResumen = System.IO.Path.GetFileName(rutaSeleccionada); // Mostrar solo nombre de archivo
                ActualizarEstadoBotonProcesar();
            }
        }

        private void OnSeleccionarArchivoVentasPropias(object? sender, EventArgs e)
        {
            string? rutaSeleccionada = _view.PedirRutaArchivoExcel("Selecciona tu archivo de Ventas Propias");
            if (!string.IsNullOrEmpty(rutaSeleccionada))
            {
                _rutaArchivoVentasPropias = rutaSeleccionada;
                _view.RutaArchivoVentasPropias = System.IO.Path.GetFileName(rutaSeleccionada); // Mostrar solo nombre de archivo
                ActualizarEstadoBotonProcesar();
            }
        }

        // En MainPresenter.cs

        private async Task OnProcesarArchivosAsync()
        {
            if (string.IsNullOrEmpty(_rutaArchivoResumen) || string.IsNullOrEmpty(_rutaArchivoVentasPropias))
            {
                _view.MostrarMensajeError("Error", "Faltan seleccionar uno o ambos archivos Excel.");
                return;
            }

            _view.StatusText = "Procesando archivos... por favor espera.";
            _view.ControlesSeleccionEnabled = false;
            _view.BotonProcesarEnabled = false;

            // --- ESTE ES EL BLOQUE QUE LLAMA AL MODELO REAL ---
            try
            {
                // Llama al método del modelo que hace el trabajo pesado
                int filasActualizadas = await _model.ProcesarArchivosAsync(_rutaArchivoResumen, _rutaArchivoVentasPropias);

                // Obtener el nombre esperado del archivo de salida para mostrarlo al usuario
                // NOTA: _model debe existir y GenerarNombreArchivoSalida debe ser public en ProcesadorExcel
                string nombreArchivoSalida = Path.GetFileName(_model.GenerarNombreArchivoSalida(_rutaArchivoVentasPropias));

                if (filasActualizadas > 0)
                {
                    _view.MostrarMensajeInfo("Proceso Completado", $"Se actualizaron {filasActualizadas} filas.\nEl resultado se guardó en:\n'{nombreArchivoSalida}'");
                    _view.StatusText = $"Completado. {filasActualizadas} filas actualizadas en {nombreArchivoSalida}.";
                }
                else
                {
                    _view.MostrarMensajeInfo("Proceso Completado", "No se encontraron nuevas coincidencias para actualizar.");
                    _view.StatusText = "Completado. No se encontraron coincidencias.";
                    // Considera si quieres informar que no se guardó un archivo nuevo si no hubo cambios.
                }
            }
            catch (Exception ex)
            {
                // Captura errores específicos del Modelo o generales
                _view.MostrarMensajeError("Error en Procesamiento", $"Ocurrió un error: {ex.Message}");
                _view.StatusText = "Error durante el procesamiento.";
                // Aquí podrías añadir logging del error completo si lo necesitas:
                // Console.WriteLine($"ERROR DETALLADO: {ex.ToString()}");
            }
            finally
            {
                // Siempre volver a habilitar controles
                _view.ControlesSeleccionEnabled = true;
                ActualizarEstadoBotonProcesar(); // Habilitará el botón si los archivos siguen seleccionados
            }
            // --- FIN DEL BLOQUE QUE LLAMA AL MODELO ---
        }

        // El resto de MainPresenter (OnSeleccionar..., ActualizarEstadoBotonProcesar...) sigue igual
        // --- Lógica del Presenter ---

        private void ActualizarEstadoBotonProcesar()
        {
            // Habilita el botón "Procesar" solo si ambas rutas tienen valor
            bool habilitar = !string.IsNullOrEmpty(_rutaArchivoResumen) &&
                             !string.IsNullOrEmpty(_rutaArchivoVentasPropias);
            _view.BotonProcesarEnabled = habilitar;
        }
    }
}