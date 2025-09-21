using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static SoD2_Editor.Form1;
using static SoD2_Editor.Form1.RangedWeapon.RangedWeaponItemInstance;

namespace SoD2_Editor
{
    public partial class Form1
    {
        public enum ECharacterStanding : byte
        {
            Stranger = 0,
            Recruit = 1,
            Citizen = 2,
            Hero = 3,
            Leader = 4,
            Count = 5,
            MAX = 6
        }
        public enum ECommunityResourceType : byte
        {
            Ammo = 0,
            Food = 1,
            Fuel = 2,
            Influence = 3,
            Materials = 4,
            Meds = 5,
            Parts = 6,
            Prestige = 7,
            Counts = 8,
            ECommunityResourceType_MAX = 9
        }
        public enum EEnclaveType : byte
        {
            Default = 0,
            AmbientMission = 1,
            Legacy = 2,
            RadioTrader = 3,
            Persistent = 4,
            Num = 5,
            MAX = 6
        }
        public enum EMeleeTypeEnum : byte
        {
            Blunt = 0,
            Bladed = 1,
            Heavy = 2,
            EMeleeTypeEnum_MAX = 3
        }
        public enum ETimeOfDayPeriod : byte
        {
            Daytime = 0,
            Nighttime = 1,
            ETimeOfDayPeriod_MAX = 2
        }




















        public class UObject
        {
            public IntPtr BaseAddress { get; set; }

            public UObject(IntPtr addr)
            {
                BaseAddress = addr;
            }

            public string Name => GetNameFromNameOffset(RInt32(BaseAddress + 0x18));
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
        }
        public class CommunityComponent : UObject
        {
            public CommunityComponent(IntPtr addr) : base(addr) { }
            public IntPtr MoraleSettingsPtr => RIntPtr(BaseAddress + 0x168);
            public IntPtr BuffResourceManagerPtr => RIntPtr(BaseAddress + 0x190);
            public float MinimumActionSpeedAdjustment => RSingle(BaseAddress + 0x198);
            public float MinimumInfluenceProductionMultiplier => RSingle(BaseAddress + 0x19c);
            public TimeOfDayComponent TimeOfDayComponent => new TimeOfDayComponent(RIntPtr(BaseAddress + 0x1a0));
            public float CommunityStandingHighWaterMark => RSingle(BaseAddress + 0x12f0);
            public float PlayTime => RSingle(BaseAddress + 0x1308);
        }
        public class DaytonCharacter : UObject
        {
            public DaytonCharacter(IntPtr addr) : base(addr) { }
            public int ID
            {
                get => RInt32(BaseAddress + 0x368);
                set => WInt32(BaseAddress + 0x368, value);
            }
            public string FirstName => new TextProperty(RIntPtr(BaseAddress + 0x380)).Value;
            public string LastName => new TextProperty(RIntPtr(BaseAddress + 0x398)).Value;
            public IntPtr NickNamePtr => RIntPtr(BaseAddress + 0x3b0);

            public string VoiceID => GetNameFromNameOffset(RInt32(BaseAddress + 0x3C8));
            public string CulturalBackground => GetNameFromNameOffset(RInt32(BaseAddress + 0x3D0));
            public string HumanDefinition => GetNameFromNameOffset(RInt32(BaseAddress + 0x3E8));
            public ECharacterStanding StandingLevel
            {
                get => (ECharacterStanding)RUInt8(BaseAddress + 0x412);
                set => WUInt8(BaseAddress + 0x412, (byte)value);
            }
            public string HeroBonus => GetNameFromNameOffset(RInt32(BaseAddress + 0x418));

            public string LeaderType => GetNameFromNameOffset(RInt32(BaseAddress + 0x428));
            public Equipment Equipment => new Equipment(RIntPtr(BaseAddress + 0x438));
            public List<CharacterSkillRecord> Skills
            {
                get
                {
                    var skills = new List<CharacterSkillRecord>();
                    int count = numSkills;

                    if (count <= 0)
                        return skills;

                    IntPtr skillsArrayPtr = RIntPtr(BaseAddress + 0x450);

                    for (int i = 0; i < count; i++)
                    {
                        IntPtr skillPtr = skillsArrayPtr + i * 0x18;

                        if (skillPtr != IntPtr.Zero)
                        {
                            skills.Add(new CharacterSkillRecord(skillPtr));
                        }
                    }

                    return skills;
                }
            }
            public int numSkills => RInt32(BaseAddress + 0x458);
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
            public float TraumaCounter
            {
                get => RSingle(BaseAddress + 0x48C);
                set => WSingle(BaseAddress + 0x48C, value);
            }
            public float InjuryRecoveryCounter
            {
                get => RSingle(BaseAddress + 0x490);
                set => WSingle(BaseAddress + 0x490, value);
            }

            public float MaxStaminaBase => RSingle(BaseAddress + 0xAF4);

            public float MaxHealthBase => RSingle(BaseAddress + 0xB00);
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
        }

        public class DaytonHumanCharacter : UObject
        {
            public DaytonHumanCharacter(IntPtr addr) : base(addr) { }
            public DaytonCharacterComponent CharacterComponent => new DaytonCharacterComponent(RIntPtr(BaseAddress + 0xBa0));
            public float MaxSickness => RSingle(BaseAddress + 0xc3c);
            public float CurrentEffectiveMaxHealth => RSingle(BaseAddress + 0xc58);
            public float CurrentHealth => RSingle(BaseAddress + 0xc5c);
            public byte bIsInCombat => RUInt8(BaseAddress + 0xc70);
        }
        public class DaytonLocalPlayer : UObject
        {
            public DaytonLocalPlayer(IntPtr addr) : base(addr) { }
            public DaytonPlayerController DaytonPlayerController => new DaytonPlayerController(RIntPtr(BaseAddress + 0x30));
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
            public float LastSampledCommunityStanding => RSingle(BaseAddress + 0x554);
            public float LastSampledTimeOfDay => RSingle(BaseAddress + 0x558);
            public byte bIsNightTime => RUInt8(BaseAddress + 0x55c);
            public int numSpawnedPawns => RInt32(BaseAddress + 0x5f8);
            public int AmbientPopulationCapCount => RInt32(BaseAddress + 0x610);
        }
        public class DaytonPlayerController : UObject
        {
            public DaytonPlayerController(IntPtr addr) : base(addr) { }
            public IntPtr EnclaveTradeManagerPtr => RIntPtr(BaseAddress + 0x758);
            public IntPtr FollowerManagerPtr => RIntPtr(BaseAddress + 0x7a8);
            public float InfestationRangeToReportOnHUD => RSingle(BaseAddress + 0x8b4);
            public float BloodPlagueRangeToReportOnHUD => RSingle(BaseAddress + 0x8b8);
            public int ProximateZombieCount => RInt32(BaseAddress + 0x944);
            public DaytonHumanCharacter AnalyticsPawn => new DaytonHumanCharacter(RIntPtr(BaseAddress + 0xb58));
            public CommunityComponent CommunityComponent => new CommunityComponent(RIntPtr(BaseAddress + 0xb60));
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

        public class Equipment : UObject
        {
            public Equipment(IntPtr addr) : base(addr) { }
            public MeleeWeaponItemInstance MeleeWeaponItemInstance => new MeleeWeaponItemInstance(RIntPtr(RIntPtr(BaseAddress + 0x38) + 0x00));
            public RangedWeaponItemInstance RangedWeaponItemInstance => new RangedWeaponItemInstance(RIntPtr(RIntPtr(BaseAddress + 0x38) + 0x08));
            public RangedWeaponItemInstance SideArmRangedWeaponItemInstance => new RangedWeaponItemInstance(RIntPtr(RIntPtr(BaseAddress + 0x38) + 0x28));
        }
        public class FireSettings : UObject
        {
            public FireSettings(IntPtr addr) : base(addr) { }
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
        public class MapStateComponent : UObject
        {
            public MapStateComponent(IntPtr addr) : base(addr) { }
        }
        public class MeleeWeaponItemInstance : UObject
        {
            public MeleeWeaponItemInstance(IntPtr addr) : base(addr) { }
            public MeleeWeaponResourceStats Stats => new MeleeWeaponResourceStats(RIntPtr(BaseAddress + 0xF8));

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
        public class RangedWeapon : UObject
        {
            public RangedWeapon(IntPtr addr) : base(addr) { }
            public RangedWeaponResourceStats Stats => new RangedWeaponResourceStats(RIntPtr(BaseAddress + 0x28));

            public class RangedWeaponItemInstance : UObject
            {
                public RangedWeaponItemInstance(IntPtr addr) : base(addr) { }
                public RangedWeapon RangedWeapon => new RangedWeapon(RIntPtr(BaseAddress + 0x70));

            }
        }
        public class RangedWeaponItemInstance : UObject
        {
            public RangedWeaponItemInstance(IntPtr addr) : base(addr) { }
            public RangedWeapon RangedWeapon => new RangedWeapon(RIntPtr(BaseAddress + 0x70));
            
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
            public float PerceptionLoudness
            {
                get => RSingle(BaseAddress + 0x2D8);
                set => WSingle(BaseAddress + 0x2D8, value);
            }
            public string TracerSettings => RUnicodeStr(RIntPtr(BaseAddress + 0x358));
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

    }
}

