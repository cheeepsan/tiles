using System;
using NetTopologySuite.Geometries;
using UnityEngine;

namespace Common
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
}