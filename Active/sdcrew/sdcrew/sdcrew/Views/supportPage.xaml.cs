using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class supportPage : ContentPage
    {
        public supportPage()
        {
            this.Title = "Support";
            InitializeComponent();
        }
    }
}