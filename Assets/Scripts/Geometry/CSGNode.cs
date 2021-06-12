using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RealtimeCSG
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
        [SerializeField] public PrefabInstantiateBehaviour PrefabBehaviour = PrefabInstantiateBehaviour.Reference;
        [SerializeField] public PrefabSourceAlignment PrefabSourceAlignment = PrefabSourceAlignment.AlignedTop;
        [SerializeField] public PrefabDestinationAlignment PrefabDestinationAlignment = PrefabDestinationAlignment.AlignToSurface;
        public Int32 NodeID = 0;
#endif
    }
}
