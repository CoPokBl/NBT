using NBT;
using NBT.Tags;

namespace Tests;

public class NbtJsonTest {
    
    [Test]
    public void CompoundTest() {
        INbtTag tag = new CompoundTag(null, 
            new BooleanTag("blue", false),
            new DoubleTag("age", 19.3),
            new IntegerTag("manzanas", 3),
            new CompoundTag("person", 
                new StringTag("name", "CoPokBl"), 
                new IntegerTag("age", 1024)));
        
        INbtTag tag2 = INbtTag.FromJson(tag.ToJsonString());
        
        Assert.That(tag2, Is.TypeOf<CompoundTag>());
        
        CompoundTag tag2Compound = (CompoundTag)tag2;
        Assert.Multiple(() => {
            Assert.That(tag2Compound["blue"].GetBoolean(), Is.EqualTo(false));
            Assert.That(Math.Abs(tag2Compound["age"].GetDouble() - 19.3), Is.LessThan(0.01));
            Assert.That(tag2Compound["manzanas"].GetInteger(), Is.EqualTo(3));
            Assert.That(tag2Compound["person"], Is.TypeOf<CompoundTag>());
        });
        CompoundTag person = tag2Compound["person"].GetCompound();
        Assert.Multiple(() => {
            Assert.That(person["name"].GetString(), Is.EqualTo("CoPokBl"));
            Assert.That(person["age"].GetInteger(), Is.EqualTo(1024));
        });
    }

    [Test]
    public void BooleanTest() {
        INbtTag tag = new BooleanTag("blue", false);
        INbtTag tag2 = INbtTag.FromJson(tag.ToJsonString());
        
        Assert.That(tag2, Is.TypeOf<BooleanTag>());
        Assert.That(tag2.GetBoolean(), Is.EqualTo(false));
    }
}