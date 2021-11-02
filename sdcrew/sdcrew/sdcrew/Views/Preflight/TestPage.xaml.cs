using sdcrew.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestPage : ContentPage
    {
        private readonly PreflightServices preflightServices;

        filteredTailNumber filteredTail = new filteredTailNumber();

        public TestPage()
        {
            preflightServices = new PreflightServices();
            InitializeComponent();
            BindingContext = this;
        }

    }

    
}