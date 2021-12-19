using DocumentFormat.OpenXml.Drawing;
using Plugin.Settings;
using sdcrew.Services;
using sdcrew.Views.ViewHelpers.Themes;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace sdcrew.Views.ViewHelpers
{
    public class ThemeManager
    {
        //public static void ChangeTheme(string themeName)
        //{
        //    Application.Current.Resources.Clear();
        //    Application.Current.Resources.MergedDictionaries.Clear();
        //    var type = typeof(ThemeManager);
        //    var uri = $"{type.Assembly.GetName().Name}.Views.ViewHelpers.Themes.{themeName}.xaml";

        //    ResourceDictionary myResourceDictionary = new ResourceDictionary();

        //    myResourceDictionary.Source = new Uri(uri, UriKind.Relative);

        //    var theme = Type.GetType(uri);

        //    Application.Current.Resources.MergedDictionaries.Add(myResourceDictionary);
        //}


        public enum Theme
        {
            Default,
            Blue
        }

        /// <summary>
        /// Changes the theme of the app.
        /// Add additional switch cases for more themes you add to the app.
        /// This also updates the local key storage value for the selected theme.
        /// </summary>
        /// <param name="theme"></param>
        public static void ChangeTheme(Theme theme)
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();

                //Update local key value with the new theme you select.
                CrossSettings.Current.AddOrUpdateValue("SelectedTheme", (int)theme);

                switch (theme)
                {
                    case Theme.Default:
                        {
                            mergedDictionaries.Add(new defaultTheme());
                            break;
                        }
                    case Theme.Blue:
                        {
                            mergedDictionaries.Add(new blueTheme());
                            break;
                        }
                    default:
                        mergedDictionaries.Add(new defaultTheme());
                        break;
                }
            }
        }

        public static void LoadTheme()
        {
            Theme currentTheme = CurrentTheme();
            ChangeTheme(currentTheme);

            
        }

        /// <summary>
        /// Gives current/last selected theme from the local storage.
        /// </summary>
        /// <returns></returns>
        public static Theme CurrentTheme()
        {
            return (Theme)CrossSettings.Current.GetValueOrDefault("SelectedTheme", (int)Theme.Default);
        }


    }
}


