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
                        tlpInspector.AutoScrollPosition = new Point(0, 0);

                        tlpInspector.Controls.Clear();
                        tlpInspector.ColumnStyles.Clear();
                        tlpInspector.RowStyles.Clear();
                        tlpInspector.ColumnCount = 8;
                        tlpInspector.RowCount = 0;

                        tlpInspector.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Address
                        tlpInspector.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80)); // Offset
                        tlpInspector.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Class
                        tlpInspector.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200)); // Type
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
                            if (inspecting.Interface.BaseAddress != IntPtr.Zero)
                                fullClass += ":";
                            chain.Add(inspecting);
                            inspecting = inspecting.Interface;
                        }

                        lblInspector.Text = $"{obj.BaseAddress.ToInt64():X16} {fullClass}";
                        
                        //for (int i = chain.Count - 1; i >= 0; i--)
                        for (int i = 0; i < chain.Count - 1; i++)
                            {
                            var initprops = chain[i].GetProperties();
                            foreach (var p in initprops)
                            {
                                tlpInspector.RowCount++;
                                tlpInspector.RowStyles.Add(new RowStyle(SizeType.Absolute, 16));
                                int rowIndex = tlpInspector.RowCount - 1;

                                
                                tlpInspector.Controls.Add(new Label { Text = $"{chain[i].Name}", AutoSize = true, Font = new Font("Consolas", 9) }, 2, rowIndex);
                                tlpInspector.Controls.Add(new Label { Text = p.Type, AutoSize = true, Font = new Font("Consolas", 9) }, 3, rowIndex);
                                tlpInspector.Controls.Add(new Label { Text = p.Name, AutoSize = true, Font = new Font("Consolas", 9) }, 5, rowIndex);



                                Button btnInspect = new Button
                                {
                                    Text = "Edit",
                                    Tag = (p, obj.BaseAddress + p.Offset),
                                    Width = 25,
                                    Height = 10
                                };
                                btnInspect.Click += (s, f) =>
                                {
                                    var (prop, addrPtr) = ((UProperty, IntPtr))((Button)s).Tag;

                                    txtInspectorAddress.Text = RIntPtr(addrPtr).ToString("X");
                                };

                                //If it's an object, add the button to Inspect
                                if (p.Type == "ObjectProperty")
                                    tlpInspector.Controls.Add(btnInspect, 4, rowIndex);

                                //If it's a function, hide the offset and address
                                if (p.Type != "Function")
                                {
                                    tlpInspector.Controls.Add(new Label { Text = $"{IntPtr.Add(obj.BaseAddress, p.Offset).ToString("X16")}", AutoSize = true, Font = new Font("Consolas", 9) }, 0, rowIndex);
                                    tlpInspector.Controls.Add(new Label { Text = $"0x{p.Offset:X4}", AutoSize = true, Font = new Font("Consolas", 9) }, 1, rowIndex);
                                }
                                Label valLabel = new Label { AutoSize = true, Font = new Font("Consolas", 9), Name = $"val_{p.BaseAddress}{p.Name}" };
                                tlpInspector.Controls.Add(valLabel, 6, rowIndex);
                                tlpInspector.RowStyles.Add(new RowStyle(SizeType.AutoSize, 16));

                                tlpInspector.Controls.Add(new Label { Text = $"0x{p.BaseAddress.ToString("X16")}", AutoSize = true, Font = new Font("Consolas", 9) }, 7, rowIndex);
                                Console.WriteLine($"{tlpInspector.RowCount} {p.Name}");
                            }//end foreach
                        }
                        
                        tlpInspector.RowCount++;
                        

                    }
                    
                    void AddProps(UClass objClass)
                    {
                        var props = objClass.GetAllProperties();

                        foreach (var p in props)
                        {
                            IntPtr valueAddr = obj.BaseAddress + p.Offset;
                            string valueStr;

                            switch (p.Type)
                            {
                                case "BoolProperty":
                                case "ByteProperty":
                                    valueStr = RUInt8(valueAddr).ToString();
                                    break;
                                case "ClassProperty":
                                    valueStr = (new UClass(RIntPtr(valueAddr))).Name;
                                    break;
                                case "FloatProperty":
                                    valueStr = RSingle(valueAddr).ToString();
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
                                    IntPtr ptr = RIntPtr(valueAddr);
                                    if (ptr != IntPtr.Zero)
                                    {
                                        UObject refObj = new UObject(ptr);
                                        valueStr = $"0x{ptr.ToInt64():X16}   ({refObj.Class.BaseClass.Name}:{refObj.Name})";
                                    }
                                    else
                                    {
                                        valueStr = "null";
                                    }
                                    break;
                                case "StrProperty":
                                    valueStr = RUnicodeStr(RIntPtr(valueAddr));
                                    break;
                                case "TextProperty":
                                    TextProperty tp = new TextProperty(RIntPtr(valueAddr));
                                    valueStr = tp.Value;
                                    break;
                                case "UInt32Property":
                                    valueStr = RUInt32(valueAddr).ToString();
                                    break;
                                case "UInt64Property":
                                    valueStr = RUInt64(valueAddr).ToString();
                                    break;
                                default:
                                    valueStr = "(unhandled)";
                                    break;
                            }

                            var valControl = tlpInspector.Controls.Find($"val_{p.BaseAddress}{p.Name}", false).FirstOrDefault() as Label;
                            if (valControl != null && valControl.Text != valueStr)
                            {
                                valControl.Text = valueStr;
                            }
                        }
                    }//end AddProps
                    AddProps(obj.Class);
                    
                }
                tlpInspector.ResumeLayout();
            }
    }//end UpdateInspector

    }
}
