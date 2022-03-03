using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using sdcrew.Services;
using RestSharp;

using System.IO;
using Plugin.XamarinFormsSaveOpenPDFPackage;

using Xamarin.Forms;
using sdcrew.ViewModels;
using sdcrew.Views.Login;
using Microsoft.AppCenter.Crashes;

namespace sdcrew.Repositories
{
    public class RequestsService : IRequestsService
    {
        private static HttpClient _instance;
        private static HttpClient HttpClientInstance => _instance ?? (_instance = new HttpClient());

        public string GetAccessToken()
        {
            string AccessToken = Settings.StoreAccessToken;
            return AccessToken;
        }

        private void setupHttpClient()
        {
            string token = GetAccessToken();

            HttpClientInstance.DefaultRequestHeaders.Accept.Clear();
            HttpClientInstance.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(token))
            {
                HttpClientInstance.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync()
                                            .ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception(content);
                }

                throw new HttpRequestException(content);
            }
        }

        #region Gets

        public async Task<TResult> GetAsync<TResult>(string uri, string token = "")
        {
            setupHttpClient();

            HttpResponseMessage response = await HttpClientInstance.GetAsync(uri)
                                                                   .ConfigureAwait(false);

            await HandleResponse(response);

            string serialized = await response.Content.ReadAsStringAsync();
            serialized = serialized.Replace("[", "").Replace("]", "");


            TResult result = await Task.Run(() => JsonConvert.DeserializeObject<TResult>(serialized))
                                       .ConfigureAwait(false);

            return result;
        }

        public async Task<string> GetAsyncJsonResult(string uri, string token = "")
        {
            setupHttpClient();

            HttpResponseMessage response = await HttpClientInstance.GetAsync(uri)
                                                                   .ConfigureAwait(false);

            await HandleResponse(response);

            string serialized = await response.Content.ReadAsStringAsync();
            //serialized = serialized.Replace("[", "").Replace("]", "");

            return serialized;
        }

        public async Task<string> GetJsonResult(string uri, string token = "")
        {
            var client = new RestClient(uri);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + Settings.StoreAccessToken);
            IRestResponse response = client.Execute(request);

            await Task.Delay(0);

            string serialized = response.Content.ToString();
            return serialized;
        }

        public async Task<string> GetXMLResult(string uri, string token = "")
        {
            var client = new RestClient(uri);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + Settings.StoreAccessToken);
            IRestResponse response = client.Execute(request);

            await Task.Delay(0);

            string serialized = response.Content.ToString();
            return serialized;
        }

        #endregion

        #region Puts

        public Task<TResult> PutAsync<TResult>(string uri, TResult data, string token = "")
        {
            return PutAsync<TResult, TResult>(uri, data, token);
        }

        public async Task<TResult> PutAsync<TRequest, TResult>(string uri, TRequest data, string token = "")
        {
            setupHttpClient();

            string serialized = await Task.Run(() => JsonConvert.SerializeObject(data))
                                          .ConfigureAwait(false);

            HttpResponseMessage response = await HttpClientInstance.PutAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"))
                                                                   .ConfigureAwait(false);

            await HandleResponse(response);

            string responseData = await response.Content.ReadAsStringAsync()
                                                .ConfigureAwait(false);

            return await Task.Run(() => JsonConvert.DeserializeObject<TResult>(responseData));
        }

        public async Task<bool> PutAsync(dynamic obj, string url)
        {
            var client = new RestClient(url);
            client.Timeout = -1;

            var request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", "Bearer " + Services.Settings.StoreAccessToken);
            request.AddHeader("Content-Type", "application/json");

            string body = JsonConvert.SerializeObject(obj);

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            await Task.Delay(1);
            try
            {
                IRestResponse response = client.Execute(request);
                return true;
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc);
                return false;
            }
        }

        #endregion

        #region Posts

        public Task<string> PostAsync<TResult>(string uri, TResult data, string token = "")
        {
            return PostAsync<TResult, TResult>(uri, data);
        }

        public async Task<string> PostAsync<TRequest, TResult>(string uri, TRequest data, string token = "")
        {
            setupHttpClient();
            string responseData = "";
            try
            {

                string serialized = await Task.Run(() => JsonConvert.SerializeObject(data));
                HttpResponseMessage response = await HttpClientInstance.PostAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));
                //           .ConfigureAwait(false);
                await HandleResponse(response);
                responseData = await response.Content.ReadAsStringAsync()
                                                .ConfigureAwait(false);
            }
            catch (Exception exc)
            {
                Console.Write(exc);
            }
            return responseData;
        }

        public async Task<bool> Post_Custom(dynamic obj, string url)
        {
            var client = new RestClient(url);
            client.Timeout = -1;

            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + Services.Settings.StoreAccessToken);
            request.AddHeader("Content-Type", "application/json");

            string body = JsonConvert.SerializeObject(obj);

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            await Task.Delay(1);
            try
            {
                IRestResponse response = client.Execute(request);
                return true;
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc);
                return false;
            }
        }

        public async Task<string> PostAsyncWithReturnValue(dynamic obj, string url)
        {
            var client = new RestClient(url);
            client.Timeout = -1;

            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + Services.Settings.StoreAccessToken);
            request.AddHeader("Content-Type", "application/json");

            string body = JsonConvert.SerializeObject(obj);

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            await Task.Delay(1);
            try
            {
                IRestResponse response = client.Execute(request);
                return response.Content.ToString();
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc);
                return null;
            }
        }

        public async Task<MemoryStream> PostAsyncPDF(string uri, dynamic data, string getIteneraryName, string token = "")
        {
            setupHttpClient();

            Stream content;
            MemoryStream stream = new MemoryStream();

            try
            {
                string serialized = await Task.Run(() => JsonConvert.SerializeObject(data));

                HttpResponseMessage response = await HttpClientInstance.PostAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));
                await HandleResponse(response);

                if (response.IsSuccessStatusCode)
                {
                    content = await response.Content.ReadAsStreamAsync();
                    content.CopyTo(stream);
                }
                //Device.BeginInvokeOnMainThread(async () => await CrossXamarinFormsSaveOpenPDFPackage.Current.SaveAndView(Guid.NewGuid() + ".pdf", "application/pdf", stream, PDFOpenContext.ChooseApp));

                //Device.BeginInvokeOnMainThread(async () => await CrossXamarinFormsSaveOpenPDFPackage.Current.SaveAndView(getIteneraryName + ".pdf", "application/pdf", stream, PDFOpenContext.ChooseApp));

                return stream;
            }
            catch (Exception exc)
            {
                Console.Write(exc);

                return null;
            }
        }

        #endregion
    }



}
