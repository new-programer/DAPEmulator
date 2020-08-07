namespace DAPClient
{
    partial class client
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ipAddress = new System.Windows.Forms.TextBox();
            this.port = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.clientLog = new System.Windows.Forms.RichTextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.dap_factor = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.version = new System.Windows.Forms.TextBox();
            this.sn = new System.Windows.Forms.TextBox();
            this.factor = new System.Windows.Forms.TextBox();
            this.pressure = new System.Windows.Forms.TextBox();
            this.T = new System.Windows.Forms.TextBox();
            this.btn_calibration = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(19, 26);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "ip（DAP_emulator）：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(417, 26);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "port：";
            // 
            // ipAddress
            // 
            this.ipAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ipAddress.Location = new System.Drawing.Point(215, 23);
            this.ipAddress.Margin = new System.Windows.Forms.Padding(4);
            this.ipAddress.Name = "ipAddress";
            this.ipAddress.Size = new System.Drawing.Size(194, 30);
            this.ipAddress.TabIndex = 2;
            // 
            // port
            // 
            this.port.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.port.Location = new System.Drawing.Point(471, 23);
            this.port.Margin = new System.Windows.Forms.Padding(4);
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(100, 30);
            this.port.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(706, 14);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(173, 42);
            this.button1.TabIndex = 4;
            this.button1.Text = "connection";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.CreateConnection);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(22, 356);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 29);
            this.label3.TabIndex = 6;
            this.label3.Text = "comm log：";
            // 
            // clientLog
            // 
            this.clientLog.Location = new System.Drawing.Point(24, 284);
            this.clientLog.Margin = new System.Windows.Forms.Padding(4);
            this.clientLog.Name = "clientLog";
            this.clientLog.Size = new System.Drawing.Size(855, 265);
            this.clientLog.TabIndex = 7;
            this.clientLog.Text = "";
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.button3.Location = new System.Drawing.Point(706, 62);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(173, 43);
            this.button3.TabIndex = 10;
            this.button3.Text = "stability check";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.SendMsg);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(162, 130);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(0, 17);
            this.label6.TabIndex = 16;
            // 
            // dap_factor
            // 
            this.dap_factor.AutoSize = true;
            this.dap_factor.Location = new System.Drawing.Point(163, 125);
            this.dap_factor.Name = "dap_factor";
            this.dap_factor.Size = new System.Drawing.Size(0, 17);
            this.dap_factor.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(67, 119);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(116, 25);
            this.label7.TabIndex = 18;
            this.label7.Text = "s/w version:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(68, 145);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(115, 25);
            this.label8.TabIndex = 18;
            this.label8.Text = "Internal SN:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(22, 171);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(161, 25);
            this.label9.TabIndex = 18;
            this.label9.Text = "Correction factor:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(60, 197);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(123, 25);
            this.label10.TabIndex = 18;
            this.label10.Text = "Air pressure:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(53, 223);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(130, 25);
            this.label11.TabIndex = 18;
            this.label11.Text = "Temperature:";
            // 
            // version
            // 
            this.version.Location = new System.Drawing.Point(197, 122);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(222, 22);
            this.version.TabIndex = 15;
            // 
            // sn
            // 
            this.sn.Location = new System.Drawing.Point(197, 148);
            this.sn.Name = "sn";
            this.sn.Size = new System.Drawing.Size(222, 22);
            this.sn.TabIndex = 15;
            // 
            // factor
            // 
            this.factor.Location = new System.Drawing.Point(197, 174);
            this.factor.Name = "factor";
            this.factor.Size = new System.Drawing.Size(222, 22);
            this.factor.TabIndex = 15;
            // 
            // pressure
            // 
            this.pressure.Location = new System.Drawing.Point(197, 201);
            this.pressure.Name = "pressure";
            this.pressure.Size = new System.Drawing.Size(222, 22);
            this.pressure.TabIndex = 15;
            // 
            // T
            // 
            this.T.Location = new System.Drawing.Point(197, 226);
            this.T.Name = "T";
            this.T.Size = new System.Drawing.Size(222, 22);
            this.T.TabIndex = 15;
            // 
            // btn_calibration
            // 
            this.btn_calibration.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_calibration.Location = new System.Drawing.Point(670, 193);
            this.btn_calibration.Name = "btn_calibration";
            this.btn_calibration.Size = new System.Drawing.Size(195, 32);
            this.btn_calibration.TabIndex = 11;
            this.btn_calibration.Text = "Calibration Validate";
            this.btn_calibration.UseVisualStyleBackColor = true;
            this.btn_calibration.Click += new System.EventHandler(this.CalibrationValidate);
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.Location = new System.Drawing.Point(472, 193);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(85, 32);
            this.button6.TabIndex = 11;
            this.button6.Text = "Reset";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.Reset);
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.Location = new System.Drawing.Point(563, 193);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(101, 32);
            this.button7.TabIndex = 11;
            this.button7.Text = "Save PT";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.SavePT);
            // 
            // button8
            // 
            this.button8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button8.Location = new System.Drawing.Point(603, 148);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(262, 32);
            this.button8.TabIndex = 11;
            this.button8.Text = "Save Correction Factor";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.SaveCorrectionFactor);
            // 
            // button9
            // 
            this.button9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button9.Location = new System.Drawing.Point(472, 148);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(124, 32);
            this.button9.TabIndex = 11;
            this.button9.Text = "Load Data";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.LoadData);
            // 
            // client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(902, 562);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.dap_factor);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.T);
            this.Controls.Add(this.pressure);
            this.Controls.Add(this.factor);
            this.Controls.Add(this.sn);
            this.Controls.Add(this.version);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.btn_calibration);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.clientLog);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.port);
            this.Controls.Add(this.ipAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "client";
            this.Text = "client";
            this.Load += new System.EventHandler(this.client_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ipAddress;
        private System.Windows.Forms.TextBox port;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox clientLog;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label dap_factor;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox version;
        private System.Windows.Forms.TextBox sn;
        private System.Windows.Forms.TextBox factor;
        private System.Windows.Forms.TextBox pressure;
        private System.Windows.Forms.TextBox T;
        private System.Windows.Forms.Button btn_calibration;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
    }
}

