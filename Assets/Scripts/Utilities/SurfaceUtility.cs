using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace RealtimeCSG
{
    internal static class SurfaceUtility
    {
        public static Vector2 ConvertModelToTextureSpace(CSGBrush brush, int surfaceIndex, Vector3 worldCoordinate)
        {
            if (brush.NodeID == CommonVariables.InvalidNodeID ||
                brush.Shape == null || surfaceIndex < 0 ||
                surfaceIndex >= brush.Shape.Surfaces.Length ||
                CSGModelManager.External.ConvertModelToTextureSpace == null)
            {
                return CommonVariables.zeroVector2;
            }

            if (brush.Model == null || brush.ModelTransform == null)
            {
                return CommonVariables.zeroVector2;
            }

            var modelTransform = brush.ModelTransform;
            var modelToWorldSpace = modelTransform.localToWorldMatrix;
            Vector2 texCoordinate;
            return CSGModelManager.External.ConvertModelToTextureSpace(brush.NodeID, surfaceIndex, modelToWorldSpace, worldCoordinate, out texCoordinate) ?
                texCoordinate : CommonVariables.zeroVector2;
        }

        public static bool RotateSurfaces(SelectedBrushSurface[] selectedSurfaces, RotationCircle rotationCircle)
        {
            if (selectedSurfaces.Length <= 0)
            {
                return false;
            }

            var prevFlags = new TexGenFlag[selectedSurfaces.Length];
            var brushSurfaces = new Dictionary<CSGBrush, List<int>>();
            for (var i=0; i<selectedSurfaces.Length; i++)
            {
                var brush = selectedSurfaces[i].Brush;
                var surfaceIndex = selectedSurfaces[i].SurfaceIndex;

                List<int> indices;
                if (!brushSurfaces.TryGetValue(brush, out indices))
                {
                    indices = new List<int>();
                    brushSurfaces.Add(brush, indices);
                }
                indices.Add(surfaceIndex);

                var texGenIndex = brush.Shape.Surfaces[surfaceIndex].TexGenIndex;
                prevFlags[i] = brush.Shape.TexGenFlags[texGenIndex];
            }

            bool modified = false;
            foreach (var pair in brushSurfaces)
            {
                var brush = pair.Key;
                var surfaceIndices = pair.Value;

                if (brush.Model == null || brush.ModelTransform == null)
                {
                    continue;
                }

                //var brushPosition		= brush.hierarchyItem.transform.position;
                //var brushPosition		= brush.hierarchyItem.transform.InverseTransformPoint(Vector3.zero);
                var brushLocalNormal = brush.BrushTransform.InverseTransformVector(rotationCircle.RotateSurfaceNormal);
                var shape = brush.Shape;
                for (var s = 0; s < surfaceIndices.Count; s++)
                {
                    var surfaceIndex = surfaceIndices[s];
                    if (Mathf.Abs(Vector3.Dot(brushLocalNormal, shape.Surfaces[surfaceIndex].Plane.Normal)) > CommonVariables.AngleEpsilon)
                    {
                        var texGenIndex = shape.Surfaces[surfaceIndex].TexGenIndex;

                        RotateTextureCoordAroundWorldPoint(brush, surfaceIndex, rotationCircle.RotateCenterPoint,
                                                           rotationCircle.RotateCurrentSnappedAngle);

                        shape.TexGens[texGenIndex].RotationAngle = shape.TexGens[texGenIndex].RotationAngle % 360.0f;
                        modified = true;
                    }
                }
            }
            return modified;
        }

        public static Matrix4x4 GetModelToTextureSpaceMatrix(TexGen texGen, TexGenFlag texGenFlag, CSGSurface surface, Matrix4x4 localFromModel)
        {

        }

        public static bool ConvertModelToTextureCoord(ref TexGen texGen, TexGenFlag texGenFlag, ref CSGSurface surface, Matrix4x4 localFromModel,
                                                      Vector3 localPoint, out Vector2 textureSpacePoint)
        {

        }

        public static bool RotateTextureCoorAroundWorldPoint(CSGBrush brush, int surfaceIndex, Vector3 worldCenter, float angle)
        {

        }
    }
}
