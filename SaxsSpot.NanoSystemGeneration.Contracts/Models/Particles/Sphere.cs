namespace SaxsSpot.NanoSystemGeneration.Contracts.Models;

public record Sphere(float Radius, float X = 0, float Y = 0, float Z = 0) : Particle(X, Y, Z)
{
    public override float GetVolume()
    {
        if(_volume == -1)
        {
            _volume = MathF.Pow(Radius, 3) * MathF.PI * 4f / 3f;
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

    public override bool IsBoundOfCubeZone(Parallelepiped cube)
    {
        return !(cube.A/2 < X + Radius) && !(-cube.A/2 > X - Radius) && !(cube.A/2 < Y + Radius) 
               && !(-cube.A/2 > Y - Radius) && !(cube.A/2 < Z + Radius) && !(-cube.A/2 > Z - Radius);
    }
    
    public override string ToString()
    {
        return $"{Radius} {X} {Y} {Z}";
    }
}