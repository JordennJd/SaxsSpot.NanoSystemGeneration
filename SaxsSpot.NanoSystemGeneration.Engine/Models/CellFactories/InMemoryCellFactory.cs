using SaxsSpot.NanoSystemGeneration.Engine.Models.Cells;

namespace SaxsSpot.NanoSystemGeneration.Engine.Models.CellFactories;

public class InMemoryCellFactory : CellFactory
{
    public override InMemoryCell MakeCell(CellCoordinates cellCoordinates, float xBorder, float yBorder, float zBorder)
    {
        return new InMemoryCell(cellCoordinates, [], xBorder, yBorder, zBorder);
    }
}