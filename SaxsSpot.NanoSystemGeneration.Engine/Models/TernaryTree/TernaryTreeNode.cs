using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;
using SaxsSpot.NanoSystemGeneration.Engine.Models.Cells;

namespace SaxsSpot.NanoSystemGeneration.Engine.Models.QuadTree;

public class TernaryTreeNode
{
    private TernaryNodeCoordinates _cellCoordinates;
    private float _size;
    private IList<Particle> _particles = new List<Particle>();
    private IList<TernaryTreeNode>? _children;
    private TernaryTreeNode? _parent;
    private IList<TernaryTreeNode?> _neighbors = new List<TernaryTreeNode?>();

    public TernaryTreeNode(float minNodeSize, float size, TernaryNodeCoordinates? cellCoordinates = null, TernaryTreeNode? parent = null)
    {
        _cellCoordinates = cellCoordinates ?? new TernaryNodeCoordinates(0,0,0);
        _size = size;
        _parent = parent;

        if (size / 3 < minNodeSize)
        {
            return;
        }

        float childSize = size / 3;
        float offset = size / 3;

        _children = new List<TernaryTreeNode>();

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

                    _children.Add(new TernaryTreeNode(minNodeSize, childSize, childCoord, this));
                }
            }
        }

        if (parent is null)
        {
            var deepestNodes = new List<TernaryTreeNode>();
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

    public bool TryInsertParticle(Particle particle)
    {
        TernaryTreeNode deepestNodeForParticle = FindDeepestNodeForParticle(this, particle);
        ArgumentNullException.ThrowIfNull(deepestNodeForParticle, "Particle inbound of generation zone");
        var nearParticles = deepestNodeForParticle.GetParticles();

        if (nearParticles.Any(particle.IsIntersect)
            || deepestNodeForParticle._neighbors.SelectMany(x => x.GetParticles()).Any(particle.IsIntersect))
        {
            return false;
        }
        
        deepestNodeForParticle._particles.Add(particle);
        
        return true;
    }

    public TernaryTreeNode FindDeepestNodeForParticle(TernaryTreeNode root, Particle particle)
    {
        TernaryTreeNode current = root;

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

    public IEnumerable<Particle> GetParticles()
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

    private void GetDeepestNodes(TernaryTreeNode current, List<TernaryTreeNode> result)
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
}

public record TernaryNodeCoordinates(float X, float Y, float Z);