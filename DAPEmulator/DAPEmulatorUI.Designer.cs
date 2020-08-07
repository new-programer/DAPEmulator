namespace DAPEmulator
{
    partial class DAPEmulatorUI
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
            this.ip = new System.Windows.Forms.TextBox();
            this.port = new System.Windows.Forms.TextBox();
            this.listenStart = new System.Windows.Forms.Button();
            this.listenStop = new System.Windows.Forms.Button();
            this.msgReceive = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.factor = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.version = new System.Windows.Forms.Label();
            this.sn = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ip
            // 
            this.ip.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ip.Location = new System.Drawing.Point(68, 23);
            this.ip.Margin = new System.Windows.Forms.Padding(4);
            this.ip.Name = "ip";
            this.ip.Size = new System.Drawing.Size(185, 30);
            this.ip.TabIndex = 0;
            // 
            // port
            // 
            this.port.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.port.Location = new System.Drawing.Point(324, 23);
            this.port.Margin = new System.Windows.Forms.Padding(4);
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(132, 30);
            this.port.TabIndex = 2;
            // 
            // listenStart
            // 
            this.listenStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listenStart.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.listenStart.Location = new System.Drawing.Point(475, 21);
            this.listenStart.Margin = new System.Windows.Forms.Padding(4);
            this.listenStart.Name = "listenStart";
            this.listenStart.Size = new System.Drawing.Size(173, 30);
            this.listenStart.TabIndex = 4;
            this.listenStart.Text = "start emulator";
            this.listenStart.UseVisualStyleBackColor = true;
            this.listenStart.Click += new System.EventHandler(this.StartListen);
            // 
            // listenStop
            // 
            this.listenStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listenStop.ForeColor = System.Drawing.Color.Red;
            this.listenStop.Location = new System.Drawing.Point(660, 21);
            this.listenStop.Margin = new System.Windows.Forms.Padding(4);
            this.listenStop.Name = "listenStop";
            this.listenStop.Size = new System.Drawing.Size(183, 30);
            this.listenStop.TabIndex = 5;
            this.listenStop.Text = "close emulator";
            this.listenStop.UseVisualStyleBackColor = true;
            this.listenStop.Click += new System.EventHandler(this.StopListen);
            // 
            // msgReceive
            // 
            this.msgReceive.Location = new System.Drawing.Point(37, 192);
            this.msgReceive.Margin = new System.Windows.Forms.Padding(4);
            this.msgReceive.Name = "msgReceive";
            this.msgReceive.Size = new System.Drawing.Size(815, 294);
            this.msgReceive.TabIndex = 9;
            this.msgReceive.Text = "";
            this.msgReceive.TextChanged += new System.EventHandler(this.msgReceive_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(32, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 25);
            this.label1.TabIndex = 11;
            this.label1.Text = "ip:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(270, 25);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 25);
            this.label2.TabIndex = 12;
            this.label2.Text = "port:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(32, 156);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 25);
            this.label3.TabIndex = 13;
            this.label3.Text = "log:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(32, 118);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(161, 25);
            this.label4.TabIndex = 11;
            this.label4.Text = "Correction factor:";
            // 
            // factor
            // 
            this.factor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.factor.Location = new System.Drawing.Point(207, 115);
            this.factor.Margin = new System.Windows.Forms.Padding(4);
            this.factor.Name = "factor";
            this.factor.Size = new System.Drawing.Size(132, 30);
            this.factor.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(32, 71);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(116, 25);
            this.label7.TabIndex = 19;
            this.label7.Text = "s/w version:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(418, 72);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(115, 25);
            this.label8.TabIndex = 20;
            this.label8.Text = "Internal SN:";
            // 
            // version
            // 
            this.version.AutoSize = true;
            this.version.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.version.Location = new System.Drawing.Point(145, 74);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(63, 20);
            this.version.TabIndex = 19;
            this.version.Text = "version";
            // 
            // sn
            // 
            this.sn.AutoSize = true;
            this.sn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sn.Location = new System.Drawing.Point(537, 74);
            this.sn.Name = "sn";
            this.sn.Size = new System.Drawing.Size(92, 20);
            this.sn.TabIndex = 19;
            this.sn.Text = "Internal SN";
            // 
            // DAPEmulatorUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(887, 498);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.version);
            this.Controls.Add(this.sn);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.msgReceive);
            this.Controls.Add(this.listenStop);
            this.Controls.Add(this.listenStart);
            this.Controls.Add(this.factor);
            this.Controls.Add(this.port);
            this.Controls.Add(this.ip);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DAPEmulatorUI";
            this.Text = "DAPEmulatorUI";
            this.Load += new System.EventHandler(this.DAPEmulatorUI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox ip;
        public System.Windows.Forms.TextBox port;
        private System.Windows.Forms.Button listenStart;
        private System.Windows.Forms.Button listenStop;
        public System.Windows.Forms.RichTextBox msgReceive;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox factor;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.Label version;
        public System.Windows.Forms.Label sn;
    }
}

