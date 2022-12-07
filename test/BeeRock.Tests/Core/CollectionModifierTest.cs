using BeeRock.Core.Entities.CodeGen;

namespace BeeRock.Tests.Core;

[TestClass]
public class CollectionModifierTest {
    [TestMethod]
    public void Test_that_generic_collection_is_matched() {
        var line =
            @"		public System.Threading.Tasks.Task<System.Collections.Generic.ICollection<Pet>> FindPetsByStatus([Microsoft.AspNetCore.Mvc.FromQuery] System.Collections.Generic.IEnumerable<Anonymous> status)";

        var m = new CollectionModifier();
        Assert.IsTrue(m.CanModify(line, 0));

        var expected =
            $"		public System.Threading.Tasks.Task<System.Collections.Generic.List<Pet>> FindPetsByStatus([Microsoft.AspNetCore.Mvc.FromQuery] System.Collections.Generic.List<Anonymous> status)";
        var newLine = m.Modify();
        Assert.AreEqual(expected, newLine);
    }
}
