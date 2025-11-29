namespace NBT.Tags;

public class DoubleTag(string? name, double value) : INbtTag<DoubleTag>, IEquatable<DoubleTag> {
    public string? Name { get; } = name;
    public double Value { get; } = value;

    public byte GetPrefix() {
        return NbtTagPrefix.Double;
    }
    
    public string? GetName() {
        return Name;
    }
    
    DoubleTag INbtTag<DoubleTag>.WithName(string? name) {
        return new DoubleTag(name, Value);
    }

    public INbtTag WithName(string? name) => ((INbtTag<DoubleTag>)this).WithName(name);
    
    public byte[] Serialise(bool noType = false) {
        return new NbtBuilder().WriteType(GetPrefix(), noType).WriteName(Name).WriteDouble(Value).ToArray();
    }

    public bool Equals(DoubleTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Value.Equals(other.Value);
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((DoubleTag)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Name, Value);
    }

    public static bool operator ==(DoubleTag? left, DoubleTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(DoubleTag? left, DoubleTag? right) {
        return !(left == right);
    }
}
