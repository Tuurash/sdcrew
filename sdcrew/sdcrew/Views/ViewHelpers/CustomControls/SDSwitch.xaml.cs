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

            //Comment out Later

            //SwitchPanUpdate += (sender, e) =>
            //{
            //    Color fromColorGradient1 = IsToggled ? Color.FromHex("#B1E3F2") : Color.FromHex("#808080");
            //    Color toColorGradient1 = IsToggled ? Color.FromHex("#808080") : Color.FromHex("#B1E3F2");

            //    Color fromColorGradient2 = IsToggled ? Color.FromHex("#B1E3F2") : Color.FromHex("#808080");
            //    Color toColorGradient2 = IsToggled ? Color.FromHex("#808080") : Color.FromHex("#B1E3F2");

            //    double t = e.Percentage * 0.01;

            //    KnobBackground = new LinearGradientBrush(new GradientStopCollection
            //    {
            //        new GradientStop
            //        {
            //            Color =  ColorAnimationUtil.ColorAnimation(fromColorGradient1, toColorGradient1, t),
            //            Offset = 0
            //        },
            //        new GradientStop
            //        {
            //            Color = ColorAnimationUtil.ColorAnimation(fromColorGradient2, toColorGradient2, t),
            //            Offset = 1
            //        }
            //    }, new Point(0.6, 1), new Point(1, 0));
            //};
        }
    }
}
