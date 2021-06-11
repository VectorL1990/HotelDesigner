using System;
using UnityEngine;

namespace RealtimeCSG
{
    public enum BrushFlags
    {
        None = 0,
        InfiniteBrush = 1
    }

#if UNITY_EDITOR
    [AddComponentMenu("CSG/Brush")]
    // Question part
    [ExecuteInEditMode]
#endif
    public sealed partial class CSGBrush : CSGNode
    {
#if UNITY_EDITOR
        public CSGOperationType OperationType = CSGOperationType.Additive;

        public BrushFlags Flags = BrushFlags.None;

        public CSGShape Shape;

        public CSGMesh BrushMesh;
#endif

#if UNITY_EDITOR
        [HideInInspector][NonSerialized]Color? OutlineColor;


        [HideInInspector] [NonSerialized] public readonly CSGShape compareShape = new CSGShape();
#endif
    }
}
