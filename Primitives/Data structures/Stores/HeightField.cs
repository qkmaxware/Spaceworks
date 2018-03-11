using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    public enum ColourComponent {
        Red, Green, Blue, Alpha, Luminosity
    }

    public class HeightField {

        private float[] heightmap;

        public int width {
            get; private set;
        }

        public int height {
            get; private set;
        }

        public float this[int x, int y] {
            get {
                return heightmap[x + width * y];
            }
            set {
                heightmap[x + width * y] = value;
            }
        }

        public HeightField(int width, int height) {
            this.width = Mathf.Max(1, width);
            this.height = Mathf.Max(1, height);

            heightmap = new float[this.width * this.height];
        }

        public HeightField(Texture2D texture, ColourComponent component = ColourComponent.Luminosity, bool invertX = false, bool invertY = false) {
            this.width = texture.width;
            this.height = texture.height;

            this.heightmap = new float[this.width * this.height];

            for (int w = 0; w < texture.width; w++) {
                for (int h = 0; h < texture.height; h++) {
                    this[w, h] = SampleColour(component, texture.GetPixel(
                        invertX ? ((texture.width - 1) - w) : w, 
                        invertY ? ((texture.height - 1) - h) : h
                    ));
                }
            }

        }

        /// <summary>
        /// Sampel a height from a colour
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        private static float SampleColour(ColourComponent heightColour, Color colour) {
            switch (heightColour) {
                case ColourComponent.Alpha:
                    return colour.a;
                case ColourComponent.Blue:
                    return colour.b;
                case ColourComponent.Green:
                    return colour.g;
                case ColourComponent.Luminosity:
                    return 0.21f * colour.r + 0.72f * colour.g + 0.07f * colour.b;
                default:
                    return colour.r;
            }
        }

    }

}