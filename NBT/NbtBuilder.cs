using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace NBT;

public class NbtBuilder {
    private byte[] _buffer;
    private int _position;
    private bool _usingArrayPool;
    private const int InitialCapacity = 256;
    private const int ArrayPoolThreshold = 1024; // Only use ArrayPool for buffers larger than this
    
    public NbtBuilder() {
        _buffer = new byte[InitialCapacity];
        _position = 0;
        _usingArrayPool = false;
    }
    
    public byte[] ToArray() {
        if (_position == _buffer.Length && !_usingArrayPool) {
            // Perfect size match, can return buffer directly if not from pool
            byte[] perfectMatch = _buffer;
            _buffer = Array.Empty<byte>();
            return perfectMatch;
        }
        
        byte[] result = new byte[_position];
        Array.Copy(_buffer, 0, result, 0, _position);
        
        if (_usingArrayPool) {
            ArrayPool<byte>.Shared.Return(_buffer);
        }
        
        _buffer = Array.Empty<byte>();
        return result;
    }
    
    private void EnsureCapacity(int additionalBytes) {
        int requiredCapacity = _position + additionalBytes;
        if (requiredCapacity <= _buffer.Length) {
            return;
        }
        
        int newCapacity = Math.Max(_buffer.Length * 2, requiredCapacity);
        byte[] newBuffer;
        
        // Use ArrayPool only for large buffers
        if (newCapacity >= ArrayPoolThreshold && !_usingArrayPool) {
            newBuffer = ArrayPool<byte>.Shared.Rent(newCapacity);
            Array.Copy(_buffer, 0, newBuffer, 0, _position);
            _usingArrayPool = true;
        } else if (_usingArrayPool) {
            newBuffer = ArrayPool<byte>.Shared.Rent(newCapacity);
            Array.Copy(_buffer, 0, newBuffer, 0, _position);
            ArrayPool<byte>.Shared.Return(_buffer);
        } else {
            newBuffer = new byte[newCapacity];
            Buffer.BlockCopy(_buffer, 0, newBuffer, 0, _position);
        }
        
        _buffer = newBuffer;
    }

    public NbtBuilder WriteDouble(double value) {
        EnsureCapacity(sizeof(double));
        BinaryPrimitives.WriteDoubleBigEndian(_buffer.AsSpan(_position, sizeof(double)), value);
        _position += sizeof(double);
        return this;
    }

    public NbtBuilder WriteFloat(float value) {
        EnsureCapacity(sizeof(float));
        BinaryPrimitives.WriteSingleBigEndian(_buffer.AsSpan(_position, sizeof(float)), value);
        _position += sizeof(float);
        return this;
    }

    public NbtBuilder Write(byte[] value) {
        EnsureCapacity(value.Length);
        Array.Copy(value, 0, _buffer, _position, value.Length);
        _position += value.Length;
        return this;
    }

    public NbtBuilder Write(ReadOnlySpan<byte> value) {
        EnsureCapacity(value.Length);
        value.CopyTo(_buffer.AsSpan(_position));
        _position += value.Length;
        return this;
    }

    public NbtBuilder Write(Span<byte> value) {
        return Write((ReadOnlySpan<byte>)value);
    }
    
    public NbtBuilder Write(byte value) {
        EnsureCapacity(1);
        _buffer[_position++] = value;
        return this;
    }

    public NbtBuilder WriteType(byte value, bool noType) {
        if (!noType) Write(value);
        return this;
    }

    public NbtBuilder WriteName(string? name) {
        if (name != null) WriteString(name);
        return this;
    }

    public NbtBuilder Write(INbtTag tag) {
        return Write(tag.Serialise());
    }

    public NbtBuilder WriteString(string value) {
        int length = Encoding.UTF8.GetByteCount(value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(length, ushort.MaxValue);

        EnsureCapacity(sizeof(ushort) + length);
        
        // Write length
        BinaryPrimitives.WriteUInt16BigEndian(_buffer.AsSpan(_position, sizeof(ushort)), (ushort)length);
        _position += sizeof(ushort);
        
        // Write string bytes directly to buffer
        int bytesWritten = Encoding.UTF8.GetBytes(value, _buffer.AsSpan(_position));
        _position += bytesWritten;
        
        return this;
    }

    public NbtBuilder WriteInteger(int value) {
        EnsureCapacity(sizeof(int));
        BinaryPrimitives.WriteInt32BigEndian(_buffer.AsSpan(_position, sizeof(int)), value);
        _position += sizeof(int);
        return this;
    }
    
    public NbtBuilder WriteLong(long value) {
        EnsureCapacity(sizeof(long));
        BinaryPrimitives.WriteInt64BigEndian(_buffer.AsSpan(_position, sizeof(long)), value);
        _position += sizeof(long);
        return this;
    }
    
    public NbtBuilder WriteShort(short value) {
        EnsureCapacity(sizeof(short));
        BinaryPrimitives.WriteInt16BigEndian(_buffer.AsSpan(_position, sizeof(short)), value);
        _position += sizeof(short);
        return this;
    }
    
    public NbtBuilder WriteByte(sbyte value) {
        EnsureCapacity(1);
        _buffer[_position++] = (byte)(value < 0 ? 256 + value : value);
        return this;
    }
}
