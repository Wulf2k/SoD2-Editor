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
        private void tabs_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



        private DataTable _inspectorTable;

        private void InitInspectorGrid()
        {
            _inspectorTable = new DataTable();
            _inspectorTable.Columns.Add("Address", typeof(string));
            _inspectorTable.Columns.Add("Offset", typeof(string));
            _inspectorTable.Columns.Add("Ptr", typeof(string));
            _inspectorTable.Columns.Add("Type", typeof(string));
            _inspectorTable.Columns.Add("Name", typeof(string));
            _inspectorTable.Columns.Add("Value", typeof(string));
            

            dgvInspector.DataSource = _inspectorTable;

            dgvInspector.ReadOnly = true;
            dgvInspector.AllowUserToAddRows = false;
            dgvInspector.AllowUserToDeleteRows = false;
            dgvInspector.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInspector.MultiSelect = false;


            
            //dgvInspector.Columns["Ptr"].Visible = false;
        }
        private void UpdateInspector()
        {
            if (!long.TryParse(txtInspectorAddress.Text,
                              System.Globalization.NumberStyles.HexNumber,
                              null,
                              out long addr) && addr > 0)
            {
                return;
            }

            UObject obj = new UObject((IntPtr)addr);
            bool init = false;

            if (obj.Class.BaseAddress != IntPtr.Zero)
            {
                if (lastInspected != obj.BaseAddress)
                {
                    init = true;
                    lblInspector.Text = "Processing...";
                    lblInspector.Refresh();

                    if (inspectHistory.Count == 0 || inspectHistory.Last() != obj.BaseAddress)
                    {
                        inspectHistory.Add(obj.BaseAddress);

                        if (inspectHistory.Count > 50)
                            inspectHistory.RemoveAt(0);
                    }
                    if (inspectHistory.Count > 1)
                    {
                        btnInspectorBack.Enabled = true;
                    }
                    else
                    {
                        btnInspectorBack.Enabled = false;
                    }
                    lastInspected = obj.BaseAddress;
                }
            }
            dgvInspector.SuspendLayout();
            
            if (init)
            {
                dgvInspector.DataSource = null;
                _inspectorTable.Rows.Clear();
                string fullClass = "";
                var chain = new List<UClass>();
                UClass inspecting = obj.Class;
                while (inspecting.BaseAddress != IntPtr.Zero)
                {
                    fullClass += $"{inspecting.Name} (0x{inspecting.Size.ToString("X")})";
                    if (inspecting.BaseClass.BaseAddress != IntPtr.Zero)
                        fullClass += ":";
                    chain.Add(inspecting);
                    inspecting = inspecting.BaseClass;
                }

                lblInspector.Text = $"{obj.Name} - {obj.BaseAddress.ToInt64():X16} - {fullClass}";
                lblInspectorPath.Text = obj.Path();

                for (int i = 0; i < chain.Count - 1; i++)
                {
                    var initprops = chain[i].GetProperties();
                    foreach (var p in initprops)
                    {
                        string offsetStr = $"0x{p.Offset:X4}";
                        string nameStr = p.Name;

                        bool exists = _inspectorTable.Rows
                            .Cast<DataRow>()
                            .Any(r => r["Offset"].ToString() == offsetStr && r["Name"].ToString() == nameStr);

                        if (!exists)
                        {
                            IntPtr valueAddr = obj.BaseAddress + p.Offset;
                            DataRow row = _inspectorTable.NewRow();
                            row["Address"] = valueAddr.ToString("X16");
                            row["Offset"] = offsetStr;
                            row["Type"] = p.Field.Type;
                            row["Name"] = nameStr;
                            //row["Value"] = valueStr;
                            //row["Ptr"] = valueAddr;

                            _inspectorTable.Rows.Add(row);
                        }
                    }
                }
                InspectorUpdateRows();
                dgvInspector.DataSource = _inspectorTable;
                dgvInspector.Columns["Address"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvInspector.Columns["Offset"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvInspector.Columns["Ptr"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvInspector.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvInspector.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvInspector.Columns["Value"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


                dgvInspector.Columns["Address"].Visible = false;
            }//end init
            else
            {
                InspectorUpdateRows();
            }
            dgvInspector.ResumeLayout();

        }
        private void InspectorUpdateRows()
        {
            foreach (DataRow row in _inspectorTable.Rows)
            {
                IntPtr valueAddr = IntPtr.Zero;
                if (long.TryParse(row["Address"].ToString(),
                              System.Globalization.NumberStyles.HexNumber,
                              null,
                              out long vaddr) && vaddr > 0)
                {
                    valueAddr = (IntPtr)vaddr;
                }

                string newValue = "";
                string newPtr = "";

                switch (row["Type"])
                {
                    case "AssetClassProperty":
                        newValue = GetObjFromObjId(RInt32(valueAddr)).Name;
                        break;

                    case "ArrayProperty":
                        int num = RInt32(valueAddr + 0x8);
                        int maxnum = RInt32(valueAddr + 0xc);
                        
                        newValue = $"{num}/{maxnum}";
                        break;

                    case "BoolProperty":
                    case "ByteProperty":
                        newValue = RUInt8(valueAddr).ToString();
                        break;

                    case "ClassProperty":
                        IntPtr defptr = RIntPtr(RIntPtr(valueAddr) + 0x100);
                        if (defptr != IntPtr.Zero)
                        {
                            UObject refObj = new UObject(defptr);
                            newValue = $"{refObj.Type}";
                            newPtr = defptr.ToString("X");
                        }
                        else
                        {
                            newValue = "null";
                            newPtr = "";
                        }
                        break;

                    case "FloatProperty":
                        newValue = RSingle(valueAddr).ToString();
                        break;

                    case "Int8Property":
                        newValue = RBytes(valueAddr, 1)[0].ToString();
                        break;

                    case "InterfaceProperty":
                        IntPtr ifacePtr = RIntPtr(valueAddr);
                        if (ifacePtr != IntPtr.Zero)
                        {
                            UObject refObj = new UObject(ifacePtr);
                            newValue = $"{refObj.Name}";
                            newPtr = ifacePtr.ToString("X");
                        }
                        else
                        {
                            newValue = "null";
                            newPtr = "";
                        }

                        break;

                    case "IntProperty":
                        newValue = RInt32(valueAddr).ToString();
                        break;

                    case "MulticastDelegateProperty":
                        newValue = GetNameFromNameOffset(RInt32(RIntPtr(valueAddr) + 0x8));
                        break;

                    case "NameProperty":
                        newValue = GetNameFromNameOffset(RInt32(valueAddr));
                        break;

                    case "ObjectProperty":
                        IntPtr objPtr = RIntPtr(valueAddr);
                        if (objPtr != IntPtr.Zero)
                        {
                            UObject refObj = new UObject(objPtr);
                            newValue = $"{refObj.Name}";
                            newPtr = refObj.BaseAddress.ToString("X");
                        }
                        else
                        {
                            newValue = "null";
                            newPtr = "";
                        }
                        break;

                    case "StrProperty":
                        newValue = RUnicodeStr(RIntPtr(valueAddr));
                        break;

                    case "StructProperty":
                        newValue = "(struct)";
                        break;

                    case "TextProperty":
                        FText tp = new FText(valueAddr);
                        newValue = tp.Value;
                        break;

                    case "UInt16Property":
                        newValue = RUInt16(valueAddr).ToString();
                        break;

                    case "UInt32Property":
                        newValue = RUInt32(valueAddr).ToString();
                        break;

                    case "UInt64Property":
                        newValue = RUInt64(valueAddr).ToString();
                        break;

                    case "WeakObjectProperty":
                        UObject weakObj = GetObjFromObjId(RInt32(valueAddr));
                        newValue = weakObj.Name;
                        newPtr = weakObj.BaseAddress.ToString("X");
                        break;

                    default:
                        newValue = $"{row["Type"]} - (unhandled)";
                        break;
                }
                string type = row["Type"].ToString();

                if (type == "Function" && row["Offset"].ToString() != "")
                        row["Offset"] = "";


                if (!Equals(row["Value"], newValue))
                {
                    row["Value"] = newValue;
                }
                if (!Equals(row["Ptr"], newPtr))
                {
                    row["Ptr"] = newPtr;
                }
                /*if (!Equals(row["Type"], newType))
                {
                    row["Type"] = newType;
                }*/
            }
        }
        private void dgvInspector_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvInspector.Rows[e.RowIndex];
            txtInspectorValue.Text = row.Cells["Value"].Value?.ToString();
            lblInspectorType.Text = row.Cells["Type"].Value?.ToString();
            lblInspectorAddress.Text = row.Cells["Address"].Value?.ToString();

            

            btnInspectorSetValue.Enabled = false;
            switch (row.Cells["Type"].Value?.ToString())
            {
                case "BoolProperty":
                case "ByteProperty":
                case "IntProperty":
                case "FloatProperty":
                case "UInt32Property":
                    btnInspectorSetValue.Enabled = true;
                    break;
            }

        }
        private void dgvInspector_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvInspector.Rows[e.RowIndex];
            txtInspectorAddress.Text = row.Cells["Ptr"].Value?.ToString();
        }

        private void btnInspectorSetValue_Click(object sender, EventArgs e)
        {
            string hexAddr = lblInspectorAddress.Text;
            IntPtr ptr = (IntPtr)Convert.ToInt64(hexAddr, 16);

            switch (lblInspectorType.Text)
            {
                case "BoolProperty":
                case "ByteProperty":
                    byte byteValue;
                    if (!byte.TryParse(txtInspectorValue.Text, out byteValue))
                    {
                        Output("Failed to parse byte value");
                        return;
                    }

                    WUInt8(ptr, byteValue);
                    Output("ByteProperty value set");
                    break;
                case "FloatProperty":
                    float floatValue;
                    if (!float.TryParse(txtInspectorValue.Text, out floatValue))
                    {
                        Output("Failed to parse float value");
                        return;
                    }
                    
                    WSingle(ptr, floatValue);
                    Output("FloatProperty value set");
                    break;
                case "IntProperty":
                    int intValue;
                    if (!int.TryParse(txtInspectorValue.Text, out intValue))
                    {
                        Output("Failed to parse int value");
                        return;
                    }

                    WInt32(ptr, intValue);
                    Output("IntProperty value set");
                    break;
                case "UInt32Property":
                    uint uint32Value;
                    if (!uint.TryParse(txtInspectorValue.Text, out uint32Value))
                    {
                        Output("Failed to parse uint32 value");
                        return;
                    }

                    WUInt32(ptr, uint32Value);
                    Output("UInt32Property value set");
                    break;
            }
        }
        private void btnInspectorBack_Click(object sender, EventArgs e)
        {
            if (inspectHistory.Count > 1)
            {
                inspectHistory.RemoveAt(inspectHistory.Count - 1);
                IntPtr prevAddr = inspectHistory.Last();
                txtInspectorAddress.Text = $"{prevAddr.ToString("X")}";
            }
        }
        private void btnInspectController_Click(object sender, EventArgs e)
        {
            txtInspectorAddress.Text = localPlayer?.DaytonPlayerController.BaseAddress.ToString("X");
        }

        private void btnInspectClosestEnemy_Click(object sender, EventArgs e)
        {
            txtInspectorAddress.Text = currDaytonHumanCharacter?.ClosestEnemy.ToString("X");
        }
        private void btnInspectDHC_Click(object sender, EventArgs e)
        {
            txtInspectorAddress.Text = currDaytonHumanCharacter?.BaseAddress.ToString("X");
        }
        private void btnInspectEngine_Click(object sender, EventArgs e)
        {
            txtInspectorAddress.Text = RIntPtr(addresses.Get("GameEngine")).ToString("X");
        }
        
        private void btnInspectWorld_Click(object sender, EventArgs e)
        {
            txtInspectorAddress.Text = world?.BaseAddress.ToString("X");
        }


        private void dgvInspectItem_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var dgv = sender as DataGridView;
            if (dgv == null || !dgv.Columns.Contains("Addr"))
                return;

            string addrHex = dgv.Rows[e.RowIndex].Cells["Addr"].Value?.ToString() ?? "";
            txtInspectorAddress.Text = addrHex;
            tabs.SelectedTab = tabs.TabPages["tabInspector"];
        }
    }
}
