using System;
using System.Net;

namespace Janus.API
{
    class HTTPHandler : IHTTPHandler
    {
        public IAsyncResult Get(string uri, AsyncCallback cb)
        {
            var request = HttpWebRequest.Create(uri);
            request.Method = "GET";
            var state = new Tuple<WebRequest>(request);

            return request.BeginGetResponse(cb, state);
        }
    }
}