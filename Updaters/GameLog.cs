using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoD2_Editor
{
    public partial class Form1 : Form
    {
        private int _lastLogEnd = 0;

        private void UpdateLog()
        {
            int newLogEnd = GameLog.LogEndOffset;
            if (newLogEnd == _lastLogEnd)
                return;

            if (newLogEnd > _lastLogEnd)
            {
                string diff = RAsciiStr(GameLog.GameLogTextPtr + _lastLogEnd, newLogEnd - _lastLogEnd);
                rtbGameLog.AppendText(diff);
            }
            else
            {
                if (RInt64(GameLog.BaseAddress) > 0)
                    rtbGameLog.Text = RAsciiStr(GameLog.GameLogTextPtr, newLogEnd);
            }

            rtbGameLog.SelectionStart = rtbGameLog.Text.Length;
            rtbGameLog.ScrollToCaret();

            _lastLogEnd = newLogEnd;
        }
        private class LogLevelRow
        {
            public string Name { get; set; }
            public byte Value { get; set; }
        }
        private BindingList<LogLevelRow> _logLevelRows = new BindingList<LogLevelRow>();
        private void InitializeLogLevelGrid(List<string> fieldNames)
        {
            dgvLogLevels.SuspendLayout();
            dgvLogLevels.Columns.Clear();
            dgvLogLevels.AutoGenerateColumns = false;
            dgvLogLevels.AllowUserToAddRows = false;
            dgvLogLevels.AllowUserToDeleteRows = false;
            dgvLogLevels.RowHeadersVisible = false;
            dgvLogLevels.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLogLevels.MultiSelect = false;
            dgvLogLevels.EditMode = DataGridViewEditMode.EditOnEnter;

            dgvLogLevels.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Name",
                DataPropertyName = "Name",
                ReadOnly = true
                
            });

            var valueColumn = new DataGridViewTextBoxColumn
            {
                Name = "Value",
                HeaderText = "Value",
                DataPropertyName = "Value",
                Width = 80
            };
            dgvLogLevels.Columns.Add(valueColumn);

            _logLevelRows.Clear();

            foreach (var name in fieldNames)
            {
                _logLevelRows.Add(new LogLevelRow { Name = name, Value = 0 });
            }

            dgvLogLevels.DataSource = _logLevelRows;
            dgvLogLevels.ResumeLayout();
        }

        private void UpdateLogLevels()
        {
            if (_logLevelRows.Count == 0)
                InitializeLogLevelGrid(logNames);

            foreach (var row in _logLevelRows)
            {
                IntPtr addr = gamelogs.Get(row.Name);
                byte currentValue = RUInt8(addr);

                if (row.Value != currentValue)
                {
                    row.Value = currentValue;
                }
            }

            dgvLogLevels.Refresh();
        }
        private void dgvLogLevels_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var row = _logLevelRows[e.RowIndex];

            lblLogLevel.Text = row.Name;
            txtLogLevelValue.Text = row.Value.ToString();
        }
        private void btnSetLogLevel_Click(object sender, EventArgs e)
        {
            if (byte.TryParse(txtLogLevelValue.Text, out byte newValue))
            {
                WUInt8(gamelogs.Get(lblLogLevel.Text), newValue);
            }
        }
        
        private void btnGameLogLevelsSave_Click(object sender, EventArgs e)
        {
            const string baseKeyPath = @"Software\Wulf\SoDEaD\LogLevels";

            using (RegistryKey baseKey = Registry.CurrentUser.CreateSubKey(baseKeyPath))
            {
                if (baseKey == null)
                    throw new InvalidOperationException("Failed to open registry key: " + baseKeyPath);

                foreach (var gamelog in gamelogs._map)
                {
                    string name = gamelog.Key;
                    baseKey.SetValue(name, RUInt8(gamelogs.Get(name)), RegistryValueKind.QWord);
                }
            }
            Output("LogLevels saved to HKCU\\Software\\Wulf\\SoDEaD\\LogLevels");
        }

        private void btnGameLogLevelsLoad_Click(object sender, EventArgs e)
        {
            const string baseKeyPath = @"Software\Wulf\SoDEaD\LogLevels";

            using (RegistryKey baseKey = Registry.CurrentUser.OpenSubKey(baseKeyPath))
            {
                if (baseKey == null)
                {
                    Output("No saved LogLevels found in registry.");
                    return;
                }

                foreach (string name in baseKey.GetValueNames())
                {
                    object value = baseKey.GetValue(name);
                    if (value is long qwordValue)
                    {
                        WUInt8(gamelogs.Get(name), (byte)qwordValue);
                    }
                    else
                    {
                        Output($"Skipping invalid registry value for {name}");
                    }
                }
            }
            Output("LogLevels loaded from HKCU\\Software\\Wulf\\SoDEaD\\LogLevels");
        }
    }
}
