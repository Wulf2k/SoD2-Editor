using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoD2_Editor
{
    public partial class Form1 : Form
    {
        private DataTable _communityResourceTable;
        private BindingSource _bindingSource;
        private CommunityResourceBase _selectedCommunityResource;
        private string _selectedCommunityResourceField;
        private void UpdateCommunity()
        {
            switch (tabCommunityTabs.SelectedTab.Name)
            {
                case "tabCommunityResources":
                    UpdateCommunityResources();
                    break;
            }


        }





        private void UpdateCommunityResources()
        {

            DaytonLocalPlayer localPlayer = new DaytonLocalPlayer(RIntPtr(addresses.Get("DaytonLocalPlayer")));
            CommunityComponent Community = localPlayer.DaytonPlayerController.CommunityComponent;

            if (Community.BaseAddress == IntPtr.Zero)
                return;

            List<CommunityResourceBase> resources = Community.Resources;

            while (_communityResourceTable.Rows.Count < resources.Count)
                _communityResourceTable.Rows.Add(_communityResourceTable.NewRow());
            dgvCommunityResources.SuspendLayout();

            for (int i = 0; i < resources.Count; i++)
            {
                var res = resources[i];
                var row = _communityResourceTable.Rows[i];

                row["Addr"] = res.BaseAddress.ToString("X");
                row["Type"] = res.ResourceType.ToString();
                row["Supply"] = res.Supply;
                row["Accumulator"] = res.Accumulator;
                row["Description"] = res.Description;
            }
            dgvCommunityResources.ResumeLayout(false);
            while (_communityResourceTable.Rows.Count > resources.Count)
                _communityResourceTable.Rows.RemoveAt(_communityResourceTable.Rows.Count - 1);
        }
        private void InitializeCommunityResourceGrid()
        {
            _communityResourceTable = new DataTable();
            _communityResourceTable.Columns.Add("Addr", typeof(string));
            _communityResourceTable.Columns.Add("Type", typeof(string));
            _communityResourceTable.Columns.Add("Supply", typeof(float));
            _communityResourceTable.Columns.Add("Accumulator", typeof(float));
            _communityResourceTable.Columns.Add("Description", typeof(string));
            
            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _communityResourceTable;
            dgvCommunityResources.DataSource = _bindingSource;
            EnableDarkMode(dgvCommunityResources);

            dgvCommunityResources.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvCommunityResources.Columns["Type"].Width = 80;
            dgvCommunityResources.Columns["Supply"].Width = 80;
            dgvCommunityResources.Columns["Accumulator"].Width = 100;
            dgvCommunityResources.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvCommunityResources.Columns["Addr"].Visible = false;

            dgvCommunityResources.CellClick += dgvCommunityResources_CellClick;
        }
        private void dgvCommunityResources_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var columnName = dgvCommunityResources.Columns[e.ColumnIndex].Name;
            if (columnName != "Supply" && columnName != "Accumulator")
                return;

            var resources = new DaytonLocalPlayer(RIntPtr(addresses.Get("DaytonLocalPlayer")))
                .DaytonPlayerController.CommunityComponent.Resources;

            if (e.RowIndex >= resources.Count)
                return;

            var res = resources[e.RowIndex];
            _selectedCommunityResource = res;
            _selectedCommunityResourceField = columnName;

            txtCommunityNewVal.Text = columnName == "Supply" ? res.Supply.ToString("0.00") : res.Accumulator.ToString("0.00");
            lblCommunityResourceName.Text = res.DisplayName;
        }
        private void btnCommunityResourceSetValue_Click(object sender, EventArgs e)
        {
            if (_selectedCommunityResource == null || string.IsNullOrEmpty(_selectedCommunityResourceField))
            {
                Output("No Resource Selected.");
                return;
            }

            if (!float.TryParse(txtCommunityNewVal.Text, out float newValue))
            {
                Output("Not a valid number.");
                return;
            }

            if (_selectedCommunityResourceField == "Supply")
            {
                _selectedCommunityResource.Supply = newValue;
            }
            else if (_selectedCommunityResourceField == "Accumulator")
            {
                _selectedCommunityResource.Accumulator = newValue;
            }

            Output($"Set {_selectedCommunityResource.Name} to {newValue}");
        }
    }
}
