using NBT;
using NBT.Tags;

namespace Tests;

public class Tests {
    [Test]
    public void SerialiseDeserialise() {
        TestTagNoErrors(new BooleanTag(true), tag => {
            Assert.That(tag, Is.AssignableTo(typeof(ByteTag)));
            Assert.That(((ByteTag)tag).Value, Is.EqualTo(0x01));
        });
        TestTagNoErrors(new ByteTag(0x56), tag => {
            Assert.That(tag, Is.AssignableTo(typeof(ByteTag)));
            Assert.That(((ByteTag)tag).Value, Is.EqualTo(0x56));
        });
        TestTagNoErrors(new DoubleTag(56.34), tag => {
            Assert.That(tag, Is.AssignableTo(typeof(DoubleTag)));
            Assert.That(((DoubleTag)tag).Value, Is.EqualTo(56.34));
        });
        TestTagNoErrors(new FloatTag(56.34f), tag => {
            Assert.That(tag, Is.AssignableTo(typeof(FloatTag)));
            Assert.That(((FloatTag)tag).Value, Is.EqualTo(56.34f));
        });
        TestTagNoErrors(new IntegerTag(6), tag => {
            Assert.That(tag, Is.AssignableTo(typeof(IntegerTag)));
            Assert.That(((IntegerTag)tag).Value, Is.EqualTo(6));
        });
        TestTagNoErrors(new StringTag("hello there"), tag => {
            Assert.That(tag, Is.AssignableTo(typeof(StringTag)));
            Assert.That(((StringTag)tag).Value, Is.EqualTo("hello there"));
        });
        TestTagNoErrors(new CompoundTag(), tag => {
            Assert.That(tag, Is.AssignableTo(typeof(CompoundTag)));
            Assert.That(((CompoundTag)tag).Children, Is.Empty);
        });
        TestTagNoErrors(new CompoundTag(
                ("potato", new BooleanTag(true)), 
                ("someint", new IntegerTag(7))
                ), tag => {
            Assert.That(tag, Is.AssignableTo(typeof(CompoundTag)));
            CompoundTag comp = (CompoundTag)tag;
            
            Assert.That(comp.ChildCount, Is.EqualTo(2));
            using (Assert.EnterMultipleScope()) {
                Assert.That(comp.Children.First().child, Is.AssignableTo(typeof(ByteTag)));
                Assert.That(((ByteTag)comp.Children.First().child).Value, Is.EqualTo(0x01));
                Assert.That(comp.Children.First().key, Is.EqualTo("potato"));
            }
            using (Assert.EnterMultipleScope()) {
                Assert.That(comp.Children.Skip(1).First().child, Is.AssignableTo(typeof(IntegerTag)));
                Assert.That(((IntegerTag)comp.Children.Skip(1).First().child).Value, Is.EqualTo(7));
                Assert.That(comp.Children.Skip(1).First().key, Is.EqualTo("someint"));
            }
        });
        TestTagNoErrors(new ListTag<IntegerTag>(
        [
            new IntegerTag(7), 
            new IntegerTag(2)
        ]), tag => {
            Assert.That(tag, Is.AssignableTo(typeof(ListTag<IntegerTag>)));
            ListTag<IntegerTag> comp = (ListTag<IntegerTag>)tag;
            Assert.That(comp.Tags.Length, Is.EqualTo(2));
            
            using (Assert.EnterMultipleScope()) {
                Assert.That(comp.Tags[0], Is.AssignableTo(typeof(IntegerTag)));
                Assert.That(comp.Tags[0].Value, Is.EqualTo(7));
            }
            using (Assert.EnterMultipleScope()) {
                Assert.That(comp.Tags[1], Is.AssignableTo(typeof(IntegerTag)));
                Assert.That(comp.Tags[1].Value, Is.EqualTo(2));
            }
        });
        
        // More complex
        CompoundTag someTag = new(
            ("name", new StringTag("Test")), 
            ("age", new IntegerTag(30)), 
            ("SomeList", new ListTag<IntegerTag>([new IntegerTag(1), new IntegerTag(2)]
            )),
            ("AnArrayOfBytes", new ArrayTag<sbyte>(0, 1, 2))
        );
        byte[] serialised = someTag.Serialise();
        byte[] enc = CompressionHelper.CompressZLib(serialised);
        
        INbtTag deserialised = NbtReader.ReadNbt(enc, false, NbtCompressionType.ZLib);
        Assert.That(deserialised, Is.AssignableTo(typeof(CompoundTag)));
        CompoundTag deserialisedComp = (CompoundTag)deserialised;
        Assert.That(deserialisedComp.ChildCount, Is.EqualTo(4));
        using (Assert.EnterMultipleScope()) {
            Assert.That(deserialisedComp.Children.First().child, Is.AssignableTo(typeof(StringTag)));
            Assert.That(((StringTag)deserialisedComp.Children.First().child).Value, Is.EqualTo("Test"));
            Assert.That(deserialisedComp.Children.First().key, Is.EqualTo("name"));
        }
    }

    /// <summary>
    /// Binary NBT keeps byte and short tags as they are, so a field written as a byte or a short
    /// still has to widen when read back as a float or a double.
    /// </summary>
    [Test]
    public void DeserialisedWholeNumbers_CoerceToFloatAndDouble() {
        CompoundTag original = new(
            ("byte", new ByteTag(-7)),
            ("short", new ShortTag(300)),
            ("int", new IntegerTag(70000)),
            ("long", new LongTag(5_000_000_000L)));
        
        CompoundTag tag = (CompoundTag)NbtReader.ReadNbt(original.Serialise());
        
        using (Assert.EnterMultipleScope()) {
            Assert.That(tag["byte"].GetDouble(), Is.EqualTo(-7d));
            Assert.That(tag["byte"].GetFloat(), Is.EqualTo(-7f));
            Assert.That(tag["short"].GetDouble(), Is.EqualTo(300d));
            Assert.That(tag["short"].GetFloat(), Is.EqualTo(300f));
            Assert.That(tag["int"].GetDouble(), Is.EqualTo(70000d));
            Assert.That(tag["int"].GetFloat(), Is.EqualTo(70000f));
            Assert.That(tag["long"].GetDouble(), Is.EqualTo(5_000_000_000d));
            Assert.That(tag["long"].GetFloat(), Is.EqualTo(5_000_000_000f));
        }
    }

    private static void TestTagNoErrors(INbtTag tag, Action<INbtTag>? checker = null) {
        INbtTag thing = NbtReader.ReadNbt(tag.Serialise());
        checker?.Invoke(thing);
    }

    
}