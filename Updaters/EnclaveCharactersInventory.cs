using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoD2_Editor
{
    public partial class Form1 : Form
    {
        private DataTable _inventoryRows = new DataTable();
        private void InitCharacterInventoryTable()
        {
            _inventoryRows = new DataTable();
            _inventoryRows.Columns.Add("Addr", typeof(string));
            _inventoryRows.Columns.Add("Type", typeof(string));
            _inventoryRows.Columns.Add("Name", typeof(string));
            _inventoryRows.Columns.Add("Qty", typeof(int));
            _inventoryRows.Columns.Add("Value", typeof(float));
            _inventoryRows.PrimaryKey = new[] { _inventoryRows.Columns["Addr"] };

            dgvEnclaveCharactersInventory.DataSource = _inventoryRows;
            dgvEnclaveCharactersInventory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEnclaveCharactersInventory.MultiSelect = false;
            dgvEnclaveCharactersInventory.ReadOnly = true;
            dgvEnclaveCharactersInventory.AllowUserToAddRows = false;
            dgvEnclaveCharactersInventory.AllowUserToDeleteRows = false;
            dgvEnclaveCharactersInventory.RowHeadersVisible = false;

            dgvEnclaveCharactersInventory.Columns["Addr"].Visible = false;
            dgvEnclaveCharactersInventory.Columns["Name"].HeaderText = "Name";

        }
        private void UpdateCharacterInventoryList(DaytonCharacter chr)
        {
            
            foreach (var item in chr.CharacterRecord.Inventory.Slots)
            {
                string hexAddr = item.BaseAddress.ToString("X");

                DataRow row = _inventoryRows.Rows.Find(hexAddr);
                if (row == null)
                {
                    row = _inventoryRows.NewRow();
                    row["Addr"] = hexAddr;
                    row["Type"] = item.Type;
                    row["Name"] = item.Name;
                    _inventoryRows.Rows.Add(row);
                }
                else
                {
                    Item i = new Item(item.ItemClass.ClassDefaultObject.BaseAddress);
                    string name = i.DisplayName;
                    name = name.Replace("{!v}", "");
                    name = name.Replace("{[0,+]'}", "");
                    name = name.Replace("{[0,+]s}", "");
                    name = name.Replace("{[0,+]es}", "");
                    row["Name"] = name;




                    float val = i.InfluenceValue;
                    row["Value"] = val;



                    if (item is AmmoItemInstance ammo)
                        row["Qty"] = ammo.stackCount;

                    else if (item is BackpackItemInstance backpack)
                        row["Qty"] = DBNull.Value;

                    else if (item is CloseCombatItemInstance closeCombat)
                        row["Qty"] = closeCombat.stackCount;

                    else if (item is ConsumableItemInstance cons)
                        row["Qty"] = cons.stackCount;

                    else if (item is FacilityModItemInstance facilityMod)
                        row["Qty"] = DBNull.Value;

                    else if (item is MeleeWeaponItemInstance melee)
                        row["Qty"] = DBNull.Value;

                    else if (item is MiscellaneousItemInstance misc)
                        row["Qty"] = misc.stackCount;

                    else if (item is RangedWeaponItemInstance ranged)
                        row["Qty"] = DBNull.Value;

                    else if (item is RangedWeaponModItemInstance rangedMod)
                        row["Qty"] = DBNull.Value;

                    else if (item is ResourceItemInstance resource)
                        row["Qty"] = resource.stackCount;

                    else if (item is ItemInstance baseItem)
                        row["Qty"] = DBNull.Value;
                    else
                        row["Qty"] = DBNull.Value;
                }
            }

            for (int i = _inventoryRows.Rows.Count - 1; i >= 0; i--)
            {
                string hexAddr = _inventoryRows.Rows[i]["Addr"].ToString();
                if (!chr.CharacterRecord.Inventory.Slots.Any(e => e.BaseAddress.ToString("X") == hexAddr))
                    _inventoryRows.Rows[i].Delete();
            }
           
        }
        private void btnEnclaveCharactersInventorySetQty_Click(object sender, EventArgs e)
        {
            if (dgvEnclaveCharactersInventory.SelectedRows.Count == 0)
                return;

            var row = dgvEnclaveCharactersInventory.SelectedRows[0];
            string addrHex = row.Cells["Addr"].Value.ToString();
            string type = row.Cells["Type"].Value.ToString();

            if (!ulong.TryParse(addrHex, System.Globalization.NumberStyles.HexNumber, null, out ulong baseAddr))
            {
                Output("Address failed to parse.");
                return;
            }
            ItemInstance instance = null;
            int newQty = int.Parse(TxtEnclaveCharactersInventoryNewQty.Text);

            switch (type)
            {
                case "AmmoItemInstance":
                    instance = new AmmoItemInstance((IntPtr)baseAddr);
                    ((AmmoItemInstance)instance).stackCount = newQty;
                    break;

                case "CloseCombatItemInstance":
                    instance = new CloseCombatItemInstance((IntPtr)baseAddr);
                    ((CloseCombatItemInstance)instance).stackCount = newQty;
                    break;

                case "ConsumableItemInstance":
                    instance = new ConsumableItemInstance((IntPtr)baseAddr);
                    ((ConsumableItemInstance)instance).stackCount = newQty;
                    break;

                case "MiscellaneousItemInstance":
                    instance = new MiscellaneousItemInstance((IntPtr)baseAddr);
                    ((MiscellaneousItemInstance)instance).stackCount = newQty;
                    break;

                default:
                    Output($"Changing Qty unsupported for {type}");
                    return;
            }
            Output($"Quantity set.");
        }
    }
}
