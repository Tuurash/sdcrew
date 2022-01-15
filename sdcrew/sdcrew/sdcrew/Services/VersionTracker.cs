using System;
using System.Globalization;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json;


using Xamarin.Essentials;
using Xamarin.Forms;

namespace sdcrew.Services
{


    public class VersionTracker
    {

        string currentVersion = "";
        string LatestVersion = "";

        int CurrentIsLatest = 0;

        App app;

        public VersionTracker()
        {
            currentVersion = VersionTracking.CurrentVersion;
        }

        public async Task<int> CheckForUpdate()
        {
            //Android
            if (Device.RuntimePlatform == Device.Android)
            {
                //var version = string.Empty;
                //var url = $"https://play.google.com/store/apps/details?id=com.satcomdirect.crew&hl=en";

                //using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                //{
                //    using (var handler = new HttpClientHandler())
                //    {
                //        using (var client = new HttpClient(handler))
                //        {
                //            using (var responseMsg = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead))
                //            {
                //                if (!responseMsg.IsSuccessStatusCode)
                //                {
                //                    //throw new LatestVersionException($"Error connecting to the Play Store. Url={url}.");
                //                }

                //                try
                //                {
                //                    var content = responseMsg.Content == null ? null : await responseMsg.Content.ReadAsStringAsync();

                //                    var versionMatch = Regex.Match(content, "<div[^>]*>Current Version</div><span[^>]*><div[^>]*><span[^>]*>(.*?)<").Groups[1];

                //                    if (versionMatch.Success)
                //                    {
                //                        version = versionMatch.Value.Trim();
                //                    }
                //                }
                //                catch (Exception e)
                //                {
                //                    //throw new LatestVersionException($"Error parsing content from the Play Store. Url={url}.", e);
                //                }
                //            }
                //        }
                //    }
                //}
            }

            //iOS
            else if (Device.RuntimePlatform == Device.iOS)
            {
                try
                {
                    var http = new HttpClient();

                    var response = await http.GetAsync($"http://itunes.apple.com/lookup?bundleId=com.satcomdirect.crew");  //{_bundleIdentifier}
                    var content = response.Content == null ? null : await response.Content.ReadAsStringAsync();

                    content = content.Replace("\n\n\n{\n \"resultCount\":1,\n \"results\": [\n", "");
                    content = content.Substring(0, content.Length - 6);

                    var appLookup = JsonConvert.DeserializeObject<App>(content);

                    LatestVersion = appLookup.version;

                    CurrentIsLatest = String.Compare(LatestVersion, currentVersion);
                }
                catch (Exception)
                {
                    return 0;
                }
            }

            return CurrentIsLatest;
        }

        public async Task OpenAppInStore()
        {
            string url = "";

            if (Device.RuntimePlatform == Device.Android)
            {
                url = "https://play.google.com/store/apps/details?id=com.sisystems.Sisystems";
                await Browser.OpenAsync(url, BrowserLaunchMode.External);
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                var location = RegionInfo.CurrentRegion.Name.ToLower();
                url = "https://apps.apple.com/us/app/sd-crew/id1483000089?mt=8";
                await Browser.OpenAsync(url, BrowserLaunchMode.External);
            }
        }

    }

    internal class App
    {
        public string version { get; set; }
    }
}
