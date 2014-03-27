using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Minitrello.Phone.ViewModels;

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
            if (BoardsListSelector.SelectedItem == null)
                return;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + (BoardsListSelector.SelectedItem as ItemViewModel).ID, UriKind.Relative));


            // Reset selected item to null (no selection)
            BoardsListSelector.SelectedItem = null;
        }
    }
}