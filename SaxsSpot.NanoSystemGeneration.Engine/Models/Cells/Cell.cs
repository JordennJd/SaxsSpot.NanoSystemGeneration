using SaxsSpot.NanoSystemGeneration.Contracts.Models;

namespace SaxsSpot.NanoSystemGeneration.Engine.Models.Cells;

public abstract record Cell(CellCoordinates Coordinates, CellCoordinates[] NearCells,
    float XBorder, float YBorder, float ZBorder)
{
    public abstract IEnumerable<Particle> GetParticles();
    public abstract void Add(Particle parallelepiped);
    public abstract void Delete();
    public CellCoordinates[] NearCells { get; set; } = NearCells;
}

public record CellCoordinates(int X, int Y, int Z);