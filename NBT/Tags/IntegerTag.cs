namespace NBT.Tags;

public class IntegerTag(string? name, int value) : INbtTag<IntegerTag>, IEquatable<IntegerTag> {
    public string? Name { get; } = name;
    public int Value { get; } = value;

    public byte GetPrefix() {
        return NbtTagPrefix.Integer;
    }
    
    public string? GetName() {
        return Name;
    }
    
    IntegerTag INbtTag<IntegerTag>.WithName(string? name) {
        return new IntegerTag(name, Value);
    }

    public INbtTag WithName(string? name) => ((INbtTag<IntegerTag>)this).WithName(name);
    
    public byte[] Serialise(bool noType = false) {
        return new NbtBuilder().WriteType(GetPrefix(), noType).WriteName(Name).WriteInteger(Value).ToArray();
    }

    public bool Equals(IntegerTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Value == other.Value;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((IntegerTag)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Name, Value);
    }

    public static bool operator ==(IntegerTag? left, IntegerTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(IntegerTag? left, IntegerTag? right) {
        return !(left == right);
    }
}
