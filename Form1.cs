using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Iced.Intel.AssemblerRegisters;
using static SoD2_Editor.Form1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace SoD2_Editor
{
    public partial class Form1 : Form
    {
        private bool connected = false;
        private static IntPtr _ba;


        private static Enclave currEnclave = null;
        private static DaytonHumanCharacter currDaytonHumanCharacter = null;
        private static DaytonCharacter currDaytonCharacter = null;
        private static DaytonCharacter selectedChar = null;

        private System.Windows.Forms.Timer refreshTimer;

        private static uint verIdent = 0;
        private static AddressBook addresses = new AddressBook();
        private static AddressBook funcs = new AddressBook();
        private static AddressBook hooks = new AddressBook();
        private World world = null;

        

        private void GameLogAdd(string name, IntPtr addr1, IntPtr addr2)
        {
            addresses.Add(name, addr1, addr2);
            logNames.Add(name);
        }
        public Form1()
        {
            InitializeComponent();

            refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Interval = 66;
            refreshTimer.Tick += RefreshTimer_Tick;

            chkIsDemo.MouseUp += (s, e) => WUInt8(addresses.Get("ULIsDemo"), (byte)(1 - RUInt8(addresses.Get("ULIsDemo"))));
            chkIsDemo.KeyUp += (s, e) => WUInt8(addresses.Get("ULIsDemo"), (byte)(1 - RUInt8(addresses.Get("ULIsDemo"))));

            EnableDarkMode(this);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            tabs.SelectedIndex = 1;
            tabControlEnclaves.SelectedIndex = 1;
        }

        private List<string> logNames;
        public void InitAddresses()
        {
            addresses.Add("DaytonLocalPlayer", _ba + 0x045BDB10, _ba + 0x0470C690);
            addresses.Add("DaytonVehicleVtPtr", _ba + 0x03418E40, _ba + 0x034E8930);
            addresses.Add("GameLogPtr", _ba + 0x044243e0, _ba + 0x04572f60);
            addresses.Add("NamesTablePtr", _ba + 0x044DB248, _ba + 0x04629DC8);
            addresses.Add("ObjTablePtr", _ba + 0x044E3B30, _ba + 0x046326B0);
            addresses.Add("WorldPtr", _ba + 0x045D7C88, _ba + 0x04726808);
            addresses.Add("ULIsDemo", _ba + 0x043fb1f0, _ba + 0x04549d70);

            //Functions
            funcs.Add("CheatAddFatigue", _ba + 0x01cfa00, _ba + 0x01d0ab0);
            funcs.Add("CheatAddSickness", _ba + 0x01cfa40, _ba + 0x01d0af0);
            //funcs.Add("", _ba + 0x, _ba + 0x);

            //Hooks
            hooks.Add("AnalyticsZombieDamagedHook", _ba + 0x251df00, _ba + 0x251f510);
            hooks.Add("AnalyticsZombieDamagedReturn", _ba + 0x251df10, _ba + 0x251f520);
            //hooks.Add("", _ba + 0x, _ba + 0x);


            //This is dumb, why didn't I just make a different addressbook like funcs?
            //Because I'm dumb, that's why.
            //TODO:  Be less dumb.
            logNames = new List<string> { };
            GameLogAdd("LogActor", _ba + 0x045b2720, _ba + 0x047012a0);
            GameLogAdd("LogActorComponent", _ba + 0x045b4e70, _ba + 0x047039f0);
            GameLogAdd("LogAIModule", _ba + 0x0459c030, _ba + 0x046eabb0);
            GameLogAdd("LogAkAudio", _ba + 0x046030c0, _ba + 0x04751c40);
            GameLogAdd("LogAssetRegistry", _ba + 0x0459b9d8, _ba + 0x046ea558);
            GameLogAdd("LogBlueprint", _ba + 0x045b43f0, _ba + 0x04702f70);
            GameLogAdd("LogBoonsBanesSaveLoad", _ba + 0x043fc3f0, _ba + 0x0454af70);
            GameLogAdd("LogBoonsBanesStandardNotificationEvents", _ba + 0x043fc3e0, _ba + 0x0454af60);
            GameLogAdd("LogBounties", _ba + 0x043fc880, _ba + 0x0454b400);
            GameLogAdd("LogBounty", _ba + 0x043fc870, _ba + 0x0454b3f0);
            GameLogAdd("LogCasting", _ba + 0x043ff858, _ba + 0x0454e3d8);
            GameLogAdd("LogChallengeModeComponent", _ba + 0x043fc8a8, _ba + 0x0454b428);
            GameLogAdd("LogCharacterRuntimeSpawnList", _ba + 0x043fad10, _ba + 0x04549890);
            GameLogAdd("LogCharacterSkill", _ba + 0x043faa70, _ba + 0x045495f0);
            GameLogAdd("LogCommunityComponent", _ba + 0x043fb550, _ba + 0x0454a0d0);
            GameLogAdd("LogConsoleManager", _ba + 0x04422ba0, _ba + 0x04571720);
            GameLogAdd("LogConsoleResponse", _ba + 0x04422b90, _ba + 0x04571710);
            GameLogAdd("LogContainerDatabase", _ba + 0x043faf88, _ba + 0x04549b08);
            GameLogAdd("LogContainerManager", _ba + 0x043faf98, _ba + 0x04549b18);
            GameLogAdd("LogContainerProtocolComponent", _ba + 0x043fafb8, _ba + 0x04549b38);
            GameLogAdd("LogContentStreaming", _ba + 0x045d49e0, _ba + 0x04723560);
            GameLogAdd("LogContentUnlock", _ba + 0x043fd010, _ba + 0x0454bb90);
            GameLogAdd("LogConversation", _ba + 0x043fb060, _ba + 0x04549be0);
            GameLogAdd("LogD3D11RHI", _ba + 0x044f1310, _ba + 0x0463fe90);
            GameLogAdd("LogDataTable", _ba + 0x045b7600, _ba + 0x04706180);
            GameLogAdd("LogDaybreakGameMode", _ba + 0x043fd258, _ba + 0x0454bdd8);
            GameLogAdd("LogDaytonCharacter", _ba + 0x043facd0, _ba + 0x04549850);
            GameLogAdd("LogDaytonGame", _ba + 0x043fda18, _ba + 0x0454c598);
            GameLogAdd("LogDaytonGameSession", _ba + 0x043fda40, _ba + 0x0454c5c0);
            GameLogAdd("LogDaytonHumanAIController", _ba + 0x043fa218, _ba + 0x04548d98);
            GameLogAdd("LogDaytonPlayerController", _ba + 0x043fb2d0, _ba + 0x04549e50);
            GameLogAdd("LogDaytonWorldSettings", _ba + 0x043fdc18, _ba + 0x0454c798);
            GameLogAdd("LogDebugHud", _ba + 0x043fdc28, _ba + 0x0454c7a8);
            GameLogAdd("LogDemo", _ba + 0x045b7720, _ba + 0x047062a0);
            GameLogAdd("LogDropbag", _ba + 0x043fdf40, _ba + 0x0454cac0);
            GameLogAdd("LogEmotes", _ba + 0x04403fb8, _ba + 0x04552b38);
            GameLogAdd("LogEnclave", _ba + 0x043fb7b8, _ba + 0x0454a338);
            GameLogAdd("LogEngine", _ba + 0x045d6380, _ba + 0x04724f00);
            GameLogAdd("LogEntitlements", _ba + 0x043fdf50, _ba + 0x0454cad0);
            GameLogAdd("LogEquipmentComponent", _ba + 0x043fee60, _ba + 0x0454d9e0);
            GameLogAdd("LogEventsGifts", _ba + 0x043fe020, _ba + 0x0454cba0);
            GameLogAdd("LogExternalProfiler", _ba + 0x0445a2f0, _ba + 0x045a8e70);
            GameLogAdd("LogFateDeck", _ba + 0x043fbbe8, _ba + 0x0454a768);
            GameLogAdd("LogFormattedText", _ba + 0x043fe5d0, _ba + 0x0454d150);
            GameLogAdd("LogFrontendMoviePlayer", _ba + 0x043fe9a0, _ba + 0x0454d520);
            GameLogAdd("LogGameInstance", _ba + 0x043fd8e0, _ba + 0x0454c460);
            GameLogAdd("LogGameMode", _ba + 0x045b8770, _ba + 0x047072f0);
            GameLogAdd("LogGameState", _ba + 0x045b8a68, _ba + 0x047075e8);
            GameLogAdd("LogGameTelemetry", _ba + 0x045e79c0, _ba + 0x04736540);
            //GameLogAdd("LogHAL", _ba + 0x044559b8, _ba + 0x045a4538);   //Still reports in log even if 0
            GameLogAdd("LogHardwareCaps", _ba + 0x045fcab0, _ba + 0x0474b630);
            GameLogAdd("LogHeartlandGameModeComponent", _ba + 0x043fec10, _ba + 0x0454d790);
            GameLogAdd("LogHelpTips", _ba + 0x043fbc08, _ba + 0x0454a788);
            GameLogAdd("LogIggyAS", _ba + 0x045fd890, _ba + 0x0474c410);
            GameLogAdd("LogIggyEngine", _ba + 0x045fcd20, _ba + 0x0474b8a0);
            GameLogAdd("LogIggyFlush", _ba + 0x045fe158, _ba + 0x0474ccd8);
            GameLogAdd("LogIggyTextureSubstitution", _ba + 0x045fe0f0, _ba + 0x0474cc70);
            GameLogAdd("LogInit", _ba + 0x04455ab8, _ba + 0x045a4638);
            GameLogAdd("LogInputRemapping", _ba + 0x043fec50, _ba + 0x0454d7d0);
            GameLogAdd("LogKick", _ba + 0x04403478, _ba + 0x04551ff8);
            GameLogAdd("LogLandscape", _ba + 0x04591450, _ba + 0x046dffd0);
            GameLogAdd("LogLoad", _ba + 0x04455b28, _ba + 0x045a46a8);
            GameLogAdd("LogLocationCasting", _ba + 0x043ff890, _ba + 0x0454e410);
            GameLogAdd("LogMaterial", _ba + 0x045bf760, _ba + 0x0470e2e0);
            GameLogAdd("LogMaterialParameter", _ba + 0x045b5688, _ba + 0x04704208);
            GameLogAdd("LogMemory", _ba + 0x04455a58, _ba + 0x045a45d8);
            GameLogAdd("LogMissionCasting", _ba + 0x043ff930, _ba + 0x0454e4b0);
            GameLogAdd("LogMissionCreation", _ba + 0x044002f8, _ba + 0x0454ee78);
            GameLogAdd("LogMissionMode", _ba + 0x044029a8, _ba + 0x04551528);
            GameLogAdd("LogMissionVariableEvent", _ba + 0x043fbeb0, _ba + 0x0454aa30);
            GameLogAdd("LogModuleManager", _ba + 0x0445a2e0, _ba + 0x045a8e60);
            GameLogAdd("LogMoviePlayer", _ba + 0x0459e580, _ba + 0x046ed100);
            GameLogAdd("LogMultiplayerRewards", _ba + 0x043fbf80, _ba + 0x0454ab00);
            GameLogAdd("LogMusicManager", _ba + 0x043fa608, _ba + 0x04549188);
            GameLogAdd("LogNarrator", _ba + 0x043febd0, _ba + 0x0454d750);
            GameLogAdd("LogNavigation", _ba + 0x045b29f0, _ba + 0x04701570);
            GameLogAdd("LogNavOctree", _ba + 0x045b2a00, _ba + 0x04701580);
            GameLogAdd("LogNet", _ba + 0x045b74e0, _ba + 0x04706060);
            GameLogAdd("LogNetVersion", _ba + 0x04455ff0, _ba + 0x045a4b70);
            GameLogAdd("LogOnline", _ba + 0x04602170, _ba + 0x04750cf0);
            GameLogAdd("LogOutfits", _ba + 0x043fcee0, _ba + 0x0454ba60);
            GameLogAdd("LogPackageLocalizationCache", _ba + 0x044e14f0, _ba + 0x044630070);
            GameLogAdd("LogPakFile", _ba + 0x045e4c80, _ba + 0x04733800);
            GameLogAdd("LogParticles", _ba + 0x045c3228, _ba + 0x04711da8);
            GameLogAdd("LogPhysics", _ba + 0x045c58d8, _ba + 0x04714458);
            GameLogAdd("LogPlagueNodeSpawn", _ba + 0x043fc0a8, _ba + 0x0454ac28);
            GameLogAdd("LogPlayerPortrait", _ba + 0x04402d20, _ba + 0x045518a0);
            GameLogAdd("LogPlayerSpawn", _ba + 0x043febf0, _ba + 0x0454d770);
            GameLogAdd("LogPluginManager", _ba + 0x044e11a0, _ba + 0x0462fd20);
            GameLogAdd("LogPrefabActor", _ba + 0x04402d60, _ba + 0x045518e0);
            GameLogAdd("LogPresence", _ba + 0x04402b88, _ba + 0x04551708);
            GameLogAdd("LogPrimitiveComponent", _ba + 0x045b5780, _ba + 0x04704300);
            GameLogAdd("LogRenderer", _ba + 0x04581770, _ba + 0x046d02f0);
            GameLogAdd("LogRHI", _ba + 0x044f8470, _ba + 0x04646ff0);
            GameLogAdd("LogRollbar", _ba + 0x043fda28, _ba + 0x0454c5a8);
            GameLogAdd("LogSaveManager", _ba + 0x04403590, _ba + 0x04552110);
            GameLogAdd("LogScouting", _ba + 0x04403760, _ba + 0x045522e0);
            GameLogAdd("LogScript", _ba + 0x04455ae8, _ba + 0x045a4668);
            GameLogAdd("LogScriptCore", _ba + 0x044e37b8, _ba + 0x04632338);
            GameLogAdd("LogSettings", _ba + 0x04404498, _ba + 0x04553018);
            GameLogAdd("LogSkinnedMeshComp", _ba + 0x045b5bc0, _ba + 0x04704740);
            GameLogAdd("LogSlate", _ba + 0x044e5ef8, _ba + 0x04634a78);
            GameLogAdd("LogSpawn", _ba + 0x045d7c40, _ba + 0x047267c0);
            GameLogAdd("LogStoryArc", _ba + 0x04403820, _ba + 0x045523a0);
            GameLogAdd("LogStoryDirector", _ba + 0x04403ab0, _ba + 0x04552630);
            GameLogAdd("LogStreamableManager", _ba + 0x045d4968, _ba + 0x047234e8);
            GameLogAdd("LogStreaming", _ba + 0x04455aa8, _ba + 0x045a4628);
            GameLogAdd("LogTaskGraph", _ba + 0x044224f0, _ba + 0x04571070);
            GameLogAdd("LogTemp", _ba + 0x04455a78, _ba + 0x045a45f8);
            GameLogAdd("LogTitleStorage", _ba + 0x04601d30, _ba + 0x047508b0);
            GameLogAdd("LogTransitionFadeToBlack", _ba + 0x04404810, _ba + 0x04553390);
            GameLogAdd("LogTrueSky", _ba + 0x0460a4d0, _ba + 0x04759030);
            GameLogAdd("LogULCrash", _ba + 0x045e4f60, _ba + 0x04733ae0);
            GameLogAdd("LogUObjectArray", _ba + 0x044e38b8, _ba + 0x04632438);
            GameLogAdd("LogVanillaGameMode", _ba + 0x043fec00, _ba + 0x0454d780);
            GameLogAdd("LogVehicleSpawn", _ba + 0x04405ae0, _ba + 0x04554660);
            GameLogAdd("LogVideoRecordingBPLibrary", _ba + 0x04404850, _ba + 0x045533d0);
            GameLogAdd("LogWindows", _ba + 0x04455a08, _ba + 0x045a4588);
            GameLogAdd("LogWorld", _ba + 0x045d7c30, _ba + 0x047267b0);
            GameLogAdd("LogWwiseBankManager", _ba + 0x04603790, _ba + 0x04752310);

            logNames.Add("");
            //GameLogAdd("LogNavigation", _ba + 0x0, _ba + 0x0);
        }

        private void EnableDarkMode(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
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
        public class AddressBook
        {
            private readonly Dictionary<string, (IntPtr MS, IntPtr Steam)> _map
                = new Dictionary<string, (IntPtr, IntPtr)>();

            public void Add(string name, IntPtr msAddr, IntPtr steamAddr)
            {
                _map[name] = (msAddr, steamAddr);
            }

            public IntPtr Get(string name)
            {
                if (!_map.TryGetValue(name, out var pair))
                    return (IntPtr)0;

                switch (verIdent)
                {
                    case 0x49274E48: // MS build
                        return pair.MS;

                    case 0xD54624B7: // Steam build
                        return pair.Steam;

                    default:
                        throw new InvalidOperationException("Unknown game version");
                }
            }
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
                Output("Connect Failed - Not found");
            } else {
                InitAddresses();
                verIdent = RUInt32(_ba + 0xE8);



                lblVer.Text = "Dunno";
                if (verIdent == 0x49274E48) // MS version
                {
                    connected = true;
                    lblVer.Text = "MS Build 741435";
                    Output("Connected - MS Build 741435");
                }
                else if (verIdent == 0xD54624B7) // Steam version
                {
                    connected = true;
                    lblVer.Text = "Steam Build 741435";
                    Output("Connected - Steam Build 741435");
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
        private int _lastLogEnd = 0;

        private void UpdateLog()
        {
            int newLogEnd = GameLog.LogEndOffset;
            if (newLogEnd == _lastLogEnd)
                return;

            if (newLogEnd > _lastLogEnd)
            { 
                string diff = RAsciiStr(GameLog.GameLogTextPtr + _lastLogEnd, newLogEnd - _lastLogEnd);
                rtbGameLog.AppendText(diff);
            }
            else
            {
                if (RInt64(GameLog.BaseAddress) > 0)
                    rtbGameLog.Text = RAsciiStr(GameLog.GameLogTextPtr, newLogEnd);
            }

            rtbGameLog.SelectionStart = rtbGameLog.Text.Length;
            rtbGameLog.ScrollToCaret();

            _lastLogEnd = newLogEnd;
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
                world = new World(RIntPtr(addresses.Get("WorldPtr")));
                if (tabs.SelectedTab == null)
                    return;

                var gameMode = world.GameMode;
                var persistentLevel = world.PersistentLevel;
                var worldSettings = persistentLevel.WorldSettings;

                var enclaveManager = gameMode.EnclaveManager;
                DaytonLocalPlayer localPlayer = new DaytonLocalPlayer(RIntPtr(addresses.Get("DaytonLocalPlayer")));

                currEnclave = localPlayer.DaytonPlayerController.AnalyticsPawn.CharacterComponent.DaytonCharacter.Enclave;
                currDaytonHumanCharacter = localPlayer.DaytonPlayerController.AnalyticsPawn;
                currDaytonCharacter = currDaytonHumanCharacter.CharacterComponent.DaytonCharacter;



                switch (tabs.SelectedTab.Name)
                {
                    case "tabWorld":
                        string newWorldLabel = $"{world.Name}\n{persistentLevel.Name}\n{worldSettings.Name}";
                        if (newWorldLabel != lblWorldName.Text)
                            lblWorldName.Text = newWorldLabel;

                        chkIsDemo.Checked = RUInt8(addresses.Get("ULIsDemo")) > 0;

                        string newTimeDilation = worldSettings.TimeDilation.ToString("F4");
                        if (newTimeDilation != txtTimeDilation.Text)
                            txtTimeDilation.Text = newTimeDilation;


                        
                        float tod = localPlayer.DaytonPlayerController.CommunityComponent.TimeOfDayComponent.TimeOfDay;
                        int hourPart = (int)Math.Floor(tod / 100);
                        double fracPart = (tod % 100) / 100.0;
                        int totalSeconds = (int)Math.Round(3600 * fracPart);
                        int minutes = totalSeconds / 60;
                        int seconds = totalSeconds % 60;
                        if (minutes == 60)
                        {
                            minutes = 0;
                            hourPart++;
                        }

                        if (seconds == 60)
                        {
                            seconds = 0;
                            minutes++;
                        }
                        string result = string.Format("{0:D2}:{1:D2}:{2:D2}", hourPart, minutes, seconds);
                        lblWorldToD.Text = result;

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
                                selectedChar = chr;
                                /*
                                if (chr.TraitNames != null && chr.TraitNames.Count > 0)
                                {
                                    chrdetails.AppendLine("Traits:");
                                    foreach (var trait in chr.TraitNames)
                                    {
                                        chrdetails.AppendLine($"    {trait}");
                                    }
                                }*/
                                switch (tabControlEnclavesCharacters.SelectedTab.Name)
                                {
                                    case "tabEnclavesCharactersDetails":
                                        UpdateCharacterDetails(chr);
                                        break;
                                    case "tabEnclavesCharactersSkills":
                                        UpdateCharacterSkills(chr);
                                        break;
                                    case "tabEnclavesCharactersEquipment":
                                        switch (tabControlEnclavesCharactersEquipment.SelectedTab.Name)
                                        {
                                            case "tabControlEnclavesCharactersEquipmentMelee":
                                                UpdateMeleeWeaponDetails(chr.CharacterRecord.Equipment.MeleeWeaponItemInstance.Stats);
                                                break;
                                            case "tabControlEnclavesCharactersEquipmentSideArm":
                                                UpdateRangedWeaponDetails(tlpSideArmWeaponStats, chr.CharacterRecord.Equipment.SideArmRangedWeaponItemInstance.RangedWeapon.Stats);
                                                break;
                                            case "tabControlEnclavesCharactersEquipmentRanged":
                                                UpdateRangedWeaponDetails(tlpRangedWeaponStats, chr.CharacterRecord.Equipment.RangedWeaponItemInstance.RangedWeapon.Stats);
                                                break;
                                        }
                                        break;
                                    case "tabEnclavesCharactersInventory":
                                        UpdateCharacterInventoryList(chr);

                                        break;
                                }
                            }
                            else
                            {

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

                    case "tabGameLog":
                        UpdateLog();
                        UpdateLogLevels();
                        break;
                    case "tabAnalytics":
                        UpdateAnalytics();
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateAnalytics()
        {

            if (!AZDresults.Equals(IntPtr.Zero))
            {
                var analytics = new ZombieDamagedAnalytics(AZDresults);
                
                lblAnalyticsZombieDamagedDetail.Text =
                    $"{"Zombie Type ID",-20}: {analytics.ZombieTypeId,7}\n" +
                    $"{"Cause of Damage",-20}: {analytics.CauseOfDamageType}\n" +
                    $"{"DealerState",-20}: {analytics.DealerState}\n" +
                    $"{"PreDamageState",-20}: {analytics.PreDamageState}\n" +
                    $"{"ResultingState",-20}: {analytics.ResultingState}\n" +
                    $"{"Zombie ID",-20}: {analytics.ZombieId,7}\n" +
                    $"{"Is Plague Zombie",-20}: {analytics.IsPlagueZombie,7}\n" +
                    $"{"PosX",-20}: {analytics.ZombieX,7}\n" +
                    $"{"PosY",-20}: {analytics.ZombieY,7}\n" +
                    $"{"PosZ",-20}: {analytics.ZombieZ,7}\n" +
                    $"{"Killed",-20}: {analytics.Killed,7}\n" +
                    $"{"Dealer ID",-20}: {analytics.DealerId,7}\n" +
                    $"{"Stun Chance",-20}: {analytics.StunChance,14:F6}\n" +
                    $"{"Down Chance",-20}: {analytics.DownChance,14:F6}\n" +
                    $"{"Kill Chance",-20}: {analytics.KillChance,14:F6}\n" +
                    $"{"Dismember Chance",-20}: {analytics.DismemberChance,14:F6}\n" +
                    $"{"Headshot Counter",-20}: {analytics.HeadshotCounter,7}";
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
            _characterSkillRows.Clear();
            

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
                        _characterSkillRows.Clear();
                        _meleeWeaponDetailRows.Clear();
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
                    };//end link.click

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
                    string newName = $"{characters[i].CharacterRecord.FirstName} {characters[i].CharacterRecord.LastName}";
                    if (link.Text != newName)
                        link.Text = newName;

                    if (_selectedCharacterLink != link)
                    {
                        link.BackColor = Color.Transparent;
                    } else {
                        link.BackColor = Color.DarkRed;
                    }
                        

                    if (newName == $"{currDaytonCharacter.CharacterRecord.FirstName} {currDaytonCharacter.CharacterRecord.LastName}")
                    {
                        link.BackColor = Color.DarkGreen;
                    } else {
                        if ((selectedChar != null)  && (newName != $"{selectedChar.CharacterRecord.FirstName} {selectedChar.CharacterRecord.LastName}"))
                            link.BackColor = Color.Transparent;
                    }

                }
            }

        }
        private class InventoryRowEntry
        {
            public Label NameLabel { get; set; }
            public Label QuantityLabel { get; set; }
            public Button EditButton { get; set; }
        }

        private readonly Dictionary<int, InventoryRowEntry> _inventoryRows = new Dictionary<int, InventoryRowEntry>();
        private void InitializeCharacterInventoryTable()
        {
            tlpEnclaveCharactersInventory.SuspendLayout();
            tlpEnclaveCharactersInventory.Controls.Clear();
            tlpEnclaveCharactersInventory.RowStyles.Clear();
            tlpEnclaveCharactersInventory.RowCount = 0;
            tlpEnclaveCharactersInventory.ColumnStyles.Clear();
            _inventoryRows.Clear();

            tlpEnclaveCharactersInventory.ColumnCount = 3;
            tlpEnclaveCharactersInventory.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
            tlpEnclaveCharactersInventory.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
            tlpEnclaveCharactersInventory.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));

            tlpEnclaveCharactersInventory.ResumeLayout();
        }

        private void UpdateCharacterInventoryList(DaytonCharacter chr)
        {
            if (chr.CharacterRecord.Inventory.BaseAddress.ToString("X") != txtEnclavesCharactersInventoryAddress.Text)
            {
                _inventoryRows.Clear();
                tlpEnclaveCharactersInventory.RowCount = 0;
            }
            if (tlpEnclaveCharactersInventory.RowCount == 0)
                InitializeCharacterInventoryTable();


            txtEnclavesCharactersInventoryAddress.Text = chr.CharacterRecord.Inventory.BaseAddress.ToString("X");
            tlpEnclaveCharactersInventory.SuspendLayout();

            int slotCount = chr.CharacterRecord.Inventory.Slots.Count;

            for (int slotIndex = 0; slotIndex < slotCount; slotIndex++)
            {

                var item = chr.CharacterRecord.Inventory.Slots[slotIndex];

                if (!_inventoryRows.ContainsKey(slotIndex))
                {
                    int rowIndex = tlpEnclaveCharactersInventory.RowCount++;

                    var lblName = new Label
                    {
                        Text = $"{tlpEnclaveCharactersInventory.RowCount} {item.ItemClass?.Name}" ?? $"Unknown ({slotIndex})",
                        AutoSize = true,
                        Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Left,
                        Font = new Font("Consolas", 9),
                    };

                    var lblQuantity = new Label
                    {
                        Text = item.stackCount.ToString(),
                        AutoSize = true,
                        Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Left,
                        Font = new Font("Consolas", 9),
                    };

                    var btnEdit = new Button
                    {
                        Text = "Edit",
                        Width = 25,
                        Height = 10,
                        Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Left
                    };

                    btnEdit.Click += (s, e) =>
                    {
                        string input = ShowInputDialog("Edit Quantity",
                            $"Enter new stack quantity for slot {slotIndex} ({item.ItemClass?.Name ?? "Unknown"})",
                            lblQuantity.Text);

                        if (int.TryParse(input, out int newQty))
                        {
                            item.stackCount = newQty;
                            lblQuantity.Text = newQty.ToString();
                        }
                    };




                    tlpEnclaveCharactersInventory.Controls.Add(lblName, 0, rowIndex);
                    tlpEnclaveCharactersInventory.Controls.Add(btnEdit, 1, rowIndex);
                    tlpEnclaveCharactersInventory.Controls.Add(lblQuantity, 2, rowIndex);
                    tlpEnclaveCharactersInventory.RowStyles.Add(new RowStyle(SizeType.Absolute, 16));


                    _inventoryRows[slotIndex] = new InventoryRowEntry
                    {
                        NameLabel = lblName,
                        QuantityLabel = lblQuantity,
                        EditButton = btnEdit
                    };
                }
                else
                {
                    var row = _inventoryRows[slotIndex];
                    row.NameLabel.Text = item.ItemClass?.Name ?? $"Unknown ({slotIndex})";
                    row.QuantityLabel.Text = item.stackCount.ToString();

                    row.NameLabel.Visible = true;
                    row.QuantityLabel.Visible = true;
                    row.EditButton.Visible = true;
                }
            }




                /*var toRemove = _inventoryRows.Keys.Where(k => k >= slotCount).ToList();
                foreach (var key in toRemove)
                {
                    var row = _inventoryRows[key];
                    tlpEnclaveCharactersInventory.Controls.Remove(row.NameLabel);
                    tlpEnclaveCharactersInventory.Controls.Remove(row.QuantityLabel);
                    tlpEnclaveCharactersInventory.Controls.Remove(row.EditButton);
                    _inventoryRows.Remove(key);
                }*/

                tlpEnclaveCharactersInventory.ResumeLayout();
        }






        private class CharacterDetailRowEntry
        {
            public Label ValueLabel { get; set; }
            public Button EditButton { get; set; }
        }

        private Dictionary<string, CharacterDetailRowEntry> _characterDetailRows = new Dictionary<string, CharacterDetailRowEntry>();

        private void InitializeCharacterTable()
        {
            tlpEnclavesCharactersDetails.SuspendLayout();
            tlpEnclavesCharactersDetails.Controls.Clear();
            tlpEnclavesCharactersDetails.RowStyles.Clear();
            tlpEnclavesCharactersDetails.RowCount = 0;
            tlpEnclavesCharactersDetails.ColumnStyles.Clear();
            _characterDetailRows.Clear();

            tlpEnclavesCharactersDetails.ColumnCount = 3;
            tlpEnclavesCharactersDetails.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));
            tlpEnclavesCharactersDetails.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tlpEnclavesCharactersDetails.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));


            void AddRow(string fieldName, bool editable = false, Action<string> onEdit = null)
            {
                int rowIndex = tlpEnclavesCharactersDetails.RowCount++;

                var lblName = new Label
                {
                    Text = fieldName,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left,
                    Font = new Font("Consolas", 9),
                    TextAlign = ContentAlignment.MiddleRight
                };

                var lblValue = new Label
                {
                    Text = string.Empty,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left,
                    Font = new Font("Consolas", 9),
                    TextAlign = ContentAlignment.MiddleRight
                };
                Button editButton = null;
                if (editable && onEdit != null)
                {
                    editButton = new Button
                    {
                        Text = "Edit",
                        Width = 50,
                        Height = 10
                    };
                    editButton.Click += (s, e) =>
                    {
                        string input = ShowInputDialog("Edit Value", $"Enter new value for {fieldName}", lblValue.Text);
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            onEdit(input);
                            lblValue.Text = input;
                        }
                    };
                    tlpEnclavesCharactersDetails.Controls.Add(editButton, 0, rowIndex);
                }
                foreach (RowStyle style in tlpEnclavesCharactersDetails.RowStyles)
                {
                    style.SizeType = SizeType.Absolute;
                    style.Height = 22;
                }
                tlpEnclavesCharactersDetails.Controls.Add(lblName, 1, rowIndex);
                tlpEnclavesCharactersDetails.Controls.Add(lblValue, 2, rowIndex);
                _characterDetailRows[fieldName] = new CharacterDetailRowEntry { ValueLabel = lblValue, EditButton = editButton };
            }

            AddRow("Enclave Name");
            AddRow("ID");
            AddRow("First Name");
            AddRow("Last Name");
            AddRow("Voice ID");
            AddRow("Cultural Background");
            AddRow("Human Definition");
            AddRow("Philosophy");
            AddRow("Hero Bonus");
            AddRow("Leader Type");
            AddRow("Standing Level", editable: true, onEdit: v => { selectedChar.CharacterRecord.StandingLevel = (ECharacterStanding)byte.Parse(v); });
            AddRow("Current Standing", editable: true, onEdit: v => { selectedChar.CharacterRecord.CurrStanding = float.Parse(v);  });
            AddRow("Current Fatigue", editable: true, onEdit: v => {  selectedChar.CharacterRecord.CurrFatigue = float.Parse(v);  });
            AddRow("Current Stamina", editable: true, onEdit: v => { selectedChar.CharacterRecord.CurrStam = float.Parse(v); });
            AddRow("Max Stamina Base");
            AddRow("Current Health", editable: true, onEdit: v => { selectedChar.CharacterRecord.CurrHealth = float.Parse(v); });
            AddRow("Max Health Base");
            AddRow("Injury Recovery Counter");
            AddRow("Trauma Counter");
            AddRow("Trauma", editable: true, onEdit: v => { selectedChar.Trauma = float.Parse(v); });
            AddRow("Current Sick", editable: true, onEdit: v => { selectedChar.CharacterRecord.CurrSick = float.Parse(v); });
            AddRow("Max Sick");
            AddRow("Current Plague");
            AddRow("Max Plague");
            AddRow("IsInWorld");
            AddRow("Position X");
            AddRow("Position Y");
            AddRow("Position Z");
            AddRow("");  //Empty row added because it magically fixes things and I'd rather not waste the time figuring out why


            tlpEnclavesCharactersDetails.ResumeLayout();
        }
        private void UpdateCharacterDetails(DaytonCharacter chr)
        {
            if (_characterDetailRows.Count == 0)
                InitializeCharacterTable();

            _characterDetailRows["ID"].ValueLabel.Text = chr.CharacterRecord.ID.ToString();
            _characterDetailRows["First Name"].ValueLabel.Text = chr.CharacterRecord.FirstName;
            _characterDetailRows["Last Name"].ValueLabel.Text = chr.CharacterRecord.LastName;
            _characterDetailRows["Voice ID"].ValueLabel.Text = chr.CharacterRecord.VoiceID.ToString();
            _characterDetailRows["Cultural Background"].ValueLabel.Text = chr.CharacterRecord.CulturalBackground;
            _characterDetailRows["Human Definition"].ValueLabel.Text = chr.CharacterRecord.HumanDefinition;
            _characterDetailRows["Philosophy"].ValueLabel.Text = chr.CharacterRecord.Philosophy1.ToString();
            _characterDetailRows["Hero Bonus"].ValueLabel.Text = chr.CharacterRecord.HeroBonus;
            _characterDetailRows["Leader Type"].ValueLabel.Text = chr.CharacterRecord.LeaderType;
            _characterDetailRows["Standing Level"].ValueLabel.Text = chr.CharacterRecord.StandingLevel.ToString();
            _characterDetailRows["Current Standing"].ValueLabel.Text = $"{chr.CharacterRecord.CurrStanding,12:F4}";
            _characterDetailRows["Current Health"].ValueLabel.Text = $"{chr.CharacterRecord.CurrHealth,12:F4}";
            _characterDetailRows["Current Stamina"].ValueLabel.Text = $"{chr.CharacterRecord.CurrStam,12:F4}";
            _characterDetailRows["Current Fatigue"].ValueLabel.Text = $"{chr.CharacterRecord.CurrFatigue,12:F4}";
            _characterDetailRows["Current Sick"].ValueLabel.Text = $"{chr.CharacterRecord.CurrSick,12:F4}";
            _characterDetailRows["Current Plague"].ValueLabel.Text = $"{chr.CharacterRecord.CurrPlague,12:F4}";
            _characterDetailRows["Trauma Counter"].ValueLabel.Text = $"{chr.CharacterRecord.TraumaCounter,12:F4}";
            _characterDetailRows["Injury Recovery Counter"].ValueLabel.Text = $"{chr.CharacterRecord.InjuryRecoveryCounter,12:F4}";
            _characterDetailRows["Max Stamina Base"].ValueLabel.Text = $"{chr.MaxStaminaBase,12:F4}";
            _characterDetailRows["Max Health Base"].ValueLabel.Text = $"{chr.MaxHealthBase,12:F4}";
            _characterDetailRows["Trauma"].ValueLabel.Text = $"{chr.Trauma,12:F4}";
            _characterDetailRows["Max Sick"].ValueLabel.Text = $"{chr.MaxSick,12:F4}";
            _characterDetailRows["Max Plague"].ValueLabel.Text = $"{chr.MaxPlague,12:F4}";
            _characterDetailRows["Enclave Name"].ValueLabel.Text = chr.Enclave?.DisplayName ?? "N/A";
            _characterDetailRows["IsInWorld"].ValueLabel.Text = $"{chr.IsInWorld,7}";
            _characterDetailRows["Position X"].ValueLabel.Text = $"{chr.XPos,12:F4}";
            _characterDetailRows["Position Y"].ValueLabel.Text = $"{chr.YPos,12:F4}";
            _characterDetailRows["Position Z"].ValueLabel.Text = $"{chr.ZPos,12:F4}";   
        }

        private string ShowInputDialog(string title, string prompt, string defaultValue = "")
        {
            Form promptForm = new Form()
            {
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = title,
                StartPosition = FormStartPosition.CenterParent
            };

            Label lblPrompt = new Label() { Left = 10, Top = 20, Text = prompt, AutoSize = true };
            TextBox txtInput = new TextBox() { Left = 10, Top = 50, Width = 360, Text = defaultValue };
            Button btnOk = new Button() { Text = "OK", Left = 220, Width = 70, Top = 80, DialogResult = DialogResult.OK };
            Button btnCancel = new Button() { Text = "Cancel", Left = 300, Width = 70, Top = 80, DialogResult = DialogResult.Cancel };

            btnOk.Click += (sender, e) => { promptForm.Close(); };
            btnCancel.Click += (sender, e) => { promptForm.Close(); };

            promptForm.Controls.Add(lblPrompt);
            promptForm.Controls.Add(txtInput);
            promptForm.Controls.Add(btnOk);
            promptForm.Controls.Add(btnCancel);

            promptForm.AcceptButton = btnOk;
            promptForm.CancelButton = btnCancel;

            return promptForm.ShowDialog() == DialogResult.OK ? txtInput.Text : null;
        }


        private class CharacterSkillRowEntry
        {
            public Label LevelLabel { get; set; }
            public Label XpLabel { get; set; }
            public Button EditLevelButton { get; set; }
            public Button EditXpButton { get; set; }
        }
        private Dictionary<string, CharacterSkillRowEntry> _characterSkillRows = new Dictionary<string, CharacterSkillRowEntry>();
        private void InitializeSkillTable()
        {
            tlpEnclavesCharactersSkills.Controls.Clear();
            tlpEnclavesCharactersSkills.RowStyles.Clear();
            tlpEnclavesCharactersSkills.RowCount = 0;
            tlpEnclavesCharactersSkills.ColumnStyles.Clear();
            _characterSkillRows.Clear();

            tlpEnclavesCharactersSkills.ColumnCount = 5;
            tlpEnclavesCharactersSkills.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200)); // Name
            tlpEnclavesCharactersSkills.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30));  // Edit Level
            tlpEnclavesCharactersSkills.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));  // Level
            tlpEnclavesCharactersSkills.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30));  // Edit XP
            tlpEnclavesCharactersSkills.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));  // XP
            tlpEnclavesCharactersSkills.AutoScroll = true;
        }
        private void UpdateCharacterSkills(DaytonCharacter chr)
        {
            tlpEnclavesCharactersSkills.SuspendLayout();

            if (_characterSkillRows.Count == 0)
                InitializeSkillTable();
            
            

            foreach (var skill in chr.CharacterRecord.Skills)
            {
                if (!_characterSkillRows.TryGetValue(skill.Name, out var row))
                {
                    int rowIndex = tlpEnclavesCharactersSkills.RowCount++;

                    var lblName = new Label
                    {
                        Text = skill.Name,
                        AutoSize = true,
                        Anchor = AnchorStyles.Left,
                        Font = new Font("Consolas", 9)
                    };

                    var lblLevel = new Label
                    {
                        Text = skill.CurrentLevel.ToString(),
                        AutoSize = true,
                        Anchor = AnchorStyles.Left,
                        Font = new Font("Consolas", 9)
                    };

                    var lblXp = new Label
                    {
                        Text = $"{skill.CurrentXp:F4}",
                        AutoSize = true,
                        Anchor = AnchorStyles.Left,
                        Font = new Font("Consolas", 9),
                        TextAlign = ContentAlignment.MiddleRight
                    };

                    var btnEditLevel = new Button
                    {
                        Text = "Edit",
                        Width = 25,
                        Height = 20
                    };
                    btnEditLevel.Click += (s, e) =>
                    {
                        string input = ShowNumericInputDialog("Edit Skill Level", $"Enter new level for {skill.Name}", skill.CurrentLevel.ToString(), isFloat: false);
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            skill.CurrentLevel = byte.Parse(input);
                            lblLevel.Text = input;
                        }
                    };

                    var btnEditXp = new Button
                    {
                        Text = "Edit",
                        Width = 25,
                        Height = 20
                    };
                    btnEditXp.Click += (s, e) =>
                    {
                        string input = ShowNumericInputDialog("Edit Skill XP", $"Enter new XP for {skill.Name}", skill.CurrentXp.ToString("F4"), isFloat: true);
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            skill.CurrentXp = float.Parse(input);
                            lblXp.Text = $"{skill.CurrentXp:F4}";
                        }
                    };

                    tlpEnclavesCharactersSkills.Controls.Add(lblName, 0, rowIndex);
                    tlpEnclavesCharactersSkills.Controls.Add(btnEditLevel, 1, rowIndex);
                    tlpEnclavesCharactersSkills.Controls.Add(lblLevel, 2, rowIndex);
                    tlpEnclavesCharactersSkills.Controls.Add(btnEditXp, 3, rowIndex);
                    tlpEnclavesCharactersSkills.Controls.Add(lblXp, 4, rowIndex);

                    tlpEnclavesCharactersSkills.RowStyles.Add(new RowStyle(SizeType.Absolute, 16));

                    row = new CharacterSkillRowEntry
                    {
                        LevelLabel = lblLevel,
                        XpLabel = lblXp,
                        EditLevelButton = btnEditLevel,
                        EditXpButton = btnEditXp
                    };
                    _characterSkillRows[skill.Name] = row;
                }
                else
                {
                    row.LevelLabel.Text = skill.CurrentLevel.ToString();
                    row.XpLabel.Text = $"{skill.CurrentXp,10:F4}";
                }
            }
            tlpEnclavesCharactersSkills.RowCount++;
            tlpEnclavesCharactersSkills.ResumeLayout();
        }
        
        private string ShowNumericInputDialog(string title, string prompt, string currentValue, bool isFloat = false)
        {
            using (Form form = new Form())
            {
                form.Text = title;
                form.Width = 300;
                form.Height = 150;
                form.StartPosition = FormStartPosition.CenterParent;

                Label lbl = new Label() { Left = 10, Top = 20, Text = prompt, AutoSize = true };
                TextBox txt = new TextBox() { Left = 10, Top = 50, Width = 250, Text = currentValue };

                Button ok = new Button() { Text = "OK", Left = 100, Width = 80, Top = 80, DialogResult = DialogResult.OK };
                form.Controls.Add(lbl);
                form.Controls.Add(txt);
                form.Controls.Add(ok);
                form.AcceptButton = ok;

                ok.Click += (s, e) =>
                {
                    if (isFloat)
                    {
                        if (!float.TryParse(txt.Text, out _))
                        {
                            MessageBox.Show("Please enter a valid floating-point number.");
                            form.DialogResult = DialogResult.None;
                        }
                    }
                    else
                    {
                        if (!int.TryParse(txt.Text, out _))
                        {
                            MessageBox.Show("Please enter a valid integer.");
                            form.DialogResult = DialogResult.None;
                        }
                    }
                };

                return form.ShowDialog() == DialogResult.OK ? txt.Text : null;
            }
        }




        private class LogLevelRowEntry
        {
            public Label NameLabel { get; set; }
            public NumericUpDown ValueControl { get; set; }
        }
        private Dictionary<string, LogLevelRowEntry> _logLevelRows = new Dictionary<string, LogLevelRowEntry>();
        private void InitializeLogLevelTable(List<string> fieldNames)
        {
            tlpGameLogLogLevels.SuspendLayout();
            tlpGameLogLogLevels.Controls.Clear();
            tlpGameLogLogLevels.RowStyles.Clear();
            tlpGameLogLogLevels.RowCount = 0;
            tlpGameLogLogLevels.ColumnStyles.Clear();
            _logLevelRows.Clear();

            tlpGameLogLogLevels.ColumnCount = 2;
            tlpGameLogLogLevels.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tlpGameLogLogLevels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); 

            void AddRow(string fieldName)
            {
                int rowIndex = tlpGameLogLogLevels.RowCount++;

                var lblName = new Label
                {
                    Text = fieldName,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left,
                    Font = new Font("Consolas", 9),
                    TextAlign = ContentAlignment.MiddleRight
                };

                var numValue = new NumericUpDown
                {
                    Minimum = 0,
                    Maximum = 255,
                    Value = 0,
                    Width = 80,
                    ReadOnly = true,
                    Anchor = AnchorStyles.Left
                };
                if (fieldName.Length < 1)
                    numValue.Visible = false;


                numValue.ValueChanged += (s, e) =>
                {
                    IntPtr addr = addresses.Get(fieldName);
                    WUInt8(addr, (byte)numValue.Value);
                };

                tlpGameLogLogLevels.Controls.Add(lblName, 0, rowIndex);
                tlpGameLogLogLevels.Controls.Add(numValue, 1, rowIndex);

                _logLevelRows[fieldName] = new LogLevelRowEntry
                {
                    NameLabel = lblName,
                    ValueControl = numValue
                };
            }

            foreach (var name in fieldNames)
                AddRow(name);

            tlpGameLogLogLevels.ResumeLayout();
        }
        private void UpdateLogLevels()
        {
            if (_logLevelRows.Count < 1)
            {
                InitializeLogLevelTable(logNames);
            }
            foreach (var kvp in _logLevelRows)
            {
                string name = kvp.Key;
                var entry = kvp.Value;

                IntPtr addr = addresses.Get(name);
                byte value = RUInt8(addr);

                if (entry.ValueControl.Value != value)
                {
                    entry.ValueControl.Value = value;
                }
            }
        }

        private class WeaponDetailRowEntry
        {
            public Label ValueLabel { get; set; }
            public Button EditButton { get; set; }
        }

        private Dictionary<string, WeaponDetailRowEntry> _meleeWeaponDetailRows = new Dictionary<string, WeaponDetailRowEntry>();
        private Dictionary<string, WeaponDetailRowEntry> _rangedWeaponDetailRows = new Dictionary<string, WeaponDetailRowEntry>();

        private void InitializeMeleeWeaponTable(MeleeWeaponResourceStats meleeStats)
        {
            tlpMeleeWeaponStats.SuspendLayout();
            tlpMeleeWeaponStats.Controls.Clear();
            tlpMeleeWeaponStats.RowStyles.Clear();
            tlpMeleeWeaponStats.RowCount = 0;
            tlpMeleeWeaponStats.ColumnStyles.Clear();
            _meleeWeaponDetailRows.Clear();

            tlpMeleeWeaponStats.ColumnCount = 3;
            tlpMeleeWeaponStats.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));
            tlpMeleeWeaponStats.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));
            tlpMeleeWeaponStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            void AddRow(string fieldName, bool editable = false, Action<string> onEdit = null)
            {
                int rowIndex = tlpMeleeWeaponStats.RowCount++;

                var lblName = new Label
                {
                    Text = fieldName,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left,
                    Font = new Font("Consolas", 9),
                    TextAlign = ContentAlignment.MiddleRight

                };

                var lblValue = new Label
                {
                    Text = string.Empty,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left,
                    Font = new Font("Consolas", 9),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                Button editButton = null;
                if (editable && onEdit != null)
                {
                    editButton = new Button
                    {
                        Text = "Edit",
                        Width = 50,
                        Height = 10
                    };
                    editButton.Click += (s, e) =>
                    {
                        string input = ShowInputDialog("Edit Value", $"Enter new value for {fieldName}", lblValue.Text);
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            onEdit(input);
                            lblValue.Text = input;
                        }
                    };
                    tlpMeleeWeaponStats.Controls.Add(editButton, 0, rowIndex);
                }

                foreach (RowStyle style in tlpMeleeWeaponStats.RowStyles)
                {
                    style.SizeType = SizeType.Absolute;
                    style.Height = 16;
                }

                tlpMeleeWeaponStats.Controls.Add(lblName, 1, rowIndex);
                tlpMeleeWeaponStats.Controls.Add(lblValue, 2, rowIndex);
                _meleeWeaponDetailRows[fieldName] = new WeaponDetailRowEntry { ValueLabel = lblValue, EditButton = editButton };
            }

            AddRow("Weapon Name");
            AddRow("Weapon Type");
            AddRow("Weapon Desc");
            AddRow("Melee Type");

            AddRow("Dismember", editable: true, onEdit: v => { meleeStats.Dismember = float.Parse(v); });
            AddRow("DismemberDelta", editable: true, onEdit: v => { meleeStats.DismemberDelta = float.Parse(v); });
            AddRow("Impact", editable: true, onEdit: v => { meleeStats.Impact = float.Parse(v); });
            AddRow("ImpactDelta", editable: true, onEdit: v => { meleeStats.ImpactDelta = float.Parse(v); });
            AddRow("Knockdown", editable: true, onEdit: v => { meleeStats.Knockdown = float.Parse(v); });
            AddRow("KnockdownDelta", editable: true, onEdit: v => { meleeStats.KnockdownDelta = float.Parse(v); });
            AddRow("Lethality", editable: true, onEdit: v => { meleeStats.Lethality = float.Parse(v); });
            AddRow("LethalityDelta", editable: true, onEdit: v => { meleeStats.LethalityDelta = float.Parse(v); });
            AddRow("Weight", editable: true, onEdit: v => { meleeStats.Weight = float.Parse(v); });
            AddRow("PerceptionLoudness", editable: true, onEdit: v => { meleeStats.PerceptionLoudness = float.Parse(v); });
            AddRow("Speed", editable: true, onEdit: v => { meleeStats.Speed = float.Parse(v); });
            AddRow("SwingCost", editable: true, onEdit: v => { meleeStats.SwingCost = float.Parse(v); });
            AddRow("InfluenceValue", editable: true, onEdit: v => { meleeStats.InfluenceValue = float.Parse(v); });
            AddRow("PrestigeValue", editable: true, onEdit: v => { meleeStats.PrestigeValue = float.Parse(v); });
            AddRow("Durability", editable: true, onEdit: v => { meleeStats.Durability = float.Parse(v); });
            AddRow("DurabilityLossPerHitMin", editable: true, onEdit: v => { meleeStats.DurabilityLossPerHitMin = float.Parse(v); });
            AddRow("DurabilityLossPerHitMax", editable: true, onEdit: v => { meleeStats.DurabilityLossPerHitMax = float.Parse(v); });
            AddRow("DurabilityLossPerFinisherMin", editable: true, onEdit: v => { meleeStats.DurabilityLossPerFinisherMin = float.Parse(v); });
            AddRow("DurabilityLossPerFinisherMax", editable: true, onEdit: v => { meleeStats.DurabilityLossPerFinisherMax = float.Parse(v); });

            AddRow("");

            tlpMeleeWeaponStats.ResumeLayout();
        }
        private void UpdateMeleeWeaponDetails(MeleeWeaponResourceStats weapon)
        {
            string weaponName = weapon.WeaponName;
            weaponName = weaponName.Replace("{!v}", "");
            weaponName = weaponName.Replace("{[0,+]'}", "");
            weaponName = weaponName.Replace("{[0,+]s}", "");
            weaponName = weaponName.Replace("{[0,+]es}", "");


            if ((_meleeWeaponDetailRows.Count != 0) &&
                //(_meleeWeaponDetailRows["Weapon Name"] != null) &&
                (_meleeWeaponDetailRows["Weapon Name"].ValueLabel.Text != weaponName))
            {
                _meleeWeaponDetailRows.Clear();
                InitializeMeleeWeaponTable(weapon); ;
            }
            if (_meleeWeaponDetailRows.Count == 0)
                InitializeMeleeWeaponTable(weapon);

            if (weapon.BaseAddress == (IntPtr)0)
                tlpMeleeWeaponStats.Visible = false;
            else
                tlpMeleeWeaponStats.Visible = true;

            
            _meleeWeaponDetailRows["Weapon Name"].ValueLabel.Text = weaponName;
            _meleeWeaponDetailRows["Weapon Type"].ValueLabel.Text = weapon.WeaponType;

            string weaponDesc = weapon.WeaponDesc;
            weaponDesc = weaponDesc.Replace("<br>", Environment.NewLine);
            _meleeWeaponDetailRows["Weapon Desc"].ValueLabel.Text = weaponDesc;
            _meleeWeaponDetailRows["Weapon Desc"].ValueLabel.BorderStyle = BorderStyle.FixedSingle;

            _meleeWeaponDetailRows["Melee Type"].ValueLabel.Text = weapon.MeleeType.ToString();

            _meleeWeaponDetailRows["Dismember"].ValueLabel.Text = $"{weapon.Dismember,12:F4}";
            _meleeWeaponDetailRows["DismemberDelta"].ValueLabel.Text = $"{weapon.DismemberDelta,12:F4}";
            _meleeWeaponDetailRows["Impact"].ValueLabel.Text = $"{weapon.Impact,12:F4}";
            _meleeWeaponDetailRows["ImpactDelta"].ValueLabel.Text = $"{weapon.ImpactDelta,12:F4}";
            _meleeWeaponDetailRows["Knockdown"].ValueLabel.Text = $"{weapon.Knockdown,12:F4}";
            _meleeWeaponDetailRows["KnockdownDelta"].ValueLabel.Text = $"{weapon.KnockdownDelta,12:F4}";
            _meleeWeaponDetailRows["Lethality"].ValueLabel.Text = $"{weapon.Lethality,12:F4}";
            _meleeWeaponDetailRows["LethalityDelta"].ValueLabel.Text = $"{weapon.LethalityDelta,12:F4}";
            _meleeWeaponDetailRows["Weight"].ValueLabel.Text = $"{weapon.Weight,12:F4}";
            _meleeWeaponDetailRows["PerceptionLoudness"].ValueLabel.Text = $"{weapon.PerceptionLoudness,12:F4}";
            _meleeWeaponDetailRows["Speed"].ValueLabel.Text = $"{weapon.Speed,12:F4}";
            _meleeWeaponDetailRows["SwingCost"].ValueLabel.Text = $"{weapon.SwingCost,12:F4}";
            _meleeWeaponDetailRows["InfluenceValue"].ValueLabel.Text = $"{weapon.InfluenceValue,12:F4}";
            _meleeWeaponDetailRows["PrestigeValue"].ValueLabel.Text = $"{weapon.PrestigeValue,12:F4}";
            _meleeWeaponDetailRows["Durability"].ValueLabel.Text = $"{weapon.Durability,12:F4}";
            _meleeWeaponDetailRows["DurabilityLossPerHitMin"].ValueLabel.Text = $"{weapon.DurabilityLossPerHitMin,12:F4}";
            _meleeWeaponDetailRows["DurabilityLossPerHitMax"].ValueLabel.Text = $"{weapon.DurabilityLossPerHitMax,12:F4}";
            _meleeWeaponDetailRows["DurabilityLossPerFinisherMin"].ValueLabel.Text = $"{weapon.DurabilityLossPerFinisherMin,12:F4}";
            _meleeWeaponDetailRows["DurabilityLossPerFinisherMax"].ValueLabel.Text = $"{weapon.DurabilityLossPerFinisherMax,12:F4}";
        }
        private void InitializeRangedWeaponTable(TableLayoutPanel tlp, RangedWeaponResourceStats rangedStats)
        {
            tlp.SuspendLayout();
            tlp.Controls.Clear();
            tlp.RowStyles.Clear();
            tlp.RowCount = 0;
            tlp.ColumnStyles.Clear();
            _rangedWeaponDetailRows.Clear();

            tlp.ColumnCount = 3;
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            void AddRow(string fieldName, bool editable = false, Action<string> onEdit = null)
            {
                int rowIndex = tlp.RowCount++;

                var lblName = new Label
                {
                    Text = fieldName,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left,
                    Font = new Font("Consolas", 9),
                    TextAlign = ContentAlignment.MiddleRight
                };

                var lblValue = new Label
                {
                    Text = string.Empty,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left,
                    Font = new Font("Consolas", 9),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                Button editButton = null;
                if (editable && onEdit != null)
                {
                    editButton = new Button
                    {
                        Text = "Edit",
                        Width = 50,
                        Height = 10
                    };
                    editButton.Click += (s, e) =>
                    {
                        string input = ShowInputDialog("Edit Value", $"Enter new value for {fieldName}", lblValue.Text);
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            onEdit(input);
                            lblValue.Text = input;
                        }
                    };
                    tlp.Controls.Add(editButton, 0, rowIndex);
                }

                foreach (RowStyle style in tlp.RowStyles)
                {
                    style.SizeType = SizeType.Absolute;
                    style.Height = 16;
                }

                tlp.Controls.Add(lblName, 1, rowIndex);
                tlp.Controls.Add(lblValue, 2, rowIndex);
                _rangedWeaponDetailRows[fieldName] = new WeaponDetailRowEntry { ValueLabel = lblValue, EditButton = editButton };
            }

            AddRow("Weapon Name");
            AddRow("Weapon Type");
            AddRow("Weapon Desc");

            AddRow("KickAngle", editable: true, onEdit: v => { rangedStats.KickAngle = float.Parse(v); });
            AddRow("ProjectileCount", editable: true, onEdit: v => { rangedStats.ProjectileCount = int.Parse(v); });
            AddRow("MagazineSize", editable: true, onEdit: v => { rangedStats.MagazineSize = int.Parse(v); });
            AddRow("Impact", editable: true, onEdit: v => { rangedStats.Impact = float.Parse(v); });
            AddRow("ImpactDelta", editable: true, onEdit: v => { rangedStats.ImpactDelta = float.Parse(v); });
            AddRow("Knockdown", editable: true, onEdit: v => { rangedStats.Knockdown = float.Parse(v); });
            AddRow("KnockdownDelta", editable: true, onEdit: v => { rangedStats.KnockdownDelta = float.Parse(v); });
            AddRow("Dismember", editable: true, onEdit: v => { rangedStats.Dismember = float.Parse(v); });
            AddRow("DismemberDelta", editable: true, onEdit: v => { rangedStats.DismemberDelta = float.Parse(v); });
            AddRow("Penetration", editable: true, onEdit: v => { rangedStats.Penetration = float.Parse(v); });
            AddRow("Range", editable: true, onEdit: v => { rangedStats.Range = float.Parse(v); });
            AddRow("Weight", editable: true, onEdit: v => { rangedStats.Weight = float.Parse(v); });
            AddRow("InfluenceValue", editable: true, onEdit: v => { rangedStats.InfluenceValue = int.Parse(v); });
            AddRow("PrestigeValue", editable: true, onEdit: v => { rangedStats.PrestigeValue = int.Parse(v); });
            AddRow("Durability", editable: true, onEdit: v => { rangedStats.Durability = float.Parse(v); });
            AddRow("PerceptionLoudness", editable: true, onEdit: v => { rangedStats.PerceptionLoudness = float.Parse(v); });
            AddRow("TracerSettings");

            AddRow("");

            tlp.ResumeLayout();
        }
        private void UpdateRangedWeaponDetails(TableLayoutPanel tlp, RangedWeaponResourceStats weapon)
        {
            string weaponName = weapon.WeaponName;
            weaponName = weaponName.Replace("{!v}", "");
            weaponName = weaponName.Replace("{[0,+]'}", "");
            weaponName = weaponName.Replace("{[0,+]s}", "");
            weaponName = weaponName.Replace("{[0,+]es}", "");


            if ((_rangedWeaponDetailRows.Count != 0) &&
                (_rangedWeaponDetailRows["Weapon Name"].ValueLabel.Text != weaponName))
            {
                _rangedWeaponDetailRows.Clear();
                InitializeRangedWeaponTable(tlp, weapon); ;
            }
            if (_rangedWeaponDetailRows.Count == 0)
                InitializeRangedWeaponTable(tlp, weapon);

            if (weapon.BaseAddress == (IntPtr)0)
                tlp.Visible = false;
            else
                tlp.Visible = true;

            _rangedWeaponDetailRows["Weapon Name"].ValueLabel.Text = weaponName;
            _rangedWeaponDetailRows["Weapon Type"].ValueLabel.Text = weapon.WeaponType;

            string weaponDesc = weapon.WeaponDesc;
            weaponDesc = weaponDesc.Replace("<br>", Environment.NewLine);
            _rangedWeaponDetailRows["Weapon Desc"].ValueLabel.Text = weaponDesc;
            _rangedWeaponDetailRows["Weapon Desc"].ValueLabel.BorderStyle = BorderStyle.FixedSingle;

            _rangedWeaponDetailRows["KickAngle"].ValueLabel.Text = $"{weapon.KickAngle,12:F4}";
            _rangedWeaponDetailRows["ProjectileCount"].ValueLabel.Text = $"{weapon.ProjectileCount,7}";
            _rangedWeaponDetailRows["MagazineSize"].ValueLabel.Text = $"{weapon.MagazineSize,7}";
            _rangedWeaponDetailRows["Impact"].ValueLabel.Text = $"{weapon.Impact,12:F4}";
            _rangedWeaponDetailRows["ImpactDelta"].ValueLabel.Text = $"{weapon.ImpactDelta,12:F4}";
            _rangedWeaponDetailRows["Knockdown"].ValueLabel.Text = $"{weapon.Knockdown,12:F4}";
            _rangedWeaponDetailRows["KnockdownDelta"].ValueLabel.Text = $"{weapon.KnockdownDelta,12:F4}";
            _rangedWeaponDetailRows["Dismember"].ValueLabel.Text = $"{weapon.Dismember,12:F4}";
            _rangedWeaponDetailRows["DismemberDelta"].ValueLabel.Text = $"{weapon.DismemberDelta,12:F4}";
            _rangedWeaponDetailRows["Penetration"].ValueLabel.Text = $"{weapon.Penetration,12:F4}";
            _rangedWeaponDetailRows["Range"].ValueLabel.Text = $"{weapon.Range,12:F4}";
            _rangedWeaponDetailRows["Weight"].ValueLabel.Text = $"{weapon.Weight,12:F4}";
            _rangedWeaponDetailRows["InfluenceValue"].ValueLabel.Text = $"{weapon.InfluenceValue,7}";
            _rangedWeaponDetailRows["PrestigeValue"].ValueLabel.Text = $"{weapon.PrestigeValue,7}";
            _rangedWeaponDetailRows["Durability"].ValueLabel.Text = $"{weapon.Durability,12:F4}";
            _rangedWeaponDetailRows["PerceptionLoudness"].ValueLabel.Text = $"{weapon.PerceptionLoudness,12:F4}";
            _rangedWeaponDetailRows["TracerSettings"].ValueLabel.Text = weapon.TracerSettings;
        }



        private void btnTimeDilationMinus_Click(object sender, EventArgs e)
        {
            if (world.PersistentLevel.WorldSettings.TimeDilation > 0.001) { world.PersistentLevel.WorldSettings.TimeDilation /= 2; }
        }
        private void btnTimeDilationPlus_Click(object sender, EventArgs e)
        {
            if (world.PersistentLevel.WorldSettings.TimeDilation < 1000) { world.PersistentLevel.WorldSettings.TimeDilation *= 2; }
        }

        private void btnGameLogLogLevelsMin_Click(object sender, EventArgs e)
        {
            foreach (string logName in logNames)
            {
                WUInt8(addresses.Get(logName), 0);
            }
        }
        private void btnGameLogLogLevelMax_Click(object sender, EventArgs e)
        {
            foreach (string logName in logNames)
            {
                WUInt8(addresses.Get(logName), 9);
            }
        }
        private void btnGameLogClear_Click(object sender, EventArgs e)
        {
            int length = GameLog.LogEndOffset;
            if (length <= 0)
            { 
                Output("LogEndOffset <= 0");
                return;
            }

            byte[] zeros = new byte[length];
            WBytes(GameLog.GameLogTextPtr, zeros);
            GameLog.LogEndOffset = 0;
            GameLog.LogEndOffset2 = 0;
            rtbGameLog.Clear();
            _lastLogEnd = 0;

            Output("Log Cleared");
        }


        private void Output(string output)
        {
            txtOutput.Text = $"[{DateTime.Now:HH:mm:ss}] {output}";
        }
        IntPtr AZDresults = IntPtr.Zero;
        int AZDresultsSize = 0x1000;
        IntPtr AZDstrings = IntPtr.Zero;
        int AZDstringsSize = 0x1000;
        private void btnHookZombieDamagedAnalytics_Click(object sender, EventArgs e)
        {

            /*  
            IntPtr newfunc = Alloc(0x1024);
            Console.WriteLine(newfunc.ToString("X"));

            Iced.Intel.Assembler asm = new Iced.Intel.Assembler(bitness: 64);
            asm.sub(rsp, 0x1000);
            asm.pushfq();

            asm.popfq();
            asm.add(rsp, 0x1000);
            asm.ret();


            var stream = new MemoryStream();
            asm.Assemble(new Iced.Intel.StreamCodeWriter(stream), (ulong)newfunc);
            byte[] machineCode = stream.ToArray();

            WBytes(newfunc, machineCode);
            CreateRemoteThread(_proc, IntPtr.Zero, 0, newfunc, IntPtr.Zero, 0, IntPtr.Zero);
            */



            //Hook Analytics for zombie hit
            IntPtr AnalyticsZombieDamagedHook = hooks.Get("AnalyticsZombieDamagedHook");
            IntPtr AnalyticsZombieDamagedReturn = hooks.Get("AnalyticsZombieDamagedReturn");
            IntPtr AZDhookedFunc = Alloc(0x1000);
            AZDresults = Alloc(AZDresultsSize);
            AZDstrings = Alloc(AZDstringsSize);
            int CauseOfDamageIdOffset = 0x80;
            int DealerStateOffset = 0xC0;
            int PreDamageStateOffset = 0x100;
            int ResultingStateOffset = 0x140;
            Console.WriteLine($"AZDhookedFunc: {AZDhookedFunc.ToString("X")}");
            Console.WriteLine($"AZDresults: {AZDresults.ToString("X")}");
            Console.WriteLine($"AZDstrings: {AZDstrings.ToString("X")}");

            Iced.Intel.Assembler asm = new Iced.Intel.Assembler(bitness: 64);
            //Start of hooked code
            asm.sub(rsp, 0x1000);
            asm.pushfq();



            //Copy AnalyticsHelper
            asm.mov(rax, AZDresults.ToInt64());
            asm.push(rcx);
            asm.push(rsi);
            asm.push(rdi);
            asm.mov(rsi, rcx);
            asm.mov(rdi, rax);
            asm.mov(rcx, 0x200);
            asm.rep.movsb();
            asm.pop(rdi);
            asm.pop(rsi);
            asm.pop(rcx);

            //Copy CauseOfDamageId String
            asm.mov(rax, AZDstrings.ToInt64());
            asm.push(rcx);
            asm.push(rsi);
            asm.push(rdi);
            asm.mov(rsi, __[rcx + 0x170]);
            asm.add(rax, CauseOfDamageIdOffset);
            asm.mov(rdi, rax);
            asm.mov(ecx, __[rcx + 0x178]);
            asm.add(ecx, ecx);
            asm.rep.movsb();
            asm.mov(rcx, AZDresults.ToInt64());
            asm.mov(__[rcx + 0x170], rax);
            asm.pop(rdi);
            asm.pop(rsi);
            asm.pop(rcx);
            //Copy DealerState String
            asm.mov(rax, AZDstrings.ToInt64());
            asm.push(rcx);
            asm.push(rsi);
            asm.push(rdi);
            asm.mov(rsi, __[rcx + 0x188]);
            asm.add(rax, DealerStateOffset);
            asm.mov(rdi, rax);
            asm.mov(ecx, __[rcx + 0x190]);
            asm.add(ecx, ecx);
            asm.rep.movsb();
            asm.mov(rcx, AZDresults.ToInt64());
            asm.mov(__[rcx + 0x188], rax);
            asm.pop(rdi);
            asm.pop(rsi);
            asm.pop(rcx);
            //Copy PreDamageState String
            asm.mov(rax, AZDstrings.ToInt64());
            asm.push(rcx);
            asm.push(rsi);
            asm.push(rdi);
            asm.mov(rsi, __[rcx + 0x1A8]);
            asm.add(rax, PreDamageStateOffset);
            asm.mov(rdi, rax);
            asm.mov(ecx, __[rcx + 0x1B0]);
            asm.add(ecx, ecx);
            asm.rep.movsb();
            asm.mov(rcx, AZDresults.ToInt64());
            asm.mov(__[rcx + 0x1A8], rax);
            asm.pop(rdi);
            asm.pop(rsi);
            asm.pop(rcx);
            //Copy ResultingState String
            asm.mov(rax, AZDstrings.ToInt64());
            asm.push(rcx);
            asm.push(rsi);
            asm.push(rdi);
            asm.mov(rsi, __[rcx + 0x1B8]);
            asm.add(rax, ResultingStateOffset);
            asm.mov(rdi, rax);
            asm.mov(ecx, __[rcx + 0x1C0]);
            asm.add(ecx, ecx);
            asm.rep.movsb();
            asm.mov(rcx, AZDresults.ToInt64());
            asm.mov(__[rcx + 0x1B8], rax);
            asm.pop(rdi);
            asm.pop(rsi);
            asm.pop(rcx);


            

            



            asm.popfq();
            asm.add(rsp, 0x1000);
            //Replacing code we broke with our hook
            asm.push(rbp);
            asm.push(rbx);
            asm.push(rdi);
            asm.lea(rbp, rsp - 0x47);
            asm.sub(rsp, 0xa0);
            asm.mov(rax, AnalyticsZombieDamagedReturn.ToInt64());
            asm.jmp(rax);
            //Jmp back out of hook
            var stream = new MemoryStream();
            asm.Assemble(new Iced.Intel.StreamCodeWriter(stream), (ulong)AZDhookedFunc);
            byte[] machineCode = stream.ToArray();
            WBytes(AZDhookedFunc, machineCode);


            asm = new Iced.Intel.Assembler(bitness: 64);
            asm.mov(rax, AZDhookedFunc.ToInt64());
            asm.jmp(rax);
            stream = new MemoryStream();
            asm.Assemble(new Iced.Intel.StreamCodeWriter(stream), (ulong)AZDhookedFunc);
            machineCode = stream.ToArray();
            WBytes(AnalyticsZombieDamagedHook, machineCode);

        }

        private void btnUnhookZombieDamagedAnalytics_Click(object sender, EventArgs e)
        {
            lblAnalyticsZombieDamagedDetail.Text = "Unhooked";
            IntPtr AnalyticsZombieDamagedHook = hooks.Get("AnalyticsZombieDamagedHook");

            Iced.Intel.Assembler asm = new Iced.Intel.Assembler(bitness: 64);
            //Start of hooked code
            asm.sub(rsp, 0x1000);



            AZDresults = Alloc(0x1000);
            AZDstrings = Alloc(0x1000);
        }

        
    }
}
