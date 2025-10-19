using System;
using System.Collections.Generic;
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

        private DataTable _enclaveTable;
        private EnclaveManager _enclaveManagerRef;

        private void InitEnclaveTable()
        {
            _enclaveTable = new DataTable();
            _enclaveTable.Columns.Add("Addr", typeof(string));
            _enclaveTable.Columns.Add("Name", typeof(string));
            _enclaveTable.PrimaryKey = new[] { _enclaveTable.Columns["Addr"] };

            dgvEnclaves.DataSource = _enclaveTable;
            dgvEnclaves.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEnclaves.MultiSelect = false;
            dgvEnclaves.ReadOnly = true;
            dgvEnclaves.AllowUserToAddRows = false;
            dgvEnclaves.AllowUserToDeleteRows = false;
            dgvEnclaves.RowHeadersVisible = false;

            dgvEnclaves.Columns["Addr"].Visible = false;
            dgvEnclaves.Columns["Name"].HeaderText = "Name";
            dgvEnclaves.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
        private void UpdateEnclaveList(EnclaveManager enclaveManager)
        {
            _enclaveManagerRef = enclaveManager;
            if (_enclaveManagerRef.Name != "EnclaveManagerBp")
                return;

            foreach (var enclave in enclaveManager.Enclaves)
            {
                string hexAddr = enclave.BaseAddress.ToString("X");

                DataRow row = _enclaveTable.Rows.Find(hexAddr);
                if (row == null)
                {
                    row = _enclaveTable.NewRow();
                    row["Addr"] = hexAddr;
                    row["Name"] = enclave.DisplayName;
                    _enclaveTable.Rows.Add(row);
                }
                else
                {
                    row["Name"] = enclave.DisplayName;
                }
            }

            for (int i = _enclaveTable.Rows.Count - 1; i >= 0; i--)
            {
                string hexAddr = _enclaveTable.Rows[i]["Addr"].ToString();
                if (!enclaveManager.Enclaves.Any(e => e.BaseAddress.ToString("X") == hexAddr))
                    _enclaveTable.Rows[i].Delete();
            }
        }
        private void dgvEnclaves_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEnclaves.SelectedRows.Count == 0)
                return;

            string hexAddr = dgvEnclaves.SelectedRows[0].Cells["Addr"].Value.ToString();
            txtEnclaveAddress.Text = hexAddr;



            txtCharacterAddress.Text = "0";
            lblInventory.Text = "";
            lblCharacterTraits.Text = "";
            lblCharactersLabel.Text = "";
            lblEnclavesLabel.Text = "";
            lblEquipment.Text = "";
            lblMelee.Text = "";
            lblRanged.Text = "";
            lblSidearm.Text = "";

            _characterDetailRows.Clear();
            _characterSkillsTable.Clear();
            _inventoryRows.Clear();
            _meleeWeaponDetailRows.Clear();
            _rangedWeaponDetailRows.Clear();

            tlpEnclavesCharactersDetails.Controls.Clear();
            txtEnclavesCharactersInventoryAddress.Text = "0";
            tlpEnclaveCharactersInventory.Controls.Clear();
            tlpMeleeWeaponStats.Controls.Clear();
            tlpRangedWeaponStats.Controls.Clear();
            tlpSideArmWeaponStats.Controls.Clear();
        }
    }
}
