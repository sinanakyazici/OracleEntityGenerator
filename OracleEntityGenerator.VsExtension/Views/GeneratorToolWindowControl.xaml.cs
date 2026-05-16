using System.ComponentModel;
using System.Windows.Controls;
using OracleEntityGenerator.VsExtension.ViewModels;

namespace OracleEntityGenerator.VsExtension.Views;

public partial class GeneratorToolWindowControl : System.Windows.Controls.UserControl
{
    public GeneratorToolWindowControl()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        DataContext = new GeneratorToolWindowViewModel();
    }

    private void PasswordBox_OnPasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is GeneratorToolWindowViewModel viewModel
            && sender is PasswordBox passwordBox)
        {
            viewModel.Password = passwordBox.Password;
        }
    }

    private void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is INotifyPropertyChanged oldViewModel)
        {
            oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (e.NewValue is INotifyPropertyChanged newViewModel)
        {
            newViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(GeneratorToolWindowViewModel.Password)
            || DataContext is not GeneratorToolWindowViewModel viewModel
            || PasswordInput.Password == viewModel.Password)
        {
            return;
        }

        PasswordInput.Password = viewModel.Password;
    }
}
