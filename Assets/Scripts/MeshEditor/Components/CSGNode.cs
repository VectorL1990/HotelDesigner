using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace CSG
{
    #if UNITY_EDITOR
    [Serializable]
    public enum PrefabSourceAlignment : byte
    {
        AlignedFront,
        AlignedBack,
        AligendLeft,
        AlignedRight,
        AlignedTop,
        AlignedBottom
    }

    public enum PrefabDestinationAlignment : byte
    {
        AlignToSurface,
        AlignSurfaceUp,
        Default
    }

    public enum PrefabInstantiateBehaviour
    {
        Reference,
        Copy
    }
    #endif

    public abstract class CSGNode : MonoBehaviour
    {
        #if UNITY_EDITOR
        [SerializedField] public PrefabInstantiateBehaviour PrefabBehaviour = PrefabInstantiateBehaviour.Reference;
        [SerializedField] public PrefabSourceAlignment PrefabSourceAlignment = PrefabSourceAlignment.AlignedTop;
        [SerializedField] public PrefabDestinationAlignment PrefabDestinationAlignment = PrefabDestinationAlignment.AlignToSurface;
        public const Int32 InvalidNodeID = 0;
        #endif
    }
}