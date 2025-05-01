using SaxsSpot.NanoSystemGeneration.Contracts.Models;

namespace SaxsSpot.NanoSystemGeneration.Engine.Models.Cells;

public record InMemoryCell(CellCoordinates CellCoordinates, CellCoordinates[] NearCells,
    float XBorder, float YBorder, float ZBorder) : Cell(CellCoordinates, NearCells, XBorder, YBorder, ZBorder)
{
    private List<Particle> Items { get; } = new();
    
    public override IEnumerable<Particle> GetParticles()
    {
        return Items;
    }

    public override void Add(Particle parallelepiped)
    {
        Items.Add(parallelepiped);
    }
    
    public override void Delete()
    {
        Items.Clear();
    }
}