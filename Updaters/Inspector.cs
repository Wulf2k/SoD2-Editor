using System;
using System.Collections.Generic;
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
            if (tabs.SelectedTab.Name == "tabInspector")
                return;

            tlpInspector.SuspendLayout();
            txtInspectorAddress.Text = "0";
            lastInspected = IntPtr.Zero;
            tlpInspector.AutoScrollPosition = new Point(0, 0);
            foreach (Control c in tlpInspector.Controls)
                c.Dispose();
            lblInspector.Text = "None";
            tlpInspector.Controls.Clear();
            tlpInspector.ColumnStyles.Clear();
            tlpInspector.RowStyles.Clear();
            tlpInspector.ColumnCount = 8;
            tlpInspector.RowCount = 0;
            tlpInspector.ResumeLayout();
        }
        public void UpdateInspector()
        {

            if (long.TryParse(txtInspectorAddress.Text,
                              System.Globalization.NumberStyles.HexNumber,
                              null,
                              out long addr) && addr > 0)
            {
                UObject obj = new UObject((IntPtr)addr);
                bool init = false;

                if (obj.Class.BaseAddress != IntPtr.Zero)
                {
                    if (lastInspected != obj.BaseAddress)
                    {
                        init = true;

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

                    

                    tlpInspector.SuspendLayout();

                    if (init)
                    {
                        lblInspector.Text = "Processing...";
                        lblInspector.Refresh();
                        tlpInspector.AutoScrollPosition = new Point(0, 0);

                        tlpInspector.Visible = false;

                        foreach (Control c in tlpInspector.Controls)
                            c.Dispose();
                        tlpInspector.Controls.Clear();
                        tlpInspector.ColumnStyles.Clear();
                        tlpInspector.RowStyles.Clear();
                        tlpInspector.ColumnCount = 8;
                        tlpInspector.RowCount = 0;

                        tlpInspector.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Address
                        tlpInspector.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Offset
                        tlpInspector.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Class
                        tlpInspector.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Type
                        tlpInspector.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30)); // Edit button
                        tlpInspector.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Name
                        tlpInspector.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Value
                        tlpInspector.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Prop pointer



                        tlpInspector.RowCount++;
                        tlpInspector.RowStyles.Add(new RowStyle(SizeType.AutoSize));

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
                        
                        //for (int i = chain.Count - 1; i >= 0; i--)
                        for (int i = 0; i < chain.Count - 1; i++)
                            {
                            var initprops = chain[i].GetProperties();
                            foreach (var p in initprops)
                            {
                                string labelName = $"val_{p.Name}";
                                if (!tlpInspector.Controls.ContainsKey(labelName))
                                {
                                    tlpInspector.RowCount++;
                                    tlpInspector.RowStyles.Add(new RowStyle(SizeType.Absolute, 16));
                                    int rowIndex = tlpInspector.RowCount - 1;

                                    //Class Name
                                    //tlpInspector.Controls.Add(new Label { Text = $"{chain[i].Name}", AutoSize = true, Font = new Font("Consolas", 9) }, 2, rowIndex);

                                    //Type
                                    //tlpInspector.Controls.Add(new Label { Text = p.Field.Type, AutoSize = true, Font = new Font("Consolas", 9) }, 3, rowIndex);

                                    //Object Name
                                    tlpInspector.Controls.Add(new Label { Text = p.Name, AutoSize = true, Font = new Font("Consolas", 9) }, 5, rowIndex);


                                    Button btnInspectAssetClassProperty = new Button
                                    {
                                        Text = "Edit",
                                        Tag = (p, obj.BaseAddress + p.Offset),
                                        Width = 25,
                                        Height = 10
                                    };
                                    btnInspectAssetClassProperty.Click += (s, f) =>
                                    {
                                        var (propTuple, addrPtr) = (((int Offset, string Name, UField Field), IntPtr))((Button)s).Tag;
                                        txtInspectorAddress.Text = RIntPtr(GetObjFromObjId(RInt32(addrPtr)).BaseAddress + 0x100).ToString("X");
                                        //valueStr = GetObjFromObjId(RInt32(valueAddr)).Name;
                                    };
                                    Button btnInspectClassProperty = new Button
                                    {
                                        Text = "Edit",
                                        Tag = (p, obj.BaseAddress + p.Offset),
                                        Width = 25,
                                        Height = 10
                                    };
                                    btnInspectClassProperty.Click += (s, f) =>
                                    {
                                        var (propTuple, addrPtr) = (((int Offset, string Name, UField Field), IntPtr))((Button)s).Tag;
                                        txtInspectorAddress.Text = RIntPtr(RIntPtr(addrPtr)+0x100).ToString("X");
                                    };
                                    Button btnInspect = new Button
                                    {
                                        Text = "Edit",
                                        Tag = (p, obj.BaseAddress + p.Offset),
                                        Width = 25,
                                        Height = 10
                                    };
                                    btnInspect.Click += (s, f) =>
                                    {
                                        var (propTuple, addrPtr) = (((int Offset, string Name, UField Field), IntPtr))((Button)s).Tag;
                                        txtInspectorAddress.Text = RIntPtr(addrPtr).ToString("X");
                                    };
                                    if(p.Field.Type == "AssetClassProperty")
                                        tlpInspector.Controls.Add(btnInspectAssetClassProperty, 4, rowIndex);
                                    if (p.Field.Type == "ClassProperty")
                                        tlpInspector.Controls.Add(btnInspectClassProperty, 4, rowIndex);
                                    //valueStr = GetObjFromObjId(RInt32(valueAddr)).Name;
                                    //If it's an object, add the button to Inspect
                                    if ((p.Field.Type == "ObjectProperty") || (p.Field.Type == "InterfaceProperty")) 
                                        tlpInspector.Controls.Add(btnInspect, 4, rowIndex);

                                    //If it's a function, hide the offset and address
                                    if ((p.Field.Type != "Function") && (p.Field.Type != "DelegateFunction"))
                                    {
                                        //Address
                                        IntPtr objaddr = obj.BaseAddress;
                                        objaddr += p.Offset;
                                        //tlpInspector.Controls.Add(new Label { Text = $"{objaddr.ToString("X16")}", AutoSize = true, Font = new Font("Consolas", 9) }, 0, rowIndex);

                                        //Offset
                                        tlpInspector.Controls.Add(new Label { Text = $"0x{p.Offset:X4}", AutoSize = true, Font = new Font("Consolas", 9) }, 1, rowIndex);
                                    }


                                    //Value Label
                                    //Label valLabel = new Label { AutoSize = true, Font = new Font("Consolas", 9), Name = $"val_{p.Field.BaseAddress}{p.Name}" };
                                    Label valLabel = new Label { AutoSize = true, Font = new Font("Consolas", 9), Name = $"val_{p.Name}" };
                                    tlpInspector.Controls.Add(valLabel, 6, rowIndex);
                                    tlpInspector.RowStyles.Add(new RowStyle(SizeType.AutoSize, 16));


                                    //Propdef pointer
                                    //tlpInspector.Controls.Add(new Label { Text = $"0x{p.Field.BaseAddress.ToString("X16")}", AutoSize = true, Font = new Font("Consolas", 9) }, 7, rowIndex);
                                    //Console.WriteLine($"{tlpInspector.RowCount} {p.Field.Name}");

                                    //Console.WriteLine($"{p.Offset.ToString("X4")} {p.Field.Type,-25} {p.Name}");
                                }
                            }//end foreach
                        }
                        
                        tlpInspector.RowCount++;
                        tlpInspector.Visible = true;
                    }//end if init
                    
                    void AddProps(UClass objClass)
                    {
                        var props = objClass.GetAllProperties();

                        foreach (var p in props)
                        {
                            IntPtr valueAddr = obj.BaseAddress + p.Offset;
                            string valueStr;

                            switch (p.Field.Type)
                            {
                                case "AssetClassProperty":                                    
                                    valueStr = GetObjFromObjId(RInt32(valueAddr)).Name;
                                    break;
                                case "ArrayProperty":
                                    int num = RInt32(valueAddr + 0x8);
                                    int maxnum = RInt32(valueAddr + 0xc);
                                    UStruct arrprop = new UStruct(p.Field.BaseAddress).innerField;
                                    arrprop = arrprop.innerField;
                                    valueStr = $"{arrprop.Name}: {num}/{maxnum}";
                                    break;
                                case "BoolProperty":
                                case "ByteProperty":
                                    valueStr = RUInt8(valueAddr).ToString();
                                    break;
                                case "ClassProperty":
                                    //valueStr = (new UClass(RIntPtr(valueAddr))).Name;
                                    IntPtr defptr = RIntPtr(RIntPtr(valueAddr) + 0x100);
                                    if (defptr != IntPtr.Zero)
                                    {
                                        valueAddr = defptr;
                                        UObject refObj = new UObject(defptr);
                                        valueStr = $"0x{defptr.ToInt64():X16}   ({refObj.Name})";
                                    }
                                    else
                                    {
                                        valueStr = "null";
                                    }
                                    break;
                                case "FloatProperty":
                                    valueStr = RSingle(valueAddr).ToString();
                                    break;
                                case "Int8Property":
                                    valueStr = RBytes(valueAddr, 1)[0].ToString();
                                    break;
                                case "InterfaceProperty":
                                    IntPtr ptr = RIntPtr(valueAddr);
                                    if (ptr != IntPtr.Zero)
                                    {
                                        UObject refObj = new UObject(ptr);
                                        valueStr = $"0x{ptr.ToInt64():X16}   ({refObj.Name})";
                                    }
                                    else
                                    {
                                        valueStr = "null";
                                    }
                                    break;
                                case "IntProperty":
                                    valueStr = RInt32(valueAddr).ToString();
                                    break;
                                case "MulticastDelegateProperty":
                                    valueStr = GetNameFromNameOffset(RInt32(RIntPtr(valueAddr) + 0x8));
                                    break;
                                case "NameProperty":
                                    valueStr = GetNameFromNameOffset(RInt32(valueAddr));
                                    break;
                                case "ObjectProperty":
                                    IntPtr objpropptr = RIntPtr(valueAddr);
                                    if (objpropptr != IntPtr.Zero)
                                    {
                                        UObject refObj = new UObject(objpropptr);
                                        valueStr = $"0x{objpropptr.ToInt64():X16}   ({refObj.Name})";
                                    }
                                    else
                                    {
                                        valueStr = "null";
                                    }
                                    break;
                                case "StrProperty":
                                    valueStr = RUnicodeStr(RIntPtr(valueAddr));
                                    break;
                                case "StructProperty":
                                    valueStr = "";
                                    break;
                                case "TextProperty":
                                    FText tp = new FText(valueAddr);
                                    valueStr = tp.Value;
                                    break;
                                case "UInt16Property":
                                    valueStr = RUInt16(valueAddr).ToString();
                                    break;
                                case "UInt32Property":
                                    valueStr = RUInt32(valueAddr).ToString();
                                    break;
                                case "UInt64Property":
                                    valueStr = RUInt64(valueAddr).ToString();
                                    break;
                                case "WeakObjectProperty":
                                    UObject o = GetObjFromObjId(obj.objId);
                                    valueStr = o.Name;
                                    break;
                                default:
                                    valueStr = $"{p.Field.Type} - (unhandled)";
                                    break;
                            }
                            valueStr = $"{p.Field.Type,-25}: {valueStr}";
                            //valueStr = $"{valueStr}";

                            //var valControl = tlpInspector.Controls.Find($"val_{p.Field.BaseAddress}{p.Name}", false).FirstOrDefault() as Label;
                            var valControl = tlpInspector.Controls.Find($"val_{p.Name}", false).FirstOrDefault() as Label;
                            if (valControl != null && valControl.Text != valueStr)
                            {
                                valControl.Text = valueStr;
                            }
                        }
                    }//end AddProps
                    AddProps(obj.Class);
                    tlpInspector.ResumeLayout();
                }
                
            }
        }//end UpdateInspector
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
            txtInspectorAddress.Text = localPlayer.DaytonPlayerController.BaseAddress.ToString("X");
        }

        private void btnInspectClosestEnemy_Click(object sender, EventArgs e)
        {
            txtInspectorAddress.Text = currDaytonHumanCharacter.ClosestEnemy.ToString("X");
        }
        private void btnInspectDHC_Click(object sender, EventArgs e)
        {
            txtInspectorAddress.Text = currDaytonHumanCharacter.BaseAddress.ToString("X");
        }
        private void btnInspectEngine_Click(object sender, EventArgs e)
        {
            txtInspectorAddress.Text = RIntPtr(addresses.Get("GameEngine")).ToString("X");
        }
        
        private void btnInspectWorld_Click(object sender, EventArgs e)
        {
            txtInspectorAddress.Text = world.BaseAddress.ToString("X");
        }
    }
}
