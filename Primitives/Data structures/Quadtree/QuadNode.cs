using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    public class QuadNode<T> {

        public T value;
        private QuadNode<T>[] tree = null;

        public QuadNode(Range3d area) {
            this.range = area;
        }

        public QuadNode(Range3d area, T value) {
            this.range = area;
            this.value = value;
        }

        public int depth {
            get; private set;
        }

        public QuadNode<T> parent {
            get; private set;
        }

        public Range3d range {
            get; private set;
        }

        public QuadNode<T> this[Quadrant q] {
            get {
                if (tree == null)
                    return null;
                return tree[(int)q];
            }
            set {
                if (tree == null)
                    tree = new QuadNode<T>[4];
                tree[(int)q] = value;
                value.parent = this;
                value.depth = this.depth + 1;
            }
        }

        public bool isLeaf {
            get {
                return tree == null;
            }
        }

        public bool isBranch {
            get {
                return !isLeaf;
            }
        }

        public bool isRoot {
            get {
                return this.parent == null;
            }
        }

        public void Subdivide() {
            //Create 4 subdivided ranges
            Vector3 topl = this.range.a;
            Vector3 topr = this.range.b;
            Vector3 btnl = this.range.d;
            Vector3 btnr = this.range.c;

            Vector3 tc = Vector3.Lerp(topl, topr, 0.5f);
            Vector3 lm = Vector3.Lerp(topl, btnl, 0.5f);
            Vector3 rm = Vector3.Lerp(topr, btnr, 0.5f);
            Vector3 mc = Vector3.Lerp(lm, rm, 0.5f);
            Vector3 bc = Vector3.Lerp(btnl, btnr, 0.5f);

            //Create 4 children, one for each quadrant
            this[Quadrant.NorthWest] = new QuadNode<T>(new Range3d(topl, tc, mc, lm));
            this[Quadrant.NorthEast] = new QuadNode<T>(new Range3d(tc, topr, rm, mc));
            this[Quadrant.SouthWest] = new QuadNode<T>(new Range3d(lm, mc, bc, btnl));
            this[Quadrant.SouthEast] = new QuadNode<T>(new Range3d(mc, rm, btnr, bc));
        }

        public void Trim() {
            if (isLeaf)
                return;

            foreach (QuadNode<T> node in tree)
                node.parent = null;
            this.tree = null;
        }

    }

}
