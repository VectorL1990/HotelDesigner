using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RealtimeCSG
{
    [Serializable]
    public sealed class CSGShape
    {
        [SerializeField] public CSGSurface[] Surfaces = new CSGSurface[0];

        [SerializeField] public TexGen[] TexGens = new TexGen[0];

        [SerializeField] public TexGenFlag[] TexGenFlags = new TexGenFlag[0];

#if UNITY_EDITOR
        public CSGShape()
        {

        }

        public CSGShape(CSGShape other)
        {
            CopyFrom(other);
        }

        public CSGShape(int polygonCount)
        {
            Surfaces = new CSGSurface[polygonCount];
            TexGenFlags = new TexGenFlag[polygonCount];
            TexGens = new TexGen[polygonCount];
        }

        public void Reset()
        {
            Surfaces = new CSGSurface[0];
            TexGens = new TexGen[0];
            TexGenFlags = new TexGenFlag[0];
        }

        public void CopyFrom(CSGShape other)
        {
            if (other == null)
            {
                Reset();
                return;
            }

            if (Surfaces != null)
            {
                if (Surfaces == null || Surfaces.Length != other.Surfaces.Length)
                {
                    Surfaces = new CSGSurface[other.Surfaces.Length];
                }
                Array.Copy(other.Surfaces, Surfaces, other.Surfaces.Length);
            }
            else
            {
                Surfaces = null;
            }

            if (TexGens != null)
            {
                if (TexGens == null || TexGens.Length != other.TexGens.Length)
                {
                    TexGens = new TexGen[other.TexGens.Length];
                }
                Array.Copy(other.TexGens, TexGens, other.TexGens.Length);
            }
            else
            {
                TexGens = null;
            }

            if (TexGenFlags != null)
            {
                if (TexGenFlags == null || TexGenFlags.Length != other.TexGenFlags.Length)
                {
                    TexGenFlags = new TexGenFlag[other.TexGenFlags.Length];
                }
                Array.Copy(other.TexGenFlags, TexGenFlags, other.TexGenFlags.Length);
            }
            else
            {
                TexGenFlags = null;
            }
        }

        public CSGShape Clone()
        {
            return new CSGShape(this);
        }

        public bool CheckMaterials()
        {
            bool dirty = false;
            if (Surfaces == null || Surfaces.Length == 0)
            {
                Debug.LogWarning("Surfaces == null || Surfaces.Length == 0");
                return true;
            }

            int maxTexGenIndex = 0;
            for (int i = 0; i < Surfaces.Length; i++)
            {
                maxTexGenIndex = Mathf.Max(maxTexGenIndex, Surfaces[i].TexGenIndex);
            }
            maxTexGenIndex++;

            if (TexGens == null || TexGens.Length < maxTexGenIndex)
            {
                dirty = true;
                var newTexGens = new TexGen[maxTexGenIndex];
                var newTexGenFlags = new TexGenFlag[maxTexGenIndex];
                if (TexGens != null &&
                    TexGens.Length > 0)
                {
                    for (int i = 0; i < TexGens.Length; i++)
                    {
                        newTexGens[i] = TexGens[i];
                        newTexGenFlags[i] = TexGenFlags[i];
                    }
                }
                TexGens = newTexGens;
                TexGenFlags = newTexGenFlags;
            }

            return dirty;
        }

        public void EnsureInitialized(CSGShape shape)
        {
            if (TexGens == null || TexGens.Length != shape.TexGens.Length)
            {
                TexGens = new TexGen[shape.TexGens.Length];
                TexGenFlags = new TexGenFlag[shape.TexGens.Length];
            }
            Array.Copy(shape.TexGens, TexGens, shape.TexGens.Length);
            Array.Copy(shape.TexGenFlags, TexGenFlags, shape.TexGenFlags.Length);

            if (Surfaces == null || Surfaces.Length != shape.Surfaces.Length)
            {
                Surfaces = new CSGSurface[shape.Surfaces.Length];
            }
            Array.Copy(shape.Surfaces, Surfaces, shape.Surfaces.Length);
        }
#endif
    }
}
