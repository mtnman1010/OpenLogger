using System.Windows;
using System.Windows.Threading;
using OpenLogger.Sample.MVVM.ViewModels;

namespace OpenLogger.Sample.MVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window, IViewService
    {
        private readonly Dispatcher dispatcher;
        private readonly ViewModel mainViewModel;

        public MainView(Dispatcher dispatcher, ViewModel mainViewModel)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.dispatcher = dispatcher;
            this.mainViewModel = mainViewModel;
            this.mainViewModel.ViewService = this;
            this.mainViewModel.Dispatcher = dispatcher;

            DataContext = mainViewModel;

            SetBinding(TitleProperty, "Title");

            InitializeComponent();
        }
    }
}
