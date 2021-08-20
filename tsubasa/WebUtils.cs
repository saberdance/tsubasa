using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace tsubasa
{
    public class WebUtilsException : Exception
    {
        public WebUtilsException() : base() { }
        public WebUtilsException(string message) : base(message) { }
        public WebUtilsException(string message, Exception inner) : base(message, inner) { }
        protected WebUtilsException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class WebHelper
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public WebHelper(string rootUri,string token = null)
        {
            _httpClient.BaseAddress = new(rootUri);
            if (!string.IsNullOrEmpty(token))
            {
                //需要token就添加token
                _httpClient.DefaultRequestHeaders.Add("client-token-user", token);
            }
        }

        public void AddCreadentials(string username, string password)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")
                ));
        }

        public void AddSingleHeader(string headerKey,string value)
        {
            _httpClient.DefaultRequestHeaders.Add(headerKey, value);     
        }

        public async Task<bool> DownloadFile(string uri, string filePath)
        {
            try
            {
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                using (var fileStream = new FileStream(filePath,FileMode.Create))
                {
                    var netstream = await _httpClient.GetStreamAsync(uri);
                    await netstream.CopyToAsync(fileStream);//写入文件
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Error<WebHelper>($"下载:[{uri}]出错");
                Logger.Error<WebHelper>($"错误信息:{e}");
                return false;
            }
            
        }

        public async Task<string> GetRequest(string router)
        {
            try
            {
                var response = await _httpClient.GetAsync(router);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Logger.Error<WebHelper>($"GET通讯错误:{e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Logger.Error<WebHelper>($"GET非通讯错误:{e.Message}");
                return null;
            }
        }

        public async Task<string> PostFormRequest(string router, FormUrlEncodedContent content)
        {
            try
            {
                var response = await _httpClient.PostAsync(router, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Logger.Error<WebHelper>($"POST通讯错误:{e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Logger.Error<WebHelper>($"POST非通讯错误:{e.Message}");
                return null;
            }
        }

        public async Task<string> PostJsonRequest(string router, string jsonString)
        {
            try
            {
                HttpContent content = new StringContent(jsonString);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var response = await _httpClient.PostAsync(router, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Logger.Error<WebHelper>($"POST通讯错误:{e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Logger.Error<WebHelper>($"POST非通讯错误:{e.Message}");
                return null;
            }
        }

        public async Task<string> PutJsonRequest(string router, string jsonString)
        {
            try
            {
                HttpContent content = new StringContent(jsonString);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var response = await _httpClient.PutAsync(router, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Logger.Error<WebHelper>($"PUT通讯错误:{e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Logger.Error<WebHelper>($"PUT非通讯错误:{e.Message}");
                return null;
            }
        }
    }
}
