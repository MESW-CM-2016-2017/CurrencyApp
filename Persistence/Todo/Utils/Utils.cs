using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Utils
{
    public class Utils
    {
        public static string hello()
        {
            DateTime now = DateTime.Now;
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            String nowDate = unixTimestamp.ToString();
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(unixTimestamp).ToLocalTime();
            string formattedDate = dt.ToString("dd-MM-yyyy hh:mm:ss");

            return "";
        }

        public static Int32 getCurrentTimestamp()
        {
            DateTime now = DateTime.Now;
            return toUnixTimestamp(now);
        }

        public static String getCurrentDateString()
        {
            DateTime now = DateTime.Now;
            Int32 ts = toUnixTimestamp(now);
            return toDateString(ts);
        }

        public static Int32 toUnixTimestamp(DateTime time)
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        public static String toDateString(Int32 ts)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(ts).ToLocalTime();
            string formattedDate = dt.ToString("dd-MM-yyyy hh:mm:ss");
            return formattedDate;
        }

        public static bool hasInternetAccess()
        {
            bool response = CrossConnectivity.Current.IsConnected;
            return response;
        }
    }
}
