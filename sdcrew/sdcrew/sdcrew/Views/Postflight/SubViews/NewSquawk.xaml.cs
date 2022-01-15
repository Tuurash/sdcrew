using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace sdcrew.Views.Postflight.SubViews
{
    public partial class NewSquawk : ContentView
    {
        public NewSquawk()
        {
            InitializeComponent();
        }

        public NewSquawk(int squawkGridRow)
        {
            InitializeComponent();
        }

        void EditSquawk_Tapped(System.Object sender, System.EventArgs e)
        {
            if (editSquawkForm.IsVisible == false)
            {
                editSquawkForm.IsVisible = true;
            }
            else
            {
                editSquawkForm.IsVisible = false;
            }

            MessagingCenter.Send<App>((App)Application.Current, "SquawkUpdated");
        }
    }
}
