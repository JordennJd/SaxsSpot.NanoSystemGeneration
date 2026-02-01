using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex32;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationInfo;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;

namespace SaxsSpot.NanoSystemGeneration.Engine.Models.TernaryTree;

public class TernaryTreeNode
{
    private TernaryNodeCoordinates _cellCoordinates;
    private float _size;
    private IList<Particle> _particles = new List<Particle>();
    private IList<TernaryTreeNode>? _children;
    private TernaryTreeNode? _parent;
    public IList<TernaryTreeNode?> _neighbors = new List<TernaryTreeNode?>();
    private readonly Parallelepiped _cubeBound;

    public TernaryTreeNode(float minNodeSize, double size, TernaryNodeCoordinates? cellCoordinates = null, TernaryTreeNode? parent = null)
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

    public void InsertParticle(Particle particle, GenerationInfo? info = null, ParticleGenerationInfo? particleInfo = null)
    {
        TernaryTreeNode deepestNodeSphereForParticle = FindDeepestNodeForParticle(this, particle);
        
        deepestNodeSphereForParticle._particles.Add(particle);
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
    
    public TernaryTreeNode FindDeepestNodeForVector(TernaryTreeNode root, Vector<float> vector)
    {
        TernaryTreeNode current = root;

        while (current._children != null)
        {
            bool found = false;
            foreach (var child in current._children)
            {
                if (child.Contains(vector))
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
    
    private bool Contains(Vector<float> p)
    {
        float halfSize = _size / 2;
        return
            p[0] >= _cellCoordinates.X - halfSize &&
            p[0] <= _cellCoordinates.X + halfSize &&
            p[1] >= _cellCoordinates.Y - halfSize &&
            p[1] <= _cellCoordinates.Y + halfSize &&
            p[2] >= _cellCoordinates.Z - halfSize &&
            p[2] <= _cellCoordinates.Z + halfSize;
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

    public Parallelepiped GetCubeBound()
    {
        return _cubeBound;
    }
}