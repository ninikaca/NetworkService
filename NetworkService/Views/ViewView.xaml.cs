using NetworkService.ViewModel;
using System.Windows.Controls;

namespace NetworkService.Views
{
    /// <summary>
    /// Interaction logic for ViewView.xaml
    /// </summary>
    public partial class ViewView : UserControl
    {
        public static UserControl UserControl { get; set; }
        public ViewView()
        {
            InitializeComponent();

            UserControl = this;

            DataContext = new ViewViewModel(rightGridOnCanvas);
        }
    }
}
