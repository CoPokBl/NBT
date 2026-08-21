using NBT;
using NBT.Tags;

namespace Tests;

public class NbtJsonTest {

    [Test]
    public void CompoundTest() {
        INbtTag tag = new CompoundTag(
            ("blue", new BooleanTag(false)),
            ("age", new DoubleTag(19.3)),
            ("manzanas", new IntegerTag(3)),
            ("person", new CompoundTag(
                ("name", new StringTag("CoPokBl")), 
                ("age", new IntegerTag(1024)))));
        
        INbtTag tag2 = INbtTag.FromJson(tag.ToJsonString());
        
        Assert.That(tag2, Is.TypeOf<CompoundTag>());
        
        CompoundTag tag2Compound = (CompoundTag)tag2;
        using (Assert.EnterMultipleScope()) {
            Assert.That(tag2Compound["blue"].GetBoolean(), Is.EqualTo(false));
            Assert.That(Math.Abs(tag2Compound["age"].GetDouble() - 19.3), Is.LessThan(0.01));
            Assert.That(tag2Compound["manzanas"].GetInteger(), Is.EqualTo(3));
            Assert.That(tag2Compound["person"], Is.TypeOf<CompoundTag>());
        }
        CompoundTag person = tag2Compound["person"].GetCompound();
        using (Assert.EnterMultipleScope()) {
            Assert.That(person["name"].GetString(), Is.EqualTo("CoPokBl"));
            Assert.That(person["age"].GetInteger(), Is.EqualTo(1024));
        }
    }

    [Test]
    public void BooleanTest() {
        INbtTag tag = new BooleanTag(false);
        INbtTag tag2 = INbtTag.FromJson(tag.ToJsonString());
        
        Assert.That(tag2, Is.TypeOf<BooleanTag>());
        Assert.That(tag2.GetBoolean(), Is.EqualTo(false));
    }

    #region Round trip coercion

    // JSON has no NBT type information, so every whole number comes back as an IntegerTag and
    // every fractional number as a DoubleTag, no matter what tag type it was written from.
    // The getters therefore have to coerce, and these tests pin down that they all do.

    private static INbtTag RoundTrip(INbtTag tag) {
        return INbtTag.FromJson(tag.ToJsonString());
    }

    // Assert.Throws has both TestDelegate and Action overloads, so a lambda has to be typed first.
    private static void AssertThrows<TException>(Action code) where TException : Exception {
        Assert.Throws<TException>(code);
    }

    private static IEnumerable<TestCaseData> WholeNumberTags() {
        yield return new TestCaseData(new ByteTag(42)).SetName("Whole_ByteTag");
        yield return new TestCaseData(new ShortTag(42)).SetName("Whole_ShortTag");
        yield return new TestCaseData(new IntegerTag(42)).SetName("Whole_IntegerTag");
        yield return new TestCaseData(new LongTag(42)).SetName("Whole_LongTag");
    }

    /// <summary>
    /// Every whole number tag survives JSON as an IntegerTag, which must coerce to all
    /// the numeric getters. Regression test for GetDouble/GetFloat rejecting an IntegerTag.
    /// </summary>
    [Test, TestCaseSource(nameof(WholeNumberTags))]
    public void WholeNumber_RoundTrip_CoercesToEveryNumericGetter(INbtTag original) {
        INbtTag tag = RoundTrip(original);

        Assert.That(tag, Is.TypeOf<IntegerTag>());
        using (Assert.EnterMultipleScope()) {
            Assert.That(tag.GetByte(), Is.EqualTo((sbyte)42));
            Assert.That(tag.GetShort(), Is.EqualTo((short)42));
            Assert.That(tag.GetInteger(), Is.EqualTo(42));
            Assert.That(tag.GetLong(), Is.EqualTo(42L));
            Assert.That(tag.GetFloat(), Is.EqualTo(42f));
            Assert.That(tag.GetDouble(), Is.EqualTo(42d));
            Assert.That(tag.GetBoolean(), Is.True);
        }
    }

    private static IEnumerable<TestCaseData> NegativeWholeNumberTags() {
        yield return new TestCaseData(new ByteTag(-42)).SetName("Negative_ByteTag");
        yield return new TestCaseData(new ShortTag(-42)).SetName("Negative_ShortTag");
        yield return new TestCaseData(new IntegerTag(-42)).SetName("Negative_IntegerTag");
        yield return new TestCaseData(new LongTag(-42)).SetName("Negative_LongTag");
    }
    
    [Test, TestCaseSource(nameof(NegativeWholeNumberTags))]
    public void NegativeWholeNumber_RoundTrip_CoercesToEveryNumericGetter(INbtTag original) {
        INbtTag tag = RoundTrip(original);
        
        Assert.That(tag, Is.TypeOf<IntegerTag>());
        using (Assert.EnterMultipleScope()) {
            Assert.That(tag.GetByte(), Is.EqualTo((sbyte)-42));
            Assert.That(tag.GetShort(), Is.EqualTo((short)-42));
            Assert.That(tag.GetInteger(), Is.EqualTo(-42));
            Assert.That(tag.GetLong(), Is.EqualTo(-42L));
            Assert.That(tag.GetFloat(), Is.EqualTo(-42f));
            Assert.That(tag.GetDouble(), Is.EqualTo(-42d));
            Assert.That(tag.GetBoolean(), Is.True);
        }
    }
    
    private static IEnumerable<TestCaseData> FractionalTags() {
        yield return new TestCaseData(new FloatTag(1.5f), 1.5).SetName("Fractional_FloatTag");
        yield return new TestCaseData(new DoubleTag(1.5), 1.5).SetName("Fractional_DoubleTag");
        yield return new TestCaseData(new FloatTag(-56.34f), -56.34).SetName("Fractional_NegativeFloatTag");
        yield return new TestCaseData(new DoubleTag(19.3), 19.3).SetName("Fractional_PreciseDoubleTag");
    }
    
    /// <summary>
    /// Fractional tags come back as DoubleTag, which has to coerce to both float and double.
    /// </summary>
    [Test, TestCaseSource(nameof(FractionalTags))]
    public void Fractional_RoundTrip_CoercesToFloatAndDouble(INbtTag original, double expected) {
        INbtTag tag = RoundTrip(original);
        
        Assert.That(tag, Is.TypeOf<DoubleTag>());
        using (Assert.EnterMultipleScope()) {
            Assert.That(tag.GetDouble(), Is.EqualTo(expected).Within(1e-9));
            Assert.That(tag.GetFloat(), Is.EqualTo((float)expected).Within(1e-5f));
        }
    }
    
    /// <summary>
    /// A double that happens to hold a whole number still writes a decimal point, so it does
    /// not silently become an IntegerTag on the way back.
    /// </summary>
    [Test]
    public void WholeValuedDouble_RoundTrip_StaysDouble() {
        INbtTag tag = RoundTrip(new DoubleTag(19.0));
        
        Assert.That(tag, Is.TypeOf<DoubleTag>());
        using (Assert.EnterMultipleScope()) {
            Assert.That(tag.GetDouble(), Is.EqualTo(19d));
            Assert.That(tag.GetFloat(), Is.EqualTo(19f));
        }
    }
    
    [Test]
    public void Boolean_RoundTrip_CoercesToWholeNumberGetters() {
        INbtTag tag = RoundTrip(new BooleanTag(true));
        
        Assert.That(tag, Is.TypeOf<BooleanTag>());
        using (Assert.EnterMultipleScope()) {
            Assert.That(tag.GetBoolean(), Is.True);
            Assert.That(tag.GetByte(), Is.EqualTo((sbyte)1));
            Assert.That(tag.GetShort(), Is.EqualTo((short)1));
            Assert.That(tag.GetInteger(), Is.EqualTo(1));
            Assert.That(tag.GetLong(), Is.EqualTo(1L));
        }
    }
    
    /// <summary>
    /// A boolean is the one tag that comes back from JSON still sitting on a ByteTag, so it is
    /// the JSON reachable case for byte to double/float widening.
    /// </summary>
    [Test]
    public void Boolean_RoundTrip_CoercesToFloatAndDouble() {
        INbtTag tag = RoundTrip(new BooleanTag(true));
        
        using (Assert.EnterMultipleScope()) {
            Assert.That(tag.GetDouble(), Is.EqualTo(1d));
            Assert.That(tag.GetFloat(), Is.EqualTo(1f));
        }
    }
    
    [TestCase(0, false)]
    [TestCase(1, true)]
    [TestCase(-1, true)]
    [TestCase(7, true)]
    public void Integer_RoundTrip_CoercesToBoolean(int value, bool expected) {
        INbtTag tag = RoundTrip(new IntegerTag(value));
        
        Assert.That(tag.GetBoolean(), Is.EqualTo(expected));
    }
    
    [Test]
    public void String_RoundTrip_KeepsValue() {
        INbtTag tag = RoundTrip(new StringTag("hello there"));
        
        Assert.That(tag, Is.TypeOf<StringTag>());
        Assert.That(tag.GetString(), Is.EqualTo("hello there"));
    }
    
    [Test]
    public void Empty_RoundTrip_StaysEmpty() {
        CompoundTag tag = (CompoundTag)RoundTrip(new CompoundTag(("nothing", new EmptyTag())));
        
        Assert.That(tag["nothing"], Is.TypeOf<EmptyTag>());
    }
    
    #endregion
    
    #region Round trip boundary values
    
    private static IEnumerable<TestCaseData> BoundaryWholeNumbers() {
        yield return new TestCaseData(new ByteTag(sbyte.MinValue), (long)sbyte.MinValue).SetName("Boundary_SByteMin");
        yield return new TestCaseData(new ByteTag(sbyte.MaxValue), (long)sbyte.MaxValue).SetName("Boundary_SByteMax");
        yield return new TestCaseData(new ShortTag(short.MinValue), (long)short.MinValue).SetName("Boundary_ShortMin");
        yield return new TestCaseData(new ShortTag(short.MaxValue), (long)short.MaxValue).SetName("Boundary_ShortMax");
        yield return new TestCaseData(new IntegerTag(int.MinValue), (long)int.MinValue).SetName("Boundary_IntMin");
        yield return new TestCaseData(new IntegerTag(int.MaxValue), (long)int.MaxValue).SetName("Boundary_IntMax");
        yield return new TestCaseData(new LongTag(long.MinValue), long.MinValue).SetName("Boundary_LongMin");
        yield return new TestCaseData(new LongTag(long.MaxValue), long.MaxValue).SetName("Boundary_LongMax");
    }
    
    /// <summary>
    /// Extreme values must survive the round trip without overflowing into a different number.
    /// </summary>
    [Test, TestCaseSource(nameof(BoundaryWholeNumbers))]
    public void BoundaryWholeNumber_RoundTrip_KeepsValue(INbtTag original, long expected) {
        INbtTag tag = RoundTrip(original);
        
        Assert.That(tag.GetLong(), Is.EqualTo(expected));
    }
    
    [Test]
    public void BoundaryDoubles_RoundTrip_KeepValue() {
        CompoundTag tag = (CompoundTag)RoundTrip(new CompoundTag(
            ("max", new DoubleTag(double.MaxValue)),
            ("min", new DoubleTag(double.MinValue)),
            ("epsilon", new DoubleTag(double.Epsilon)),
            ("floatMax", new FloatTag(float.MaxValue))));
        
        using (Assert.EnterMultipleScope()) {
            Assert.That(tag["max"].GetDouble(), Is.EqualTo(double.MaxValue));
            Assert.That(tag["min"].GetDouble(), Is.EqualTo(double.MinValue));
            Assert.That(tag["epsilon"].GetDouble(), Is.EqualTo(double.Epsilon));
            Assert.That(tag["floatMax"].GetFloat(), Is.EqualTo(float.MaxValue));
        }
    }
    
    /// <summary>
    /// A whole number too big for an int comes back as a LongTag, which still has to coerce
    /// to the wider getters.
    /// </summary>
    [Test]
    public void OversizedWholeNumber_RoundTrip_CoercesToWiderGetters() {
        INbtTag tag = RoundTrip(new LongTag(5_000_000_000L));
        
        Assert.That(tag, Is.TypeOf<LongTag>());
        using (Assert.EnterMultipleScope()) {
            Assert.That(tag.GetLong(), Is.EqualTo(5_000_000_000L));
            Assert.That(tag.GetDouble(), Is.EqualTo(5_000_000_000d));
            Assert.That(tag.GetFloat(), Is.EqualTo(5_000_000_000f));
        }
    }
    
    /// <summary>
    /// Coercion is only allowed when the value fits; a number too big for the target type
    /// must throw rather than silently wrap around.
    /// </summary>
    [Test]
    public void OutOfRangeCoercion_Throws() {
        INbtTag tooBigForByte = RoundTrip(new IntegerTag(1000));
        INbtTag tooBigForShort = RoundTrip(new IntegerTag(100000));
        INbtTag tooBigForInt = RoundTrip(new LongTag(long.MaxValue));
        
        using (Assert.EnterMultipleScope()) {
            AssertThrows<ArgumentOutOfRangeException>(() => tooBigForByte.GetByte());
            AssertThrows<ArgumentOutOfRangeException>(() => tooBigForShort.GetShort());
            AssertThrows<ArgumentOutOfRangeException>(() => tooBigForInt.GetInteger());
        }
    }
    
    /// <summary>
    /// The byte range check must use the real sbyte bounds, not an off by one version of them.
    /// </summary>
    [Test]
    public void ByteCoercion_RespectsSByteBounds() {
        using (Assert.EnterMultipleScope()) {
            Assert.That(RoundTrip(new IntegerTag(127)).GetByte(), Is.EqualTo((sbyte)127));
            Assert.That(RoundTrip(new IntegerTag(-128)).GetByte(), Is.EqualTo((sbyte)-128));
            AssertThrows<ArgumentOutOfRangeException>(() => RoundTrip(new IntegerTag(128)).GetByte());
            AssertThrows<ArgumentOutOfRangeException>(() => RoundTrip(new IntegerTag(-129)).GetByte());
        }
    }
    
    /// <summary>
    /// Coercion between unrelated types is still an error.
    /// </summary>
    [Test]
    public void IncompatibleCoercion_Throws() {
        INbtTag str = RoundTrip(new StringTag("not a number"));
        INbtTag number = RoundTrip(new IntegerTag(5));
        
        using (Assert.EnterMultipleScope()) {
            AssertThrows<InvalidCastException>(() => str.GetInteger());
            AssertThrows<InvalidCastException>(() => str.GetDouble());
            AssertThrows<InvalidCastException>(() => number.GetString());
            AssertThrows<InvalidCastException>(() => number.GetCompound());
            AssertThrows<InvalidCastException>(() => number.GetList());
        }
    }
    
    #endregion
    
    #region Round trip collections
    
    /// <summary>
    /// JSON has one array type, so every array and list tag comes back as a ListTag and the
    /// array getters have to read it element by element.
    /// </summary>
    [Test]
    public void IntArray_RoundTrip_CoercesToArrayGetters() {
        INbtTag tag = RoundTrip(new ArrayTag<int>(1, -2, 3));
        
        Assert.That(tag, Is.TypeOf<ListTag>());
        using (Assert.EnterMultipleScope()) {
            Assert.That(tag.GetIntegers().ToArray(), Is.EqualTo(new[] { 1, -2, 3 }));
            Assert.That(tag.GetLongs().ToArray(), Is.EqualTo(new[] { 1L, -2L, 3L }));
            Assert.That(tag.GetBytes().ToArray(), Is.EqualTo(new sbyte[] { 1, -2, 3 }));
        }
    }
    
    [Test]
    public void ByteArray_RoundTrip_CoercesToArrayGetters() {
        INbtTag tag = RoundTrip(new ArrayTag<sbyte>(0, 1, -2, sbyte.MaxValue, sbyte.MinValue));
        
        using (Assert.EnterMultipleScope()) {
            Assert.That(tag.GetBytes().ToArray(), Is.EqualTo(new sbyte[] { 0, 1, -2, sbyte.MaxValue, sbyte.MinValue }));
            Assert.That(tag.GetIntegers().ToArray(), Is.EqualTo(new[] { 0, 1, -2, 127, -128 }));
            Assert.That(tag.GetLongs().ToArray(), Is.EqualTo(new[] { 0L, 1L, -2L, 127L, -128L }));
        }
    }
    
    [Test]
    public void LongArray_RoundTrip_KeepsValues() {
        INbtTag tag = RoundTrip(new ArrayTag<long>(0, long.MaxValue, long.MinValue));
        
        Assert.That(tag.GetLongs().ToArray(), Is.EqualTo(new[] { 0L, long.MaxValue, long.MinValue }));
    }
    
    [Test]
    public void EmptyArrays_RoundTrip_StayEmpty() {
        CompoundTag tag = (CompoundTag)RoundTrip(new CompoundTag(
            ("ints", new ArrayTag<int>()),
            ("bytes", new ArrayTag<sbyte>()),
            ("longs", new ArrayTag<long>()),
            ("list", new ListTag())));
        
        using (Assert.EnterMultipleScope()) {
            Assert.That(tag["ints"].GetIntegers().ToArray(), Is.Empty);
            Assert.That(tag["bytes"].GetBytes().ToArray(), Is.Empty);
            Assert.That(tag["longs"].GetLongs().ToArray(), Is.Empty);
            Assert.That(tag["list"].GetList().Tags.ToArray(), Is.Empty);
        }
    }
    
    [Test]
    public void TypedList_RoundTrip_CoercesElements() {
        INbtTag tag = RoundTrip(new ListTag<FloatTag>(new FloatTag(1.5f), new FloatTag(-2.25f)));
        
        ListTag list = tag.GetList();
        Assert.That(list.Tags.Length, Is.EqualTo(2));
        using (Assert.EnterMultipleScope()) {
            Assert.That(list.Tags[0].GetFloat(), Is.EqualTo(1.5f));
            Assert.That(list.Tags[1].GetDouble(), Is.EqualTo(-2.25d));
        }
    }
    
    [Test]
    public void ListOfCompounds_RoundTrip_KeepsChildren() {
        INbtTag tag = RoundTrip(new ListTag<CompoundTag>(
            new CompoundTag(("name", new StringTag("a")), ("hp", new ShortTag(20))),
            new CompoundTag(("name", new StringTag("b")), ("hp", new ShortTag(-5)))));
        
        ListTag list = tag.GetList();
        Assert.That(list.Tags.Length, Is.EqualTo(2));
        using (Assert.EnterMultipleScope()) {
            Assert.That(list.Tags[0].GetCompound()["name"].GetString(), Is.EqualTo("a"));
            Assert.That(list.Tags[0].GetCompound()["hp"].GetShort(), Is.EqualTo((short)20));
            Assert.That(list.Tags[1].GetCompound()["name"].GetString(), Is.EqualTo("b"));
            Assert.That(list.Tags[1].GetCompound()["hp"].GetShort(), Is.EqualTo((short)-5));
        }
    }
    
    #endregion
    
    #region Round trip everything at once
    
    /// <summary>
    /// One compound holding every tag type, read back with every matching getter.
    /// </summary>
    [Test]
    public void EveryTagType_RoundTrip_ReadsBackWithEveryGetter() {
        CompoundTag original = new(
            ("empty", new EmptyTag()),
            ("string", new StringTag("CoPokBl")),
            ("bool", new BooleanTag(true)),
            ("byte", new ByteTag(-7)),
            ("short", new ShortTag(300)),
            ("int", new IntegerTag(70000)),
            ("long", new LongTag(5_000_000_000L)),
            ("float", new FloatTag(0.5f)),
            ("double", new DoubleTag(-1234.5)),
            ("intArray", new ArrayTag<int>(1, 2, 3)),
            ("byteArray", new ArrayTag<sbyte>(4, 5, 6)),
            ("longArray", new ArrayTag<long>(7, 8, 9)),
            ("list", new ListTag<StringTag>(new StringTag("x"), new StringTag("y"))),
            ("nested", new CompoundTag(
                ("deep", new CompoundTag(
                    ("value", new IntegerTag(9)))))));
        
        CompoundTag tag = (CompoundTag)RoundTrip(original);
        
        using (Assert.EnterMultipleScope()) {
            Assert.That(tag["empty"], Is.TypeOf<EmptyTag>());
            Assert.That(tag["string"].GetString(), Is.EqualTo("CoPokBl"));
            Assert.That(tag["bool"].GetBoolean(), Is.True);
            Assert.That(tag["byte"].GetByte(), Is.EqualTo((sbyte)-7));
            Assert.That(tag["short"].GetShort(), Is.EqualTo((short)300));
            Assert.That(tag["int"].GetInteger(), Is.EqualTo(70000));
            Assert.That(tag["long"].GetLong(), Is.EqualTo(5_000_000_000L));
            Assert.That(tag["float"].GetFloat(), Is.EqualTo(0.5f));
            Assert.That(tag["double"].GetDouble(), Is.EqualTo(-1234.5));
            Assert.That(tag["intArray"].GetIntegers().ToArray(), Is.EqualTo(new[] { 1, 2, 3 }));
            Assert.That(tag["byteArray"].GetBytes().ToArray(), Is.EqualTo(new sbyte[] { 4, 5, 6 }));
            Assert.That(tag["longArray"].GetLongs().ToArray(), Is.EqualTo(new[] { 7L, 8L, 9L }));
            Assert.That(tag["list"].GetList().Tags[0].GetString(), Is.EqualTo("x"));
            Assert.That(tag["nested"].GetCompound()["deep"].GetCompound()["value"].GetInteger(), Is.EqualTo(9));
        }
        
        // Widening coercions across the whole compound.
        using (Assert.EnterMultipleScope()) {
            Assert.That(tag["byte"].GetDouble(), Is.EqualTo(-7d));
            Assert.That(tag["byte"].GetFloat(), Is.EqualTo(-7f));
            Assert.That(tag["short"].GetDouble(), Is.EqualTo(300d));
            Assert.That(tag["short"].GetFloat(), Is.EqualTo(300f));
            Assert.That(tag["int"].GetDouble(), Is.EqualTo(70000d));
            Assert.That(tag["int"].GetFloat(), Is.EqualTo(70000f));
            Assert.That(tag["int"].GetLong(), Is.EqualTo(70000L));
            Assert.That(tag["float"].GetDouble(), Is.EqualTo(0.5d));
            Assert.That(tag["double"].GetFloat(), Is.EqualTo(-1234.5f));
        }
    }
    
    /// <summary>
    /// A second round trip must not drift, so JSON output is stable once the types have settled.
    /// </summary>
    [Test]
    public void RoundTrip_IsIdempotent() {
        CompoundTag original = new(
            ("string", new StringTag("CoPokBl")),
            ("bool", new BooleanTag(false)),
            ("int", new IntegerTag(1024)),
            ("double", new DoubleTag(19.3)),
            ("list", new ListTag<IntegerTag>(new IntegerTag(1), new IntegerTag(2))));
        
        INbtTag once = RoundTrip(original);
        INbtTag twice = RoundTrip(once);
        
        using (Assert.EnterMultipleScope()) {
            Assert.That(twice.ToJsonString(), Is.EqualTo(once.ToJsonString()));
            Assert.That(twice, Is.EqualTo(once));
        }
    }
    
    #endregion
}
