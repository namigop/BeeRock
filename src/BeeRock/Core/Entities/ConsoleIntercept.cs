using System.Text;

namespace BeeRock.Core.Entities;

public class ConsoleIntercept : TextWriter {
    private StringBuilder sb = new();
    private static object key = new();

    public override void Write(char value) {
        if (Enabled)
            lock (key) {
                if (value == (char)27) {
                    sb.Append(' ');
                    return;
                }

                sb.Append(value);
            }
    }

    public bool Enabled { get; set; }

    public override Encoding Encoding { get; } = Encoding.UTF8;

    public string Read() {
        if (!Enabled)
            return "";

        lock (key) {
            var s = this.sb.ToString();
            sb.Clear();
            return s;
        }
    }
}
