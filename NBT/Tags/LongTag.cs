namespace NBT.Tags;

public class LongTag(string? name, long value) : INbtTag<LongTag>, IEquatable<LongTag> {
    public string? Name { get; } = name;
    public long Value { get; } = value;

    public byte GetPrefix() {
        return NbtTagPrefix.Long;
    }
    
    public string? GetName() {
        return Name;
    }
    
    LongTag INbtTag<LongTag>.WithName(string? name) {
        return new LongTag(name, Value);
    }

    public INbtTag WithName(string? name) => ((INbtTag<LongTag>)this).WithName(name);
    
    public byte[] Serialise(bool noType = false) {
        return new NbtBuilder().WriteType(GetPrefix(), noType).WriteName(Name).WriteLong(Value).ToArray();
    }

    public bool Equals(LongTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Value == other.Value;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((LongTag)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Name, Value);
    }

    public static bool operator ==(LongTag? left, LongTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(LongTag? left, LongTag? right) {
        return !(left == right);
    }
}
