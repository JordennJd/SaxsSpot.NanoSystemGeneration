using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;

namespace SaxsSpot.NanoSystemGeneration.Tests;

public class IntersectionServiceTest
{
    [Test]
    public async Task ShouldNotIntersect()
    {
        var p1 = new Parallelepiped
            (2.772324f, 1f, -79.450584f, 49.394794f, -49.808155f, 0.02957218f, 1.1955575f, 0.9764833f);
        
        var p2 = new Parallelepiped
            (2.7477918f, 1f, -80.17603f, 46.601997f, -48.929157f, -1.0379723f, 0.7851855f, -0.16450155f);

        await Assert.ThatAsync(() => Task.FromResult(IntersectionService.IsIntersect(p1, p2)), Is.False);
    }
}