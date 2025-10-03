using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SoD2_Editor.Form1;

namespace SoD2_Editor
{
    public partial class Form1 : Form
    {
        public class ArrayProperty
        {
            public IntPtr BaseAddress { get; set; }

            public ArrayProperty(IntPtr addr)
            {
                BaseAddress = addr;
            }
            public int Num => RInt32(BaseAddress + 0x8);
            public int Max => RInt32(BaseAddress + 0xC);

            public IntPtr Data => RIntPtr(BaseAddress);

            public List<UObject> Entries
            {
                get
                {
                    var list = new List<UObject>();
                    IntPtr dataPtr = Data;

                    if (dataPtr == IntPtr.Zero || Num <= 0)
                        return list;

                    for (int i = 0; i < Num; i++)
                    {
                        IntPtr objPtr = RIntPtr(dataPtr + (i * IntPtr.Size));
                        if (objPtr != IntPtr.Zero)
                        {
                            list.Add(new UObject(objPtr));
                        }
                    }

                    return list;
                }
            }
        }

        public class UClass : UObject
        {
            public UClass(IntPtr addr) : base(addr) { }
            public UClass BaseClass => new UClass(RIntPtr(BaseAddress + 0x30));
            public int Size => RInt32(BaseAddress + 0x40);
            public UProperty Owner => new UProperty(BaseAddress + 0x30);
            //public UProperty nextProp => new UProperty(RIntPtr(BaseAddress + 0x58));
            public UClass Interface => new UClass(RIntPtr(BaseAddress + 0x30));
            public UObject InterfaceOut => new UObject(RIntPtr(BaseAddress + 0x38));
            public UProperty firstProp => new UProperty(RIntPtr(BaseAddress + 0x60));
            public UClass innerClass => new UClass(RIntPtr(BaseAddress + 0x78));



            public List<UProperty> GetProperties()
            {
                var props = new List<UProperty>();

                if (BaseAddress != IntPtr.Zero)
                {
                    UProperty prop = firstProp;
                    while (prop.BaseAddress != IntPtr.Zero)
                    {
                        props.Add(prop);
                        /*switch (prop.Type)
                        {
                            case "StructProperty":
                                props.AddRange(prop.innerClass.GetProperties());
                                break;
                            default:
                                break;
                        }*/
                        prop = prop.nextProp;
                    }
                }
                return props;
            }
            public List<UProperty> GetAllProperties()
            {
                var props = new List<UProperty>();

                if (BaseAddress != IntPtr.Zero)
                {
                    UClass inter = Interface;
                    while (inter.Interface.BaseAddress != IntPtr.Zero)
                    {
                        var tmpprops = inter.GetProperties();
                        tmpprops.AddRange(props);
                        props = tmpprops;
                        inter = inter.Interface;
                    }



                    props.AddRange(GetProperties());
                }
                return props;
            }
        }
    }
    public class UObject
    {
        public IntPtr BaseAddress { get; set; }

        public UObject(IntPtr addr)
        {
            BaseAddress = addr;
        }

        public UClass Class => new UClass(RIntPtr(BaseAddress + 0x10));
        public string Name => GetNameFromNameOffset(RInt32(BaseAddress + 0x18));
        public UObject Outer => new UObject(RIntPtr(BaseAddress + 0x20));






        /*public IntPtr GetPtrTo(string propertyName)
        {
            int offset = Class.GetPropertyOffset(propertyName);
            if (offset < 0)
                return IntPtr.Zero;
            return BaseAddress + offset;
        }*/


        public string Path()
        {
            //TODO:  Add path separaters
            void ParsePath(UObject obj, StringBuilder build)
            {
                if (obj.Outer.BaseAddress != IntPtr.Zero)
                {
                    ParsePath(obj.Outer, build);
                }
                build.Append(obj.Name);


            }
            var sb = new StringBuilder();
            ParsePath(this, sb);
            return sb.ToString();
        }
    }
    public class UFunction : UObject
    {
        public UFunction(IntPtr addr) : base(addr) { }
        public IntPtr FunctionPtr => RIntPtr(BaseAddress + 0xb0);
    }
    public class UProperty : UObject
    {
        public UProperty(IntPtr addr) : base(addr) { }
        public string Type => Class.Name;
        public UProperty nextProp => new UProperty(RIntPtr(BaseAddress + 0x28));
        public int ArrayDim => RInt32(BaseAddress + 0x30);
        public int ElementSize => RInt32(BaseAddress + 0x34);
        public uint PropertyFlags => RUInt32(BaseAddress + 0x38);
        //public UProperty nextProp => new UProperty(RIntPtr(BaseAddress + 0x58));

        public UClass innerClass => new UClass(RIntPtr(BaseAddress + 0x78));
        public int Offset => RInt32(BaseAddress + 0x50);
    }
}


