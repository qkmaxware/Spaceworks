using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    public class Range {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;
        public Vector2 d;

        public Range() { }
        public Range(Vector2 tl, Vector2 tr, Vector2 br, Vector2 bl) {
            this.a = tl; this.b = tr; this.c = br; this.d = bl;
        }

        public Vector2 center {
            get {
                return (a + b + c + d) * 0.25f;
            }
        }

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

        public Range scale(float f) {
            Range b = new Range();
            b.a = this.a * f;
            b.b = this.b * f;
            b.c = this.c * f;
            b.d = this.d * f;
            return b;
        }

    }

    public class Range3d {
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

        public Range3d() { }
        public Range3d(Vector3 tl, Vector3 tr, Vector3 br, Vector3 bl) {
            this.a = tl; this.b = tr; this.c = br; this.d = bl;
        }

        public Vector3 center {
            get {
                return (a + b + c + d) * 0.25f;
            }
        }

        public Vector3 normal {
            get {
                return Vector3.Normalize(Vector3.Cross((b - a).normalized, (d - a).normalized));
            }
        }

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

        public Range3d scale(float f) {
            Range3d b = new Range3d();
            b.a = this.a * f;
            b.b = this.b * f;
            b.c = this.c * f;
            b.d = this.d * f;
            return b;
        }

    }

}