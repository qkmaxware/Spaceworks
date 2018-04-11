using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    /// <summary>
    /// 4x4 matrix with double precision
    /// </summary>
    [System.Serializable]
    public class Matrixd4x4 {

        //Elements
        public double m11; public double m12; public double m13; public double m14;
        public double m21; public double m22; public double m23; public double m24;
        public double m31; public double m32; public double m33; public double m34;
        public double m41; public double m42; public double m43; public double m44;

        /// <summary>
        /// Get a zero matrix
        /// </summary>
        /// <returns></returns>
        public static Matrixd4x4 zero {
            get {
                return new Matrixd4x4(
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                    0, 0, 0, 0
                );
            }
        }

        /// <summary>
        /// Get the identity matrix
        /// </summary>
        /// <returns></returns>
        public static Matrixd4x4 identity {
            get {
                return new Matrixd4x4(
                    1, 0, 0, 0,
                    0, 1, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1
                );
            }
        }

        /// <summary>
        /// Convert from double precision to unity single precision
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 matrix4x4 {
            get {
                return new Matrix4x4(
                    new Vector4((float)m11, (float)m21, (float)m31, (float)m41),
                    new Vector4((float)m12, (float)m22, (float)m32, (float)m42),
                    new Vector4((float)m13, (float)m23, (float)m33, (float)m43),
                    new Vector4((float)m14, (float)m24, (float)m34, (float)m44)
                );
            }
        }

        #region constructors

        /// <summary>
        /// Create matrix with components
        /// </summary>
        /// <param name="m11"></param>
        /// <param name="m12"></param>
        /// <param name="m13"></param>
        /// <param name="m14"></param>
        /// <param name="m21"></param>
        /// <param name="m22"></param>
        /// <param name="m23"></param>
        /// <param name="m24"></param>
        /// <param name="m31"></param>
        /// <param name="m32"></param>
        /// <param name="m33"></param>
        /// <param name="m34"></param>
        /// <param name="m41"></param>
        /// <param name="m42"></param>
        /// <param name="m43"></param>
        /// <param name="m44"></param>
        public Matrixd4x4(double m11 = 0, double m12 = 0, double m13 = 0, double m14 = 0, double m21 = 0, double m22 = 0, double m23 = 0, double m24 = 0, double m31 = 0, double m32 = 0, double m33 = 0, double m34 = 0, double m41 = 0, double m42 = 0, double m43 = 0, double m44 = 0) {
            this.m11 = m11;
            this.m12 = m12;
            this.m13 = m13;
            this.m14 = m14;

            this.m21 = m21;
            this.m22 = m22;
            this.m23 = m23;
            this.m24 = m24;

            this.m31 = m31;
            this.m32 = m32;
            this.m33 = m33;
            this.m34 = m34;

            this.m41 = m41;
            this.m42 = m42;
            this.m43 = m43;
            this.m44 = m44;
        }

        /// <summary>
        /// Create from single precision matrix
        /// </summary>
        /// <param name="mf"></param>
        public Matrixd4x4(Matrix4x4 mf) {
            this.m11 = mf.m00;
            this.m12 = mf.m01;
            this.m13 = mf.m02;
            this.m14 = mf.m03;

            this.m21 = mf.m10;
            this.m22 = mf.m11;
            this.m23 = mf.m12;
            this.m24 = mf.m13;

            this.m31 = mf.m20;
            this.m32 = mf.m21;
            this.m33 = mf.m22;
            this.m34 = mf.m23;

            this.m41 = mf.m30;
            this.m42 = mf.m31;
            this.m43 = mf.m32;
            this.m44 = mf.m33;
        }

        /// <summary>
        /// Copy from other double precision matrix
        /// </summary>
        /// <param name="o"></param>
        public Matrixd4x4(Matrixd4x4 o) {
            this.m11 = o.m11;
            this.m12 = o.m12;
            this.m13 = o.m13;
            this.m14 = o.m14;

            this.m21 = o.m21;
            this.m22 = o.m22;
            this.m23 = o.m23;
            this.m24 = o.m24;

            this.m31 = o.m31;
            this.m32 = o.m32;
            this.m33 = o.m33;
            this.m34 = o.m34;

            this.m41 = o.m41;
            this.m42 = o.m42;
            this.m43 = o.m43;
            this.m44 = o.m44;
        }

        #endregion

        #region operators

        /// <summary>
        /// Add two matrices
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrixd4x4 operator +(Matrixd4x4 a, Matrixd4x4 b) {
            return new Matrixd4x4(
                a.m11 + b.m11, a.m12 + b.m12, a.m13 + b.m13, a.m14 + b.m14,
                a.m21 + b.m21, a.m22 + b.m22, a.m23 + b.m23, a.m24 + b.m24,
                a.m31 + b.m31, a.m32 + b.m32, a.m33 + b.m33, a.m34 + b.m34,
                a.m41 + b.m41, a.m42 + b.m42, a.m43 + b.m43, a.m44 + b.m44
            );
        }

        /// <summary>
        /// Subtract two matrices
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrixd4x4 operator -(Matrixd4x4 a, Matrixd4x4 b) {
            return new Matrixd4x4(
                a.m11 - b.m11, a.m12 - b.m12, a.m13 - b.m13, a.m14 - b.m14,
                a.m21 - b.m21, a.m22 - b.m22, a.m23 - b.m23, a.m24 - b.m24,
                a.m31 - b.m31, a.m32 - b.m32, a.m33 - b.m33, a.m34 - b.m34,
                a.m41 - b.m41, a.m42 - b.m42, a.m43 - b.m43, a.m44 - b.m44
            );
        }

        /// <summary>
        /// Scale all elements of a matrix
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrixd4x4 operator *(Matrixd4x4 a, double b) {
            return new Matrixd4x4(
                a.m11 * b, a.m12 * b, a.m13 * b, a.m14 + b,
                a.m21 * b, a.m22 * b, a.m23 * b, a.m24 + b,
                a.m31 * b, a.m32 * b, a.m33 * b, a.m34 + b,
                a.m41 * b, a.m42 * b, a.m43 * b, a.m44 + b
            );
        }

        /// <summary>
        /// Scale all elements of a matrix
        /// </summary>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Matrixd4x4 operator *(double b, Matrixd4x4 a) {
            return a * b;
        }

        /// <summary>
        /// Multiply two matrices
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Matrixd4x4 operator *(Matrixd4x4 lhs, Matrixd4x4 rhs) {
            Matrixd4x4 result = new Matrixd4x4();

            result.m11 = lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31 + lhs.m14 * rhs.m41;
            result.m12 = lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32 + lhs.m14 * rhs.m42;
            result.m13 = lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33 + lhs.m14 * rhs.m43;
            result.m14 = lhs.m11 * rhs.m14 + lhs.m12 * rhs.m24 + lhs.m13 * rhs.m34 + lhs.m14 * rhs.m44;

            result.m21 = lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31 + lhs.m24 * rhs.m41;
            result.m22 = lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32 + lhs.m24 * rhs.m42;
            result.m23 = lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33 + lhs.m24 * rhs.m43;
            result.m24 = lhs.m21 * rhs.m14 + lhs.m22 * rhs.m24 + lhs.m23 * rhs.m34 + lhs.m24 * rhs.m44;

            result.m31 = lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31 + lhs.m34 * rhs.m41;
            result.m32 = lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32 + lhs.m34 * rhs.m42;
            result.m33 = lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33 + lhs.m34 * rhs.m43;
            result.m34 = lhs.m31 * rhs.m14 + lhs.m32 * rhs.m24 + lhs.m33 * rhs.m34 + lhs.m34 * rhs.m44;

            result.m41 = lhs.m41 * rhs.m11 + lhs.m42 * rhs.m21 + lhs.m43 * rhs.m31 + lhs.m44 * rhs.m41;
            result.m42 = lhs.m41 * rhs.m12 + lhs.m42 * rhs.m22 + lhs.m43 * rhs.m32 + lhs.m44 * rhs.m42;
            result.m43 = lhs.m41 * rhs.m13 + lhs.m42 * rhs.m23 + lhs.m43 * rhs.m33 + lhs.m44 * rhs.m43;
            result.m44 = lhs.m41 * rhs.m14 + lhs.m42 * rhs.m24 + lhs.m43 * rhs.m34 + lhs.m44 * rhs.m44;

            return result;
        }

        /// <summary>
        /// Multiply a matrix by a column vector
        /// </summary>
        /// <param name="m"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3d operator *(Matrixd4x4 m, Vector3d vector) {
            Vector3d result = new Vector3d();

            result.x = m.m11 * vector.x + m.m12 * vector.y + m.m13 * vector.z;
            result.y = m.m21 * vector.x + m.m22 * vector.y + m.m23 * vector.z;
            result.z = m.m31 * vector.x + m.m32 * vector.y + m.m33 * vector.z;

            return result;
        }

        /// <summary>
        /// Matrix equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Matrixd4x4 a, Matrixd4x4 b) {
            return a.m11 == b.m11 && a.m12 == b.m12 && a.m13 == b.m13 && a.m14 == b.m14
                && a.m21 == b.m21 && a.m22 == b.m22 && a.m23 == b.m23 && a.m24 == b.m24
                && a.m31 == b.m31 && a.m32 == b.m32 && a.m33 == b.m33 && a.m34 == b.m34
                && a.m41 == b.m41 && a.m42 == b.m42 && a.m43 == b.m43 && a.m44 == b.m44;
        }

        /// <summary>
        /// Matrix inequality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Matrixd4x4 a, Matrixd4x4 b) {
            return !(a == b);
        }

        /// <summary>
        /// Matrix equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            if (obj is Matrixd4x4) {
                return this == (Matrixd4x4)obj;
            }
            return false;
        }

        /// <summary>
        /// Matrix hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return base.GetHashCode();
        }

        #endregion

    }

}
