using System.Windows;
using System.Windows.Controls;

namespace LaHuevonadaManager.Services;

public static class SimpleInput
{
    public static string? Show(string title, string label)
    {
        var box = new TextBox { MinWidth = 260 };
        var ok = false;
        var win = Build(title, label, box, () => ok = true);
        win.ShowDialog();
        return ok ? box.Text : null;
    }

    public static int ShowInt(string title, string label)
    {
        while (true)
        {
            var text = Show(title, label);
            if (text is null) return 0;
            if (int.TryParse(text, out var value) && value >= 0) return value;
            MessageBox.Show("Ingrese un número entero válido.");
        }
    }

    public static decimal ShowDecimal(string title, string label)
    {
        while (true)
        {
            var text = Show(title, label);
            if (text is null) return 0;
            if (decimal.TryParse(text, out var value) && value >= 0) return value;
            MessageBox.Show("Ingrese un valor válido.");
        }
    }

    private static Window Build(string title, string label, Control input, Action onOk)
    {
        var win = new Window { Title = title, Width = 390, Height = 185, WindowStartupLocation = WindowStartupLocation.CenterScreen, ResizeMode = ResizeMode.NoResize };
        var panel = new StackPanel { Margin = new Thickness(18) };
        panel.Children.Add(new TextBlock { Text = label, Margin = new Thickness(0,0,0,8), FontWeight = FontWeights.SemiBold });
        panel.Children.Add(input);
        var buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0,16,0,0) };
        var cancel = new Button { Content = "Cancelar", MinWidth = 80 };
        var accept = new Button { Content = "Aceptar", MinWidth = 80 };
        cancel.Click += (_, _) => win.Close();
        accept.Click += (_, _) => { onOk(); win.Close(); };
        buttons.Children.Add(cancel); buttons.Children.Add(accept); panel.Children.Add(buttons); win.Content = panel; return win;
    }
}
