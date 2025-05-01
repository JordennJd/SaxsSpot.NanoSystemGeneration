using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;
using SaxsSpot.NanoSystemGeneration.Engine.Models.CellFactories;
using SaxsSpot.NanoSystemGeneration.Engine.Models.Cells;

namespace SaxsSpot.NanoSystemGeneration.Engine.Services;

public class CellManager<T> where T : Cell
{
    private readonly List<List<List<T>>> _cells;
    
    public CellManager(IList<Particle> particles, float globalSize)
    {
	    var maxSemiDiagonal = particles.MaxBy(x => x.GetVolume())?.GetDiameter();

	    var cellCount = Convert.ToInt32(globalSize / (maxSemiDiagonal * 2)) - 1;

	    cellCount = cellCount > 70 ? 70 : cellCount;
	    var cellSize = globalSize / cellCount;
	    
	    var cellFactory = CellFactory.GetFactory(typeof(T));

	    if (cellCount < 3) throw new ArgumentException("count of cells cant be less then 27");
	    
	    var size = cellSize * cellCount;
        var leftBorder = -size / 2f;
        var rightBorder = size / 2f;

        _cells = new List<List<List<T>>>(cellCount);

        for (var col = leftBorder + cellSize; col <= rightBorder + 0.1; col += cellSize)
        {
            var colums = new List<List<T>>(cellCount);
            
            for (var row = leftBorder+cellSize; row <= rightBorder + 0.1; row+=cellSize)
            {
                var items = new List<T>(cellCount);
                
                for (var cell = leftBorder+cellSize; cell <= rightBorder + 0.1; cell+=cellSize)
                {
                    var singleCell = (T)cellFactory.MakeCell(new CellCoordinates(_cells.Count, colums.Count, items.Count), col, row, cell);
                    
                    singleCell.NearCells = GetNeighborsIndexes(singleCell, cellCount).ToArray();
                    
                    items.Add(singleCell);
                }

                colums.Add(items);
            }

            _cells.Add(colums);
        }
    }

	public IList<Particle> GetParticles()
	{
		var list = new List<Particle>();
		
		foreach (var col in _cells)
		foreach (var row in col)
		foreach (var cell in row)
		foreach (var par in cell.GetParticles()) list.Add(par);
		
		return list;
	}

	public async Task ClearAsync()
	{
		await Task.Run(() =>
		{
			foreach (var cell in _cells.SelectMany(col => col.SelectMany(row => row)))
				cell.Delete();
		});
	}

	public bool TryAddParallelepipedToCell(Particle particle)
	{
		var cell = FindCellForParallelepiped(particle);

		if (cell.GetParticles().Any(particle.IsIntersect) || 
		    GetNeighbors(cell)
			    .SelectMany(x => x.GetParticles())
			    .Any(particle.IsIntersect))
		{
			return false;
		}

		cell.Add(particle);
		
		return true;
	}
	
	private IEnumerable<CellCoordinates> GetNeighborsIndexes(T cell, int cellCount)
	{
		for (var l = cell.CellCoordinates.X - 1; l <= cell.CellCoordinates.X + 1; l++)
		for (var m = cell.CellCoordinates.Y - 1; m <= cell.CellCoordinates.Y + 1; m++)
		for (var n = cell.CellCoordinates.Z - 1; n <= cell.CellCoordinates.Z + 1; n++)
		{
			if (cell.CellCoordinates.X == l && cell.CellCoordinates.Y == m && cell.CellCoordinates.Z == n) continue;
			if (l < 0 || l >= cellCount || m < 0 || m >= cellCount || n < 0 || n >= cellCount) continue;
			yield return new CellCoordinates(l, m, n);
		}
	}

	private IEnumerable<T> GetNeighbors(T cell)
	{
		return cell.NearCells.Select(index => _cells[index.X][index.Y][index.Z]);
	}

	private T FindCellForParallelepiped(Particle parallelepiped) //TODO Rework to tree
	{
		for (var col = 0; col < _cells.Count; col++) // for (var col in Enumerable.Range(0, Cells.Count -1))
			if (parallelepiped.Z <= _cells[0][0][col].ZBorder &&
				(col > 0 ? parallelepiped.Z >= _cells[0][0][col - 1].ZBorder : true))
				for (var row = 0; row < _cells.Count; row++) // for (var row in Enumerable.Range(0, Cells.Count - 1))
					if (parallelepiped.Y <= _cells[0][row][col].YBorder &&
						(row > 0 ? parallelepiped.Y >= _cells[0][row - 1][col].YBorder : true))
						for (var cell = 0;
							 cell < _cells.Count;
							 cell++) // for (var cell in Enumerable.Range(0, Cells.Count - 1))
							if (parallelepiped.X <= _cells[cell][row][col].XBorder &&
								(cell > 0 ? parallelepiped.X >= _cells[cell - 1][row][col].XBorder : true))
								return _cells[cell][row][col];
		
		throw new Exception("Cell not found");
	}
}