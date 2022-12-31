using System.Text;

namespace BeeRock.Core.Entities;

/// <summary>
///     Used to overwrite Console.Out so that we can get the log messages
///     of asp.net core
/// </summary>
public class ConsoleIntercept : TextWriter {
    private const int Capacity = 100_000;
    private static readonly object Key = new();
    private readonly StringBuilder _sb = new();


    public override Encoding Encoding { get; } = Encoding.UTF8;

    public override void Write(char value) {
        lock (Key) {
            if (_sb.Length > Capacity) _sb.Clear();

            if (value == (char)27) {
                //ESC char
                _sb.Append(' ');
                return;
            }

            _sb.Append(value);
        }
    }

    /// <summary>
    ///     Read the buffer and clear it so that it doesnt grow too large
    /// </summary>
    public string Read() {
        lock (Key) {
            var s = _sb.ToString();
            _sb.Clear();
            return s;
        }
    }
}
