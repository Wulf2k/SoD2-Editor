using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SoD2_Editor
{
    public partial class Form1
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer, int dwSize, IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
            [In] byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress,
            uint dwSize, uint flNewProtect, out uint lpflOldProtect);

        private static IntPtr _proc;
        private static bool _bigEndian = false;

        public static byte[] RBytes(IntPtr addr, int sizeToRead)
        {
            byte[] buff = new byte[sizeToRead];
            ReadProcessMemory(_proc, addr, buff, buff.Length, IntPtr.Zero);
            return buff;
        }

        public static string RAsciiStr(IntPtr addr, int length = 0x200)
        {
            var bytes = RBytes(addr, length);
            return Encoding.ASCII.GetString(bytes).Split('\0')[0];
        }

        public static string RUnicodeStr(IntPtr addr)
        {
            return Encoding.Unicode.GetString(RBytes(addr, 0x400)).Split('\0')[0];
        }

        public static object ReadAndConvert(IntPtr addr, int byteCount, string convertType)
        {
            byte[] buff = RBytes(addr, byteCount);
            if (_bigEndian)
                Array.Reverse(buff);

            switch (convertType)
            {
                case "Int16": return BitConverter.ToInt16(buff, 0);
                case "Int32": return BitConverter.ToInt32(buff, 0);
                case "Int64": return BitConverter.ToInt64(buff, 0);
                case "UInt8": return buff[0];
                case "UInt16": return BitConverter.ToUInt16(buff, 0);
                case "UInt32": return BitConverter.ToUInt32(buff, 0);
                case "UInt64": return BitConverter.ToUInt64(buff, 0);
                case "Single": return BitConverter.ToSingle(buff, 0);
                default: throw new ArgumentException($"Unknown conversion type {convertType}");
            }
        }

        public static short RInt16(IntPtr addr) => (short)ReadAndConvert(addr, 2, "Int16");
        public static int RInt32(IntPtr addr) => (int)ReadAndConvert(addr, 4, "Int32");
        public static long RInt64(IntPtr addr) => (long)ReadAndConvert(addr, 8, "Int64");
        public static byte RUInt8(IntPtr addr) => (byte)ReadAndConvert(addr, 1, "UInt8");
        public static ushort RUInt16(IntPtr addr) => (ushort)ReadAndConvert(addr, 2, "UInt16");
        public static uint RUInt32(IntPtr addr) => (uint)ReadAndConvert(addr, 4, "UInt32");
        public static ulong RUInt64(IntPtr addr) => (ulong)ReadAndConvert(addr, 8, "UInt64");
        public static float RSingle(IntPtr addr) => (float)ReadAndConvert(addr, 4, "Single");
        public static IntPtr RIntPtr(IntPtr addr)
        {
            if (IntPtr.Size == 8)
                return (IntPtr)RInt64(addr);
            else
                return (IntPtr)RInt32(addr);
        }

        public static void WBytes(IntPtr addr, byte[] data)
        {
            int written;
            bool success = WriteProcessMemory(_proc, addr, data, data.Length, out written);
            if (!success || written != data.Length)
                //throw new Exception($"Failed to write memory at {addr}");
                Console.WriteLine($"Failed to write memory at {addr.ToString("X")}");
        }

        public static void WInt16(IntPtr addr, short val) => WBytes(addr, BitConverter.GetBytes(val));
        public static void WInt32(IntPtr addr, int val) => WBytes(addr, BitConverter.GetBytes(val));
        public static void WInt64(IntPtr addr, long val) => WBytes(addr, BitConverter.GetBytes(val));
        public static void WUInt8(IntPtr addr, byte val) => WBytes(addr, new byte[] { val });
        public static void WUInt16(IntPtr addr, ushort val) => WBytes(addr, BitConverter.GetBytes(val));
        public static void WUInt32(IntPtr addr, uint val) => WBytes(addr, BitConverter.GetBytes(val));
        public static void WUInt64(IntPtr addr, ulong val) => WBytes(addr, BitConverter.GetBytes(val));
        public static void WSingle(IntPtr addr, float val) => WBytes(addr, BitConverter.GetBytes(val));

        public static void WAsciiStr(IntPtr addr, string str, int maxLen = 0x50)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            if (bytes.Length >= maxLen)
                Array.Resize(ref bytes, maxLen - 1);
            byte[] padded = new byte[maxLen];
            Array.Copy(bytes, padded, bytes.Length);
            WBytes(addr, padded);
        }

        public static void WUnicodeStr(IntPtr addr, string str, int maxLen = 0x50)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            if (bytes.Length >= maxLen)
                Array.Resize(ref bytes, maxLen - 2);
            byte[] padded = new byte[maxLen];
            Array.Copy(bytes, padded, bytes.Length);
            WBytes(addr, padded);
        }

        public static string GetNameFromNameOffset(int offset)
        {
            IntPtr table = RIntPtr(addresses.Get("NamesTablePtr"));
            if (offset < 0x1000)
            {
                offset = RInt32((IntPtr)(_ba + 0x44da8e0 + (offset * 4)));
            }
            string name = RAsciiStr((IntPtr)(table.ToInt64() + offset + 8));
            return name;
        }

        public static IntPtr GetObjFromObjId(int id)
        {
            IntPtr table = (IntPtr)RInt64(addresses.Get("ObjTablePtr"));
            IntPtr obj = (IntPtr)RInt64((IntPtr)(table.ToInt64() + (0x18 * id)));
            return obj;
        }
    }
}
