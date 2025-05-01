using System.Reflection;
using SaxsSpot.NanoSystemGeneration.Engine.Models.Cells;

namespace SaxsSpot.NanoSystemGeneration.Engine.Models.CellFactories;

public abstract class CellFactory
{
    public abstract Cell MakeCell(CellCoordinates cellCoordinates, float xBorder, float yBorder, float zBorder);

    public static CellFactory GetFactory(Type cellType)
    {
        var ourType = typeof(CellFactory);
        
        var list = Assembly.GetAssembly(ourType)?.GetTypes()
            .Where(type => type.IsSubclassOf(ourType)).ToList();

        if (list is null || list!.Count == 0) throw new ArgumentException("Factory not found");
        
        foreach (var item in list)
        {
            var methods = item.GetMethods();

            if (methods.Any(method => method.ReturnType == cellType))
            {
                return (CellFactory)Activator.CreateInstance(item)!
                       ?? throw new Exception("Factory not found");
            }
        }

        throw new ArgumentException("Factory not found");
    }
}