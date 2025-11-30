namespace NBT.Tags;

/// <summary>
/// A compound NBT tag.
/// </summary>
/// <param name="name">It's name if used as a child of another compound tag, otherwise it should be null.</param>
/// <param name="children">Child properties, should all have names, null values are ignored.</param>
public class CompoundTag(string? name, params INbtTag?[] children) : INbtTag<CompoundTag>, IEquatable<CompoundTag> {
    /// <summary>Child properties, should all have names, null values are ignored.</summary>
    public INbtTag?[] Children { get; } = children;
    public string? Name { get; } = name;

    private Dictionary<string, INbtTag>? _childrenMap;
    public Dictionary<string, INbtTag> ChildrenMap {
        get {
            if (_childrenMap != null) {
                return _childrenMap;
            }
            
            _childrenMap = new Dictionary<string, INbtTag>();
            foreach (INbtTag? child in Children) {
                if (child != null) _childrenMap.Add(child.GetName()!, child);
            }

            return _childrenMap;
        }
    }
    
    public INbtTag? this[string name] => ChildrenMap.GetValueOrDefault(name);
    
    public bool Contains(string name) => ChildrenMap.ContainsKey(name);

    public string? GetName() {
        return Name;
    }
    
    CompoundTag INbtTag<CompoundTag>.WithName(string? name) {
        return new CompoundTag(name, Children);
    }

    public INbtTag WithName(string? name) => ((INbtTag<CompoundTag>)this).WithName(name);
    
    public CompoundTag WithChild(INbtTag child) {
        if (child == null) {
            throw new ArgumentNullException(nameof(child), "Child cannot be null");
        }
        if (child.GetName() == null) {
            throw new ArgumentException("Child tags of a compound tag must have names", nameof(child));
        }
        
        List<INbtTag?> children = Children.ToList();
        children.Add(child);
        return new CompoundTag(Name, children.ToArray());
    }
    
    public byte GetPrefix() {
        return NbtTagPrefix.Compound;
    }

    public NbtBuilder Write(NbtBuilder builder, bool noType = false) {
        builder.WriteType(GetPrefix(), noType).WriteName(Name);  // no write start
        foreach (INbtTag? child in Children) {
            if (child == null) continue;
            if (child.GetName() == null) {
                throw new ArgumentException("Child tags of a compound tag must have names", nameof(child));
            }

            child.Write(builder);
        }
        return builder.Write(NbtTagPrefix.End);
    }

    public bool Equals(CompoundTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Name != other.Name) return false;
        
        // Get non-null children for comparison
        INbtTag[] thisChildren = Children.Where(c => c != null).ToArray()!;
        INbtTag[] otherChildren = other.Children.Where(c => c != null).ToArray()!;
        
        if (thisChildren.Length != otherChildren.Length) return false;
        
        // Compare using ChildrenMap for name-based lookup
        foreach (INbtTag child in thisChildren) {
            string? childName = child.GetName();
            if (childName == null) return false;
            
            if (!other.ChildrenMap.TryGetValue(childName, out INbtTag? otherChild)) {
                return false;
            }
            
            if (!child.Equals(otherChild)) {
                return false;
            }
        }
        
        return true;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((CompoundTag)obj);
    }

    public override int GetHashCode() {
        HashCode hash = new();
        hash.Add(Name);
        foreach (INbtTag? child in Children) {
            if (child != null) {
                hash.Add(child);
            }
        }
        return hash.ToHashCode();
    }

    public static bool operator ==(CompoundTag? left, CompoundTag? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(CompoundTag? left, CompoundTag? right) {
        return !(left == right);
    }
}
