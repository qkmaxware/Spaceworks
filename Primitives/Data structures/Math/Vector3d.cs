using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    /// <summary>
    /// Double precision 3d vector
    /// </summary>
    [System.Serializable]
    public class Vector3d {
        public double x;
        public double y;
        public double z;

        public static readonly Vector3d zero = new Vector3d();
        public static readonly Vector3d one = new Vector3d(1, 1, 1);
        public static readonly Vector3d i = new Vector3d(1, 0, 0);
        public static readonly Vector3d j = new Vector3d(0, 1, 0);
        public static readonly Vector3d k = new Vector3d(0, 0, 1);
        public static readonly Vector3d right = i;
        public static readonly Vector3d left = -1 * right;
        public static readonly Vector3d up = j;
        public static readonly Vector3d down = -1 * up;
        public static readonly Vector3d forward = k;
        public static readonly Vector3d back = -1 * forward;

        /// <summary>
        /// Convert double precision vector to single precision
        /// </summary>
        public Vector3 vector3 {
            get {
                return new Vector3((float)x, (float)y, (float)z);
            }
        }

        /// <summary>
        /// Length of the vector
        /// </summary>
        public double magnitude {
            get {
                return System.Math.Sqrt(this.sqrMagnitude);
            }
        }

        /// <summary>
        /// Squared length
        /// </summary>
        public double sqrMagnitude {
            get {
                return x * x + y * y + z * z;
            }
        }

        /// <summary>
        /// Vector with same direction of unit length
        /// </summary>
        public Vector3d normalized {
            get {
                double M = this.magnitude;
                if (M == 0)
                    return Vector3d.zero;
                double m = 1 / M;
                return new Vector3d(x * m, y * m, z * m);
            }
        }

        /// <summary>
        /// Construct vector from components
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3d(double x = 0, double y = 0, double z = 0) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Construct vector from lower precision vector
        /// </summary>
        /// <param name="vec"></param>
        public Vector3d(Vector3 vec) {
            this.x = vec.x;
            this.y = vec.y;
            this.z = vec.z;
        }

        /// <summary>
        /// Copy a double precision vector
        /// </summary>
        /// <param name="other"></param>
        public Vector3d(Vector3d other) {
            this.x = other.x;
            this.y = other.y;
            this.z = other.z;
        }

        /// <summary>
        /// Cross product of two vectors
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3d Cross(Vector3d u, Vector3d v) {
            return new Vector3d(
                u.y * v.z - u.z * v.y,
                u.z * v.x - u.x * v.z,
                u.x * v.y - u.y * v.x
            );
        }

        /// <summary>
        /// Dot product of two vectors
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double Dot(Vector3d u, Vector3d v) {
            return u.x * v.x + u.y * v.y + u.z * v.z;
        }

        /// <summary>
        /// Compute the angle between two vectors in radians
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Angle(Vector3d a, Vector3d b) {
            double dot = Vector3d.Dot(a.normalized, b.normalized);
            return System.Math.Acos(
                dot < -1 ? -1 : (dot > 1 ? 1 : dot)
            );
        }

        /// <summary>
        /// Rotate a vector about a normal by a given angle
        /// </summary>
        /// <param name="v">vector to rotate</param>
        /// <param name="rad">angle in radians</param>
        /// <param name="n">normal vector</param>
        /// <returns></returns>
        public static Vector3d Rotate(Vector3d v, double rad, Vector3d n) {
            double cos = System.Math.Cos(rad);
            double sin = System.Math.Sin(rad);

            double oneMinusCos = 1 - cos;

            double a11 = oneMinusCos * n.x * n.x + cos;
            double a12 = oneMinusCos * n.x * n.y - n.z * sin;
            double a13 = oneMinusCos * n.x * n.z + n.y * sin;
            double a21 = oneMinusCos * n.x * n.y + n.z * sin;
            double a22 = oneMinusCos * n.y * n.y + cos;
            double a23 = oneMinusCos * n.y * n.z - n.x * sin;
            double a31 = oneMinusCos * n.x * n.z - n.y * sin;
            double a32 = oneMinusCos * n.y * n.z + n.x * sin;
            double a33 = oneMinusCos * n.z * n.z + cos;

            return new Vector3d(
                v.x * a11 + v.y * a12 + v.z * a13,
                v.x * a21 + v.y * a22 + v.z * a23,
                v.x * a31 + v.y * a32 + v.z * a33
            );
        }

        #region operators

        /// <summary>
        /// Add two vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3d operator +(Vector3d a, Vector3d b) {
            return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        /// <summary>
        /// Add two vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3d operator +(Vector3 a, Vector3d b) {
            return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        /// <summary>
        /// Add two vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3d operator +(Vector3d a, Vector3 b) {
            return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        /// <summary>
        /// Subtract two vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3d operator -(Vector3d a, Vector3d b) {
            return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        /// <summary>
        /// Subtract two vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3d operator -(Vector3 a, Vector3d b) {
            return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        /// <summary>
        /// Subtract two vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3d operator -(Vector3d a, Vector3 b) {
            return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        /// <summary>
        /// Scale vector
        /// </summary>
        /// <param name="a"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3d operator *(double a, Vector3d v) {
            return new Vector3d(a * v.x, a * v.y, a * v.z);
        }
        /// <summary>
        /// Scale vector
        /// </summary>
        /// <param name="v"></param>
        /// <param name="a"></param>
        /// <returns></returns>

        public static Vector3d operator *(Vector3d v, double a) {
            return new Vector3d(a * v.x, a * v.y, a * v.z);
        }

        /// <summary>
        /// Scale vector
        /// </summary>
        /// <param name="v"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector3d operator /(Vector3d v, double a) {
            return v * (1 / a);
        }

        /// <summary>
        /// Test vector equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Vector3d a, Vector3d b) {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        /// <summary>
        /// Test vector inequality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Vector3d a, Vector3d b) {
            return !(a == b);
        }

        /// <summary>
        /// Test vector equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            if (obj is Vector3d) {
                return this == (Vector3d)obj;
            }
            return false;
        }

        /// <summary>
        /// Get hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return base.GetHashCode();
        }

        /// <summary>
        /// Convert to string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return "(" + this.x + ", " + this.y + ", " + this.z + ")"; 
        }

        #endregion
    }


}

