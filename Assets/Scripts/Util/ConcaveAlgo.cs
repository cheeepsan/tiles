using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Algorithm.Hull;
using NetTopologySuite.Geometries;
using NetTopologySuite.Triangulate;
using UnityEngine;


//https://www.codeproject.com/Articles/775753/A-Convex-Hull-Algorithm-and-its-implementation-in
namespace Util
{
    public struct MeshData
    {
        public Vector3[] triangleVertices;
        public Vector2[] uuvs;
        public Vector3[] normals;
        public Vector4[] tangents;
        public int[] triagnles;
        public Point centroid;
        public String name;
    }

    /*
     * 1. Divide points into blocks
     * 2. Find outer points for blocks
     * 3. Get center
     * 4. Generate triangles
     * 5. Generate mesh data
     */
    public class ConcaveAlgo
    {
        private List<Transform> _points;

        public ConcaveAlgo(List<Transform> points)
        {
            _points = points;
        }

        public List<MeshData> CalculateMeshes()
        {
            Dictionary<String, List<Transform>> transformGroups = GroupAdjacentPoints();
            List<MeshData> meshData = GenerateMeshes(transformGroups);

            return meshData;
        }
        
        private List<MeshData> GenerateMeshes(Dictionary<String, List<Transform>> transformGroups)
        {
            List<MeshData> meshes = new List<MeshData>();
            
            foreach (var group in transformGroups)
            {
                var calculatedPolygon = CalculatePolygon(group.Value);
                // move mesh to (0,0)
                Point offsetDirection = GetOffsetDirection(calculatedPolygon.Centroid);
                Point center = calculatedPolygon.Centroid;

                MeshData mesh = GetTriangleMeshes(calculatedPolygon, center, offsetDirection, group.Key);
                meshes.Add(mesh);
            }

            return meshes;
        }
        
        private Dictionary<String, List<Transform>> GroupAdjacentPoints()
        {
            Dictionary<Transform, List<Transform>> dict = new Dictionary<Transform, List<Transform>>();

            foreach (var point in _points)
            {
                FindAdjacent(dict, point, _points);
            }

            int i = 0;
            Dictionary<String, Dictionary<Transform, Transform>> groups =
                new Dictionary<String, Dictionary<Transform, Transform>>();

            while (dict.Count > 0) //??
            {
                var ret = IterateOverGraph(dict);
                groups.Add(i.ToString(), ret);
                i++;
            }

            var listOfGroups = new Dictionary<String, List<Transform>>();
            foreach (var group in groups)
            {
                var transformDictionary = group.Value;

                var flatGroup = new List<Transform>();

                foreach (var pair in transformDictionary)
                {
                    flatGroup.Add(pair.Key);
                    flatGroup.Add(pair.Value);
                }

                var distinctFlatGroup = flatGroup.Distinct().ToList();
                listOfGroups.Add(group.Key, distinctFlatGroup);
            }

            return listOfGroups;
        }

        private void FindAdjacent(Dictionary<Transform, List<Transform>> dict, Transform tile, List<Transform> points)
        {
            List<Transform> nearby = new List<Transform>();

            float x = tile.position.x;
            float z = tile.position.z;
            Vector2 pos = new Vector2(x, z);
            foreach (var point in points)
            {
                float currX = point.position.x;
                float currZ = point.position.z;

                Vector2 currPos = new Vector2(currX, currZ);
                if (Vector2.Distance(pos, currPos) < 1.5f) // pythagoras of scale x by z
                {
                    nearby.Add(point);
                }
            }

            dict.Add(tile, nearby);
        }

        private Geometry CalculatePolygon(List<Transform> polygonPoints)
        {
            var totalVertex = new Coordinate[polygonPoints.Count + 1];

            for (int i = 0; i < polygonPoints.Count; i++)
            {
                Vector3 currTransform = polygonPoints[i].position;
                totalVertex[i] = new Coordinate(currTransform.x, currTransform.z);
            }

            // closing the polygon
            var first = polygonPoints.First().position;
            totalVertex[polygonPoints.Count] = new Coordinate(first.x, first.z);

            Polygon polygon = new Polygon(new LinearRing(totalVertex));
            var concaveHull = new ConcaveHull(polygon)
            {
                HolesAllowed = false,
                MaximumEdgeLength = 1000
            };

            var calculatedPolygon = concaveHull.GetHull();

            return calculatedPolygon;
        }

        private MeshData GetTriangleMeshes(Geometry calculatedPolygon, Point center, Point offsetDirection, String polygonName)
        {
            // bufer to enlarge polygon by (1) unit
            Coordinate[] coords = calculatedPolygon.Buffer(1).Coordinates;
            
            DelaunayTriangulationBuilder triangulationBuilder = new DelaunayTriangulationBuilder();
            triangulationBuilder.SetSites(coords);
            GeometryCollection triangles = triangulationBuilder.GetTriangles(GeometryFactory.Default);

            List<Vector2> uuvs = new List<Vector2>();
            List<Vector3> triangleVertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector4> tangents = new List<Vector4>();
            Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

            // stolen from LowPolyWater, no other reason for these values
            float uvFactorX = 1.0f / calculatedPolygon.NumPoints;
            float uvFactorY = 1.0f / calculatedPolygon.NumPoints;

            foreach (Geometry triangle in triangles)
            {
                // left side, hence Reverse
                foreach (var c in triangle.Coordinates.SkipLast(1).Reverse())
                {
                    var offsetX = (float)(center.X);
                    var offsetZ = (float)(center.Y);

                    float coordX = (float)c.X;
                    float coordZ = (float)c.Y;


                    triangleVertices.Add(new Vector3(coordX - offsetX, 0, coordZ - offsetZ));
                    normals.Add(Vector3.back);
                    tangents.Add(tangent);
                    uuvs.Add(new Vector2((float)c.X * uvFactorX, (float)c.Y * uvFactorY));
                }
            }

            Vector3[] tV = triangleVertices.ToArray();
            int size = (tV.Length / 3);
            int[] amoutOfTriangles = Enumerable.Range(0, size * 3).ToArray();
            return new MeshData()
            {
                triangleVertices = tV,
                uuvs = uuvs.ToArray(),
                normals = normals.ToArray(),
                tangents = tangents.ToArray(),
                triagnles = amoutOfTriangles,
                centroid = center,
                name = polygonName
            };
        }

        private Point GetOffsetDirection(Point center)
        {
            float x = (float)center.X;
            float y = (float)center.Y;

            float offsetX;
            float offsetY;

            if (x > 0)
            {
                offsetX = 1;
            }
            else
            {
                offsetX =  -1;
            }

            if (y > 0)
            {
                offsetY = 1;
            }
            else
            {
                offsetY =  -1;
            }

            return new Point(offsetX, offsetY);
        }
        
        //https://stackoverflow.com/questions/615202/c-sharp-graph-traversal
        private Dictionary<Transform, Transform> IterateOverGraph(Dictionary<Transform, List<Transform>> data)
        {
            Dictionary<Transform, bool> visited = new Dictionary<Transform, bool>();
            Dictionary<Transform, Transform> graph = new Dictionary<Transform, Transform>();

            Queue<Transform> worklist = new Queue<Transform>();

            Transform first = data.First().Key;

            visited.Add(first, false);
            worklist.Enqueue(first);
            while (worklist.Count != 0)
            {
                Transform t = worklist.Dequeue();
                List<Transform> neighbours = data[t];
                foreach (var neighbour in neighbours)
                {
                    if (!visited.ContainsKey(neighbour))
                    {
                        visited.Add(neighbour, false);
                        graph.Add(neighbour, t);
                        worklist.Enqueue(neighbour);
                    }
                }

                data.Remove(t);
            }

            return graph;
        }
        
                /*
        public void GroupAdjacentPoints()
        {
            Dictionary<Transform, List<Transform>> dict = new Dictionary<Transform, List<Transform>>();

            foreach (var point in _points)
            {
                FindAdjacent(dict, point, _points);
            }

            int i = 0;
            Dictionary<String, Dictionary<Transform, Transform>> groups =
                new Dictionary<String, Dictionary<Transform, Transform>>();

            while (dict.Count > 0) //??
            {
                var ret = IterateOverGraph(dict);
                groups.Add(i.ToString(), ret);
                i++;
            }

            var listOfGroups = new Dictionary<String, List<Transform>>();
            foreach (var group in groups)
            {
                var transformDictionary = group.Value;

                var flatGroup = new List<Transform>();

                foreach (var pair in transformDictionary)
                {
                    flatGroup.Add(pair.Key);
                    flatGroup.Add(pair.Value);
                }

                var distinctFlatGroup = flatGroup.Distinct().ToList();
                listOfGroups.Add(group.Key, distinctFlatGroup);
            }


            foreach (var v in listOfGroups)
            {
                // init coords for convex
                List<Transform> p = v.Value;
                var totalVertex = new Coordinate[p.Count + 1];

                for (i = 0; i < p.Count; i++)
                {
                    Vector3 currTransform = p[i].position;
                    totalVertex[i] = new Coordinate(currTransform.x, currTransform.z);
                }

                // closing the polygon
                var first = p.First().position;
                totalVertex[p.Count] = new Coordinate(first.x, first.z);

                Polygon polygon = new Polygon(new LinearRing(totalVertex));
                var l = new ConcaveHull(polygon)
                {
                    HolesAllowed = false,
                    MaximumEdgeLength = 1000
                };
                // triangles
                DelaunayTriangulationBuilder triangulationBuilder = new DelaunayTriangulationBuilder();

                var calculatedPolygon = l.GetHull();
                var coords = calculatedPolygon.Coordinates;
                // move mesh to (0,0)
                Point offset = calculatedPolygon.Centroid;

                triangulationBuilder.SetSites(coords);
                GeometryCollection triangles = triangulationBuilder.GetTriangles(GeometryFactory.Default);

                List<Vector2> uuvs = new List<Vector2>();
                List<Vector3> triangleVertices = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<Vector4> tangents = new List<Vector4>();
                Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

                float uvFactorX = 1.0f / calculatedPolygon.NumPoints;
                float uvFactorY = 1.0f / calculatedPolygon.NumPoints;
                foreach (Geometry triangle in triangles)
                {
                    foreach (var c in triangle.Coordinates.SkipLast(1).Reverse())
                    {
                        var offsetX = (float)offset.X;
                        var offsetZ = (float)offset.Y;
                        triangleVertices.Add(new Vector3((float)c.X - offsetX, 0, (float)c.Y - offsetZ));
                        normals.Add(Vector3.back);
                        tangents.Add(tangent);
                        uuvs.Add(new Vector2((float)c.X * uvFactorX, (float)c.Y * uvFactorY));
                    }
                }

                Mesh m = new Mesh
                {
                    name = "test mesh " + v.Key
                };

                GameObject gb = (GameObject)Resources.Load("Meshes/TideMesh");

                var inst = GameObject.Instantiate(gb);
                inst.name = v.Key;


                Vector3[] tV = triangleVertices.ToArray();

                var size = (tV.Length / 3);


                m.SetVertices(tV);
                m.triangles = Enumerable.Range(0, size * 3).ToArray();
                m.normals = normals.ToArray();
                m.tangents = tangents.ToArray();
                m.uv = uuvs.ToArray();
                var center = calculatedPolygon.Centroid;
                inst.transform.position = new Vector3((float)center.X, 0.66f, (float)center.Y);
                m.RecalculateNormals();

                inst.GetComponent<MeshFilter>().sharedMesh = m;
                m.RecalculateBounds();
            }
        }
        */
    }
}