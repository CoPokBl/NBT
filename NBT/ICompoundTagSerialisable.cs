using NBT.Tags;

namespace NBT;

/// <summary>
/// <p> An interface that can be serialised into a compound NBT tag. </p>
/// <p> Provides methods for performing the serialisation. </p>
/// <p> If you can, use <see cref="CompoundTagSerialisable"/> because it can be used as an
/// <see cref="INbtTag"/> directly</p>
///
/// <seealso cref="CompoundTagSerialisable"/>
/// </summary>
public interface ICompoundTagSerialisable {

    public CompoundTag SerialiseToTag();
}

/// <summary>
/// <p> A class that can be serialised into a compound NBT tag. </p>
/// <p> Provides methods for performing the serialisation. </p>
/// <p> This is preferred over <see cref="ICompoundTagSerialisable"/> because it can be used as an
/// <see cref="INbtTag"/> directly </p>
///
/// <seealso cref="ICompoundTagSerialisable"/>
/// </summary>
public abstract class CompoundTagSerialisable : ICompoundTagSerialisable, INbtTag {
    
    public byte GetPrefix() {
        return NbtTagPrefix.Compound;
    }

    public NbtBuilder Write(NbtBuilder builder, bool noType = false) {
        return SerialiseToTag().Write(builder, noType);
    }

    public abstract CompoundTag SerialiseToTag();
}
