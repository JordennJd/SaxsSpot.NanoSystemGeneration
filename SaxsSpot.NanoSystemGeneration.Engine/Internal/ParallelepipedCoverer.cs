using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex32;

namespace SaxsSpot.NanoSystemGeneration.Engine.Internal;

internal static  class ParallelepipedCoverer
{
    /// <summary>
    ///     Fills the surface of a parallelepiped with vectors based on the given coordinates and density.
    /// </summary>
    /// <param name="par">The coordinates of the parallelepiped.</param>
    /// <param name="density">The density of the vectors.</param>
    /// <returns>A matrix of vectors representing the filled surface.</returns>
    public static async Task<List<List<List<Vector<float>>>>> FillSurface(ParallelepipedCoordinates par, int density)
    {
        return await FillSurfaceFromCoordinates(par, density);;
    }

    public static List<List<Vector<float>>> FillBorders(ParallelepipedCoordinates par, int density)
    {
        var MatrixOfVectors = new List<List<Vector<float>>>();
        var Border1 = ToLinearVectors(par.A, par.B, density);
        var Border2 = ToLinearVectors(par.A, par.D, density);
        var Border3 = ToLinearVectors(par.A, par.A1, density);
        var Border4 = ToLinearVectors(par.C, par.D, density);
        var Border5 = ToLinearVectors(par.C, par.B, density);
        var Border6 = ToLinearVectors(par.C, par.C1, density);
        var Border7 = ToLinearVectors(par.D1, par.D, density);
        var Border8 = ToLinearVectors(par.D1, par.A1, density);
        var Border9 = ToLinearVectors(par.D1, par.C1, density);
        var Border10 = ToLinearVectors(par.B1, par.A1, density);
        var Border11 = ToLinearVectors(par.B1, par.B, density);
        var Border12 = ToLinearVectors(par.B1, par.C1, density);
        
        //
        //   C+---+D
        //  /   / |      
        // B+---+A|      
        // | C1   |D1 
        // |    |/ 
        // B1+---+A1	
        
        MatrixOfVectors.Add(Border1);
        MatrixOfVectors.Add(Border2);
        MatrixOfVectors.Add(Border3);
        MatrixOfVectors.Add(Border4);
        MatrixOfVectors.Add(Border5);
        MatrixOfVectors.Add(Border6);
        MatrixOfVectors.Add(Border7);
        MatrixOfVectors.Add(Border8);
        MatrixOfVectors.Add(Border9);
        MatrixOfVectors.Add(Border10);
        MatrixOfVectors.Add(Border11);
        MatrixOfVectors.Add(Border12);
        return MatrixOfVectors;
    }

    /// <summary>
    ///     Fills the surface of a parallelepiped with vectors based on the given coordinates and density.
    /// </summary>
    /// <param name="par">The coordinates of the parallelepiped.</param>
    /// <param name="density">The density of the vectors.</param>
    /// <returns>A matrix of vectors representing the filled surface.</returns>
    private static async Task<List<List<List<Vector<float>>>>> FillSurfaceFromCoordinates(ParallelepipedCoordinates par,
        int density)
    {
        var MatrixOfVectors = new List<List<List<Vector<float>>>>();
        var Up = FillPlane(par.A, par.B, par.C, par.D, density);
        var Bottom = FillPlane(par.A1, par.B1, par.C1, par.D1, density);
        var Front = FillPlane(par.A1, par.A, par.B, par.B1, density);
        var Back = FillPlane(par.D1, par.D, par.C, par.C1, density);
        var Right = FillPlane(par.A1, par.A, par.D, par.D1, density);
        var Left = FillPlane(par.B1, par.B, par.C, par.C1, density);
        //
        //   C+---+D
        //  /   / |      
        // B+---+A|      
        // | C1   |D1 
        // |    |/ 
        // B1+---+A1	
        MatrixOfVectors.Add(await Up);
        MatrixOfVectors.Add(await Bottom);
        MatrixOfVectors.Add(await Front);
        MatrixOfVectors.Add(await Back);
        MatrixOfVectors.Add(await Right);
        MatrixOfVectors.Add(await Left);
        return MatrixOfVectors;
    }

    private static List<Vector<float>> ToLinearVectors(Vector<float> vec1, Vector<float> vec2, int density)
    {
        var xs = Generate.LinearSpaced(density, vec1[0], vec2[0]);
        var ys = Generate.LinearSpaced(density, vec1[1], vec2[1]);
        var zs = Generate.LinearSpaced(density, vec1[2], vec2[2]);
        var list = new List<Vector<float>>();
        for (var i = 0; i < density; i++) list.Add(Vector<float>.Build.DenseOfArray([(float)xs[i], (float)ys[i], (float)zs[i]]));
        return list;
    }

    private static async Task<List<List<Vector<float>>>> FillPlane(Vector<float> vec1, Vector<float> vec2,
        Vector<float> vec3, Vector<float> vec4, int density)
    {
        return await Task.Run(() =>
        {
            var LeftVectors = ToLinearVectors(vec1, vec2, density);
            var RightVectors = ToLinearVectors(vec3, vec4, density);
            var MatrixOfVectors = new List<List<Vector<float>>>();
            for (var i = 0; i < density; i++)
                MatrixOfVectors.Add(ToLinearVectors(LeftVectors[i], RightVectors[density - 1 - i], density));
            return MatrixOfVectors;
        });
    }
}