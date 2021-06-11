using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace RealtimeCSG
{
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct TexGen
    {
        public Vector2 Translation;

        public Vector2 Scale;

        public float RotationAngle;

        public Material RenderMaterial;

        public PhysicMaterial PhysicsMaterial;

        public UInt32 SmoothingGroup;

        public TexGen(Material renderMaterial = null, PhysicMaterial physicsMaterial = null)
        {
            Translation = Vector3.zero;
            Scale = Vector3.one;
            RotationAngle = 0.0f;
            RenderMaterial = renderMaterial;
            PhysicsMaterial = physicsMaterial;
            SmoothingGroup = 0;
        }

        public Matrix4x4 GeneratePlaneSpaceToTextureSpaceMatrix()
        {
            var sx = Scale.x;
            var sy = Scale.y;
            var r = Mathf.Deg2Rad * -RotationAngle;
            var rs = Mathf.Sin(r);
            var rc = Mathf.Cos(r);
            var tx = Translation.x;
            var ty = Translation.y;

            //*
            var scaleMatrix = new Matrix4x4()
            {
                m00 = -sx,
                m10 = 0.0f,
                m20 = 0.0f,
                m30 = 0.0f,
                m01 = 0.0f,
                m11 = sy,
                m21 = 0.0f,
                m31 = 0.0f,
                m02 = 0.0f,
                m12 = 0.0f,
                m22 = 1.0f,
                m32 = 0.0f,
                m03 = 0.0f,
                m13 = 0.0f,
                m23 = 0.0f,
                m33 = 1.0f
            };

            var translationMatrix = new Matrix4x4()
            {
                m00 = 1.0f,
                m10 = 0.0f,
                m20 = 0.0f,
                m30 = 0.0f,
                m01 = 0.0f,
                m11 = 1.0f,
                m21 = 0.0f,
                m31 = 0.0f,
                m02 = 0.0f,
                m12 = 0.0f,
                m22 = 1.0f,
                m32 = 0.0f,
                m03 = tx,
                m13 = ty,
                m23 = 0.0f,
                m33 = 1.0f
            };

            var rotationMatrix = new Matrix4x4()
            {
                m00 = rc,
                m10 = -rs,
                m20 = 0.0f,
                m30 = 0.0f,
                m01 = rs,
                m11 = rc,
                m21 = 0.0f,
                m31 = 0.0f,
                m02 = 0.0f,
                m12 = 0.0f,
                m22 = 1.0f,
                m32 = 0.0f,
                m03 = 0.0f,
                m13 = 0.0f,
                m23 = 0.0f,
                m33 = 1.0f
            };
            return (translationMatrix
                    * scaleMatrix)
                    * rotationMatrix;
        }
    }
}
