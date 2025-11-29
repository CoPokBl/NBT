namespace NBT.Tags;

public class ListTag<T> : ListTag, IEquatable<ListTag<T>> where T : INbtTag {
    public new T[] Tags => base.Tags.Cast<T>().ToArray();
    
    public ListTag(string? name, T[] tags) : base(name, tags.Cast<INbtTag>().ToArray()) {
        if (typeof(T) == typeof(INbtTag)) {
            throw new ArgumentException("List must only be of one type.", nameof(T));
        }
    }
    
    public new ListTag<T> WithName(string name) {
        return new ListTag<T>(Name, Tags);
    }

    public bool Equals(ListTag<T>? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other);
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is ListTag<T> typedList) return Equals(typedList);
        if (obj is ListTag list) return base.Equals(list);
        return false;
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public static bool operator ==(ListTag<T>? left, ListTag<T>? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(ListTag<T>? left, ListTag<T>? right) {
        return !(left == right);
    }
}

public class ListTag(string? name, INbtTag[] tags) : INbtTag<ListTag>, IEquatable<ListTag> {
    public string? Name { get; } = name;
    public INbtTag[] Tags { get; } = tags;

    public byte GetPrefix() {
        return NbtTagPrefix.List;
    }
    
    public string? GetName() {
        return Name;
    }
    
    ListTag INbtTag<ListTag>.WithName(string? name) {
        return new ListTag(name, Tags);
    }

    public INbtTag WithName(string? name) => ((INbtTag<ListTag>)this).WithName(name);

    public byte[] Serialise(bool noType = false) {
        byte type = Tags.Length == 0 ? NbtTagPrefix.End : Tags[0].GetPrefix();

        NbtBuilder builder = new NbtBuilder().WriteType(NbtTagPrefix.List, noType).WriteName(Name).Write(type).WriteInteger(Tags.Length);
        foreach (INbtTag tag in Tags) {
            builder.Write(tag.Serialise(true));
        }
        return builder.ToArray();
    }

    public bool Equals(ListTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Name != other.Name) return false;
        if (Tags.Length != other.Tags.Length) return false;
        
        for (int i = 0; i < Tags.Length; i++) {
            if (!Tags[i].Equals(other.Tags[i])) {
                return false;
            }
        }
        
        return true;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not ListTag listTag) return false;
        return Equals(listTag);
    }

    public override int GetHashCode() {
        HashCode hash = new();
        hash.Add(Name);
        foreach (INbtTag tag in Tags) {
            hash.Add(tag);
        }
        return hash.ToHashCode();
    }

    public static bool operator ==(ListTag? left, ListTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(ListTag? left, ListTag? right) {
        return !(left == right);
    }
}
