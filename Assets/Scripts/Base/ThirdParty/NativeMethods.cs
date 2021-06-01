#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;

namespace RealtimeCSG
{
	[Serializable, StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct CSGSurfaceIntersection
    {
		public Plane LocalPlane;
		public Plane ModelPlane;
		public Plane WorldPlane;

		public Vector3 WorldIntersection;
		public Vector2 SurfaceIntersection;

		public float Distance;

		public readonly static CSGSurfaceIntersection None = new CSGSurfaceIntersection()
		{
			LocalPlane = new Plane(Vector3.zero, 0),
			ModelPlane = new Plane(Vector3.zero, 0),
			WorldPlane = new Plane(Vector3.zero, 0),
			WorldIntersection = Vector3.zero,
			SurfaceIntersection = Vector2.zero,
			Distance = float.PositiveInfinity
		};
    };

	// CSG Temp struct
	public struct CSGTreeBrushIntersection
    {
		
    }

	#region delegates

	#region Tree delegates
	internal delegate bool GenerateModelDelegate			(Int32				userID,
															 out Int32			generatedModelNodeID);

	internal delegate bool SetModelDelegate					(Int32				modelNodeID,
															 bool				isEnabled);

	internal delegate bool SetDirtyDelegate					(Int32				nodeID);
	
	internal delegate bool SetModelEnabledDelegate			(Int32				modelNodeID,
															 bool				isEnabled);
#endregion

#region Selection delegates
	internal delegate bool RayCastMultiDelegate				(int					modelCount,
															 CSGModel[]				models,
															 Vector3				rayStart,
															 Vector3				rayEnd,
															 bool					ignoreInvisiblePolygons,
															 out LegacyBrushIntersection[] intersections,
															 CSGBrush[]				ignoreBrushes = null);

	internal delegate bool RayCastIntoModelMultiDelegate	(CSGModel				model, 
															 Vector3				rayStart,
															 Vector3				rayEnd,
															 bool					ignoreInvisiblePolygons,
															 out LegacyBrushIntersection[] intersections,
															 CSGBrush[]				ignoreBrushes = null);

	internal delegate bool RayCastIntoModelDelegate			(CSGModel				model, 
															 Vector3				rayStart,
															 Vector3				rayEnd,
															 bool					ignoreInvisiblePolygons,
															 out LegacyBrushIntersection	intersection,
															 CSGBrush[]				ignoreBrushes = null);
		
	internal delegate bool RayCastIntoBrushDelegate			(Int32					brushNodeID, 
															 Vector3				rayStart,
															 Vector3				rayEnd,
															 Matrix4x4				modelTransformation,
															 out LegacyBrushIntersection	intersection,
															 bool					ignoreInvisiblePolygons);

	internal delegate bool RayCastIntoBrushSurfaceDelegate	(Int32							brushNodeID, 
															 Int32							surfaceIndex,
															 Vector3						rayStart,
															 Vector3						rayEnd,
															 Matrix4x4						modelTransformation,
															 out LegacySurfaceIntersection	intersection);

	internal delegate bool GetItemsInFrustumDelegate		(CSGModel				model, 
															 Plane[]				planes, 
															 HashSet<GameObject>	gameObjects);
#endregion

	
	internal delegate bool SetChildNodesDelegate			(Int32				nodeID,
															 Int32				childCount,
															 Int32[]			childrenNodeIDs);
	internal delegate bool DestroyNodeDelegate				(Int32				operationNodeID);

	internal delegate bool DestroyNodesDelegate				(Int32[]			operationNodeIDs);

#region Operation delegates
	internal delegate bool GenerateOperationDelegate		(Int32				userID,
															 out Int32			generatedOperationNodeID);

	internal delegate bool SetOperationOperationTypeDelegate(Int32				operationNodeID,
															 CSGOperationType	operation);

#endregion

#region Brush delegates	
	internal delegate bool GenerateBrushDelegate			(Int32				userID,
															 out Int32			generatedBrushNodeID);

	internal delegate Int32 GetBrushMeshIDDelegate			(Int32				brushNodeID);
	internal delegate bool SetBrushMeshIDDelegate			(Int32				brushNodeID, Int32 brushMeshIndex);
	
	internal delegate CSGTreeBrushFlags GetBrushFlagsDelegate(Int32				brushNodeID);
	internal delegate bool SetBrushFlagsDelegate			(Int32				brushNodeID, CSGTreeBrushFlags flags);

	internal delegate bool SetBrushOperationTypeDelegate	(Int32				brushNodeID,
															 Foundation.CSGOperationType operation);

	internal delegate bool SetBrushToModelSpaceDelegate		(Int32				brushNodeID,
															 Matrix4x4			localToModelSpace);

	internal delegate Int32 CreateBrushMeshDelegate			(Int32				userID, Foundation.BrushMesh brushMesh);
	
	internal delegate bool UpdateBrushMeshDelegate			(Int32				brushMeshIndex,
															 Foundation.BrushMesh brushMesh);

	internal delegate bool DestroyBrushMeshDelegate			(Int32				brushMeshIndex);
#endregion

#region Misc
	internal delegate List<List<Vector2>> ConvexPartitionDelegate (Vector2[] points);

#endregion

#region TexGen manipulation delegates	

	internal delegate bool GetSurfaceMinMaxTexCoordsDelegate(Int32				brushNodeID,
															 Int32				surfaceIndex,
															 Matrix4x4			modelLocalToWorldMatrix,
															 out Vector2		minTextureCoordinate, 
															 out Vector2		maxTextureCoordinate);

	internal delegate bool GetSurfaceMinMaxWorldCoordDelegate(Int32				brushNodeID,
															 Int32				surfaceIndex, 
															 out Vector3		minWorldCoordinate, 
															 out Vector3		maxWorldCoordinate);

	internal delegate bool ConvertWorldToTextureCoordDelegate(Int32				brushNodeID,
															  Int32				surfaceIndex, 
															  Matrix4x4			modelTransformation,
															  Vector3			worldCoordinate, 
															  out Vector2		textureCoordinate);

	internal delegate bool ConvertTextureToWorldCoordDelegate(Int32				brushNodeID,
															  Int32				surfaceIndex, 
															  float				textureCoordinateU, 
															  float				textureCoordinateV,
															  // workaround for mac-osx related bug
															  ref Matrix4x4		modelTransformation,
															  ref float			worldCoordinateX, 
															  ref float			worldCoordinateY, 
															  ref float			worldCoordinateZ);


#endregion

#region Outlines
	internal delegate UInt64 GetBrushOutlineGenerationDelegate(Int32 brushNodeID);
	internal delegate bool GetBrushOutlineDelegate			(Int32					brushNodeID,
															 out GeometryWireframe	geometryWireframe);
	internal delegate bool GetSurfaceOutlineDelegate		(Int32					brushNodeID,
															 Int32					surfaceIndex,
															 out GeometryWireframe	outline);
#endregion

#region Meshes

	internal delegate void ResetCSGDelegate();

	internal delegate bool UpdateAllModelMeshesDelegate		();
	internal delegate bool ModelMeshesNeedUpdateDelegate	();
	internal delegate bool GetMeshDescriptionsDelegate		(CSGModel		model,
															 ref GeneratedMeshDescription[]	meshDescriptions);

	internal delegate GeneratedMeshContents GetModelMeshDelegate(int modelNodeID, GeneratedMeshDescription meshDescription);
	internal delegate bool GetModelMeshNoAllocDelegate(int modelNodeID, GeneratedMeshDescription meshDescription, ref GeneratedMeshContents generatedMeshData);

    #endregion

    #endregion

    internal sealed class NativeMethods
    {
        public ResetCSGDelegate ResetCSG;
        public ConvexPartitionDelegate ConvexPartition;

        public SetDirtyDelegate SetDirty;
        public SetChildNodesDelegate SetChildNodes;
        public DestroyNodeDelegate DestroyNode;
        public DestroyNodesDelegate DestroyNodes;

        public GenerateModelDelegate GenerateModel;
        public SetModelEnabledDelegate SetModelEnabled;

        public GenerateOperationDelegate GenerateOperation;
        public SetOperationOperationTypeDelegate SetOperationOperationType;

        public GenerateBrushDelegate GenerateBrush;
        public GetBrushMeshIDDelegate GetBrushMeshID;
        public SetBrushMeshIDDelegate SetBrushMeshID;
        public GetBrushFlagsDelegate GetBrushFlags;
        public SetBrushFlagsDelegate SetBrushFlags;
        public SetBrushOperationTypeDelegate SetBrushOperationType;
        public SetBrushToModelSpaceDelegate SetBrushToModelSpace;


        public CreateBrushMeshDelegate CreateBrushMesh;
        public UpdateBrushMeshDelegate UpdateBrushMesh;
        public DestroyBrushMeshDelegate DestroyBrushMesh;


        public UpdateAllModelMeshesDelegate UpdateAllModelMeshes;
        public GetMeshDescriptionsDelegate GetMeshDescriptions;

        public GetModelMeshDelegate GetModelMesh;
        public GetModelMeshNoAllocDelegate GetModelMeshNoAlloc;


        public RayCastMultiDelegate RayCastMulti;
        public RayCastIntoModelMultiDelegate RayCastIntoModelMulti;
        public RayCastIntoBrushDelegate RayCastIntoBrush;
        public RayCastIntoBrushSurfaceDelegate RayCastIntoBrushSurface;
        public GetItemsInFrustumDelegate GetItemsInFrustum;

        public GetSurfaceMinMaxTexCoordsDelegate GetSurfaceMinMaxTexCoords;
        public ConvertWorldToTextureCoordDelegate ConvertModelToTextureSpace;
        public ConvertTextureToWorldCoordDelegate ConvertTextureToModelSpace;


        public GetBrushOutlineGenerationDelegate GetBrushOutlineGeneration;
        public GetBrushOutlineDelegate GetBrushOutline;
        public GetSurfaceOutlineDelegate GetSurfaceOutline;
    }

	internal static class NativeMethodBindings
    {
		// CSG Temp var
		const string NativePluginName = "";

        #region C# methods to be called from C++

		// CSG Temp function
		public static void RegisterUnityMethods()
        {

        }

		// CSG temp function
		public static void ClearUnityMethods()
        {

        }

		#endregion


		#region C++ Registration/Update functions

		#region Diagnostics
		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void LogDiagnostics();

		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void RebuildAll();
		#endregion

		#region Scene event functions
		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern void ClearAllNodes();
		#endregion

		#region Polygon convex decomposition
		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool DecomposeStart(Int32 VertexCount, [In] IntPtr Vertices, out Int32 PolygonCount);

		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool DecomposeGetSizes(Int32 PolygonCount, [Out] IntPtr PolygonSizes);

		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool DecomposeGetPolygon(Int32 PolygonIndex, Int32 VertexSize, [Out] IntPtr Vertices);
		#endregion

		#region Models C++ functions
		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool GenerateTree(Int32 UserID, out Int32 GeneratedTreeNodeID);

		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool SetTreeEnabled(Int32 ModelNode, bool IsEnabled);

		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool RayCastMutiGet(int ObjectCount, [Out] IntPtr OutputBrushIntersection);

		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern Int32 RayCastIntoTreeMultiCount(Int32 ModelNodeID,
															[In] ref Vector3 WorldRayStart,
															[In] ref Vector3 WorldRayEnd,
															[In] ref Matrix4x4 ModelLocalToWorldMatrix,
															int InFilterInvisiblePolygons,
															bool IgnoreInvisiblePolygons,
															[In] IntPtr IgnoreNodeIDs,
															Int32 IgnoreNodeIDCount);

		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool RayCastIntoBrush(Int32 BrushNodeID,
													[In] ref Vector3 RayStart,
													[In] ref Vector3 RayEnd,
													[In] ref Matrix4x4 ModelLocalToWorldMatrix,
													bool IgnoreInvisiblePolygons,
													out CSGTreeBrushIntersection OutputBrushIntersection);

		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool RayCastIntoBrushSurface(Int32 BrushNodeID,
															Int32 SurfaceID,
															[In] ref Vector3 RayStart,
															[In] ref Vector3 RayEnd,
															[In] ref Matrix4x4 ModelLocalToWorldMatrix,
															out CSGSurfaceIntersection OutputSurfaceIntersection);

		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int FindNodesInFrustum(Int32 ModelNodeID, Int32 PlaneCount, [In] IntPtr Planes);

		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool RetrieveUserIDsInFrustum(Int32 ObjectIDCount, [Out] IntPtr ObjectIDs);

		// CSG temp var
		static readonly List<LegacyBrushIntersection> IntersectionList = new List<LegacyBrushIntersection>();
		static int PrevIntersectionCount = -1;
		static CSGTreeBrushIntersection[] OutputIntersections;
		#endregion

		#region Operation C++ functions
		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool GenerateBranch(Int32 UserID, out Int32 GeneratedOperationNodeID);

		// CSG temp function
		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool SetBranchOperationType(Int32 OperationNodeID, uint Operation);

		#endregion

		#region Brush C++ functions
		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool GenerateBrush(Int32 UserID, out Int32 GeneratedBrushNodeID);

		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern Int32 GetBrushMeshID(Int32 BrushNodeID);

		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool SetBrushMeshID(Int32 BrushNodeID, Int32 BrushMeshIndex);

		// CSG temp function
		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
		private static extern CSGTreeBrushFlags GetBrushFlags(Int32 BrushNodeID);
		#endregion

		#region Others
		[DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SetDirty(Int32 NodeID);

        [DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SetChildNodes(Int32 NodeID, Int32 ChildCount, [In] IntPtr ChildrenNodeIDs);

        [DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool DestroyNode(Int32 NodeID);

        [DllImport(NativePluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool DestroyNodes(Int32 NodeCount, [In] IntPtr NodeIDs);
		#endregion


		#endregion

		// CSG temp function
		private static List<List<Vector2>> ConvexPartition(Vector2[] Points)
        {
			var Polygons = new List<List<Vector2>>();
			return Polygons;
        }

        // CSG temp function
        private static bool RayCastIntoModelMulti()
        {
            return true;
        }

		// CSG temp function
		private static bool RayCastMulti()
        {
			return true;
        }

		// CSG temp function
		private static bool RayCastIntoBrush()
        {
			return true;
        }

		// CSG temp function
		private static bool RayCastIntoBrushSurface()
        {
			return true;
        }

		// CSG temp function
		private static bool GetItemsInFrustum()
        {
			return true;
        }

		// CSG temp function
		private static bool SetChildNodes(Int32 NodeID, Int32 ChildCount, Int32[] ChildrenNodeIDs)
        {
			return true;
        }

		// CSG temp function
		private static bool DestroyNodes(Int32 NodeCount, Int32[] NodeIDs)
        {
			return true;
        }
    }
}

#endif