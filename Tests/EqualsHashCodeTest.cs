using NBT.Tags;

namespace Tests;

public class EqualsHashCodeTests {
    
    #region StringTag Tests
    
    [Test]
    public void StringTag_Equals_SameValues_ReturnsTrue() {
        StringTag tag1 = new("value");
        StringTag tag2 = new("value");
        
        Assert.That(tag1.Equals(tag2), Is.True);
        Assert.That(tag1 == tag2, Is.True);
        Assert.That(tag1 != tag2, Is.False);
    }
    
    [Test]
    public void StringTag_Equals_DifferentValue_ReturnsFalse() {
        StringTag tag1 = new("value1");
        StringTag tag2 = new("value2");
        
        Assert.That(tag1.Equals(tag2), Is.False);
    }
    
    [Test]
    public void StringTag_GetHashCode_SameValues_SameHashCode() {
        StringTag tag1 = new("value");
        StringTag tag2 = new("value");
        
        Assert.That(tag1.GetHashCode(), Is.EqualTo(tag2.GetHashCode()));
    }
    
    [Test]
    public void StringTag_Equals_NullName_Works() {
        StringTag tag1 = new("value");
        StringTag tag2 = new("value");
        
        Assert.That(tag1.Equals(tag2), Is.True);
    }
    
    #endregion
    
    #region IntegerTag Tests
    
    [Test]
    public void IntegerTag_Equals_SameValues_ReturnsTrue() {
        IntegerTag tag1 = new(42);
        IntegerTag tag2 = new(42);
        
        Assert.That(tag1.Equals(tag2), Is.True);
        Assert.That(tag1 == tag2, Is.True);
    }
    
    [Test]
    public void IntegerTag_Equals_DifferentValue_ReturnsFalse() {
        IntegerTag tag1 = new(42);
        IntegerTag tag2 = new(43);
        
        Assert.That(tag1.Equals(tag2), Is.False);
    }
    
    [Test]
    public void IntegerTag_GetHashCode_SameValues_SameHashCode() {
        IntegerTag tag1 = new(42);
        IntegerTag tag2 = new(42);
        
        Assert.That(tag1.GetHashCode(), Is.EqualTo(tag2.GetHashCode()));
    }
    
    #endregion
    
    #region ByteTag Tests
    
    [Test]
    public void ByteTag_Equals_SameValues_ReturnsTrue() {
        ByteTag tag1 = new(0x42);
        ByteTag tag2 = new(0x42);
        
        Assert.That(tag1.Equals(tag2), Is.True);
        Assert.That(tag1 == tag2, Is.True);
    }
    
    [Test]
    public void ByteTag_Equals_DifferentValue_ReturnsFalse() {
        ByteTag tag1 = new(0x42);
        ByteTag tag2 = new(0x43);
        
        Assert.That(tag1.Equals(tag2), Is.False);
    }
    
    [Test]
    public void ByteTag_GetHashCode_SameValues_SameHashCode() {
        ByteTag tag1 = new(0x42);
        ByteTag tag2 = new(0x42);
        
        Assert.That(tag1.GetHashCode(), Is.EqualTo(tag2.GetHashCode()));
    }
    
    #endregion
    
    #region BooleanTag Tests
    
    [Test]
    public void BooleanTag_Equals_SameValues_ReturnsTrue() {
        BooleanTag tag1 = new(true);
        BooleanTag tag2 = new(true);
        
        Assert.That(tag1.Equals(tag2), Is.True);
        Assert.That(tag1 == tag2, Is.True);
    }
    
    [Test]
    public void BooleanTag_Equals_DifferentValue_ReturnsFalse() {
        BooleanTag tag1 = new(true);
        BooleanTag tag2 = new(false);
        
        Assert.That(tag1.Equals(tag2), Is.False);
    }
    
    [Test]
    public void BooleanTag_GetHashCode_SameValues_SameHashCode() {
        BooleanTag tag1 = new(true);
        BooleanTag tag2 = new(true);
        
        Assert.That(tag1.GetHashCode(), Is.EqualTo(tag2.GetHashCode()));
    }
    
    #endregion
    
    #region DoubleTag Tests
    
    [Test]
    public void DoubleTag_Equals_SameValues_ReturnsTrue() {
        DoubleTag tag1 = new(3.14159);
        DoubleTag tag2 = new(3.14159);
        
        Assert.That(tag1.Equals(tag2), Is.True);
        Assert.That(tag1 == tag2, Is.True);
    }
    
    [Test]
    public void DoubleTag_Equals_DifferentValue_ReturnsFalse() {
        DoubleTag tag1 = new(3.14159);
        DoubleTag tag2 = new(2.71828);
        
        Assert.That(tag1.Equals(tag2), Is.False);
    }
    
    [Test]
    public void DoubleTag_GetHashCode_SameValues_SameHashCode() {
        DoubleTag tag1 = new(3.14159);
        DoubleTag tag2 = new(3.14159);
        
        Assert.That(tag1.GetHashCode(), Is.EqualTo(tag2.GetHashCode()));
    }
    
    #endregion
    
    #region FloatTag Tests
    
    [Test]
    public void FloatTag_Equals_SameValues_ReturnsTrue() {
        FloatTag tag1 = new(3.14f);
        FloatTag tag2 = new(3.14f);
        
        Assert.That(tag1.Equals(tag2), Is.True);
        Assert.That(tag1 == tag2, Is.True);
    }
    
    [Test]
    public void FloatTag_Equals_DifferentValue_ReturnsFalse() {
        FloatTag tag1 = new(3.14f);
        FloatTag tag2 = new(2.71f);
        
        Assert.That(tag1.Equals(tag2), Is.False);
    }
    
    [Test]
    public void FloatTag_GetHashCode_SameValues_SameHashCode() {
        FloatTag tag1 = new(3.14f);
        FloatTag tag2 = new(3.14f);
        
        Assert.That(tag1.GetHashCode(), Is.EqualTo(tag2.GetHashCode()));
    }
    
    #endregion
    
    #region LongTag Tests
    
    [Test]
    public void LongTag_Equals_SameValues_ReturnsTrue() {
        LongTag tag1 = new(9223372036854775807L);
        LongTag tag2 = new(9223372036854775807L);
        
        Assert.That(tag1.Equals(tag2), Is.True);
        Assert.That(tag1 == tag2, Is.True);
    }
    
    [Test]
    public void LongTag_Equals_DifferentValue_ReturnsFalse() {
        LongTag tag1 = new(9223372036854775807L);
        LongTag tag2 = new(1234567890L);
        
        Assert.That(tag1.Equals(tag2), Is.False);
    }
    
    [Test]
    public void LongTag_GetHashCode_SameValues_SameHashCode() {
        LongTag tag1 = new(9223372036854775807L);
        LongTag tag2 = new(9223372036854775807L);
        
        Assert.That(tag1.GetHashCode(), Is.EqualTo(tag2.GetHashCode()));
    }
    
    #endregion
    
    #region ShortTag Tests
    
    [Test]
    public void ShortTag_Equals_SameValues_ReturnsTrue() {
        ShortTag tag1 = new(12345);
        ShortTag tag2 = new(12345);
        
        Assert.That(tag1.Equals(tag2), Is.True);
        Assert.That(tag1 == tag2, Is.True);
    }
    
    [Test]
    public void ShortTag_Equals_DifferentValue_ReturnsFalse() {
        ShortTag tag1 = new(12345);
        ShortTag tag2 = new(12346);
        
        Assert.That(tag1.Equals(tag2), Is.False);
    }
    
    [Test]
    public void ShortTag_GetHashCode_SameValues_SameHashCode() {
        ShortTag tag1 = new(12345);
        ShortTag tag2 = new(12345);
        
        Assert.That(tag1.GetHashCode(), Is.EqualTo(tag2.GetHashCode()));
    }
    
    #endregion
    
    #region EmptyTag Tests
    
    [Test]
    public void EmptyTag_Equals_TwoEmptyTags_ReturnsTrue() {
        EmptyTag tag1 = new();
        EmptyTag tag2 = new();
        
        Assert.That(tag1.Equals(tag2), Is.True);
        Assert.That(tag1 == tag2, Is.True);
    }
    
    [Test]
    public void EmptyTag_GetHashCode_SameHashCode() {
        EmptyTag tag1 = new();
        EmptyTag tag2 = new();
        
        Assert.That(tag1.GetHashCode(), Is.EqualTo(tag2.GetHashCode()));
    }
    
    #endregion
    
    #region ListTag Tests
    
    [Test]
    public void ListTag_Equals_SameValues_ReturnsTrue() {
        ListTag tag1 = new([new IntegerTag(1), new IntegerTag(2)]);
        ListTag tag2 = new([new IntegerTag(1), new IntegerTag(2)]);
        
        Assert.That(tag1.Equals(tag2), Is.True);
        Assert.That(tag1 == tag2, Is.True);
    }
    
    [Test]
    public void ListTag_Equals_DifferentOrder_ReturnsFalse() {
        ListTag tag1 = new([new IntegerTag(1), new IntegerTag(2)]);
        ListTag tag2 = new([new IntegerTag(2), new IntegerTag(1)]);
        
        Assert.That(tag1.Equals(tag2), Is.False);
    }
    
    [Test]
    public void ListTag_Equals_DifferentLength_ReturnsFalse() {
        ListTag tag1 = new([new IntegerTag(1), new IntegerTag(2)]);
        ListTag tag2 = new([new IntegerTag(1)]);
        
        Assert.That(tag1.Equals(tag2), Is.False);
    }
    
    [Test]
    public void ListTag_GetHashCode_SameValues_SameHashCode() {
        ListTag tag1 = new([new IntegerTag(1), new IntegerTag(2)]);
        ListTag tag2 = new([new IntegerTag(1), new IntegerTag(2)]);
        
        Assert.That(tag1.GetHashCode(), Is.EqualTo(tag2.GetHashCode()));
    }
    
    [Test]
    public void ListTag_Generic_Equals_SameValues_ReturnsTrue() {
        ListTag<IntegerTag> tag1 = new([new IntegerTag(1), new IntegerTag(2)]);
        ListTag<IntegerTag> tag2 = new([new IntegerTag(1), new IntegerTag(2)]);
        
        Assert.That(tag1.Equals(tag2), Is.True);
        Assert.That(tag1 == tag2, Is.True);
    }
    
    #endregion
    
    #region ArrayTag Tests
    
    [Test]
    public void ArrayTag_Int_Equals_SameValues_ReturnsTrue() {
        ArrayTag<int> tag1 = new(1, 2, 3);
        ArrayTag<int> tag2 = new(1, 2, 3);
        
        Assert.That(tag1.Equals(tag2), Is.True);
        Assert.That(tag1 == tag2, Is.True);
    }
    
    [Test]
    public void ArrayTag_Int_Equals_DifferentValues_ReturnsFalse() {
        ArrayTag<int> tag1 = new(1, 2, 3);
        ArrayTag<int> tag2 = new(1, 2, 4);
        
        Assert.That(tag1.Equals(tag2), Is.False);
    }
    
    [Test]
    public void ArrayTag_Int_Equals_DifferentLength_ReturnsFalse() {
        ArrayTag<int> tag1 = new(1, 2, 3);
        ArrayTag<int> tag2 = new(1, 2);
        
        Assert.That(tag1.Equals(tag2), Is.False);
    }
    
    [Test]
    public void ArrayTag_Int_GetHashCode_SameValues_SameHashCode() {
        ArrayTag<int> tag1 = new(1, 2, 3);
        ArrayTag<int> tag2 = new(1, 2, 3);
        
        Assert.That(tag1.GetHashCode(), Is.EqualTo(tag2.GetHashCode()));
    }
    
    [Test]
    public void ArrayTag_Long_Equals_SameValues_ReturnsTrue() {
        ArrayTag<long> tag1 = new(1L, 2L, 3L);
        ArrayTag<long> tag2 = new(1L, 2L, 3L);
        
        Assert.That(tag1.Equals(tag2), Is.True);
    }
    
    [Test]
    public void ArrayTag_SByte_Equals_SameValues_ReturnsTrue() {
        ArrayTag<sbyte> tag1 = new(1, 2, 3);
        ArrayTag<sbyte> tag2 = new(1, 2, 3);
        
        Assert.That(tag1.Equals(tag2), Is.True);
    }
    
    #endregion
    
    #region CompoundTag Tests
    
    [Test]
    public void CompoundTag_Equals_SameValues_ReturnsTrue() {
        CompoundTag tag1 = new(
            ("int", new IntegerTag(42)),
            ("string", new StringTag("value")));
        CompoundTag tag2 = new(
            ("int", new IntegerTag(42)),
            ("string", new StringTag("value")));
        
        Assert.That(tag1.Equals(tag2), Is.True);
        Assert.That(tag1 == tag2, Is.True);
    }
    
    [Test]
    public void CompoundTag_Equals_DifferentOrder_ReturnsTrue() {
        // CompoundTag should compare by name lookup, not by order
        CompoundTag tag1 = new(
            ("int", new IntegerTag(42)),
            ("string", new StringTag("value")));
        CompoundTag tag2 = new(
            ("string", new StringTag("value")),
            ("int", new IntegerTag(42)));
        
        Assert.That(tag1.Equals(tag2), Is.True);
    }
    
    [Test]
    public void CompoundTag_Equals_DifferentChildValue_ReturnsFalse() {
        CompoundTag tag1 = new(("int", new IntegerTag(42)));
        CompoundTag tag2 = new(("int", new IntegerTag(43)));
        
        Assert.That(tag1.Equals(tag2), Is.False);
    }
    
    [Test]
    public void CompoundTag_Equals_DifferentChildCount_ReturnsFalse() {
        CompoundTag tag1 = new(
            ("int", new IntegerTag(42)),
            ("string", new StringTag("value")));
        CompoundTag tag2 = new(
            ("int", new IntegerTag(42)));
        
        Assert.That(tag1.Equals(tag2), Is.False);
    }
    
    [Test]
    public void CompoundTag_GetHashCode_SameValues_SameHashCode() {
        CompoundTag tag1 = new(
            ("int", new IntegerTag(42)),
            ("string", new StringTag("value")));
        CompoundTag tag2 = new(
            ("int", new IntegerTag(42)),
            ("string", new StringTag("value")));
        
        Assert.That(tag1.GetHashCode(), Is.EqualTo(tag2.GetHashCode()));
    }
    
    [Test]
    public void CompoundTag_GetHashCode_DifferentOrder_SameHashCode() {
        CompoundTag tag1 = new(
            ("int", new IntegerTag(42)),
            ("string", new StringTag("value"))
        );
        CompoundTag tag2 = new(
            ("string", new StringTag("value")),
            ("int", new IntegerTag(42))
        );
        
        Assert.That(tag1.GetHashCode(), Is.EqualTo(tag2.GetHashCode()));
    }
    
    [Test]
    public void CompoundTag_Equals_NestedCompound_ReturnsTrue() {
        CompoundTag tag1 = new(("inner", new CompoundTag(("value", new IntegerTag(42)))));
        CompoundTag tag2 = new(("inner", new CompoundTag(("value", new IntegerTag(42)))));
        
        Assert.That(tag1.Equals(tag2), Is.True);
    }
    
    [Test]
    public void CompoundTag_Equals_NestedCompound_DifferentValue_ReturnsFalse() {
        CompoundTag tag1 = new(("inner", new CompoundTag(("value", new IntegerTag(42)))));
        CompoundTag tag2 = new(("inner", new CompoundTag(("value", new IntegerTag(43)))));
        
        Assert.That(tag1.Equals(tag2), Is.False);
    }
    
    #endregion
    
    #region Null and Edge Cases
    
    [Test]
    public void StringTag_Equals_Null_ReturnsFalse() {
        StringTag tag = new("value");
        
        Assert.That(tag.Equals(null), Is.False);
        Assert.That(tag == null, Is.False);
        Assert.That(null == tag, Is.False);
    }
    
    [Test]
    public void StringTag_Equals_SameReference_ReturnsTrue() {
        StringTag tag = new("value");
        
        Assert.That(tag.Equals(tag), Is.True);
#pragma warning disable CS1718
        Assert.That(tag == tag, Is.True);
#pragma warning restore CS1718
    }
    
    [Test]
    public void NullTags_Equals_BothNull_ReturnsTrue() {
        StringTag? tag1 = null;
        StringTag? tag2 = null;
        
        Assert.That(tag1 == tag2, Is.True);
    }
    
    [Test]
    public void CompoundTag_Equals_EmptyCompound_ReturnsTrue() {
        CompoundTag tag1 = new();
        CompoundTag tag2 = new();
        
        Assert.That(tag1.Equals(tag2), Is.True);
    }
    
    [Test]
    public void ListTag_Equals_EmptyList_ReturnsTrue() {
        ListTag tag1 = new([]);
        ListTag tag2 = new([]);
        
        Assert.That(tag1.Equals(tag2), Is.True);
    }
    
    [Test]
    public void ArrayTag_Equals_EmptyArray_ReturnsTrue() {
        ArrayTag<int> tag1 = new();
        ArrayTag<int> tag2 = new();
        
        Assert.That(tag1.Equals(tag2), Is.True);
    }
    
    #endregion
    
    #region Cross-Type Tests
    
    [Test]
    public void IntegerTag_Equals_Object_DifferentType_ReturnsFalse() {
        IntegerTag intTag = new(42);
        StringTag strTag = new("42");
        
        Assert.That(intTag.Equals(strTag), Is.False);
    }
    
    [Test]
    public void ByteTag_Equals_Object_BooleanTag_ReturnsFalse() {
        // BooleanTag inherits from ByteTag, but should be considered different types
        ByteTag byteTag = new(1);
        BooleanTag boolTag = new(true);
        
        // These are different types, so Equals should return false
        Assert.That(byteTag.Equals((object)boolTag), Is.False);
    }
    
    #endregion
    
    #region Dictionary/HashSet Usage Tests
    
    [Test]
    public void IntegerTag_CanBeUsedInHashSet() {
        HashSet<IntegerTag> set = [];
        IntegerTag tag1 = new(42);
        IntegerTag tag2 = new(42);
        IntegerTag tag3 = new(43);
        
        set.Add(tag1);
        
        Assert.That(set.Contains(tag2), Is.True);
        Assert.That(set.Contains(tag3), Is.False);
    }
    
    [Test]
    public void CompoundTag_CanBeUsedInHashSet() {
        HashSet<CompoundTag> set = [];
        CompoundTag tag1 = new(("value", new IntegerTag(42)));
        CompoundTag tag2 = new(("value", new IntegerTag(42)));
        CompoundTag tag3 = new(("value", new IntegerTag(43)));
        
        set.Add(tag1);
        
        Assert.That(set.Contains(tag2), Is.True);
        Assert.That(set.Contains(tag3), Is.False);
    }
    
    [Test]
    public void StringTag_CanBeUsedAsDictionaryKey() {
        Dictionary<StringTag, string> dict = new();
        StringTag key1 = new("value");
        StringTag key2 = new("value");
        
        dict[key1] = "test";
        
        Assert.That(dict.ContainsKey(key2), Is.True);
        Assert.That(dict[key2], Is.EqualTo("test"));
    }
    
    #endregion
}
