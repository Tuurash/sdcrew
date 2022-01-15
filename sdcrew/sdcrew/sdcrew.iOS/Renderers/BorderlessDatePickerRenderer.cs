using System;
using System.ComponentModel;

using sdcrew.iOS.Renderers;
using sdcrew.Views.ViewHelpers.Renderers;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BorderlessDatePicker), typeof(BorderlessDatePickerRenderer))]
namespace sdcrew.iOS.Renderers
{
    public class BorderlessDatePickerRenderer : DatePickerRenderer
    {
        public static void Init() { }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            //Control.Layer.BorderWidth = 0;
            Control.BorderStyle = UITextBorderStyle.None;
        }
    }
}
