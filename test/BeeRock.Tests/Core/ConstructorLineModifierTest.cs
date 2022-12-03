using BeeRock.Core.Entities.CodeGen;
using BeeRock.Core.Utils;

namespace BeeRock.Tests;

[TestClass]
public class ConstructorLineModifierTest {

    [TestMethod]
    public void Test_that_lines_not_matched_are_unmodified() {
        var line = @"[Microsoft.AspNetCore.Mvc.HttpPost, Microsoft.AspNetCore.Mvc.Route(""pet"")]";

        var expected = line;

        var m = new ConstructorLineModifier();
        Assert.IsFalse(m.CanModify(line, 0));

    }


    [TestMethod]
    public void Test_that_named_controller_is_matched() {
        var line = "public PetStoreController(IPetStoreController implementation)";

        var m = new ConstructorLineModifier();
        Assert.IsTrue(m.CanModify(line, 0));
        Assert.AreEqual("PetStore", m.ClassName);

        var expected = "public PetStoreController()";
        var newLine = m.Modify();
        Assert.AreEqual(expected, newLine);
    }

    [TestMethod]
    public void Test_that_unnamed_controller_is_matched() {
        var line = "public Controller(IController implementation)";

        var m = new ConstructorLineModifier();
        Assert.IsTrue(m.CanModify(line, 0));
        Assert.AreEqual("", m.ClassName);

        var expected = "public Controller()";
        var newLine = m.Modify();
        Assert.AreEqual(expected, newLine);
    }
}
