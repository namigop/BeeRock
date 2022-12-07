using BeeRock.Core.Utils;

namespace BeeRock.Tests.Core;

[TestClass]
public class RequiresTest {

    [TestMethod]
    [ExpectedException(typeof(RequiresException))]
    public void Test_that_Requires_NullChecking_Works() {
        object? o = null;
        Requires.NotNull(o, "somename");
    }


    [TestMethod]
    [ExpectedException(typeof(RequiresException))]
    public void Test_that_Requires_string_NullOrEmptyChecking_Works() {
        string? o = null;
        Requires.NotNullOrEmpty(o, "somename");
    }

    [TestMethod]
    [ExpectedException(typeof(RequiresException))]
    public void Test_that_Requires_string_EmptyChecking_Works() {
        string? o = "";
        Requires.NotNullOrEmpty(o, "somename");
    }

    [TestMethod]
    [ExpectedException(typeof(RequiresException))]
    public void Test_that_Requires_ArrayNotEmpty_Works() {
        string[] o = Array.Empty<string>();
        Requires.NotNullOrEmpty(o, "somearray");
    }
    [TestMethod]
    [ExpectedException(typeof(RequiresException))]
    public void Test_that_Requires_ListNotEmpty_Works() {
        var o = new List<string>();
        Requires.NotNullOrEmpty(o, "somelist");
    }

    [TestMethod]
    [ExpectedException(typeof(RequiresException))]
    public void Test_that_Requires_EnumerableNotEmpty_Works() {
        var o = new List<string>().AsEnumerable();
        Requires.NotNullOrEmpty(o, "somelist");
    }

    [TestMethod]
    [ExpectedException(typeof(RequiresException))]
    public void Test_that_Requires_AssertTrue_Works() {
        Func<bool> test = () => false;
        Requires.IsTrue(test, "sometest");
    }

    [TestMethod]
    [ExpectedException(typeof(RequiresException))]
    public void Test_that_Requires_AssertTrue_with_args_works() {
        Func<string, bool> test = s => false;
        Requires.IsTrue(test, "myarg","sometest");
    }

}
