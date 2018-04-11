using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    /// <summary>
    /// Represents a sphere in 3d space
    /// </summary>
    public class Sphere  {
        /// <summary>
        /// Center of sphere
        /// </summary>
        public Vector3 center;
        /// <summary>
        /// Radius of sphere
        /// </summary>
        public float radius;

        /// <summary>
        /// Sphere of 0 radius
        /// </summary>
        public Sphere() { }
        /// <summary>
        /// Sphere at position with radius
        /// </summary>
        /// <param name="center">position</param>
        /// <param name="radius">radius</param>
        public Sphere(Vector3 center, float radius) {
            this.radius = radius;
            this.center = center;
        }

        /// <summary>
        /// Hashcode
        /// </summary>
        /// <returns></returns>
		public override int GetHashCode ()
		{
			return center.GetHashCode () + radius.GetHashCode ();
		}
    }
}
