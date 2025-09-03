using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SoD2_Editor.Form1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace SoD2_Editor
{
    public partial class Form1 : Form
    {
        private bool connected = false;
        private string _ver = "Dunno";
        private static IntPtr _ba;
        private static IntPtr _daytonVehicleVtPtr;
        private static IntPtr _daytonLocalPlayer;
        private static IntPtr _namesTablePtr;
        private static IntPtr _objTablePtr;
        private static IntPtr _worldPtr;

        private static IntPtr ULIsDemo;

        private static Enclave currEnclave;
        private static DaytonHumanCharacter currDaytonHumanCharacter;

        private System.Windows.Forms.Timer refreshTimer;

        private World world;

        
        public Form1()
        {
            InitializeComponent();

            refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Interval = 66;
            refreshTimer.Tick += RefreshTimer_Tick;

            chkIsDemo.MouseUp += (s, e) => WUInt8(ULIsDemo, (byte)(1 - RUInt8(ULIsDemo)));
            chkIsDemo.KeyUp += (s, e) => WUInt8(ULIsDemo, (byte)(1 - RUInt8(ULIsDemo)));

            EnableDarkMode(this);
        }
        private void EnableDarkMode(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                // Background
                if (ctrl is TextBox || ctrl is RichTextBox)
                {
                    ctrl.BackColor = Color.FromArgb(30, 30, 30);
                    ctrl.ForeColor = Color.WhiteSmoke;
                }
                else if (ctrl is Button)
                {
                    ctrl.BackColor = Color.FromArgb(45, 45, 45);
                    ctrl.ForeColor = Color.WhiteSmoke;
                }
                else if (ctrl is Label)
                {
                    ctrl.ForeColor = Color.WhiteSmoke;
                }
                else if (ctrl is TabControl || ctrl is Panel || ctrl is System.Windows.Forms.GroupBox)
                {
                    ctrl.BackColor = Color.FromArgb(25, 25, 25);
                    ctrl.ForeColor = Color.WhiteSmoke;
                }
                else
                {
                    ctrl.BackColor = Color.FromArgb(25, 25, 25);
                    ctrl.ForeColor = Color.WhiteSmoke;
                }

                if (ctrl.HasChildren)
                {
                    EnableDarkMode(ctrl);
                }
            }

            parent.BackColor = Color.FromArgb(20, 20, 20);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string procName = "StateOfDecay2-Win64-Shipping";
            var process = Process.GetProcessesByName(procName).FirstOrDefault();
            if (process == null)
            {
                return;
            }
            _proc = OpenProcess(0x001F0FFF, false, process.Id);
            _ba = process.MainModule.BaseAddress;
            connected = false;
            if (_proc == IntPtr.Zero)
            {
                lblVer.Text = "Not found.";
            } else {
                uint verIdent = RUInt32(_ba + 0xE8);
                lblVer.Text = "Dunno";
                if (verIdent == 0x49274E48) // MS version
                {
                    connected = true;
                    lblVer.Text = "MS Build 741435";
                    _daytonLocalPlayer = _ba + 0x045BDB10;
                    _daytonVehicleVtPtr = _ba + 0x03418E40;
                    _namesTablePtr = _ba + 0x044DB248;
                    _objTablePtr = _ba + 0x044E3B30;
                    _worldPtr = _ba + 0x045D7C88;
                    ULIsDemo = _ba + 0x43fb1f0;

                }
                else if (verIdent == 0xD54624B7) // Steam version
                {
                    connected = true;
                    lblVer.Text = "Steam Build 741435";
                    _daytonLocalPlayer = _ba + 0x0470C690;
                    _daytonVehicleVtPtr = _ba + 0x034E8930;
                    _namesTablePtr = _ba + 0x04629DC8;
                    _objTablePtr = _ba + 0x046326B0;
                    _worldPtr = _ba + 0x04726808;
                    ULIsDemo = _ba + 0x4549d70;
                }
                if (connected)
                {
                    refreshTimer.Start();
                }
                else
                {
                    refreshTimer.Stop();
                }
            }        
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (RUInt32(_ba + 0xE8) == 0)
            {
                connected = false;
                txtCharacterAddress.Text = "0";
                txtEnclaveAddress.Text = "0";
                lblVer.Text = "Not connected.";
            }
            else
            {
                world = new World(RIntPtr(_worldPtr));
                if (tabs.SelectedTab == null)
                    return;

                var gameMode = world.GameMode;
                var persistentLevel = world.PersistentLevel;
                var worldSettings = persistentLevel.WorldSettings;

                var enclaveManager = gameMode.EnclaveManager;
                DaytonLocalPlayer localPlayer = new DaytonLocalPlayer(RIntPtr(_daytonLocalPlayer));

                currEnclave = localPlayer.DaytonPlayerController.AnalyticsPawn.CharacterComponent.DaytonCharacter.Enclave;
                currDaytonHumanCharacter = localPlayer.DaytonPlayerController.AnalyticsPawn;

                switch (tabs.SelectedTab.Name)
                {
                    case "tabWorld":
                        string newWorldLabel = $"{world.Name}\n{persistentLevel.Name}\n{worldSettings.Name}";
                        if (newWorldLabel != lblWorldName.Text)
                            lblWorldName.Text = newWorldLabel;

                        chkIsDemo.Checked = RUInt8(ULIsDemo) > 0;

                        string newTimeDilation = worldSettings.TimeDilation.ToString("F4");
                        if (newTimeDilation != txtTimeDilation.Text)
                            txtTimeDilation.Text = newTimeDilation;
                        break;

                    case "tabEnclaves":
                        string newEnclavesLabel = $"{world.Name}\n{gameMode.Name}\n{enclaveManager.Name}";
                        if (newEnclavesLabel != lblEnclavesLabel.Text)
                            lblEnclavesLabel.Text = newEnclavesLabel;

                        string newNumEnclaves = $"{enclaveManager.NumEnclaves} Enclaves";
                        if (newNumEnclaves != lblNumEnclaves.Text)
                            lblNumEnclaves.Text = newNumEnclaves;

                        UpdateEnclaveList(enclaveManager);
                        UpdateCharacterList();
                        if (long.TryParse(txtEnclaveAddress.Text, System.Globalization.NumberStyles.HexNumber, null, out long encaddr) && encaddr > 0)
                        {
                            IntPtr enclavePtr = new IntPtr(encaddr);
                            var enclave = new Enclave(enclavePtr);

                            lblEnclavesNumCharacters.Text = $"{enclave.NumCharacters} Characters";

                            if (enclave.NumCharacters == 0) { txtCharacterAddress.Text = "0"; }

                            var enclaveDetails = new StringBuilder();
                            enclaveDetails.AppendLine($"{enclave.DisplayName}");
                            enclaveDetails.AppendLine($"{enclave.SchemaPath}");
                            enclaveDetails.AppendLine($"{enclave.Source}");
                            enclaveDetails.AppendLine($"");
                            enclaveDetails.AppendLine($"Characters:             {enclave.NumCharacters}");
                            enclaveDetails.AppendLine($"NumMemberDeaths:        {enclave.NumMemberDeaths}");
                            enclaveDetails.AppendLine($"Influence:              {enclave.Influence}");
                            enclaveDetails.AppendLine($"bDisplayOnMap:          {enclave.bDisplayOnMap}");
                            enclaveDetails.AppendLine($"EnclaveType:            {enclave.EnclaveType}");
                            enclaveDetails.AppendLine($"bTradesUsingPrestige:   {enclave.bTradesUsingPrestige}");
                            enclaveDetails.AppendLine($"bDisbandsOnAnyRecruit:  {enclave.bDisbandsOnAnyRecruit}");


                                
                            lblEnclaveDetails.Text = enclaveDetails.ToString();
                            if (long.TryParse(txtCharacterAddress.Text, System.Globalization.NumberStyles.HexNumber, null, out long chraddr) && chraddr > 0)
                            {
                                IntPtr chrPtr = new IntPtr(chraddr);
                                var chr = new DaytonCharacter(chrPtr);

                                var chrdetails = new StringBuilder();
                                chrdetails.AppendLine($"ID:                     {chr.ID}");
                                chrdetails.AppendLine($"First Name:             {chr.FirstName}");
                                chrdetails.AppendLine($"Last Name:              {chr.LastName}");
                                chrdetails.AppendLine($"Voice ID:               {chr.VoiceID}");
                                chrdetails.AppendLine($"Cultural Background:    {chr.CulturalBackground}");
                                chrdetails.AppendLine($"Human Definition:       {chr.HumanDefinition}");
                                chrdetails.AppendLine($"Hero Bonus:             {chr.HeroBonus}");
                                chrdetails.AppendLine($"Leader Type:            {chr.LeaderType}");
                                chrdetails.AppendLine($"Standing Level:         {chr.StandingLevel}");
                                chrdetails.AppendLine($"Current Standing:   {chr.CurrStanding,10:F4}");
                                chrdetails.AppendLine($"Current Health:     {chr.CurrHealth,10:F4}");
                                chrdetails.AppendLine($"Current Stamina:    {chr.CurrStam,10:F4}");
                                chrdetails.AppendLine($"Current Fatigue:    {chr.CurrFatigue,10:F4}");
                                chrdetails.AppendLine($"Current Sick:       {chr.CurrSick,10:F4}");
                                chrdetails.AppendLine($"Current Plague:     {chr.CurrPlague,10:F4}");
                                chrdetails.AppendLine($"Trauma Counter:     {chr.TraumaCounter,10:F4}");
                                chrdetails.AppendLine($"Inj Rcvry Counter:  {chr.InjuryRecoveryCounter,10:F4}");
                                chrdetails.AppendLine($"Max Stamina Base:   {chr.MaxStaminaBase,10:F4}");
                                chrdetails.AppendLine($"Max Health Base:    {chr.MaxHealthBase,10:F4}");
                                chrdetails.AppendLine($"Max Sick:           {chr.MaxSick,10:F4}");
                                chrdetails.AppendLine($"Max Plague:         {chr.MaxPlague,10:F4}");
                                chrdetails.AppendLine($"Enclave Name:           {chr.Enclave?.DisplayName ?? "N/A"}");
                                chrdetails.AppendLine($"Position:               X={chr.XPos,12:F4}");
                                chrdetails.AppendLine($"                        Y={chr.YPos,12:F4}");
                                chrdetails.AppendLine($"                        Z={chr.ZPos,12:F4}");
                                chrdetails.AppendLine($"IsInWorld:              {chr.IsInWorld}");

                                if (chr.TraitNames != null && chr.TraitNames.Count > 0)
                                {
                                    chrdetails.AppendLine("Traits:");
                                    foreach (var trait in chr.TraitNames)
                                    {
                                        chrdetails.AppendLine($"    {trait}");
                                    }
                                }
                                lblCharacterDetails.Text = chrdetails.ToString();
                            }
                            else
                            {
                                lblCharacterDetails.Text = "characterDetails";
                            }


                        } else { 
                            lblEnclaveDetails.Text = "enclaveLabel";
                            lblEnclavesNumCharacters.Text = "x Characters";
                        }

                        break;
                    case "tabSpawner":
                        var spawner = gameMode.DynamicPawnSpawner;

                        string newSpawnerLabel = $"{world.Name}\n{gameMode.Name}\n{spawner.Name}";
                        if (newSpawnerLabel != lblEnclavesLabel.Text)
                            lblSpawnerLabel.Text = newSpawnerLabel;

                        var spawnerdetails = new StringBuilder();
                        spawnerdetails.AppendLine($"Active:                               {spawner.Active}");
                        spawnerdetails.AppendLine($"Unlimited Spawning Enabled:           {spawner.bIsUnlimitedSpawningEnabled}");
                        spawnerdetails.AppendLine($"OHKO Zombies Enabled:                 {spawner.bIsOHKOZombiesEnabled}");
                        spawnerdetails.AppendLine($"Max Population Multiplier:        {spawner.MaxPopulationMultiplier,10:F4}");
                        spawnerdetails.AppendLine($"Max Kills Per Cell:               {spawner.MaxKillsPerCell,10:F4}");
                        spawnerdetails.AppendLine($"Min Kills for Adj Bonus:          {spawner.MinKillsForAdjacencyBonus,10:F4}");
                        spawnerdetails.AppendLine($"Min Dampened Spawn Mult:          {spawner.MinDampenedSpawnMultiplier,10:F4}");
                        spawnerdetails.AppendLine($"Kill Decay (sec):                 {spawner.KillDecayInSeconds,10:F4}");
                        spawnerdetails.AppendLine($"Player Decay (sec):               {spawner.PlayerDecayInSeconds,10:F4}");
                        spawnerdetails.AppendLine($"Community Standing:               {spawner.LastSampledCommunityStanding,10:F4}");
                        spawnerdetails.AppendLine($"Time of Day:                      {spawner.LastSampledTimeOfDay,10:F4}");
                        spawnerdetails.AppendLine($"Night Time:                           {spawner.bIsNightTime}");
                        spawnerdetails.AppendLine($"Num Spawned Pawns:                    {spawner.numSpawnedPawns}");
                        spawnerdetails.AppendLine($"Ambient Pop Cap Count:                {spawner.AmbientPopulationCapCount}");

                        lblSpawnerDetails.Text = spawnerdetails.ToString();

                        break;


                    default:
                        break;
                }
            }
        }
        
        private LinkLabel _selectedEnclaveLink = null;
        private void UpdateEnclaveList(EnclaveManager enclaveManager)
        {
            int currentCount = flowEnclaves.Controls.Count;
            int newCount = enclaveManager.NumEnclaves;

            while (currentCount < newCount)
            {
                var link = new LinkLabel
                {
                    AutoSize = true,
                    LinkColor = Color.LightBlue,
                    Margin = new Padding(3),
                    Cursor = Cursors.Hand,
                    BackColor = flowEnclaves.BackColor
                };
                int index = currentCount;
                link.Click += (s, e) => OnEnclaveLinkClicked((LinkLabel)s, enclaveManager.Enclaves[index]);
                flowEnclaves.Controls.Add(link);
                currentCount++;
            }

            while (currentCount > newCount)
            {
                flowEnclaves.Controls.RemoveAt(currentCount - 1);
                currentCount--;
            }

            for (int i = 0; i < newCount; i++)
            {
                try
                {
                    var link = (LinkLabel)flowEnclaves.Controls[i];
                    string newName = enclaveManager.Enclaves[i].DisplayName;
                    //string newName = $"{i}: {enclaveManager.Enclaves[i].EnclaveName}";
                    if (link.Text != newName)
                        link.Text = newName;

                    if (link == _selectedEnclaveLink)
                        link.BackColor = Color.DarkRed;
                    else
                        link.BackColor = flowEnclaves.BackColor;

                    if (newName == currEnclave.DisplayName)
                        link.BackColor = Color.DarkGreen;
                } catch (Exception ex) { Console.WriteLine($@"UpdateEnclaveList:  {ex}"); }
            }
        }

        private void OnEnclaveLinkClicked(LinkLabel clickedLink, Enclave enclave)
        {
            if (_selectedEnclaveLink != null && _selectedEnclaveLink != clickedLink)
            {
                _selectedEnclaveLink.BackColor = flowEnclaves.BackColor;
            }

            clickedLink.BackColor = Color.DarkRed;
            _selectedEnclaveLink = clickedLink;
            txtEnclaveAddress.Text = enclave.BaseAddress.ToString("X");

            txtCharacterAddress.Text = "0";
            foreach (Control ctrl in flowEnclaveCharacters.Controls)
            {
                if (ctrl is LinkLabel link)
                {
                    link.BackColor = Color.Transparent;
                }
            }
        }

        private LinkLabel _selectedCharacterLink = null;

        private void UpdateCharacterList()
        {
            Enclave _currentEnclave = null;
            if (long.TryParse(txtEnclaveAddress.Text, System.Globalization.NumberStyles.HexNumber, null, out long encaddr) && encaddr > 0)
            {
                IntPtr encPtr = (IntPtr)encaddr;
                _currentEnclave = new Enclave(encPtr);
            }

            if (_currentEnclave != null)
            {


                var characters = _currentEnclave.Characters;
                int currentCount = flowEnclaveCharacters.Controls.Count;
                int newCount = characters.Count;

                while (currentCount < newCount)
                {
                    var link = new LinkLabel
                    {
                        AutoSize = true,
                        LinkColor = Color.LightBlue,
                        Margin = new Padding(3),
                        Cursor = Cursors.Hand,
                        BackColor = Color.Transparent
                    };

                    int capturedIndex = currentCount;

                    link.Click += (s, e) =>
                    {
                        if (_selectedCharacterLink != null)
                            _selectedCharacterLink.BackColor = Color.Transparent;

                        link.BackColor = Color.DarkRed;
                        _selectedCharacterLink = link;
                        //This redeclares because of weird caching.
                        if (long.TryParse(txtEnclaveAddress.Text, System.Globalization.NumberStyles.HexNumber, null, out long encaddr2) && encaddr2 > 0)
                        {
                            IntPtr encPtr = (IntPtr)encaddr2;
                            _currentEnclave = new Enclave(encPtr);
                        }

                        if (_currentEnclave != null && capturedIndex < _currentEnclave.Characters.Count)
                        {
                            txtCharacterAddress.Text = _currentEnclave.Characters[capturedIndex].BaseAddress.ToString("X");
                        }
                        else
                        {
                            txtCharacterAddress.Text = "";
                        }
                    };

                    flowEnclaveCharacters.Controls.Add(link);
                    currentCount++;
                }

                while (currentCount > newCount)
                {
                    flowEnclaveCharacters.Controls.RemoveAt(currentCount - 1);
                    currentCount--;
                }

                for (int i = 0; i < newCount; i++)
                {
                    var link = (LinkLabel)flowEnclaveCharacters.Controls[i];
                    string newName = $"{characters[i].FirstName} {characters[i].LastName}";
                    if (link.Text != newName)
                        link.Text = newName;

                    if (_selectedCharacterLink != link)
                        link.BackColor = Color.Transparent;
                }
            }

        }


        private void btnTimeDilationMinus_Click(object sender, EventArgs e)
        {
            if (world.PersistentLevel.WorldSettings.TimeDilation > 0.001) { world.PersistentLevel.WorldSettings.TimeDilation /= 2; }
        }

        private void btnTimeDilationPlus_Click(object sender, EventArgs e)
        {
            if (world.PersistentLevel.WorldSettings.TimeDilation < 1000) { world.PersistentLevel.WorldSettings.TimeDilation *= 2; }
        }
    }
}
