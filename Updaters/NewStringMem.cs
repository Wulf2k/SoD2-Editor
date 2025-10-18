using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoD2_Editor
{
    public partial class Form1 : Form
    {
        public static IntPtr newStringsPtr = IntPtr.Zero;
        public static int newStringsOffset = 0;


        public static IntPtr newString(string text)
        {
            text += '\0';
            if (newStringsOffset + (text.Length + 2) * 2 > 0x10000)
                newStringsPtr = IntPtr.Zero;
            if (newStringsPtr == IntPtr.Zero)
            {
                newStringsPtr = Alloc(0x10000);
                newStringsOffset = 0;
            }

            int currOffset = newStringsOffset;
            WUnicodeStr(newStringsPtr + currOffset, text);
            newStringsOffset += text.Length * 2;
            return newStringsPtr + currOffset;
        }

    }
}
