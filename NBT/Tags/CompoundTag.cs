using System.Collections;
using System.Collections.Specialized;

namespace NBT.Tags;

/// <summary>
/// A compound NBT tag.
/// </summary>
/// <param name="name">It's name if used as a child of another compound tag, otherwise it should be null.</param>
/// <param name="children">Child properties, should all have names, null values are ignored.</param>
public class CompoundTag : INbtTag, IEquatable<CompoundTag> {
    /// <summary>Child properties, should all have names, null values are ignored.</summary>
    private readonly OrderedDictionary _children;

    public int ChildCount => _children.Count;
    public IEnumerable<(string key, INbtTag child)> Children {
        get {
            foreach (DictionaryEntry child in _children) {
                if (child.Value == null) continue;
                yield return ((string)child.Key, (INbtTag)child.Value);
            }
        }
    }

    public INbtTag? this[string name] => _children.Contains(name) ? (INbtTag?)_children[name] : null;
    
    public bool Contains(string name) => _children.Contains(name);

    private CompoundTag(OrderedDictionary children) {
        _children = children;
    }
    
    public CompoundTag(IDictionary<string, INbtTag?> children) {
        _children = new OrderedDictionary();
        foreach ((string key, INbtTag? tag) in children) {
            if (tag == null) continue;
            _children.Add(key, tag);
        }
    }

    public CompoundTag(params (string, INbtTag?)[] children) {
        _children = new OrderedDictionary();
        foreach ((string key, INbtTag? tag) in children) {
            if (tag == null) continue;
            _children.Add(key, tag);
        }
    }

    public CompoundTag WithChild(string key, INbtTag child, int index = -1) {
        if (child == null) {
            throw new ArgumentNullException(nameof(child), "Child cannot be null");
        }
        
        OrderedDictionary children = new();
        foreach (DictionaryEntry k in _children) {
            children.Add(k.Key, k.Value);
        }

        if (index == -1) {
            children.Add(key, child);
        }
        else {
            children.Insert(index, key, child);
        }
        
        return new CompoundTag(children);
    }
    
    public byte GetPrefix() {
        return NbtTagPrefix.Compound;
    }

    public NbtBuilder Write(NbtBuilder builder, bool noType = false) {
        builder.WriteType(GetPrefix(), noType);  // no write start
        foreach (DictionaryEntry kvp in _children) {
            INbtTag child = (INbtTag)kvp.Value!;
            
            builder.Write(child.GetPrefix());
            builder.WriteString((string)kvp.Key);
            child.Write(builder, true);
        }
        
        return builder.Write(NbtTagPrefix.End);
    }

    public bool Equals(CompoundTag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        
        if (_children.Values.Count != other._children.Values.Count) {
            return false;
        }
        
        foreach (DictionaryEntry kvp in _children) {
            string key = (string)kvp.Key;
            INbtTag child = (INbtTag)kvp.Value!;
            if (!other._children.Contains(key)) {
                return false;
            }

            INbtTag? otherChild = (INbtTag?)other._children[key];
            if (otherChild == null || !child.Equals(otherChild)) {
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
        foreach (DictionaryEntry kvp in _children.Cast<DictionaryEntry>().OrderBy(e => (string)e.Key)) {
            hash.Add((string)kvp.Key);
            hash.Add((INbtTag)kvp.Value!);
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
