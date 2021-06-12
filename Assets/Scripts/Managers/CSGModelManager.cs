using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;
using System.Reflection;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;

namespace RealtimeCSG
{
    internal partial class CSGModelManager
    {
        private static bool IsInitialized = false;
        private static readonly object LockObj = new object();

        internal static NativeMethods External;
    }
}
