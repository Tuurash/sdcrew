using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace sdcrew.Services
{
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string SettingsKey = "settings_key";
        private static readonly string SettingsDefault = string.Empty;

        private const string AccessTokenKey = "access_key";
        private static readonly string AccessTokenVal = string.Empty;


        private const string imageKey = "image_key";
        private static readonly string imagePath = string.Empty;

        private const string AuthidentifierUrlkey = "AuthidentifierUrl_key";
        private static readonly string AuthidentifierUrlval = string.Empty;

        private const string filterItemskey = "filterItems_key";
        private static readonly string filterItemsVal = string.Empty;

        private const string timeZonekey = "timezone_key";
        private static readonly string timeZoneVal = string.Empty;

        private const string RefreshTimeKey = "Refresh_Key";
        private static readonly string RefreshTimeVal = string.Empty;

        private const string UserEmail = "userMail_key";
        private static readonly string userEmailVal = string.Empty;

        #endregion

        public static string GetUserMail
        {
            get
            {
                return AppSettings.GetValueOrDefault(UserEmail, userEmailVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(UserEmail, value);
            }
        }

        public static string GetRefreshTime
        {
            get
            {
                return AppSettings.GetValueOrDefault(RefreshTimeKey, RefreshTimeVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(RefreshTimeKey, value);
            }
        }

        public static string GeneralSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(SettingsKey, value);
            }
        }

        public static string StoreImage
        {
            get
            {
                return AppSettings.GetValueOrDefault(imageKey, imagePath);
            }
            set
            {
                AppSettings.AddOrUpdateValue(imageKey, value);
            }
        }

        public static string StoreAccessToken
        {
            get
            {
                return AppSettings.GetValueOrDefault(AccessTokenKey, AccessTokenVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AccessTokenKey, value);
            }
        }

        public static string StoreAuthidentifierUrl
        {
            get
            {
                return AppSettings.GetValueOrDefault(AuthidentifierUrlkey, AuthidentifierUrlval);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AuthidentifierUrlkey, value);
            }
        }

        public static string FilterItems
        {
            get
            {
                return AppSettings.GetValueOrDefault(filterItemskey, filterItemsVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(filterItemskey, value);
            }
        }

        public static string TimeZone
        {
            get
            {
                return AppSettings.GetValueOrDefault(timeZonekey, timeZoneVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(timeZonekey, value);
            }
        }

    }
}

