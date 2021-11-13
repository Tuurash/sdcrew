using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Plugin.Segmented.Control.iOS;
using UIKit;
using Xamarin.Forms;

namespace sdcrew.iOS
{

    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {

        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            //global::Xamarin.Forms.FormsMaterial.Init();

            // Lets log them to our output window.
            LogFonts();

            SegmentedControlRenderer.Initialize();
            DependencyService.Register<ASWebAuthenticationSessionBrowser>();



            Rg.Plugins.Popup.Popup.Init();
            LoadApplication(new App());


            Plugin.InputKit.Platforms.iOS.Config.Init();
            return base.FinishedLaunching(app, options);
        }

        private void LogFonts()
        {
            foreach (NSString family in UIFont.FamilyNames)
            {
                foreach (NSString font in UIFont.FontNamesForFamilyName(family))
                {
                    Console.WriteLine(@"{0}", font);
                }
            }
        }

    }
}
