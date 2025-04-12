using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemGeneration.Entities;

namespace SaxsSpot.NanoSystemGeneration.Storage.DbContexts;

public class NanoSystemDbContext : GenericDbContext<NanoSystem>
{
    public NanoSystemDbContext(IConfiguration configuration) : base(configuration)
    {
    }
}