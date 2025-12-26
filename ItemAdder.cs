using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Iced.Intel.AssemblerRegisters;

namespace SoD2_Editor
{
    public partial class Form1 : Form
    {
        List<UClass> ammo = new List<UClass>();
        List<UClass> backpack = new List<UClass>();
        List<UClass> ccw = new List<UClass>();
        List<UClass> cons = new List<UClass>();
        List<UClass> ranged = new List<UClass>();
        List<UClass> facilitymods = new List<UClass>();
        List<UClass> melee = new List<UClass>();
        List<UClass> misc = new List<UClass>();
        List<UClass> mods = new List<UClass>();
        List<UClass> resource = new List<UClass>();
        List<UClass> unknown = new List<UClass>();

        List<UClass> vehicles = new List<UClass>();

        DataTable ammoItemDt = new DataTable();
        DataTable backpackItemDt = new DataTable();
        DataTable consItemDt = new DataTable();
        DataTable ccwItemDt = new DataTable();
        DataTable facModsItemDt = new DataTable();
        DataTable meleeItemDt = new DataTable();
        DataTable miscItemDt = new DataTable();
        DataTable modsItemDt = new DataTable();
        DataTable rangedItemDt = new DataTable();
        DataTable resourceItemDt = new DataTable();

        IntPtr CodeAddItem = IntPtr.Zero;
        private IntPtr _selectedItemToAdd = IntPtr.Zero;

        private async Task InitItemLists()
        {
            lblItemListStatus.Text = "Initializing Item Lists....";
            lblItemListStatus.Refresh();

            clbItemLists.Items.Clear();
            clbItemLists.Items.Add("Initializing...");
            clbItemLists.SelectedIndex = 0;

            ammo = new List<UClass>();
            backpack = new List<UClass>();
            ccw = new List<UClass>();
            cons = new List<UClass>();
            ranged = new List<UClass>();
            facilitymods = new List<UClass>();
            melee = new List<UClass>();
            misc = new List<UClass>();
            mods = new List<UClass>();
            resource = new List<UClass>();
            unknown = new List<UClass>();
            ammoItemDt = new DataTable();
            backpackItemDt = new DataTable();
            ccwItemDt = new DataTable();
            facModsItemDt = new DataTable();
            modsItemDt = new DataTable();
            meleeItemDt = new DataTable();
            miscItemDt = new DataTable();
            rangedItemDt = new DataTable();
            resourceItemDt = new DataTable();

            //vehicles = new List<UClass>();

            //var miscTable = await Task.Run(() =>
            await Task.Run(async () =>
            {
                int numobjs = RInt32(addresses.Get("ObjTablePtr") + 0xc);
                objTablePtr = RIntPtr(addresses.Get("ObjTablePtr"));

                for (int i = 0; i < numobjs; i++)
                {
                    UClass obj = new UClass(RIntPtr(objTablePtr + i * 0x18));
                    if (obj.Type != "BlueprintGeneratedClass")
                    {
                        continue;
                    }
                    string path = obj.Path();

                    
                    if (path.StartsWith("/Game/Items/CloseCombatItems") && !path.Contains("/SourceData"))
                        ccw.Add(obj);
                    else if (path.StartsWith("/Game/Items/MeleeWeapons") && !path.Contains("/Design"))
                        melee.Add(obj);
                    else if (path.StartsWith("/Game/Items/Mods"))
                        mods.Add(obj);
                    else if (path.StartsWith("/Game/Items/Consumables"))
                        cons.Add(obj);
                    else if (path.StartsWith("/Game/Items/RangedWeapons") &&
                    !path.Contains("/Design") &&
                    !path.Contains("/Arced"))
                        ranged.Add(obj);
                    else if (path.StartsWith("/Game/Items/MiscellaneousItems"))
                        misc.Add(obj);
                    else if (path.StartsWith("/Game/Items/FacilityModItems"))
                        facilitymods.Add(obj);


                    else if (path.StartsWith("/Game/Items/ResourceItems/Food/"))
                    {
                        if (obj.Name != "FoodRucksack_C")
                            resource.Add(obj);
                    }
                    else if (path.StartsWith("/Game/Items/ResourceItems/Meds/"))
                    {
                        if (obj.Name != "MedsRucksack_C")
                            resource.Add(obj);
                    }
                    else if (path.StartsWith("/Game/Items/ResourceItems/Ammo/"))
                    {
                        if (obj.Name != "AmmoRucksack_C")
                            resource.Add(obj);
                    }
                    else if (path.StartsWith("/Game/Items/ResourceItems/Fuel/"))
                    {
                        if (obj.Name != "FuelRucksack_C")
                            resource.Add(obj);
                    }
                    else if (path.StartsWith("/Game/Items/ResourceItems/Materials/"))
                    {
                        if (obj.Name != "MaterialsRucksack_C")
                            resource.Add(obj);
                    }
                    else if (path.StartsWith("/Game/Items/ResourceItems/Parts/"))
                    {
                        if (obj.Name != "PartsRucksack_C")
                            resource.Add(obj);
                    }
                    else if (path.StartsWith("/Game/Items/BackpackItems"))
                        backpack.Add(obj);
                    else if (path.StartsWith("/Game/Items/Ammo"))
                        ammo.Add(obj);
                }



                foreach (UObject obj2 in vehicles)
                  Console.WriteLine($"{obj2.BaseAddress.ToString("X")} - {obj2.Path()}");

                DataTable itemList(
                    List<UClass> list, 
                    Action<DataRow, UClass> perRow = null, 
                    Action<DataTable> extraColumns = null)
                {
                    var dt = new DataTable();
                    dt.Columns.Add("Addr", typeof(string));
                    dt.Columns.Add("Type", typeof(string));
                    dt.Columns.Add("Class", typeof(string));
                    dt.Columns.Add("Path", typeof(string));
                    dt.Columns.Add("Name", typeof(string));
                    dt.Columns.Add("Weight", typeof(float));
                    dt.Columns.Add("Value", typeof(float));

                    extraColumns?.Invoke(dt);

                    dt.PrimaryKey = new[] { dt.Columns["Addr"] };

                    foreach (UClass cls in list)
                    {
                        Item obj = new Item(cls.ClassDefaultObject.BaseAddress);
                        string hexAddr = cls.BaseAddress.ToString("X");

                        var row = dt.NewRow();
                        row["Addr"] = hexAddr;
                        row["Type"] = obj.Type;
                        row["Class"] = cls.Name;
                        row["Path"] = cls.Path();
                        row["Name"] = ItemStripMarkupText(obj.DisplayName);
                        row["Weight"] = obj.Weight;
                        row["Value"] = obj.InfluenceValue;

                        perRow?.Invoke(row, cls);
                        dt.Rows.Add(row);
                    }
                    return dt;
                }



                await Task.Run(async () =>
                {
                    var ammoTask = Task.Run(() => itemList(ammo));
                    var backpackTask = Task.Run(() => itemList(
                        backpack,
                        perRow: (row, cls) =>
                        {
                            var bpObj = new BackpackItem(cls.ClassDefaultObject.BaseAddress);
                            row["Capacity"] = bpObj.Capactity;
                        },
                        extraColumns: dt =>
                        {
                            dt.Columns.Add("Capacity", typeof(int));
                        }));
                    var consTask = Task.Run(() => itemList(cons));
                    var ccwTask = Task.Run(() => itemList(ccw));
                    var facModsTask = Task.Run(() => itemList(facilitymods));
                    var miscTask = Task.Run(() => itemList(
                        misc,
                        perRow: (row, cls) =>
                        {
                            var miscObj = new MiscellaneousItem(cls.ClassDefaultObject.BaseAddress);
                            row["ItemType"] = miscObj.MiscellaneousItemType.ToString();
                            row["ItemSubType"] = miscObj.ItemSubType.ToString();
                        },
                        extraColumns: dt =>
                        {
                            dt.Columns.Add("ItemType", typeof(string));
                            dt.Columns.Add("ItemSubType", typeof(string));
                        }));
                    var modsTask = Task.Run(() => itemList(mods));
                    var resourceTask = Task.Run(() => itemList(
                        resource,
                        perRow: (row, cls) =>
                        {
                            var resObj = new ResourceItem(cls.ClassDefaultObject.BaseAddress);
                            row["Resource"] = resObj.ResourceType.ToString();
                        },
                        extraColumns: dt =>
                        {
                            dt.Columns.Add("Resource", typeof(string));
                        }));
                    var meleeTask = Task.Run(() => itemList(
                        melee,
                        perRow: (row, cls) =>
                        {
                            var meleeObj = new MeleeWeaponItem(cls.ClassDefaultObject.BaseAddress);
                            row["DType"] = meleeObj.MeleeType.ToString();
                            row["Dismember"] = meleeObj.Resource.Stats.Dismember;
                            row["DismemberDelta"] = meleeObj.Resource.Stats.DismemberDelta;
                            row["Impact"] = meleeObj.Resource.Stats.Impact;
                            row["ImpactDelta"] = meleeObj.Resource.Stats.ImpactDelta;
                            row["Knockdown"] = meleeObj.Resource.Stats.Knockdown;
                            row["KnockdownDelta"] = meleeObj.Resource.Stats.KnockdownDelta;
                            row["Lethality"] = meleeObj.Resource.Stats.Lethality;
                            row["LethalityDelta"] = meleeObj.Resource.Stats.LethalityDelta;
                            row["Loudness"] = meleeObj.Resource.Stats.PerceptionLoudness;
                            row["Speed"] = meleeObj.Resource.Stats.Speed;
                            row["SwingCost"] = meleeObj.Resource.Stats.SwingCost;
                            row["Durability"] = meleeObj.Resource.Stats.Durability;
                            row["DurabilityLossPerHitMin"] = meleeObj.Resource.Stats.DurabilityLossPerHitMin;
                            row["DurabilityLossPerHitMax"] = meleeObj.Resource.Stats.DurabilityLossPerHitMax;
                            row["DurabilityLossPerFinisherMin"] = meleeObj.Resource.Stats.DurabilityLossPerFinisherMin;
                            row["DurabilityLossPerFinisherMax"] = meleeObj.Resource.Stats.DurabilityLossPerFinisherMax;
                        },
                        extraColumns: dt =>
                        {
                            dt.Columns.Add("DType", typeof(string));
                            dt.Columns.Add("Dismember", typeof(float));
                            dt.Columns.Add("DismemberDelta", typeof(float));
                            dt.Columns.Add("Impact", typeof(float));
                            dt.Columns.Add("ImpactDelta", typeof(float));
                            dt.Columns.Add("Knockdown", typeof(float));
                            dt.Columns.Add("KnockdownDelta", typeof(float));
                            dt.Columns.Add("Lethality", typeof(float));
                            dt.Columns.Add("LethalityDelta", typeof(float));
                            dt.Columns.Add("Loudness", typeof(float));
                            dt.Columns.Add("Speed", typeof(float));
                            dt.Columns.Add("SwingCost", typeof(float));
                            dt.Columns.Add("Durability", typeof(float));
                            dt.Columns.Add("DurabilityLossPerHitMin", typeof(float));
                            dt.Columns.Add("DurabilityLossPerHitMax", typeof(float));
                            dt.Columns.Add("DurabilityLossPerFinisherMin", typeof(float));
                            dt.Columns.Add("DurabilityLossPerFinisherMax", typeof(float));
                        }));
                    var rangedTask = Task.Run(() => itemList(
                        ranged,
                        perRow: (row, cls) =>
                        {
                            var rangedObj = new RangedWeapon(cls.ClassDefaultObject.BaseAddress);
                            row["WClass"] = rangedObj.WeaponClass;
                            row["ProjectileCount"] = rangedObj.Stats.ProjectileCount;
                            row["MagazineSize"] = rangedObj.Stats.MagazineSize;
                            row["AmmoConsumedPerShot"] = rangedObj.Stats.AmmoConsumedPerShot;
                            row["Impact"] = rangedObj.Stats.Impact;
                            row["ImpactDelta"] = rangedObj.Stats.ImpactDelta;
                            row["Knockdown"] = rangedObj.Stats.Knockdown;
                            row["KnockdownDelta"] = rangedObj.Stats.KnockdownDelta;
                            row["Dismember"] = rangedObj.Stats.Dismember;
                            row["DismemberDelta"] = rangedObj.Stats.DismemberDelta;
                            row["Penetration"] = rangedObj.Stats.Penetration;
                            row["Range"] = rangedObj.Stats.Range;
                            row["Durability"] = rangedObj.Stats.Durability;
                            row["DurabilityLossPerShotMin"] = rangedObj.Stats.DurabilityLossPerShotMin;
                            row["DurabilityLossPerShotMax"] = rangedObj.Stats.DurabilityLossPerShotMax;
                            row["PerceptionLoudness"] = rangedObj.Stats.PerceptionLoudness;
                        },
                        extraColumns: dt =>
                        {
                            dt.Columns.Add("WClass", typeof(string));
                            dt.Columns.Add("ProjectileCount", typeof(int));
                            dt.Columns.Add("MagazineSize", typeof(int));
                            dt.Columns.Add("AmmoConsumedPerShot", typeof(int));
                            dt.Columns.Add("Impact", typeof(float));
                            dt.Columns.Add("ImpactDelta", typeof(float));
                            dt.Columns.Add("Knockdown", typeof(float));
                            dt.Columns.Add("KnockdownDelta", typeof(float));
                            dt.Columns.Add("Dismember", typeof(float));
                            dt.Columns.Add("DismemberDelta", typeof(float));
                            dt.Columns.Add("Penetration", typeof(float));
                            dt.Columns.Add("Range", typeof(float));
                            dt.Columns.Add("Durability", typeof(float));
                            dt.Columns.Add("DurabilityLossPerShotMin", typeof(float));
                            dt.Columns.Add("DurabilityLossPerShotMax", typeof(float));
                            dt.Columns.Add("PerceptionLoudness", typeof(float));
                        }
                    ));
                    

                    await Task.WhenAll(ammoTask, backpackTask, ccwTask, facModsTask, meleeTask, miscTask, modsTask, rangedTask, resourceTask);

                    ammoItemDt = ammoTask.Result;
                    backpackItemDt = backpackTask.Result;
                    consItemDt = consTask.Result;
                    ccwItemDt = ccwTask.Result;
                    facModsItemDt = facModsTask.Result;
                    meleeItemDt = meleeTask.Result;
                    miscItemDt = miscTask.Result;
                    modsItemDt = modsTask.Result;
                    rangedItemDt = rangedTask.Result;
                    resourceItemDt = resourceTask.Result;
                });
                
            });

            clbItemLists.Items.Clear();
            clbItemLists.Items.Add("Ammo");
            clbItemLists.Items.Add("Backpacks");
            clbItemLists.Items.Add("Consumables");
            clbItemLists.Items.Add("Close Combat");
            clbItemLists.Items.Add("Facility Mods");
            clbItemLists.Items.Add("Melee");
            clbItemLists.Items.Add("Misc");
            clbItemLists.Items.Add("Mods");
            clbItemLists.Items.Add("Ranged");
            clbItemLists.Items.Add("Rucksacks");

            lblItemListStatus.Text = "";
            lblItemListStatus.Refresh();
            clbItemLists.SelectedIndex = 0;

            dgvItemAdder.CellClick += dgvItemAdd_CellClick;
        }

        private void clbItemLists_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvItemAdder.SuspendLayout();
            dgvItemAdder.DataSource = null;
            dgvItemAdder.Rows.Clear();
            DataTable dt = new DataTable();
            switch (clbItemLists.Items[clbItemLists.SelectedIndex].ToString())
            {
                case "Ammo":
                    dt = ammoItemDt;
                    break;
                case "Backpacks":
                    dt = backpackItemDt;
                    break;
                case "Consumables":
                    dt = consItemDt;
                    break;
                case "Close Combat":
                    dt = ccwItemDt;
                    break;
                case "Facility Mods":
                    dt = facModsItemDt;
                    break;
                case "Melee":
                    dt = meleeItemDt;
                    break;
                case "Mods":
                    dt = modsItemDt;
                    break;
                case "Misc":
                    dt = miscItemDt;
                    break;
                case "Ranged":
                    dt = rangedItemDt;
                    break;
                case "Rucksacks":
                    dt = resourceItemDt;
                    break;
                default:
                    return;
            }


            dgvItemAdder.DataSource = dt;
            dgvItemAdder.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItemAdder.MultiSelect = false;
            dgvItemAdder.ReadOnly = true;
            dgvItemAdder.AllowUserToAddRows = false;
            dgvItemAdder.AllowUserToDeleteRows = false;
            dgvItemAdder.RowHeadersVisible = false;
            dgvItemAdder.Columns["Addr"].Visible = false;
            dgvItemAdder.Columns["Class"].Visible = false;
            //dgvEnclaveItemAddMisc.Columns["Type"].Visible = false;

            switch (clbItemLists.Items[clbItemLists.SelectedIndex].ToString())
            {
                case "Ammo":
                    dgvItemAdder.Columns["Type"].Visible = false;
                    dgvItemAdder.Columns["Path"].Visible = false;
                    break;
                case "Backpacks":
                    dgvItemAdder.Columns["Type"].Visible = false;
                    dgvItemAdder.Columns["Path"].Visible = false;
                    break;
                case "Consumables":
                    //dgvItemAdder.Columns["Type"].Visible = false;
                    dgvItemAdder.Columns["Path"].Visible = false;
                    break;
                case "Close Combat":
                    dgvItemAdder.Columns["Type"].Visible = false;
                    dgvItemAdder.Columns["Path"].Visible = false;
                    break;
                case "Facility Mods":
                    dgvItemAdder.Columns["Type"].Visible = false;
                    dgvItemAdder.Columns["Path"].Visible = false;
                    break;
                case "Melee":
                    dgvItemAdder.Columns["DType"].DisplayIndex = 0;
                    dgvItemAdder.Columns["Type"].Visible = false;
                    dgvItemAdder.Columns["Path"].Visible = false;
                    break;
                case "Mods":
                    dgvItemAdder.Columns["Path"].Visible = false;
                    break;
                case "Misc":
                    dgvItemAdder.Columns["ItemType"].DisplayIndex = 0;
                    dgvItemAdder.Columns["ItemSubType"].DisplayIndex = 1;
                    dgvItemAdder.Columns["Type"].Visible = false;
                    dgvItemAdder.Columns["Path"].Visible = false;
                    break;
                case "Ranged":
                    dgvItemAdder.Columns["WClass"].DisplayIndex = 0;
                    dgvItemAdder.Columns["Type"].Visible = false;
                    dgvItemAdder.Columns["Path"].Visible = false;
                    break;
                case "Rucksacks":
                    dgvItemAdder.Columns["Resource"].DisplayIndex = 0;
                    dgvItemAdder.Columns["Type"].Visible = false;
                    dgvItemAdder.Columns["Path"].Visible = false;
                    break;
                default:
                    return;
            }
            dgvItemAdder.ResumeLayout();
        }



        private void dgvItemAdd_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var dgv = sender as DataGridView;
            if (dgv == null || !dgv.Columns.Contains("Name"))
                return;

            var row = dgv.Rows[e.RowIndex];
            string itemName = row.Cells["Class"].Value?.ToString() ?? "";
            lbltemAddName.Text = itemName;

            string addrHex = row.Cells["Addr"].Value?.ToString();

            if (!string.IsNullOrEmpty(addrHex) &&
                ulong.TryParse(addrHex, System.Globalization.NumberStyles.HexNumber, null, out ulong addr))
            {
                _selectedItemToAdd = (IntPtr)addr;
            }
            else
            {
                _selectedItemToAdd = IntPtr.Zero;
            }
        }
        



        private void btnItemAdderAdd_Click(object sender, EventArgs e)
        {
            Inventory inv;
            if (long.TryParse(txtEnclaveAddress.Text, System.Globalization.NumberStyles.HexNumber, null, out long encaddr) && encaddr > 0)
            {
                IntPtr enclavePtr = new IntPtr(encaddr);
                var enclave = new Enclave(enclavePtr);
                inv = enclave.Inventory;
            }
            else
            {
                Output("Failed to parse Enclave address");
                return;
            }

            //UClass ItemBP = new UClass(RIntPtr(_selectedItemToAdd + 0x10));
            UClass ItemBP = new UClass(_selectedItemToAdd);
            AddItem(inv, ItemBP);
        }



        
        private void AddItem(Inventory inv, UClass ItemBP)
        {
            if (CodeAddItem == IntPtr.Zero)
                CodeAddItem = Alloc(0x1000);

            if (inv.BaseAddress == IntPtr.Zero)
            {
                Output("Inventory pointer not found.");
                return;
            }

            if (ItemBP.Type != "BlueprintGeneratedClass")
            {
                Output("BlueprintGeneratedClass not found, aborting Add.");
                return;
            }


            int Quantity = 1;

            AddressBook ItemAddFuncs = new AddressBook();

            ItemAddFuncs.Add("CreateItemInstance", _ba + 0x004efce0, _ba + 0x004f0490);
            ItemAddFuncs.Add("TryAddItem", _ba + 0x00502010, _ba + 0x00502900);

            Iced.Intel.Assembler asm = new Iced.Intel.Assembler(bitness: 64);

            asm.sub(rsp, 0x1000);
            asm.pushfq();


            asm.mov(rcx, (ulong)ItemBP.BaseAddress);
            asm.mov(rdx, Quantity);
            asm.call((ulong)ItemAddFuncs.Get("CreateItemInstance"));

            asm.mov(rcx, (ulong)inv.BaseAddress);
            asm.mov(rdx, rax);
            asm.mov(r8, 7);
            asm.mov(r9, 1);
            asm.mov(rax, rsp);
            asm.add(rax, 0x20);
            asm.mov(rbx, 1);
            asm.mov(__[rax], rbx);
            asm.call((ulong)ItemAddFuncs.Get("TryAddItem"));


            asm.popfq();
            asm.add(rsp, 0x1000);
            asm.ret();

            var stream = new MemoryStream();
            asm.Assemble(new Iced.Intel.StreamCodeWriter(stream), (ulong)CodeAddItem);
            byte[] machineCode = stream.ToArray();
            WBytes(CodeAddItem, machineCode);

            Console.WriteLine(CodeAddItem.ToString("X"));
            CreateRemoteThread(_proc, IntPtr.Zero, 0, CodeAddItem, IntPtr.Zero, 0, IntPtr.Zero);
            Output($"Added {ItemBP.Name} to {inv.Name}");
        }
    }
}
