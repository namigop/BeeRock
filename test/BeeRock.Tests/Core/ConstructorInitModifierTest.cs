using BeeRock.Core.Entities.CodeGen;

namespace BeeRock.Tests.Core;

[TestClass]
public class ConstructorInitModifierTest {
    [TestMethod]
    public void Test_that_constructor_init_is_matched() {
        var line = @"   _implementation = implementation;";

        var m = new ConstructorInitModifier();
        Assert.IsTrue(m.CanModify(line, 0));

        var newLine = m.Modify();
        Assert.IsTrue(string.IsNullOrWhiteSpace(newLine));
    }

    [TestMethod]
    public void Test_that_lines_not_matched_are_unmodified() {
        var line = @"[Microsoft.AspNetCore.Mvc.HttpPost, Microsoft.AspNetCore.Mvc.Route(""pet"")]";

        var expected = line;

        var m = new ConstructorInitModifier();
        Assert.IsFalse(m.CanModify(line, 0));
    }
}
