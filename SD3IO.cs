using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SD3IO
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SaveHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public sbyte[] existsString;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public byte[] unknown1;
        public CharacterHeader character1;
        public sbyte music;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public byte[] unknown2;
        public CharacterHeader character2;
        public sbyte location;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public byte[] unknown3;
        public CharacterHeader character3;
        public int timePlayed;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CharacterHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)] public sbyte[] name;
        public sbyte level;
        public short currentHP;
        public short maxHP;
        public short currentMP;
        public short maxMP;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public byte[] unknown;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CharacterStatus
    {
        public short statusHPCurrent;
        public short statusMPCurrent;
        public sbyte statusStr;
        public sbyte statusAgi;
        public sbyte statusVit;
        public sbyte statusInt;
        public sbyte statusSpr;
        public sbyte statusLuk;
        public short exp;
        public short remainderTotalExp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)] public byte[] unknown1;
        public sbyte techGaugeMax;
        public short expForNextLevel;
        public short remainderExpForNextLevel;
        public sbyte level;
        public byte unknown2;
        public sbyte skin;
        public sbyte classChangeMade;
        public sbyte clss;
        public sbyte eqWeapon;
        public sbyte eqHelm;
        public sbyte eqArmor;
        public sbyte eqGut;
        public sbyte eqShield;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)] public byte[] unknown3;
        public short atk;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)] public byte[] unknown4;
        public sbyte evadeRate;
        public short def;
        public short mdef;
        public short unknownPower;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public sbyte[] extraWeapons;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)] public byte[] unknown5;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public sbyte[] extraArmor;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)] public byte[] unknown6;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public sbyte[] extraGaut;
        public byte unknown7;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public sbyte[] extraShield;
        public byte unknown8;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)] public sbyte[] spells;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)] public sbyte[] tgt;
        public short statusHPMax;
        public short statusMPMax;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SaveData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 258)] public byte[] unknown1;
        public CharacterStatus character1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 112)] public byte[] unknown2;
        public CharacterStatus character2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 112)] public byte[] unknown3;
        public CharacterStatus character3;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 110)] public byte[] unknown4;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)] public sbyte[] characterName1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)] public sbyte[] characterName2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)] public sbyte[] characterName3;
        public ushort currency;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 529)] public byte[] unknown5;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 103)] public sbyte[] storage;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 27)] public byte[] unknown6;
        public short location;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 46)] public byte[] unknown7;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)] public sbyte[] ringItem;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 158)] public byte[] unknown8;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi, Size = 2048)]
    public struct SaveSlot
    {
        public SaveHeader header;
        public SaveData data;
        //public ushort checksum;
        public readonly ushort CheckSum
        {
            get
            {
                GCHandle? headerHandle = null, dataHandle = null;
                try
                {
                    int headerSize = Marshal.SizeOf(header);
                    int dataSize = Marshal.SizeOf(data);
                    byte[] byteHeader = new byte[headerSize];
                    byte[] byteData = new byte[dataSize];

                    headerHandle = GCHandle.Alloc(byteHeader, GCHandleType.Pinned);
                    dataHandle = GCHandle.Alloc(byteData, GCHandleType.Pinned);

                    Marshal.StructureToPtr<SaveHeader>(header, headerHandle.Value.AddrOfPinnedObject(), false);
                    Marshal.StructureToPtr<SaveData>(data, dataHandle.Value.AddrOfPinnedObject(), false);

                    var checksum = byteHeader.Skip(128).Concat(byteData).Sum(x => x);
                    return (ushort)checksum;
                }
                finally
                {
                    if (dataHandle?.IsAllocated ?? false)
                    {
                        dataHandle?.Free();
                    }
                    if (headerHandle?.IsAllocated ?? false)
                    {
                        headerHandle?.Free();
                    }
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct Root
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public SaveSlot[] slots;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)] public byte[] unknown;
    }

    public interface IIO
    {
        public static readonly sbyte[] EXISTSTRING = new sbyte[] { 101, 120, 105, 115, 116, 32, 32, 32 };

        public abstract bool Read(string pathString, out Root result);
        public abstract void Write(String path, Root data);
    }

    public class SD3IO : IIO
    {
        public bool Read(String pathString, out Root result)
        {
            if (!File.Exists(pathString))
            {
                result = new Root();
                return false;
            }

            ReadOnlySpan<byte> saveDataByteArray = File.ReadAllBytes(pathString);
            if (saveDataByteArray.Length != 8192)
            {
                result = new Root();
                return false;
            }

            GCHandle? handle = null;
            try
            {
                handle = GCHandle.Alloc(saveDataByteArray.ToArray(), GCHandleType.Pinned);
                result = Marshal.PtrToStructure<Root>(handle.Value.AddrOfPinnedObject());
            }
            finally
            {
                if (handle?.IsAllocated ?? false)
                {
                    handle?.Free();
                }
            }
            return true;
        }

        public async Task ReadAsync(String pathString)
        {
            if (!File.Exists(pathString))
            {

            }

            var saveDataByteArray = await File.ReadAllBytesAsync(pathString);
            if (saveDataByteArray.Length != 8192)
            {
            }

            GCHandle? handle = null;
            try
            {
                handle = GCHandle.Alloc(saveDataByteArray.ToArray(), GCHandleType.Pinned);
                var result = Marshal.PtrToStructure<Root>(handle.Value.AddrOfPinnedObject());
            }
            finally
            {
                if (handle?.IsAllocated ?? false)
                {
                    handle?.Free();
                }
            }
        }

        public void Write(String path, Root data)
        {
            byte[] saveDataByteArray = new byte[Marshal.SizeOf(data)];
            GCHandle? handle = null;
            try
            {
                handle = GCHandle.Alloc(saveDataByteArray, GCHandleType.Pinned);
                Marshal.StructureToPtr<Root>(data, handle.Value.AddrOfPinnedObject(), false);
            }
            finally
            {
                if (handle?.IsAllocated ?? false)
                {
                    handle?.Free();
                }
            }

            File.WriteAllBytes(path, saveDataByteArray);
        }
    }
}
