using System.Buffers.Binary;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using NBT.Tags;

namespace NBT;

public class NbtReader {
    private Stream? _input;
    private byte[]? _sourceData;
    private int _position;
    private readonly NbtCompressionType _compression;

    public NbtReader(Stream input, NbtCompressionType compression = NbtCompressionType.None) {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _compression = compression;
    }

    public NbtReader(byte[] data, NbtCompressionType compression = NbtCompressionType.None) {
        // For maximum speed, convert stream to byte array upfront
        _sourceData = data ?? throw new ArgumentNullException(nameof(data));
        _position = 0;
        _compression = compression;
    }

    /// <summary>
    /// Parses the current object's data into a tag.
    /// </summary>
    /// <param name="impliedRoot">
    /// Whether there should be an implied root compound tag.
    /// NOTE that using this will read the entire input stream, so it should only be used
    /// when the input stream is a single NBT tag (or the remaining is a single NBT tag).
    /// </param>
    /// <returns>The parsed tag.</returns>
    /// <remarks><see cref="impliedRoot"/> is mainly used for NBT files.</remarks>
    /// <exception cref="InvalidDataException">When the data contains invalid information.</exception>
    public INbtTag ToTag(bool impliedRoot = false) {
        switch (_compression) {
            case NbtCompressionType.GZip:
                throw new NotImplementedException();
            
            case NbtCompressionType.ZLib:
                // decompress the input stream with CompressionHelper.DecompressZlib
                if (_sourceData != null) {
                    _input = DecompressZlib(new MemoryStream(_sourceData));
                    _sourceData = null; // Switch to stream mode
                } else if (_input != null) {
                    _input = DecompressZlib(_input);
                } else {
                    throw new InvalidOperationException("No input data available for decompression");
                }
                break;
            
            case NbtCompressionType.None:
                break;  // do nothing
            
            default:
                throw new ArgumentOutOfRangeException(nameof(_compression), _compression, null);
        }
        
        if (impliedRoot) {
            if (_sourceData != null) {
                // For byte array source, create modified array
                byte[] newData = new byte[_sourceData.Length - _position + 2];
                newData[0] = NbtTagPrefix.Compound;
                Array.Copy(_sourceData, _position, newData, 1, _sourceData.Length - _position);
                newData[^1] = NbtTagPrefix.End;
                _sourceData = newData;
                _position = 0;
            } else if (_input != null) {
                MemoryStream ms = new();
                ms.WriteByte(NbtTagPrefix.Compound);
                _input.CopyTo(ms);
                ms.WriteByte(NbtTagPrefix.End);
                ms.Position = 0;
                _input = ms;
            } else {
                throw new InvalidOperationException("No input data available for implied root processing");
            }
        }
        
        // Start by reading the tag type
        byte type = Read();

        // At the root level no tag will have a name
        return type switch {
            NbtTagPrefix.String => new StringTag(null, ReadString()),
            NbtTagPrefix.Byte => new ByteTag(null, ReadByte()),
            NbtTagPrefix.Integer => new IntegerTag(null, ReadInteger()),
            NbtTagPrefix.Long => new LongTag(null, ReadLong()),
            NbtTagPrefix.Short => new ShortTag(null, ReadShort()),
            NbtTagPrefix.Float => new FloatTag(null, ReadFloat()),
            NbtTagPrefix.Double => new DoubleTag(null, ReadDouble()),
            NbtTagPrefix.Compound => ReadCompoundTag(),
            NbtTagPrefix.End => new EmptyTag(),
            NbtTagPrefix.List => ReadList(),
            NbtTagPrefix.Integers => ReadArray(ReadInteger),
            NbtTagPrefix.Bytes => ReadArray(ReadByte),
            NbtTagPrefix.Longs => ReadArray(ReadLong),
            _ => throw new InvalidDataException($"Unknown type {type}")
        };
    }
    
    public static INbtTag ReadNbt(byte[] data, bool impliedRoot = false, NbtCompressionType compression = NbtCompressionType.None) {
        return new NbtReader(data, compression).ToTag(impliedRoot);
    }
    
    public static Stream DecompressZlib(Stream input) {
        ZLibStream zlibStream = new(input, CompressionMode.Decompress);
        // MemoryStream output = new();
        // zlibStream.CopyTo(output);
        return zlibStream;
    }


    // returns it strongly typed for convenience, except when it contains nested lists
    public ListTag ReadList() {
        byte type = Read();
        int length = ReadInteger();

        switch (type) {
            case NbtTagPrefix.End:  // an empty list
                return new ListTag(null, []);
            case NbtTagPrefix.String:
                StringTag[] stringTags = new StringTag[length];
                for (int i = 0; i < length; i++) stringTags[i] = new StringTag(null, ReadString());
                return new ListTag<StringTag>(null, stringTags);
            case NbtTagPrefix.Byte:
                ByteTag[] byteTags = new ByteTag[length];
                for (int i = 0; i < length; i++) byteTags[i] = new ByteTag(null, ReadByte());
                return new ListTag<ByteTag>(null, byteTags);
            case NbtTagPrefix.Integer:
                IntegerTag[] intTags = new IntegerTag[length];
                for (int i = 0; i < length; i++) intTags[i] = new IntegerTag(null, ReadInteger());
                return new ListTag<IntegerTag>(null, intTags);
            case NbtTagPrefix.Long:
                LongTag[] longTags = new LongTag[length];
                for (int i = 0; i < length; i++) longTags[i] = new LongTag(null, ReadLong());
                return new ListTag<LongTag>(null, longTags);
            case NbtTagPrefix.Short:
                ShortTag[] shortTags = new ShortTag[length];
                for (int i = 0; i < length; i++) shortTags[i] = new ShortTag(null, ReadShort());
                return new ListTag<ShortTag>(null, shortTags);
            case NbtTagPrefix.Float:
                FloatTag[] floatTags = new FloatTag[length];
                for (int i = 0; i < length; i++) floatTags[i] = new FloatTag(null, ReadFloat());
                return new ListTag<FloatTag>(null, floatTags);
            case NbtTagPrefix.Double:
                DoubleTag[] doubleTags = new DoubleTag[length];
                for (int i = 0; i < length; i++) doubleTags[i] = new DoubleTag(null, ReadDouble());
                return new ListTag<DoubleTag>(null, doubleTags);
            case NbtTagPrefix.List:
                ListTag[] listTags = new ListTag[length];
                for (int i = 0; i < length; i++) listTags[i] = ReadList();
                return new ListTag<ListTag>(null, listTags);
            case NbtTagPrefix.Integers:
                ArrayTag<int>[] intArrayTags = new ArrayTag<int>[length];
                for (int i = 0; i < length; i++) intArrayTags[i] = ReadArray(ReadInteger);
                return new ListTag<ArrayTag<int>>(null, intArrayTags);
            case NbtTagPrefix.Bytes:
                ArrayTag<sbyte>[] byteArrayTags = new ArrayTag<sbyte>[length];
                for (int i = 0; i < length; i++) byteArrayTags[i] = ReadArray(ReadByte);
                return new ListTag<ArrayTag<sbyte>>(null, byteArrayTags);
            case NbtTagPrefix.Longs:
                ArrayTag<long>[] longArrayTags = new ArrayTag<long>[length];
                for (int i = 0; i < length; i++) longArrayTags[i] = ReadArray(ReadLong);
                return new ListTag<ArrayTag<long>>(null, longArrayTags);
            case NbtTagPrefix.Compound:
                CompoundTag[] compoundTags = new CompoundTag[length];
                for (int i = 0; i < length; i++) compoundTags[i] = ReadCompoundTag();
                return new ListTag<CompoundTag>(null, compoundTags);
        }
        
        throw new InvalidDataException($"Unknown type {type}");
    }

    public ArrayTag<T> ReadArray<T>(Func<T> readFunc) {
        int length = ReadInteger();
        T[] vals = new T[length];
        for (int i = 0; i < length; i++) {
            vals[i] = readFunc();
        }
        return new ArrayTag<T>(null, vals);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte Read() {
        if (_sourceData != null) {
            return _sourceData[_position++];
        }
        
        if (_input == null) {
            throw new InvalidOperationException("No input data available");
        }
        
        int b = _input.ReadByte();
        if (b == -1) {
            throw new EndOfStreamException("Reached end of stream while reading NBT data.");
        }
        return (byte)b;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadIntoSpan(Span<byte> destination) {
        if (_sourceData != null) {
            _sourceData.AsSpan(_position, destination.Length).CopyTo(destination);
            _position += destination.Length;
            return;
        }
        
        if (_input == null) {
            throw new InvalidOperationException("No input data available");
        }
        
        int bytesRead = _input.Read(destination);
        if (bytesRead < destination.Length) {
            throw new EndOfStreamException($"Expected to read {destination.Length} bytes, but only read {bytesRead} bytes.");
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte[] Read(int count) {
        if (_sourceData != null) {
            byte[] result = new byte[count];
            Buffer.BlockCopy(_sourceData, _position, result, 0, count);
            _position += count;
            return result;
        }
        
        if (_input == null) {
            throw new InvalidOperationException("No input data available");
        }
        
        byte[] buffer = new byte[count];
        int bytesRead = _input.Read(buffer, 0, count);
        if (bytesRead < count) {
            throw new EndOfStreamException($"Expected to read {count} bytes, but only read {bytesRead} bytes.");
        }
        return buffer;
    }
    
    public sbyte ReadByte() {
        byte b = Read();
        if (b >= 128) {
            // Convert to two's complement for negative values
            return (sbyte)(b - 256); // 256 - 128 = 128, so -128 becomes 128
        }
        return (sbyte)(b & 0xFF);
    }

    public CompoundTag ReadCompoundTag() {
        List<INbtTag> children = [];
        
        // each child is written, but with a name this time.
        while (true) {
            byte type = Read();
            if (type == NbtTagPrefix.End) {
                return new CompoundTag(null, children.ToArray());
            }
            
            string name = ReadString();
            
            switch (type) {
                case NbtTagPrefix.String:
                    children.Add(new StringTag(name, ReadString()));
                    break;
                case NbtTagPrefix.Byte:
                    children.Add(new ByteTag(name, ReadByte()));
                    break;
                case NbtTagPrefix.Integer:
                    children.Add(new IntegerTag(name, ReadInteger()));
                    break;
                case NbtTagPrefix.Long:
                    children.Add(new LongTag(name, ReadLong()));
                    break;
                case NbtTagPrefix.Short:
                    children.Add(new ShortTag(name, ReadShort()));
                    break;
                case NbtTagPrefix.Float:
                    children.Add(new FloatTag(name, ReadFloat()));
                    break;
                case NbtTagPrefix.Double:
                    children.Add(new DoubleTag(name, ReadDouble()));
                    break;
                case NbtTagPrefix.Compound:
                    children.Add(ReadCompoundTag().WithName(name));
                    break;
                case NbtTagPrefix.List:
                    children.Add(ReadList().WithName(name));
                    break;
                case NbtTagPrefix.Integers:
                    children.Add(ReadArray(ReadInteger).WithName(name));
                    break;
                case NbtTagPrefix.Bytes:
                    children.Add(ReadArray(ReadByte).WithName(name));
                    break;
                case NbtTagPrefix.Longs:
                    children.Add(ReadArray(ReadLong).WithName(name));
                    break;
                default:
                    throw new InvalidDataException($"Unknown tag type {type}");
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString() {
        ushort len;
        if (_sourceData != null) {
            len = BinaryPrimitives.ReadUInt16BigEndian(_sourceData.AsSpan(_position));
            _position += sizeof(ushort);
        } else {
            Span<byte> lenBytes = stackalloc byte[sizeof(ushort)];
            ReadIntoSpan(lenBytes);
            len = BinaryPrimitives.ReadUInt16BigEndian(lenBytes);
        }

        if (len == 0) {
            return string.Empty;
        }
        
        // Direct decode from source buffer when possible
        if (_sourceData != null) {
            string result = Encoding.UTF8.GetString(_sourceData.AsSpan(_position, len));
            _position += len;
            return result;
        }
        
        // Use stackalloc for small strings
        if (len <= 256) {
            Span<byte> textBytes = stackalloc byte[len];
            ReadIntoSpan(textBytes);
            return Encoding.UTF8.GetString(textBytes);
        } else {
            byte[] textBytes = System.Buffers.ArrayPool<byte>.Shared.Rent(len);
            try {
                ReadIntoSpan(textBytes.AsSpan(0, len));
                return Encoding.UTF8.GetString(textBytes, 0, len);
            } finally {
                System.Buffers.ArrayPool<byte>.Shared.Return(textBytes);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInteger() {
        if (_sourceData != null) {
            int result = BinaryPrimitives.ReadInt32BigEndian(_sourceData.AsSpan(_position));
            _position += sizeof(int);
            return result;
        }
        Span<byte> bytes = stackalloc byte[sizeof(int)];
        ReadIntoSpan(bytes);
        return BinaryPrimitives.ReadInt32BigEndian(bytes);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadLong() {
        if (_sourceData != null) {
            long result = BinaryPrimitives.ReadInt64BigEndian(_sourceData.AsSpan(_position));
            _position += sizeof(long);
            return result;
        }
        Span<byte> bytes = stackalloc byte[sizeof(long)];
        ReadIntoSpan(bytes);
        return BinaryPrimitives.ReadInt64BigEndian(bytes);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadShort() {
        if (_sourceData != null) {
            short result = BinaryPrimitives.ReadInt16BigEndian(_sourceData.AsSpan(_position));
            _position += sizeof(short);
            return result;
        }
        Span<byte> bytes = stackalloc byte[sizeof(short)];
        ReadIntoSpan(bytes);
        return BinaryPrimitives.ReadInt16BigEndian(bytes);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadFloat() {
        if (_sourceData != null) {
            float result = BinaryPrimitives.ReadSingleBigEndian(_sourceData.AsSpan(_position));
            _position += sizeof(float);
            return result;
        }
        Span<byte> bytes = stackalloc byte[sizeof(float)];
        ReadIntoSpan(bytes);
        return BinaryPrimitives.ReadSingleBigEndian(bytes);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ReadDouble() {
        if (_sourceData != null) {
            double result = BinaryPrimitives.ReadDoubleBigEndian(_sourceData.AsSpan(_position));
            _position += sizeof(double);
            return result;
        }
        Span<byte> bytes = stackalloc byte[sizeof(double)];
        ReadIntoSpan(bytes);
        return BinaryPrimitives.ReadDoubleBigEndian(bytes);
    }
}
