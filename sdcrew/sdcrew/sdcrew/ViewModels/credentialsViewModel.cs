//using Android.Content.Res;

using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient.Results;
using Newtonsoft.Json;
using sdcrew.Utils;
using sdcrew.Views.Dashboard;
using sdcrew.Views.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using sdcrew.Services;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using sdcrew.Services.Data;
using System.Diagnostics;
using System.ComponentModel;

namespace sdcrew.ViewModels
{
    public class credentialsViewModel: BaseViewModel, INotifyPropertyChanged
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

        UserServices _userServices;

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

            public User(string identityToken, string accessToken, DateTime accessTokenGoodUntilUTC, string refreshToken, string name,string email,string customerId,string accountId)
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

        private User? _currentUser = null;

        public event EventHandler<UserChangedEventArgs> UserChanged;
        public credentialsViewModel()
        {
            _userServices = new UserServices();
            InitSdclientOptions();
        }

        public async Task Initialize()
        {
            MainThread.BeginInvokeOnMainThread(() => { IsBusy = true; });

            _client = InitOIDC();

            var user = await LoadSavedUserAsync();
            user = await RefreshAsync(user).ConfigureAwait(false);
            CurrentUser = user;

            if (pass!=0)
            {
                
                MainThread.BeginInvokeOnMainThread(async() =>await RedirectToDashboard());
                //await Task.Run(() => RedirectToDashboard());
            }

            IsBusy = false;
        }

        private OidcClient InitOIDC()
        {
            string AuthUrl = Settings.StoreAuthidentifierUrl;
            //AuthUrl = "https://identity.satcomdirect.com";
            string ClientId = "CrewMobileClient";

            //if (Settings.StoreAuthidentifierUrl != "")
            //{
            //    AuthUrl = Settings.StoreAuthidentifierUrl;
            //}
            //else
            //{
            //    AuthUrl = "https://identity.satcomdirect.com";
            //}

            var options = new OidcClientOptions
            {

                Browser = DependencyService.Get<IBrowser>(),
                Authority = AuthUrl,
                ClientId = ClientId,

                Scope = "openid profile offline_access sdportalapi",
                RedirectUri = "sdcrew://callback",
                ClientSecret="",
                PostLogoutRedirectUri = "sdcrew://logout",  //"sdcrew://logout",
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
        };

           return _client = new OidcClient(options);
        }

        private void InitSdclientOptions()
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

            _client = new OidcClient(options);
        }

        public User? CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                SaveUserAsync(_currentUser);

                UserChanged?.Invoke(this, new UserChangedEventArgs(_currentUser));
                if (value == null)
                {
                    Task.Run(async () => await StopRefreshingToken());
                }
                else
                {
                    Task.Run(async () => await StartRefreshingToken());
                }
            }
        }

        public async Task<User?> LoginAsync()
        {
            InitSdclientOptions();
            //await Initialize();
            var result = await _client.LoginAsync(new LoginRequest());

            if (result.IsError)
            {
                CurrentUser = null;
            }
            else
            {
                CurrentUser = CreateUser(result);
            }

            return CurrentUser;
        }

        #region CheckSavedUser

        private static async Task<User?> LoadSavedUserAsync()
        {
            Console.WriteLine($"[oidc] loading saved user...");
            try
            {
                var strUser = await SecureStorage.GetAsync(CurrentUserKey);

                if (strUser == null)
                {
                    strUser = Settings.GeneralSettings;
                }

                if (strUser == null || strUser == "")
                {
                    Console.WriteLine($"[oidc] no saved user found");
                    return null;
                }

                var tUser = JsonConvert.DeserializeObject<User>(strUser);
                Console.WriteLine($"[oidc] user found [{tUser.Name}]");
                return tUser;
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

        int pass = 0;

        private async Task<User?> RefreshAsync(User? user)
        {
            Console.WriteLine($"Refreshing user [{user?.Name}]");

            _client = InitOIDC();
            if (user == null)
            {
                return null;
            }

            var tUser = user.Value;

            if (string.IsNullOrWhiteSpace(tUser.RefreshToken))
            {
                Console.WriteLine($"Refresh token is empty");
                return null;
            }

            try
            {
                var result = await _client.RefreshTokenAsync(tUser.RefreshToken);
                if (result.IsError)
                {
                    Console.WriteLine($"Error while refreshing [{result.Error}]");
                    // TODO(jpr): log to appcenter
                    return null;
                }
                //Store AccessToken into localstorage
                Settings.StoreAccessToken = string.Empty;
                Settings.StoreAccessToken = tUser.AccessToken;

                Console.WriteLine($"Refreshed user");
                pass = pass + 1;
                return CreateUser(result, tUser.Name, tUser.Email, tUser.CustomerId, tUser.AccountId);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while refreshing [{e.Message}]");
                // TODO(jpr): log to appcenter
                return null;
            }
        }


        //SaveUserAsync was static
        public Task SaveUserAsync(User? user)
        {
            if (user is User tUser)
            {
                Console.WriteLine($"[oidc] saving user [{tUser.Name}]...");
                var strUser = JsonConvert.SerializeObject(tUser);
                SecureStorage.SetAsync(CurrentUserKey, strUser);

                //Save to local settings storage
                Settings.GeneralSettings = strUser;

                //Store Refreshtoken into localstorage
                Settings.StoreAccessToken = string.Empty;
                Settings.StoreAccessToken = tUser.AccessToken;

            }
            else
            {
                Console.WriteLine($"[oidc] removing saved user...");
                SecureStorage.Remove(CurrentUserKey);
                Settings.StoreAccessToken = string.Empty;
                Console.WriteLine($"[oidc] done");
            }
            return null;
        }

        //Helpers
        private bool IsRefreshing { get; set; } = false;

        private Semaphore RefreshLock = new Semaphore(1, 1, "RefreshLock");

        private CancellationTokenSource tokenSource;

        private readonly TimeSpan TimeInAdvanceToRequestRefresh = TimeSpan.FromSeconds(10);


        public async Task StartRefreshingToken()
        {
            Console.WriteLine($"REFRESH START CALLED [isRefreshing {IsRefreshing}] [name {CurrentUser?.Name}]");

            if (CurrentUser == null || IsRefreshing)
            {
                return;
            }

            RefreshLock.WaitOne();

            Console.WriteLine($"REFRESH START ENTERED");

            if (CurrentUser == null || IsRefreshing)
            {
                goto cleanup;
            }

            IsRefreshing = true;

            tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            Action refresh = async () =>
            {
                var innerToken = token;
                Console.WriteLine("REFRESH STARTING");
                while (!innerToken.IsCancellationRequested)
                {
                    Console.WriteLine("REFRESH TICK");

                    var user = CurrentUser;

                    if (user == null)
                    {
                        break;
                    }
                    else
                    {
                        var tUser = user.Value;

                        var dt = tUser.AccessTokenGoodUntilUTC - DateTime.UtcNow - TimeInAdvanceToRequestRefresh;

                        Settings.AccessTokenExpiry = tUser.AccessTokenGoodUntilUTC.ToString();

                        dt = dt < TimeSpan.Zero ? TimeSpan.Zero : dt;
                        Console.WriteLine($"REFRESH SCHEDULED [{(int)dt.TotalMinutes}m{dt.Seconds}s]");

                        await Task.Run(() =>
                        {
                            Task.Run(async () => await Initialize());
                        });

                        await Task.Delay(dt);
                        if (!innerToken.IsCancellationRequested)
                        {
                            CurrentUser = await RefreshAsync(CurrentUser).ConfigureAwait(false);
                        }
                    }
                }

                Console.WriteLine("REFRESH STOPPED");
            };
            await Task.Run(refresh);
        cleanup:
            RefreshLock.Release();
        }

        public async Task StopRefreshingToken()
        {
            Console.WriteLine($"REFRESH STOP CALLED [isRefreshing {IsRefreshing}]");
            if (!IsRefreshing)
            {
                return;
            }

            RefreshLock.WaitOne();

            Console.WriteLine($"REFRESH STOP ENTERED");

            if (!IsRefreshing)
            {
                goto cleanup;
            }

            IsRefreshing = false;

            tokenSource.Cancel();
            tokenSource = null;

        cleanup:
            RefreshLock.Release();
        }

        #endregion

        //Logout events
        public async Task LogoutAsync()
        {
            //await PerformLogoutTask();

            CurrentUser = null;

            Settings.GeneralSettings = "";
            SecureStorage.Remove("scheduler.current_user");
            Settings.StoreAccessToken = string.Empty;

            Device.BeginInvokeOnMainThread(async () => await PerformLogoutTask());
                
        }

        private Task PerformLogoutTask()
        {
            var idToken = _userServices.GetUser().Result.IdentityToken;

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

        private async void PerformLogin(object obj)
        {
            IsBusy = true;

            await LoginAsync();

            if (CurrentUser != null)
            {

                await RedirectToDashboard();
                IsBusy = false;
            }
        }

        private async Task RedirectToDashboard()
        {
            await _userServices.AddUser().ConfigureAwait(false);

            App.Current.MainPage = new AppShell();
            //await this.routingService.NavigateTo("///main");
        }

    }

    public class AccountOptions
    {
        // More account options
        public static bool AutomaticRedirectAfterSignOut = true;
    }

}
