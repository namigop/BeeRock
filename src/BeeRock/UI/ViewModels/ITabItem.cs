using System.Windows.Input;

namespace BeeRock.UI.ViewModels;

public interface ITabItem {
    string Name { get; set; }
    ICommand CloseCommand { get; }
    string TabType { get; }

    string HeaderText { get; }
    public bool IsServiceHost { get; }
}
