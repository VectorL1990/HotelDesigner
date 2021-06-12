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

        [HideInInspector] [NonSerialized] public Color? OutlineColor;

        // Original ChildNodeData info
        #region Original ChildNodeData
        [HideInInspector] [NonSerialized] public CSGModel Model;
        [HideInInspector] [NonSerialized] public CSGOperation Operation;
        public Transform ModelTransform = null;

        // Original ParentNodeData info
        public bool ChildrenModified = true;

        //  Original HierarchyItem info
        public bool TransformInitialized = false;
        public Transform BrushTransform;
        public CSGBrush Parent;
        public CSGBrush[] ChildNodes = new CSGBrush[0];
        public int SiblingIndex = -1;
        public int PrevSiblingIndex = -1;

        public int LastLoopCount = -1;
        public int CachedTransformSiblingIndex;
        public static int CurrentLoopCount { get; set; }
        #endregion

        [HideInInspector] [NonSerialized] public readonly CSGShape compareShape = new CSGShape();

    }
}
