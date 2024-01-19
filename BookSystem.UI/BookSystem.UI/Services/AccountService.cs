using BookSystem.UI.Models;
using DotNet.Cookies;
using System.Text.Json;

namespace BookSystem.UI.Services
{
    public class AccountService
    {
        
        private HttpClient httpClient;
        public AccountService()
        {
            httpClient = new HttpClient();
        }

        public async Task<Dictionary<string, object>> Login(AccountModel login, HttpResponse response)
        {
            string url = Constants.Host + "Account/login";
            HttpResponseMessage responseMessage = await httpClient.PostAsJsonAsync(url, login);
            var m = await responseMessage.Content.ReadAsStringAsync();
            Dictionary<string, object>? mm = JsonSerializer.Deserialize<Dictionary<string, object>>(m);
            string data = await responseMessage.Content.ReadAsStringAsync();
            if (responseMessage.IsSuccessStatusCode)
            {
                Dictionary<string, object>? dataJson = JsonSerializer.Deserialize<Dictionary<string, object>>(data);
                Dictionary<string, object>? dataInfoJson = JsonSerializer.Deserialize<Dictionary<string, object>>(dataJson!["info"].ToString()!);
                
                DateTime date = DateTime.Parse(dataInfoJson!["expire"].ToString()!);
                CookieOptions cookie = new CookieOptions();
                cookie.Expires = date;
                response.Cookies.Append("token", dataInfoJson!["token"].ToString()!);
                response.Cookies.Append("role", dataInfoJson!["role"].ToString()!);


                return new Dictionary<string, object>
                {
                    {"message",dataJson!["message"] },
                    {"code",200 }
                };
            }else
            {
                Dictionary<string, object>? dataJson = JsonSerializer.Deserialize<Dictionary<string, object>>(data);
                return new Dictionary<string, object>{
                    {"message",dataJson!["Message"] },
                    {"code",400 }
                };
            }




        }
    
        public async Task<Dictionary<string,object>> Register (AccountModel register)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            HttpResponseMessage httpResponse = await httpClient.PostAsJsonAsync(Constants.Host + "Account/Register",register);
            string data = await httpResponse.Content.ReadAsStringAsync();
            Dictionary<string, object> dataJson = JsonSerializer.Deserialize<Dictionary<string, object>>(data)!;
            if (httpResponse.IsSuccessStatusCode)
            {
                response.Add("message", dataJson["message"]);
                response.Add("code", 200);
                return response;
            }
            else
            {
                response.Add("message", dataJson["Message"]);
                response.Add("code", 400);
                return response;
            }
        }
    }


}
