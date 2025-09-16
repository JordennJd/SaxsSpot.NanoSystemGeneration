using System.Globalization;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;

namespace SaxsSpot.NanoSystemGeneration.Contracts.Models;

public record Sphere(float Radius, float X = 0, float Y = 0, float Z = 0) : Particle(X, Y, Z)
{
    public override ParticleKind ParticleKind { get; init; } = ParticleKind.Sphere;
    
    public override double GetVolume()
    {
        if(_volume == -1)
        {
            _volume = Math.Pow(Radius, 3) * Math.PI * 4d / 3d;
        }
		
        return _volume;
    }

    public override float GetDiameter()
    {
        return 2 * Radius;
    }

    public override void ChangePosition(float x, float y, float z, float fi = 0, float theta = 0, float zenit = 0)
    {
        X = x;
        Y = y;
        Z = z;
    }
    
    public override float GetParticleSize() => Radius;
    
    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture, 
            "{0} {1} {2} {3}", 
            Radius, X, Y, Z);
    }
}