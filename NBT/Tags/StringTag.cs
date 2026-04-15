namespace NBT.Tags;

public class StringTag(string value) : INbtTag, IEquatable<StringTag> {
    public string Value { get; } = value;

    public byte GetPrefix() {
        return NbtTagPrefix.String;
    }
    
    public NbtBuilder Write(NbtBuilder builder, bool noType = false) {
        return builder.WriteType(GetPrefix(), noType).WriteString(Value);
    }

    public bool Equals(StringTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((StringTag)obj);
    }

    public override int GetHashCode() {
        return Value.GetHashCode();
    }

    public static bool operator ==(StringTag? left, StringTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(StringTag? left, StringTag? right) {
        return !(left == right);
    }
}
