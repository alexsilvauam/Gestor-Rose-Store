namespace Gestor_Rose_Store.View
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnSeleccionarResumen = new Button();
            lblRutaResumen = new Label();
            btnSeleccionarVentasPropias = new Button();
            lblRutaVentasPropias = new Label();
            lblStatus = new Label();
            btnProcesar = new Button();
            SuspendLayout();
            // 
            // btnSeleccionarResumen
            // 
            btnSeleccionarResumen.Location = new Point(38, 53);
            btnSeleccionarResumen.Name = "btnSeleccionarResumen";
            btnSeleccionarResumen.Size = new Size(220, 49);
            btnSeleccionarResumen.TabIndex = 0;
            btnSeleccionarResumen.Text = "Seleccionar Resumen de Ventas";
            btnSeleccionarResumen.UseVisualStyleBackColor = true;
            // 
            // lblRutaResumen
            // 
            lblRutaResumen.BorderStyle = BorderStyle.FixedSingle;
            lblRutaResumen.Location = new Point(38, 105);
            lblRutaResumen.Name = "lblRutaResumen";
            lblRutaResumen.Size = new Size(220, 25);
            lblRutaResumen.TabIndex = 2;
            lblRutaResumen.Text = "(Ningún archivo seleccionado)";
            // 
            // btnSeleccionarVentasPropias
            // 
            btnSeleccionarVentasPropias.Location = new Point(485, 53);
            btnSeleccionarVentasPropias.Name = "btnSeleccionarVentasPropias";
            btnSeleccionarVentasPropias.Size = new Size(220, 49);
            btnSeleccionarVentasPropias.TabIndex = 3;
            btnSeleccionarVentasPropias.Text = "Seleccionar Ventas Propias";
            btnSeleccionarVentasPropias.UseVisualStyleBackColor = true;
            // 
            // lblRutaVentasPropias
            // 
            lblRutaVentasPropias.BorderStyle = BorderStyle.FixedSingle;
            lblRutaVentasPropias.Location = new Point(485, 105);
            lblRutaVentasPropias.Name = "lblRutaVentasPropias";
            lblRutaVentasPropias.Size = new Size(220, 25);
            lblRutaVentasPropias.TabIndex = 4;
            lblRutaVentasPropias.Text = "(Ningún archivo seleccionado)";
            // 
            // lblStatus
            // 
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.Dock = DockStyle.Bottom;
            lblStatus.Location = new Point(0, 328);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(782, 25);
            lblStatus.TabIndex = 6;
            lblStatus.Text = "Listo.";
            // 
            // btnProcesar
            // 
            btnProcesar.Enabled = false;
            btnProcesar.Location = new Point(254, 186);
            btnProcesar.Name = "btnProcesar";
            btnProcesar.Size = new Size(220, 49);
            btnProcesar.TabIndex = 5;
            btnProcesar.Text = "Procesar y Actualizar Ventas";
            btnProcesar.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(782, 353);
            Controls.Add(lblStatus);
            Controls.Add(btnProcesar);
            Controls.Add(lblRutaVentasPropias);
            Controls.Add(btnSeleccionarVentasPropias);
            Controls.Add(lblRutaResumen);
            Controls.Add(btnSeleccionarResumen);
            Name = "frmMain";
            Text = "Gestor de Concordancia de Ventas Rose Store";
            Load += this.frmMain_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button btnSeleccionarResumen;
        private Label lblRutaResumen;
        private Button btnSeleccionarVentasPropias;
        private Label lblRutaVentasPropias;
        private Label lblStatus;
        private Button btnProcesar;
    }
}
