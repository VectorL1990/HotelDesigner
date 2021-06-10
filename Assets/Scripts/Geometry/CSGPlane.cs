using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;


namespace RealtimeCSG
{
    [Serializable, StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    class CSGPlane
    {
        // plane equation: Ax+By+Cz-D=0
        public float A;
        public float B;
        public float C;
        public float D;

        // Normal vector is equal to equation's coefficients
        // Because A(x1 - x2) + B(y1 - y2) + C(z1 - z2) - D = 0
        // (x1 - x2, y1 - y2, z1 - z2) is a vector in plane, so dot product with normal is zero
        public Vector3 Normal
        {
            get { return new Vector3(A, B, C); }
            set 
            {
                A = value.x;
                B = value.y;
                C = value.z;
            }
        }

        // We should notice that Normal vector is normalized
        // Because if ABC is placed into plane equation we get A*AD + B*BD + C*CD - D = 0
        // Which means (A^2 + B^2 + C^2)D - D = 0
        public Vector3 PointOnPlane
        {
            get { return Normal * D; }
        }

        #region Constructor
        public CSGPlane(UnityEngine.Plane plane)
        {
            A = plane.normal.x;
            B = plane.normal.y;
            C = plane.normal.z;
            D = -plane.distance;
        }

        public CSGPlane(Vector3 normal, float d)
        {
            var norm = normal.normalized;
            A = norm.x;
            B = norm.y;
            C = norm.z;
            D = d;
        }

        public CSGPlane(Vector3 normal, Vector3 pointInPlane)
        {
            var norm = normal.normalized;
            A = norm.x;
            B = norm.y;
            C = norm.z;
            D = Vector3.Dot(normal, pointInPlane);
        }

        public CSGPlane(Quaternion rotation, Vector3 pointOnPlane)
        {
            var normal = (rotation * Vector3.up).normalized;
            A = normal.x;
            B = normal.y;
            C = normal.z;
            D = Vector3.Dot(normal, pointOnPlane);
        }

        public CSGPlane(float a, float b, float c, float d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            Normalize();
        }

        public CSGPlane(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var ab = (p2 - p1);
            var ac = (p3 - p1);
            var normal = Vector3.Cross(ab, ac).normalized;

            A = normal.x;
            B = normal.y;
            C = normal.z;
            D = Vector3.Dot(normal, p1);
        }
        #endregion


        #region Ray Intersection
        public Vector3 RayIntersection(UnityEngine.Ray ray)
        {
            var startX = (double)ray.origin.x;
            var startY = (double)ray.origin.y;
            var startZ = (double)ray.origin.z;

            var directionX = (double)ray.direction.x;
            var directionY = (double)ray.direction.y;
            var directionZ = (double)ray.direction.z;

            // Actually [A, B, C] can be thought as normal vector
            // Dot product operation makes start point(vector) project on to normal vector
            var distanceA = (A * startX) + (B * startY) + (C * startZ) - D;
            var length = (A * directionX) + (B * directionY) + (C * directionZ);
            var delta = distanceA / length;

            var x = startX - (delta * directionX);
            var y = startY - (delta * directionY);
            var z = startZ - (delta * directionZ);

            return new Vector3((float)x, (float)y, (float)z);
        }

        public bool TryRayIntersection(UnityEngine.Ray ray, out Vector3 intersection)
        {
            var start = ray.origin;
            var end = ray.origin + ray.direction * 1000.0f;
            var distanceA
        }
        #endregion
    }
}
