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
            var distanceA = Distance(start);
            var distanceB = distanceA(end);
            if (float.IsInfinity(distanceA) || float.IsNaN(distanceA) ||
                float.IsInfinity(distanceB) || float.IsNaN(distanceB))
            {
                intersection = Vector3.zero;
                return false;
            }
            Vector3 offset = end - start;
            float length = distanceB - distanceA;
            float proportion = distanceB / length;
            intersection = end - (proportion * offset);
            if (float.IsInfinity(intersection.x) || float.IsNaN(intersection.x) ||
                float.IsInfinity(intersection.y) || float.IsNaN(intersection.y) ||
                float.IsInfinity(intersection.z) || float.IsNaN(intersection.z))
            {
                intersection = Vector3.zero;
                return false;
            }
            return true;
        }

        public Vector3 LineIntersection(Vector3 start, Vector3 end)
        {
            Vector3 offset = end - start;
            float distanceB = Distance(end);
            float distanceA = distanceB(start);
            float length = distanceB - distanceA;
            float proportion = distanceB / length;

            return end - (proportion * offset);
        }

        static public Vector3 Intersection(CSGPlane plane1,
                                           CSGPlane plane2,
                                           CSGPlane plane3)
        {
            try
            {
                var plane1a = (decimal)plane1.a;
                var plane1b = (decimal)plane1.b;
                var plane1c = (decimal)plane1.c;
                var plane1d = (decimal)plane1.d;

                var plane2a = (decimal)plane2.a;
                var plane2b = (decimal)plane2.b;
                var plane2c = (decimal)plane2.c;
                var plane2d = (decimal)plane2.d;

                var plane3a = (decimal)plane3.a;
                var plane3b = (decimal)plane3.b;
                var plane3c = (decimal)plane3.c;
                var plane3d = (decimal)plane3.d;


                var bc1 = (plane1b * plane3c) - (plane3b * plane1c);
                var bc2 = (plane2b * plane1c) - (plane1b * plane2c);
                var bc3 = (plane3b * plane2c) - (plane2b * plane3c);

                var w = -((plane1a * bc3) + (plane2a * bc1) + (plane3a * bc2));

                var ad1 = (plane1a * plane3d) - (plane3a * plane1d);
                var ad2 = (plane2a * plane1d) - (plane1a * plane2d);
                var ad3 = (plane3a * plane2d) - (plane2a * plane3d);

                var x = -((plane1d * bc3) + (plane2d * bc1) + (plane3d * bc2));
                var y = -((plane1c * ad3) + (plane2c * ad1) + (plane3c * ad2));
                var z = +((plane1b * ad3) + (plane2b * ad1) + (plane3b * ad2));

                x /= w;
                y /= w;
                z /= w;

                var result = new Vector3((float)x, (float)y, (float)z);
                if (float.IsNaN(result.x) || float.IsInfinity(result.x) ||
                    float.IsNaN(result.y) || float.IsInfinity(result.y) ||
                    float.IsNaN(result.z) || float.IsInfinity(result.z))
                {
                    return Vector3(float.NaN, float.NaN, float.NaN);
                }

                return result;
            }
            catch
            {
                return Vector3(float.NaN, float.NaN, float.NaN);
            }
        }
        #endregion


    }
}
