using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;

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
}