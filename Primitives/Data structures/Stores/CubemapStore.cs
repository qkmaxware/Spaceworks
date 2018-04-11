using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    /// <summary>
    /// Stores 6 textures in a cubemap configuration along with inversion settings
    /// </summary>
    [System.Serializable]
    public class CubemapStore {
        [System.Serializable]
        public class InvertSettings {
            public bool invertX = false;
            public bool invertY = false;
        }

        public Texture2D top;
        public InvertSettings invertTop;
        public Texture2D bottom;
        public InvertSettings invertBottom;
        public Texture2D left;
        public InvertSettings invertLeft;
        public Texture2D right;
        public InvertSettings invertRight;
        public Texture2D front;
        public InvertSettings invertFront;
        public Texture2D back;
        public InvertSettings invertBack;
    }
}
