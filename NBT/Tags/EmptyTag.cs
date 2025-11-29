namespace NBT.Tags;

public class EmptyTag : INbtTag<EmptyTag>, IEquatable<EmptyTag> {
    
    public byte GetPrefix() {
        return NbtTagPrefix.End;
    }

    public string GetName() {
        throw new NotSupportedException("EmptyTag may only be used at the root level.");
    }

    public EmptyTag WithName(string? name) {
        throw new NotSupportedException("EmptyTag may only be used at the root level.");
    }

    INbtTag INbtTag.WithName(string? name) {
        throw new NotSupportedException("EmptyTag may only be used at the root level.");
    }

    public byte[] Serialise(bool noType = false) {
        return new NbtBuilder()
            .Write(GetPrefix())
            .ToArray();
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
