using BeeRock.Core.Entities;
using BeeRock.Core.Entities.Scripting;

namespace BeeRock.Tests.Core;

[TestClass]
public class ScriptingTest {
    [TestMethod]
    public void Test_that_python_scripts_are_evaluated_correctly() {
        var expression = "\"batman\".upper()";
        var result = PyEngine.Evaluate(expression, "", "", null).ToString();
        var expected = "BATMAN";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Test_that_python_expressions_get_an_implicit_return() {
        var expression = "1 + 2";
        var result = PyEngine.Evaluate(expression, "", "", null);
        var expected = 3;
        Assert.AreEqual(expected, Convert.ToInt32(result));
    }

    [TestMethod]
    public void Test_that_python_expressions_ending_in_semicolon_do_not_get_an_implicit_return() {
        var expression = "\"abc\" + \"cde\" ;";
        var result = PyEngine.Evaluate(expression, "", "", null);
        Assert.AreEqual(null, result);
    }


    [TestMethod]
    public void Test_that_json_with_scripts_is_evaluated() {
        var scriptVariables = new Dictionary<string, object> {
            { "username", "i am" },
            { "lastname", "ironman" },
            { "age", 100 }
        };

        var json = @"
{
  ""id"": 42,
  ""username"": ""<<username>>"",
  ""firstName"": ""string"",
  ""lastName"": ""<<lastname.upper()>>"",
  ""email"": ""string"",
  ""password"": ""string"",
  ""phone"": ""string"",
  ""age"": <<age - 50>>
}
";

        var expected = @"
{
  ""id"": 42,
  ""username"": ""i am"",
  ""firstName"": ""string"",
  ""lastName"": ""IRONMAN"",
  ""email"": ""string"",
  ""password"": ""string"",
  ""phone"": ""string"",
  ""age"": 50
}
";
        var swaggerUrl = "http://notneeded";
        var result = ScriptedJson.Evaluate(json, swaggerUrl, "", scriptVariables);
        Assert.AreEqual(expected, result);
    }


    [TestMethod]
    public void Test_that_comments_in_json_scripts_is_ignored() {
        var json = @"
//This is a comment and will be ignored
{
  ""id"": 42,
  ""username"": ""i am"",
//This line will also be ignored
  ""firstName"": ""string"",
  ""lastName"": ""IRONMAN"",
  ""email"": ""string"",
  ""password"": ""string"",
  ""phone"": ""string"",
  ""age"": 50
}
//So is this comment
";


        var expected = @"
{
  ""id"": 42,
  ""username"": ""i am"",
  ""firstName"": ""string"",
  ""lastName"": ""IRONMAN"",
  ""email"": ""string"",
  ""password"": ""string"",
  ""phone"": ""string"",
  ""age"": 50
}
";
        var swaggerUrl = "http://notneeded";
        var result = ScriptedJson.Evaluate(json, swaggerUrl, "", null);
        Assert.AreEqual(expected, result);
    }
}
