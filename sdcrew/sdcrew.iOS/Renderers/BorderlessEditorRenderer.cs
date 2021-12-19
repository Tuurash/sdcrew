using System.ComponentModel;
using sdcrew.iOS.Renderers;
using sdcrew.Views.ViewHelpers.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BorderlessEditor), typeof(BorderlessEditorRenderer))]
namespace sdcrew.iOS.Renderers
{
    public class BorderlessEditorRenderer : EditorRenderer
    {
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            Control.Layer.BorderWidth = 0;
            
        }
    }
}

