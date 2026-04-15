namespace NBT.Tags;

public class FloatTag(float value) : INbtTag, IEquatable<FloatTag> {
    public float Value { get; } = value;

    public byte GetPrefix() {
        return NbtTagPrefix.Float;
    }

    public NbtBuilder Write(NbtBuilder builder, bool noType = false) {
        return builder.WriteType(GetPrefix(), noType).WriteFloat(Value);
    }

    public bool Equals(FloatTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value.Equals(other.Value);
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((FloatTag)obj);
    }

    public override int GetHashCode() {
        return Value.GetHashCode();
    }

    public static bool operator ==(FloatTag? left, FloatTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(FloatTag? left, FloatTag? right) {
        return !(left == right);
    }
}
