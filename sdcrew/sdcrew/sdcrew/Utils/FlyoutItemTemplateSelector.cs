using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace sdcrew.Utils
{
    public class FlyoutItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NavigationHeaderTemplate { get; set; }
        public DataTemplate NavigationItemTemplate { get; set; }


        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is ShellGroupItem && ((ShellGroupItem)item).Route.Contains("Header"))
            {
                // Make sure a header item is not clickable.
                ((ShellGroupItem)item).IsEnabled = false;
                return NavigationHeaderTemplate;
            }
            else
                return NavigationItemTemplate;
        }
    }
}
