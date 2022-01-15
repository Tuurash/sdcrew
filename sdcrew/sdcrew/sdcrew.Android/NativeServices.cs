using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using sdcrew.Services;
using sdcrew.Views.ViewHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace sdcrew.Droid
{
    public class NativeServices : INativeServices
    {

        public void OnThemeChanged(ThemeManager.Theme theme)
        {
            MainActivity activity = MainActivity.Instance;
            var intent = MainActivity.Instance.Intent;

            activity.Finish();
            activity.StartActivity(intent);

        }
    }
}