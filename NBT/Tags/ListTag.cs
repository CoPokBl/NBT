namespace NBT.Tags;

public class ListTag<T> : ListTag, IEquatable<ListTag<T>> where T : INbtTag {
    private readonly T[] _tags;
    public new ReadOnlySpan<T> Tags => _tags;

    public ListTag(params T[] tags) {
        _tags = tags.ToArray();
        TagsArray = tags.Cast<INbtTag>().ToArray();
        Verify();
    }
    
    public ListTag(IEnumerable<T> tags) {
        _tags = tags.ToArray();
        TagsArray = _tags.Cast<INbtTag>().ToArray();
        Verify();
    }

    public bool Equals(ListTag<T>? other) {
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

public class ListTag : INbtTag, IEquatable<ListTag> {
    protected INbtTag[] TagsArray;
    public ReadOnlySpan<INbtTag> Tags => TagsArray;

    public ListTag(params INbtTag[] tags) {
        TagsArray = tags.ToArray();
        Verify();
    }

    public ListTag(IEnumerable<INbtTag> tags) {
        TagsArray = tags.ToArray();
        Verify();
    }

    protected void Verify() {
        if (TagsArray.Length == 0) {
            return;
        }
        
        byte prefix = TagsArray[0].GetPrefix();
        for (int i = 1; i < TagsArray.Length; i++) {
            if (TagsArray[i].GetPrefix() != prefix) {
                throw new ArrayTypeMismatchException("All elements of a list tag must be the same type");
            }
        }
    }

    public byte GetPrefix() {
        return NbtTagPrefix.List;
    }
    
    public NbtBuilder Write(NbtBuilder builder, bool noType = false) {
        byte type = TagsArray.Length == 0 ? NbtTagPrefix.End : TagsArray[0].GetPrefix();
        
        builder = builder.WriteType(NbtTagPrefix.List, noType).Write(type).WriteInteger(TagsArray.Length);
        foreach (INbtTag tag in TagsArray) {
            tag.Write(builder, true);
        }
        return builder;
    }

    public bool Equals(ListTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (TagsArray.Length != other.TagsArray.Length) return false;
        
        for (int i = 0; i < TagsArray.Length; i++) {
            if (!TagsArray[i].Equals(other.TagsArray[i])) {
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
