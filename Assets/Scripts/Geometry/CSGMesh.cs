using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RealtimeCSG
{
    [Serializable]
    public struct HalfEdge
    {
        public HalfEdge(int twinIndex, short polygonIndex, short vertexIndex, bool hardEdge = true)
        {
            TwinIndex = twinIndex;
            PolygonIndex = polygonIndex;
            HardEdge = hardEdge;
            VertexIndex = vertexIndex;
        }

        public int TwinIndex;
        public short PolygonIndex;
        public bool HardEdge;
        public short VertexIndex;
    }

    [Serializable]
    public sealed class Polygon
    {
        public Polygon(int[] edges, int texGenIndex)
        {
            EdgeIndices = edges;
            TexGenIndex = texGenIndex;
        }

        // Indices to all HalfEdges in the mesh that this polygon uses
        public int[] EdgeIndices;
        public int TexGenIndex;
    }

    [Serializable]
    public sealed class CSGMesh
    {
        public Vector3[] Vertices;
        public HalfEdge[] Edges;
        public Polygon[] Polygons;

        public CSGMesh()
        {

        }

        public CSGMesh(CSGMesh other)
        {
            CopyFrom(other);
        }

        public void Reset()
        {
            Vertices = null;
            Edges = null;
            Polygons = null;
        }

        public void CopyFrom(CSGMesh other)
        {
            if (other == null)
            {
                Reset();
                return;
            }
            if (other.Vertices != null)
            {
                if (Vertices == null || Vertices.Length != other.Vertices.Length)
                {
                    Vertices = new Vector3[other.Vertices.Length];
                }
                Array.Copy(other.Vertices, Vertices, other.Vertices.Length);
            }
            else
            {
                Vertices = null;
            }

            if (other.Edges != null)
            {
                if (Edges == null || Edges.Length != other.Edges.Length)
                {
                    Edges = new HalfEdge[other.Edges.Length];
                }
                Array.Copy(other.Edges, Edges, other.Edges.Length);
            }
            else
            {
                Edges = null;
            }

            if (other.Polygons != null)
            {
                if (Polygons == null || Polygons.Length != other.Polygons.Length)
                {
                    Polygons = new Polygon[other.Polygons.Length];
                }
                for (var i = 0; i < other.Polygons.Length; i++)
                {
                    if (other.Polygons[i].EdgeIndices == null || other.Polygons[i].EdgeIndices.Length == 0)
                    {
                        continue;
                    }
                    var newEdges = new int[other.Polygons[i].EdgeIndices.Length];
                    Array.Copy(other.Polygons[i].EdgeIndices, newEdges, other.Polygons[i].EdgeIndices.Length);
                    Polygons[i] = new Polygon(newEdges, other.Polygons[i].TexGenIndex);
                }
            }
            else
            {
                Polygons = null;
            }
        }


        public CSGMesh Clone()
        {
            return new CSGMesh(this);
        }

        public Vector3 GetVertex(int halfEdgeIndex)
        {
            if (halfEdgeIndex < 0 || halfEdgeIndex >= Edges.Length)
            {
                return Vector3.zero;
            }
            var vertexIndex = Edges[halfEdgeIndex].VertexIndex;
            if (vertexIndex < 0 || vertexIndex >= Vertices.Length)
            {
                return Vector3.zero;
            }
            return Vertices[vertexIndex];
        }

        public Vector3[] GetVertices(int[] halfEdgeIndices)
        {
            var vertices = new Vector3[halfEdgeIndices.Length];
            for (var i=0; i<halfEdgeIndices.Length; i++)
            {
                if (halfEdgeIndices[i] < 0 || halfEdgeIndices[i] >= Edges.Length)
                {
                    vertices[i] = Vector3.zero;
                    continue;
                }
                var vertexIndex = Edges[halfEdgeIndices[i]].VertexIndex;
                if (vertexIndex < 0 || vertexIndex >= Vertices.Length)
                {
                    vertices[i] = Vector3.zero;
                    continue;
                }
                vertices[i] = Vertices[vertexIndex];
            }
            return vertices;
        }

        public Vector3 GetVertex(ref HalfEdge halfEdge)
        {
            return Vertices[halfEdge.VertexIndex];
        }

        public short GetVertexIndex(int halfEdgeIndex)
        {
            return Edges[halfEdgeIndex].VertexIndex;
        }

        public short GetVertexIndex(ref HalfEdge halfEdge)
        {
            return halfEdge.VertexIndex;
        }

        public Vector3 GetTwinEdgeVertex(ref HalfEdge halfEdge)
        {
            return Vertices[Edges[halfEdge.TwinIndex].VertexIndex];
        }

        public Vector3 GetTwinEdgeVertex(int halfEdgeIndex)
        {
            return Vertices[Edges[Edges[halfEdgeIndex].TwinIndex].VertexIndex];
        }

        public short GetTwinEdgeVertexIndex(ref HalfEdge halfEdge)
        {
            return Edges[halfEdge.TwinIndex].VertexIndex;
        }

        public short GetTwinEdgeVertexIndex(int halfEdgeIndex) 
        {
            return Edges[Edges[halfEdgeIndex].TwinIndex].VertexIndex; 
        }

        public int GetTwinEdgeIndex(ref HalfEdge halfEdge) 
        { 
            return halfEdge.TwinIndex; 
        }

        public int GetTwinEdgeIndex(int halfEdgeIndex) 
        { 
            return Edges[halfEdgeIndex].TwinIndex; 
        }

        public short GetTwinEdgePolygonIndex(int halfEdgeIndex) 
        {
            return Edges[Edges[halfEdgeIndex].TwinIndex].PolygonIndex; 
        }

        public short GetEdgePolygonIndex(int halfEdgeIndex) 
        { 
            return Edges[halfEdgeIndex].PolygonIndex;
        }

        public int GetNextEdgeIndexAroundVertex(int halfEdgeIndex) 
        { 
            return GetTwinEdgeIndex(GetNextEdgeIndex(halfEdgeIndex));
        }


        public int GetPrevEdgeIndex(int halfEdgeIndex)
        {
            var edge = Edges[halfEdgeIndex];
            var polygonIndex = edge.PolygonIndex;
            if (polygonIndex < 0 || polygonIndex >= Polygons.Length)
            { 
                return -1;
            }
            var edgeIndices = Polygons[polygonIndex].EdgeIndices;
            for (int i = 1; i < edgeIndices.Length; i++)
            {
                if (edgeIndices[i] == halfEdgeIndex)
                {
                    return edgeIndices[i - 1];
                }
            }
            return edgeIndices[edgeIndices.Length - 1];
        }

        public int GetNextEdgeIndex(int halfEdgeIndex)
        {
            var edge = Edges[halfEdgeIndex];
            var polygonIndex = edge.PolygonIndex;
            if (polygonIndex < 0 || polygonIndex >= Polygons.Length)
            {
                return -1;
            }

            var edgeIndices = Polygons[polygonIndex].EdgeIndices;
            for (int i = 0; i < edgeIndices.Length - 1; i++)
            {
                if (edgeIndices[i] == halfEdgeIndex)
                {
                    return edgeIndices[i + 1];
                }
            }
            return edgeIndices[0];
        }
    }
}
