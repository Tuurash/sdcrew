using sdcrew.Droid.Renderers;
using sdcrew.Views.ViewHelpers.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


using Android.Content;


[assembly: ExportRenderer(typeof(BorderlessEditor), typeof(BorderlessEditorRenderer))]
namespace sdcrew.Droid.Renderers
{
    public class BorderlessEditorRenderer : EditorRenderer
    {
        public BorderlessEditorRenderer(Context context) : base(context)
        { }
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                Control.Background = null;

                var layoutParams = new MarginLayoutParams(Control.LayoutParameters);
                layoutParams.SetMargins(0, 0, 0, 0);
                LayoutParameters = layoutParams;
                Control.LayoutParameters = layoutParams;
                Control.SetPadding(0, 0, 0, 0);
                SetPadding(0, 0, 0, 0);
            }
        }
    }
}
