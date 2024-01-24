using GmWeb.Logic.Utility.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace GmWeb.Web.Api;
public class Program : GmProgram
{
    public static void Main(string[] args) => CreateWebHostBuilder<Startup>(args).Build().Run();
}
