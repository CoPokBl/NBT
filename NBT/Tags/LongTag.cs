namespace NBT.Tags;

public class LongTag(long value) : INbtTag, IEquatable<LongTag> {
    public long Value { get; } = value;

    public byte GetPrefix() {
        return NbtTagPrefix.Long;
    }

    public NbtBuilder Write(NbtBuilder builder, bool noType = false) {
        return builder.WriteType(GetPrefix(), noType).WriteLong(Value);
    }

    public bool Equals(LongTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((LongTag)obj);
    }

    public override int GetHashCode() {
        return Value.GetHashCode();
    }

    public static bool operator ==(LongTag? left, LongTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(LongTag? left, LongTag? right) {
        return !(left == right);
    }
}
