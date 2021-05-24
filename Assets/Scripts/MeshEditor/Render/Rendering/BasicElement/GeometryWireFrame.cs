using System;
using UnityEngine;

namespace CSG
{
#if UNITY_EDITOR
    [Serilizable]
    public sealed class GeometryWireFrame
    {
        public Vector3[] Vertices = null;
        public Int32[] VisibleOuterLines = null;
        public Int32[] VisibleInnerLines = null;
        public Int32[] VisibleTriangles = null;
        public Int32[] InvisibleOuterLines = null;
        public Int32[] InvisibleInnerLines = null;
        public Int32[] InvalidLines = null;
        public UInt64 OutLineGeneration = 0;
    }
#endif
}