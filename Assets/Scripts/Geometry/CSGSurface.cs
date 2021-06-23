using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace RealtimeCSG
{
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CSGSurface
    {
        public CSGPlane Plane;

        // Tangent vector for the surface
        public Vector3 Tangent;

        public Vector3 BiNormal;

        public Int32 TexGenIndex;

        public Matrix4x4 GenerateLocalBrushSpaceToPlaneSpaceMatrix()
        {
            var normal = Plane.Normal;
            var tangent = Tangent;
            var biNormal = BiNormal;
            var pointOnPlane = Plane.PointOnPlane;

            // Previous 3x3 matrix represents rotation, it's calculated by
            // R = e * e'
            // e represents local brush space
            // e' represents plane space(z axis is up)
            // Last row of the matrix is translation, which represents local brush space translating to plane space
            return new Matrix4x4()
            {
                m00 = tangent.x,
                m01 = tangent.y,
                m02 = tangent.z,
                m03 = Vector3.Dot(tangent, pointOnPlane),

                m10 = biNormal.x,
                m11 = biNormal.y,
                m12 = biNormal.z,
                m13 = Vector3.Dot(biNormal, pointOnPlane),

                m20 = normal.x,
                m21 = normal.y,
                m22 = normal.z,
                m23 = Vector3.Dot(normal, pointOnPlane),

                m30 = 0.0f,
                m31 = 0.0f,
                m32 = 0.0f,
                m33 = 1.0f
            };
        }

        public override string ToString() 
        { 
            return string.Format("Plane: {0} Tangent: {1} BiNormal: {2} TexGenIndex: {3}", Plane, Tangent, BiNormal, TexGenIndex); 
        }
    }
}
