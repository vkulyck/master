using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Services.Deltas;
namespace GmWeb.Logic.Data.Context.Carma;

public partial class CarmaContext : BaseCarmaContext
{
    private readonly DeltaService _deltas;
    public DeltaService DeltaService => this._deltas;
    public CarmaContext()
    {
        _deltas = DeltaService.Instance;
    }
    public CarmaContext(DbContextOptions<CarmaContext> options) : base(options)
    {
        _deltas = DeltaService.Instance;
    }
    public CarmaContext(DeltaService deltas, DbContextOptions<CarmaContext> options) : base(options)
    {
        _deltas = deltas;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

}
