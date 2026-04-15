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
            Assert.Multiple(() => {
                Assert.That(comp.Children.First().child, Is.AssignableTo(typeof(ByteTag)));
                Assert.That(((ByteTag)comp.Children.First().child).Value, Is.EqualTo(0x01));
                Assert.That(comp.Children.First().key, Is.EqualTo("potato"));
            });
            Assert.Multiple(() => {
                Assert.That(comp.Children.Skip(1).First().child, Is.AssignableTo(typeof(IntegerTag)));
                Assert.That(((IntegerTag)comp.Children.Skip(1).First().child).Value, Is.EqualTo(7));
                Assert.That(comp.Children.Skip(1).First().key, Is.EqualTo("someint"));
            });
        });
        TestTagNoErrors(new ListTag<IntegerTag>(
        [
            new IntegerTag(7), 
            new IntegerTag(2)
        ]), tag => {
            Assert.That(tag, Is.AssignableTo(typeof(ListTag<IntegerTag>)));
            ListTag<IntegerTag> comp = (ListTag<IntegerTag>)tag;
            Assert.That(comp.Tags.Length, Is.EqualTo(2));
            
            Assert.Multiple(() => {
                Assert.That(comp.Tags[0], Is.AssignableTo(typeof(IntegerTag)));
                Assert.That(comp.Tags[0].Value, Is.EqualTo(7));
            });
            Assert.Multiple(() => {
                Assert.That(comp.Tags[1], Is.AssignableTo(typeof(IntegerTag)));
                Assert.That(comp.Tags[1].Value, Is.EqualTo(2));
            });
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
        Assert.Multiple(() => {
            Assert.That(deserialisedComp.Children.First().child, Is.AssignableTo(typeof(StringTag)));
            Assert.That(((StringTag)deserialisedComp.Children.First().child).Value, Is.EqualTo("Test"));
            Assert.That(deserialisedComp.Children.First().key, Is.EqualTo("name"));
        });
    }

    private static void TestTagNoErrors(INbtTag tag, Action<INbtTag>? checker = null) {
        INbtTag thing = NbtReader.ReadNbt(tag.Serialise());
        checker?.Invoke(thing);
    }

    
}