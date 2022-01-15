using System;
using CoreAnimation;
using Foundation;
using sdcrew.iOS.Renderers;
using sdcrew.Views.ViewHelpers.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


[assembly: ExportRenderer(typeof(DashedFrame), typeof(DashedFrameRenderer))]
namespace sdcrew.iOS.Renderers
{
    public class DashedFrameRenderer : FrameRenderer
    {
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CAShapeLayer viewBorder = new CAShapeLayer();
            viewBorder.StrokeColor = UIColor.White.CGColor;
            viewBorder.FillColor = null;
            viewBorder.LineDashPattern = new NSNumber[] { new NSNumber(5), new NSNumber(2) };
            viewBorder.Frame = NativeView.Bounds;
            viewBorder.Path = UIBezierPath.FromRect(NativeView.Bounds).CGPath;

            Layer.BorderColor = Color.Transparent.ToCGColor();
            Layer.AddSublayer(viewBorder);

            // If you don't want the shadow effect
            Element.HasShadow = false;
        }
    }
}
