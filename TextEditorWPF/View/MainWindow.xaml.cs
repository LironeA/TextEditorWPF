using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TextEditorWPF.ViewModel;

namespace TextEditorWPF.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _vm { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            _vm = App.Current.Services.GetService<MainWindowViewModel>();
            _vm.Canvas = canvas;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //_vm.TextChanged(((TextBox)sender).Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _vm.TextChanged(textBox.Text);
        }
    }
}