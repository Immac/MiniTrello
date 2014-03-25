using System;
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
    public partial class Boards : PhoneApplicationPage
    {
        public Boards()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadBoards();
        }

        private void LoadBoards()
        {
            
        }
    }
}