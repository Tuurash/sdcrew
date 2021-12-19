//using Android.Content.Res;

using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient.Results;

using Newtonsoft.Json;

using sdcrew.Services;
using sdcrew.Services.Data;
using sdcrew.Views.Dashboard;

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace sdcrew.ViewModels
{
    public class credentialsViewModel : BaseViewModel, INotifyPropertyChanged
    {
        OidcClient _client;

        ActivityIndicator loader = new ActivityIndicator();

        private static string CurrentUserKey => "scheduler.current_user";

        #region ISLoadING

        private bool isLoading;
        public bool IsLoading
        {
            get
            {
                return isLoading;
            }

            set
            {
                isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));

            }
        }

        #endregion

        //save user after login line 284
        UserServices _userServices = new UserServices();


        public credentialsViewModel()
        {
            _client = InitOIDC();
        }


        //Structs
        public readonly struct User
        {
            public string IdentityToken { get; }
            public string AccessToken { get; }
            public DateTime AccessTokenGoodUntilUTC { get; }
            public string RefreshToken { get; }
            public string Name { get; }
            public string Email { get; }
            public string CustomerId { get; }
            public string AccountId { get; }

            public User(string identityToken, string accessToken, DateTime accessTokenGoodUntilUTC, string refreshToken, string name, string email, string customerId, string accountId)
            {
                IdentityToken = identityToken;
                AccessToken = accessToken;

                AccessTokenGoodUntilUTC = accessTokenGoodUntilUTC.ToUniversalTime();
                RefreshToken = refreshToken;
                Name = name;
                CustomerId = customerId;
                AccountId = accountId;
                Email = email;
            }
        }

        protected struct OidcEnvironmentSettings
        {
            public string Authority;
            public string ClientId;
        }

        public readonly struct UserChangedEventArgs
        {
            public User? User { get; }

            public UserChangedEventArgs(User? user)
            {
                User = user;
            }
        }

        public event EventHandler<UserChangedEventArgs> UserChanged;

        private OidcClient InitOIDC()
        {
            string AuthUrl = "https://identity.satcomdirect.com";
            string ClientId = "CrewMobileClient";

            if (Settings.StoreAuthidentifierUrl != "")
            {
                AuthUrl = Settings.StoreAuthidentifierUrl;
            }
            else
            {
                AuthUrl = "https://identity.satcomdirect.com";
            }

            var options = new OidcClientOptions
            {

                Browser = DependencyService.Get<IBrowser>(),
                Authority = AuthUrl,
                ClientId = ClientId,

                Scope = "openid profile offline_access sdportalapi",
                RedirectUri = "sdcrew://callback",
                PostLogoutRedirectUri = "sdcrew://logout",  //"sdcrew://logout",
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,

            };

            return _client = new OidcClient(options);
        }

        #region CheckSavedUser

        private static async Task<UserSecrets> LoadSavedUserAsync()
        {
            Console.WriteLine($"[oidc] loading saved user...");
            try
            {
                var strUser = await SecureStorage.GetAsync(CurrentUserKey);

                if (strUser == null)
                {
                    strUser = Settings.UserSecrets;
                }

                if (strUser == null || strUser == "")
                {
                    Console.WriteLine($"[oidc] no saved user found");
                    return null;
                }

                var tUserSecrets = JsonConvert.DeserializeObject<UserSecrets>(strUser);
                Console.WriteLine($"[oidc] user found [{tUserSecrets.Name}]");
                return tUserSecrets;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[oidc] error while loading user");
                Console.WriteLine($"[oidc] msg [{e.Message}]");
                return null;
            }
        }

        #endregion

        #region CreateUSer

        //Keep
        private static User CreateUser(LoginResult result)
        {
            var fetched_Email = result.User.Claims.First(x => x.Type == "email").Value;
            var fetched_CustomerId = result.User.Claims.First(x => x.Type == "http://schemas.satcomdirect.com/ws/2014/10/identity/claims/customerids").Value;
            var fetched_AccountId = result.User.Claims.First(x => x.Type == "http://schemas.satcomdirect.com/ws/2014/10/identity/claims/accountids").Value;

            //Store Refreshtoken into localstorage
            Settings.StoreAccessToken = string.Empty;
            Settings.StoreAccessToken = result.AccessToken;

            return new User(
                identityToken: result.IdentityToken,
                accessToken: result.AccessToken,
                accessTokenGoodUntilUTC: result.AccessTokenExpiration.ToUniversalTime(),
                refreshToken: result.RefreshToken,
                name: result.User.Identity.Name,
                email: fetched_Email,
                accountId: fetched_AccountId,
                customerId: fetched_CustomerId
            );


        }

        private static User CreateUser(RefreshTokenResult result, string Name, string Email, string CustomerID, string AccountID)
        {
            return new User(
                identityToken: result.IdentityToken,
                accessToken: result.AccessToken,
                accessTokenGoodUntilUTC: result.AccessTokenExpiration.ToUniversalTime(),
                refreshToken: result.RefreshToken,
                name: Name,
                email: Email,
                customerId: CustomerID,
                accountId: AccountID
            );
        }

        #endregion

        #region RefreshToken

        int RefreshAttemptCount = 0;

        public async Task<int> PerformRefreshToken()
        {
            var goodUntil = DateTime.Parse(Services.Settings.AccessTokenExpiry);

            var remainingMinuites = (int)Math.Round((goodUntil - DateTime.Now).TotalMinutes);

            if (remainingMinuites > 5 & remainingMinuites < 180)
            {
                if (remainingMinuites < 15)
                {
                    await RefreshTokenRefreshingAsync();
                }
                RefreshAttemptCount = 0;
                return 1;
            }
            else
            {
                RefreshAttemptCount++;
                Console.WriteLine($"Attempt Refreshing user");
                await RefreshTokenRefreshingAsync();
                return await PerformRefreshToken();
            }

        }

        public async Task RefreshTokenRefreshingAsync()
        {
            #region Refreshing

            var tUser = new Models.User();

            tUser = JsonConvert.DeserializeObject<Models.User>(Settings.UserSecrets);

            if (string.IsNullOrWhiteSpace(tUser.RefreshToken))
            {
                tUser = JsonConvert.DeserializeObject<Models.User>(Settings.UserSecrets);
            }
            try
            {
                var result = await _client.RefreshTokenAsync(tUser.RefreshToken);
                if (result.IsError)
                {
                    Console.WriteLine($"Error while refreshing [{result.Error}]");
                }
                await UpdateUserAsync(tUser, result);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while refreshing [{e.Message}]");
                Settings.StoreAccessToken = string.Empty;
            }

            #endregion
        }

        //Keep
        //SaveUserAsync was static
        public async Task SaveUserAsync(User? user)
        {
            if (user is User tUser)
            {
                Console.WriteLine($"[oidc] saving user [{tUser.Name}]...");

                var strUser = JsonConvert.SerializeObject(tUser);
                await SecureStorage.SetAsync(CurrentUserKey, strUser);

                //Save to local settings storage
                Settings.GeneralSettings = strUser;
                await _userServices.AddUser();

                //save User Secrets
                var uSecrets = new UserSecrets
                {
                    Name = tUser.Name,
                    AccessToken = tUser.AccessToken,
                    AccessTokenExpiration = tUser.AccessTokenGoodUntilUTC,
                    IdentityToken = tUser.IdentityToken,
                    RefreshToken = tUser.RefreshToken,
                };

                var strUserSecrets = JsonConvert.SerializeObject(uSecrets);
                Settings.UserSecrets = strUserSecrets;
                Settings.StoreAccessToken = uSecrets.AccessToken;
                Settings.AccessTokenExpiry = uSecrets.AccessTokenExpiration.ToString();
            }
            else
            {
                Console.WriteLine($"[oidc] removing saved user...");
                Settings.UserSecrets = string.Empty;
                SecureStorage.Remove(CurrentUserKey);
                Settings.StoreAccessToken = string.Empty;
                Console.WriteLine($"[oidc] done");
            }

        }

        private async Task UpdateUserAsync(Models.User tUser, RefreshTokenResult result)
        {
            //Store AccessToken into localstorage
            Settings.StoreAccessToken = result.AccessToken;
            Settings.AccessTokenExpiry = result.AccessTokenExpiration.ToString();

            var uSecrets = new UserSecrets
            {
                Name = tUser.Name,
                AccessToken = result.AccessToken,
                AccessTokenExpiration = result.AccessTokenExpiration,
                IdentityToken = result.IdentityToken,
                RefreshToken = result.RefreshToken,
            };

            await Task.Delay(0);


            var strUserSecrets = JsonConvert.SerializeObject(uSecrets);
            Settings.UserSecrets = strUserSecrets;

            Console.WriteLine($"Updated user.. Saving into Cache");
        }

        //Helpers
        //private Semaphore RefreshLock = new Semaphore(1, 1, "RefreshLock");
        #endregion

        //Logout events
        public async Task LogoutAsync()
        {
            await PerformLogoutTask();

            Settings.GeneralSettings = "";
            SecureStorage.Remove("scheduler.current_user");
            Settings.StoreAccessToken = string.Empty;
        }

        private Task PerformLogoutTask()
        {
            var userSecrets = JsonConvert.DeserializeObject<UserSecrets>(Settings.UserSecrets);

            var idToken = userSecrets.IdentityToken;

            if (idToken != null)
            {
                var request = new LogoutRequest
                {
                    IdTokenHint = idToken,
                };
                return _client.LogoutAsync(request);
            }

            return Task.CompletedTask;
        }

        public ICommand LoginCommand => new Command(PerformLogin);

        public async Task<User?> LoginAsync()
        {
            User fetchedUser = new User();

            IsBusy = true;

            LoginResult result = await _client.LoginAsync(new LoginRequest());

            if (result.IsError)
            {
                return null;
            }
            else
            {
                fetchedUser = CreateUser(result);
                await SaveUserAsync(fetchedUser);
            }

            return fetchedUser;
        }

        private async void PerformLogin(object obj)
        {
            var ResultUser = await LoginAsync();

            if (ResultUser != null)
            {
                await RedirectToDashboard();
            }
        }

        private async Task RedirectToDashboard()
        {
            await Task.Delay(1);

            App.Current.MainPage = new AppShell();
        }

    }

    public class AccountOptions
    {
        // More account options
        public static bool AutomaticRedirectAfterSignOut = true;
    }

    public class UserSecrets
    {
        public string Name { get; set; }
        public string IdentityToken { get; set; }
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public string RefreshToken { get; set; }
    }

}
