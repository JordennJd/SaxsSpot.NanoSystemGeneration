using System.Numerics;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;

namespace SaxsSpot.NanoSystemGeneration.Contracts.Models;

public record Parallelepiped(float A, float E, float X = 0, float Y = 0, float Z = 0, float Phi = 0, float Theta = 0,
    float Zenit = 0) : Particle(X, Y, Z)
{
    public float Phi { get; private set; } = Phi;
    
    public float Theta { get; private set; } = Theta;
    
    public float Zenit { get; private set; } = Zenit;

    internal ParallelepipedCoordinates? Coordinates { get; set; }
        
    public override ParticleKind ParticleKind { get; init; } = ParticleKind.Parallelepiped;
    
    public override float GetVolume()
    
    {
        if(_volume == -1)
        {
            _volume = A*A*A*E;
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
    
    public override float GetParticleSize() => A;

    public override string ToString()
    {
        return $"{A} {E} {X} {Y} {Z} {Phi} {Theta} {Zenit}";
    }
    
    public Vector3[] GetVertices()
    {
        Vector3[] localVertices = new Vector3[8];
        float halfSide = A / 2;

        Matrix4x4 Rotation = Matrix4x4.CreateFromYawPitchRoll(Theta, Phi, Zenit);

        Vector3 Center = new Vector3(X, Y, Z);

        localVertices[0] = new Vector3(-halfSide, -halfSide, -halfSide * E);
        localVertices[1] = new Vector3(halfSide, -halfSide, -halfSide * E);
        localVertices[2] = new Vector3(halfSide, halfSide, -halfSide * E);
        localVertices[3] = new Vector3(-halfSide, halfSide, -halfSide * E);
        localVertices[4] = new Vector3(-halfSide, -halfSide, halfSide * E);
        localVertices[5] = new Vector3(halfSide, -halfSide, halfSide * E);
        localVertices[6] = new Vector3(halfSide, halfSide, halfSide * E);
        localVertices[7] = new Vector3(-halfSide, halfSide, halfSide * E);

        for (int i = 0; i < 8; i++)
        {
            localVertices[i] = Vector3.Transform(localVertices[i], Rotation) + Center;
        }

        return localVertices;
    }
}