using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    #region member fields

    PathIllustrator illustrator;
    [SerializeField]
    LayerMask tileMask;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if(illustrator == null)
        {
            illustrator = GetComponent<PathIllustrator>();
        }
    }

    /// <summary>
    /// Main pathfinding function, marks tiles as being in frontier, while keeping a copy of the frontier
    /// in "currentFrontier" for later clearing
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public Path FindPath(Tile origin, Tile destination)
    {
        List<Tile> openSet = new List<Tile>();
        List<Tile> closedSet = new List<Tile>();

        openSet.Add(origin);
        origin.costFromOrigin = 0;

        float tileDistance = origin.GetComponent<MeshFilter>().sharedMesh.bounds.extents.z * 2;

        while(openSet.Count > 0)
        {
            openSet.Sort((x, y) => x.TotalCost.CompareTo(y.TotalCost));
            Tile currentTile = openSet[0];

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            // Destination reached
            if(currentTile == destination)
            {
                return PathBetween(destination, origin);
            }

            foreach(Tile neighbor in NeighborTiles(currentTile))
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                float costToNeighbor = currentTile.costFromOrigin + neighbor.terrainCost + tileDistance;
                if(costToNeighbor < neighbor.costFromOrigin || !openSet.Contains(neighbor))
                {
                    neighbor.costFromOrigin = costToNeighbor;
                    neighbor.costToDestination = Vector3.Distance(destination.transform.position, neighbor.transform.position);
                    neighbor.parent = currentTile;

                    if(!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }


    /// <summary>
    /// Returns a list of all neighboring hexagonal tiles and ladders
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    private List<Tile> NeighborTiles(Tile origin)
    {
        List<Tile> tiles = new List<Tile>();
        Vector3 direction = Vector3.forward * (origin.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x * 2);
        float rayLength = 4f;
        float rayHeightOffset = 1f;

        // Rotatea raycast in 90 degree steps and find all adjacent tiles
        for(int i = 0; i < 8; i++)
        {
            direction = Quaternion.Euler(0f, 45f, 0f) * direction;

            Vector3 aboveTilePos = (origin.transform.position + direction).With(y: origin.transform.position.y + rayHeightOffset);

            if(Physics.Raycast(aboveTilePos, Vector3.down, out RaycastHit hit, rayLength, tileMask))
            {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                if(hitTile.Occupied == false)
                {
                    tiles.Add(hitTile);
                }
            }

            Debug.DrawRay(aboveTilePos, Vector3.down * rayLength, Color.blue);
        }

        // Additionally add connected tiles such as ladders
        if(origin.connectedTile != null)
        {
            tiles.Add(origin.connectedTile);
        }
        return tiles;
    }

    /// <summary>
    /// Called by Interact.cs to create a path between two tiles on the grid
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public Path PathBetween(Tile destination, Tile source)
    {
        Path path = MakePath(destination, source);
        illustrator.IllustratePath(path);
        return path;
    }


    /// <summary>
    /// Creates a path between two tiles
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    private Path MakePath(Tile destination, Tile origin)
    {
        List<Tile> tiles = new List<Tile>();
        Tile current = destination;

        while(current != origin)
        {
            tiles.Add(current);
            if(current.parent != null)
            {
                current = current.parent;
            }
            else
            {
                break;
            }
        }

        tiles.Add(origin);
        tiles.Reverse();

        Path path = new Path();
        path.tiles = tiles.ToArray();

        return path;
    }
}
