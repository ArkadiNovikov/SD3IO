using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
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
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public ushort[] name;
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
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 258)] private byte[] unknown1;
        public CharacterStatus character1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 112)] private byte[] unknown2;
        public CharacterStatus character2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 112)] private byte[] unknown3;
        public CharacterStatus character3;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 110)] private byte[] unknown4;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public ushort[] characterName1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public ushort[] characterName2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public ushort[] characterName3;
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

        public abstract Root Read(string pathString);
        public abstract Task<Root> ReadAsync(string pathString);
        public abstract void Write(String path, Root data);
        public abstract Task WriteAsync(String path, Root data);
    }

    public class SD3IO : IIO
    {
        public Root Read(String pathString)
        {
            var task = ReadAsync(pathString);
            task.Wait();
            return task.Result;
        }

        public async Task<Root> ReadAsync(String pathString)
        {
            if (!File.Exists(pathString))
            {
                return new Root();
            }

            var saveDataByteArray = await File.ReadAllBytesAsync(pathString);
            if (saveDataByteArray.Length != 8192)
            {
                return new Root();
            }

            GCHandle? handle = null;
            try
            {
                handle = GCHandle.Alloc(saveDataByteArray.ToArray(), GCHandleType.Pinned);
                return Marshal.PtrToStructure<Root>(handle.Value.AddrOfPinnedObject());
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
            var task = WriteAsync(path, data);
            task.Wait();
            if (task.IsFaulted)
            {
                throw task.Exception;
            }
        }
        public async Task WriteAsync(String path, Root data)
        {
            byte[] saveDataByteArray = new byte[Marshal.SizeOf(data)];
            if (saveDataByteArray.Length != 8192)
            {
                return;
            }

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

            await File.WriteAllBytesAsync(path, saveDataByteArray);
        }

        public string NameByteArrayToString(ushort[] name)
        {
            var nameStr = "";
            foreach (var item in name)
            {
                if (Data.nameStringTable.Length >= item)
                {
                    nameStr += Data.nameStringTable[item];
                }
                else
                {
                    nameStr += Data.nameStringTable[Data.nameStringTable.Length - 1];
                }
            }

            return nameStr;
        }

        public ushort[] NameStringToByteArray([DisallowNull] string nameStr)
        {
            return null;
        }
    }

    public static class Data
    {
        public static readonly sbyte[] EXISTSTRING = new sbyte[] { 101, 120, 105, 115, 116, 32, 32, 32 };

        public static readonly char[] nameStringTable = {
            '　', 'ー', '？', 'あ', 'い', 'う', 'え', 'お', 'か', 'き',
            'く', 'け', 'こ', 'さ', 'し', 'す', 'せ', 'そ', 'た', 'ち',
            'つ', 'て', 'と', 'な', 'に', 'ぬ', 'ね', 'の', 'は', 'ひ',
            'ふ', 'へ', 'ほ', 'ま', 'み', 'む', 'め', 'も', 'や', 'ゆ',
            'よ', 'ら', 'り', 'る', 'れ', 'ろ', 'わ', 'を', 'ん', 'っ',
            'ゃ', 'ゅ', 'ょ', 'ぁ', 'ぃ', 'ぅ', 'ぇ', 'ぉ', 'ッ', 'ャ',
            'ュ', 'ョ', 'ァ', 'ィ', 'ゥ', 'ェ', 'ォ', 'ア', 'イ', 'ウ',
            'エ', 'オ', 'カ', 'キ', 'ク', 'ケ', 'コ', 'サ', 'シ', 'ス',
            'セ', 'ソ', 'タ', 'チ', 'ツ', 'テ', 'ト', 'ナ', 'ニ', 'ヌ',
            'ネ', 'ノ', 'ハ', 'ヒ', 'フ', 'ヘ', 'ホ', 'マ', 'ミ', 'ム',
            'メ', 'モ', 'ヤ', 'ユ', 'ヨ', 'ラ', 'リ', 'ル', 'レ', 'ロ',
            'ワ', 'ヲ', 'ン', 'が', 'ぎ', 'ぐ', 'げ', 'ご', 'ざ', 'じ',
            'ず', 'ぜ', 'ぞ', 'だ', 'ぢ', 'づ', 'で', 'ど', 'ば', 'び',
            'ぶ', 'べ', 'ぼ', 'ガ', 'ギ', 'グ', 'ゲ', 'ゴ', 'ザ', 'ジ',
            'ズ', 'ゼ', 'ゾ', 'ダ', 'ヂ', 'ヅ', 'デ', 'ド', 'バ', 'ビ',
            'ブ', 'ベ', 'ボ', 'ヴ', 'ぱ', 'ぴ', 'ぷ', 'ぺ', 'ぽ', 'パ',
            'ピ', 'プ', 'ペ', 'ポ', '～', '…', '・', '！', '？'
        };

        public static readonly string[] seven = { "マナの祝日", "ルナの日", "サラマンダーの日", "ウンディーネの日", "ドリアードの日", "ジンの日", "ノームの日" };

        public static void GetData<T>(ref T value) where T : JSON.JsonObject
        {
            var assembly = Assembly.GetExecutingAssembly();

            var path = assembly.GetName().Name;
            switch (value)
            {
                case JSON.Items:
                    path += ".data.items.json";
                    break;
                case JSON.Skills:
                    path += ".data.skills.json";
                    break;
                case JSON.Characters:
                    path += ".data.characters.json";
                    break;
                case JSON.Weapons:
                    path += ".data.weapons.json";
                    break;
                case JSON.Armors:
                    path += ".data.armors.json";
                    break;
            }

            var stream = assembly.GetManifestResourceStream(path);
            value = Utf8Json.JsonSerializer.Deserialize<T>(stream);
        }
    }

    namespace JSON
    {
        public interface JsonObject { }


        public class Items : JsonObject
        {
            public Item[] items { get; set; }
        }

        public class Item
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Skills : JsonObject
        {
            public Skill[] skills { get; set; }
        }

        public class Skill
        {
            public int id { get; set; }
            public string name { get; set; }
        }


        public class Characters : JsonObject
        {
            public Character[] characters { get; set; }
        }

        public class Character
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Weapons : JsonObject
        {
            public Weapon[] weapons { get; set; }
        }

        public class Weapon
        {
            public int id { get; set; }
            public string name { get; set; }
            public int capablyCharacter { get; set; }
            public int capablyClass { get; set; }
        }

        public class Armors : JsonObject
        {
            public Armor[] armors { get; set; }
        }

        public class Armor
        {
            public int id { get; set; }
            public string name { get; set; }
            public int capablyCharacter { get; set; }
            public int capablyClass { get; set; }
        }
    }
}
