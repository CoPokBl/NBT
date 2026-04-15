namespace NBT.Tags;

public class ShortTag(short value) : INbtTag, IEquatable<ShortTag> {
    public short Value { get; } = value;

    public byte GetPrefix() {
        return NbtTagPrefix.Short;
    }

    public NbtBuilder Write(NbtBuilder builder, bool noType = false) {
        return builder.WriteType(GetPrefix(), noType).WriteShort(Value);
    }

    public bool Equals(ShortTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ShortTag)obj);
    }

    public override int GetHashCode() {
        return Value.GetHashCode();
    }

    public static bool operator ==(ShortTag? left, ShortTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(ShortTag? left, ShortTag? right) {
        return !(left == right);
    }
}
