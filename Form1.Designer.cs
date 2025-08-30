namespace SoD2_Editor
{
    partial class Form1
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
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblVer = new System.Windows.Forms.Label();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabWorld = new System.Windows.Forms.TabPage();
            this.chkIsDemo = new System.Windows.Forms.CheckBox();
            this.lblWorldName = new System.Windows.Forms.Label();
            this.btnTimeDilationPlus = new System.Windows.Forms.Button();
            this.btnTimeDilationMinus = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTimeDilation = new System.Windows.Forms.TextBox();
            this.tabEnclaves = new System.Windows.Forms.TabPage();
            this.tabControlEnclaves = new System.Windows.Forms.TabControl();
            this.tabEnclaveDetails = new System.Windows.Forms.TabPage();
            this.lblEnclaveDetails = new System.Windows.Forms.Label();
            this.txtEnclaveAddress = new System.Windows.Forms.TextBox();
            this.tabEnclaveCharacters = new System.Windows.Forms.TabPage();
            this.lblCharacterDetails = new System.Windows.Forms.Label();
            this.txtCharacterAddress = new System.Windows.Forms.TextBox();
            this.flowEnclaveCharacters = new System.Windows.Forms.FlowLayoutPanel();
            this.lblEnclavesNumCharacters = new System.Windows.Forms.Label();
            this.flowEnclaves = new System.Windows.Forms.FlowLayoutPanel();
            this.lblNumEnclaves = new System.Windows.Forms.Label();
            this.lblEnclavesLabel = new System.Windows.Forms.Label();
            this.tabSpawner = new System.Windows.Forms.TabPage();
            this.lblSpawnerLabel = new System.Windows.Forms.Label();
            this.lblSpawnerDetails = new System.Windows.Forms.Label();
            this.tabs.SuspendLayout();
            this.tabWorld.SuspendLayout();
            this.tabEnclaves.SuspendLayout();
            this.tabControlEnclaves.SuspendLayout();
            this.tabEnclaveDetails.SuspendLayout();
            this.tabEnclaveCharacters.SuspendLayout();
            this.tabSpawner.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblVer
            // 
            this.lblVer.AutoSize = true;
            this.lblVer.Location = new System.Drawing.Point(93, 17);
            this.lblVer.Name = "lblVer";
            this.lblVer.Size = new System.Drawing.Size(78, 13);
            this.lblVer.TabIndex = 1;
            this.lblVer.Text = "Not connected";
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Controls.Add(this.tabWorld);
            this.tabs.Controls.Add(this.tabEnclaves);
            this.tabs.Controls.Add(this.tabSpawner);
            this.tabs.Location = new System.Drawing.Point(12, 41);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(901, 743);
            this.tabs.TabIndex = 3;
            // 
            // tabWorld
            // 
            this.tabWorld.Controls.Add(this.chkIsDemo);
            this.tabWorld.Controls.Add(this.lblWorldName);
            this.tabWorld.Controls.Add(this.btnTimeDilationPlus);
            this.tabWorld.Controls.Add(this.btnTimeDilationMinus);
            this.tabWorld.Controls.Add(this.label1);
            this.tabWorld.Controls.Add(this.txtTimeDilation);
            this.tabWorld.Location = new System.Drawing.Point(4, 22);
            this.tabWorld.Name = "tabWorld";
            this.tabWorld.Padding = new System.Windows.Forms.Padding(3);
            this.tabWorld.Size = new System.Drawing.Size(893, 717);
            this.tabWorld.TabIndex = 0;
            this.tabWorld.Text = "World";
            this.tabWorld.UseVisualStyleBackColor = true;
            // 
            // chkIsDemo
            // 
            this.chkIsDemo.AutoSize = true;
            this.chkIsDemo.Location = new System.Drawing.Point(80, 23);
            this.chkIsDemo.Name = "chkIsDemo";
            this.chkIsDemo.Size = new System.Drawing.Size(355, 17);
            this.chkIsDemo.TabIndex = 6;
            this.chkIsDemo.Text = "UL.IsDemo (WARNING:  Will reset some Options/Settings to defaults)";
            this.chkIsDemo.UseVisualStyleBackColor = true;
            // 
            // lblWorldName
            // 
            this.lblWorldName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblWorldName.AutoSize = true;
            this.lblWorldName.Location = new System.Drawing.Point(10, 666);
            this.lblWorldName.Name = "lblWorldName";
            this.lblWorldName.Size = new System.Drawing.Size(58, 13);
            this.lblWorldName.TabIndex = 5;
            this.lblWorldName.Text = "worldLabel";
            // 
            // btnTimeDilationPlus
            // 
            this.btnTimeDilationPlus.Location = new System.Drawing.Point(212, 44);
            this.btnTimeDilationPlus.Name = "btnTimeDilationPlus";
            this.btnTimeDilationPlus.Size = new System.Drawing.Size(20, 23);
            this.btnTimeDilationPlus.TabIndex = 4;
            this.btnTimeDilationPlus.Text = "+";
            this.btnTimeDilationPlus.UseVisualStyleBackColor = true;
            this.btnTimeDilationPlus.Click += new System.EventHandler(this.btnTimeDilationPlus_Click);
            // 
            // btnTimeDilationMinus
            // 
            this.btnTimeDilationMinus.Location = new System.Drawing.Point(186, 44);
            this.btnTimeDilationMinus.Name = "btnTimeDilationMinus";
            this.btnTimeDilationMinus.Size = new System.Drawing.Size(20, 23);
            this.btnTimeDilationMinus.TabIndex = 3;
            this.btnTimeDilationMinus.Text = "-";
            this.btnTimeDilationMinus.UseVisualStyleBackColor = true;
            this.btnTimeDilationMinus.Click += new System.EventHandler(this.btnTimeDilationMinus_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Time Dilation";
            // 
            // txtTimeDilation
            // 
            this.txtTimeDilation.Location = new System.Drawing.Point(80, 46);
            this.txtTimeDilation.Name = "txtTimeDilation";
            this.txtTimeDilation.Size = new System.Drawing.Size(100, 20);
            this.txtTimeDilation.TabIndex = 0;
            // 
            // tabEnclaves
            // 
            this.tabEnclaves.Controls.Add(this.tabControlEnclaves);
            this.tabEnclaves.Controls.Add(this.flowEnclaves);
            this.tabEnclaves.Controls.Add(this.lblNumEnclaves);
            this.tabEnclaves.Controls.Add(this.lblEnclavesLabel);
            this.tabEnclaves.Location = new System.Drawing.Point(4, 22);
            this.tabEnclaves.Name = "tabEnclaves";
            this.tabEnclaves.Padding = new System.Windows.Forms.Padding(3);
            this.tabEnclaves.Size = new System.Drawing.Size(893, 717);
            this.tabEnclaves.TabIndex = 1;
            this.tabEnclaves.Text = "Enclaves";
            this.tabEnclaves.UseVisualStyleBackColor = true;
            // 
            // tabControlEnclaves
            // 
            this.tabControlEnclaves.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlEnclaves.Controls.Add(this.tabEnclaveDetails);
            this.tabControlEnclaves.Controls.Add(this.tabEnclaveCharacters);
            this.tabControlEnclaves.Location = new System.Drawing.Point(144, 13);
            this.tabControlEnclaves.Name = "tabControlEnclaves";
            this.tabControlEnclaves.SelectedIndex = 0;
            this.tabControlEnclaves.Size = new System.Drawing.Size(743, 698);
            this.tabControlEnclaves.TabIndex = 11;
            // 
            // tabEnclaveDetails
            // 
            this.tabEnclaveDetails.Controls.Add(this.lblEnclaveDetails);
            this.tabEnclaveDetails.Controls.Add(this.txtEnclaveAddress);
            this.tabEnclaveDetails.Location = new System.Drawing.Point(4, 22);
            this.tabEnclaveDetails.Name = "tabEnclaveDetails";
            this.tabEnclaveDetails.Padding = new System.Windows.Forms.Padding(3);
            this.tabEnclaveDetails.Size = new System.Drawing.Size(735, 672);
            this.tabEnclaveDetails.TabIndex = 0;
            this.tabEnclaveDetails.Text = "Details";
            this.tabEnclaveDetails.UseVisualStyleBackColor = true;
            // 
            // lblEnclaveDetails
            // 
            this.lblEnclaveDetails.AutoSize = true;
            this.lblEnclaveDetails.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEnclaveDetails.Location = new System.Drawing.Point(3, 32);
            this.lblEnclaveDetails.Name = "lblEnclaveDetails";
            this.lblEnclaveDetails.Size = new System.Drawing.Size(91, 13);
            this.lblEnclaveDetails.TabIndex = 12;
            this.lblEnclaveDetails.Text = "enclaveDetails";
            // 
            // txtEnclaveAddress
            // 
            this.txtEnclaveAddress.Enabled = false;
            this.txtEnclaveAddress.Location = new System.Drawing.Point(6, 6);
            this.txtEnclaveAddress.Name = "txtEnclaveAddress";
            this.txtEnclaveAddress.Size = new System.Drawing.Size(107, 20);
            this.txtEnclaveAddress.TabIndex = 11;
            this.txtEnclaveAddress.Text = "0";
            // 
            // tabEnclaveCharacters
            // 
            this.tabEnclaveCharacters.Controls.Add(this.lblCharacterDetails);
            this.tabEnclaveCharacters.Controls.Add(this.txtCharacterAddress);
            this.tabEnclaveCharacters.Controls.Add(this.flowEnclaveCharacters);
            this.tabEnclaveCharacters.Controls.Add(this.lblEnclavesNumCharacters);
            this.tabEnclaveCharacters.Location = new System.Drawing.Point(4, 22);
            this.tabEnclaveCharacters.Name = "tabEnclaveCharacters";
            this.tabEnclaveCharacters.Padding = new System.Windows.Forms.Padding(3);
            this.tabEnclaveCharacters.Size = new System.Drawing.Size(735, 672);
            this.tabEnclaveCharacters.TabIndex = 1;
            this.tabEnclaveCharacters.Text = "Characters";
            this.tabEnclaveCharacters.UseVisualStyleBackColor = true;
            // 
            // lblCharacterDetails
            // 
            this.lblCharacterDetails.AutoSize = true;
            this.lblCharacterDetails.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCharacterDetails.Location = new System.Drawing.Point(144, 35);
            this.lblCharacterDetails.Name = "lblCharacterDetails";
            this.lblCharacterDetails.Size = new System.Drawing.Size(103, 13);
            this.lblCharacterDetails.TabIndex = 14;
            this.lblCharacterDetails.Text = "characterDetails";
            // 
            // txtCharacterAddress
            // 
            this.txtCharacterAddress.Enabled = false;
            this.txtCharacterAddress.Location = new System.Drawing.Point(147, 9);
            this.txtCharacterAddress.Name = "txtCharacterAddress";
            this.txtCharacterAddress.Size = new System.Drawing.Size(107, 20);
            this.txtCharacterAddress.TabIndex = 13;
            this.txtCharacterAddress.Text = "0";
            // 
            // flowEnclaveCharacters
            // 
            this.flowEnclaveCharacters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flowEnclaveCharacters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowEnclaveCharacters.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowEnclaveCharacters.Location = new System.Drawing.Point(9, 28);
            this.flowEnclaveCharacters.Name = "flowEnclaveCharacters";
            this.flowEnclaveCharacters.Size = new System.Drawing.Size(132, 536);
            this.flowEnclaveCharacters.TabIndex = 9;
            // 
            // lblEnclavesNumCharacters
            // 
            this.lblEnclavesNumCharacters.AutoSize = true;
            this.lblEnclavesNumCharacters.Location = new System.Drawing.Point(12, 12);
            this.lblEnclavesNumCharacters.Name = "lblEnclavesNumCharacters";
            this.lblEnclavesNumCharacters.Size = new System.Drawing.Size(66, 13);
            this.lblEnclavesNumCharacters.TabIndex = 8;
            this.lblEnclavesNumCharacters.Text = "x Characters";
            // 
            // flowEnclaves
            // 
            this.flowEnclaves.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flowEnclaves.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowEnclaves.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowEnclaves.Location = new System.Drawing.Point(6, 29);
            this.flowEnclaves.Name = "flowEnclaves";
            this.flowEnclaves.Size = new System.Drawing.Size(132, 634);
            this.flowEnclaves.TabIndex = 8;
            // 
            // lblNumEnclaves
            // 
            this.lblNumEnclaves.AutoSize = true;
            this.lblNumEnclaves.Location = new System.Drawing.Point(10, 13);
            this.lblNumEnclaves.Name = "lblNumEnclaves";
            this.lblNumEnclaves.Size = new System.Drawing.Size(59, 13);
            this.lblNumEnclaves.TabIndex = 7;
            this.lblNumEnclaves.Text = "x Enclaves";
            // 
            // lblEnclavesLabel
            // 
            this.lblEnclavesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblEnclavesLabel.AutoSize = true;
            this.lblEnclavesLabel.Location = new System.Drawing.Point(10, 666);
            this.lblEnclavesLabel.Name = "lblEnclavesLabel";
            this.lblEnclavesLabel.Size = new System.Drawing.Size(76, 13);
            this.lblEnclavesLabel.TabIndex = 6;
            this.lblEnclavesLabel.Text = "enclavesLabel";
            // 
            // tabSpawner
            // 
            this.tabSpawner.Controls.Add(this.lblSpawnerDetails);
            this.tabSpawner.Controls.Add(this.lblSpawnerLabel);
            this.tabSpawner.Location = new System.Drawing.Point(4, 22);
            this.tabSpawner.Name = "tabSpawner";
            this.tabSpawner.Size = new System.Drawing.Size(893, 717);
            this.tabSpawner.TabIndex = 2;
            this.tabSpawner.Text = "Spawner";
            this.tabSpawner.UseVisualStyleBackColor = true;
            // 
            // lblSpawnerLabel
            // 
            this.lblSpawnerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSpawnerLabel.AutoSize = true;
            this.lblSpawnerLabel.Location = new System.Drawing.Point(10, 666);
            this.lblSpawnerLabel.Name = "lblSpawnerLabel";
            this.lblSpawnerLabel.Size = new System.Drawing.Size(73, 13);
            this.lblSpawnerLabel.TabIndex = 6;
            this.lblSpawnerLabel.Text = "spawnerLabel";
            // 
            // lblSpawnerDetails
            // 
            this.lblSpawnerDetails.AutoSize = true;
            this.lblSpawnerDetails.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpawnerDetails.Location = new System.Drawing.Point(128, 59);
            this.lblSpawnerDetails.Name = "lblSpawnerDetails";
            this.lblSpawnerDetails.Size = new System.Drawing.Size(91, 13);
            this.lblSpawnerDetails.TabIndex = 13;
            this.lblSpawnerDetails.Text = "spawnerDetails";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(925, 796);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.lblVer);
            this.Controls.Add(this.btnConnect);
            this.Name = "Form1";
            this.Text = "SoDEaD - Editor and Dataviewer";
            this.tabs.ResumeLayout(false);
            this.tabWorld.ResumeLayout(false);
            this.tabWorld.PerformLayout();
            this.tabEnclaves.ResumeLayout(false);
            this.tabEnclaves.PerformLayout();
            this.tabControlEnclaves.ResumeLayout(false);
            this.tabEnclaveDetails.ResumeLayout(false);
            this.tabEnclaveDetails.PerformLayout();
            this.tabEnclaveCharacters.ResumeLayout(false);
            this.tabEnclaveCharacters.PerformLayout();
            this.tabSpawner.ResumeLayout(false);
            this.tabSpawner.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblVer;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabWorld;
        private System.Windows.Forms.TabPage tabEnclaves;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTimeDilation;
        private System.Windows.Forms.Button btnTimeDilationPlus;
        private System.Windows.Forms.Button btnTimeDilationMinus;
        private System.Windows.Forms.Label lblWorldName;
        private System.Windows.Forms.Label lblEnclavesLabel;
        private System.Windows.Forms.Label lblNumEnclaves;
        private System.Windows.Forms.FlowLayoutPanel flowEnclaves;
        private System.Windows.Forms.TabControl tabControlEnclaves;
        private System.Windows.Forms.TabPage tabEnclaveDetails;
        private System.Windows.Forms.Label lblEnclaveDetails;
        private System.Windows.Forms.TextBox txtEnclaveAddress;
        private System.Windows.Forms.TabPage tabEnclaveCharacters;
        private System.Windows.Forms.Label lblCharacterDetails;
        private System.Windows.Forms.TextBox txtCharacterAddress;
        private System.Windows.Forms.FlowLayoutPanel flowEnclaveCharacters;
        private System.Windows.Forms.Label lblEnclavesNumCharacters;
        private System.Windows.Forms.CheckBox chkIsDemo;
        private System.Windows.Forms.TabPage tabSpawner;
        private System.Windows.Forms.Label lblSpawnerDetails;
        private System.Windows.Forms.Label lblSpawnerLabel;
    }
}

