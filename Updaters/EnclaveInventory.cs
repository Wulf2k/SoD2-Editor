using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static Iced.Intel.AssemblerRegisters;

namespace SoD2_Editor
{
    public partial class Form1 : Form
    {
        private Enclave lastEncInv = new Enclave(IntPtr.Zero);
        private DataTable _EnclaveInventoryRows = new DataTable();
        private void InitEnclaveInventoryTable()
        {
            _EnclaveInventoryRows = new DataTable();
            _EnclaveInventoryRows.Columns.Add("Addr", typeof(string));
            _EnclaveInventoryRows.Columns.Add("Type", typeof(string));
            _EnclaveInventoryRows.Columns.Add("Class", typeof(string));
            _EnclaveInventoryRows.Columns.Add("Name", typeof(string));
            _EnclaveInventoryRows.Columns.Add("Qty", typeof(string));
            _EnclaveInventoryRows.Columns.Add("Value", typeof(string));
            _EnclaveInventoryRows.PrimaryKey = new[] { _EnclaveInventoryRows.Columns["Addr"] };

            dgvEnclaveInventory.DataSource = _EnclaveInventoryRows;
            dgvEnclaveInventory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEnclaveInventory.MultiSelect = false;
            dgvEnclaveInventory.ReadOnly = true;
            dgvEnclaveInventory.AllowUserToAddRows = false;
            dgvEnclaveInventory.AllowUserToDeleteRows = false;
            dgvEnclaveInventory.RowHeadersVisible = false;

            dgvEnclaveInventory.Columns["Addr"].Visible = false;
            dgvEnclaveInventory.Columns["Class"].Visible = false;
            dgvEnclaveInventory.Columns["Name"].HeaderText = "Name";

        }
        private void UpdateEnclaveInventoryList(Enclave enc)
        {
            
            dgvEnclaveInventory.SuspendLayout();
            if (lastEncInv.BaseAddress != enc.BaseAddress)
            {
                lastEncInv = enc;
                _EnclaveInventoryRows.Clear();
            }
            foreach (var item in enc.Inventory.Slots)
            {
                string hexAddr = item.BaseAddress.ToString("X");
                Item i = new Item(item.ItemClass.ClassDefaultObject.BaseAddress);
                string name = ItemStripMarkupText(i.DisplayName);

                DataRow row = _EnclaveInventoryRows.Rows.Find(hexAddr);
                if (row == null)
                {
                    row = _EnclaveInventoryRows.NewRow();
                    row["Addr"] = hexAddr;
                    row["Type"] = item.Type;
                    row["Class"] = i.Class.Name;
                    row["Name"] = name;
                    _EnclaveInventoryRows.Rows.Add(row);
                    object qty = DBNull.Value;
                    if (item is AmmoItemInstance ammo) qty = ammo.stackCount;
                    else if (item is CloseCombatItemInstance closeCombat) qty = closeCombat.stackCount;
                    else if (item is ConsumableItemInstance cons) qty = cons.stackCount;
                    else if (item is MiscellaneousItemInstance misc) qty = misc.stackCount;
                    else if (item is ResourceItemInstance resource) qty = resource.stackCount;
                    row["Qty"] = qty.ToString();
                }
                else
                {
                    SetIfChanged(row, "Name", name);
                    SetIfChanged(row, "Value", i.InfluenceValue.ToString("0000"));

                    object qty = DBNull.Value;
                    if (item is AmmoItemInstance ammo) qty = ammo.stackCount;
                    else if (item is CloseCombatItemInstance closeCombat) qty = closeCombat.stackCount;
                    else if (item is ConsumableItemInstance cons) qty = cons.stackCount;
                    else if (item is MiscellaneousItemInstance misc) qty = misc.stackCount;
                    else if (item is ResourceItemInstance resource) qty = resource.stackCount;

                    SetIfChanged(row, "Qty", qty.ToString());
                }
            }

            /*
            for (int i = _EnclaveInventoryRows.Rows.Count - 1; i >= 0; i--)
            {
                string hexAddr = _EnclaveInventoryRows.Rows[i]["Addr"].ToString();
                if (!enc.Inventory.Slots.Any(e => e.BaseAddress.ToString("X") == hexAddr))
                    _EnclaveInventoryRows.Rows[i].Delete();
            }*/
            dgvEnclaveInventory.ResumeLayout();


        }
        private void SetIfChanged(DataRow row, string column, object newValue)
        {
            var current = row[column];
            if ((current == DBNull.Value && newValue != DBNull.Value) ||
                (current != DBNull.Value && !Equals(current, newValue)))
            {
                row[column] = newValue;
            }
        }
        private void btnEnclaveInventorySetQty_Click(object sender, EventArgs e)
        {
            if (dgvEnclaveInventory.SelectedRows.Count == 0)
                return;

            var row = dgvEnclaveInventory.SelectedRows[0];
            string addrHex = row.Cells["Addr"].Value.ToString();
            string type = row.Cells["Type"].Value.ToString();

            if (!ulong.TryParse(addrHex, System.Globalization.NumberStyles.HexNumber, null, out ulong baseAddr))
            {
                Output("Address failed to parse.");
                return;
            }
            ItemInstance instance = null;
            int newQty = int.Parse(txtEnclaveInventoryNewQty.Text);

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
        

        private void btnTogglenclaveItemAdder_Click(object sender, EventArgs e)
        {
            pnlItemAdder.Visible = !pnlItemAdder.Visible;
        }

        
    }
}
