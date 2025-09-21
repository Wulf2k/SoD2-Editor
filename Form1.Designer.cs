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
            this.tabControlEnclavesCharacters = new System.Windows.Forms.TabControl();
            this.tabEnclavesCharactersDetails = new System.Windows.Forms.TabPage();
            this.tlpEnclavesCharactersDetails = new System.Windows.Forms.TableLayoutPanel();
            this.tabEnclavesCharactersSkills = new System.Windows.Forms.TabPage();
            this.tlpEnclavesCharactersSkills = new System.Windows.Forms.TableLayoutPanel();
            this.tabEnclavesCharactersEquipment = new System.Windows.Forms.TabPage();
            this.tabControlEnclavesCharactersEquipment = new System.Windows.Forms.TabControl();
            this.tabControlEnclavesCharactersEquipmentMelee = new System.Windows.Forms.TabPage();
            this.tlpMeleeWeaponStats = new System.Windows.Forms.TableLayoutPanel();
            this.tabControlEnclavesCharactersEquipmentSideArm = new System.Windows.Forms.TabPage();
            this.tlpSideArmWeaponStats = new System.Windows.Forms.TableLayoutPanel();
            this.tabControlEnclavesCharactersEquipmentRanged = new System.Windows.Forms.TabPage();
            this.tlpRangedWeaponStats = new System.Windows.Forms.TableLayoutPanel();
            this.txtCharacterAddress = new System.Windows.Forms.TextBox();
            this.flowEnclaveCharacters = new System.Windows.Forms.FlowLayoutPanel();
            this.lblEnclavesNumCharacters = new System.Windows.Forms.Label();
            this.flowEnclaves = new System.Windows.Forms.FlowLayoutPanel();
            this.lblNumEnclaves = new System.Windows.Forms.Label();
            this.lblEnclavesLabel = new System.Windows.Forms.Label();
            this.tabSpawner = new System.Windows.Forms.TabPage();
            this.lblSpawnerDetails = new System.Windows.Forms.Label();
            this.lblSpawnerLabel = new System.Windows.Forms.Label();
            this.tabGameLog = new System.Windows.Forms.TabPage();
            this.tabGameLogTabs = new System.Windows.Forms.TabControl();
            this.tabGameLogLogLevels = new System.Windows.Forms.TabPage();
            this.tlpGameLogLogLevels = new System.Windows.Forms.TableLayoutPanel();
            this.btnGameLogLogLevelsMin = new System.Windows.Forms.Button();
            this.btnGameLogLogLevelMax = new System.Windows.Forms.Button();
            this.tabGameLogLog = new System.Windows.Forms.TabPage();
            this.btnGameLogClear = new System.Windows.Forms.Button();
            this.rtbGameLog = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblWorldToD = new System.Windows.Forms.Label();
            this.tabs.SuspendLayout();
            this.tabWorld.SuspendLayout();
            this.tabEnclaves.SuspendLayout();
            this.tabControlEnclaves.SuspendLayout();
            this.tabEnclaveDetails.SuspendLayout();
            this.tabEnclaveCharacters.SuspendLayout();
            this.tabControlEnclavesCharacters.SuspendLayout();
            this.tabEnclavesCharactersDetails.SuspendLayout();
            this.tabEnclavesCharactersSkills.SuspendLayout();
            this.tabEnclavesCharactersEquipment.SuspendLayout();
            this.tabControlEnclavesCharactersEquipment.SuspendLayout();
            this.tabControlEnclavesCharactersEquipmentMelee.SuspendLayout();
            this.tabControlEnclavesCharactersEquipmentSideArm.SuspendLayout();
            this.tabControlEnclavesCharactersEquipmentRanged.SuspendLayout();
            this.tabSpawner.SuspendLayout();
            this.tabGameLog.SuspendLayout();
            this.tabGameLogTabs.SuspendLayout();
            this.tabGameLogLogLevels.SuspendLayout();
            this.tabGameLogLog.SuspendLayout();
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
            this.tabs.Controls.Add(this.tabGameLog);
            this.tabs.Location = new System.Drawing.Point(12, 41);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(1354, 743);
            this.tabs.TabIndex = 3;
            // 
            // tabWorld
            // 
            this.tabWorld.Controls.Add(this.lblWorldToD);
            this.tabWorld.Controls.Add(this.label2);
            this.tabWorld.Controls.Add(this.chkIsDemo);
            this.tabWorld.Controls.Add(this.lblWorldName);
            this.tabWorld.Controls.Add(this.btnTimeDilationPlus);
            this.tabWorld.Controls.Add(this.btnTimeDilationMinus);
            this.tabWorld.Controls.Add(this.label1);
            this.tabWorld.Controls.Add(this.txtTimeDilation);
            this.tabWorld.Location = new System.Drawing.Point(4, 22);
            this.tabWorld.Name = "tabWorld";
            this.tabWorld.Padding = new System.Windows.Forms.Padding(3);
            this.tabWorld.Size = new System.Drawing.Size(1346, 717);
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
            this.tabEnclaves.Size = new System.Drawing.Size(1346, 717);
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
            this.tabControlEnclaves.Size = new System.Drawing.Size(1196, 698);
            this.tabControlEnclaves.TabIndex = 11;
            // 
            // tabEnclaveDetails
            // 
            this.tabEnclaveDetails.Controls.Add(this.lblEnclaveDetails);
            this.tabEnclaveDetails.Controls.Add(this.txtEnclaveAddress);
            this.tabEnclaveDetails.Location = new System.Drawing.Point(4, 22);
            this.tabEnclaveDetails.Name = "tabEnclaveDetails";
            this.tabEnclaveDetails.Padding = new System.Windows.Forms.Padding(3);
            this.tabEnclaveDetails.Size = new System.Drawing.Size(1188, 672);
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
            this.tabEnclaveCharacters.Controls.Add(this.tabControlEnclavesCharacters);
            this.tabEnclaveCharacters.Controls.Add(this.txtCharacterAddress);
            this.tabEnclaveCharacters.Controls.Add(this.flowEnclaveCharacters);
            this.tabEnclaveCharacters.Controls.Add(this.lblEnclavesNumCharacters);
            this.tabEnclaveCharacters.Location = new System.Drawing.Point(4, 22);
            this.tabEnclaveCharacters.Name = "tabEnclaveCharacters";
            this.tabEnclaveCharacters.Padding = new System.Windows.Forms.Padding(3);
            this.tabEnclaveCharacters.Size = new System.Drawing.Size(1188, 672);
            this.tabEnclaveCharacters.TabIndex = 1;
            this.tabEnclaveCharacters.Text = "Characters";
            this.tabEnclaveCharacters.UseVisualStyleBackColor = true;
            // 
            // tabControlEnclavesCharacters
            // 
            this.tabControlEnclavesCharacters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlEnclavesCharacters.Controls.Add(this.tabEnclavesCharactersDetails);
            this.tabControlEnclavesCharacters.Controls.Add(this.tabEnclavesCharactersSkills);
            this.tabControlEnclavesCharacters.Controls.Add(this.tabEnclavesCharactersEquipment);
            this.tabControlEnclavesCharacters.Location = new System.Drawing.Point(147, 35);
            this.tabControlEnclavesCharacters.Name = "tabControlEnclavesCharacters";
            this.tabControlEnclavesCharacters.SelectedIndex = 0;
            this.tabControlEnclavesCharacters.Size = new System.Drawing.Size(1035, 631);
            this.tabControlEnclavesCharacters.TabIndex = 16;
            // 
            // tabEnclavesCharactersDetails
            // 
            this.tabEnclavesCharactersDetails.Controls.Add(this.tlpEnclavesCharactersDetails);
            this.tabEnclavesCharactersDetails.Location = new System.Drawing.Point(4, 22);
            this.tabEnclavesCharactersDetails.Name = "tabEnclavesCharactersDetails";
            this.tabEnclavesCharactersDetails.Padding = new System.Windows.Forms.Padding(3);
            this.tabEnclavesCharactersDetails.Size = new System.Drawing.Size(1027, 605);
            this.tabEnclavesCharactersDetails.TabIndex = 0;
            this.tabEnclavesCharactersDetails.Text = "Details";
            this.tabEnclavesCharactersDetails.UseVisualStyleBackColor = true;
            // 
            // tlpEnclavesCharactersDetails
            // 
            this.tlpEnclavesCharactersDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpEnclavesCharactersDetails.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpEnclavesCharactersDetails.ColumnCount = 3;
            this.tlpEnclavesCharactersDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpEnclavesCharactersDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpEnclavesCharactersDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpEnclavesCharactersDetails.Location = new System.Drawing.Point(6, 6);
            this.tlpEnclavesCharactersDetails.Name = "tlpEnclavesCharactersDetails";
            this.tlpEnclavesCharactersDetails.RowCount = 1;
            this.tlpEnclavesCharactersDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 631F));
            this.tlpEnclavesCharactersDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 631F));
            this.tlpEnclavesCharactersDetails.Size = new System.Drawing.Size(1015, 601);
            this.tlpEnclavesCharactersDetails.TabIndex = 15;
            // 
            // tabEnclavesCharactersSkills
            // 
            this.tabEnclavesCharactersSkills.Controls.Add(this.tlpEnclavesCharactersSkills);
            this.tabEnclavesCharactersSkills.Location = new System.Drawing.Point(4, 22);
            this.tabEnclavesCharactersSkills.Name = "tabEnclavesCharactersSkills";
            this.tabEnclavesCharactersSkills.Padding = new System.Windows.Forms.Padding(3);
            this.tabEnclavesCharactersSkills.Size = new System.Drawing.Size(1027, 605);
            this.tabEnclavesCharactersSkills.TabIndex = 1;
            this.tabEnclavesCharactersSkills.Text = "Skills";
            this.tabEnclavesCharactersSkills.UseVisualStyleBackColor = true;
            // 
            // tlpEnclavesCharactersSkills
            // 
            this.tlpEnclavesCharactersSkills.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpEnclavesCharactersSkills.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpEnclavesCharactersSkills.ColumnCount = 5;
            this.tlpEnclavesCharactersSkills.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpEnclavesCharactersSkills.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpEnclavesCharactersSkills.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpEnclavesCharactersSkills.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpEnclavesCharactersSkills.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 489F));
            this.tlpEnclavesCharactersSkills.Location = new System.Drawing.Point(6, 6);
            this.tlpEnclavesCharactersSkills.Name = "tlpEnclavesCharactersSkills";
            this.tlpEnclavesCharactersSkills.RowCount = 1;
            this.tlpEnclavesCharactersSkills.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 599F));
            this.tlpEnclavesCharactersSkills.Size = new System.Drawing.Size(1021, 599);
            this.tlpEnclavesCharactersSkills.TabIndex = 16;
            // 
            // tabEnclavesCharactersEquipment
            // 
            this.tabEnclavesCharactersEquipment.Controls.Add(this.tabControlEnclavesCharactersEquipment);
            this.tabEnclavesCharactersEquipment.Location = new System.Drawing.Point(4, 22);
            this.tabEnclavesCharactersEquipment.Name = "tabEnclavesCharactersEquipment";
            this.tabEnclavesCharactersEquipment.Size = new System.Drawing.Size(1027, 605);
            this.tabEnclavesCharactersEquipment.TabIndex = 2;
            this.tabEnclavesCharactersEquipment.Text = "Equipment";
            this.tabEnclavesCharactersEquipment.UseVisualStyleBackColor = true;
            // 
            // tabControlEnclavesCharactersEquipment
            // 
            this.tabControlEnclavesCharactersEquipment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlEnclavesCharactersEquipment.Controls.Add(this.tabControlEnclavesCharactersEquipmentMelee);
            this.tabControlEnclavesCharactersEquipment.Controls.Add(this.tabControlEnclavesCharactersEquipmentSideArm);
            this.tabControlEnclavesCharactersEquipment.Controls.Add(this.tabControlEnclavesCharactersEquipmentRanged);
            this.tabControlEnclavesCharactersEquipment.Location = new System.Drawing.Point(3, 3);
            this.tabControlEnclavesCharactersEquipment.Name = "tabControlEnclavesCharactersEquipment";
            this.tabControlEnclavesCharactersEquipment.SelectedIndex = 0;
            this.tabControlEnclavesCharactersEquipment.Size = new System.Drawing.Size(1021, 599);
            this.tabControlEnclavesCharactersEquipment.TabIndex = 0;
            // 
            // tabControlEnclavesCharactersEquipmentMelee
            // 
            this.tabControlEnclavesCharactersEquipmentMelee.Controls.Add(this.tlpMeleeWeaponStats);
            this.tabControlEnclavesCharactersEquipmentMelee.Location = new System.Drawing.Point(4, 22);
            this.tabControlEnclavesCharactersEquipmentMelee.Name = "tabControlEnclavesCharactersEquipmentMelee";
            this.tabControlEnclavesCharactersEquipmentMelee.Padding = new System.Windows.Forms.Padding(3);
            this.tabControlEnclavesCharactersEquipmentMelee.Size = new System.Drawing.Size(1013, 573);
            this.tabControlEnclavesCharactersEquipmentMelee.TabIndex = 0;
            this.tabControlEnclavesCharactersEquipmentMelee.Text = "Melee";
            this.tabControlEnclavesCharactersEquipmentMelee.UseVisualStyleBackColor = true;
            // 
            // tlpMeleeWeaponStats
            // 
            this.tlpMeleeWeaponStats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpMeleeWeaponStats.ColumnCount = 2;
            this.tlpMeleeWeaponStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMeleeWeaponStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMeleeWeaponStats.Location = new System.Drawing.Point(6, 6);
            this.tlpMeleeWeaponStats.Name = "tlpMeleeWeaponStats";
            this.tlpMeleeWeaponStats.RowCount = 2;
            this.tlpMeleeWeaponStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMeleeWeaponStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMeleeWeaponStats.Size = new System.Drawing.Size(1001, 561);
            this.tlpMeleeWeaponStats.TabIndex = 0;
            // 
            // tabControlEnclavesCharactersEquipmentSideArm
            // 
            this.tabControlEnclavesCharactersEquipmentSideArm.Controls.Add(this.tlpSideArmWeaponStats);
            this.tabControlEnclavesCharactersEquipmentSideArm.Location = new System.Drawing.Point(4, 22);
            this.tabControlEnclavesCharactersEquipmentSideArm.Name = "tabControlEnclavesCharactersEquipmentSideArm";
            this.tabControlEnclavesCharactersEquipmentSideArm.Size = new System.Drawing.Size(1013, 573);
            this.tabControlEnclavesCharactersEquipmentSideArm.TabIndex = 2;
            this.tabControlEnclavesCharactersEquipmentSideArm.Text = "SideArm";
            this.tabControlEnclavesCharactersEquipmentSideArm.UseVisualStyleBackColor = true;
            // 
            // tlpSideArmWeaponStats
            // 
            this.tlpSideArmWeaponStats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpSideArmWeaponStats.ColumnCount = 2;
            this.tlpSideArmWeaponStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSideArmWeaponStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSideArmWeaponStats.Location = new System.Drawing.Point(6, 6);
            this.tlpSideArmWeaponStats.Name = "tlpSideArmWeaponStats";
            this.tlpSideArmWeaponStats.RowCount = 2;
            this.tlpSideArmWeaponStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSideArmWeaponStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSideArmWeaponStats.Size = new System.Drawing.Size(1001, 561);
            this.tlpSideArmWeaponStats.TabIndex = 2;
            // 
            // tabControlEnclavesCharactersEquipmentRanged
            // 
            this.tabControlEnclavesCharactersEquipmentRanged.Controls.Add(this.tlpRangedWeaponStats);
            this.tabControlEnclavesCharactersEquipmentRanged.Location = new System.Drawing.Point(4, 22);
            this.tabControlEnclavesCharactersEquipmentRanged.Name = "tabControlEnclavesCharactersEquipmentRanged";
            this.tabControlEnclavesCharactersEquipmentRanged.Padding = new System.Windows.Forms.Padding(3);
            this.tabControlEnclavesCharactersEquipmentRanged.Size = new System.Drawing.Size(1013, 573);
            this.tabControlEnclavesCharactersEquipmentRanged.TabIndex = 1;
            this.tabControlEnclavesCharactersEquipmentRanged.Text = "Ranged";
            this.tabControlEnclavesCharactersEquipmentRanged.UseVisualStyleBackColor = true;
            // 
            // tlpRangedWeaponStats
            // 
            this.tlpRangedWeaponStats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpRangedWeaponStats.ColumnCount = 2;
            this.tlpRangedWeaponStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpRangedWeaponStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpRangedWeaponStats.Location = new System.Drawing.Point(6, 6);
            this.tlpRangedWeaponStats.Name = "tlpRangedWeaponStats";
            this.tlpRangedWeaponStats.RowCount = 2;
            this.tlpRangedWeaponStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpRangedWeaponStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpRangedWeaponStats.Size = new System.Drawing.Size(1001, 561);
            this.tlpRangedWeaponStats.TabIndex = 1;
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
            this.tabSpawner.Size = new System.Drawing.Size(1346, 717);
            this.tabSpawner.TabIndex = 2;
            this.tabSpawner.Text = "Spawner";
            this.tabSpawner.UseVisualStyleBackColor = true;
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
            // tabGameLog
            // 
            this.tabGameLog.Controls.Add(this.tabGameLogTabs);
            this.tabGameLog.Location = new System.Drawing.Point(4, 22);
            this.tabGameLog.Name = "tabGameLog";
            this.tabGameLog.Size = new System.Drawing.Size(1346, 717);
            this.tabGameLog.TabIndex = 3;
            this.tabGameLog.Text = "GameLog";
            this.tabGameLog.UseVisualStyleBackColor = true;
            // 
            // tabGameLogTabs
            // 
            this.tabGameLogTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabGameLogTabs.Controls.Add(this.tabGameLogLogLevels);
            this.tabGameLogTabs.Controls.Add(this.tabGameLogLog);
            this.tabGameLogTabs.Location = new System.Drawing.Point(3, 3);
            this.tabGameLogTabs.Name = "tabGameLogTabs";
            this.tabGameLogTabs.SelectedIndex = 0;
            this.tabGameLogTabs.Size = new System.Drawing.Size(1340, 711);
            this.tabGameLogTabs.TabIndex = 0;
            // 
            // tabGameLogLogLevels
            // 
            this.tabGameLogLogLevels.AutoScroll = true;
            this.tabGameLogLogLevels.AutoScrollMinSize = new System.Drawing.Size(0, 4000);
            this.tabGameLogLogLevels.Controls.Add(this.tlpGameLogLogLevels);
            this.tabGameLogLogLevels.Controls.Add(this.btnGameLogLogLevelsMin);
            this.tabGameLogLogLevels.Controls.Add(this.btnGameLogLogLevelMax);
            this.tabGameLogLogLevels.Location = new System.Drawing.Point(4, 22);
            this.tabGameLogLogLevels.Name = "tabGameLogLogLevels";
            this.tabGameLogLogLevels.Padding = new System.Windows.Forms.Padding(3);
            this.tabGameLogLogLevels.Size = new System.Drawing.Size(1332, 685);
            this.tabGameLogLogLevels.TabIndex = 0;
            this.tabGameLogLogLevels.Text = "LogLevels";
            this.tabGameLogLogLevels.UseVisualStyleBackColor = true;
            // 
            // tlpGameLogLogLevels
            // 
            this.tlpGameLogLogLevels.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpGameLogLogLevels.ColumnCount = 2;
            this.tlpGameLogLogLevels.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpGameLogLogLevels.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpGameLogLogLevels.Location = new System.Drawing.Point(6, 32);
            this.tlpGameLogLogLevels.Name = "tlpGameLogLogLevels";
            this.tlpGameLogLogLevels.RowCount = 2;
            this.tlpGameLogLogLevels.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpGameLogLogLevels.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpGameLogLogLevels.Size = new System.Drawing.Size(1201, 17260);
            this.tlpGameLogLogLevels.TabIndex = 0;
            // 
            // btnGameLogLogLevelsMin
            // 
            this.btnGameLogLogLevelsMin.Location = new System.Drawing.Point(6, 3);
            this.btnGameLogLogLevelsMin.Name = "btnGameLogLogLevelsMin";
            this.btnGameLogLogLevelsMin.Size = new System.Drawing.Size(75, 23);
            this.btnGameLogLogLevelsMin.TabIndex = 2;
            this.btnGameLogLogLevelsMin.Text = "All 0";
            this.btnGameLogLogLevelsMin.UseVisualStyleBackColor = true;
            this.btnGameLogLogLevelsMin.Click += new System.EventHandler(this.btnGameLogLogLevelsMin_Click);
            // 
            // btnGameLogLogLevelMax
            // 
            this.btnGameLogLogLevelMax.Location = new System.Drawing.Point(87, 3);
            this.btnGameLogLogLevelMax.Name = "btnGameLogLogLevelMax";
            this.btnGameLogLogLevelMax.Size = new System.Drawing.Size(75, 23);
            this.btnGameLogLogLevelMax.TabIndex = 1;
            this.btnGameLogLogLevelMax.Text = "All 9";
            this.btnGameLogLogLevelMax.UseVisualStyleBackColor = true;
            this.btnGameLogLogLevelMax.Click += new System.EventHandler(this.btnGameLogLogLevelMax_Click);
            // 
            // tabGameLogLog
            // 
            this.tabGameLogLog.Controls.Add(this.btnGameLogClear);
            this.tabGameLogLog.Controls.Add(this.rtbGameLog);
            this.tabGameLogLog.Location = new System.Drawing.Point(4, 22);
            this.tabGameLogLog.Name = "tabGameLogLog";
            this.tabGameLogLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabGameLogLog.Size = new System.Drawing.Size(1332, 685);
            this.tabGameLogLog.TabIndex = 1;
            this.tabGameLogLog.Text = "Log";
            this.tabGameLogLog.UseVisualStyleBackColor = true;
            // 
            // btnGameLogClear
            // 
            this.btnGameLogClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGameLogClear.Location = new System.Drawing.Point(1251, 656);
            this.btnGameLogClear.Name = "btnGameLogClear";
            this.btnGameLogClear.Size = new System.Drawing.Size(75, 23);
            this.btnGameLogClear.TabIndex = 1;
            this.btnGameLogClear.Text = "Clear Log";
            this.btnGameLogClear.UseVisualStyleBackColor = true;
            this.btnGameLogClear.Click += new System.EventHandler(this.btnGameLogClear_Click);
            // 
            // rtbGameLog
            // 
            this.rtbGameLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbGameLog.Location = new System.Drawing.Point(3, 6);
            this.rtbGameLog.Name = "rtbGameLog";
            this.rtbGameLog.Size = new System.Drawing.Size(1323, 644);
            this.rtbGameLog.TabIndex = 0;
            this.rtbGameLog.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Time of Day";
            // 
            // lblWorldToD
            // 
            this.lblWorldToD.AutoSize = true;
            this.lblWorldToD.Location = new System.Drawing.Point(77, 73);
            this.lblWorldToD.Name = "lblWorldToD";
            this.lblWorldToD.Size = new System.Drawing.Size(51, 13);
            this.lblWorldToD.TabIndex = 8;
            this.lblWorldToD.Text = "unknown";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1378, 796);
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
            this.tabControlEnclavesCharacters.ResumeLayout(false);
            this.tabEnclavesCharactersDetails.ResumeLayout(false);
            this.tabEnclavesCharactersSkills.ResumeLayout(false);
            this.tabEnclavesCharactersEquipment.ResumeLayout(false);
            this.tabControlEnclavesCharactersEquipment.ResumeLayout(false);
            this.tabControlEnclavesCharactersEquipmentMelee.ResumeLayout(false);
            this.tabControlEnclavesCharactersEquipmentSideArm.ResumeLayout(false);
            this.tabControlEnclavesCharactersEquipmentRanged.ResumeLayout(false);
            this.tabSpawner.ResumeLayout(false);
            this.tabSpawner.PerformLayout();
            this.tabGameLog.ResumeLayout(false);
            this.tabGameLogTabs.ResumeLayout(false);
            this.tabGameLogLogLevels.ResumeLayout(false);
            this.tabGameLogLog.ResumeLayout(false);
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
        private System.Windows.Forms.TextBox txtCharacterAddress;
        private System.Windows.Forms.FlowLayoutPanel flowEnclaveCharacters;
        private System.Windows.Forms.Label lblEnclavesNumCharacters;
        private System.Windows.Forms.CheckBox chkIsDemo;
        private System.Windows.Forms.TabPage tabSpawner;
        private System.Windows.Forms.Label lblSpawnerDetails;
        private System.Windows.Forms.Label lblSpawnerLabel;
        private System.Windows.Forms.TableLayoutPanel tlpEnclavesCharactersDetails;
        private System.Windows.Forms.TabControl tabControlEnclavesCharacters;
        private System.Windows.Forms.TabPage tabEnclavesCharactersDetails;
        private System.Windows.Forms.TabPage tabEnclavesCharactersSkills;
        private System.Windows.Forms.TableLayoutPanel tlpEnclavesCharactersSkills;
        private System.Windows.Forms.TabPage tabGameLog;
        private System.Windows.Forms.TabControl tabGameLogTabs;
        private System.Windows.Forms.TabPage tabGameLogLogLevels;
        private System.Windows.Forms.TableLayoutPanel tlpGameLogLogLevels;
        private System.Windows.Forms.TabPage tabGameLogLog;
        private System.Windows.Forms.RichTextBox rtbGameLog;
        private System.Windows.Forms.Button btnGameLogLogLevelsMin;
        private System.Windows.Forms.Button btnGameLogLogLevelMax;
        private System.Windows.Forms.Button btnGameLogClear;
        private System.Windows.Forms.TabPage tabEnclavesCharactersEquipment;
        private System.Windows.Forms.TabControl tabControlEnclavesCharactersEquipment;
        private System.Windows.Forms.TabPage tabControlEnclavesCharactersEquipmentMelee;
        private System.Windows.Forms.TabPage tabControlEnclavesCharactersEquipmentRanged;
        private System.Windows.Forms.TableLayoutPanel tlpMeleeWeaponStats;
        private System.Windows.Forms.TableLayoutPanel tlpRangedWeaponStats;
        private System.Windows.Forms.TabPage tabControlEnclavesCharactersEquipmentSideArm;
        private System.Windows.Forms.TableLayoutPanel tlpSideArmWeaponStats;
        private System.Windows.Forms.Label lblWorldToD;
        private System.Windows.Forms.Label label2;
    }
}

