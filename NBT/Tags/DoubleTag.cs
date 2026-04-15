namespace NBT.Tags;

public class DoubleTag(double value) : INbtTag, IEquatable<DoubleTag> {
    public double Value { get; } = value;

    public byte GetPrefix() {
        return NbtTagPrefix.Double;
    }
    
    public NbtBuilder Write(NbtBuilder builder, bool noType = false) {
        return builder.WriteType(GetPrefix(), noType).WriteDouble(Value);
    }

    public bool Equals(DoubleTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value.Equals(other.Value);
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((DoubleTag)obj);
    }

    public override int GetHashCode() {
        return Value.GetHashCode();
    }

    public static bool operator ==(DoubleTag? left, DoubleTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(DoubleTag? left, DoubleTag? right) {
        return !(left == right);
    }
}
