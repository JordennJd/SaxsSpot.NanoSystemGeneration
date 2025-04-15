using System.Numerics;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;

namespace SaxsSpot.NanoSystemGeneration.Contracts.Models;

public record Parallelepiped(float A, float E, float X = 0, float Y = 0, float Z = 0, float Phi = 0, float Theta = 0,
    float Zenit = 0) : Particle(X, Y, Z)
{
    public float Phi { get; private set; } = Phi;
    
    public float Theta { get; private set; } = Theta;
    
    public float Zenit { get; private set; } = Zenit;

    public override ParticleKind ParticleKind { get; init; } = ParticleKind.Parallelepiped;
    
    public override float GetVolume()
    
    {
        if(_volume == -1)
        {
            _volume = MathF.Pow(A*A*A*E, 3);
        }
		
        return _volume;    
    }

    public override float GetDiameter()
    {
        return MathF.Sqrt(A * A + A * E + A * A);
    }


    public override void ChangePosition(float x, float y, float z, float fi = 0, float theta = 0, float zenit = 0)
    {
        X = x;
        Y = y;
        Z = z;
        Phi = fi;
        Theta = theta;
        Zenit = zenit;
        
    }

    public override string ToString()
    {
        return $"{A} {E} {X} {Y} {Z} {Phi} {Theta} {Zenit}";
    }
}