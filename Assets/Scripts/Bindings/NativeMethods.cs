#if UNITY_EIDTOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSG
{
    #region Outlines
    internal delegate UInt64 GetBrushOutlineGenerationDelegate(Int32 BrushNodeID);
    #endregion

    internal sealed class NativeMethods
    {
        public GetBrushOutlineGenerationDelegate GetBrushOutlienGeneration;
        public GetBrushOutlineDelegate				GetBrushOutline;
    }
}

#endif