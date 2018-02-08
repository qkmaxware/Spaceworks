using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    [System.Serializable]
    public class Long3 {
        public long x;
        public long y;
        public long z;

        public Long3() { }
        public Long3(Long3 other) {
            this.x = other.x;
            this.y = other.y;
            this.z = other.z;
        }
        public Long3(long x, long y, long z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override bool Equals(System.Object obj) {
            if (obj is Long3 == false)
                return false;
            Long3 r = (Long3)obj;
            return this.x == r.x && this.y == r.y && this.z == r.z;
        }

        public override int GetHashCode() {
            return (int)(x ^ y ^ z);
        }

        public static Long3 operator -(Long3 a, Long3 b) {
            return new Long3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Long3 operator +(Long3 a, Long3 b) {
            return new Long3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

    }

}
