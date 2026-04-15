namespace NBT.Tags;

public class EmptyTag : INbtTag, IEquatable<EmptyTag> {
    
    public byte GetPrefix() {
        return NbtTagPrefix.End;
    }

    public NbtBuilder Write(NbtBuilder builder, bool noType = false) {
        return builder.WriteType(GetPrefix(), noType);
    }

    public bool Equals(EmptyTag? other) {
        return other is not null;
    }

    public override bool Equals(object? obj) {
        return obj is EmptyTag;
    }

    public override int GetHashCode() {
        return typeof(EmptyTag).GetHashCode();  // All EmptyTag instances are considered equal
    }

    public static bool operator ==(EmptyTag? left, EmptyTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(EmptyTag? left, EmptyTag? right) {
        return !(left == right);
    }
}
