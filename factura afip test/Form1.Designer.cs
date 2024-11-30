namespace factura_afip_test
{
    partial class Form1
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
            generarTRA = new Button();
            FirmarTRA = new Button();
            solicitudSoap = new Button();
            revisar = new Button();
            SuspendLayout();
            // 
            // generarTRA
            // 
            generarTRA.Location = new Point(542, 25);
            generarTRA.Name = "generarTRA";
            generarTRA.Size = new Size(100, 23);
            generarTRA.TabIndex = 3;
            generarTRA.Text = "GenerarTRA";
            generarTRA.UseVisualStyleBackColor = true;
            generarTRA.Click += button3_Click;
            // 
            // FirmarTRA
            // 
            FirmarTRA.Location = new Point(542, 69);
            FirmarTRA.Name = "FirmarTRA";
            FirmarTRA.Size = new Size(100, 23);
            FirmarTRA.TabIndex = 4;
            FirmarTRA.Text = "Firmar TRA";
            FirmarTRA.UseVisualStyleBackColor = true;
            FirmarTRA.Click += FirmarTRA_Click;
            // 
            // solicitudSoap
            // 
            solicitudSoap.Location = new Point(542, 108);
            solicitudSoap.Name = "solicitudSoap";
            solicitudSoap.Size = new Size(100, 48);
            solicitudSoap.TabIndex = 5;
            solicitudSoap.Text = "Enviar solicitud SOAP";
            solicitudSoap.UseVisualStyleBackColor = true;
            solicitudSoap.Click += solicitudSoap_Click;
            // 
            // revisar
            // 
            revisar.Location = new Point(668, 69);
            revisar.Name = "revisar";
            revisar.Size = new Size(75, 23);
            revisar.TabIndex = 6;
            revisar.Text = "Revisar";
            revisar.UseVisualStyleBackColor = true;
        
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(revisar);
            Controls.Add(solicitudSoap);
            Controls.Add(FirmarTRA);
            Controls.Add(generarTRA);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion
        private Button generarTRA;
        private Button FirmarTRA;
        private Button solicitudSoap;
        private Button revisar;
    }
}
