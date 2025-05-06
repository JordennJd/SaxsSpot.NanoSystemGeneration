using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

namespace SaxsSpot.NanoSystemGeneration.Engine.Internal;

internal record ParallelepipedCoordinates(Vector<float> A, Vector<float> B, Vector<float> C, Vector<float> D,
    Vector<float> A1, Vector<float> B1, Vector<float> C1, Vector<float> D1)
{
    public Vector<float> A { get; set; } = A;
    
    public Vector<float> B { get; set; } = B;
    
    public Vector<float> C { get; set; } = C;
    
    public Vector<float> D { get; set; } = D;
    
    public Vector<float> A1 { get; set; } = A1;
    
    public Vector<float> B1 { get; set; } = B1;
    
    public Vector<float> C1 { get; set; } = C1;
    
    public Vector<float> D1 { get; set; } = D1;
    
    public List<List<Vector<float>>>? CachedBorders { get; set; }

    public IEnumerable<Vector<float>> ForAll()
    {
        yield return A;yield return B;yield return C;yield return D;yield return A1;yield return B1;yield return C1;yield return D1;
    }

    public ParallelepipedCoordinates Copy()
    {
        return new ParallelepipedCoordinates(
            Vector.Build.DenseOfVector(A),
            Vector.Build.DenseOfVector(B),
            Vector.Build.DenseOfVector(C),
            Vector.Build.DenseOfVector(D),
            Vector.Build.DenseOfVector(A1),
            Vector.Build.DenseOfVector(B1),
            Vector.Build.DenseOfVector(C1),
            Vector.Build.DenseOfVector(D1));
    }
}