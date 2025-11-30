namespace NBT.Tags;

public class ByteTag(string? name, sbyte value) : INbtTag<ByteTag>, IEquatable<ByteTag> {
    public string? Name { get; } = name;
    public sbyte Value { get; } = value;

    public bool BoolValue => Value != 0x00;

    public byte GetPrefix() {
        return NbtTagPrefix.Byte;
    }

    public string? GetName() {
        return Name;
    }

    ByteTag INbtTag<ByteTag>.WithName(string? name) {
        return new ByteTag(name, Value);
    }

    public INbtTag WithName(string? name) => ((INbtTag<ByteTag>)this).WithName(name);

    public byte[] Serialise(bool noType = false) {
        return new NbtBuilder().WriteType(GetPrefix(), noType).WriteName(Name).WriteByte(Value).ToArray();
    }

    public bool Equals(ByteTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Value == other.Value;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ByteTag)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Name, Value);
    }

    public static bool operator ==(ByteTag? left, ByteTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(ByteTag? left, ByteTag? right) {
        return !(left == right);
    }
}
