using System.Collections.Generic;
using System.Web.Http.Filters;


namespace G.Extensions.Helpers
{
    public static class HttpRequestContextHelper
    {
        public static string GetBodyFromRequest(HttpActionExecutedContext context)
        {
            string data;
            using (var stream = context.Request.Content.ReadAsStreamAsync().Result)
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }
                data = context.Request.Content.ReadAsStringAsync().Result;
            }
            return data;
        }

        public static object GetQueryStringFromRequest(HttpActionExecutedContext context)
        {
            var queryStringList = new List<KeyValuePair<string,
                string>>();

            if (context.Request.RequestUri != null)
            {
                var queryCollection = context.Request.RequestUri.Query.Substring(1).Split('&');

                foreach (var item in queryCollection)
                {
                    var itm = item.Split('=');
                    var queryString = new KeyValuePair<string,
                        string>(itm[0], itm[1]);
                    queryStringList.Add(queryString);
                }
            }

            return queryStringList;
        }

        public static List<KeyValuePair<string, string>> GetHeadersFromRequest(HttpActionExecutedContext context)
        {
            var headersList = new List<KeyValuePair<string,
                string>>();

            foreach (var header in context.Request.Headers)
            {
                headersList.Add(new KeyValuePair<string, string>(header.Key, ((string[])(header.Value))[0]));
            }

            return headersList;
        }
    }
}