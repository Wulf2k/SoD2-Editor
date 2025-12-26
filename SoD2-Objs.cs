using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static SoD2_Editor.Form1;


namespace SoD2_Editor
{
    public partial class Form1
    {
        public static ItemInstance ItemInstanceLookup(IntPtr baseAddress)
        {
            var tmpSlot = new ItemInstance(baseAddress);

            switch (tmpSlot.Name)
            {
                case "AmmoItemInstance":
                    return new AmmoItemInstance(baseAddress);
                case "BackpackItemInstance":
                    return new BackpackItemInstance(baseAddress);
                case "CloseCombatItemInstance":
                    return new CloseCombatItemInstance(baseAddress);
                case "ConsumableItemInstance":
                    return new ConsumableItemInstance(baseAddress);
                case "FacilityModItemInstance":
                    return new FacilityModItemInstance(baseAddress);
                case "MeleeWeaponItemInstance":
                    return new MeleeWeaponItemInstance(baseAddress);
                case "MiscellaneousItemInstance":
                    return new MiscellaneousItemInstance(baseAddress);
                case "RangedWeaponItemInstance":
                    return new RangedWeaponItemInstance(baseAddress);
                case "RangedWeaponModItemInstance":
                    return new RangedWeaponModItemInstance(baseAddress);
                case "ResourceItemInstance":
                    return new ResourceItemInstance(baseAddress);
                default:
                    return tmpSlot.BaseAddress != IntPtr.Zero
                        ? new ItemInstance(baseAddress)
                        : null;
            }
        }




















        public static class GameLog
        {
            public static IntPtr BaseAddress => RIntPtr(addresses.Get("GameLogPtr"));
            public static IntPtr GameLogTextPtr => RIntPtr(BaseAddress + 0x98);
            public static int LogEndOffset
            {
                get => RInt32(BaseAddress + 0xa8);
                set => WInt32(BaseAddress + 0xa8, value);
            }
            public static int LogEndOffset2
            {
                get => RInt32(BaseAddress + 0xac);
                set => WInt32(BaseAddress + 0xac, value);
            }
        }



        public class AmmoItemInstance : ItemInstance
        {
            public AmmoItemInstance(IntPtr addr) : base(addr) { }
            public override UClass ItemClass => new UClass(RIntPtr(BaseAddress + 0x48));
            public override int stackCount
            {
                get => RInt32(BaseAddress + 0x50);
                set => WInt32(BaseAddress + 0x50, value);
            }
        }
        public class BackpackItem : Item
        {
            public BackpackItem(IntPtr addr) : base(addr) { }
            public int Capactity => RInt32(BaseAddress + 0x124);
        }
        public class BackpackItemInstance : ItemInstance
        {
            public BackpackItemInstance(IntPtr addr) : base(addr) { }
            public override UClass ItemClass => new UClass(RIntPtr(BaseAddress + 0x48));
        }
        public class CapsuleComponent : UObject
        {
            public CapsuleComponent(IntPtr addr) : base(addr) { }
            public Vector3 WarpPos
            {
                get
                {
                    float x = RSingle(BaseAddress + 0x170);
                    float y = RSingle(BaseAddress + 0x174);
                    float z = RSingle(BaseAddress + 0x178);
                    return new Vector3(x, y, z);
                }
                set
                {
                    WSingle(BaseAddress + 0x170, value.X);
                    WSingle(BaseAddress + 0x174, value.Y);
                    WSingle(BaseAddress + 0x178, value.Z);
                }
            }
        }
        public class CharacterSkillRecord
        {
            public IntPtr BaseAddress { get; set; }
            public CharacterSkillRecord(IntPtr addr)
            {
                BaseAddress = addr;
            }
            public int NameId
            {
                get => RInt32(BaseAddress + 0x0);
                set => WInt32(BaseAddress + 0x0, value);
            }
            public string Name
            {
                get => GetNameFromNameOffset(NameId);
            }
            public byte CurrentLevel
            {
                get => RUInt8(BaseAddress + 0x8);
                set => WUInt8(BaseAddress + 0x8, value);
            }
            public float CurrentXp
            {
                get => RSingle(BaseAddress + 0xC);
                set => WSingle(BaseAddress + 0xC, value);
            }
            public string GrantingTrait => GetNameFromNameOffset(RInt32(BaseAddress + 0x10));
        }
        public class CloseCombatItemInstance : ItemInstance
        {
            public CloseCombatItemInstance(IntPtr addr) : base(addr) { }
            public override UClass ItemClass => new UClass(RIntPtr(BaseAddress + 0x50));
            public override int stackCount
            {
                get => RInt32(BaseAddress + 0x48);
                set => WInt32(BaseAddress + 0x48, value);
            }
            public MeleeWeaponResourceStats Stats => new MeleeWeaponResourceStats(RIntPtr(BaseAddress + 0x58));
        }
        public class CommunityComponent : UObject
        {
            public CommunityComponent(IntPtr addr) : base(addr) { }
            public IntPtr MoraleSettingsPtr => RIntPtr(BaseAddress + 0x168);
            public IntPtr BuffResourceManagerPtr => RIntPtr(BaseAddress + 0x190);
            public float MinimumActionSpeedAdjustment => RSingle(BaseAddress + 0x198);
            public float MinimumInfluenceProductionMultiplier => RSingle(BaseAddress + 0x19c);
            public TimeOfDayComponent TimeOfDayComponent => new TimeOfDayComponent(RIntPtr(BaseAddress + 0x1a0));
            public List<CommunityResourceBase> Resources
            {
                get
                {
                    var resources = new List<CommunityResourceBase>();
                    int count = NumResources;
                    if (count <= 0) return resources;

                    IntPtr arrayPtr = RIntPtr(BaseAddress + 0x1160);
                    for (int i = 0; i < count; i++)
                    {
                        IntPtr resPtr = RIntPtr(arrayPtr + i * IntPtr.Size);
                        if (resPtr != IntPtr.Zero)
                        {
                            resources.Add(new CommunityResourceBase(resPtr));
                        }
                    }
                    return resources;
                }
            }
            public int NumResources => RInt32(BaseAddress + 0x1168);
            public float CommunityStandingHighWaterMark => RSingle(BaseAddress + 0x12f0);
            public float PlayTime => RSingle(BaseAddress + 0x1308);
        }
        public class CommunityResourceBase : UObject
        {
            public CommunityResourceBase(IntPtr addr) : base(addr) { }
            public ECommunityResourceType ResourceType => (ECommunityResourceType)RUInt8(BaseAddress + 0x48);
            public string DisplayName => (new FText(BaseAddress + 0x50)).Value;
            public string Description => (new FText(BaseAddress + 0x68)).Value;
            public float Supply
            {
                get => RSingle(BaseAddress + 0xf8);
                set => WSingle(BaseAddress + 0xf8, value);
            }
            public float Accumulator
            {
                get => RSingle(BaseAddress + 0x100);
                set => WSingle(BaseAddress + 0x100, value);
            }

        }
        public class ConsumableItemInstance : ItemInstance
        {
            public ConsumableItemInstance(IntPtr addr) : base(addr) { }
            public override UClass ItemClass => new UClass(RIntPtr(BaseAddress + 0x48));
            public override int stackCount
            {
                get => RInt32(BaseAddress + 0x50);
                set => WInt32(BaseAddress + 0x50, value);
            }
        }
        public class DaytonCharacter : UObject
        {
            public DaytonCharacter(IntPtr addr) : base(addr) { }

            public DaytonCharacterRecord CharacterRecord => new DaytonCharacterRecord(BaseAddress + 0x368);


            public float MaxStaminaBase => RSingle(BaseAddress + 0xAF4);
            public float MaxStaminaModifier => RSingle(BaseAddress + 0xAF8);

            public float MaxHealthBase => RSingle(BaseAddress + 0xB00);
            public float MaxHealthModifier => RSingle(BaseAddress + 0xB04);
            public float Trauma
            {
                get => RSingle(BaseAddress + 0xb08);
                set => WSingle(BaseAddress + 0xb08, value);
            }

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
            public DaytonCharacterComponent CharacterComponent => new DaytonCharacterComponent(RIntPtr(BaseAddress + 0xE30));

            public Vector3 Position
            {
                get
                {
                    float x = RSingle(BaseAddress + 0xff0);
                    float y = RSingle(BaseAddress + 0xff4);
                    float z = RSingle(BaseAddress + 0xff8);
                    return new Vector3(x, y, z);
                }
            }
            public float XPos => RSingle(BaseAddress + 0xFF0);

            public float YPos => RSingle(BaseAddress + 0xFF4);

            public float ZPos => RSingle(BaseAddress + 0xFF8);
            public byte IsInWorld => RUInt8(BaseAddress + 0x1028);
        }
        public class DaytonCharacterComponent : UObject
        {
            public DaytonCharacterComponent(IntPtr addr) : base(addr) { }
            public DaytonHumanCharacter Character => new DaytonHumanCharacter(RIntPtr(BaseAddress + 0xB8));
            public DaytonCharacter DaytonCharacter => new DaytonCharacter(RIntPtr(BaseAddress + 0x190));
        }
        public class DaytonCharacterRecord
        {
            public IntPtr BaseAddress { get; set; }
            public DaytonCharacterRecord(IntPtr addr)
            {
                BaseAddress = addr;
            }

            public int ID
            {
                get => RInt32(BaseAddress + 0x0);
                set => WInt32(BaseAddress + 0x0, value);
            }

            public string FirstName
            {
                get => new FText(BaseAddress + 0x18).Value;
                set => new FText(BaseAddress + 0x18).Value = value;
            }
            public string LastName
            {
                get => new FText(BaseAddress + 0x30).Value;
                set => new FText(BaseAddress + 0x30).Value = value;
            }
            public string NickName
            {
                get => new FText(BaseAddress + 0x48).Value;
                set => new FText(BaseAddress + 0x48).Value = value;
            }

            public string VoiceID => GetNameFromNameOffset(RInt32(BaseAddress + 0x60));
            public string CulturalBackground => GetNameFromNameOffset(RInt32(BaseAddress + 0x68));
            public string HumanDefinition => GetNameFromNameOffset(RInt32(BaseAddress + 0x80));

            public ECharacterPhilosophy Philosophy1
            {
                get => (ECharacterPhilosophy)RUInt8(BaseAddress + 0xA8);
                set => WUInt8(BaseAddress + 0xA8, (byte)value);
            }

            public ECharacterPhilosophy Philosophy2
            {
                get => (ECharacterPhilosophy)RUInt8(BaseAddress + 0xA8);
                set => WUInt8(BaseAddress + 0xA8, (byte)value);
            }

            public ECharacterStanding StandingLevel
            {
                get => (ECharacterStanding)RUInt8(BaseAddress + 0xAA);
                set => WUInt8(BaseAddress + 0xAA, (byte)value);
            }

            public string HeroBonus => GetNameFromNameOffset(RInt32(BaseAddress + 0xB0));
            public string LeaderType => GetNameFromNameOffset(RInt32(BaseAddress + 0xC0));

            public float CurrStanding
            {
                get => RSingle(BaseAddress + 0xC8);
                set => WSingle(BaseAddress + 0xC8, value);
            }

            public Equipment Equipment => new Equipment(RIntPtr(BaseAddress + 0xD0));
            public Inventory Inventory => new Inventory(RIntPtr(BaseAddress + 0xE0));

            public List<CharacterSkillRecord> Skills
            {
                get
                {
                    var skills = new List<CharacterSkillRecord>();
                    int count = numSkills;

                    if (count <= 0)
                        return skills;

                    IntPtr skillsArrayPtr = RIntPtr(BaseAddress + 0xE8);

                    for (int i = 0; i < count; i++)
                    {
                        IntPtr skillPtr = skillsArrayPtr + i * 0x18;
                        if (skillPtr != IntPtr.Zero)
                            skills.Add(new CharacterSkillRecord(skillPtr));
                    }
                    return skills;
                }
            }

            public int numSkills => RInt32(BaseAddress + 0xF0);

            public List<string> TraitNames
            {
                get
                {
                    var traits = new List<string>();
                    IntPtr traitsPtr = RIntPtr(BaseAddress + 0xF8);
                    if (traitsPtr == IntPtr.Zero)
                        return traits;

                    int numTraits = RInt32(BaseAddress + 0x100);
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

            public float CurrHealth
            {
                get => RSingle(BaseAddress + 0x108);
                set => WSingle(BaseAddress + 0x108, value);
            }

            public float CurrStam
            {
                get => RSingle(BaseAddress + 0x10C);
                set => WSingle(BaseAddress + 0x10C, value);
            }

            public float CurrFatigue
            {
                get => RSingle(BaseAddress + 0x110);
                set => WSingle(BaseAddress + 0x110, value);
            }

            public float CurrSick
            {
                get => RSingle(BaseAddress + 0x118);
                set => WSingle(BaseAddress + 0x118, value);
            }

            public float CurrPlague
            {
                get => RSingle(BaseAddress + 0x11C);
                set => WSingle(BaseAddress + 0x11C, value);
            }

            public float TraumaCounter
            {
                get => RSingle(BaseAddress + 0x124);
                set => WSingle(BaseAddress + 0x124, value);
            }

            public float InjuryRecoveryCounter
            {
                get => RSingle(BaseAddress + 0x128);
                set => WSingle(BaseAddress + 0x128, value);
            }
            public int ZombiesKilled
            {
                get => RInt32(BaseAddress + 0x12C);
                set => WInt32(BaseAddress + 0x12C, value);
            }
        }
        public class DaytonCheatManager : UObject
        {
            public DaytonCheatManager(IntPtr addr) : base(addr) { }

            public bool bInvisibleToZombies
            {
                get => RUInt8(BaseAddress + 0x87) > 0;
                set => WUInt8(BaseAddress + 0x87, Convert.ToByte(value));
            }
        }
        public class DaytonGameGameMode : UObject
        {
            public DaytonGameGameMode(IntPtr addr) : base(addr) { }

            public EnclaveManager EnclaveManager => new EnclaveManager(RIntPtr(BaseAddress + 0x458));
            public DynamicPawnSpawner DynamicPawnSpawner => new DynamicPawnSpawner(RIntPtr(BaseAddress + 0x4c8));
        }
        public class DaytonGameGameState : UObject
        {
            public DaytonGameGameState(IntPtr addr) : base(addr) { }
            public FireSettings FireSettings => new FireSettings(RIntPtr(BaseAddress + 0x410));
            public MapStateComponent MapStateComponent => new MapStateComponent(RIntPtr(BaseAddress + 0x440));
        }
        public class DaytonGameInstance : GameInstance
        {
            public DaytonGameInstance(IntPtr addr) : base(addr) { }
            public DaytonSaveManager SaveGameManager => new DaytonSaveManager(RIntPtr(BaseAddress + 0x910));
            public ItemCatalogManager ItemCatalogManager => new ItemCatalogManager(RIntPtr(BaseAddress + 0xa18));
        }
        public class DaytonHumanCharacter : UObject
        {
            public DaytonHumanCharacter(IntPtr addr) : base(addr) { }
            public CapsuleComponent CapsuleComponent => new CapsuleComponent(RIntPtr(BaseAddress + 0x1b0));
            public DaytonCharacterComponent CharacterComponent => new DaytonCharacterComponent(RIntPtr(BaseAddress + 0xBa0));
            public float MaxSickness => RSingle(BaseAddress + 0xc3c);
            public float CurrentEffectiveMaxHealth => RSingle(BaseAddress + 0xc58);
            public float CurrentHealth => RSingle(BaseAddress + 0xc5c);
            public byte bIsInCombat => RUInt8(BaseAddress + 0xc70);
            public IntPtr ClosestEnemy => RIntPtr(BaseAddress + 0xe70);
        }
        public class DaytonLocalPlayer : UObject
        {
            public DaytonLocalPlayer(IntPtr addr) : base(addr) { }
            public DaytonPlayerController DaytonPlayerController => new DaytonPlayerController(RIntPtr(BaseAddress + 0x30));
        }
        public class DaytonPlayerController : UObject
        {
            public DaytonPlayerController(IntPtr addr) : base(addr) { }
            public DaytonCheatManager CheatManager
            {
                get => new DaytonCheatManager(RIntPtr(BaseAddress + 0x458));
                set => WIntPtr(BaseAddress + 0x458, value.BaseAddress);
            }
            public IntPtr EnclaveTradeManagerPtr => RIntPtr(BaseAddress + 0x758);
            public IntPtr FollowerManagerPtr => RIntPtr(BaseAddress + 0x7a8);
            public float InfestationRangeToReportOnHUD => RSingle(BaseAddress + 0x8b4);
            public float BloodPlagueRangeToReportOnHUD => RSingle(BaseAddress + 0x8b8);
            public int ProximateZombieCount => RInt32(BaseAddress + 0x944);
            public MapUI MapUI => new MapUI(RIntPtr(BaseAddress + 0x9b8));
            public DaytonHumanCharacter AnalyticsPawn => new DaytonHumanCharacter(RIntPtr(BaseAddress + 0xb58));
            public CommunityComponent CommunityComponent => new CommunityComponent(RIntPtr(BaseAddress + 0xb60));
        }
        public class DaytonSaveManager : UObject
        {
            public DaytonSaveManager(IntPtr addr) : base(addr) { }
        }
        public class DynamicPawnSpawner : UObject
        {
            public DynamicPawnSpawner(IntPtr addr) : base(addr) { }
            public byte Active
            {
                get => RUInt8(BaseAddress + 0x370);
                set => WUInt8(BaseAddress + 0x370, value);
            }
            public byte bIsUnlimitedSpawningEnabled => RUInt8(BaseAddress + 0x37c);
            public byte bIsOHKOZombiesEnabled => RUInt8(BaseAddress + 0x37d);
            public float MaxPopulationMultiplier => RSingle(BaseAddress + 0x460);
            public float MaxKillsPerCell => RSingle(BaseAddress + 0x484);
            public float MinKillsForAdjacencyBonus => RSingle(BaseAddress + 0x488);
            public float MinDampenedSpawnMultiplier => RSingle(BaseAddress + 0x498);
            public float KillDecayInSeconds => RSingle(BaseAddress + 0x49c);
            public float PlayerDecayInSeconds => RSingle(BaseAddress + 0x4a8);
            public float LastSampledCommunityStanding => RSingle(BaseAddress + 0x554);
            public float LastSampledTimeOfDay => RSingle(BaseAddress + 0x558);
            public byte bIsNightTime => RUInt8(BaseAddress + 0x55c);
            public int numSpawnedPawns => RInt32(BaseAddress + 0x5f8);
            public int AmbientPopulationCapCount => RInt32(BaseAddress + 0x610);
        }
        public class Enclave : UObject
        {
            public Enclave(IntPtr addr) : base(addr) { }


            public string DisplayName => NameProperty.Value;
            public string Source => RUnicodeStr(RIntPtr(BaseAddress + 0x200));
            public string SchemaPath => RUnicodeStr(RIntPtr(BaseAddress + 0x210));
            public FText NameProperty => new FText(BaseAddress + 0x228);
            public Inventory Inventory => new Inventory(RIntPtr(BaseAddress + 0x288));
            public int Influence => RInt32(BaseAddress + 0x2A0);
            public int NumMemberDeaths => RInt32(BaseAddress + 0x2A8);
            public byte bDisplayOnMap => RUInt8(BaseAddress + 0x2c8);
            public EEnclaveType EnclaveType => (EEnclaveType)RUInt8(BaseAddress + 0x2c9);
            public byte bTradesUsingPrestige => RUInt8(BaseAddress + 0x2ca);
            public byte bDisbandsOnAnyRecruit => RUInt8(BaseAddress + 0x2cb);
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
            public Vector3 BaseCenter
            {
                get
                {
                    float x = RSingle(BaseAddress + 0xae8);
                    float y = RSingle(BaseAddress + 0xaec);
                    float z = RSingle(BaseAddress + 0xaf0);
                    return new Vector3(x, y, z);
                }
            }
        }
        public class EnclaveManager : UObject
        {
            public EnclaveManager(IntPtr addr) : base(addr) { }

            public int NumEnclaves
            {
                get => RInt32(RIntPtr(BaseAddress + 0x170) + 0x10);
                set => WInt32(RIntPtr(BaseAddress + 0x170) + 0x10, value);
            }

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
        public class Equipment : UObject
        {
            public Equipment(IntPtr addr) : base(addr) { }
            public MeleeWeaponItemInstance MeleeWeaponItemInstance => new MeleeWeaponItemInstance(RIntPtr(RIntPtr(BaseAddress + 0x38) + 0x00));
            public RangedWeaponItemInstance RangedWeaponItemInstance => new RangedWeaponItemInstance(RIntPtr(RIntPtr(BaseAddress + 0x38) + 0x08));
            public RangedWeaponItemInstance SideArmRangedWeaponItemInstance => new RangedWeaponItemInstance(RIntPtr(RIntPtr(BaseAddress + 0x38) + 0x28));
        }
        public class FacilityModItemInstance : ItemInstance
        {
            public FacilityModItemInstance(IntPtr addr) : base(addr) { }
            public override int stackCount
            {
                get => RInt32(BaseAddress + 0x48);
                set => WInt32(BaseAddress + 0x48, value);
            }
            public override UClass ItemClass => new UClass(RIntPtr(BaseAddress + 0x50));

        }
        public class FireSettings : UObject
        {
            public FireSettings(IntPtr addr) : base(addr) { }
        }
        public class GameEngine : UObject
        {
            public GameEngine(IntPtr addr) : base(addr) { }
            public GameViewport GameViewport => new GameViewport(RIntPtr(BaseAddress + 0x6c8));
            public DaytonGameInstance GameInstance => new DaytonGameInstance(RIntPtr(BaseAddress + 0xd40));

        }
        public class GameInstance : UObject
        {
            public GameInstance(IntPtr addr) : base(addr) { }
        }
        public class GameViewport : UObject
        {
            public GameViewport(IntPtr addr) : base(addr) { }
            public UConsole ViewportConsole
            {
                get
                {
                    return new UConsole(RIntPtr(BaseAddress + 0x38));
                }
                set
                {
                    WIntPtr(BaseAddress + 0x38, value.BaseAddress);
                }
            }

        }
        public class Inventory : ItemInstance
        {
            public Inventory(IntPtr addr) : base(addr) { }
            public List<ItemInstance> Slots
            {
                get
                {
                    var slots = new List<ItemInstance>();
                    IntPtr arrPtr = RIntPtr(BaseAddress + 0x200);
                    for (int i = 0; i < numSlots; i++)
                    {
                        IntPtr slotPtr = RIntPtr(arrPtr + i * IntPtr.Size);
                        if (slotPtr != IntPtr.Zero)
                        {
                            IntPtr instancePtr = slotPtr;
                            if (instancePtr != IntPtr.Zero)
                            {
                                ItemInstance slot = ItemInstanceLookup(instancePtr);
                                if (slot != null)
                                    slots.Add(slot);
                            }
                        }
                    }//end for
                    return slots;
                } //end get
            }
            public int numSlots => RInt32(BaseAddress + 0x208);
        }
        public class Item : UObject
        {
            public Item(IntPtr addr) : base(addr) { }
            public string DisplayName => (new FText(BaseAddress + 0x28)).Value;
            public string DisplayDescription => (new FText(BaseAddress + 0x58)).Value;
            public float Weight => RSingle(BaseAddress + 0xF0);
            public int InfluenceValue => RInt32(BaseAddress + 0xF4);
        }
        public class ItemCatalog : UObject
        {
            public ItemCatalog(IntPtr addr) : base(addr) { }
            public string ID => GetNameFromNameOffset(RInt32(BaseAddress + 0x28));
            public List<string> BountyItems
            {
                get
                {
                    IntPtr startPtr = RIntPtr(BaseAddress + 0x170);
                    var bountyItems = new List<string>();
                    for (int i = 0; i < numBountyItems; i++)
                    {
                        //worldItems.Add($"{GetObjFromObjId(RInt32(startPtr + i * 0x20)).Name}  {GetObjFromObjId(RInt32(startPtr + i * 0x20)).Type}");
                        bountyItems.Add($"{RUnicodeStr(RIntPtr(startPtr + i * 0x50 + 0x10))}");
                    }
                    return bountyItems;
                }
            }
            public int numBountyItems => RInt32(BaseAddress + 0x178);
            public List<string> WorldItems
            {
                get
                {
                    IntPtr startPtr = RIntPtr(BaseAddress + 0x180);
                    var worldItems = new List<string>();
                    for (int i = 0; i < numWorldItems; i++)
                    {
                        //worldItems.Add($"{GetObjFromObjId(RInt32(startPtr + i * 0x20)).Name}  {GetObjFromObjId(RInt32(startPtr + i * 0x20)).Type}");
                        worldItems.Add($"{RUnicodeStr(RIntPtr(startPtr + i * 0x20 + 0x10))}");
                    }
                    return worldItems;
                }
            }
            public int numWorldItems => RInt32(BaseAddress + 0x188);
        }
        public class ItemCatalogManager : UObject
        {
            public ItemCatalogManager(IntPtr addr) : base(addr) { }
            public List<ItemCatalog> Catalogs
            {
                get
                {
                    var catalogs = new List<ItemCatalog>();
                    IntPtr arrPtr = RIntPtr(BaseAddress + 0x28);
                    for (int i = 0; i < numCatalogs; i++)
                    {
                        IntPtr catalogPtr = RIntPtr(arrPtr + i * IntPtr.Size);
                        if (catalogPtr != IntPtr.Zero)
                            catalogs.Add(new ItemCatalog(catalogPtr));
                    }//end for
                    return catalogs;
                } //end get
            }
            public int numCatalogs => RInt32(BaseAddress + 0x30);
        }
        public class ItemInstance : UObject
        {
            public ItemInstance(IntPtr addr) : base(addr) { }
            public virtual UClass ItemClass => new UClass(RIntPtr(BaseAddress + 0x10));
            public virtual int stackCount { get; set; }
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
        public class MapStateComponent : UObject
        {
            public MapStateComponent(IntPtr addr) : base(addr) { }
            public List<Vector2> WaypointPositions
            {
                get
                {
                    var wpPosList = new List<Vector2>();
                    IntPtr posPtr = RIntPtr(BaseAddress + 0x278);
                    for (int i = 0; i < numWaypointPositions; i++)
                    {
                        float x = RSingle(posPtr + i * 0x8);
                        float y = RSingle(posPtr + i * 0x8 + 0x4);
                        wpPosList.Add(new Vector2(x, y));
                    }
                    return wpPosList;
                }
            }
            public int numWaypointPositions => RInt32(BaseAddress + 0x280);
            public List<WaypointActor> WaypointActors
            {
                get
                {
                    var waypoints = new List<WaypointActor>();
                    IntPtr arrPtr = RIntPtr(BaseAddress + 0x290);
                    for (int i = 0; i < numWayPointActors; i++)
                    {
                        IntPtr wp = RIntPtr(arrPtr + i * IntPtr.Size);
                        if (wp != IntPtr.Zero)
                        {
                            WaypointActor waypoint = new WaypointActor(wp);
                            waypoints.Add(waypoint);
                        }
                    }
                    return waypoints;
                }
            }
            public int numWayPointActors => RInt32(BaseAddress + 0x298);
        }
        public class MapUI : UObject
        {
            public MapUI(IntPtr addr) : base(addr) { }
            public Vector2 MapBoundsMin
            {
                get
                {
                    float x = RSingle(BaseAddress + 0x600);
                    float y = RSingle(BaseAddress + 0x604);
                    return new Vector2(x, y);
                }
            }
            public Vector2 MapBoundsMax
            {
                get
                {
                    float x = RSingle(BaseAddress + 0x608);
                    float y = RSingle(BaseAddress + 0x60c);
                    return new Vector2(x, y);
                }
            }
            public MapStateComponent MapState => new MapStateComponent(RIntPtr(BaseAddress + 0x968));
        }
        public class MeleeWeaponItem : WeaponItem
        {
            public MeleeWeaponItem(IntPtr addr) : base(addr) { }
            public MeleeWeaponResource Resource => new MeleeWeaponResource(BaseAddress + 0x128);
            public EMeleeTypeEnum MeleeType => (EMeleeTypeEnum)RUInt8(BaseAddress + 0x210);
        }
        public class MeleeWeaponItemInstance : ItemInstance
        {
            public MeleeWeaponItemInstance(IntPtr addr) : base(addr) { }
            public MeleeWeaponResourceStats Stats => new MeleeWeaponResourceStats(RIntPtr(BaseAddress + 0xF8));
            public override UClass ItemClass => new UClass(RIntPtr(BaseAddress + 0x100));
            public float Durability
            {
                get => RSingle(BaseAddress + 0x169c);
                set => WSingle(BaseAddress + 0x169c, value);
            }

        }
        public class MeleeWeaponResourceStats : UObject
        {
            public MeleeWeaponResourceStats(IntPtr addr) : base(addr) { }

            public float Dismember
            {
                get => RSingle(BaseAddress + 0x34);
                set => WSingle(BaseAddress + 0x34, value);
            }

            public float DismemberDelta
            {
                get => RSingle(BaseAddress + 0x3C);
                set => WSingle(BaseAddress + 0x3C, value);
            }

            public float Impact
            {
                get => RSingle(BaseAddress + 0x44);
                set => WSingle(BaseAddress + 0x44, value);
            }

            public float ImpactDelta
            {
                get => RSingle(BaseAddress + 0x4C);
                set => WSingle(BaseAddress + 0x4C, value);
            }

            public float Knockdown
            {
                get => RSingle(BaseAddress + 0x54);
                set => WSingle(BaseAddress + 0x54, value);
            }

            public float KnockdownDelta
            {
                get => RSingle(BaseAddress + 0x5C);
                set => WSingle(BaseAddress + 0x5C, value);
            }

            public float Lethality
            {
                get => RSingle(BaseAddress + 0x64);
                set => WSingle(BaseAddress + 0x64, value);
            }

            public float LethalityDelta
            {
                get => RSingle(BaseAddress + 0x6C);
                set => WSingle(BaseAddress + 0x6C, value);
            }

            public float Weight
            {
                get => RSingle(BaseAddress + 0x74);
                set => WSingle(BaseAddress + 0x74, value);
            }

            public float PerceptionLoudness
            {
                get => RSingle(BaseAddress + 0x7C);
                set => WSingle(BaseAddress + 0x7C, value);
            }

            public float Speed
            {
                get => RSingle(BaseAddress + 0x84);
                set => WSingle(BaseAddress + 0x84, value);
            }

            public float SwingCost
            {
                get => RSingle(BaseAddress + 0x8C);
                set => WSingle(BaseAddress + 0x8C, value);
            }

            public float InfluenceValue
            {
                get => RSingle(BaseAddress + 0x94);
                set => WSingle(BaseAddress + 0x94, value);
            }

            public float PrestigeValue
            {
                get => RSingle(BaseAddress + 0x9C);
                set => WSingle(BaseAddress + 0x9C, value);
            }

            public float Durability
            {
                get => RSingle(BaseAddress + 0xA4);
                set => WSingle(BaseAddress + 0xA4, value);
            }

            public float DurabilityLossPerHitMin
            {
                get => RSingle(BaseAddress + 0xAC);
                set => WSingle(BaseAddress + 0xAC, value);
            }

            public float DurabilityLossPerHitMax
            {
                get => RSingle(BaseAddress + 0xB4);
                set => WSingle(BaseAddress + 0xB4, value);
            }

            public float DurabilityLossPerFinisherMin
            {
                get => RSingle(BaseAddress + 0xBC);
                set => WSingle(BaseAddress + 0xBC, value);
            }

            public float DurabilityLossPerFinisherMax
            {
                get => RSingle(BaseAddress + 0xC4);
                set => WSingle(BaseAddress + 0xC4, value);
            }
            public string WeaponName => RUnicodeStr(RIntPtr(BaseAddress + 0xC8));
            public string WeaponType => RUnicodeStr(RIntPtr(BaseAddress + 0xD8));
            public string WeaponDesc => RUnicodeStr(RIntPtr(BaseAddress + 0xE8));
            public EMeleeTypeEnum MeleeType => (EMeleeTypeEnum)RUInt8(BaseAddress + 0x110);
        }
        public class MeleeWeaponResource
        {
            public MeleeWeaponResource(IntPtr addr) => BaseAddress = addr;
            public IntPtr BaseAddress { get; set; }
            public MeleeWeaponStats Stats => new MeleeWeaponStats(BaseAddress);
        }
        public class MeleeWeaponStats
        {
            public MeleeWeaponStats(IntPtr addr) => BaseAddress = addr;
            public IntPtr BaseAddress { get; set; }
            public float Dismember
            {
                get => RSingle(BaseAddress + 0xC);
                set => WSingle(BaseAddress + 0xC, value);
            }

            public float DismemberDelta
            {
                get => RSingle(BaseAddress + 0x14);
                set => WSingle(BaseAddress + 0x14, value);
            }

            public float Impact
            {
                get => RSingle(BaseAddress + 0x1C);
                set => WSingle(BaseAddress + 0x1C, value);
            }

            public float ImpactDelta
            {
                get => RSingle(BaseAddress + 0x24);
                set => WSingle(BaseAddress + 0x24, value);
            }

            public float Knockdown
            {
                get => RSingle(BaseAddress + 0x2C);
                set => WSingle(BaseAddress + 0x2C, value);
            }

            public float KnockdownDelta
            {
                get => RSingle(BaseAddress + 0x34);
                set => WSingle(BaseAddress + 0x34, value);
            }

            public float Lethality
            {
                get => RSingle(BaseAddress + 0x3C);
                set => WSingle(BaseAddress + 0x3C, value);
            }

            public float LethalityDelta
            {
                get => RSingle(BaseAddress + 0x44);
                set => WSingle(BaseAddress + 0x44, value);
            }

            public float Weight
            {
                get => RSingle(BaseAddress + 0x4C);
                set => WSingle(BaseAddress + 0x4C, value);
            }

            public float PerceptionLoudness
            {
                get => RSingle(BaseAddress + 0x54);
                set => WSingle(BaseAddress + 0x54, value);
            }

            public float Speed
            {
                get => RSingle(BaseAddress + 0x5C);
                set => WSingle(BaseAddress + 0x5C, value);
            }

            public float SwingCost
            {
                get => RSingle(BaseAddress + 0x64);
                set => WSingle(BaseAddress + 0x64, value);
            }

            public float InfluenceValue
            {
                get => RSingle(BaseAddress + 0x6C);
                set => WSingle(BaseAddress + 0x6C, value);
            }

            public float PrestigeValue
            {
                get => RSingle(BaseAddress + 0x74);
                set => WSingle(BaseAddress + 0x74, value);
            }

            public float Durability
            {
                get => RSingle(BaseAddress + 0x7C);
                set => WSingle(BaseAddress + 0x7C, value);
            }

            public float DurabilityLossPerHitMin
            {
                get => RSingle(BaseAddress + 0x84);
                set => WSingle(BaseAddress + 0x84, value);
            }

            public float DurabilityLossPerHitMax
            {
                get => RSingle(BaseAddress + 0x8C);
                set => WSingle(BaseAddress + 0x8C, value);
            }

            public float DurabilityLossPerFinisherMin
            {
                get => RSingle(BaseAddress + 0x94);
                set => WSingle(BaseAddress + 0x94, value);
            }

            public float DurabilityLossPerFinisherMax
            {
                get => RSingle(BaseAddress + 0x9C);
                set => WSingle(BaseAddress + 0x9C, value);
            }
        }
        public class MiscellaneousItem : Item
        {
            public MiscellaneousItem(IntPtr addr) : base(addr) { }
            public EMiscellaneousItemType MiscellaneousItemType => (EMiscellaneousItemType)RUInt8(BaseAddress + 0x10c);
            public EMiscellaneousItemSubType ItemSubType => (EMiscellaneousItemSubType)RUInt8(BaseAddress + 0x10d);
        }
        public class MiscellaneousItemInstance : ItemInstance
        {
            public MiscellaneousItemInstance(IntPtr addr) : base(addr) { }
            public override int stackCount
            {
                get => RInt32(BaseAddress + 0x48);
                set => WInt32(BaseAddress + 0x48, value);
            }
            public override UClass ItemClass => new UClass(RIntPtr(BaseAddress + 0x50));

        }
        public class RangedWeapon : UObject
        {
            public RangedWeapon(IntPtr addr) : base(addr) { }
            public string WeaponClass => RUnicodeStr(RIntPtr(BaseAddress + 0x128));
            //public RangedWeaponResourceStats Stats => new RangedWeaponResourceStats(BaseAddress + 0x100);
            public RangedWeaponResourceStats Stats => new RangedWeaponResourceStats(RIntPtr(BaseAddress + 0x28));

        }
        public class RangedWeaponItemInstance : ItemInstance
        {
            public RangedWeaponItemInstance(IntPtr addr) : base(addr) { }
            public float Durability
            {
                get => RSingle(BaseAddress + 0x60);
                set => WSingle(BaseAddress + 0x60, value);
            }
            public override UClass ItemClass => new UClass(RIntPtr(BaseAddress + 0x68));
            public RangedWeapon RangedWeapon => new RangedWeapon(RIntPtr(BaseAddress + 0x70));

        }

        public class RangedWeaponModItemInstance : ItemInstance
        {
            public RangedWeaponModItemInstance(IntPtr addr) : base(addr) { }
            public override int stackCount
            {
                get => RInt32(BaseAddress + 0x48);
                set => WInt32(BaseAddress + 0x48, value);
            }
            public override UClass ItemClass => new UClass(RIntPtr(BaseAddress + 0x50));

        }
        public class RangedWeaponResourceStats : UObject
        {
            public RangedWeaponResourceStats(IntPtr addr) : base(addr) { }
            public string WeaponName => RUnicodeStr(RIntPtr(BaseAddress + 0x48));
            public string WeaponType => RUnicodeStr(RIntPtr(BaseAddress + 0x58));
            public string WeaponDesc => RUnicodeStr(RIntPtr(BaseAddress + 0x68));
            public float KickAngle
            {
                get => RSingle(BaseAddress + 0x124);
                set => WSingle(BaseAddress + 0x124, value);
            }
            public int ProjectileCount
            {
                get => RInt32(BaseAddress + 0x15c);
                set => WInt32(BaseAddress + 0x15c, value);
            }
            public int MagazineSize
            {
                get => RInt32(BaseAddress + 0x164);
                set => WInt32(BaseAddress + 0x164, value);
            }
            public int AmmoConsumedPerShot
            {
                get => RInt32(BaseAddress + 0x174);
                set => WInt32(BaseAddress + 0x174, value);
            }
            public float Impact
            {
                get => RSingle(BaseAddress + 0x230);
                set => WSingle(BaseAddress + 0x230, value);
            }
            public float ImpactDelta
            {
                get => RSingle(BaseAddress + 0x238);
                set => WSingle(BaseAddress + 0x238, value);
            }
            public float Knockdown
            {
                get => RSingle(BaseAddress + 0x240);
                set => WSingle(BaseAddress + 0x240, value);
            }
            public float KnockdownDelta
            {
                get => RSingle(BaseAddress + 0x248);
                set => WSingle(BaseAddress + 0x248, value);
            }
            public float Dismember
            {
                get => RSingle(BaseAddress + 0x250);
                set => WSingle(BaseAddress + 0x250, value);
            }
            public float DismemberDelta
            {
                get => RSingle(BaseAddress + 0x258);
                set => WSingle(BaseAddress + 0x258, value);
            }
            public float Penetration
            {
                get => RSingle(BaseAddress + 0x260);
                set => WSingle(BaseAddress + 0x260, value);
            }
            public float Range
            {
                get => RSingle(BaseAddress + 0x278);
                set => WSingle(BaseAddress + 0x278, value);
            }
            public float Weight
            {
                get => RSingle(BaseAddress + 0x288);
                set => WSingle(BaseAddress + 0x288, value);
            }
            public int InfluenceValue
            {
                get => RInt32(BaseAddress + 0x290);
                set => WInt32(BaseAddress + 0x290, value);
            }
            public int PrestigeValue
            {
                get => RInt32(BaseAddress + 0x298);
                set => WInt32(BaseAddress + 0x298, value);
            }
            public float Durability
            {
                get => RSingle(BaseAddress + 0x2A0);
                set => WSingle(BaseAddress + 0x2A0, value);
            }
            public float DurabilityLossPerShotMin
            {
                get => RSingle(BaseAddress + 0x2A8);
                set => WSingle(BaseAddress + 0x2A8, value);
            }
            public float DurabilityLossPerShotMax
            {
                get => RSingle(BaseAddress + 0x2B0);
                set => WSingle(BaseAddress + 0x2B0, value);
            }
            public float PerceptionLoudness
            {
                get => RSingle(BaseAddress + 0x2D8);
                set => WSingle(BaseAddress + 0x2D8, value);
            }
            public string TracerSettings => RUnicodeStr(RIntPtr(BaseAddress + 0x358));
        }
        public class ResourceItem : UObject
        {
            public ResourceItem(IntPtr addr) : base(addr) { }
            public ECommunityResourceType ResourceType => (ECommunityResourceType)RUInt8(BaseAddress + 0x128);
        }
        public class ResourceItemInstance : ItemInstance
        {
            public ResourceItemInstance(IntPtr addr) : base(addr) { }
            public override UClass ItemClass => new UClass(RIntPtr(BaseAddress + 0x48));
        }



        public class TimeOfDayComponent : UObject
        {
            public TimeOfDayComponent(IntPtr addr) : base(addr) { }
            public byte bDebugEnabled
            {
                get => RUInt8(BaseAddress + 0x100);
                set => WUInt8(BaseAddress + 0x100, value);
            }
            public float DayLength
            {
                get => RSingle(BaseAddress + 0x118);
                set => WSingle(BaseAddress + 0x118, value);
            }
            public float TimeOfDay
            {
                get => RSingle(BaseAddress + 0x11c);
                set => WSingle(BaseAddress + 0x11c, value);
            }
            public float TimeOfDayVisual
            {
                get => RSingle(BaseAddress + 0x120);
                set => WSingle(BaseAddress + 0x120, value);
            }
            public float DaytimeStart
            {
                get => RSingle(BaseAddress + 0x124);
                set => WSingle(BaseAddress + 0x124, value);
            }
            public float DaytimeEnd
            {
                get => RSingle(BaseAddress + 0x128);
                set => WSingle(BaseAddress + 0x128, value);
            }
            public ETimeOfDayPeriod TimeOfDayPeriod
            {
                get => (ETimeOfDayPeriod)RUInt8(BaseAddress + 0x12c);
                set => WUInt8(BaseAddress + 0x12c, (byte)value);
            }

        }
        public class UConsole : UObject
        {
            public UConsole(IntPtr addr) : base(addr) { }
            public DaytonLocalPlayer ConsoleTargetPlayer
            {
                get => new DaytonLocalPlayer(RIntPtr(BaseAddress + 0x38));
                set => WIntPtr(BaseAddress + 0x38, value.BaseAddress);
            }
        }
        public class WaypointActor : UObject
        {
            public WaypointActor(IntPtr addr) : base(addr) { }
        }
        public class WaypointIndicatorComponent : UObject
        {
            public WaypointIndicatorComponent(IntPtr addr) : base(addr) { }

        }
        public class WeaponItem : Item
        {
            public WeaponItem(IntPtr addr) : base(addr) { }
        }

        public class World : UObject
        {
            public World(IntPtr addr) : base(addr) { }

            public Level PersistentLevel => new Level(RIntPtr(BaseAddress + 0x30));
            public DaytonGameGameState GameState => new DaytonGameGameState(RIntPtr(BaseAddress + 0x58));
            public DaytonGameGameMode GameMode => new DaytonGameGameMode(RIntPtr(BaseAddress + 0x110));
            public DaytonGameInstance GameInstance => new DaytonGameInstance(RIntPtr(BaseAddress + 0x140));
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
        public class ZombieDamagedAnalytics
        {
            public IntPtr BaseAddress { get; set; }

            public ZombieDamagedAnalytics(IntPtr addr)
            {
                BaseAddress = addr;
            }
            public EZombieType ZombieTypeId => (EZombieType)RUInt8(BaseAddress + 0x150);
            public int ZombieId => RInt32(BaseAddress + 0x154);
            public byte IsPlagueZombie => RUInt8(BaseAddress + 0x158);
            public int ZombieX => RInt32(BaseAddress + 0x15C);
            public int ZombieY => RInt32(BaseAddress + 0x160);
            public int ZombieZ => RInt32(BaseAddress + 0x164);
            public EAnalyticsCauseOfDamage CauseOfDamageType => (EAnalyticsCauseOfDamage)RUInt8(BaseAddress + 0x168);
            public string CauseOfDamageId => RUnicodeStr(RIntPtr(BaseAddress + 0x170));
            public byte Killed => RUInt8(BaseAddress + 0x180);
            public int DealerId => RInt32(BaseAddress + 0x184);
            public string DealerState => RUnicodeStr(RIntPtr(BaseAddress + 0x188));
            public float StunChance => RSingle(BaseAddress + 0x198);
            public float DownChance => RSingle(BaseAddress + 0x19c);
            public float KillChance => RSingle(BaseAddress + 0x1a0);
            public float DismemberChance => RSingle(BaseAddress + 0x1a4);
            public string PreDamageState => RUnicodeStr(RIntPtr(BaseAddress + 0x1a8));
            public string ResultingState => RUnicodeStr(RIntPtr(BaseAddress + 0x1b8));
            public int HeadshotCounter => RInt32(BaseAddress + 0x1d8);
        }
    }
}

