using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;

namespace Angelo.Connect.Web {
    public class Program {
        public static void Main(string[] args) {

            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls(
                    "http://localhost:60000", 
                    "http://localhost:60010",
                    "http://localhost:60011",
                    "http://localhost:60012",
                    "http://localhost:60013",
                    "http://localhost:60020",
                    "http://localhost:60021",
                    "http://localhost:60022",
                    "http://localhost:60030",
                    "http://localhost:60031"
                )
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
