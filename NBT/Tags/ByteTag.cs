namespace NBT.Tags;

public class ByteTag(sbyte value) : INbtTag, IEquatable<ByteTag> {
    public sbyte Value { get; } = value;

    public bool BoolValue => Value != 0x00;

    public byte GetPrefix() {
        return NbtTagPrefix.Byte;
    }

    public NbtBuilder Write(NbtBuilder builder, bool noType = false) {
        return builder.WriteType(GetPrefix(), noType).WriteByte(Value);
    }

    public bool Equals(ByteTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ByteTag)obj);
    }

    public override int GetHashCode() {
        return Value.GetHashCode();
    }

    public static bool operator ==(ByteTag? left, ByteTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(ByteTag? left, ByteTag? right) {
        return !(left == right);
    }
}
