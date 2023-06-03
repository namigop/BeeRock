using BeeRock.Core.Entities;

namespace BeeRock.Tests.Core;

[TestClass]
public class ProxyRoutingTest {
    [TestMethod]
    public void Test_path_templates_are_converted_to_regex() {
        var uri = new Uri("https://localhost:80/v1/store/42");
        var pathTemplate = "{version}/store/{id}";
        var (regex, names) = RouteChecker.ConvertToRegex(pathTemplate);
        var expected = "(?<version>.*)/store/(?<id>.*)";
        Assert.AreEqual(expected, regex);
        Assert.AreEqual(2, names.Length);
        Assert.IsTrue(names.Contains("version"));
        Assert.IsTrue(names.Contains("id"));

        var (match, _) = RouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
        Assert.AreEqual(true, match.Success);

        pathTemplate = "{version}/{store}/{id}";
        var (regex2, _) = RouteChecker.ConvertToRegex(pathTemplate);
        expected = "(?<version>.*)/(?<store>.*)/(?<id>.*)";
        Assert.AreEqual(expected, regex2);

        var (match2, _) = RouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
        Assert.AreEqual(true, match2.Success);
    }

    [TestMethod]
    public void Test_fixed_path_templates_are_not_converted_to_regex() {
        var pathTemplate = "v1/abc/def/pet/123";
        var (regex, _) = RouteChecker.ConvertToRegex(pathTemplate);
        var expected = pathTemplate;
        Assert.AreEqual(expected, regex);
    }

    [TestMethod]
    public void Test_fixed_path_templates_are_matched() {

        var pathTemplate = "v1/abc/def/pet/123";

        //Exact Match
        var uri = new Uri("https://localhost:80/v1/abc/def/pet/123");
        var (match, _) = RouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
        Assert.AreEqual(true, match.Success);

        //partial match will also fail
        uri = new Uri("https://localhost:80/v1/abc/def/pet/123/456/cde");
        var (match2, _) = RouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
        Assert.AreEqual(false, match2.Success);

        //non-match of course fails
        uri = new Uri("https://localhost:80/v1/abc/thsiswillfaule/pet/123");
        var (match3, _) = RouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
        Assert.AreEqual(false, match3.Success);
    }

    [TestMethod]
    public void Test_path_templates_are_matched() {
        var uri = new Uri("https://localhost:80/v1/store/42");
        var pathTemplate = "{version}/store/{id}";

        var (match, _) = RouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
        Assert.AreEqual(true, match.Success);

        Assert.AreEqual("v1", match.Groups["version"].Value);
        Assert.AreEqual("42", match.Groups["id"].Value);

        pathTemplate = "{version}/{store123}/{id}";
        var (match2, _) = RouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
        Assert.AreEqual(true, match2.Success);
        Assert.AreEqual("v1", match2.Groups["version"].Value);
        Assert.AreEqual("store", match2.Groups["store123"].Value);
        Assert.AreEqual("42", match2.Groups["id"].Value);

        uri = new Uri("https://localhost:80/v1/thisisadifferent/def/42");
        var (match3, _) = RouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
        Assert.AreEqual(true, match3.Success);
        Assert.AreEqual("v1/thisisadifferent", match3.Groups["version"].Value); //greedy matching
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Test_path_templates_with_dups_are_not_allowed() {
        var uri = new Uri("https://localhost:80/v1/store/42");
        var pathTemplate = "{version}/{version}/{id}";

        //will throw an exception
        var (match, _) = RouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
    }
}
