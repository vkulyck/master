using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

using CommonDbContext = GmWeb.Logic.Data.Context.Carma.CarmaContext;
using User = GmWeb.Logic.Data.Models.Carma.User;
using Agency = GmWeb.Logic.Data.Models.Carma.Agency;
using ActivityCalendar = GmWeb.Logic.Data.Models.Carma.ActivityCalendar;

using Microsoft.AspNetCore.Identity;
using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Data.Models.Carma;
using Gender = GmWeb.Logic.Enums.Gender;
using PrimaryLanguages = GmWeb.Logic.Services.Datasets.Languages.PrimaryLanguages;
using EnumExtensions = GmWeb.Logic.Utility.Extensions.Enums.EnumExtensions;

namespace GmWeb.Tests.Api.Services
{
    public class LoggingService
    {
        public ILogger<ConsoleLoggerProvider> AppLogger { get; private set; }
        public ILoggerFactory LoggerFactory { get; private set; }
        public LoggingService(ILoggerFactory factory)
        {
            this.LoggerFactory = factory;
            this.AppLogger = factory.CreateLogger<ConsoleLoggerProvider>();
        }
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder => builder
                .AddConsole()
                .AddFilter(level => level >= LogLevel.Trace)
            );
        }
    }
}
