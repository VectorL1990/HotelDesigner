using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;


namespace RealtimeCSG
{
    [Serializable, StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    public struct CSGPlane
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
            var distanceB = Distance(end);
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
            float distanceA = Distance(start);
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
                var plane1a = (decimal)plane1.A;
                var plane1b = (decimal)plane1.B;
                var plane1c = (decimal)plane1.C;
                var plane1d = (decimal)plane1.D;

                var plane2a = (decimal)plane2.A;
                var plane2b = (decimal)plane2.B;
                var plane2c = (decimal)plane2.C;
                var plane2d = (decimal)plane2.D;

                var plane3a = (decimal)plane3.A;
                var plane3b = (decimal)plane3.B;
                var plane3c = (decimal)plane3.C;
                var plane3d = (decimal)plane3.D;


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

        #region Utilities
        public float Distance(Vector3 point)
        {
            float distance = A * point.x + B * point.y + C * point.z - D;
            return distance;
        }

        public void Normalize()
        {
            var magnitude = 1.0f / Mathf.Sqrt(A * A + B * B + C * C);
            A *= magnitude;
            B *= magnitude;
            C *= magnitude;
            D *= magnitude;
        }

        public void Transform(Matrix4x4 transformation)
        {
            var ittrans = transformation.inverse.transpose;
            var vector = ittrans * new Vector4(A, B, C, -D);
            A = vector.x;
            B = vector.y;
            C = vector.z;
            D = -vector.w;
        }

        public CSGPlane Negated() 
        {
            return new CSGPlane(-A, -B, -C, -D); 
        }

        public CSGPlane Translated(Vector3 translation)
        {
            return new CSGPlane(A, B, C,
                                // translated offset = Normal.Dotproduct(translation)
                                // normal = A,B,C
                                D + (A * translation.x) +
                                    (B * translation.y) +
                                    (C * translation.z));
        }

        /// <summary>Project a point on this plane</summary>
        /// <param name="point">A point</param>
        /// <returns>The projected point</returns>
        public Vector3 Project(Vector3 point)
        {
            float px = point.x;
            float py = point.y;
            float pz = point.z;

            float nx = Normal.x;
            float ny = Normal.y;
            float nz = Normal.z;

            float ax = (px - (nx * D)) * nx;
            float ay = (py - (ny * D)) * ny;
            float az = (pz - (nz * D)) * nz;
            float dot = ax + ay + az;

            float rx = px - (dot * nx);
            float ry = py - (dot * ny);
            float rz = pz - (dot * nz);

            return new Vector3(rx, ry, rz);
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^
                    B.GetHashCode() ^
                    C.GetHashCode() ^
                    D.GetHashCode();
        }

        public bool Equals(CSGPlane other)
        {
            if (System.Object.ReferenceEquals(this, other))
                return true;
            if (System.Object.ReferenceEquals(other, null))
                return false;
            return Mathf.Abs(this.Distance(other.PointOnPlane)) <= CommonVariables.DistanceEpsilon &&
                    Mathf.Abs(other.Distance(this.PointOnPlane)) <= CommonVariables.DistanceEpsilon &&
                    Mathf.Abs(A - other.A) <= CommonVariables.NormalEpsilon &&
                    Mathf.Abs(B - other.B) <= CommonVariables.NormalEpsilon &&
                    Mathf.Abs(C - other.C) <= CommonVariables.NormalEpsilon;
        }

        public override bool Equals(object obj)
        {
            if (System.Object.ReferenceEquals(this, obj))
                return true;
            if (!(obj is CSGPlane))
                return false;
            CSGPlane other = (CSGPlane)obj;
            if (System.Object.ReferenceEquals(other, null))
                return false;
            return Mathf.Abs(this.Distance(other.PointOnPlane)) <= CommonVariables.DistanceEpsilon &&
                    Mathf.Abs(other.Distance(this.PointOnPlane)) <= CommonVariables.DistanceEpsilon &&
                    Mathf.Abs(A - other.A) <= CommonVariables.NormalEpsilon &&
                    Mathf.Abs(B - other.B) <= CommonVariables.NormalEpsilon &&
                    Mathf.Abs(C - other.C) <= CommonVariables.NormalEpsilon;
        }

        public static bool operator ==(CSGPlane left, CSGPlane right)
        {
            if (System.Object.ReferenceEquals(left, right))
                return true;
            if (System.Object.ReferenceEquals(left, null) ||
                System.Object.ReferenceEquals(right, null))
                return false;
            return Mathf.Abs(left.Distance(right.PointOnPlane)) <= CommonVariables.DistanceEpsilon &&
                    Mathf.Abs(right.Distance(left.PointOnPlane)) <= CommonVariables.DistanceEpsilon &&
                    Mathf.Abs(left.A - right.A) <= CommonVariables.NormalEpsilon &&
                    Mathf.Abs(left.B - right.B) <= CommonVariables.NormalEpsilon &&
                    Mathf.Abs(left.C - right.C) <= CommonVariables.NormalEpsilon;
        }

        public static bool operator !=(CSGPlane left, CSGPlane right)
        {
            if (System.Object.ReferenceEquals(left, right))
                return false;
            if (System.Object.ReferenceEquals(left, null) ||
                System.Object.ReferenceEquals(right, null))
                return true;
            return Mathf.Abs(left.Distance(right.PointOnPlane)) > CommonVariables.DistanceEpsilon &&
                    Mathf.Abs(right.Distance(left.PointOnPlane)) > CommonVariables.DistanceEpsilon &&
                    Mathf.Abs(left.A - right.A) > CommonVariables.NormalEpsilon ||
                    Mathf.Abs(left.B - right.B) > CommonVariables.NormalEpsilon ||
                    Mathf.Abs(left.C - right.C) > CommonVariables.NormalEpsilon;
        }
        #endregion

        public override string ToString() 
        { 
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2}, {3})", A, B, C, D); 
        }
    }
}
