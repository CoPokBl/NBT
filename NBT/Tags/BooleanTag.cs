namespace NBT.Tags;

public class BooleanTag(bool value) : ByteTag((sbyte)(value ? 0x01 : 0x00)), IEquatable<BooleanTag> {
    public new bool Value { get; } = value;

    public bool Equals(BooleanTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((BooleanTag)obj);
    }

    public override int GetHashCode() {
        return Value.GetHashCode();
    }

    public static bool operator ==(BooleanTag? left, BooleanTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(BooleanTag? left, BooleanTag? right) {
        return !(left == right);
    }
}
