using sdcrew.Droid.Renderers;
using sdcrew.Views.ViewHelpers.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


using Android.Content;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(DashedFrame), typeof(DashedFrameRenderer))]
namespace sdcrew.Droid.Renderers
{
    public class DashedFrameRenderer : Xamarin.Forms.Platform.Android.AppCompat.FrameRenderer
    {
        public DashedFrameRenderer(Context context) : base(context)
        { }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {

            base.OnElementChanged(e);
            DashedFrame customFrame = Element as DashedFrame;
            float r = customFrame.CornerRadius;
            GradientDrawable shape = new GradientDrawable();
            shape.SetCornerRadii(new float[] { r, r, r, r, r, r, r, r });
            shape.SetColor(Android.Graphics.Color.Transparent);
            shape.SetStroke(2, Color.White.ToAndroid(), 15f, 6f);

            Control.SetBackground(shape);
        }
    }
}
