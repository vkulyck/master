using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Services.Importing.Clients;
namespace GmWeb.Logic.Utility.Mapping;
public class EntityMappingFactory : MappingFactory
{
    private static EntityMappingFactory _instance;
    public static EntityMappingFactory Instance => _instance ?? (_instance = new EntityMappingFactory());
    public EntityMappingFactory()
    {
        this.AddProfile<EntityMappingProfile>();
        this.AddProfile<CarmaMappingProfile>();
    }
}