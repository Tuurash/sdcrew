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

        private const string AccessTokenExpiryKey = "AccesstokenExpiry";
        private static readonly string AccessTokenExpiryVal = string.Empty;

        private const string setAirportLocalTimeBoolKey = "AirportLocalTimeBool";
        private static readonly string setAirportLocalTimeBoolKeyVal = string.Empty;

        private const string userSecretsKey = "userSecrets";
        private static readonly string userSecretsKeyVal = string.Empty;

        private const string AppFirstloadflagkey = "FirstLoadkey";
        private static readonly string AppFirstloadflagVal = string.Empty;

        private const string RefreshtimerKey = "RefreshtimerKey";
        private static readonly string RefreshtimerVal = string.Empty;

        private const string PreflightPastDayskey = "PreflightSartDatekey";
        private static readonly string PreflightPastDaysVal = string.Empty;

        private const string PreflightFutureDayskey = "PreflightEndDatekey";
        private static readonly string PreflightFutureDaysVal = string.Empty;

        private const string PostflightPastEventsKey = "PostflightPasteventsKey";
        private static readonly string PostflightPastEventsVal = string.Empty;

        private const string PreflightSubviewVisibilityKey = "PreflightSubviewVisibilityKey";
        private static readonly string PreflightSubviewVisibilityVal = string.Empty;

        private const string PostflightSubviewVisibilityKey = "PostflightSubviewVisibilityKey";
        private static readonly string PostflightSubviewVisibilityVal = string.Empty;

        private const string PostflightSubNavItemsKey = "PostflightSubNavItemsKey";
        private static readonly string PostflightSubNavItemsVal = string.Empty;

        private const string UpdateNotificationKey = "UpdateNotificationKey";
        private static readonly string UpdateNotificationVal = string.Empty;


        private const string HasLocalUpdateKey = "UpdateNotificationKey";
        private static readonly string HasLocalUpdateVal = string.Empty;

        #endregion

        public static string HasLocalUpdates
        {
            get
            {
                return AppSettings.GetValueOrDefault(HasLocalUpdateKey, HasLocalUpdateVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(HasLocalUpdateKey, value);
            }
        }

        public static string UpdateNotification
        {
            get
            {
                return AppSettings.GetValueOrDefault(UpdateNotificationKey, UpdateNotificationVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(UpdateNotificationKey, value);
            }
        }

        public static string Refreshtimer
        {
            get
            {
                return AppSettings.GetValueOrDefault(RefreshtimerKey, RefreshtimerVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(RefreshtimerKey, value);
            }
        }

        public static string PostflightSubNavItems
        {
            get
            {
                return AppSettings.GetValueOrDefault(PostflightSubNavItemsKey, PostflightSubNavItemsVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(PostflightSubNavItemsKey, value);
            }
        }

        public static string PostflightSubviewVisibility
        {
            get
            {
                return AppSettings.GetValueOrDefault(PostflightSubviewVisibilityKey, PostflightSubviewVisibilityVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(PostflightSubviewVisibilityKey, value);
            }
        }

        public static string PreflightSubviewVisibility
        {
            get
            {
                return AppSettings.GetValueOrDefault(PreflightSubviewVisibilityKey, PreflightSubviewVisibilityVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(PreflightSubviewVisibilityKey, value);
            }
        }

        public static string PostflightPastDays
        {
            get
            {
                return AppSettings.GetValueOrDefault(PostflightPastEventsKey, PostflightPastEventsVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(PostflightPastEventsKey, value);
            }
        }


        public static string PreflightFutureDays
        {
            get
            {
                return AppSettings.GetValueOrDefault(PreflightFutureDayskey, PreflightFutureDaysVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(PreflightFutureDayskey, value);
            }
        }

        public static string PreflightPastDays
        {
            get
            {
                return AppSettings.GetValueOrDefault(PreflightPastDayskey, PreflightPastDaysVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(PreflightPastDayskey, value);
            }
        }

        public static string IsAppFirstLoaded
        {
            get
            {
                return AppSettings.GetValueOrDefault(AppFirstloadflagkey, AppFirstloadflagVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AppFirstloadflagkey, value);
            }
        }

        public static string UserSecrets
        {
            get
            {
                return AppSettings.GetValueOrDefault(userSecretsKey, userSecretsKeyVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(userSecretsKey, value);
            }
        }

        public static string setAirportLocalTimeBool
        {
            get
            {
                return AppSettings.GetValueOrDefault(setAirportLocalTimeBoolKey, setAirportLocalTimeBoolKeyVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(setAirportLocalTimeBoolKey, value);
            }
        }

        public static string AccessTokenExpiry
        {
            get
            {
                return AppSettings.GetValueOrDefault(AccessTokenExpiryKey, AccessTokenExpiryVal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AccessTokenExpiryKey, value);
            }
        }

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

