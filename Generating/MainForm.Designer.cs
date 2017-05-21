namespace Generating
{
    partial class MainForm
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
            this.glControl = new OpenTK.GLControl();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.tbSize = new System.Windows.Forms.TextBox();
            this.tbScale = new System.Windows.Forms.TextBox();
            this.tbRoughness = new System.Windows.Forms.TextBox();
            this.tbFogDensity = new System.Windows.Forms.TextBox();
            this.tbFogStart = new System.Windows.Forms.TextBox();
            this.tbFogEnd = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbFogType = new System.Windows.Forms.ComboBox();
            this.tbWaterSpecIntensity = new System.Windows.Forms.TextBox();
            this.tbWaterSpecPower = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbWaveStrength = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tbWaterSpeed = new System.Windows.Forms.TextBox();
            this.tbLightX = new System.Windows.Forms.TextBox();
            this.tbLightY = new System.Windows.Forms.TextBox();
            this.tbLightZ = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.tbLightAmbientIntensity = new System.Windows.Forms.TextBox();
            this.tbLightSpecIntensity = new System.Windows.Forms.TextBox();
            this.tbLightSpecPower = new System.Windows.Forms.TextBox();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.gbGenerationArgs = new System.Windows.Forms.GroupBox();
            this.gbFogArgs = new System.Windows.Forms.GroupBox();
            this.gbWaterArgs = new System.Windows.Forms.GroupBox();
            this.gbLightArgs = new System.Windows.Forms.GroupBox();
            this.controlPanel.SuspendLayout();
            this.gbGenerationArgs.SuspendLayout();
            this.gbFogArgs.SuspendLayout();
            this.gbWaterArgs.SuspendLayout();
            this.gbLightArgs.SuspendLayout();
            this.SuspendLayout();
            // 
            // glControl
            // 
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl.Location = new System.Drawing.Point(0, 0);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(1111, 619);
            this.glControl.TabIndex = 0;
            this.glControl.VSync = true;
            this.glControl.Load += new System.EventHandler(this.glControl_Load);
            this.glControl.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl_Paint);
            this.glControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseClick);
            this.glControl.Resize += new System.EventHandler(this.glControl_Resize);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(3, 572);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(213, 40);
            this.btnGenerate.TabIndex = 6;
            this.btnGenerate.Text = "Сгенерировать";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Размер:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Масштаб:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Шероховатость:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(11, 19);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 13);
            this.label10.TabIndex = 13;
            this.label10.Text = "Плотность:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(11, 47);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 13);
            this.label11.TabIndex = 14;
            this.label11.Text = "Начало:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(11, 75);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(41, 13);
            this.label12.TabIndex = 15;
            this.label12.Text = "Конец:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(11, 27);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(88, 13);
            this.label15.TabIndex = 18;
            this.label15.Text = "Насыщенность:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(11, 55);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(41, 13);
            this.label16.TabIndex = 19;
            this.label16.Text = "Блеск:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Позиция источника:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(74, 49);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(14, 13);
            this.label17.TabIndex = 22;
            this.label17.Text = "Y";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(11, 49);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(14, 13);
            this.label18.TabIndex = 23;
            this.label18.Text = "X";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(137, 49);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(14, 13);
            this.label19.TabIndex = 24;
            this.label19.Text = "Z";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(11, 77);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(114, 13);
            this.label20.TabIndex = 25;
            this.label20.Text = "Насыщенность фон.:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(11, 105);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(118, 13);
            this.label21.TabIndex = 26;
            this.label21.Text = "Насыщенность зерк.:";
            // 
            // tbSize
            // 
            this.tbSize.Location = new System.Drawing.Point(108, 19);
            this.tbSize.Name = "tbSize";
            this.tbSize.Size = new System.Drawing.Size(86, 20);
            this.tbSize.TabIndex = 29;
            this.tbSize.Leave += new System.EventHandler(this.tbSize_Leave);
            // 
            // tbScale
            // 
            this.tbScale.Location = new System.Drawing.Point(108, 47);
            this.tbScale.Name = "tbScale";
            this.tbScale.Size = new System.Drawing.Size(86, 20);
            this.tbScale.TabIndex = 30;
            this.tbScale.Leave += new System.EventHandler(this.tbScale_Leave);
            // 
            // tbRoughness
            // 
            this.tbRoughness.Location = new System.Drawing.Point(108, 75);
            this.tbRoughness.Name = "tbRoughness";
            this.tbRoughness.Size = new System.Drawing.Size(86, 20);
            this.tbRoughness.TabIndex = 31;
            this.tbRoughness.Leave += new System.EventHandler(this.tbRoughness_Leave);
            // 
            // tbFogDensity
            // 
            this.tbFogDensity.Location = new System.Drawing.Point(108, 16);
            this.tbFogDensity.Name = "tbFogDensity";
            this.tbFogDensity.Size = new System.Drawing.Size(86, 20);
            this.tbFogDensity.TabIndex = 32;
            this.tbFogDensity.Leave += new System.EventHandler(this.tbFogDensity_Leave);
            // 
            // tbFogStart
            // 
            this.tbFogStart.Location = new System.Drawing.Point(108, 44);
            this.tbFogStart.Name = "tbFogStart";
            this.tbFogStart.Size = new System.Drawing.Size(86, 20);
            this.tbFogStart.TabIndex = 33;
            this.tbFogStart.Leave += new System.EventHandler(this.tbFogStart_Leave);
            // 
            // tbFogEnd
            // 
            this.tbFogEnd.Location = new System.Drawing.Point(108, 72);
            this.tbFogEnd.Name = "tbFogEnd";
            this.tbFogEnd.Size = new System.Drawing.Size(86, 20);
            this.tbFogEnd.TabIndex = 34;
            this.tbFogEnd.Leave += new System.EventHandler(this.tbFogEnd_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 35;
            this.label2.Text = "Тип тумана:";
            // 
            // cbFogType
            // 
            this.cbFogType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFogType.FormattingEnabled = true;
            this.cbFogType.Items.AddRange(new object[] {
            "Линейный",
            "Exp",
            "Exp^2"});
            this.cbFogType.Location = new System.Drawing.Point(108, 100);
            this.cbFogType.Name = "cbFogType";
            this.cbFogType.Size = new System.Drawing.Size(86, 21);
            this.cbFogType.TabIndex = 36;
            this.cbFogType.Leave += new System.EventHandler(this.cbFogType_Leave);
            // 
            // tbWaterSpecIntensity
            // 
            this.tbWaterSpecIntensity.Location = new System.Drawing.Point(108, 25);
            this.tbWaterSpecIntensity.Name = "tbWaterSpecIntensity";
            this.tbWaterSpecIntensity.Size = new System.Drawing.Size(86, 20);
            this.tbWaterSpecIntensity.TabIndex = 37;
            this.tbWaterSpecIntensity.TextChanged += new System.EventHandler(this.tbWaterSpecIntensity_TextChanged);
            // 
            // tbWaterSpecPower
            // 
            this.tbWaterSpecPower.Location = new System.Drawing.Point(108, 53);
            this.tbWaterSpecPower.Name = "tbWaterSpecPower";
            this.tbWaterSpecPower.Size = new System.Drawing.Size(86, 20);
            this.tbWaterSpecPower.TabIndex = 38;
            this.tbWaterSpecPower.TextChanged += new System.EventHandler(this.tbWaterSpecPower_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 83);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 13);
            this.label9.TabIndex = 39;
            this.label9.Text = "Сила волн:";
            // 
            // tbWaveStrength
            // 
            this.tbWaveStrength.Location = new System.Drawing.Point(108, 81);
            this.tbWaveStrength.Name = "tbWaveStrength";
            this.tbWaveStrength.Size = new System.Drawing.Size(86, 20);
            this.tbWaveStrength.TabIndex = 40;
            this.tbWaveStrength.TextChanged += new System.EventHandler(this.tbWaveStrength_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(11, 111);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(85, 13);
            this.label13.TabIndex = 41;
            this.label13.Text = "Скорость волн:";
            // 
            // tbWaterSpeed
            // 
            this.tbWaterSpeed.Location = new System.Drawing.Point(108, 109);
            this.tbWaterSpeed.Name = "tbWaterSpeed";
            this.tbWaterSpeed.Size = new System.Drawing.Size(86, 20);
            this.tbWaterSpeed.TabIndex = 42;
            this.tbWaterSpeed.TextChanged += new System.EventHandler(this.tbWaterSpeed_TextChanged);
            // 
            // tbLightX
            // 
            this.tbLightX.Location = new System.Drawing.Point(28, 47);
            this.tbLightX.Name = "tbLightX";
            this.tbLightX.Size = new System.Drawing.Size(40, 20);
            this.tbLightX.TabIndex = 43;
            this.tbLightX.TextChanged += new System.EventHandler(this.tbLightX_TextChanged);
            // 
            // tbLightY
            // 
            this.tbLightY.Location = new System.Drawing.Point(91, 47);
            this.tbLightY.Name = "tbLightY";
            this.tbLightY.Size = new System.Drawing.Size(40, 20);
            this.tbLightY.TabIndex = 44;
            this.tbLightY.TextChanged += new System.EventHandler(this.tbLightY_TextChanged);
            // 
            // tbLightZ
            // 
            this.tbLightZ.Location = new System.Drawing.Point(154, 47);
            this.tbLightZ.Name = "tbLightZ";
            this.tbLightZ.Size = new System.Drawing.Size(40, 20);
            this.tbLightZ.TabIndex = 45;
            this.tbLightZ.TextChanged += new System.EventHandler(this.tbLightZ_TextChanged);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(11, 133);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(41, 13);
            this.label24.TabIndex = 46;
            this.label24.Text = "Блеск:";
            // 
            // tbLightAmbientIntensity
            // 
            this.tbLightAmbientIntensity.Location = new System.Drawing.Point(131, 75);
            this.tbLightAmbientIntensity.Name = "tbLightAmbientIntensity";
            this.tbLightAmbientIntensity.Size = new System.Drawing.Size(63, 20);
            this.tbLightAmbientIntensity.TabIndex = 47;
            this.tbLightAmbientIntensity.TextChanged += new System.EventHandler(this.tbLightAmbientIntensity_TextChanged);
            // 
            // tbLightSpecIntensity
            // 
            this.tbLightSpecIntensity.Location = new System.Drawing.Point(131, 103);
            this.tbLightSpecIntensity.Name = "tbLightSpecIntensity";
            this.tbLightSpecIntensity.Size = new System.Drawing.Size(63, 20);
            this.tbLightSpecIntensity.TabIndex = 48;
            this.tbLightSpecIntensity.TextChanged += new System.EventHandler(this.tbLightSpecIntensity_TextChanged);
            // 
            // tbLightSpecPower
            // 
            this.tbLightSpecPower.Location = new System.Drawing.Point(131, 131);
            this.tbLightSpecPower.Name = "tbLightSpecPower";
            this.tbLightSpecPower.Size = new System.Drawing.Size(63, 20);
            this.tbLightSpecPower.TabIndex = 49;
            this.tbLightSpecPower.TextChanged += new System.EventHandler(this.tbLightSpecPower_TextChanged);
            // 
            // controlPanel
            // 
            this.controlPanel.BackColor = System.Drawing.Color.Transparent;
            this.controlPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.controlPanel.Controls.Add(this.gbLightArgs);
            this.controlPanel.Controls.Add(this.gbWaterArgs);
            this.controlPanel.Controls.Add(this.gbFogArgs);
            this.controlPanel.Controls.Add(this.gbGenerationArgs);
            this.controlPanel.Controls.Add(this.btnGenerate);
            this.controlPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.controlPanel.Location = new System.Drawing.Point(888, 0);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(223, 619);
            this.controlPanel.TabIndex = 50;
            // 
            // gbGenerationArgs
            // 
            this.gbGenerationArgs.Controls.Add(this.tbScale);
            this.gbGenerationArgs.Controls.Add(this.tbSize);
            this.gbGenerationArgs.Controls.Add(this.tbRoughness);
            this.gbGenerationArgs.Controls.Add(this.label7);
            this.gbGenerationArgs.Controls.Add(this.label6);
            this.gbGenerationArgs.Controls.Add(this.label5);
            this.gbGenerationArgs.Location = new System.Drawing.Point(3, 3);
            this.gbGenerationArgs.Name = "gbGenerationArgs";
            this.gbGenerationArgs.Size = new System.Drawing.Size(213, 107);
            this.gbGenerationArgs.TabIndex = 51;
            this.gbGenerationArgs.TabStop = false;
            this.gbGenerationArgs.Text = "Параметры генерации";
            // 
            // gbFogArgs
            // 
            this.gbFogArgs.Controls.Add(this.tbFogStart);
            this.gbFogArgs.Controls.Add(this.tbFogDensity);
            this.gbFogArgs.Controls.Add(this.tbFogEnd);
            this.gbFogArgs.Controls.Add(this.label2);
            this.gbFogArgs.Controls.Add(this.cbFogType);
            this.gbFogArgs.Controls.Add(this.label12);
            this.gbFogArgs.Controls.Add(this.label11);
            this.gbFogArgs.Controls.Add(this.label10);
            this.gbFogArgs.Location = new System.Drawing.Point(3, 116);
            this.gbFogArgs.Name = "gbFogArgs";
            this.gbFogArgs.Size = new System.Drawing.Size(213, 131);
            this.gbFogArgs.TabIndex = 51;
            this.gbFogArgs.TabStop = false;
            this.gbFogArgs.Text = "Параметры тумана";
            // 
            // gbWaterArgs
            // 
            this.gbWaterArgs.Controls.Add(this.tbWaveStrength);
            this.gbWaterArgs.Controls.Add(this.tbWaterSpecIntensity);
            this.gbWaterArgs.Controls.Add(this.label16);
            this.gbWaterArgs.Controls.Add(this.tbWaterSpecPower);
            this.gbWaterArgs.Controls.Add(this.label15);
            this.gbWaterArgs.Controls.Add(this.label9);
            this.gbWaterArgs.Controls.Add(this.label13);
            this.gbWaterArgs.Controls.Add(this.tbWaterSpeed);
            this.gbWaterArgs.Location = new System.Drawing.Point(3, 253);
            this.gbWaterArgs.Name = "gbWaterArgs";
            this.gbWaterArgs.Size = new System.Drawing.Size(213, 139);
            this.gbWaterArgs.TabIndex = 51;
            this.gbWaterArgs.TabStop = false;
            this.gbWaterArgs.Text = "Параметры воды";
            // 
            // gbLightArgs
            // 
            this.gbLightArgs.Controls.Add(this.tbLightAmbientIntensity);
            this.gbLightArgs.Controls.Add(this.label21);
            this.gbLightArgs.Controls.Add(this.label20);
            this.gbLightArgs.Controls.Add(this.label19);
            this.gbLightArgs.Controls.Add(this.tbLightSpecPower);
            this.gbLightArgs.Controls.Add(this.label18);
            this.gbLightArgs.Controls.Add(this.label17);
            this.gbLightArgs.Controls.Add(this.tbLightSpecIntensity);
            this.gbLightArgs.Controls.Add(this.label3);
            this.gbLightArgs.Controls.Add(this.tbLightX);
            this.gbLightArgs.Controls.Add(this.label24);
            this.gbLightArgs.Controls.Add(this.tbLightY);
            this.gbLightArgs.Controls.Add(this.tbLightZ);
            this.gbLightArgs.Location = new System.Drawing.Point(3, 398);
            this.gbLightArgs.Name = "gbLightArgs";
            this.gbLightArgs.Size = new System.Drawing.Size(213, 166);
            this.gbLightArgs.TabIndex = 51;
            this.gbLightArgs.TabStop = false;
            this.gbLightArgs.Text = "Параметры света";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1111, 619);
            this.Controls.Add(this.controlPanel);
            this.Controls.Add(this.glControl);
            this.MinimumSize = new System.Drawing.Size(1127, 658);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.controlPanel.ResumeLayout(false);
            this.gbGenerationArgs.ResumeLayout(false);
            this.gbGenerationArgs.PerformLayout();
            this.gbFogArgs.ResumeLayout(false);
            this.gbFogArgs.PerformLayout();
            this.gbWaterArgs.ResumeLayout(false);
            this.gbWaterArgs.PerformLayout();
            this.gbLightArgs.ResumeLayout(false);
            this.gbLightArgs.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private OpenTK.GLControl glControl;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox tbSize;
        private System.Windows.Forms.TextBox tbScale;
        private System.Windows.Forms.TextBox tbRoughness;
        private System.Windows.Forms.TextBox tbFogDensity;
        private System.Windows.Forms.TextBox tbFogStart;
        private System.Windows.Forms.TextBox tbFogEnd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbFogType;
        private System.Windows.Forms.TextBox tbWaterSpecIntensity;
        private System.Windows.Forms.TextBox tbWaterSpecPower;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbWaveStrength;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbWaterSpeed;
        private System.Windows.Forms.TextBox tbLightX;
        private System.Windows.Forms.TextBox tbLightY;
        private System.Windows.Forms.TextBox tbLightZ;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox tbLightAmbientIntensity;
        private System.Windows.Forms.TextBox tbLightSpecIntensity;
        private System.Windows.Forms.TextBox tbLightSpecPower;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.GroupBox gbLightArgs;
        private System.Windows.Forms.GroupBox gbWaterArgs;
        private System.Windows.Forms.GroupBox gbFogArgs;
        private System.Windows.Forms.GroupBox gbGenerationArgs;
    }
}