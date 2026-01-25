using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex32;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;

namespace SaxsSpot.NanoSystemGeneration.Engine.Internal;

internal static  class ParallelepipedCoverer
{
    // /// <summary>
    // ///     Fills the surface of a parallelepiped with vectors based on the given coordinates and density.
    // /// </summary>
    // /// <param name="par">The coordinates of the parallelepiped.</param>
    // /// <param name="density">The density of the vectors.</param>
    // /// <returns>A matrix of vectors representing the filled surface.</returns>
    // public static async Task<List<List<List<Vector<float>>>>> FillSurface(ParallelepipedCoordinates par, int density)
    // {
    //     return await FillSurfaceFromCoordinates(par, density);;
    // }

    public static Vector<float>[][] FillBorders(Parallelepiped par, ParallelepipedCoordinates parcord, int density)
    {
        var borders = new Vector<float>[12][];
        var border1 = ToLinearVectors(parcord.A, parcord.B, density);
        var border2 = ToLinearVectors(parcord.A, parcord.D, density);
        var border3 = ToLinearVectors(parcord.A, parcord.A1, density);
        var border4 = ToLinearVectors(parcord.C, parcord.D, density);
        var border5 = ToLinearVectors(parcord.C, parcord.B, density);
        var border6 = ToLinearVectors(parcord.C, parcord.C1, density);
        var border7 = ToLinearVectors(parcord.D1, parcord.D, density);
        var border8 = ToLinearVectors(parcord.D1, parcord.A1, density);
        var border9 = ToLinearVectors(parcord.D1, parcord.C1, density);
        var border10 = ToLinearVectors(parcord.B1, parcord.A1, density);
        var border11 = ToLinearVectors(parcord.B1, parcord.B, density);
        var border12 = ToLinearVectors(parcord.B1, parcord.C1, density);
        
        //
        //   C+---+D
        //  /   / |      
        // B+---+A|      
        // | C1   |D1 
        // |    |/ 
        // B1+---+A1	
        
        borders[0] = border1;
        borders[1] = border2;
        borders[2] = border3;
        borders[3] = border4;
        borders[4] = border5;
        borders[5] = border6;
        borders[6] = border7;
        borders[7] = border8;
        borders[8] = border9;
        borders[9] = border10;
        borders[10] = border11;
        borders[11] = border12;
        return borders;
    }

    /// <summary>
    ///     Fills the surface of a parallelepiped with vectors based on the given coordinates and density.
    /// </summary>
    /// <param name="par">The coordinates of the parallelepiped.</param>
    /// <param name="density">The density of the vectors.</param>
    /// <returns>A matrix of vectors representing the filled surface.</returns>
    // private static async Task<List<List<List<Vector<float>>>>> FillSurfaceFromCoordinates(ParallelepipedCoordinates par,
    //     int density)
    // {
    //     var MatrixOfVectors = new List<List<List<Vector<float>>>>();
    //     var Up = FillPlane(par.A, par.B, par.C, par.D, density);
    //     var Bottom = FillPlane(par.A1, par.B1, par.C1, par.D1, density);
    //     var Front = FillPlane(par.A1, par.A, par.B, par.B1, density);
    //     var Back = FillPlane(par.D1, par.D, par.C, par.C1, density);
    //     var Right = FillPlane(par.A1, par.A, par.D, par.D1, density);
    //     var Left = FillPlane(par.B1, par.B, par.C, par.C1, density);
    //     //
    //     //   C+---+D
    //     //  /   / |      
    //     // B+---+A|      
    //     // | C1   |D1 
    //     // |    |/ 
    //     // B1+---+A1	
    //     MatrixOfVectors.Add(await Up);
    //     MatrixOfVectors.Add(await Bottom);
    //     MatrixOfVectors.Add(await Front);
    //     MatrixOfVectors.Add(await Back);
    //     MatrixOfVectors.Add(await Right);
    //     MatrixOfVectors.Add(await Left);
    //     return MatrixOfVectors;
    // }

    private static Vector<float>[] ToLinearVectors(Vector<float> vec1, Vector<float> vec2, int density)
    {
        var xs = Generate.LinearSpaced(density, vec1[0], vec2[0]);
        var ys = Generate.LinearSpaced(density, vec1[1], vec2[1]);
        var zs = Generate.LinearSpaced(density, vec1[2], vec2[2]);
        var list = new Vector<float>[density];
        for (var i = 0; i < density; i++) 
            list[i] = Vector<float>.Build.DenseOfArray([(float)xs[i], (float)ys[i], (float)zs[i]]);
        return list;
    }

    // private static async Task<List<List<Vector<float>>>> FillPlane(Vector<float> vec1, Vector<float> vec2,
    //     Vector<float> vec3, Vector<float> vec4, int density)
    // {
    //     return await Task.Run(() =>
    //     {
    //         var LeftVectors = ToLinearVectors(vec1, vec2, density);
    //         var RightVectors = ToLinearVectors(vec3, vec4, density);
    //         var MatrixOfVectors = new List<List<Vector<float>>>();
    //         for (var i = 0; i < density; i++)
    //             MatrixOfVectors.Add(ToLinearVectors(LeftVectors[i], RightVectors[density - 1 - i], density));
    //         return MatrixOfVectors;
    //     });
    // }

    public static Vector<float>[] FillGenerationZoneWithPoints(GenerationZone zone, int count)
    {
        if (count <= 0)
            return [];

        var halfSize = zone.GenerationZoneForm == GenerationZoneForm.Cube ? 
            zone.GlobalSize / 2 : 
            zone.GlobalSize;
    
        int pointsPerAxis = (int)Math.Ceiling(Math.Pow(count, 1f / 3f));
    
        int totalPoints = (int)Math.Pow(pointsPerAxis, 3);
    
        var xValues = Generate.LinearSpaced(pointsPerAxis, -halfSize, halfSize);
        var yValues = Generate.LinearSpaced(pointsPerAxis, -halfSize, halfSize);
        var zValues = Generate.LinearSpaced(pointsPerAxis, -halfSize, halfSize);
    
        var points = new Vector<float>[totalPoints];
        int index = 0;
    
        for (int i = 0; i < pointsPerAxis; i++)
        {
            for (int j = 0; j < pointsPerAxis; j++)
            {
                for (int k = 0; k < pointsPerAxis; k++)
                {
                    points[index] = Vector<float>.Build.Dense([
                        (float)xValues[i], 
                        (float)yValues[j], 
                        (float)zValues[k]
                    ]);
                    index++;
                }
            }
        }
    
        return points;
    }

}