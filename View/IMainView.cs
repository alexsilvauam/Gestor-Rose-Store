// IMainView.cs
using System;
using System.Windows.Forms; // Necesario para ControlEventHandler

namespace Gestor_Rose_Store.View
{
    public interface IMainView
    {
        // Propiedades para obtener/establecer textos en la UI
        string RutaArchivoResumen { get; set; }
        string RutaArchivoVentasPropias { get; set; }
        string StatusText { set; } // Solo necesitamos 'set' desde el Presenter

        // Propiedades para habilitar/deshabilitar controles
        bool BotonProcesarEnabled { get; set; }
        bool ControlesSeleccionEnabled { set; } // Para deshabilitar botones/labels mientras procesa

        // Eventos que la Vista disparará hacia el Presentador
        event EventHandler? SeleccionarArchivoResumenClick;
        event EventHandler? SeleccionarArchivoVentasPropiasClick;
        event EventHandler? ProcesarArchivosClick;

        // Métodos para que el Presenter interactúe con la Vista
        void MostrarMensajeError(string titulo, string mensaje);
        void MostrarMensajeInfo(string titulo, string mensaje);
        string? PedirRutaArchivoExcel(string titulo); // Para abrir el diálogo de selección
    }
}