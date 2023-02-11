using BeeRock.Core.Entities;

namespace BeeRock.Tests.Core;

[TestClass]
public class ProxyRoutingTest {
    [TestMethod]
    public void Test_path_templates_are_converted_to_regex() {
        var uri = new Uri("https://localhost:80/v1/store/42");
        var pathTemplate = "{version}/store/{id}";
        var regex = ProxyRouteChecker.ConvertToRegex(pathTemplate);
        var expected = "(?<version>.*)/store/(?<id>.*)";
        Assert.AreEqual(expected, regex);

        var match = ProxyRouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
        Assert.AreEqual(true, match.Success);

        pathTemplate = "{version}/{store}/{id}";
        regex = ProxyRouteChecker.ConvertToRegex(pathTemplate);
        expected = "(?<version>.*)/(?<store>.*)/(?<id>.*)";
        Assert.AreEqual(expected, regex);

        match = ProxyRouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
        Assert.AreEqual(true, match.Success);
    }

    [TestMethod]
    public void Test_fixed_path_templates_are_not_converted_to_regex() {
        var pathTemplate = "v1/abc/def/pet/123";
        var regex = ProxyRouteChecker.ConvertToRegex(pathTemplate);
        var expected = pathTemplate;
        Assert.AreEqual(expected, regex);
    }

    [TestMethod]
    public void Test_fixed_path_templates_are_matched() {
        var uri = new Uri("https://localhost:80/v1/abc/def/pet/123");
        var pathTemplate = "v1/abc/def/pet/123";
        var match = ProxyRouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
        Assert.AreEqual(true, match.Success);

        uri = new Uri("https://localhost:80/v1/abc/def/pet/123/456/cde");
        match = ProxyRouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
        Assert.AreEqual(true, match.Success);

        uri = new Uri("https://localhost:80/v1/abc/thsiswillfaule/pet/123");
        match = ProxyRouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
        Assert.AreEqual(false, match.Success);
    }

    [TestMethod]
    public void Test_path_templates_are_matched() {
        var uri = new Uri("https://localhost:80/v1/store/42");
        var pathTemplate = "{version}/store/{id}";

        var match = ProxyRouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
        Assert.AreEqual(true, match.Success);

        Assert.AreEqual("v1", match.Groups["version"].Value);
        Assert.AreEqual("42", match.Groups["id"].Value);

        pathTemplate = "{version}/{store123}/{id}";
        match = ProxyRouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
        Assert.AreEqual(true, match.Success);
        Assert.AreEqual("v1", match.Groups["version"].Value);
        Assert.AreEqual("store", match.Groups["store123"].Value);
        Assert.AreEqual("42", match.Groups["id"].Value);

        uri = new Uri("https://localhost:80/v1/thisisadifferent/def/42");
        match = ProxyRouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
        Assert.AreEqual(true, match.Success);
        Assert.AreEqual("v1/thisisadifferent", match.Groups["version"].Value); //greedy matching
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Test_path_templates_with_dups_are_not_allowed() {
        var uri = new Uri("https://localhost:80/v1/store/42");
        var pathTemplate = "{version}/{version}/{id}";

        //will throw an exception
        var match = ProxyRouteChecker.Match(uri, new ProxyRoute { From = new ProxyRoutePart { PathTemplate = pathTemplate } });
    }
}