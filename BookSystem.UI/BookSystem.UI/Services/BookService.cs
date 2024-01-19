using BookSystem.UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace BookSystem.UI.Services
{

    public class BookService
    {
        public List<BookSearchModel> books;
        private HttpClient _httpClient;
        public BookService()
        {
            _httpClient = new HttpClient();
            books = new();
        }
        public async Task<List<BookSearchModel>> SearchAndGetOne(string token, string path)
        {
            List<BookSearchModel> books = new();
            HttpResponseMessage httpResponse = await Search(token, path);
            if (httpResponse.IsSuccessStatusCode)
            {
                string data = await httpResponse.Content.ReadAsStringAsync();
                Dictionary<string, object> dataJson = JsonSerializer.Deserialize<Dictionary<string, object>>(data)!;
                Dictionary<string, object> dataJsonobject = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(dataJson["data"]))!;
                if(dataJsonobject.Count > 0)
                {
                    books.Add(new BookSearchModel
                    {
                        Author = dataJsonobject["author"].ToString()!,
                        Title = dataJsonobject["title"].ToString()!,
                        Id = int.Parse(dataJsonobject["id"].ToString()!),
                        IsAvailable = bool.Parse(dataJsonobject["available"].ToString()!)
                    });
                }
                
            }
            return books;
        }

        public async Task<List<BookSearchModel>> SearchAndGetList(string token, string path)
        {
            List<BookSearchModel> books = new();
            HttpResponseMessage httpResponse = await Search(token, path);
            if (httpResponse.IsSuccessStatusCode)
            {
                string data = await httpResponse.Content.ReadAsStringAsync();
                Dictionary<string, object> dataJson = JsonSerializer.Deserialize<Dictionary<string, object>>(data)!;
                List<Dictionary<string, object>> dataJsonobject = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(JsonSerializer.Serialize(dataJson["data"]))!;
                foreach(Dictionary<string,object> obj in dataJsonobject)
                {
                    books.Add(new BookSearchModel
                    {
                        Author = obj["author"].ToString()!,
                        Title = obj["title"].ToString()!,
                        Id = int.Parse(obj["id"].ToString()!),
                        IsAvailable = bool.Parse(obj["available"].ToString()!)
                    });
                }
            }
            return books;
        }

        private async Task<HttpResponseMessage> Search(string token ,string path)
        {
            List<BookSearchModel> books = new List<BookSearchModel>();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            
            string url = $"{Constants.Host}{path}";
            return await _httpClient.GetAsync(url);
        }

        public async Task<Dictionary<string, object>> BorrowBooks( string token ,List<int> booksId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            string url = $"{Constants.Host}book/borrow";
            Dictionary<string, object> body = new Dictionary<string, object>
            {
                {"booksId",booksId}
            };
            HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync(url,body);
            Dictionary<string, object> response = new();
            string data = await httpResponse.Content.ReadAsStringAsync();
            if (httpResponse.IsSuccessStatusCode)
            {
                Dictionary<string, object>  dataJson = JsonSerializer.Deserialize<Dictionary<string, object>>(data)!;
                response.Add("message", dataJson["message"]);
            }else
            {
                Dictionary<string, object> dataJson = JsonSerializer.Deserialize<Dictionary<string, object>>(data)!;
                response.Add("message", dataJson["Message"]);
            }
            response.Add("code", httpResponse.IsSuccessStatusCode ? "200" : "400");
            return response;
        }
        public async Task<List<BorrowedBooksModel>> GetBorrowedBooks(string token, int page)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            string url = $"{Constants.Host}book/borrowed/{page}";
            HttpResponseMessage httpResponse = await _httpClient.GetAsync(url);
            List<BorrowedBooksModel> response = new();
            if (httpResponse.IsSuccessStatusCode) {
                string data = await httpResponse.Content.ReadAsStringAsync();
                Dictionary<string, object> dataJson = JsonSerializer.Deserialize<Dictionary<string, object>>(data)!;
                List<Dictionary<string, object>> dataJsonObj = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(dataJson["data"].ToString()!)!;

                foreach(Dictionary<string,object> obj in dataJsonObj)
                {
                    response.Add(new BorrowedBooksModel
                    {
                        Id = int.Parse(obj["id"].ToString()!),
                        Title = obj["title"].ToString()!
                    });
                }
            }else
            {
                string data = await httpResponse.Content.ReadAsStringAsync();
                Dictionary<string, object> dataJson = JsonSerializer.Deserialize<Dictionary<string, object>>(data)!;
                throw new Exception(dataJson["Message"].ToString());
            }
            return response;
        }
       
        public async Task<Dictionary<string, object>> ReturnIt(string token, List<int>ids)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            string url = $"{Constants.Host}book/return";
            HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync(url,new Dictionary<string, object>
            {
                { "booksId", ids}
            });
            string data = await httpResponse.Content.ReadAsStringAsync();
            Dictionary<string, object> dataJson = JsonSerializer.Deserialize<Dictionary<string, object>>(data)!;
            if (httpResponse.IsSuccessStatusCode)
            {
                return new Dictionary<string, object>
                {
                    { "message",dataJson["message"]}
                };
            }else
            {
                throw new Exception(dataJson["Message"].ToString());
            }
            
            
        }
    }
}


