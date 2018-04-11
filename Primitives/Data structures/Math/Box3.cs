using UnityEngine;

namespace Spaceworks {

    /// <summary>
    /// 3d Axis aligned bounding volume box
    /// </summary>
    public class Box3 {

        /// <summary>
        /// Minimum point on the box
        /// </summary>
        /// <returns></returns>
        public Vector3 min {get; private set;}

        /// <summary>
        /// Maximum point on the box
        /// </summary>
        /// <returns></returns>
        public Vector3 max {get; private set;}

        /// <summary>
        /// Center of the box
        /// </summary>
        /// <returns></returns>
        public Vector3 center {
            get {
                return new Vector3(
                    (max.x + min.x) / 2,
                    (max.y + min.y) / 2,
                    (max.z + min.z) / 2
                );
            }
        }

        //Create a new box spanning the two points
        public Box3(Vector3 a, Vector3 b){
            min = new Vector3(
                Mathf.Min(a.x, b.x),
                Mathf.Min(a.y, b.y),
                Mathf.Min(a.z, b.z)
            );

            max = new Vector3(
                Mathf.Max(a.x, b.x),
                Mathf.Max(a.y, b.y),
                Mathf.Max(a.z, b.z)
            );
        }

        /// <summary>
        /// Test if 2 boxes intersect
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Intersects(Box3 other) {
            Vector3 d1 = other.min - max;
            Vector3 d2 = min - other.max;

            double m = Mathf.Max(d1.x, d2.x, d1.y, d2.y, d1.z, d2.z);
            return m < 0;
        }

        /// <summary>
        /// Test if box contains a point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vector3 point) {
            return (point.x > min.x && point.y > min.y && point.z > min.z) &&
                   (point.x < max.z && point.y < max.y && point.z < max.z);
        }

    }
}