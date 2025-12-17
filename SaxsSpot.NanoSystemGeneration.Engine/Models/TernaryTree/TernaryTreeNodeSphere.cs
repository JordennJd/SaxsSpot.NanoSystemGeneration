using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;

namespace SaxsSpot.NanoSystemGeneration.Engine.Models.TernaryTree;

public class TernaryTreeNodeSphere
{
    private TernaryNodeCoordinates _cellCoordinates;
    private float _size;
    private IList<Sphere> _particles = new List<Sphere>();
    private IList<TernaryTreeNodeSphere>? _children;
    private TernaryTreeNodeSphere? _parent;
    private IList<TernaryTreeNodeSphere?> _neighbors = new List<TernaryTreeNodeSphere?>();
    private readonly Parallelepiped _cubeBound;

    public TernaryTreeNodeSphere(float minNodeSize, double size, TernaryNodeCoordinates? cellCoordinates = null, TernaryTreeNodeSphere? parent = null)
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

        _children = new List<TernaryTreeNodeSphere>();

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

                    _children.Add(new TernaryTreeNodeSphere(minNodeSize, childSize, childCoord, this));
                }
            }
        }

        if (parent is null)
        {
            var deepestNodes = new List<TernaryTreeNodeSphere>();
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

    public bool TryInsertParticle(Sphere particle, GenerationInfo? info = null)
    {
        TernaryTreeNodeSphere deepestNodeSphereForParticle = FindDeepestNodeForParticle(this, particle);
        var nearParticles = deepestNodeSphereForParticle.GetParticles();

        if (nearParticles.Any(particleToCheck => IntersectionService.IsSphereIntersect(particleToCheck, particle, info)))
        {
            info?.IncrementFirstNodeIntersectionFindTimes();
            return false;
        }
        if (deepestNodeSphereForParticle._neighbors
                .Where(node => node?.IsParticleInside(particle) is true)
                .SelectMany(x => x.GetParticles()).Any(particleToCheck => IntersectionService.IsSphereIntersect(particleToCheck, particle, info)))
        {
            info?.IncrementTotalNeighborsNodesCheckedCount();
            return false;
        }
        
        deepestNodeSphereForParticle._particles.Add(particle);
        
        return true;
    }

    public TernaryTreeNodeSphere FindDeepestNodeForParticle(TernaryTreeNodeSphere root, Sphere particle)
    {
        TernaryTreeNodeSphere current = root;

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

    public IEnumerable<Sphere> GetParticles()
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

    public bool IsParticleInside(Sphere particle)
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

        if (!IntersectionService.IsInterCenterDistanceMoreThenDiagonalCheckForNodesSphere(particle,  _cubeBound))
        {
            return true;
        }
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

    private void GetDeepestNodes(TernaryTreeNodeSphere current, List<TernaryTreeNodeSphere> result)
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

public record TernaryNodeCoordinates(float X, float Y, float Z);