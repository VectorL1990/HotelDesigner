using System;
using UnityEngine;

namespace RealtimeCSG
{
    internal class RotationCircle
    {
        public Vector3 RotateStartVector;
        public Vector3 RotateCenterPoint;
        public Vector3 RotateSurfaceNormal;
        public Vector3 RotateSurfaceTangent;
        public float RotateRadius;
        public float RotateOriginalAngle;
        public float RotateCurrentStartAngle;
        public float RotateCurrentSnappedAngle;
    }
}
