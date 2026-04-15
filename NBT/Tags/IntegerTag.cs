namespace NBT.Tags;

public class IntegerTag(int value) : INbtTag, IEquatable<IntegerTag> {
    public int Value { get; } = value;

    public byte GetPrefix() {
        return NbtTagPrefix.Integer;
    }
    
    public NbtBuilder Write(NbtBuilder builder, bool noType = false) {
        return builder.WriteType(GetPrefix(), noType).WriteInteger(Value);
    }

    public bool Equals(IntegerTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((IntegerTag)obj);
    }

    public override int GetHashCode() {
        return Value.GetHashCode();
    }

    public static bool operator ==(IntegerTag? left, IntegerTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(IntegerTag? left, IntegerTag? right) {
        return !(left == right);
    }
}
