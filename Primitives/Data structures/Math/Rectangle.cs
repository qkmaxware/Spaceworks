using UnityEngine;

namespace Spaceworks {

    /// <summary>
    /// Represents a region of 2d space
    /// </summary>
    public class Rectangle2 {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;
        public Vector2 d;

        /// <summary>
        /// Create empty 
        /// </summary>
        public Rectangle2() { }

        /// <summary>
        /// Create from points
        /// </summary>
        /// <param name="tl"></param>
        /// <param name="tr"></param>
        /// <param name="br"></param>
        /// <param name="bl"></param>
        public Rectangle2(Vector2 tl, Vector2 tr, Vector2 br, Vector2 bl) {
            this.a = tl; this.b = tr; this.c = br; this.d = bl;
        }

        /// <summary>
        /// Center of the region
        /// </summary>
        /// <returns></returns>
        public Vector2 center {
            get {
                return (a + b + c + d) * 0.25f;
            }
        }

        /// <summary>
        /// Circular radius
        /// </summary>
        /// <returns></returns>
        public float radius {
            get {
                Vector2 center = this.center;
                return Mathf.Max(
                    Vector2.Distance(center, a),
                    Vector2.Distance(center, b),
                    Vector2.Distance(center, c),
                    Vector2.Distance(center, d)
                );
            }
        }

        /// <summary>
        /// Scale region by a constant
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public Rectangle2 scale(float f) {
            Rectangle2 b = new Rectangle2();
            b.a = this.a * f;
            b.b = this.b * f;
            b.c = this.c * f;
            b.d = this.d * f;
            return b;
        }

    }

    /// <summary>
    /// Flat planar region of 3d space
    /// </summary>
    public class Rectangle3 {
		/// <summary>
		/// Top Left
		/// </summary>
	    public Vector3 a;
		/// <summary>
		/// Top Right
		/// </summary>
        public Vector3 b;
		/// <summary>
		/// Bottom Right
		/// </summary>
        public Vector3 c;
		/// <summary>
		/// Bottom Left
		/// </summary>
        public Vector3 d;

        /// <summary>
        /// Zero area region
        /// </summary>
        public Rectangle3() { }
        /// <summary>
        /// Create from points in 3d space
        /// </summary>
        /// <param name="tl"></param>
        /// <param name="tr"></param>
        /// <param name="br"></param>
        /// <param name="bl"></param>
        public Rectangle3(Vector3 tl, Vector3 tr, Vector3 br, Vector3 bl) {
            this.a = tl; this.b = tr; this.c = br; this.d = bl;
        }

        /// <summary>
        /// Center of this region
        /// </summary>
        /// <returns></returns>
        public Vector3 center {
            get {
                return (a + b + c + d) * 0.25f;
            }
        }

        /// <summary>
        /// Normal vector of this plane
        /// </summary>
        /// <returns></returns>
        public Vector3 normal {
            get {
                return Vector3.Normalize(Vector3.Cross((b - a).normalized, (d - a).normalized));
            }
        }

        /// <summary>
        /// Spherical radius of this plane from center
        /// </summary>
        /// <returns></returns>
        public float radius {
            get {
                Vector3 center = this.center;
                return Mathf.Max(
                    Vector3.Distance(center, a),
                    Vector3.Distance(center, b),
                    Vector3.Distance(center, c),
                    Vector3.Distance(center, d)
                );
            }
        }

        /// <summary>
        /// Scale region by a constant
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public Rectangle3 scale(float f) {
            Rectangle3 b = new Rectangle3();
            b.a = this.a * f;
            b.b = this.b * f;
            b.c = this.c * f;
            b.d = this.d * f;
            return b;
        }

    }

}