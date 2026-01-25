using System.Globalization;
using System.Numerics;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;
using MathNet.Numerics.LinearAlgebra;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;

namespace SaxsSpot.NanoSystemGeneration.Tests;

public class IntersectionServiceTest
{
    [Test]
    public async Task ShouldNotIntersect()
    {
        var p1 = Parallelepiped.FromString
            ("5.5815296 1 -21.127586 35.804977 -25.133026 0.4084141 -0.16429679 1.493728");
        
        var p2 = Parallelepiped.FromString
            ("4.9370775 1 -20.748661 29.20415 -25.75789 -0.19859473 0.5414058 1.2596422");

        await Assert.ThatAsync(() => Task.FromResult(IntersectionService.IsIntersect(p1, p2)), Is.False);
    }

    [Test]
    public async Task TestIntersectionServiceIsPointInsideParticle()
    {
        var p2 = Parallelepiped.FromString
            ($"4.9370775 1 0 0 0 0 0 0");

        var points = ParallelepipedCoverer.FillGenerationZoneWithPoints(new GenerationZone(4.9370775, GenerationZoneForm.Cube), 100000);
        var c = 0;
        foreach (var point in points)
        {
            if (IntersectionService.IsPointInParallelepiped(point, p2))
            {
                c++;
            }
        }
        
        Console.WriteLine(c);
    }
}