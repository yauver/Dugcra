using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class Grid : MonoBehaviour
{
    public GridTile[,] tiles = new GridTile[gridSize, gridSize];

    private Mesh mesh;
    public static int gridSize = 7;
    public int xSize, ySize;
    public bool update;
    public bool save;
    public bool savePacked;
    public bool load;
    public bool isLoaded = false;

    public World world;
    public WorldPos pos;

    MeshFilter filter;
    PolygonCollider2D coll;

    public bool rendered;

    void Start()
    {
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<PolygonCollider2D>();

        filter.mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";
                
        save = false;
        savePacked = false;
    }

    void Update()
    {
        if (update)
        {
            update = false;
            RenewTiles();
            UpdateGrid();
        }
        if (save)
        {
            save = false;
            SaveAndLoadManager.SaveGrid(this, savePacked);
        }
        if (load)
        {
            load = false;
            SaveAndLoadManager.LoadGrid(this);
        }
    }

    public void SetTile(int x, int y, GridTile tile)
    {
        tiles[x, y] = tile;
    }

    public GridTile GetTile(int x, int y)
    {
        return tiles[x, y];
    }
    
    public void RenewTiles()
    {
        GridTile[,] temp = new GridTile[gridSize + 2, gridSize + 2];
        for (int x = 0; x < temp.GetLength(0); x++)
        {
            for (int y = 0; y < temp.GetLength(1); y++)
            {
                temp[x, y] = new GridTile(GridTile.TileTypes.Empty);
            }
        }

        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                temp[x + 1, y + 1] = tiles[x, y];
            }
        }

        for (int x = 1; x < temp.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < temp.GetLength(1) - 1; y++)
            {                
                if (temp[x, y].type == GridTile.TileTypes.Wall)
                {
                    temp[x, y].adjTiles[0] = temp[x, y + 1].type;
                    temp[x, y].adjTiles[1] = temp[x - 1, y].type;
                    temp[x, y].adjTiles[2] = temp[x + 1, y].type;
                    temp[x, y].adjTiles[3] = temp[x, y - 1].type;

                    temp[x, y].diagTiles[0] = temp[x - 1, y + 1].type;
                    temp[x, y].diagTiles[1] = temp[x + 1, y + 1].type;
                    temp[x, y].diagTiles[2] = temp[x - 1, y - 1].type;
                    temp[x, y].diagTiles[3] = temp[x + 1, y - 1].type;
                }
                tiles[x - 1, y - 1] = temp[x, y];
            }
        }
    }

    void UpdateGrid()
    {
        rendered = true;
        MeshData meshData = new MeshData();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                meshData = tiles[x, y].TileData(this, x, y, meshData);
            }
        }

        RenderMesh(meshData);
    }

    void RenderMesh(MeshData meshData)
    {
        filter.mesh.Clear();
        filter.mesh.vertices = meshData.vertices.ToArray();
        filter.mesh.triangles = meshData.triangles.ToArray();

        filter.mesh.uv = meshData.uv.ToArray();
        filter.mesh.RecalculateNormals();

        Mesh mesh = new Mesh();
        mesh.vertices = meshData.colVertices.ToArray();
        mesh.triangles = meshData.colTriangles.ToArray();

        var edges = BuildEdgesFromMesh(mesh);
        var paths = BuildColliderPaths(edges);
        ApplyPathsToPolygonCollider(paths);

        //coll.sharedMesh = null;
        
        //mesh.RecalculateNormals();

        //coll.sharedMesh = mesh;
    }

    public override string ToString()
    {
        return pos.x + ";" + pos.y;
    }

    #region Helper

    void ApplyPathsToPolygonCollider(List<Vector2[]> paths)
    {
        if (paths == null)
            return;

        coll.pathCount = paths.Count;
        for (int i = 0; i < paths.Count; i++)
        {
            var path = paths[i];
            coll.SetPath(i, path);
        }
    }

    Dictionary<Edge2D, int> BuildEdgesFromMesh(Mesh collMesh)
    {
        var mesh = filter.sharedMesh;

        if (mesh == null)
            return null;

        var verts = collMesh.vertices;
        var tris = collMesh.triangles;
        var edges = new Dictionary<Edge2D, int>();

        for (int i = 0; i < tris.Length - 2; i += 3)
        {

            var faceVert1 = verts[tris[i]];
            var faceVert2 = verts[tris[i + 1]];
            var faceVert3 = verts[tris[i + 2]];

            Edge2D[] faceEdges;
            faceEdges = new Edge2D[] {
                new Edge2D{ a = faceVert1, b = faceVert2 },
                new Edge2D{ a = faceVert2, b = faceVert3 },
                new Edge2D{ a = faceVert3, b = faceVert1 },
            };

            foreach (var edge in faceEdges)
            {
                if (edges.ContainsKey(edge))
                    edges[edge]++;
                else
                    edges[edge] = 1;
            }
        }

        return edges;
    }

    static List<Edge2D> GetOuterEdges(Dictionary<Edge2D, int> allEdges)
    {
        var outerEdges = new List<Edge2D>();

        foreach (var edge in allEdges.Keys)
        {
            var numSharedFaces = allEdges[edge];
            if (numSharedFaces == 1)
                outerEdges.Add(edge);
        }

        return outerEdges;
    }

    static List<Vector2[]> BuildColliderPaths(Dictionary<Edge2D, int> allEdges)
    {

        if (allEdges == null)
            return null;

        var outerEdges = GetOuterEdges(allEdges);

        var paths = new List<List<Edge2D>>();
        List<Edge2D> path = null;

        while (outerEdges.Count > 0)
        {

            if (path == null)
            {
                path = new List<Edge2D>();
                path.Add(outerEdges[0]);
                paths.Add(path);

                outerEdges.RemoveAt(0);
            }

            bool foundAtLeastOneEdge = false;

            int i = 0;
            while (i < outerEdges.Count)
            {
                var edge = outerEdges[i];
                bool removeEdgeFromOuter = false;

                if (edge.b == path[0].a)
                {
                    path.Insert(0, edge);
                    removeEdgeFromOuter = true;
                }
                else if (edge.a == path[path.Count - 1].b)
                {
                    path.Add(edge);
                    removeEdgeFromOuter = true;
                }

                if (removeEdgeFromOuter)
                {
                    foundAtLeastOneEdge = true;
                    outerEdges.RemoveAt(i);
                }
                else
                    i++;
            }

            //If we didn't find at least one edge, then the remaining outer edges must belong to a different path
            if (!foundAtLeastOneEdge)
                path = null;

        }

        var cleanedPaths = new List<Vector2[]>();

        foreach (var builtPath in paths)
        {
            var coords = new List<Vector2>();

            foreach (var edge in builtPath)
                coords.Add(edge.a);

            cleanedPaths.Add(CoordinatesCleaned(coords));
        }


        return cleanedPaths;
    }

    static bool CoordinatesFormLine(Vector2 a, Vector2 b, Vector2 c)
    {
        //If the area of a triangle created from three points is zero, they must be in a line.
        float area = a.x * (b.y - c.y) +
            b.x * (c.y - a.y) +
                c.x * (a.y - b.y);

        return Mathf.Approximately(area, 0f);

    }

    static Vector2[] CoordinatesCleaned(List<Vector2> coordinates)
    {
        List<Vector2> coordinatesCleaned = new List<Vector2>();
        coordinatesCleaned.Add(coordinates[0]);

        var lastAddedIndex = 0;

        for (int i = 1; i < coordinates.Count; i++)
        {

            var coordinate = coordinates[i];

            Vector2 lastAddedCoordinate = coordinates[lastAddedIndex];
            Vector2 nextCoordinate = (i + 1 >= coordinates.Count) ? coordinates[0] : coordinates[i + 1];

            if (!CoordinatesFormLine(lastAddedCoordinate, coordinate, nextCoordinate))
            {

                coordinatesCleaned.Add(coordinate);
                lastAddedIndex = i;

            }

        }

        return coordinatesCleaned.ToArray();

    }

    #endregion


    #region Nested

    struct Edge2D
    {

        public Vector2 a;
        public Vector2 b;

        public override bool Equals(object obj)
        {
            if (obj is Edge2D)
            {
                var edge = (Edge2D)obj;
                //An edge is equal regardless of which order it's points are in
                return (edge.a == a && edge.b == b) || (edge.b == a && edge.a == b);
            }

            return false;

        }

        public override int GetHashCode()
        {
            return a.GetHashCode() ^ b.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[" + a.x + "," + a.y + "->" + b.x + "," + b.y + "]");
        }

    }
    #endregion
}

