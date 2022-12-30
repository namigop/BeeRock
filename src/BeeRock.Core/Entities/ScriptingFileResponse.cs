using Microsoft.AspNetCore.Mvc;

namespace BeeRock.Core.Entities;

public class ScriptingFileResponse {
    public FileContentResult ToCsv(string file) {
        return ToFile(file, "text/csv");
    }
    public FileContentResult ToPng(string file) {
        return ToFile(file, "image/png");
    }
    public FileContentResult ToJpeg(string file) {
        return ToFile(file, "image/jpeg");
    }
    public FileContentResult ToPdf(string file) {
        return ToFile(file, "application/pdf");
    } 
    public FileContentResult ToFile(string file, string contentType) {
        if (!File.Exists(file))
            throw new FileNotFoundException("Missing file", file);
        return new FileContentResult(File.ReadAllBytes(file), contentType);
    }
}