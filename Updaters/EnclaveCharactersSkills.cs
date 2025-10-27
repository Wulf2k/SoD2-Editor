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


        private DataTable _characterSkillsTable;
        private CharacterSkillRecord _selectedCharacterSkillRecord;
        private string _selectedCharacterSkillRecordField;
        private void InitCharacterSkillsTable()
        {
            _characterSkillsTable = new DataTable();
            _characterSkillsTable.Columns.Add("Addr", typeof(string));
            _characterSkillsTable.Columns.Add("Name", typeof(string));
            _characterSkillsTable.Columns.Add("Level", typeof(int));
            _characterSkillsTable.Columns.Add("XP", typeof(float));
            _characterSkillsTable.Columns.Add("Granted By Trait", typeof(string));

            dgvCharacterSkills.DataSource = _characterSkillsTable;
            dgvCharacterSkills.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvCharacterSkills.MultiSelect = false;
            dgvCharacterSkills.ReadOnly = true;
            dgvCharacterSkills.AllowUserToAddRows = false;
            dgvCharacterSkills.AllowUserToDeleteRows = false;
            dgvCharacterSkills.RowHeadersVisible = false;

            dgvCharacterSkills.Columns["Addr"].Visible = false;
            dgvCharacterSkills.Columns["XP"].DefaultCellStyle.Format = "F4";
            dgvCharacterSkills.Columns["XP"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCharacterSkills.Columns["Granted By Trait"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
        private void UpdateCharacterSkills(DaytonCharacter chr)
        {
            if (chr.Name != "DaytonCharacter")
                return;

            foreach (var skill in chr.CharacterRecord.Skills)
            {
                string skillName = skill.Name;
                var row = _characterSkillsTable.Rows.Cast<DataRow>()
                             .FirstOrDefault(r => (string)r["Name"] == skillName);

                if (row == null)
                {
                    row = _characterSkillsTable.NewRow();
                    row["Addr"] = skill.BaseAddress.ToString("X");
                    row["Name"] = skill.Name;
                    row["Level"] = skill.CurrentLevel;
                    row["XP"] = skill.CurrentXp;
                    row["Granted By Trait"] = skill.GrantingTrait;
                    _characterSkillsTable.Rows.Add(row);
                }
                else
                {
                    row["Level"] = skill.CurrentLevel;
                    row["XP"] = skill.CurrentXp;
                }
            }

            for (int i = _characterSkillsTable.Rows.Count - 1; i >= 0; i--)
            {
                string name = _characterSkillsTable.Rows[i]["Name"].ToString();
                if (!chr.CharacterRecord.Skills.Any(c => c.Name == name))
                {
                    _characterSkillsTable.Rows[i].Delete();
                }
            }

        }
        private void dgvCharacterSkills_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!long.TryParse(txtCharacterAddress.Text, System.Globalization.NumberStyles.HexNumber,
                    null, out long chraddr) || chraddr == 0)
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var columnName = dgvCharacterSkills.Columns[e.ColumnIndex].Name;
            if (columnName != "Level" && columnName != "XP")
            {
                _selectedCharacterSkillRecord = null;
                _selectedCharacterSkillRecordField = "";
                lblCharacterSkillValue.Text = "";
                txtSkillValue.Text = "";
                return;
            }
                

            var skills = (new DaytonCharacter((IntPtr)chraddr)).CharacterRecord.Skills;

            if (e.RowIndex >= skills.Count)
                return;

            var skill = skills[e.RowIndex];
            lblCharacterSkillValue.Text = columnName;

            txtSkillValue.Text = columnName == "Level" ? skill.CurrentLevel.ToString() : skill.CurrentXp.ToString("0.0000");
            lblCharacterSkillValue.Text = $"{skill.Name} - {columnName}";
            _selectedCharacterSkillRecord = skill;
            _selectedCharacterSkillRecordField = columnName;
        }
        private void btnSetSkillValue_Click(object sender, EventArgs e)
        {
            if (_selectedCharacterSkillRecord == null || string.IsNullOrEmpty(_selectedCharacterSkillRecordField))
            {
                Output("No Skill Selected.");
                return;
            }

            if (!float.TryParse(txtSkillValue.Text, out float newValue))
            {
                Output("Not a valid number.");
                return;
            }

            if (_selectedCharacterSkillRecordField == "XP")
            {
                _selectedCharacterSkillRecord.CurrentXp = newValue;
            }
            else if (_selectedCharacterSkillRecordField == "Level")
            {
                if (newValue < 8 && newValue > -1)
                {
                    _selectedCharacterSkillRecord.CurrentLevel = Convert.ToByte(newValue);
                }
                else
                {
                    Output("Level must be between 0 and 7.");
                    return;
                }
            }

            Output($"Set {_selectedCharacterSkillRecord.Name} to {newValue}");
        }
    }
}
