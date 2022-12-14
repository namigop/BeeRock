using BeeRock.Core.Entities.CodeGen;

namespace BeeRock.Tests.Core;

[TestClass]
public class DictionaryModifierTest {
    [TestMethod]
    public void Test_that_idictionary_is_matched() {
        var line =
            "public System.Threading.Tasks.Task<System.Collections.Generic.IDictionary<string, int>> GetInventory()";

        var m = new DictionaryModifier();
        Assert.IsTrue(m.CanModify(line, 0));

        var expected =
            "public System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, int>> GetInventory()";

        var newLine = m.Modify();
        Assert.AreEqual(expected, newLine);
    }
}