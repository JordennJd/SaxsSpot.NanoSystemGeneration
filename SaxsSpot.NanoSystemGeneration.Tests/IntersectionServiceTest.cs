using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;

namespace SaxsSpot.NanoSystemGeneration.Tests;

public class IntersectionServiceTest
{
    [Test]
    public async Task ShouldNotIntersect()
    {
        var p1 = new Parallelepiped
            (2.7481294f, 1f, 20.82116f, 12.036571f, 6.537211f, 1.4600463f, -0.20530568f, 0.9430463f);
        
        var p2 = new Parallelepiped
            (2.3976634f, 1f, 22.755661f, 11.290086f, 4.8066974f, 0.2009636f, 0.54676735f, -0.33895788f);

        await Assert.ThatAsync(() => Task.FromResult(IntersectionService.IsIntersect(p1, p2)), Is.False);
    }
}