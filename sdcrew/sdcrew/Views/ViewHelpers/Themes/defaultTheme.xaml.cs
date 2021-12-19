using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.ViewHelpers.Themes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class defaultTheme : ResourceDictionary
    {
        public defaultTheme()
        {
            InitializeComponent();
        }
    }
}