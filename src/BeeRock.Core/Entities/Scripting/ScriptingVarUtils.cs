namespace BeeRock.Core.Entities.Scripting;

public static class ScriptingVarUtils {
    public static ParamInfo GetFileRespParamInfo() {
        var t = default(ScriptingVarBee);
        var p = new ParamInfo {
            Name = $"{ScriptingVarBee.VarName}.{nameof(t.FileResp)}", //RequestHandler.FileRespKey,
            Type = typeof(ScriptingVarFileResponse),
            TypeName = "File Response",
            DisplayValue = @"
----------------------------------------------------
Use ""bee.FileResp"" to return a file in the http response:
----------------------------------------------------
Sample usage:
1. return a csv 
   Ex. <<bee.FileResp.ToCsv(""/path/to/my/file.csv"")>>
2. return an image 
   Ex. <<bee.FileResp.ToPng(""/path/to/my/file.png"")>>
3. return an image 
   Ex. <<bee.FileResp.ToJpeg(""/path/to/my/file.jpeg"")>>
4. return a PDF 
   Ex. <<bee.FileResp.ToPdf(""/path/to/my/file.pdf"")>>
5. return any file 
   Ex. <<bee.FileResp.ToAny(""/path/to/my/file"", ""contentType"")>>
"
        };

        return p;
    }

    public static ParamInfo GetQueryStringParamInfo() {
        var p = new ParamInfo {
            Name = $"{RequestHandler.QueryStringKey}",
            Type = typeof(ScriptingQueryString),
            TypeName = "QueryString Parameters",
            DisplayValue = @"
----------------------------------------------------
Use ""queryString"" to get query parameters in the URL
----------------------------------------------------
Sample usage:
1. Get a query parameter from URL. Ex : http://localhost/v1?key=value
   Ex. <<queryString.Get(""key"")>>
"
        };

        return p;
    }

    public static ParamInfo GetLogParamInfo() {
        var t = default(ScriptingVarBee);
        var p = new ParamInfo {
            Name = $"{ScriptingVarBee.VarName}.{nameof(t.Log)}",
            Type = typeof(ScriptingLog),
            TypeName = "Logger",
            DisplayValue = @"
----------------------------------------------------
Use ""bee.Log"" to log a message from the script
----------------------------------------------------
Sample usage:
1. Log an INFO message
   Ex. <<bee.Log.Info(""msg"")>>
2. Log an WARNING message
   Ex. <<bee.Log.Warn(""msg"")>>
3. Log an ERROR message
   Ex. <<bee.Log.ERROR(""msg"")>> 
"
        };

        return p;
    }

    public static ParamInfo GetRmqParamInfo() {
        var t = default(ScriptingVarBee);
        var p = new ParamInfo {
            Name = $"{ScriptingVarBee.VarName}.{nameof(t.Rmq)}",
            Type = typeof(ScriptingVarRmq),
            TypeName = "RabbitMQ client",
            DisplayValue = @"
----------------------------------------------------
Use ""bee.Rmq"" to publish a message to RabbitMq:
----------------------------------------------------
Sample usage:
1. publish a message  
   usage:     <<bee.Rmq.Publish(""uri"", ""queue"", ""exchange"",   ""routingKey"", ""message"")>>
   example :  <<bee.Rmq.Publish(""amqp://username:password@localhost:5672/virtualHost"", ""myQueue"", ""myExchange"",   ""myRoutingKey"", ""message"")>>
"
        };

        return p;
    }


    public static ParamInfo GetContextParamInfo() {
        var t = default(ScriptingVarBee);
        var p = new ParamInfo {
            Name = $"{ScriptingVarBee.VarName}.{nameof(t.Context)}",
            Type = typeof(ScriptingVarContext),
            TypeName = "Call context",
            DisplayValue = @"
----------------------------------------------------
Use ""bee.Context"" to manually control the http response
----------------------------------------------------
Sample usage:
1. Ignore the swagger schema and manually set the response  
   Ex. Add this line to the top of the file, below the response
       <<bee.Context.Response.SetAsPassThrough(True)>>
       {
           enter your json response here
       }
2. Ignore the swagger schema and manually set the response, content type and status code  
   Ex. Add this lined to the top of the file
       <<bee.Context.Response.SetAsPassThrough()>>
       <<bee.Context.Response.SetContentType(""text/plain"")>>
       <<bee.Context.Response.SetStatusCode(401)>>
       {Enter your response text here}
"
        };

        return p;
    }

    public static ParamInfo GetHeadersParamInfo() {
        var p = new ParamInfo {
            Name = RequestHandler.HeaderKey,
            Type = typeof(Dictionary<string, object>),
            TypeName = "Http request/response headers",
            DisplayValue = @"
----------------------------------------------------
Use ""headers"" to access the request or response headers
----------------------------------------------------
Sample usage:
1. Get a header
   Ex. <<headers.Request.Get(""key"")>>
   Ex. <<headers.Response.Get(""key"")>>
2. Add or overwrite a header
   Ex. <<headers.Request.Add(""key"", ""value"")>>
   Ex. <<headers.Response.Add(""key"", ""value"")>>
3. Remove a header
   Ex. <<headers.Request.Remove(""key"")>>
   Ex. <<headers.Response.Remove(""key"")>>
"
        };
        return p;
    }

    public static ParamInfo GetProxyParamInfo() {
        var t = default(ScriptingVarBee);
        var p = new ParamInfo {
            Name = $"{ScriptingVarBee.VarName}.{nameof(t.Proxy)}",
            Type = typeof(ScriptingVarProxy),
            TypeName = "Proxy client",
            DisplayValue = @"
----------------------------------------------------
Use ""bee.Proxy"" to forward the http request to another server or url:
----------------------------------------------------
Sample usage:
1. Forward to another server. The target endpoint should match exactly the mocked endpoint
   syntax: bee.Proxy.ForwardToServer(""baseUrl"")
   Ex.  bee.Proxy.ForwardToServer(""https://petstore.swagger.io"")

2. Forward to another url
   syntax: bee.Proxy.ForwardToUrl(""targetFullUrl"")
   Ex.  bee.Proxy.ForwardToUrl(""https://petstore.swagger.io/v2/pet/"" + str(petId))
"
        };

        return p;
    }

    public static ParamInfo GetRunParamInfo() {
        var t = default(ScriptingVarBee);
        var p = new ParamInfo {
            Name = $"{ScriptingVarBee.VarName}.{nameof(t.Run)}", //RequestHandler.FileRespKey,
            Type = typeof(ScriptingVarRun),
            TypeName = "Script runner",
            DisplayValue = @"
----------------------------------------------------
Use ""bee.Run"" to execute another script.  The script should 
return a string that will be sent back in the HTTP response
----------------------------------------------------

Sample usage:
1. execute python script:  
   Ex. bee.Run.Py(""/path/to/my/file.py"")
"
        };

        return p;
    }
}
