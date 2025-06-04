using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EmplStruct;

public partial class MessageBox : Window
{

    public MessageBox()
    {
        InitializeComponent();
    }
    public MessageBox(string message)
    {
        InitializeComponent();
        this.FindControl<TextBlock>("MessageText").Text = message;
        this.FindControl<Button>("OkButton").Click += (_, _) => Close();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}