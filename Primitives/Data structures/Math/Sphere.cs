using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    public class Sphere  {
        public Vector3 center;
        public float radius;

        public Sphere() { }
        public Sphere(Vector3 center, float radius) {
            this.radius = radius;
            this.center = center;
        }

		public override int GetHashCode ()
		{
			return center.GetHashCode () + radius.GetHashCode ();
		}
    }
}
