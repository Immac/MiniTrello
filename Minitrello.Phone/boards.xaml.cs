using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace Minitrello.Phone
{
    public partial class Boards : PhoneApplicationPage
    {
        public Boards()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.ViewModel.LoadBoards();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void MainLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Do Something?
        }
    }
}