using BeeRock.Core.Utils;
using BeeRock.Models;

namespace BeeRock.Tests;

[TestClass]
public class ObjectBuilderTest {

    public class Pet {
        public string StringProp { get; set; }
        public int IntProp { get; set; }
        public double DoubleProp { get; set; }
        public float FloatProp { get; set; }
        public long LongProp { get; set; }
        public ulong ULongProp { get; set; }
        public uint UIntProp { get; set; }
        public decimal DecimalProp { get; set; }
        public short ShortProp { get; set; }
    }

    public class Person {
        public string Name { get; set; }
        public List<Pet> Pets { get; set; }
    }

    [TestMethod]
    public void Test_that_object_instance_is_created() {
        var p = ObjectBuilder.CreateNewInstance(typeof(Pet), 0);
        Assert.IsNotNull(p);
        Assert.AreEqual(typeof(Pet), p.GetType());

        var def = new Pet();
        var p2 = (Pet)p;
        Assert.AreEqual(def.IntProp, p2.IntProp);
        Assert.AreEqual(def.DoubleProp, p2.DoubleProp);
        Assert.AreEqual(def.FloatProp, p2.FloatProp);
        Assert.AreEqual(def.ULongProp, p2.ULongProp);
        Assert.AreEqual(def.UIntProp, p2.UIntProp);
        Assert.AreEqual(def.DecimalProp, p2.DecimalProp);
        Assert.AreEqual(def.ShortProp, p2.ShortProp);
        Assert.IsTrue(!string.IsNullOrWhiteSpace(p2.StringProp));
    }

    [TestMethod]
    public void Test_that_complex_class_instance_is_created() {
        var p = ObjectBuilder.CreateNewInstance(typeof(Person), 0);
        Assert.IsNotNull(p);
        Assert.AreEqual(typeof(Person), p.GetType());

        var p2 = (Person)p;

        Assert.IsTrue(!string.IsNullOrWhiteSpace(p2.Name));
        Assert.AreEqual(1, p2.Pets.Count);
    }

    [TestMethod]
    public void Test_that_List_instance_is_created() {
        var p = ObjectBuilder.CreateNewInstance(typeof(List<Pet>), 0);
        Assert.IsNotNull(p);
        Assert.AreEqual(typeof(List<Pet>), p.GetType());

        var p2 = (List<Pet>)p;
        Assert.AreEqual(1, p2.Count);
    }

    [TestMethod]
    public void Test_that_dict_instance_is_created() {
        var p = ObjectBuilder.CreateNewInstance(typeof(Dictionary<string, Pet>), 0);
        Assert.IsNotNull(p);
        Assert.AreEqual(typeof(Dictionary<string, Pet>), p.GetType());

        var p2 = (Dictionary<string, Pet>)p;
        Assert.AreEqual(1, p2.Count);

        var key = (string)ObjectBuilder.CreateNewInstance(typeof(string), 0);
        Assert.IsNotNull(p2[key]);
    }
}
