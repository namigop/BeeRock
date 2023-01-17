namespace BeeRock.Core.Entities;

public static class ScriptingVarUtils {

    public static ParamInfo GetFileRespParamInfo() {
        var t = default(ScriptingVarBee);
        var p = new ParamInfo {
            Name = $"{ScriptingVarBee.VarName}.{nameof(t.FileResp)}", //RequestHandler.FileRespKey,
            Type = typeof(ScriptingVarFileResponse),
            TypeName = "File Response",
            DisplayValue = @"
Use ""bee.FileResp"" to return a file in the http response:

Sample usage:
1. return a csv : bee.FileResp.ToCsv(""/path/to/my/file.csv"")
2. return an image : bee.FileResp.ToPng(""/path/to/my/file.png"")
3. return an image : bee.FileResp.ToJpeg(""/path/to/my/file.jpeg"")
4. return a PDF : bee.FileResp.ToPdf(""/path/to/my/file.pdf"")
5. return any file : bee.FileResp.ToAny(""/path/to/my/file"", ""contentType"")
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
Use ""bee.Rmq"" to publish a message to RabbitMq:

Sample usage:
1. publish a message : bee.Rmq.Publish(""hostName"", ""queue"", ""exchange"",   ""routingKey"", ""message"")
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
Use ""bee.Run"" to execute another script.  The script should return
a string that will be sent back in the HTTP response

Sample usage:
1. execute python:  bee.Run.Py(""/path/to/my/file.py"")
"
        };

        return p;
    }
}
