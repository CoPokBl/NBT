namespace NBT.Tags;

public class FloatTag(string? name, float value) : INbtTag<FloatTag>, IEquatable<FloatTag> {
    public string? Name { get; } = name;
    public float Value { get; } = value;

    public byte GetPrefix() {
        return NbtTagPrefix.Float;
    }
    
    public string? GetName() {
        return Name;
    }
    
    FloatTag INbtTag<FloatTag>.WithName(string? name) {
        return new FloatTag(name, Value);
    }

    public INbtTag WithName(string? name) => ((INbtTag<FloatTag>)this).WithName(name);

    public NbtBuilder Write(NbtBuilder builder, bool noType = false) {
        return builder.WriteType(GetPrefix(), noType).WriteName(Name).WriteFloat(Value);
    }

    public bool Equals(FloatTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Value.Equals(other.Value);
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((FloatTag)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Name, Value);
    }

    public static bool operator ==(FloatTag? left, FloatTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(FloatTag? left, FloatTag? right) {
        return !(left == right);
    }
}
