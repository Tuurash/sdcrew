using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinOIDC
{
    public partial class MainPage : ContentPage
    {
        OidcClient _client;
        LoginResult _result;

        Lazy<HttpClient> _apiClient = new Lazy<HttpClient>(() => new HttpClient());

        public MainPage()
        {
            InitializeComponent();
            Login.Clicked += Login_Clicked;

            var browser = DependencyService.Get<IBrowser>();

            var options = new OidcClientOptions
            {
                Authority = "https://identity.satcomdirect.com",
                ClientId = "CrewMobileClient",
                Scope = "openid profile offline_access sdportalapi",
                RedirectUri = "sdcrew://callback",
                Browser = browser,

                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
                 // RequiredHttps default is true. Remove this Policy if Https is required
                Policy = new Policy()
                {
                    Discovery = new DiscoveryPolicy { RequireHttps = false }
                }
            };

            _client = new OidcClient(options);
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            _result = await _client.LoginAsync(new LoginRequest());

            if (_result.IsError)
            {
                OutputText.Text = _result.Error;
                return;
            }

            string mail = (from r in _result.User.Claims
                           where r.Type == "email"
                           select r.Value).ToString();

            var sb = new StringBuilder(128);

            var email = _result.User.Claims.Where(x => x.Type == System.Security.Claims.ClaimTypes.Email)
              .Select(x => x.Value).ToString();

            foreach (var claim in _result.User.Claims)
            {
                sb.AppendFormat("{0}: {1}\n", claim.Type, claim.Value);
            }

            //sb.AppendFormat("\n{0}: {1}\n", "refresh token", _result?.RefreshToken ?? "none");
            //sb.AppendFormat("\n{0}: {1}\n", "access token", _result.AccessToken);

            OutputText.Text = sb.ToString();

            _apiClient.Value.SetBearerToken(_result?.AccessToken ?? "");
            _apiClient.Value.BaseAddress = new Uri("https://identity.satcomdirect.com");

        }

    }
}
