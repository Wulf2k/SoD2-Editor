using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static SoD2_Editor.Form1;

namespace SoD2_Editor
{
    public partial class Form1
    {
        public enum CharacterStanding : byte
        {
            Stranger = 0,
            Recruit = 1,
            Citizen = 2,
            Hero = 3,
            Leader = 4,
            Count = 5,
            MAX = 6
        }
        public enum EnclaveType : byte
        {
            Default = 0,
            AmbientMission = 1,
            Legacy = 2,
            RadioTrader = 3,
            Persistent = 4,
            Num = 5,
            MAX = 6
        }




















        public class UObject
        {
            public IntPtr BaseAddress { get; set; }

            public UObject(IntPtr addr)
            {
                BaseAddress = addr;
            }

            public string Name
            {
                get
                {
                    int nameId = RInt32(BaseAddress + 0x18);
                    return GetNameFromNameOffset(nameId);
                }
            }
        }

        public class CommunityComponent : UObject
        {
            public CommunityComponent(IntPtr addr) : base(addr) { }
            public IntPtr MoraleSettingsPtr => RIntPtr(BaseAddress + 0x168);
            public IntPtr BuffResourceManagerPtr => RIntPtr(BaseAddress + 0x190);
            public IntPtr TimeOfDayComponentPtr => RIntPtr(BaseAddress + 0x1a0);
            public float CommunityStandingHighWaterMark => RSingle(BaseAddress + 0x12f0);
            public float PlayTime => RSingle(BaseAddress + 0x1308);
        }
        public class DaytonCharacter : UObject
        {
            public DaytonCharacter(IntPtr addr) : base(addr)
            { }

            public int ID
            {
                get => RInt32(BaseAddress + 0x368);
                set => WInt32(BaseAddress + 0x368, value);
            }

            public string FirstName
            {
                get
                {
                    IntPtr textPropPtr = RIntPtr(BaseAddress + 0x380);
                    if (textPropPtr == IntPtr.Zero)
                        return string.Empty;
                    var textProp = new TextProperty(textPropPtr);
                    return textProp.Value;
                }
            }
            public string LastName
            {
                get
                {
                    IntPtr textPropPtr = RIntPtr(BaseAddress + 0x398);
                    if (textPropPtr == IntPtr.Zero)
                        return string.Empty;
                    var textProp = new TextProperty(textPropPtr);
                    return textProp.Value;
                }
            }
            public CharacterStanding StandingLevel => (CharacterStanding)RUInt8(BaseAddress + 0x412);
            public string HeroBonus
            {
                get
                {
                    int id = RInt32(BaseAddress + 0x418);
                    return GetNameFromNameOffset(id);
                }
            }

            public string LeaderType
            {
                get
                {
                    int id = RInt32(BaseAddress + 0x428);
                    return GetNameFromNameOffset(id);
                }
            }

            public List<string> TraitNames
            {
                get
                {
                    var traits = new List<string>();

                    IntPtr traitsPtr = RIntPtr(BaseAddress + 0x460);
                    if (traitsPtr == IntPtr.Zero)
                        return traits;

                    int numTraits = RInt32(BaseAddress + 0x468);
                    if (numTraits <= 0)
                        return traits;

                    for (int i = 0; i < numTraits; i++)
                    {
                        IntPtr traitBase = traitsPtr + (i * 0x18);

                        int nameId = RInt32(traitBase + 0x18);
                        if (nameId != 0)
                            traits.Add(GetNameFromNameOffset(nameId));
                    }

                    return traits;
                }
            }

            public float CurrStanding
            {
                get => RSingle(BaseAddress + 0x430);
                set => WSingle(BaseAddress + 0x430, value);
            }

            public float CurrHealth
            {
                get => RSingle(BaseAddress + 0x470);
                set => WSingle(BaseAddress + 0x470, value);
            }

            public float CurrStam
            {
                get => RSingle(BaseAddress + 0x474);
                set => WSingle(BaseAddress + 0x474, value);
            }

            public float CurrFatigue
            {
                get => RSingle(BaseAddress + 0x478);
                set => WSingle(BaseAddress + 0x478, value);
            }

            public float CurrSick
            {
                get => RSingle(BaseAddress + 0x480);
                set => WSingle(BaseAddress + 0x480, value);
            }

            public float CurrPlague
            {
                get => RSingle(BaseAddress + 0x484);
                set => WSingle(BaseAddress + 0x484, value);
            }

            public float MaxStaminaBase => RSingle(BaseAddress + 0xAF4);

            public float MaxHealthBase => RSingle(BaseAddress + 0xB00);

            public float MaxSick
            {
                get => RSingle(BaseAddress + 0xB38);
                set => WSingle(BaseAddress + 0xB38, value);
            }

            public float MaxPlague
            {
                get => RSingle(BaseAddress + 0xB44);
                set => WSingle(BaseAddress + 0xB44, value);
            }

            public Enclave Enclave => new Enclave(RIntPtr(BaseAddress + 0xE28));

            public float XPos => RSingle(BaseAddress + 0xFF0);

            public float YPos => RSingle(BaseAddress + 0xFF4);

            public float ZPos => RSingle(BaseAddress + 0xFF8);
            public byte IsInWorld => RUInt8(BaseAddress + 0x1028);
        }
        public class DaytonCharacterComponent : UObject
        {
            public DaytonCharacterComponent(IntPtr addr) : base(addr) { }
            public DaytonCharacter Character => new DaytonCharacter(RIntPtr(BaseAddress + 0xB8));
        }

        public class DaytonGameGameMode : UObject
        {
            public DaytonGameGameMode(IntPtr addr) : base(addr) { }

            public EnclaveManager EnclaveManager
            {
                get
                {
                    IntPtr emPtr = RIntPtr(BaseAddress + 0x458);
                    return new EnclaveManager(emPtr);
                }
            }
            public DynamicPawnSpawner DynamicPawnSpawner
            {
                get
                {
                    IntPtr dpsPtr = RIntPtr(BaseAddress + 0x4c8);
                    return new DynamicPawnSpawner(dpsPtr);
                }
            }
        }
        public class DaytonGameInstance : GameInstance
        {
            public DaytonGameInstance(IntPtr addr) : base(addr) { }
        }

        public class DaytonGameMode : UObject
        {
            public DaytonGameMode(IntPtr addr) : base(addr) { }

            public EnclaveManager EnclaveManager
            {
                get
                {
                    IntPtr emPtr = RIntPtr(BaseAddress + 0x458);
                    return new EnclaveManager(emPtr);
                }
            }
        }
        public class DaytonHumanCharacter :UObject
        {
            public DaytonHumanCharacter(IntPtr addr) : base(addr) { }
            public DaytonCharacterComponent CharacterComponent => new DaytonCharacterComponent(RIntPtr(BaseAddress + 0xBa0));
        }
        public class DaytonLocalPlayer :UObject
        {
            public DaytonLocalPlayer(IntPtr addr) : base(addr) { }
            public DaytonPlayerController DaytonPlayerController
            {
                get
                {
                    IntPtr dpcptr = RIntPtr(BaseAddress + 0x30);
                    return new DaytonPlayerController(dpcptr);
                }
            }
        }
        public class DynamicPawnSpawner : UObject
        {
            public DynamicPawnSpawner(IntPtr addr) : base(addr) { }
            public byte Active => RUInt8(BaseAddress + 0x370);
            public byte bIsUnlimitedSpawningEnabled => RUInt8(BaseAddress + 0x37c);
            public byte bIsOHKOZombiesEnabled => RUInt8(BaseAddress + 0x37d);
            public float MaxPopulationMultiplier => RSingle(BaseAddress + 0x460);
            public float MaxKillsPerCell => RSingle(BaseAddress + 0x484);
            public float MinKillsForAdjacencyBonus => RSingle(BaseAddress + 0x488);
            public float MinDampenedSpawnMultiplier => RSingle(BaseAddress + 0x498);
            public float KillDecayInSeconds => RSingle(BaseAddress + 0x49c);
            public float PlayerDecayInSeconds => RSingle(BaseAddress + 0x4a8);
            public float LastSampledCommunityStanding=> RSingle(BaseAddress + 0x554);
            public float LastSampledTimeOfDay => RSingle(BaseAddress + 0x558);
            public byte bIsNightTime => RUInt8(BaseAddress + 0x55c);
            public int numSpawnedPawns => RInt32(BaseAddress + 0x5f8);
            public int AmbientPopulationCapCount => RInt32(BaseAddress + 0x610);
        }
        public class DaytonPlayerController :UObject
        {
            public DaytonPlayerController(IntPtr addr) : base(addr) { }
            public IntPtr EnclaveTradeManagerPtr => RIntPtr(BaseAddress + 0x758);
            public IntPtr FollowerManagerPtr => RIntPtr(BaseAddress + 0x7a8);
            public float InfestationRangeToReportOnHUD => RSingle(BaseAddress + 0x8b4);
            public float BloodPlagueRangeToReportOnHUD => RSingle(BaseAddress + 0x8b8);
            public int ProximateZombieCount => RInt32(BaseAddress + 0x944);
            public DaytonHumanCharacter AnalyticsPawn => new DaytonHumanCharacter(RIntPtr(BaseAddress + 0xb58));
            public CommunityComponent CommunityComponent
            {
                get
                {
                    IntPtr ccptr = RIntPtr(BaseAddress + 0xb60);
                    return new CommunityComponent(ccptr);
                }
            }
        }
        public class Enclave : UObject
        {
            public Enclave(IntPtr addr) : base(addr) { }
            

            public string DisplayName => NameProperty.Value;
            public string Source => RUnicodeStr(RIntPtr(BaseAddress + 0x200));
            public string SchemaPath => RUnicodeStr(RIntPtr(BaseAddress + 0x210));
            public TextProperty NameProperty
            {
                get
                {
                    IntPtr textPropPtr = RIntPtr(BaseAddress + 0x228);
                    return new TextProperty(textPropPtr);
                }
            }
            public int Influence => RInt32(BaseAddress + 0x2A0);
            public int NumMemberDeaths => RInt32(BaseAddress + 0x2A8);
            public byte bDisplayOnMap => RUInt8(BaseAddress + 0x2c8);
            public EnclaveType EnclaveType => (EnclaveType)RUInt8(BaseAddress + 0x2c9);
            public byte bTradesUsingPrestige => RUInt8(BaseAddress + 0x2ca);
            public byte bDisbandsOnAnyRecruit=> RUInt8(BaseAddress + 0x2cb);
            public List<DaytonCharacter> Characters
            {
                get
                {
                    var characters = new List<DaytonCharacter>();
                    int count = NumCharacters;
                    if (count <= 0) return characters;
                    if (!this.Name.Equals("Enclave")) return characters;

                    IntPtr arrayPtr = RIntPtr(BaseAddress + 0x398);
                    for (int i = 0; i < count; i++)
                    {
                        IntPtr charPtr = RIntPtr(arrayPtr + i * IntPtr.Size);
                        if (charPtr != IntPtr.Zero)
                        {
                            characters.Add(new DaytonCharacter(charPtr));
                        }
                    }
                    return characters;
                }
            }
            public int NumCharacters => RInt32(BaseAddress + 0x3A0);
        }
        public class EnclaveManager : UObject
        {
            public EnclaveManager(IntPtr addr) : base(addr) { }

            public int NumEnclaves => RInt32(RIntPtr(BaseAddress + 0x170) + 0x10);

            public List<Enclave> Enclaves
            {
                get
                {
                    var enclaves = new List<Enclave>();
                    int count = NumEnclaves;
                    if (count <= 0) return enclaves;

                    IntPtr arrayPtr = RIntPtr(RIntPtr(BaseAddress + 0x170) + 0x8);

                    for (int i = 0; i < count; i++)
                    {
                        IntPtr enclavePtr = RIntPtr(arrayPtr + (i * IntPtr.Size));
                        if (enclavePtr != IntPtr.Zero)
                        {
                            enclaves.Add(new Enclave(enclavePtr));
                        }
                    }

                    return enclaves;
                }
            }
        }


        public class GameInstance : UObject
        {
            public GameInstance(IntPtr addr) : base(addr) { }
        }

        public class Level : UObject
        {
            public Level(IntPtr addr) : base(addr) { }

            public WorldSettings WorldSettings
            {
                get
                {
                    IntPtr wsPtr = RIntPtr(BaseAddress + 0x2D0);
                    return new WorldSettings(wsPtr);
                }
            }
        }

        
        public class TextProperty
        {
            public IntPtr BaseAddress { get; }

            public TextProperty(IntPtr baseAddress)
            {
                BaseAddress = baseAddress;
            }

            public string Value
            {
                get
                {
                    if (BaseAddress == IntPtr.Zero)
                        return string.Empty;

                    IntPtr strPtr = RIntPtr(BaseAddress + 0x28);
                    if (strPtr == IntPtr.Zero)
                        return string.Empty;

                    return RUnicodeStr(strPtr);
                }
            }
        }
        public class World : UObject
        {
            public World(IntPtr addr) : base(addr) { }

            public Level PersistentLevel
            {
                get
                {
                    IntPtr lvlPtr = RIntPtr(BaseAddress + 0x30);
                    return new Level(lvlPtr);
                }
            }

            public DaytonGameGameMode GameMode
            {
                get
                {
                    IntPtr gmPtr = RIntPtr(BaseAddress + 0x110);
                    return new DaytonGameGameMode(gmPtr);
                }
            }

            public DaytonGameInstance GameInstance
            {
                get
                {
                    IntPtr giPtr = RIntPtr(BaseAddress + 0x140);
                    return new DaytonGameInstance(giPtr);
                }
            }
        }
        
        public class WorldSettings : UObject
        {
            public WorldSettings(IntPtr addr) : base(addr) { }

            public float TimeDilation
            {
                get => RSingle(BaseAddress + 0x4F8);
                set => WSingle(BaseAddress + 0x4F8, value);
            }
        }
    }

}
