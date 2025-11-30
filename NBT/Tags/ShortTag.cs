namespace NBT.Tags;

public class ShortTag(string? name, short value) : INbtTag<ShortTag>, IEquatable<ShortTag> {
    public string? Name { get; } = name;
    public short Value { get; } = value;

    public byte GetPrefix() {
        return NbtTagPrefix.Short;
    }
    
    public string? GetName() {
        return Name;
    }
    
    ShortTag INbtTag<ShortTag>.WithName(string? name) {
        return new ShortTag(name, Value);
    }

    public INbtTag WithName(string? name) => ((INbtTag<ShortTag>)this).WithName(name);
    
    public byte[] Serialise(bool noType = false) {
        return new NbtBuilder().WriteType(GetPrefix(), noType).WriteName(Name).WriteShort(Value).ToArray();
    }

    public bool Equals(ShortTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Value == other.Value;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ShortTag)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Name, Value);
    }

    public static bool operator ==(ShortTag? left, ShortTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(ShortTag? left, ShortTag? right) {
        return !(left == right);
    }
}
