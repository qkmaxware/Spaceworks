using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    /// <summary>
    /// Represents 3 64-bit Long values
    /// </summary>
    [System.Serializable]
    public class Long3 {
        public long x;
        public long y;
        public long z;

        /// <summary>
        /// Long3 with all components zeroed
        /// </summary>
        public Long3() { }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public Long3(Long3 other) {
            this.x = other.x;
            this.y = other.y;
            this.z = other.z;
        }

        /// <summary>
        /// Create from 3 long values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Long3(long x, long y, long z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Equality tester
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(System.Object obj) {
            if (obj is Long3 == false)
                return false;
            Long3 r = (Long3)obj;
            return this.x == r.x && this.y == r.y && this.z == r.z;
        }

        /// <summary>
        /// Long3 hashcode 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return (int)(x ^ y ^ z);
        }

        /// <summary>
        /// Subtract two long3 values component wise
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Long3 operator -(Long3 a, Long3 b) {
            return new Long3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        /// <summary>
        /// Add two long3 values component wise
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Long3 operator +(Long3 a, Long3 b) {
            return new Long3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

    }

}
