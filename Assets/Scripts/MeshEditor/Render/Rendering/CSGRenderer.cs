using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSG
{
    #region Line

    sealed class LineMeshRenderer
    {
        #region LineMesh
        sealed class LineMesh
        {
            public const int MaxVertexCount = 65000;
            
            public Vector3[] Vertices1 = new Vector3[MaxVertexCount];
            public Vector3[] Vertices2 = new Vector3[MaxVertexCount];
            public Vector4[] Offsets = new Vector4[MaxVertexCount];
            public Color[] Colors = new Color[MaxVertexCount];
            public List<Vector3> NewVertices1 = new List<Vector3>(MaxVertexCount);
            public List<Vector3> NewVertices2 = new List<Vector3>(MaxVertexCount);
            public List<Vector4> NewOffsets = new List<Vector4>(MaxVertexCount);
            public List<Color> NewColors = new List<Color>(MaxVertexCount);
            public int VertexCount = 0;
            int[] Indices = null;

            Mesh Mesh_;

            public void Clear()
            {
                VertexCount = 0;
            }

            public void AddLine(Vector3 A, Vector3 B, float Thickness, float DashSize, Color InColor)
            {
                int N = VertexCount;
                Vertices1[N] = B;
                Vertices2[N] = A;
                Offsets[N] = new Vector4(Thickness, -1, DashSize);
                Colors[N] = InColor;
                N++;

                Vertices1[N] = B;
                Vertices2[N] = A;
                Offsets[N] = new Vector4(Thickness, +1, DashSize);
                Colors[N] = InColor;
                N++;

                Vertices1[N] = A;
                Vertices2[N] = B;
                Offsets[N] = new Vector4(Thickness, -1, DashSize);
                Colors[N] = InColor;
                N++;

                Vertices1[N] = A;
                Vertices2[N] = B;
                Offsets[N] = new Vector4(Thickness, +1, DashSize);
                Colors[N] = InColor;
                N++;

                VertexCount = N;
            }

            public void CommitMesh()
            {
                if (VertexCount == 0)
                {
                    if (Mesh_ != null && Mesh_.VertexCount != 0)
                    {
                        Mesh_.Clear(true);
                    }
                    return;
                }
                if (Mesh_)
                {
                    Mesh_.Clear(true);
                }
                else
                {
                    Mesh_ = new Mesh();
                    Mesh_.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
                    Mesh_.MarkDynamic();
                }

                int ReqSize = VertexCount * 6/4;
                if (Indices == null || Indices.Length() != ReqSize)
                {
                    Indices = new int[ReqSize];
                }

                for (int i=0, j=0; i<VertexCount; i+=4, j+=6)
                {
                    Indices[j+0] = i+0;
                    Indices[j+1] = i+1;
                    Indices[j+2] = i+2;
                    Indices[j+3] = i+0;
                    Indices[j+4] = i+2;
                    Indices[j+5] = i+3;
                }

                NewVertices1.Clear();
                NewVertices2.Clear();
                NewOffsets.Clear();
                NewColors.Clear();
                if (VertexCount == MaxVertexCount)
                {
                    NewVertices1.AddRange(Vertices1);
                    NewVertices2.AddRange(Vertices2);
                    NewOffsets.AddRange(Offsets);
                    NewColors.AddRange(Colors);
                }
                else
                {
                    NewVertices1.AddRange(Vertices1.Take(VertexCount));
                    NewVertices2.AddRange(Vertices2.Take(VertexCount));
                    NewOffsets.AddRange(Offsets.Take(VertexCount));
                    NewColors.AddRange(Colors.Take(VertexCount));
                }

                Mesh_.SetVertices(NewVertices1);
                Mesh_.SetUVs(0, NewVertices2);
                Mesh_.SetUVs(1, NewOffsets);
                Mesh_.SetColors(NewColors);
                Mesh_.SetIndices(Indices, MeshTopology.Triangles, 0, calculateBounds: false);
                Mesh_.RecalculateBounds();
                Mesh_.UploadMeshData(false);
            }

            public void Draw()
            {
                if (VertexCount == 0 || Mesh_ == null)
                {
                    return;
                }
                Graphics.DrawMeshNow(Mesh_, Matrix4x4.identity);
            }

            internal void Destroy()
            {
                if (Mesh_)
                {
                    UnityEngine.Object.DestroyImmediate(Mesh_);
                    Mesh_ = null;
                    Indices = null;
                }
            }
        }
        #endregion
    
        List<LineMesh> LineMeshes = new List<LineMesh>();
        int CurrentLineMesh = 0;

        public LineMeshRenderer()
        {
            LineMeshes.Add(new LineMesh());
        }
        public void Begin()
        {
            if (LineMeshes == null || LineMeshes.Count == 0)
            {
                return;
            }
            CurrentLineMesh = 0;
            for (int i=0; i< LineMeshes.Count; i++)
            {
                LineMeshes[i].Clear();
            }
        }

        public void End()
        {
            if (LineMeshes == null || LineMeshes.Count == 0)
            {
                return;
            }
            var Max = Mathf.Min(CurrentLineMesh, LineMeshes.Count);
            for (int i=0; i<= Max; i++)
            {
                LineMeshes[i].CommitMesh();
            }
        }

        public void Render(Material GenericLineMaterial)
        {
            if (LineMeshes == null || LineMeshes.Count == 0 || !GenericLineMaterial)
            {
                return;
            }
            
            if (GenericLineMaterial.SetPass(0))
            {
                var Max = Mathf.Min(CurrentLineMesh, LineMeshes.Count - 1);
                for (int i=0; i<= Max; i++)
                {
                    LineMeshes[i].Draw();
                }
            }
        }

        public void DrawLines(Matrix4x4 Matrix, Vector3[] InVertices, int[] InIndices, Color InColor, float InThickness, float InDashSize = 0.0f)
        {
            var Corner1 = new Vector4(InThickness, -1, InDashSize);
			var Corner2 = new Vector4(InThickness, +1, InDashSize);
			var Corner3 = new Vector4(InThickness, +1, InDashSize);
			var Corner4 = new Vector4(InThickness, -1, InDashSize);

			var LineMeshIndex = CurrentLineMesh;
			while (LineMeshIndex >= LineMeshes.Count) LineMeshes.Add(new LineMesh());
			if (LineMeshes[LineMeshIndex].VertexCount + (InIndices.Length * 2) <= LineMesh.MaxVertexCount)
			{
				var LineMesh_	= LineMeshes[LineMeshIndex];
				var Vertices1_	= LineMesh_.Vertices1;
				var Vertices2_	= LineMesh_.Vertices2;
				var Offsets_		= LineMesh_.Offsets;
				var Colors_		= LineMesh_.Colors;
				
				var N = LineMesh_.VertexCount;
				for (int i = 0; i < InIndices.Length; i += 2)
				{
					var A = Matrix.MultiplyPoint(InVertices[InIndices[i + 0]]);
					var B = Matrix.MultiplyPoint(InVertices[InIndices[i + 1]]);
					Vertices1_[N] = B; Vertices2_[N] = A; Offsets_[N] = Corner1; Colors_[N] = InColor; N++;
					Vertices1_[N] = B; Vertices2_[N] = A; Offsets_[N] = Corner2; Colors_[N] = InColor; N++;
					Vertices1_[N] = A; Vertices2_[N] = B; Offsets_[N] = Corner3; Colors_[N] = InColor; N++;
					Vertices1_[N] = A; Vertices2_[N] = B; Offsets_[N] = Corner4; Colors_[N] = InColor; N++;
				}
				LineMesh_.VertexCount = N;
			}
            else
			{  
				for (int i = 0; i < InIndices.Length; i += 2)
				{
					var LineMesh_	= LineMeshes[LineMeshIndex];
					var VertexCount_ = LineMesh_.VertexCount;
					if (LineMesh_.VertexCount + 4 >= LineMesh.MaxVertexCount) { LineMeshIndex++; if (LineMeshIndex >= LineMeshes.Count) LineMeshes.Add(new LineMesh()); LineMesh_ = LineMeshes[LineMeshIndex]; VertexCount_ = LineMesh_.vertexCount; }
					var Vertices1_	= LineMesh_.Vertices1;
					var Vertices2_	= LineMesh_.Vertices2;
					var Offsets_		= LineMesh_.Offsets;
					var Colors_		= LineMesh_.Colors;

					var A = Matrix.MultiplyPoint(InVertices[InIndices[i + 0]]);
					var B = Matrix.MultiplyPoint(InVertices[InIndices[i + 1]]);
					Vertices1_[VertexCount_] = B; Vertices2_[VertexCount_] = A; Offsets_[VertexCount_] = Corner1; Colors_[VertexCount_] = InColor; VertexCount_++;
					Vertices1_[VertexCount_] = B; Vertices2_[VertexCount_] = A; Offsets_[VertexCount_] = Corner2; Colors_[VertexCount_] = InColor; VertexCount_++;
					Vertices1_[VertexCount_] = A; Vertices2_[VertexCount_] = B; Offsets_[VertexCount_] = Corner3; Colors_[VertexCount_] = InColor; VertexCount_++;
					Vertices1_[VertexCount_] = A; Vertices2_[VertexCount_] = B; Offsets_[VertexCount_] = Corner4; Colors_[VertexCount_] = InColor; VertexCount_++;
					
					LineMesh_.VertexCount += 4;
				}
				CurrentLineMesh = LineMeshIndex;
			}
        }
    }

    #endregion
    
    #region OutLine
    internal class BrushOutlineManager
    {
        private static readonly Dictionary<int, GeometryWireframe> OutlineCache = new Dictionary<int, GeometryWireframe>();

        public static GeometryWireFrame GetBrushOutLine(int BrushNodeID)
        {
            if (BrushNodeID == CSGNode.InvalidNodeID)
            {
                return null;
            }

            var ExternalOutlineGeneration = CSGModelManager.GetBrushOutlineGeneration(BrushNodeID);
            GeometryWireframe Outline;
			if (!OutlineCache.TryGetValue(BrushNodeID, out Outline))
			{
                ExternalOutlineGeneration = ExternalOutlineGeneration - 1;
            }
			
			if (Outline != null && ExternalOutlineGeneration == Outline.OutLineGeneration)
			{
                return Outline;
            }

			Outline = new GeometryWireframe();
			if (!CSGModelManager.ExternalMethods.GetBrushOutline(BrushNodeID, out Outline))
			{
                return null;
            }
			
			Outline.OutLineGeneration = ExternalOutlineGeneration;
			OutlineCache[BrushNodeID] = Outline;
			return Outline;
        }
    }
    #endregion

    internal static class CSGRenderer
    {
        public static void DrawSelectedBrushes(LineMeshRenderer ZTestLineMeshRenderer, LineMeshRenderer NoZTestLineMeshRenderer, Int32[] BrushNodeIDs, Matrix4x4[] Transformations, Color WireFrameColor, float Thickness = -1)
        {
            Color SelectedOuterColor = WireFrameColor;
            Color SelectedInnerColor = SelectedOuterColor;
            Color SelectedOuterOccludedColor = SelectedOuterColor;
            Color SelectedInnerOccludedColor = SelectedInnerColor;

            SelectedOuterOccludedColor.a *= 0.5f;
            SelectedInnerOccludedColor.a *= 0.5f;

            var WireFrames = BrushOutlineManager.GetBrushOutLine(BrushNodeIDs);
            
        }
    }
}