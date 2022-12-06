using System.Text;

namespace BeeRock.Core.Entities;

public class ConsoleIntercept : TextWriter {
    private const int Capacity = 100_000;
    private static readonly object _key = new();
    private readonly StringBuilder _sb = new();


    public override Encoding Encoding { get; } = Encoding.UTF8;

    public override void Write(char value) {
        lock (_key) {
            if (_sb.Length > Capacity) _sb.Clear();

            if (value == (char)27) {
                //ESC char
                _sb.Append(' ');
                return;
            }

            _sb.Append(value);
        }
    }

    public string Read() {
        lock (_key) {
            var s = _sb.ToString();
            _sb.Clear();
            return s;
        }
    }
}