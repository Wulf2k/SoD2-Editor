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
        private DataTable _enclaveCharactersTable;
        private void InitEnclaveCharactersTable()
        {
            _enclaveCharactersTable = new DataTable();
            _enclaveCharactersTable.Columns.Add("Addr", typeof(string));
            _enclaveCharactersTable.Columns.Add("Name", typeof(string));
            _enclaveCharactersTable.PrimaryKey = new[] { _enclaveCharactersTable.Columns["Addr"] };

            dgvEnclaveCharacters.DataSource = _enclaveCharactersTable;
            dgvEnclaveCharacters.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEnclaveCharacters.MultiSelect = false;
            dgvEnclaveCharacters.ReadOnly = true;
            dgvEnclaveCharacters.AllowUserToAddRows = false;
            dgvEnclaveCharacters.AllowUserToDeleteRows = false;
            dgvEnclaveCharacters.RowHeadersVisible = false;
            dgvEnclaveCharacters.DataBindingComplete += (s, e) =>
            {
                if (dgvEnclaveCharacters.Columns.Contains("Addr"))
                    dgvEnclaveCharacters.Columns["Addr"].Visible = false;

                dgvEnclaveCharacters.Columns["Name"].HeaderText = "Name";
                dgvEnclaveCharacters.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            };
        }
        private void UpdateEnclaveCharacterList()
        {
            if (!long.TryParse(txtEnclaveAddress.Text, System.Globalization.NumberStyles.HexNumber,
                null, out long encaddr) || encaddr == 0)
                return;

            var enclave = new Enclave((IntPtr)encaddr);
            var characters = enclave.Characters;

            foreach (var character in characters)
            {
                string hexAddr = character.BaseAddress.ToString("X");
                string name = $"{character.CharacterRecord.FirstName} {character.CharacterRecord.LastName}";

                DataRow row = _enclaveCharactersTable.Rows.Find(hexAddr);
                if (row == null)
                {
                    row = _enclaveCharactersTable.NewRow();
                    row["Addr"] = hexAddr;
                    row["Name"] = name;
                    _enclaveCharactersTable.Rows.Add(row);
                }
                else
                {
                    if (!row["Name"].Equals(name))
                        row["Name"] = name;
                }
            }

            for (int i = _enclaveCharactersTable.Rows.Count - 1; i >= 0; i--)
            {
                string hexAddr = _enclaveCharactersTable.Rows[i]["Addr"].ToString();
                if (!characters.Any(c => c.BaseAddress.ToString("X") == hexAddr))
                {
                    _enclaveCharactersTable.Rows[i].Delete();
                }
            }

            string currentCharHex = currDaytonCharacter.BaseAddress.ToString("X");
            foreach (DataGridViewRow dgRow in dgvEnclaveCharacters.Rows)
            {
                if (dgRow.Cells["Addr"].Value?.ToString() == currentCharHex)
                {
                    dgRow.DefaultCellStyle.ForeColor = Color.Red;
                    dgRow.DefaultCellStyle.Font = new Font(dgvEnclaveCharacters.Font, FontStyle.Bold);
                }
                else
                {
                    dgRow.DefaultCellStyle.ForeColor = Color.White;
                    dgRow.DefaultCellStyle.Font = dgvEnclaveCharacters.Font;
                }
            }
        }


        private void dgvEnclaveCharacters_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEnclaveCharacters.SelectedRows.Count > 0)
            {
                string hexAddr = dgvEnclaveCharacters.SelectedRows[0].Cells["Addr"].Value.ToString();
                txtCharacterAddress.Text = hexAddr;

                _characterSkillsTable.Rows.Clear();
                _selectedCharacterSkillRecord = null;
                _selectedCharacterSkillRecordField = "";
                txtSkillValue.Text = "";
                lblCharacterSkillValue.Text = "";
            }
        }
    }
}
