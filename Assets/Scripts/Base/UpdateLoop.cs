using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;

namespace CSG
{
    internal sealed class UpdateLoop
    {
        static UpdateLoop CSGInstance = null;

        bool bHasRegister = false;

        static UpdateLoop()
        {
            if (CSGInstance != null)
            {
                CSGInstance.
            }
        }

        void ShutDown(bool Finalizing = false)
        {
            if (CSGInstance != this)
            {
                return;
            }

            CSGInstance = null;

            EditorApplication.update -= OnFirstUpdate;
        }

        void OnFirstUpdate()
        {
            bHasRegister = true;
            EditorApplication.update -= OnFirstUpdate;
            CSG.CSGSettings.Reload();
        }
    }
}
