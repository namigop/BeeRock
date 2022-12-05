using System.Text;

namespace BeeRock.Core.Entities;

public class ConsoleIntercept : TextWriter {
    private static readonly object _key = new();
    private readonly StringBuilder _sb = new();
    private const int Capacity = 100_000;
    public bool Enabled { get; set; }

    public override Encoding Encoding { get; } = Encoding.UTF8;

    public override void Write(char value) {
        if (Enabled) {
            lock (_key) {
                if (_sb.Length > Capacity) {
                    _sb.Clear();
                }

                if (value == (char)27) {
                    _sb.Append(' ');
                    return;
                }

                _sb.Append(value);
            }
        }
    }

    public string Read() {
        if (!Enabled)
            return "";

        lock (_key) {
            var s = _sb.ToString();
            _sb.Clear();
            return s;
        }
    }
}
