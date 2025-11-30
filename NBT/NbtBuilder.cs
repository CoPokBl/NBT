using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NBT;

public class NbtBuilder {
    private byte[] _buffer;
    private int _position;
    private bool _fromPool;
    
    private static readonly ArrayPool<byte> Pool = ArrayPool<byte>.Shared;
    
    public NbtBuilder() {
        // Use ArrayPool for maximum speed with reuse
        _buffer = Pool.Rent(2048);
        _position = 0;
        _fromPool = true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ToArray() {
        byte[] result = new byte[_position];
        Buffer.BlockCopy(_buffer, 0, result, 0, _position);
        
        // Return to pool for reuse
        if (_fromPool && _buffer != null!) {
            Pool.Return(_buffer);
            _buffer = null!;
            _fromPool = false;
        }
        
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureCapacity(int additionalBytes) {
        int requiredCapacity = _position + additionalBytes;
        if (requiredCapacity <= _buffer.Length) return;
        
        // Increase capacity
        int newCapacity = Math.Max(_buffer.Length * 2, requiredCapacity);
        byte[] newBuffer = _fromPool ? Pool.Rent(newCapacity) : new byte[newCapacity];
        Buffer.BlockCopy(_buffer, 0, newBuffer, 0, _position);
            
        if (_fromPool) {
            Pool.Return(_buffer);
        }
            
        _buffer = newBuffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NbtBuilder WriteDouble(double value) {
        EnsureCapacity(sizeof(double));
        BinaryPrimitives.WriteDoubleBigEndian(_buffer.AsSpan(_position), value);
        _position += sizeof(double);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NbtBuilder WriteFloat(float value) {
        EnsureCapacity(sizeof(float));
        BinaryPrimitives.WriteSingleBigEndian(_buffer.AsSpan(_position), value);
        _position += sizeof(float);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NbtBuilder Write(byte[] value) {
        EnsureCapacity(value.Length);
        Buffer.BlockCopy(value, 0, _buffer, _position, value.Length);
        _position += value.Length;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NbtBuilder Write(IEnumerable<byte> value) {
        if (value is byte[] arr) {
            return Write(arr);
        }
        foreach (byte b in value) {
            Write(b);
        }
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NbtBuilder Write(Span<byte> value) {
        EnsureCapacity(value.Length);
        value.CopyTo(_buffer.AsSpan(_position));
        _position += value.Length;
        return this;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NbtBuilder WriteString(string value) {
        int length = Encoding.UTF8.GetByteCount(value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(length, ushort.MaxValue);

        EnsureCapacity(sizeof(ushort) + length);
        
        // Write length directly
        BinaryPrimitives.WriteUInt16BigEndian(_buffer.AsSpan(_position), (ushort)length);
        _position += sizeof(ushort);
        
        // Write string bytes directly to buffer
        int bytesWritten = Encoding.UTF8.GetBytes(value, _buffer.AsSpan(_position));
        _position += bytesWritten;
        
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NbtBuilder WriteInteger(int value) {
        EnsureCapacity(sizeof(int));
        BinaryPrimitives.WriteInt32BigEndian(_buffer.AsSpan(_position), value);
        _position += sizeof(int);
        return this;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NbtBuilder WriteLong(long value) {
        EnsureCapacity(sizeof(long));
        BinaryPrimitives.WriteInt64BigEndian(_buffer.AsSpan(_position), value);
        _position += sizeof(long);
        return this;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NbtBuilder WriteShort(short value) {
        EnsureCapacity(sizeof(short));
        BinaryPrimitives.WriteInt16BigEndian(_buffer.AsSpan(_position), value);
        _position += sizeof(short);
        return this;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NbtBuilder WriteByte(sbyte value) {
        EnsureCapacity(1);
        _buffer[_position++] = (byte)(value < 0 ? 256 + value : value);
        return this;
    }
}
