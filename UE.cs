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
        /*
        Class                                    0x250 : Struct   0x88 : Field 0x30 : Object 0x28
        ScriptStruct                              0x98 : Struct   0x88 : Field 0x30 : Object 0x28

        ArrayProperty                             0x80 : Property 0x78 : Field 0x30 : Object 0x28 
        IntProperty    0x78 : NumericProperty     0x78 : Property 0x78 : Field 0x30 : Object 0x28
        NameProperty                              0x78 : Property 0x78 : Field 0x30 : Object 0x28
        StructProperty                            0x80 : Property 0x78 : Field 0x30 : Object 0x28
        TextProperty                              0x78 : Property 0x78 : Field 0x30 : Object 0x28
        */







        
        public class FText
        {
            public IntPtr BaseAddress { get; }

            public FText(IntPtr baseAddress)
            {
                BaseAddress = baseAddress;
            }
            public int Length
            {
                get
                {
                    return RInt32(RIntPtr(RIntPtr(BaseAddress) + 0x8) + 0x8);
                }
                set
                {
                    WInt32(RIntPtr(RIntPtr(BaseAddress) + 0x8) + 0x8, value);
                }
            }
            public int MaxLength
            {
                get
                {
                    return RInt32(RIntPtr(RIntPtr(BaseAddress) + 0x8) + 0xc);
                }
                set
                {
                    WInt32(RIntPtr(RIntPtr(BaseAddress) + 0x8) + 0xc, value);
                }
            }

            public string Value
            {
                get
                {
                    if (BaseAddress == IntPtr.Zero)
                        return string.Empty;
                    IntPtr strPtr = RIntPtr(BaseAddress);
                    strPtr = RIntPtr(strPtr + 0x8);
                    strPtr = RIntPtr(strPtr);
                    return RUnicodeStr(strPtr);
                }
                set
                {
                    if (BaseAddress != IntPtr.Zero)
                    {
                        IntPtr newPtr = newString(value);
                        IntPtr strPtr = RIntPtr(BaseAddress);

                        if (newPtr != IntPtr.Zero)
                        {
                            strPtr = RIntPtr(strPtr + 0x8);
                            WIntPtr(strPtr, newPtr);
                            Length = value.Length + 1;
                            MaxLength = value.Length + 1;
                        }
                    }
                }
            }
        }
        public class UArrayProperty : UProperty
        {
            public UArrayProperty(IntPtr baseAddress) : base(baseAddress) { }
            public UStruct inner => new UStruct(RIntPtr(BaseAddress + 0x78));

        }
        public class UClass : UStruct
        {
            public UClass(IntPtr addr) : base(addr) { }
            
            
            public UObject ClassDefaultObject => new UObject(RIntPtr(BaseAddress + 0x100));

        }
        public class UObject
        {
            public IntPtr BaseAddress { get; set; }

            public UObject(IntPtr addr)
            {
                BaseAddress = addr;
            }
            public int objId => RInt32(BaseAddress + 0xc);
            public UClass Class => new UClass(RIntPtr(BaseAddress + 0x10));
            public string Name => GetNameFromNameOffset(RInt32(BaseAddress + 0x18));
            public UObject Outer
            {
                get => new UObject(RIntPtr(BaseAddress + 0x20));
                set => WIntPtr(BaseAddress + 0x20, value.BaseAddress);
            } 

            public string Type => Class.Name;




            /*public IntPtr GetPtrTo(string propertyName)
            {
                int offset = Class.GetPropertyOffset(propertyName);
                if (offset < 0)
                    return IntPtr.Zero;
                return BaseAddress + offset;
            }*/


            public string Path()
            {
                void ParsePath(UObject obj, StringBuilder build)
                {
                    if (obj.Outer.BaseAddress != IntPtr.Zero)
                    {
                        ParsePath(obj.Outer, build);
                    }

                    if (build.Length > 0)
                        build.Append('.');

                    build.Append(obj.Name);
                }

                var sb = new StringBuilder();
                ParsePath(this, sb);
                return sb.ToString();
            }
        }
        public class UObjectProperty : UProperty
        {
            public UObjectProperty(IntPtr addr) : base(addr) { }
        }
        public class UField : UObject
        {
            public UField(IntPtr addr) : base(addr) { }
            public UField nextField => new UField(RIntPtr(BaseAddress + 0x28));
        }
        public class UFunction : UObject
        {
            public UFunction(IntPtr addr) : base(addr) { }
            public IntPtr FunctionPtr => RIntPtr(BaseAddress + 0xb0);
        }
        public class UProperty : UField
        {
            public UProperty(IntPtr addr) : base(addr) { }
            
            public UProperty nextProp => new UProperty(RIntPtr(BaseAddress + 0x28));
            public int ArrayDim => RInt32(BaseAddress + 0x30);
            public int ElementSize => RInt32(BaseAddress + 0x34);
            public uint PropertyFlags => RUInt32(BaseAddress + 0x38);
            public int Offset => RInt32(BaseAddress + 0x50);
        }
        public class UStructProperty : UProperty
        {
            public UStructProperty(IntPtr addr) : base(addr) { }
            public UStruct unkField => new UStruct(RIntPtr(BaseAddress + 0x68));
            public UStruct innerStruct => new UStruct(RIntPtr(BaseAddress + 0x78));
        }
        public class UScriptStruct : UStruct
        {
            public UScriptStruct(IntPtr addr) : base(addr) { }
        }
        public class UStruct : UField
        {
            public UStruct(IntPtr addr) : base(addr) { }
            public UClass BaseClass => new UClass(RIntPtr(BaseAddress + 0x30));
            public UClass InterfaceOut => new UClass(RIntPtr(BaseAddress + 0x38));
            public int Size => RInt32(BaseAddress + 0x40);
            public UProperty firstField => new UProperty(RIntPtr(BaseAddress + 0x60));
            public UProperty unkField => new UProperty(RIntPtr(BaseAddress + 0x68));

            public UClass innerField => new UClass(RIntPtr(BaseAddress + 0x78));



            public List<(int Offset, string Name, UField Field)> GetProperties()
            {
                var fields = new List<(int Offset, string Name, UField Field)>() { };

                if (BaseAddress != IntPtr.Zero)
                {
                    //TODO:  Figure out this handling right
                    fields.AddRange(GetProperties(innerField));
                    fields.AddRange(GetProperties(firstField));
                    fields.AddRange(GetProperties(InterfaceOut));
                    //fields.AddRange(GetProperties(unkField));
                }
                return fields;
            }
            public static List<(int Offset, string Name, UField Field)> GetProperties(UField field)
            {
                var fields = new List<(int Offset, string Name, UField Field)>() { };

                while (field.BaseAddress != IntPtr.Zero)
                {
                    
                    switch (field.Type)
                    {
                        case "ArrayProperty":
                        case "AssetClassProperty":
                        case "AssetObjectProperty":
                        case "BoolProperty":
                        case "ByteProperty":
                        case "ClassProperty":
                        case "DelegateFunction":
                        case "DelegateProperty":
                        case "DoubleProperty":
                        case "Int8Property":
                        case "IntProperty":
                        case "InterfaceProperty":
                        case "FloatProperty":
                        case "Function":
                        case "LazyObjectProperty":
                        case "MapProperty":
                        case "MulticastDelegateProperty":
                        case "NameProperty":
                        case "ObjectProperty":
                        case "SetProperty":
                        case "StrProperty":
                        case "TextProperty":
                        case "UInt16Property":
                        case "UInt32Property":
                        case "UInt64Property":
                        case "WeakObjectProperty":
                            UProperty prop = new UProperty(field.BaseAddress);
                            fields.Add((prop.Offset, prop.Name, prop));
                            break;
                        case "StructProperty":
                            UStructProperty uStructProperty = new UStructProperty(field.BaseAddress);
                            fields.Add((uStructProperty.Offset, uStructProperty.Name, uStructProperty));

                            List<(int Offset, string Name, UField Field)> innerFields = new List<(int Offset, string Name, UField Field)>();
                            int baseOffset = 0;
                            string baseName = "";
                            baseOffset = uStructProperty.Offset;
                            baseName = uStructProperty.Name;
                            innerFields = GetProperties(uStructProperty.innerStruct.InterfaceOut);
                            for (int i = 0; i < innerFields.Count; i++)
                            {
                                var inner = innerFields[i];
                                int offset = inner.Offset;
                                offset += baseOffset;
                                string name = $"{baseName}.{inner.Name}";
                                //string name = $"    {inner.Name}";
                                fields.Add((offset, name, inner.Field));
                            }
                            break;
                        default:
                            fields.Add((0, $"(fix) {field.Name}", field));
                            break;
                    }

                    

                    field = field.nextField;
                }
                return fields;
            }

            public List<(int Offset, string Name, UField Field)> GetAllProperties()
            {
                var props = new List<(int Offset, string Name, UField Prop)> { };

                if (BaseAddress != IntPtr.Zero)
                {
                    UClass inter = BaseClass;
                    while (inter.BaseClass.BaseAddress != IntPtr.Zero)
                    {
                        var tmpprops = inter.GetProperties();
                        tmpprops.AddRange(props);
                        props = tmpprops;
                        inter = inter.BaseClass;
                    }
                    props.AddRange(GetProperties());
                }
                return props;
            }
        }
    }
}


