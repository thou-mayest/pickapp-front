using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Plugin.Geolocator;

namespace pikappDes
{
    static class Utility
    {
        static Uri furi = new Uri("https://www.dropbox.com/s/4o21xuhqqmjywbt/test.txt?dl=1");
        static HttpClient client = new HttpClient();
        static string uri;
        static bool err;
        private async static Task GetLnk()
        {
            try
            {
                uri = new Uri(await client.GetStringAsync(furi) + "/api/values").ToString();
                err = false;
            }
            catch (Exception)
            {

                err = true;
            }

        }
        public static async Task<String> GetUri()
        {
            await GetLnk();
            if (!err)
                return uri;
            else
                return null;
        }

        public static async Task<Plugin.Geolocator.Abstractions.Position> GetPos()
        {

            var locator = CrossGeolocator.Current;

            locator.DesiredAccuracy = 10;


            return await locator.GetPositionAsync(TimeSpan.FromSeconds(5));


        }
    }
}
