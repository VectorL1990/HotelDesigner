using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CSG
{
    [Serializable]
    public enum MeshEditMode
    {
        Place,
        Generate,
        Edit,
        Clip,
        Surfaces
    }

    internal interface IEditMode
    {
        bool bUseUnitySelection { get; }
        bool bIgnoreUnityRect { get; }

        void HandleEvents(SceneView InSceneView, Rect InRect);

        Rect GetLastSceneGUIRect();
        bool OnSceneGUI(Rect WindowRect);
        void OnInspectGUI(EditorWindow Window, float Height);
        void OnEnableTool();
        void OnDisableTool();
        bool DeselectAll();
        bool UndoRedoPerformed();
    }

    internal sealed class MeshEditorGUIManager : ScriptableObject
    {
        static MeshEditorGUIManager Instance = null;

        [NonSerialized] bool GenerateMode = false;
        [NonSerialized] FilteredSelection FilteredSelection_ = new FilteredSelection();
        [SerializeField] MeshEditMode EditMode = MeshEditMode.Place;
        [SerializeField] IEditMode ActiveTool = null;
        static IEditMode[] EditModeTools = null;
    }
}
