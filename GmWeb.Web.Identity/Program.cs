using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using GmWeb.Logic.Utility.Config;

namespace GmWeb.Web.Identity;
public class Program : GmProgram
{
    public static void Main(string[] args) => CreateWebHostBuilder<Startup>(args).Build().Run();
}