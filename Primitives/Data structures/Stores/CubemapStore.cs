using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {
    [System.Serializable]
    public class CubemapStore {
        public Texture2D top;
        public Texture2D bottom;
        public Texture2D left;
        public Texture2D right;
        public Texture2D front;
        public Texture2D back;
    }
}
