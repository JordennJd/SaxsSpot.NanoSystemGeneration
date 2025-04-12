using MathNet.Numerics.LinearAlgebra;

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
}