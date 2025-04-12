using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemGeneration.Entities;
using SaxsSpot.NanoSystemGeneration.Storage.Contracts;
using SaxsSpot.NanoSystemGeneration.Storage.DbContexts;

namespace SaxsSpot.NanoSystemGeneration.Storage;

public class NanoSystemStorage : GenericStorage<NanoSystem>, INanoSystemStorage
{
    public NanoSystemStorage(NanoSystemDbContext dbContext) : base(dbContext)
    {
    }
}