using System;
using UnityEngine;

namespace CSG.Components
{
    [System.Serializable]
    public enum BrushFlags
    {

    }


    public sealed partial class CSGBrush : CSGNode
    {
        public const float CurrentVersion = 2.f;
        [HideInspector] public float Version = CurrentVersion;
    }
}