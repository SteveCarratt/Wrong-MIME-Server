using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace WrongMimeServer
{
    class Program
    {
        private static Dictionary<string,string> _extensionMappings = new Dictionary<string, string>()
        {
            {".html", "text/html"},
            {".js", "text/plain"}

        };

        static void Main(string[] args)
        {

            var listener = new HttpListener();
            
            listener.Start();
            listener.Prefixes.Add("http://localhost/");
            do
            {
                var context = listener.GetContext();
                var request = context.Request;

                var response = context.Response;

                var output = response.OutputStream;


                var localPath = request.Url.LocalPath.TrimStart('/');

                var requestedFile = Path.Combine(Environment.CurrentDirectory, string.IsNullOrEmpty(localPath)? "index.html" : localPath);

                if (!File.Exists(requestedFile))
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Headers.Add("content-type", _extensionMappings[Path.GetExtension(requestedFile)]);
                    response.Headers.Add("X-Content-Type-Options", "nosniff");
                    var responseBytes = Encoding.UTF8.GetBytes(File.ReadAllText(requestedFile));
                    output.Write(responseBytes, 0, responseBytes.Length);
                }
                
                output.Close();
                response.Close();

            } while (true);
        }
    }
}
