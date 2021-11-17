using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Plugin.Geolocator;
using pikappDes.Utils.modals;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace pikappDes.Utils
{
    static class Utility
    {
        static Uri furi = new Uri("https://www.dropbox.com/s/415evpihw5pduxp/lnk.txt?dl=1");
        
        
        static HttpClient client = new HttpClient();
        static string uri;
        static bool err;


        private async static Task GetLnk()
        {


            HttpClientHandler handler = new HttpClientHandler();
            handler.UseProxy = false;
            handler.Proxy = null;

            HttpClient urigetter = new HttpClient(handler);
            try
            {
                uri = new Uri(await client.GetStringAsync(furi) + "/api/values").ToString();
                //uri = "https://a473-41-62-128-147.ngrok.io/api/values";
                

                err = false;
            }
            catch (Exception)
            {

                err = true;
            }

        }

        
        public static async Task<string> GetUri(bool reload)
        {
            //if (reload || (uri != null || uri != string.Empty))
            //{
            //    await GetLnk();
            //}

            //if (!err)
            //    return uri;
            //else
            //    return null;
            //uri = "https://pickapp-service.herokuapp.com" + "/api/values";
            string uritemp = "https://27ad-197-1-79-179.ngrok.io" + "/api/values";
            return uritemp;
        }

        public static async Task<string> LoginReq(string baseURI,Creds item,string login_t)
        {
            string furi = baseURI + "/login";

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Login_t", login_t);


            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            var json = JsonConvert.SerializeObject(item, settings);
            
            var itemcontent = new StringContent(json, Encoding.UTF8, "application/json");

            var res = client.PostAsync(furi, itemcontent);

            if (res.Result.IsSuccessStatusCode)
            {
                var srespons = await res.Result.Content.ReadAsStringAsync();
                return srespons;
            }
            else
            {
                return "ERROR";
            }
            // return type : creds (always) 

        }

        public static async Task<string> SignUp(string baseURI,Creds item)
        {
            string furi = baseURI + "/sign";


            client.DefaultRequestHeaders.Clear();
           

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            var json = JsonConvert.SerializeObject(item,settings);

            var itemcontent = new StringContent(json, Encoding.UTF8, "application/json");

            var res = client.PostAsync(furi, itemcontent);

           

            if (res.Result.IsSuccessStatusCode)
            {
                var srespons = await res.Result.Content.ReadAsStringAsync();
                return srespons;
            }
            else
            {
                return "ERROR";
            }

            //should return back string "SIGNED_UP"
        }


        public static async Task<string> GetList(string baseURI,Creds item)
        {

            string furi = baseURI + "/GetList";

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            var json = JsonConvert.SerializeObject(item,settings);
            var itemcontent = new StringContent(json, Encoding.UTF8, "application/json");

            var res = client.PostAsync(furi, itemcontent);

            if(res.Result.IsSuccessStatusCode)
            {
                return await res.Result.Content.ReadAsStringAsync();
            }
            else
            {
                return "ERROR";
            }
            //return type : list<userprops>
        }


        public static async Task<string> GetProfile(string baseURI, Creds item,string UID)
        {

            string furi = baseURI + "/GetProfile";

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            var json = JsonConvert.SerializeObject(item, settings);
            var itemcontent = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("UID", UID);


            var res = client.PostAsync(furi, itemcontent);

            if (res.Result.IsSuccessStatusCode)
            {
                return await res.Result.Content.ReadAsStringAsync();
            }
            else
            {
                return "ERROR";
            }
        }

        public static async Task<string> UpdatePos(string baseURI,UserProp item)
        {
            string furi = baseURI + "/UpdatePos";

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());


            var json = JsonConvert.SerializeObject(item,settings);
            var itemcontent = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("U", Preferences.Get("UID", ""));
            client.DefaultRequestHeaders.Add("S", Preferences.Get("SID", ""));
           

            var res = client.PostAsync(furi, itemcontent);
            if (res.Result.IsSuccessStatusCode)
            {
                
                return await res.Result.Content.ReadAsStringAsync();
            }
            else
            {
                return "ERROR";
            }

            //return string "UPDATED" or "LOGIN_ERROR"
        }

        public static async Task<Plugin.Geolocator.Abstractions.Position> GetPos(bool accurate)
        {


            var locator = CrossGeolocator.Current;

            locator.DesiredAccuracy = 10;

            
            return (accurate)?(await locator.GetPositionAsync(TimeSpan.FromSeconds(5))):(await locator.GetLastKnownLocationAsync());

        }

      

    }
}
