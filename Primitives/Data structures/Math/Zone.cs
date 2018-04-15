using UnityEngine;

namespace Spaceworks {

    /// <summary>
    /// Represents a region of 2d space
    /// </summary>
    public class Zone2 {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;
        public Vector2 d;

        /// <summary>
        /// Create empty 
        /// </summary>
        public Zone2() { }

        /// <summary>
        /// Create from points
        /// </summary>
        /// <param name="tl"></param>
        /// <param name="tr"></param>
        /// <param name="br"></param>
        /// <param name="bl"></param>
        public Zone2(Vector2 tl, Vector2 tr, Vector2 br, Vector2 bl) {
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
        public Zone2 scale(float f) {
            Zone2 b = new Zone2();
            b.a = this.a * f;
            b.b = this.b * f;
            b.c = this.c * f;
            b.d = this.d * f;
            return b;
        }

        /// <summary>
        /// Subdivide this range into 4 quadrants
        /// </summary>
        /// <param name="NE">North east zone</param>
        /// <param name="NW">North west zone</param>
        /// <param name="SE">South east zone</param>
        /// <param name="SW">South west zone</param>
        public void QuadSubdivide(out Zone2 NE, out Zone2 NW, out Zone2 SE, out Zone2 SW){
            //Create 4 subdivided ranges
            Vector2 topl = this.a;
            Vector2 topr = this.b;
            Vector2 btnl = this.d;
            Vector2 btnr = this.c;

            Vector2 tc = Vector3.Lerp(topl, topr, 0.5f);
            Vector2 lm = Vector3.Lerp(topl, btnl, 0.5f);
            Vector2 rm = Vector3.Lerp(topr, btnr, 0.5f);
            Vector2 mc = Vector3.Lerp(lm, rm, 0.5f);
            Vector2 bc = Vector3.Lerp(btnl, btnr, 0.5f);

            NW = new Zone2(topl, tc, mc, lm);
            NE = new Zone2(tc, topr, rm, mc);
            SW = new Zone2(lm, mc, bc, btnl);
            SE = new Zone2(mc, rm, btnr, bc);
        }

    }

    /// <summary>
    /// Flat planar region of 3d space
    /// </summary>
    public class Zone3 {
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
        public Zone3() { }
        /// <summary>
        /// Create from points in 3d space
        /// </summary>
        /// <param name="tl"></param>
        /// <param name="tr"></param>
        /// <param name="br"></param>
        /// <param name="bl"></param>
        public Zone3(Vector3 tl, Vector3 tr, Vector3 br, Vector3 bl) {
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
        public Zone3 scale(float f) {
            Zone3 b = new Zone3();
            b.a = this.a * f;
            b.b = this.b * f;
            b.c = this.c * f;
            b.d = this.d * f;
            return b;
        }

        /// <summary>
        /// Subdivide this range into 4 quadrants
        /// </summary>
        /// <param name="NE">North east zone</param>
        /// <param name="NW">North west zone</param>
        /// <param name="SE">South east zone</param>
        /// <param name="SW">South west zone</param>
        public void QuadSubdivide(out Zone3 NE, out Zone3 NW, out Zone3 SE, out Zone3 SW){
            //Create 4 subdivided ranges
            Vector3 topl = this.a;
            Vector3 topr = this.b;
            Vector3 btnl = this.d;
            Vector3 btnr = this.c;

            Vector3 tc = Vector3.Lerp(topl, topr, 0.5f);
            Vector3 lm = Vector3.Lerp(topl, btnl, 0.5f);
            Vector3 rm = Vector3.Lerp(topr, btnr, 0.5f);
            Vector3 mc = Vector3.Lerp(lm, rm, 0.5f);
            Vector3 bc = Vector3.Lerp(btnl, btnr, 0.5f);

            NW = new Zone3(topl, tc, mc, lm);
            NE = new Zone3(tc, topr, rm, mc);
            SW = new Zone3(lm, mc, bc, btnl);
            SE = new Zone3(mc, rm, btnr, bc);
        }

    }

}