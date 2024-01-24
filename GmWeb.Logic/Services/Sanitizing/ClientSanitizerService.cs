using GmWeb.Logic.Data.Context.Carma;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Bogus;

namespace GmWeb.Logic.Services.Sanitizing;
public class ClientSanitizerService
{
    private readonly ILogger<ClientSanitizerService> _logger;
    private readonly CarmaCache _cache;

    public ClientSanitizerService(CarmaContext context, ILoggerFactory factory)
    {
        _logger = factory.CreateLogger<ClientSanitizerService>();
        _cache = new CarmaCache(context);
    }

    public async Task RunAsync()
    {
        var chunks = _cache.Users.Where(x => x.UserID > 1).ToList().Chunk(50);
        foreach (var chunk in chunks)
        {
            _logger.LogInformation($"Processing user IDs: {chunk.First().UserID} to {chunk.Last().UserID}");
            foreach(var client in chunk)
            {
                var faker = new Faker();
                client.FirstName = faker.Person.FirstName;
                client.LastName = faker.Person.LastName;
                client.Phone = $"415-555-{faker.Random.Int(0, 9999):D4}";
                client.Gender = faker.Random.Bool() ? Enums.Gender.Male : Enums.Gender.Female;
            }
            await _cache.SaveAsync();
        }
    }
}