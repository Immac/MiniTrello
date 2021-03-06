﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Minitrello.Phone
{
    public partial class menu : PhoneApplicationPage
    {
        public menu()
        {
            InitializeComponent();
        }

        private void menuMyBoards_Click(object sender, RoutedEventArgs e)
        {
            if(App.ViewModel.IsLogedIn)
                NavigationService.Navigate(new Uri("/boards.xaml", UriKind.Relative));
            else
            {
                isLoadingBlock.Text = "Wait a couple of seconds and try again...";
            }
        }
    }
}