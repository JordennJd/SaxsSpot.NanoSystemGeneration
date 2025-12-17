using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;

namespace SaxsSpot.NanoSystemGeneration.Engine.Models.TernaryTree;

public class TernaryTreeNodeParallelepiped
{
    private TernaryNodeCoordinates _cellCoordinates;
    private float _size;
    private IList<Parallelepiped> _particles = new List<Parallelepiped>();
    private IList<TernaryTreeNodeParallelepiped>? _children;
    private TernaryTreeNodeParallelepiped? _parent;
    private IList<TernaryTreeNodeParallelepiped?> _neighbors = new List<TernaryTreeNodeParallelepiped?>();
    private readonly Parallelepiped _cubeBound;

    public TernaryTreeNodeParallelepiped(float minNodeSize, double size, TernaryNodeCoordinates? cellCoordinates = null, TernaryTreeNodeParallelepiped? parent = null)
    {
        _cellCoordinates = cellCoordinates ?? new TernaryNodeCoordinates(0,0,0);
        _size = (float)size;
        _parent = parent;
        _cubeBound = new Parallelepiped(_size, 1, _cellCoordinates!.X, _cellCoordinates.Y, _cellCoordinates.Z);
        if (size / 3 < minNodeSize)
        {
            return;
        }

        float childSize = _size / 3;
        float offset = _size / 3;

        _children = new List<TernaryTreeNodeParallelepiped>();

        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                for (var z = -1; z <= 1; z++)
                {
                    var childCoord = new TernaryNodeCoordinates(
                        _cellCoordinates.X + x * offset,
                        _cellCoordinates.Y + y * offset,
                        _cellCoordinates.Z + z * offset
                    );

                    _children.Add(new TernaryTreeNodeParallelepiped(minNodeSize, childSize, childCoord, this));
                }
            }
        }

        if (parent is null)
        {
            var deepestNodes = new List<TernaryTreeNodeParallelepiped>();
            GetDeepestNodes(this, deepestNodes);

            foreach (var node in deepestNodes)
            {
                for (var x = -1; x <= 1; x++)
                {
                    for (var y = -1; y <= 1; y++)
                    {
                        for (var z = -1; z <= 1; z++)
                        {
                            var neighbor = deepestNodes.Find(deepNode =>
                                deepNode != node &&
                                Math.Abs(node._cellCoordinates.X + x * node._size - deepNode._cellCoordinates.X) <
                                0.1 &&
                                Math.Abs(node._cellCoordinates.Y + y * node._size - deepNode._cellCoordinates.Y) <
                                0.1 &&
                                Math.Abs(node._cellCoordinates.Z + z * node._size - deepNode._cellCoordinates.Z) < 0.1
                            );
                            
                            if(neighbor is not null) node._neighbors.Add(neighbor);
                        }
                    }
                }
            }
        }
    }

    public bool TryInsert(Parallelepiped particle, GenerationInfo? info = null)
    {
        TernaryTreeNodeParallelepiped deepestNodeParallelepipedForParticle = FindDeepestNodeForParticle(this, particle);
        var nearParticles = deepestNodeParallelepipedForParticle.GetParticles();
        var toCheck = deepestNodeParallelepipedForParticle._neighbors.SelectMany(x => x.GetParticles());
        foreach (var nearParticle in nearParticles)
        {
            info?.IncrementIsInterCenterDistanceLessThenSidesCheckTimesTotal();
            if (IntersectionService.IsInterCenterDistanceLessThenSidesCheck(nearParticle, particle))
            {
                particle.BackRotateMatrix = null;
                particle.IsEdgesRotated = false;
                particle.Edges = null;
                particle.Borders = null;
                particle.IsParticleInside = false;
                info?.IncrementIsInterCenterDistanceLessThenSidesCheckTimesPositive();
                return false;
            }
        }
        
        foreach (var nearParticle in toCheck)
        {
            info?.IncrementIsInterCenterDistanceLessThenSidesCheckTimesTotal();
            if (IntersectionService.IsInterCenterDistanceLessThenSidesCheck(nearParticle, particle))
            {
                particle.BackRotateMatrix = null;
                particle.IsEdgesRotated = false;
                particle.Edges = null;
                particle.Borders = null;
                particle.IsParticleInside = false;
                info?.IncrementIsInterCenterDistanceLessThenSidesCheckTimesPositive();
                return false;
            }
        }

        var isAllNotIntersected = true;
        foreach (var nearParticle in nearParticles)
        {
            info?.IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesTotal();
            if (!IntersectionService.IsInterCenterDistanceMoreThenDiagonalCheck(nearParticle, particle))
            {
                particle.BackRotateMatrix = null;
                particle.IsEdgesRotated = false;
                particle.Edges = null;
                particle.Borders = null;
                particle.IsParticleInside = false;
                info?.IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesPositive();
                isAllNotIntersected = false;
                break;
            }
        }

        if (isAllNotIntersected)
        {
            foreach (var nearParticle in toCheck)
            {
                info?.IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesTotal();
                if (!IntersectionService.IsInterCenterDistanceMoreThenDiagonalCheck(nearParticle, particle))
                {
                    particle.BackRotateMatrix = null;
                    particle.IsEdgesRotated = false;
                    particle.Edges = null;
                    particle.Borders = null;
                    particle.IsParticleInside = false;
                    info?.IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesPositive();
                    isAllNotIntersected = false;
                    break;
                }
            }
            if (isAllNotIntersected)
            {
                deepestNodeParallelepipedForParticle._particles.Add(particle);
                return true;
            }
        }
        foreach (var nearParticle in nearParticles)
        {
            info?.IncrementElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal();
            var newCord = ParallelepipedManipulator.ParallelepipedToParallelepipedCoordinates(particle).Copy();
            ParallelepipedManipulator.DoParallelepipedTransform(ref newCord, -nearParticle.X, -nearParticle.Y, -nearParticle.Z);
            ParallelepipedManipulator.DoBackParallelepipedRotate(ref newCord, nearParticle);
            if (IntersectionService.ElementaryIntersectCheckOnlyBorders(nearParticle, newCord))
            {
                info?.IncrementElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive();
                particle.BackRotateMatrix = null;
                particle.IsEdgesRotated = false;
                particle.Edges = null;
                particle.Borders = null;
                particle.IsParticleInside = false;
                return false;
            }
		
            info?.IncrementElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal();
            var oldCord = ParallelepipedManipulator.ParallelepipedToParallelepipedCoordinates(nearParticle).Copy();
            ParallelepipedManipulator.DoParallelepipedTransform(ref oldCord, -particle.X, -particle.Y, -particle.Z);
            ParallelepipedManipulator.DoBackParallelepipedRotate(ref oldCord, particle);
            if (IntersectionService.ElementaryIntersectCheckOnlyBorders(particle, oldCord))
            {
                info?.IncrementElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive();
                particle.BackRotateMatrix = null;
                particle.IsEdgesRotated = false;
                particle.Edges = null;
                particle.Borders = null;
                particle.IsParticleInside = false;
                return false;
            }
        }
        foreach (var nearParticle in toCheck)
        {
            info?.IncrementElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal();
            var newCord = ParallelepipedManipulator.ParallelepipedToParallelepipedCoordinates(particle).Copy();
            ParallelepipedManipulator.DoParallelepipedTransform(ref newCord, -nearParticle.X, -nearParticle.Y, -nearParticle.Z);
            ParallelepipedManipulator.DoBackParallelepipedRotate(ref newCord, nearParticle);
            if (IntersectionService.ElementaryIntersectCheckOnlyBorders(nearParticle, newCord))
            {
                info?.IncrementElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive();
                particle.BackRotateMatrix = null;
                particle.IsEdgesRotated = false;
                particle.Edges = null;
                particle.Borders = null;
                particle.IsParticleInside = false;
                return false;
            }
		
            info?.IncrementElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal();
            var oldCord = ParallelepipedManipulator.ParallelepipedToParallelepipedCoordinates(nearParticle).Copy();
            ParallelepipedManipulator.DoParallelepipedTransform(ref oldCord, -particle.X, -particle.Y, -particle.Z);
            ParallelepipedManipulator.DoBackParallelepipedRotate(ref oldCord, particle);
            if (IntersectionService.ElementaryIntersectCheckOnlyBorders(particle, oldCord))
            {
                info?.IncrementElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive();
                particle.BackRotateMatrix = null;
                particle.IsEdgesRotated = false;
                particle.Edges = null;
                particle.Borders = null;
                particle.IsParticleInside = false;
                return false;
            }
        }
        foreach (var nearParticle in nearParticles)
        {
            if (SAT.IsIntersect(nearParticle, particle))
            {
                return false;
            }
        }
        foreach (var nearParticle in toCheck)
        {
            if (SAT.IsIntersect(nearParticle, particle))
            {
                return false;
            }
        }
        
        deepestNodeParallelepipedForParticle._particles.Add(particle);
        return true;
    }
    
    public bool TryInsertParticle(Parallelepiped particle, GenerationInfo? info = null)
    {
        TernaryTreeNodeParallelepiped deepestNodeParallelepipedForParticle = FindDeepestNodeForParticle(this, particle);
        var nearParticles = deepestNodeParallelepipedForParticle.GetParticles();

        if (nearParticles.Any(particleToCheck => IntersectionService.IsIntersect(particleToCheck, particle, info: info)))
        {
            info?.IncrementFirstNodeIntersectionFindTimes();
            return false;
        }
        if (deepestNodeParallelepipedForParticle._neighbors
                .SelectMany(x => x.GetParticles()).Any(particleToCheck => IntersectionService.IsIntersect(particleToCheck, particle, info: info)))
        {
            info?.IncrementTotalNeighborsNodesCheckedCount();
            return false;
        }
        
        deepestNodeParallelepipedForParticle._particles.Add(particle);
        
        return true;
    }

    public TernaryTreeNodeParallelepiped FindDeepestNodeForParticle(TernaryTreeNodeParallelepiped root, Parallelepiped particle)
    {
        TernaryTreeNodeParallelepiped current = root;

        while (current._children != null)
        {
            bool found = false;
            foreach (var child in current._children)
            {
                if (child.Contains(particle))
                {
                    current = child;
                    found = true;
                    break;
                }
            }

            if (!found)
                break;
        }

        return current;
    }

    public IEnumerable<Parallelepiped> GetParticles()
    {
        foreach (var particle in _particles)
        {
            yield return particle;
        }

        if (_children is null) yield break;
        
        foreach (var particle in _children.SelectMany(x => x.GetParticles()))
        {
            yield return particle;
        }
    }

    public bool IsParticleInside(Parallelepiped particle)
    {
        // if (particle.ParticleKind == ParticleKind.Parallelepiped)
        // {
        //     if (!IntersectionService.IsInterCenterDistanceMoreThenDiagonalCheckForNodes((Parallelepiped)particle,  _cubeBound))
        //     {
        //         return true;
        //     }
        //
        //     return false;
        // }

        // if (!IntersectionService.IsInterCenterDistanceMoreThenDiagonalCheckForNodesParallelepiped(particle,  _cubeBound))
        // {
        //     return true;
        // }
        return false;
        
    }

    private bool Contains(Particle p)
    {
        float halfSize = _size / 2;
        return
            p.X >= _cellCoordinates.X - halfSize &&
            p.X <= _cellCoordinates.X + halfSize &&
            p.Y >= _cellCoordinates.Y - halfSize &&
            p.Y <= _cellCoordinates.Y + halfSize &&
            p.Z >= _cellCoordinates.Z - halfSize &&
            p.Z <= _cellCoordinates.Z + halfSize;
    }

    private void GetDeepestNodes(TernaryTreeNodeParallelepiped current, List<TernaryTreeNodeParallelepiped> result)
    {
        if (current._children is null)
        {
            result.Add(current);
            return;
        }
        
        foreach (var child in current._children)
        {
            GetDeepestNodes(child, result);
        }
    }

    public Parallelepiped GetCubeBound()
    {
        return _cubeBound;
    }
}