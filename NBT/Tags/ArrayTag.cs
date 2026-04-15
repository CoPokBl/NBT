namespace NBT.Tags;

/// <summary>
/// Represents an array of values of a specific type in NBT format.
/// The type must be one of the supported types: int, long, or sbyte.
/// </summary>
/// <typeparam name="T">One of: int, long, or sbyte.</typeparam>
public class ArrayTag<T> : INbtTag, IEquatable<ArrayTag<T>> {
    private readonly T[] _values;

    public ReadOnlySpan<T> Values => _values;
    
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Type[] SupportedTypes = [
        typeof(int), typeof(long), typeof(sbyte)
    ];
    
    public ArrayTag(params T[] values) {
        if (!SupportedTypes.Contains(typeof(T))) {
            throw new ArgumentException("Unsupported type for ArrayTag: " + typeof(T).Name);
        }
        
        _values = values;
    }

    public byte GetPrefix() {
        return typeof(T) switch {
            { } t when t == typeof(int) => NbtTagPrefix.Integers,
            { } t when t == typeof(long) => NbtTagPrefix.Longs,
            { } t when t == typeof(sbyte) => NbtTagPrefix.Bytes,
            _ => throw new ArgumentException("Unsupported type for ArrayTag: " + typeof(T).Name)
        };
    }
    
    public NbtBuilder Write(NbtBuilder builder, bool noType = false) {
        builder.WriteType(GetPrefix(), noType)
            .WriteInteger(Values.Length);
        
        foreach (T v in Values) {
            switch (v) {
                case int iv:
                    builder.WriteInteger(iv);
                    break;
                case long lv:
                    builder.WriteLong(lv);
                    break;
                case sbyte bv:
                    builder.WriteByte(bv);
                    break;
                default:
                    throw new ArgumentException("Unsupported type in ArrayTag: " + v!.GetType().Name);
            }
        }
        
        return builder;
    }

    public bool Equals(ArrayTag<T>? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Values.Length != other.Values.Length) return false;
        
        for (int i = 0; i < Values.Length; i++) {
            if (!EqualityComparer<T>.Default.Equals(Values[i], other.Values[i])) {
                return false;
            }
        }
        
        return true;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ArrayTag<T>)obj);
    }

    public override int GetHashCode() {
        HashCode hash = new();
        foreach (T value in Values) {
            hash.Add(value);
        }
        return hash.ToHashCode();
    }

    public static bool operator ==(ArrayTag<T>? left, ArrayTag<T>? right) {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(ArrayTag<T>? left, ArrayTag<T>? right) {
        return !(left == right);
    }
}
