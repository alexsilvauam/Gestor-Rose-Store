using System;
using System.Windows.Forms;
using Gestor_Rose_Store.Presenter; // Para el Presenter
using Gestor_Rose_Store.View; // Para la interfaz IMainView
using System.IO; // Para Path

namespace Gestor_Rose_Store.View
{
    // Cambiamos Form1 por frmMain aquí
    public partial class frmMain : Form, IMainView
    {
        private readonly MainPresenter _presenter;

        // Cambiamos el constructor de Form1 a frmMain
        public frmMain()
        {
            InitializeComponent();

            // La creación del Presenter sigue igual, le pasamos 'this' (esta instancia de frmMain)
            _presenter = new MainPresenter(this);

            // La conexión de eventos sigue igual
            this.btnSeleccionarResumen.Click += (sender, e) => SeleccionarArchivoResumenClick?.Invoke(this, EventArgs.Empty);
            this.btnSeleccionarVentasPropias.Click += (sender, e) => SeleccionarArchivoVentasPropiasClick?.Invoke(this, EventArgs.Empty);
            this.btnProcesar.Click += (sender, e) => ProcesarArchivosClick?.Invoke(this, EventArgs.Empty);
        }

        // --- Implementación de la interfaz IMainView (Esta parte NO cambia) ---

        public string RutaArchivoResumen
        {
            get => lblRutaResumen.Text;
            set => lblRutaResumen.Text = value;
        }

        public string RutaArchivoVentasPropias
        {
            get => lblRutaVentasPropias.Text;
            set => lblRutaVentasPropias.Text = value;
        }

        public string StatusText
        {
            set => lblStatus.Text = value;
        }

        public bool BotonProcesarEnabled
        {
            get => btnProcesar.Enabled;
            set => btnProcesar.Enabled = value;
        }

        public bool ControlesSeleccionEnabled
        {
            set
            {
                btnSeleccionarResumen.Enabled = value;
                btnSeleccionarVentasPropias.Enabled = value;
            }
        }

        public event EventHandler? SeleccionarArchivoResumenClick;
        public event EventHandler? SeleccionarArchivoVentasPropiasClick;
        public event EventHandler? ProcesarArchivosClick;


        public void MostrarMensajeError(string titulo, string mensaje)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void MostrarMensajeInfo(string titulo, string mensaje)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public string? PedirRutaArchivoExcel(string titulo)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = titulo;
                openFileDialog.Filter = "Archivos Excel (*.xlsx)|*.xlsx|Todos los archivos (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    return openFileDialog.FileName;
                }
                return null;
            }
        }
        private void frmMain_Load(object sender, EventArgs e) { }


        // --- Fin de la implementación de IMainView ---
    }
}