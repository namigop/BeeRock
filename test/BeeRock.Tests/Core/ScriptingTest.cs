using BeeRock.Core.Entities;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BeeRock.Tests.Core;

[TestClass]
public class ScriptingTest {
    [TestMethod]
    public void Test_that_python_scripts_are_evaluated_correctly() {
        var expression = "username='batman'; return username.upper()";
        var result = PyEngine.Evaluate(expression, null).ToString();
        var expected = "BATMAN";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Test_that_python_expressions_get_an_implicit_return() {
        var expression = "1 + 2";
        var result = PyEngine.Evaluate(expression, null);
        var expected = 3;
        Assert.AreEqual(expected, Convert.ToInt32( result));
    }

    [TestMethod]
    public void Test_that_json_with_scripts_is_evaluated() {
        var scriptVariables = new Dictionary<string, object>() {
            { "username", "i am" },
            { "lastname", "ironman" },
            {"age", 100}
        };

        var json = @"
{
  ""id"": <<answertolife=42; return answertolife;>>,
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
        var result = ScriptedJson.Evaluate(json, scriptVariables);
        Assert.AreEqual(expected, result);
    }
}
