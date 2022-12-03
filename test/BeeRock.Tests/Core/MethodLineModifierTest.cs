using BeeRock.Core.Entities.CodeGen;
using BeeRock.Core.Utils;

namespace BeeRock.Tests;

[TestClass]
public class MethodLineModifierTest {
    [TestMethod]
    public void Test_that_lines_not_matched_are_unmodified() {
        var line = @"[Microsoft.AspNetCore.Mvc.HttpPost, Microsoft.AspNetCore.Mvc.Route(""pet"")]";

        var expected = line;

        var m = new MethodLineModifier();
        Assert.IsFalse(m.CanModify(line, 0));

    }

    [TestMethod]
    public void Test_that_methodlines_with_return_value_is_matched() {
        var line = "		public System.Threading.Tasks.Task<ApiResponse> UploadFile(long petId, string additionalMetadata, FileParameter file)";
        var lineNumber = 1;
        var m = new MethodLineModifier();
        Assert.IsTrue(m.CanModify(line, 1));
        Assert.AreEqual("UploadFile", m.MethodName);

        var expected = $"		public System.Threading.Tasks.Task<ApiResponse> MUploadFile{lineNumber}(long petId, string additionalMetadata, FileParameter file)";
        var newLine = m.Modify();
        Assert.AreEqual(expected, newLine);
    }

    [TestMethod]
    public void Test_that_methodlines_with_no_return_value_is_matched() {
        var line = "		public System.Threading.Tasks.Task UploadFile(long petId, string additionalMetadata, FileParameter file)";
        var lineNumber = 123;
        var m = new MethodLineModifier();
        Assert.IsTrue(m.CanModify(line, lineNumber));
        Assert.AreEqual("UploadFile", m.MethodName);

        var expected = $"		public System.Threading.Tasks.Task MUploadFile{lineNumber}(long petId, string additionalMetadata, FileParameter file)";
        var newLine = m.Modify();
        Assert.AreEqual(expected, newLine);
    }
}
