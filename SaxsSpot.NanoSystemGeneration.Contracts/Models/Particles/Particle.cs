using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;

namespace SaxsSpot.NanoSystemGeneration.Contracts.Models;

/// <summary>
/// Represent a particle of system
/// </summary>
/// <param name="X"></param>
/// <param name="Y"></param>
/// <param name="Z"></param>
public abstract record Particle(float X, float Y, float Z)
{
    public float X { get; protected set; }
    
    public float Y { get; protected set; }
    
    public float Z { get; protected set; }
    
    protected float _volume = -1;

    public abstract float GetVolume();
    
    public abstract float GetDiameter();

    public abstract ParticleKind ParticleKind { get; init; }
    
    public abstract void ChangePosition(float x , float y, float z, float fi = 0, float theta = 0, float zenit = 0);
}