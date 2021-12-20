using System.Diagnostics.CodeAnalysis;

using Switch;
using Switch.Helpers;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.ViewHelpers.CustomControls
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SDSwitch : CustomSwitch
    {
        public SDSwitch()
        {
            InitializeComponent();

            SwitchPanUpdate += (sender, e) =>
            {
                Color fromBackgroundColor = IsToggled ? Color.FromHex("#80D1EA") : Color.FromHex("#797D7F");
                Color toBackgroundColor = IsToggled ? Color.FromHex("#797D7F") : Color.FromHex("#80D1EA");

                double t = e.Percentage * 0.01;

                Flex.TranslationX = -(e.TranslateX + e.XRef);
                if (IsToggled)
                {
                    if (e.Percentage >= 50)
                    {
                        MoonImg.Opacity = ((e.Percentage - 50) * 2) * 0.01;
                    }
                }
                else
                {
                    if (e.Percentage <= 50)
                    {
                        MoonImg.Opacity = (100 - (e.Percentage * 2)) * 0.01;
                    }
                }

                SDBorder.BorderColor = ColorAnimationUtil.ColorAnimation(fromBackgroundColor, toBackgroundColor, t);
            };
        }
    }
}
