namespace NBT.Tags;

public class StringTag(string? name, string value) : INbtTag<StringTag>, IEquatable<StringTag> {
    public string? Name { get; } = name;
    public string Value { get; } = value;

    public byte GetPrefix() {
        return NbtTagPrefix.String;
    }
    
    public string? GetName() {
        return Name;
    }
    
    StringTag INbtTag<StringTag>.WithName(string? name) {
        return new StringTag(name, Value);
    }

    public INbtTag WithName(string? name) => ((INbtTag<StringTag>)this).WithName(name);

    public NbtBuilder Write(NbtBuilder builder, bool noType = false) {
        return builder.WriteType(GetPrefix(), noType).WriteName(Name).WriteString(Value);
    }

    public bool Equals(StringTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Value == other.Value;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((StringTag)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Name, Value);
    }

    public static bool operator ==(StringTag? left, StringTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(StringTag? left, StringTag? right) {
        return !(left == right);
    }
}
