﻿using System.Collections.ObjectModel;
using System.Data.Linq.Mapping;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Minitrello.Phone.ViewModels;

namespace Minitrello.Phone
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        // Constructor
        public DetailsPage()
        {
            InitializeComponent();
            LanesList.ItemsSource = detailModels;
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // When page is navigated to set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string selectedIndex = "";
            if (DataContext == null)
            {
                
                if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
                {
                    int index = int.Parse(selectedIndex);
                    DataContext = App.ViewModel.Items[index];
                    
                }
                
            }
             selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
            {
                int index = int.Parse(selectedIndex);
                App.ViewModel.LoadDetails(index);
                LoadThis();
            }
           
        }

        private void LoadThis()
        {
            var lanes = App.ViewModel.LaneModels;
            
            
            foreach (var laneModel in lanes)
            {
                var content = "";
                content += laneModel.Name;
                content += "\n";
                var myCards = laneModel.Cards;
                foreach (var cardModel in myCards)
                {
                    content += "Card Name: " + cardModel.Name + "\n";
                    content += " " + cardModel.Content + "\n";
                }
                detailModels.Add(new DetailModel(content));

            }
        }
        ObservableCollection<DetailModel> detailModels = new ObservableCollection<DetailModel>();

        private void Pivot_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}

public class DetailModel
{
    public string Content { get; set; }

    public DetailModel(string station)
    {
        Content = station;
    }
}