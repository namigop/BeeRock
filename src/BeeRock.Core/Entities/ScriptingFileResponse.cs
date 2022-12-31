using Microsoft.AspNetCore.Mvc;

namespace BeeRock.Core.Entities;

//this class will be used in the python script.
public class ScriptingFileResponse {
    public FileContentResult ToCsv(string file) {
        return ToAny(file, "text/csv");
    }
    public FileContentResult ToPng(string file) {
        return ToAny(file, "image/png");
    }
    public FileContentResult ToJpeg(string file) {
        return ToAny(file, "image/jpeg");
    }
    public FileContentResult ToPdf(string file) {
        return ToAny(file, "application/pdf");
    } 
    public FileContentResult ToAny(string file, string contentType) {
        if (!File.Exists(file))
            throw new FileNotFoundException("Missing file", file);
        return new FileContentResult(File.ReadAllBytes(file), contentType);
    }
}
