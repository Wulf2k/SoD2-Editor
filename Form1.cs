using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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
        private static IntPtr lastInspected = IntPtr.Zero;
        private readonly List<IntPtr> inspectHistory = new List<IntPtr>();

        private static Enclave currEnclave = null;
        private static DaytonHumanCharacter currDaytonHumanCharacter = null;
        private static DaytonCharacter currDaytonCharacter = null;
        private static DaytonCharacter selectedChar = null;

        private System.Windows.Forms.Timer refreshTimer;

        private static uint verIdent = 0;
        private static AddressBook addresses = new AddressBook();
        private static AddressBook gamelogs = new AddressBook();
        private static AddressBook funcs = new AddressBook();
        private static AddressBook hooks = new AddressBook();
        private World world = null;
        private GameEngine eng = null;
        private DaytonLocalPlayer localPlayer = null;
        private DaytonGameInstance gameInstance = null;
        private DaytonGameGameMode gameMode = null;


        private void GameLogAdd(string name, IntPtr addr1, IntPtr addr2)
        {
            gamelogs.Add(name, addr1, addr2);
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

            chkBlindZombies.MouseUp += (s, e) => localPlayer.DaytonPlayerController.CheatManager.bInvisibleToZombies = !localPlayer.DaytonPlayerController.CheatManager.bInvisibleToZombies;
            chkBlindZombies.KeyUp += (s, e) => localPlayer.DaytonPlayerController.CheatManager.bInvisibleToZombies = !localPlayer.DaytonPlayerController.CheatManager.bInvisibleToZombies;

            chkSpawnerActive.MouseUp += (s, e) => gameMode.DynamicPawnSpawner.Active = (byte)(1 - gameMode.DynamicPawnSpawner.Active);
            chkSpawnerActive.KeyUp += (s, e) => gameMode.DynamicPawnSpawner.Active = (byte)(1 - gameMode.DynamicPawnSpawner.Active);

            EnableDarkMode(this);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            lblCharacterTraits.Text = "";
            lblCharactersLabel.Text = "";
            lblEnclavesLabel.Text = "";
            lblEquipment.Text = "";
            lblMelee.Text = "";
            lblRanged.Text = "";
            lblSidearm.Text = "";
            lblInventory.Text = "";
            InitEnclaveTable();
            InitEnclaveCharactersTable();
            InitCharacterSkillsTable();
            InitializeCommunityResourceGrid(dgvCommunityResources);

            InitInspectorGrid();
        }

        private List<string> logNames;
        public void InitAddresses()
        {
            newStringsPtr = IntPtr.Zero;
            newStringsOffset = 0;
            addresses.Add("UConsoleClass", _ba + 0x045e30b8, _ba + 0x04731c38);
            addresses.Add("UDaytonCheatManagerClass", _ba + 0x0440f260, _ba + 0x0455dde0);
            addresses.Add("DaytonLocalPlayer", _ba + 0x045BDB10, _ba + 0x0470C690);
            addresses.Add("DaytonVehicleVtPtr", _ba + 0x03418E40, _ba + 0x034E8930);
            addresses.Add("GameEngine", _ba + 0x045d59a0, _ba + 0x4724520);
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
            GameLogAdd("LogDaytonGameCharacter", _ba + 0x043fd2d8, _ba + 0x0454be58);
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
            GameLogAdd("LogHUD", _ba + 0x045bc9b0, _ba + 0x0470b530);
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
            //GameLogAdd("LogDaytonGameCharacter", _ba + 0x0, _ba + 0x0);

            //TODO:  Try add to rowcount instead of just adding an empty row
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
                    ((Button)ctrl).FlatStyle = FlatStyle.Flat;
                    ((Button)ctrl).FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
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
                else if (ctrl is DataGridView dgv)
                {
                    dgv.BackgroundColor = Color.FromArgb(25, 25, 25);
                    dgv.BorderStyle = BorderStyle.None;
                    dgv.EnableHeadersVisualStyles = false;
                    dgv.GridColor = Color.FromArgb(50, 50, 50);

                    dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 35, 35);
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.WhiteSmoke;
                    dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(55, 55, 55);
                    dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;

                    dgv.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
                    dgv.RowHeadersDefaultCellStyle.ForeColor = Color.WhiteSmoke;
                    dgv.RowHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(55, 55, 55);
                    dgv.RowHeadersDefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;

                    dgv.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
                    dgv.DefaultCellStyle.ForeColor = Color.WhiteSmoke;
                    dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(70, 70, 70);
                    dgv.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;

                    dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(35, 35, 35);
                    dgv.AlternatingRowsDefaultCellStyle.ForeColor = Color.WhiteSmoke;
                    dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(70, 70, 70);
                    dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
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
            public readonly Dictionary<string, (IntPtr MS, IntPtr Steam)> _map
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
            Output("Attempting connection");
            string procName = "StateOfDecay2-Win64-Shipping";
            var process = Process.GetProcessesByName(procName).FirstOrDefault();
            if (process == null)
            { 
                Output($"Process {procName} not found.");
                return;
            }
            _proc = OpenProcess(0x001F0FFF, false, process.Id);
            _ba = process.MainModule.BaseAddress;
            connected = false;
            if (_proc == IntPtr.Zero)
            {
                lblVer.Text = "Not found.";
                Output("Connect Failed - Unable to open process.");
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
                    Output("Process found but unknown version.");
                }
            }        
        }
        
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (RUInt32(_ba + 0xE8) == 0)
            {
                connected = false;
                tlpEnclavesCharactersDetails.Controls.Clear();
                txtCharacterAddress.Text = "0";
                txtEnclaveAddress.Text = "0";
                _enclaveTable.Rows.Clear();
                _enclaveCharactersTable.Rows.Clear();



                lblVer.Text = "Not connected.";
            }
            else
            {
                world = new World(RIntPtr(addresses.Get("WorldPtr")));
                if (tabs.SelectedTab == null)
                    return;

                gameMode = world.GameMode;
                var persistentLevel = world.PersistentLevel;
                var worldSettings = persistentLevel.WorldSettings;
                eng = new GameEngine(RIntPtr(addresses.Get("GameEngine")));

                var enclaveManager = gameMode.EnclaveManager;
                localPlayer = new DaytonLocalPlayer(RIntPtr(addresses.Get("DaytonLocalPlayer")));

                currEnclave = localPlayer.DaytonPlayerController.AnalyticsPawn.CharacterComponent.DaytonCharacter.Enclave;
                currDaytonHumanCharacter = localPlayer.DaytonPlayerController.AnalyticsPawn;
                currDaytonCharacter = currDaytonHumanCharacter.CharacterComponent.DaytonCharacter;
                gameInstance = eng.GameInstance;



                switch (tabs.SelectedTab.Name)
                {
                    case "tabWorld":
                        string newWorldLabel = $"{world.Name}\n{persistentLevel.Name}\n{worldSettings.Name}";
                        if (newWorldLabel != lblWorldName.Text)
                            lblWorldName.Text = newWorldLabel;

                        chkIsDemo.Checked = RUInt8(addresses.Get("ULIsDemo")) > 0;
                        chkBlindZombies.Checked = localPlayer.DaytonPlayerController.CheatManager.bInvisibleToZombies;
                        chkSpawnerActive.Checked = gameMode.DynamicPawnSpawner.Active > 0;

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
                        lblWorldDayLength.Text = localPlayer.DaytonPlayerController.CommunityComponent.TimeOfDayComponent.DayLength.ToString();


                        txtConsoleAddress.Text = eng.GameViewport.ViewportConsole.BaseAddress.ToString("X");
                        txtCheatManagerAddress.Text = localPlayer.DaytonPlayerController.CheatManager.BaseAddress.ToString("X");
                        


                        break; //end tabWorld

                    case "tabCommunity":
                        UpdateCommunity();
                        break;
                    case "tabEnclaves":
                        string newEnclavesLabel = $"{world.Name}\n{gameMode.Name}\n{enclaveManager.Name}";
                        if (newEnclavesLabel != lblEnclavesLabel.Text)
                            lblEnclavesLabel.Text = newEnclavesLabel;

                        if (enclaveManager.Name == "EnclaveManagerBp")
                        {
                            string newNumEnclaves = $"{enclaveManager.NumEnclaves} Enclaves";
                            if (newNumEnclaves != lblNumEnclaves.Text)
                                lblNumEnclaves.Text = newNumEnclaves;

                            UpdateEnclaveList(enclaveManager);
                            UpdateEnclaveCharacterList();
                        }
                        

                        
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
                                switch (tabControlEnclavesCharacters.SelectedTab.Name)
                                {
                                    case "tabEnclavesCharactersDetails":
                                        UpdateCharacterDetails(chr);
                                        break;
                                    case "tabEnclavesCharactersSkills":
                                        UpdateCharacterSkills(chr);
                                        break;
                                    case "tabEnclavesCharactersTraits":
                                        lblCharacterTraits.Text = string.Join(Environment.NewLine, chr.CharacterRecord.TraitNames);
                                        break;
                                    case "tabEnclavesCharactersEquipment":
                                        switch (tabControlEnclavesCharactersEquipment.SelectedTab.Name)
                                        {
                                            case "tabControlEnclavesCharactersEquipmentMelee":
                                                UpdateMeleeWeaponDetails(chr.CharacterRecord.Equipment.MeleeWeaponItemInstance);
                                                lblMelee.Text = $"{chr.CharacterRecord.Equipment.MeleeWeaponItemInstance.ItemClass.Name}";
                                                break;
                                            case "tabControlEnclavesCharactersEquipmentSideArm":
                                                UpdateRangedWeaponDetails(tlpSideArmWeaponStats, chr.CharacterRecord.Equipment.SideArmRangedWeaponItemInstance);
                                                lblSidearm.Text = $"{chr.CharacterRecord.Equipment.SideArmRangedWeaponItemInstance.ItemClass.Name}";
                                                break;
                                            case "tabControlEnclavesCharactersEquipmentRanged":
                                                UpdateRangedWeaponDetails(tlpRangedWeaponStats, chr.CharacterRecord.Equipment.RangedWeaponItemInstance);
                                                lblRanged.Text = $"{chr.CharacterRecord.Equipment.RangedWeaponItemInstance.ItemClass.Name}";
                                                break;
                                        }
                                        lblEquipment.Text = $"{chr.CharacterRecord.Equipment.Name}";
                                        break;
                                    case "tabEnclavesCharactersInventory":
                                        UpdateCharacterInventoryList(chr);
                                        lblInventory.Text = $"{chr.CharacterRecord.Inventory.Name}";
                                        break;
                                }//end switch cha tab name
                                lblCharactersLabel.Text = $"{chr.Name}";
                            }// end if parsed char addr
                            else
                            {
                                lblCharactersLabel.Text = "";
                            }
                        } //end if parse enclave address
                        else 
                        { 
                            lblEnclaveDetails.Text = "";
                            lblEnclavesNumCharacters.Text = "x Characters";
                        }

                        break; //end tabEnclaves
                    case "tabSpawner":
                        var spawner = gameMode.DynamicPawnSpawner;

                        string newSpawnerLabel = $"{world.Name}\n{gameMode.Name}\n{spawner.Name}";
                        if (newSpawnerLabel != lblEnclavesLabel.Text)
                            lblSpawnerLabel.Text = newSpawnerLabel;

                        var spawnerdetails = new StringBuilder();
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

                        break; //end tabSpawner

                    case "tabGameLog":
                        UpdateLog();
                        UpdateLogLevels();
                        break;
                    case "tabInspector":
                        UpdateInspector();
                        break;//end tabInspector

                    default:
                        break;
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
            tlpEnclaveCharactersInventory.SuspendLayout();
            int slotCount = chr.CharacterRecord.Inventory.Slots.Count;
            if ((tlpEnclaveCharactersInventory.RowCount != slotCount + 1) || 
                (chr.CharacterRecord.Inventory.BaseAddress.ToString("X") != txtEnclavesCharactersInventoryAddress.Text))
            {
                _inventoryRows.Clear();
                tlpEnclaveCharactersInventory.RowCount = 0;
            }
            if (tlpEnclaveCharactersInventory.RowCount == 0)
                InitializeCharacterInventoryTable();


            txtEnclavesCharactersInventoryAddress.Text = chr.CharacterRecord.Inventory.BaseAddress.ToString("X");
            

            

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
                    if (item.stackCount > 0)
                    {
                        row.QuantityLabel.Visible = true;
                        row.EditButton.Visible = true;
                    }
                    else
                    {
                        row.QuantityLabel.Visible = false;
                        row.EditButton.Visible = false;
                    }
                    
                }
            }
            tlpEnclaveCharactersInventory.RowCount = slotCount + 1;
            tlpEnclaveCharactersInventory.ResumeLayout();
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
                        Width = 25,
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
            AddRow("First Name", editable: true, onEdit: v => { selectedChar.CharacterRecord.FirstName = v; });
            AddRow("Last Name", editable: true, onEdit: v => { selectedChar.CharacterRecord.LastName = v; });
            AddRow("Nickname", editable: true, onEdit: v => { selectedChar.CharacterRecord.NickName = v; });
            AddRow("Voice ID");
            AddRow("Cultural Background");
            AddRow("Human Definition");
            AddRow("Philosophy1");
            AddRow("Philosophy2");
            AddRow("Hero Bonus");
            AddRow("Leader Type");
            AddRow("Zombies Killed");
            AddRow("Standing Level", editable: true, onEdit: v => { selectedChar.CharacterRecord.StandingLevel = (ECharacterStanding)byte.Parse(v); });
            AddRow("Current Standing", editable: true, onEdit: v => { selectedChar.CharacterRecord.CurrStanding = float.Parse(v);  });
            AddRow("Current Fatigue", editable: true, onEdit: v => {  selectedChar.CharacterRecord.CurrFatigue = float.Parse(v);  });
            AddRow("Current Stamina", editable: true, onEdit: v => { selectedChar.CharacterRecord.CurrStam = float.Parse(v); });
            AddRow("Max Stamina Base");
            AddRow("Max Stamina Mod");
            AddRow("Current Health", editable: true, onEdit: v => { selectedChar.CharacterRecord.CurrHealth = float.Parse(v); });
            AddRow("Max Health Base");
            AddRow("Max Health Mod");
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

            //TODO: Instead of AddRow, just add an extra ++ to RowCount and test


            tlpEnclavesCharactersDetails.ResumeLayout();
        }
        private void UpdateCharacterDetails(DaytonCharacter chr)
        {
            if (_characterDetailRows.Count == 0)
                InitializeCharacterTable();

            _characterDetailRows["ID"].ValueLabel.Text = chr.CharacterRecord.ID.ToString();
            _characterDetailRows["First Name"].ValueLabel.Text = chr.CharacterRecord.FirstName;
            _characterDetailRows["Last Name"].ValueLabel.Text = chr.CharacterRecord.LastName;
            _characterDetailRows["Nickname"].ValueLabel.Text = chr.CharacterRecord.NickName;
            _characterDetailRows["Voice ID"].ValueLabel.Text = chr.CharacterRecord.VoiceID;
            _characterDetailRows["Cultural Background"].ValueLabel.Text = chr.CharacterRecord.CulturalBackground;
            _characterDetailRows["Human Definition"].ValueLabel.Text = chr.CharacterRecord.HumanDefinition;
            _characterDetailRows["Philosophy1"].ValueLabel.Text = chr.CharacterRecord.Philosophy1.ToString();
            _characterDetailRows["Philosophy2"].ValueLabel.Text = chr.CharacterRecord.Philosophy2.ToString();
            _characterDetailRows["Hero Bonus"].ValueLabel.Text = chr.CharacterRecord.HeroBonus;
            _characterDetailRows["Leader Type"].ValueLabel.Text = chr.CharacterRecord.LeaderType;
            _characterDetailRows["Zombies Killed"].ValueLabel.Text = chr.CharacterRecord.ZombiesKilled.ToString();
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
            _characterDetailRows["Max Stamina Mod"].ValueLabel.Text = $"{chr.MaxStaminaModifier,12:F4}";
            _characterDetailRows["Max Health Base"].ValueLabel.Text = $"{chr.MaxHealthBase,12:F4}";
            _characterDetailRows["Max Health Mod"].ValueLabel.Text = $"{chr.MaxHealthModifier,12:F4}";
            _characterDetailRows["Trauma"].ValueLabel.Text = $"{chr.Trauma,12:F4}";
            _characterDetailRows["Max Sick"].ValueLabel.Text = $"{chr.MaxSick,12:F4}";
            _characterDetailRows["Max Plague"].ValueLabel.Text = $"{chr.MaxPlague,12:F4}";
            _characterDetailRows["Enclave Name"].ValueLabel.Text = chr.Enclave?.DisplayName ?? "N/A";
            _characterDetailRows["IsInWorld"].ValueLabel.Text = $"{chr.IsInWorld,7}";
            _characterDetailRows["Position X"].ValueLabel.Text = $"{chr.XPos,12:F4}";
            _characterDetailRows["Position Y"].ValueLabel.Text = $"{chr.YPos,12:F4}";
            _characterDetailRows["Position Z"].ValueLabel.Text = $"{chr.ZPos,12:F4}";   
        }

        



        



        private class WeaponDetailRowEntry
        {
            public Label ValueLabel { get; set; }
            public Button EditButton { get; set; }
        }

        private Dictionary<string, WeaponDetailRowEntry> _meleeWeaponDetailRows = new Dictionary<string, WeaponDetailRowEntry>();
        private Dictionary<string, WeaponDetailRowEntry> _rangedWeaponDetailRows = new Dictionary<string, WeaponDetailRowEntry>();

        private void InitializeMeleeWeaponTable(MeleeWeaponItemInstance meleeWeapon)
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

            MeleeWeaponResourceStats meleeStats = meleeWeapon.Stats;
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
            AddRow("Durability", editable: true, onEdit: v => { meleeWeapon.Durability = float.Parse(v); });
            AddRow("DurabilityMax", editable: true, onEdit: v => { meleeStats.Durability = float.Parse(v); });
            AddRow("DurabilityLossPerHitMin", editable: true, onEdit: v => { meleeStats.DurabilityLossPerHitMin = float.Parse(v); });
            AddRow("DurabilityLossPerHitMax", editable: true, onEdit: v => { meleeStats.DurabilityLossPerHitMax = float.Parse(v); });
            AddRow("DurabilityLossPerFinisherMin", editable: true, onEdit: v => { meleeStats.DurabilityLossPerFinisherMin = float.Parse(v); });
            AddRow("DurabilityLossPerFinisherMax", editable: true, onEdit: v => { meleeStats.DurabilityLossPerFinisherMax = float.Parse(v); });

            AddRow("");

            tlpMeleeWeaponStats.ResumeLayout();
        }
        private void UpdateMeleeWeaponDetails(MeleeWeaponItemInstance weapon)
        {

            MeleeWeaponResourceStats weaponStats = weapon.Stats;
            string weaponName = weaponStats.WeaponName;
            weaponName = weaponName.Replace("{!v}", "");
            weaponName = weaponName.Replace("{[0,+]'}", "");
            weaponName = weaponName.Replace("{[0,+]s}", "");
            weaponName = weaponName.Replace("{[0,+]es}", "");


            if ((_meleeWeaponDetailRows.Count != 0) &&
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
            _meleeWeaponDetailRows["Weapon Type"].ValueLabel.Text = weaponStats.WeaponType;

            string weaponDesc = weaponStats.WeaponDesc;
            weaponDesc = weaponDesc.Replace("<br>", Environment.NewLine);
            weaponDesc = weaponDesc.Replace("{color}", "");
            weaponDesc = weaponDesc.Replace("{color:highlight}","");
            _meleeWeaponDetailRows["Weapon Desc"].ValueLabel.Text = weaponDesc;
            _meleeWeaponDetailRows["Weapon Desc"].ValueLabel.BorderStyle = BorderStyle.FixedSingle;

            _meleeWeaponDetailRows["Melee Type"].ValueLabel.Text = weaponStats.MeleeType.ToString();

            _meleeWeaponDetailRows["Dismember"].ValueLabel.Text = $"{weaponStats.Dismember,12:F4}";
            _meleeWeaponDetailRows["DismemberDelta"].ValueLabel.Text = $"{weaponStats.DismemberDelta,12:F4}";
            _meleeWeaponDetailRows["Impact"].ValueLabel.Text = $"{weaponStats.Impact,12:F4}";
            _meleeWeaponDetailRows["ImpactDelta"].ValueLabel.Text = $"{weaponStats.ImpactDelta,12:F4}";
            _meleeWeaponDetailRows["Knockdown"].ValueLabel.Text = $"{weaponStats.Knockdown,12:F4}";
            _meleeWeaponDetailRows["KnockdownDelta"].ValueLabel.Text = $"{weaponStats.KnockdownDelta,12:F4}";
            _meleeWeaponDetailRows["Lethality"].ValueLabel.Text = $"{weaponStats.Lethality,12:F4}";
            _meleeWeaponDetailRows["LethalityDelta"].ValueLabel.Text = $"{weaponStats.LethalityDelta,12:F4}";
            _meleeWeaponDetailRows["Weight"].ValueLabel.Text = $"{weaponStats.Weight,12:F4}";
            _meleeWeaponDetailRows["PerceptionLoudness"].ValueLabel.Text = $"{weaponStats.PerceptionLoudness,12:F4}";
            _meleeWeaponDetailRows["Speed"].ValueLabel.Text = $"{weaponStats.Speed,12:F4}";
            _meleeWeaponDetailRows["SwingCost"].ValueLabel.Text = $"{weaponStats.SwingCost,12:F4}";
            _meleeWeaponDetailRows["InfluenceValue"].ValueLabel.Text = $"{weaponStats.InfluenceValue,12:F4}";
            _meleeWeaponDetailRows["PrestigeValue"].ValueLabel.Text = $"{weaponStats.PrestigeValue,12:F4}";
            _meleeWeaponDetailRows["Durability"].ValueLabel.Text = $"{weapon.Durability,12:F4}";
            _meleeWeaponDetailRows["DurabilityMax"].ValueLabel.Text = $"{weaponStats.Durability,12:F4}";
            _meleeWeaponDetailRows["DurabilityLossPerHitMin"].ValueLabel.Text = $"{weaponStats.DurabilityLossPerHitMin,12:F4}";
            _meleeWeaponDetailRows["DurabilityLossPerHitMax"].ValueLabel.Text = $"{weaponStats.DurabilityLossPerHitMax,12:F4}";
            _meleeWeaponDetailRows["DurabilityLossPerFinisherMin"].ValueLabel.Text = $"{weaponStats.DurabilityLossPerFinisherMin,12:F4}";
            _meleeWeaponDetailRows["DurabilityLossPerFinisherMax"].ValueLabel.Text = $"{weaponStats.DurabilityLossPerFinisherMax,12:F4}";
        }
        private void InitializeRangedWeaponTable(TableLayoutPanel tlp, RangedWeaponItemInstance rangedWeapon)
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

            RangedWeaponResourceStats rangedStats = rangedWeapon.RangedWeapon.Stats;

            AddRow("Weapon Name");
            AddRow("Weapon Type");
            AddRow("Weapon Desc");

            AddRow("KickAngle", editable: true, onEdit: v => { rangedStats.KickAngle = float.Parse(v); });
            AddRow("ProjectileCount", editable: true, onEdit: v => { rangedStats.ProjectileCount = int.Parse(v); });
            AddRow("MagazineSize", editable: true, onEdit: v => { rangedStats.MagazineSize = int.Parse(v); });
            AddRow("AmmoConsumedPerShot", editable: true, onEdit: v => { rangedStats.AmmoConsumedPerShot = int.Parse(v); });
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
            AddRow("Durability", editable: true, onEdit: v => { rangedWeapon.Durability = float.Parse(v); });
            AddRow("DurabilityMax", editable: true, onEdit: v => { rangedStats.Durability = float.Parse(v); });
            AddRow("DurabilityLossPerShotMin", editable: true, onEdit: v => { rangedStats.DurabilityLossPerShotMin = float.Parse(v); });
            AddRow("DurabilityLossPerShotMax", editable: true, onEdit: v => { rangedStats.DurabilityLossPerShotMax = float.Parse(v); });
            AddRow("PerceptionLoudness", editable: true, onEdit: v => { rangedStats.PerceptionLoudness = float.Parse(v); });
            AddRow("TracerSettings");

            AddRow("");

            tlp.ResumeLayout();
        }
        private void UpdateRangedWeaponDetails(TableLayoutPanel tlp, RangedWeaponItemInstance weapon)
        {

            RangedWeaponResourceStats rangedStats = weapon.RangedWeapon.Stats;

            string weaponName = rangedStats.WeaponName;
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
            _rangedWeaponDetailRows["Weapon Type"].ValueLabel.Text = rangedStats.WeaponType;

            string weaponDesc = rangedStats.WeaponDesc;
            weaponDesc = weaponDesc.Replace("<br>", Environment.NewLine);
            weaponDesc = weaponDesc.Replace("{/color}", "");
            weaponDesc = weaponDesc.Replace("{color:highlight}", "");
            _rangedWeaponDetailRows["Weapon Desc"].ValueLabel.Text = weaponDesc;
            _rangedWeaponDetailRows["Weapon Desc"].ValueLabel.BorderStyle = BorderStyle.FixedSingle;

            _rangedWeaponDetailRows["KickAngle"].ValueLabel.Text = $"{rangedStats.KickAngle,12:F4}";
            _rangedWeaponDetailRows["ProjectileCount"].ValueLabel.Text = $"{rangedStats.ProjectileCount,7}";
            _rangedWeaponDetailRows["MagazineSize"].ValueLabel.Text = $"{rangedStats.MagazineSize,7}";
            _rangedWeaponDetailRows["AmmoConsumedPerShot"].ValueLabel.Text = $"{rangedStats.AmmoConsumedPerShot,7}";
            _rangedWeaponDetailRows["Impact"].ValueLabel.Text = $"{rangedStats.Impact,12:F4}";
            _rangedWeaponDetailRows["ImpactDelta"].ValueLabel.Text = $"{rangedStats.ImpactDelta,12:F4}";
            _rangedWeaponDetailRows["Knockdown"].ValueLabel.Text = $"{rangedStats.Knockdown,12:F4}";
            _rangedWeaponDetailRows["KnockdownDelta"].ValueLabel.Text = $"{rangedStats.KnockdownDelta,12:F4}";
            _rangedWeaponDetailRows["Dismember"].ValueLabel.Text = $"{rangedStats.Dismember,12:F4}";
            _rangedWeaponDetailRows["DismemberDelta"].ValueLabel.Text = $"{rangedStats.DismemberDelta,12:F4}";
            _rangedWeaponDetailRows["Penetration"].ValueLabel.Text = $"{rangedStats.Penetration,12:F4}";
            _rangedWeaponDetailRows["Range"].ValueLabel.Text = $"{rangedStats.Range,12:F4}";
            _rangedWeaponDetailRows["Weight"].ValueLabel.Text = $"{rangedStats.Weight,12:F4}";
            _rangedWeaponDetailRows["InfluenceValue"].ValueLabel.Text = $"{rangedStats.InfluenceValue,7}";
            _rangedWeaponDetailRows["PrestigeValue"].ValueLabel.Text = $"{rangedStats.PrestigeValue,7}";
            _rangedWeaponDetailRows["Durability"].ValueLabel.Text = $"{weapon.Durability,12:F4}";
            _rangedWeaponDetailRows["DurabilityMax"].ValueLabel.Text = $"{rangedStats.Durability,12:F4}";
            _rangedWeaponDetailRows["DurabilityLossPerShotMin"].ValueLabel.Text = $"{rangedStats.DurabilityLossPerShotMin,12:F4}";
            _rangedWeaponDetailRows["DurabilityLossPerShotMax"].ValueLabel.Text = $"{rangedStats.DurabilityLossPerShotMax,12:F4}";
            _rangedWeaponDetailRows["PerceptionLoudness"].ValueLabel.Text = $"{rangedStats.PerceptionLoudness,12:F4}";
            _rangedWeaponDetailRows["TracerSettings"].ValueLabel.Text = rangedStats.TracerSettings;
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
                WUInt8(gamelogs.Get(logName), 0);
            }
        }
        private void btnGameLogLogLevelMax_Click(object sender, EventArgs e)
        {
            foreach (string logName in logNames)
            {
                WUInt8(gamelogs.Get(logName), 9);
            }
        }
        private void btnGameLogClear_Click(object sender, EventArgs e)
        {
            rtbGameLog.Text = string.Empty;
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
        

        

        private void btnDiscord_Click(object sender, EventArgs e)
        {
            //https://discord.gg/UT5yFag7Xk
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://discord.gg/UT5yFag7Xk",
                UseShellExecute = true
            });
        }

        
        

        private void btnEnableConsole_Click(object sender, EventArgs e)
        {
            DaytonLocalPlayer localPlayer = new DaytonLocalPlayer(RIntPtr(addresses.Get("DaytonLocalPlayer")));

            GameEngine eng = new GameEngine(RIntPtr(addresses.Get("GameEngine")));
            UConsole con = new UConsole(RIntPtr(RIntPtr(addresses.Get("UConsoleClass")) + 0x100));
            if (con.BaseAddress != IntPtr.Zero)
            {
                eng.GameViewport.ViewportConsole = con;
                eng.GameViewport.ViewportConsole.ConsoleTargetPlayer = localPlayer;
                if (eng.GameViewport.ViewportConsole.ConsoleTargetPlayer.BaseAddress != IntPtr.Zero)
                {
                    Output("Console Enabled - Press Insert in-game to use");
                }
                else
                {
                    Output("Local Player not found.  Try again later, or report this issue.");
                }
            }
            else
            {
                Output("Failed to Create Console.");
            }



            
        }

        

       

        



        IntPtr ItemFunc = IntPtr.Zero;
        IntPtr Items = IntPtr.Zero;
        private void btnTest_Click(object sender, EventArgs e)
        {
            /*ulong inv = (ulong)currDaytonCharacter.CharacterRecord.Inventory.BaseAddress;
            if (inv == 0)
            {
                Output("Inventory pointer not found.");
                return;
            }
            int Quantity = 1;

            //ulong ItemBP = 0x00000000775EB280;
            ulong ItemBP = 0x2CCC6C80;
            ulong FuncCreateItemInstance = (ulong)(_ba + 0x4efce0);
            ulong FuncTryAddItem = (ulong)(_ba + 0x502010);

            UObject testObj = new UObject((IntPtr)ItemBP);
            if (testObj.Type != "BlueprintGeneratedClass")
            {
                Output("Provided object is not BlueprintGeneratedClass, aborting Add.");
                return;
            }

            IntPtr CodeAddItem = Alloc(0x1000);
            Iced.Intel.Assembler asm = new Iced.Intel.Assembler(bitness: 64);
            
            asm.sub(rsp, 0x1000);
            asm.pushfq();


            asm.mov(rcx, ItemBP);  //Item blueprintclass
            asm.mov(rdx, Quantity);
            asm.call(FuncCreateItemInstance);  //Create Item Instance

            asm.mov(rcx, inv);
            asm.mov(rdx, rax);
            asm.mov(r8, 7);
            asm.mov(r9, 1);
            asm.mov(rax, rsp);
            asm.add(rax, 0x20);
            asm.mov(rbx, 1);
            asm.mov(__[rax], rbx);
            asm.call(FuncTryAddItem);


            asm.popfq();
            asm.add(rsp, 0x1000);
            asm.ret();

            var stream = new MemoryStream();
            asm.Assemble(new Iced.Intel.StreamCodeWriter(stream), (ulong)CodeAddItem);
            byte[] machineCode = stream.ToArray();
            WBytes(CodeAddItem, machineCode);

            Console.WriteLine(CodeAddItem.ToString("X"));
            CreateRemoteThread(_proc, IntPtr.Zero, 0, CodeAddItem, IntPtr.Zero, 0, IntPtr.Zero);*/


            if (ItemFunc == IntPtr.Zero)
                ItemFunc = Alloc(0x1000);
            if (Items == IntPtr.Zero)
                Items = Alloc(0x1000);

            Iced.Intel.Assembler asm = new Iced.Intel.Assembler(bitness: 64);

            asm.sub(rsp, 0x1000);
            asm.pushfq();
            
            
            asm.mov(r8, (ulong)(_ba + 0x4f3ca0));   //ConsumableItems
            asm.mov(rcx, (ulong)Items);
            asm.call(r8);

            asm.mov(r8, (ulong)(_ba + 0x4f3a00));   //CloseCombatItems
            asm.mov(rcx, (ulong)Items);
            asm.add(rcx, 0x10);
            asm.call(r8);

            asm.mov(r8, (ulong)(_ba + 0x51f550));   //MeleeWeaponItems
            asm.mov(rcx, (ulong)Items);
            asm.add(rcx, 0x20);
            asm.call(r8);

            asm.mov(r8, (ulong)(_ba + 0x51f820));   //RangedWeaponItems
            asm.mov(rcx, (ulong)Items);
            asm.add(rcx, 0x30);
            asm.call(r8);

            asm.popfq();
            asm.add(rsp, 0x1000);
            asm.ret();

            var stream = new MemoryStream();
            asm.Assemble(new Iced.Intel.StreamCodeWriter(stream), (ulong)ItemFunc);
            byte[] machineCode = stream.ToArray();
            WBytes(ItemFunc, machineCode);
            Console.WriteLine(Items.ToString("X"));

            CreateRemoteThread(_proc, IntPtr.Zero, 0, ItemFunc, IntPtr.Zero, 0, IntPtr.Zero); 
        }




        private void btnTest2_Click(object sender, EventArgs e)
        {
            IntPtr ptr = (IntPtr)0x6b785b40;
            int num = 0x69;

            for (int i = 0; i < num; i++)
            {
                IntPtr itemPtr = RIntPtr(ptr + i * IntPtr.Size);
                UObject obj = new UObject(itemPtr);
                FText DisplayName = new FText(itemPtr + 0x28);
                float infVal = RInt32(itemPtr + 0xf4);

                Console.WriteLine($"{infVal}    {DisplayName.Value.Replace("{[0,+]s}", "")}");
            }
            
            /*
            UObject obj = new UObject((IntPtr)0x775EB280);
            Console.WriteLine(obj.Name);
            Console.WriteLine(obj.Type);
            Console.WriteLine(obj.Class.Name);
            Console.WriteLine(obj.Class.Path());
            Console.WriteLine(obj.Path());
            */

            /*
            IntPtr objTablePtr = addresses.Get("ObjTablePtr");
            int numobjs = RInt32(objTablePtr - 0x8);
            objTablePtr = RIntPtr(objTablePtr);


            List<UObject> ammo = new List<UObject>();
            List<UObject> backpack = new List<UObject>();
            List<UObject> ccw = new List<UObject>();
            List<UObject> cons = new List<UObject>();
            List<UObject> ranged = new List<UObject>();
            List<UObject> facilitymods = new List<UObject>();
            List<UObject> melee = new List<UObject>();
            List<UObject> misc = new List<UObject>();
            List<UObject> mods = new List<UObject>();
            List<UObject> resource = new List<UObject>();

            for (int i = 0; i < numobjs; i++)
            {
                UObject obj = new UObject(RIntPtr(objTablePtr + i * 0x30));
                string path = obj.Path();
                if (path.Contains("/Game/Item") && (obj.Type == "BlueprintGeneratedClass"))
                //if (obj.Type == "BlueprintGeneratedClass")
                {
                    if (path.StartsWith("/Game/Items/BackpackItems"))
                        backpack.Add(obj);
                    else if (path.StartsWith("/Game/Items/MeleeWeapons"))
                        melee.Add(obj);
                    else if (path.StartsWith("/Game/Items/Mods"))
                        mods.Add(obj);
                    else if (path.StartsWith("/Game/Items/CloseCombatItems"))
                        ccw.Add(obj);
                    else if (path.StartsWith("/Game/Items/Consumables"))
                        cons.Add(obj);
                    else if (path.StartsWith("/Game/Items/RangedWeapons"))
                        ranged.Add(obj);
                    else if (path.StartsWith("/Game/Items/ResourceItems"))
                        resource.Add(obj);
                    else if (path.StartsWith("/Game/Items/MiscellaneousItems"))
                        misc.Add(obj);
                    else if (path.StartsWith("/Game/Items/FacilityModItems"))
                        facilitymods.Add(obj);
                    else if (path.StartsWith("/Game/Items/Ammo"))
                        ammo.Add(obj);
                    else
                        Console.WriteLine($"{obj.Path()}   {obj.BaseAddress.ToString("X")} {obj.Type} - {path}");
                }
            }
            
            

            foreach (UObject obj2 in backpack)
                Console.WriteLine($"{obj2.BaseAddress.ToString("X")} - {obj2.Name}");*/

            /*foreach (var catalog in gameInstance.ItemCatalogManager.Catalogs)
            {
                Console.WriteLine(catalog.ID);
                foreach (var bi in catalog.BountyItems)
                    Console.WriteLine($"BountyItem: {bi}");
                foreach (var wi in catalog.WorldItems)
                    Console.WriteLine($"WorldItem: {wi}");
            }*/
        }

        private void btnEnableCheats_Click(object sender, EventArgs e)
        {
            IntPtr dcmcp = RIntPtr(addresses.Get("UDaytonCheatManagerClass"));
            DaytonLocalPlayer localPlayer = new DaytonLocalPlayer(RIntPtr(addresses.Get("DaytonLocalPlayer")));
            DaytonPlayerController dpc = localPlayer.DaytonPlayerController;

            UClass dcmc = new UClass(dcmcp);
            DaytonCheatManager dcm = new DaytonCheatManager(dcmc.ClassDefaultObject.BaseAddress);
            dcm.Outer = dpc;
            dpc.CheatManager = dcm;

            if (dpc.CheatManager.BaseAddress != IntPtr.Zero)
            {
                Output("Cheat Manager Enabled - Press ] in-game to toggle menu.");
            }
            else
            {
                Output("Huh.  That didn't work for some reason.  Weird.");
            }
        }
        private void btnEditDilation_Click(object sender, EventArgs e)
        {
            var gameMode = world.GameMode;
            var persistentLevel = world.PersistentLevel;
            var worldSettings = persistentLevel.WorldSettings;
            string input = ShowNumericInputDialog("Edit Time Dilation", $"Enter new time dilation", worldSettings.TimeDilation.ToString(), isFloat: true);
            if (!string.IsNullOrWhiteSpace(input))
            {
                worldSettings.TimeDilation = float.Parse(input);
            }
        }

        private void btnEditToD_Click(object sender, EventArgs e)
        {
            DaytonLocalPlayer localPlayer = new DaytonLocalPlayer(RIntPtr(addresses.Get("DaytonLocalPlayer")));
            string input = ShowNumericInputDialog("Edit Time Dilation", $"Enter new time dilation", localPlayer.DaytonPlayerController.CommunityComponent.TimeOfDayComponent.TimeOfDay.ToString(), isFloat: true);
            if (!string.IsNullOrWhiteSpace(input))
            {
                localPlayer.DaytonPlayerController.CommunityComponent.TimeOfDayComponent.TimeOfDay = float.Parse(input);
            }
        }
        private void btnEditDayLength_Click(object sender, EventArgs e)
        {
            DaytonLocalPlayer localPlayer = new DaytonLocalPlayer(RIntPtr(addresses.Get("DaytonLocalPlayer")));
            string input = ShowNumericInputDialog("Edit Day Length", $"Enter new day length", localPlayer.DaytonPlayerController.CommunityComponent.TimeOfDayComponent.DayLength.ToString(), isFloat: true);
            if (!string.IsNullOrWhiteSpace(input))
            {
                localPlayer.DaytonPlayerController.CommunityComponent.TimeOfDayComponent.DayLength = float.Parse(input);
            }
        }

        private void btnWarpToEnclave_Click(object sender, EventArgs e)
        {
            Enclave _currentEnclave = null;
            if (long.TryParse(txtEnclaveAddress.Text, System.Globalization.NumberStyles.HexNumber, null, out long encaddr) && encaddr > 0)
            {
                IntPtr encPtr = (IntPtr)encaddr;
                _currentEnclave = new Enclave(encPtr);
            }
            else
            {
                Output("Failed to find enclave.");
                return;
            }
                

            if (_currentEnclave.Name == "Enclave")
            {
                currDaytonHumanCharacter.CapsuleComponent.WarpPos = _currentEnclave.BaseCenter;
                Output("Warp successful.");
            }
            else
                Output($"Aborting Warp - Enclave has incorrect name - {_currentEnclave.Name}");
        }

        private void btnEnclaveWarpToCharacter_Click(object sender, EventArgs e)
        {
            DaytonCharacter _currentCharacter = null;
            if (long.TryParse(txtCharacterAddress.Text, System.Globalization.NumberStyles.HexNumber, null, out long chraddr) && chraddr > 0)
            {
                IntPtr chrPtr = (IntPtr)chraddr;
                _currentCharacter = new DaytonCharacter(chrPtr);
            }
            else
            {
                Output("Failed to find character.");
                return;
            }    
            if (_currentCharacter.Name == "DaytonCharacter")
            {
                currDaytonHumanCharacter.CapsuleComponent.WarpPos = _currentCharacter.Position;
                Output("Warp successful.");
            }
            else
                Output($"Aborting Warp - Character has incorrect name - {_currentCharacter.Name}");
        }

        private void btnWarpToWaypoint_Click(object sender, EventArgs e)
        {
            if (localPlayer.DaytonPlayerController.MapUI.MapState.numWaypointPositions > 0)
            {
                Vector2 wpPos = new Vector2();
                wpPos = localPlayer.DaytonPlayerController.MapUI.MapState.WaypointPositions[0];
                Vector2 mapMin = localPlayer.DaytonPlayerController.MapUI.MapBoundsMin;
                Vector2 mapMax = localPlayer.DaytonPlayerController.MapUI.MapBoundsMax;
                Vector2 normMapMax = new Vector2(Math.Abs(mapMax.X) + Math.Abs(mapMin.X), Math.Abs(mapMax.Y) + Math.Abs(mapMin.Y));
                wpPos = wpPos * normMapMax;
                wpPos = wpPos - (new Vector2(Math.Abs(mapMin.X), Math.Abs(mapMin.Y)));
                currDaytonHumanCharacter.CapsuleComponent.WarpPos = new Vector3(wpPos.X, wpPos.Y, currDaytonHumanCharacter.CapsuleComponent.WarpPos.Z + 10000);
            }
            else
                Output("No MapUI waypoints found");
        }

        
    }
}