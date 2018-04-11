using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    /// <summary>
    /// Represents a node of a Quadtree
    /// </summary>
    public class QuadNode<T> {

        /// <summary>
        /// Value stored in node
        /// </summary>
        public T value;
        
        /// <summary>
        /// Children references
        /// </summary>
        private QuadNode<T>[] tree = null;

        /// <summary>
        /// Create node covering area
        /// </summary>
        /// <param name="area"></param>
        public QuadNode(Rectangle3 area) {
            this.range = area;
        }

        /// <summary>
        /// Create node covering area with value
        /// </summary>
        /// <param name="area"></param>
        /// <param name="value"></param>
        public QuadNode(Rectangle3 area, T value) {
            this.range = area;
            this.value = value;
        }

        /// <summary>
        /// Depth of node in tree
        /// </summary>
        /// <returns></returns>
        public int depth {
            get; private set;
        }

        /// <summary>
        /// Parent node reference
        /// </summary>
        /// <returns></returns>
        public QuadNode<T> parent {
            get; private set;
        }

        /// <summary>
        /// 2D area this node vocers
        /// </summary>
        /// <returns></returns>
        public Rectangle3 range {
            get; private set;
        }

        /// <summary>
        /// Index children nodes by quadrant
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Test if node has no children
        /// </summary>
        /// <returns></returns>
        public bool isLeaf {
            get {
                return tree == null;
            }
        }

        /// <summary>
        /// Test if node has any children
        /// </summary>
        /// <returns></returns>
        public bool isBranch {
            get {
                return !isLeaf;
            }
        }

        /// <summary>
        /// Test if node has no parent
        /// </summary>
        /// <returns></returns>
        public bool isRoot {
            get {
                return this.parent == null;
            }
        }

        /// <summary>
        /// Subdivide this node into 4 children
        /// </summary>
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
            this[Quadrant.NorthWest] = new QuadNode<T>(new Rectangle3(topl, tc, mc, lm));
            this[Quadrant.NorthEast] = new QuadNode<T>(new Rectangle3(tc, topr, rm, mc));
            this[Quadrant.SouthWest] = new QuadNode<T>(new Rectangle3(lm, mc, bc, btnl));
            this[Quadrant.SouthEast] = new QuadNode<T>(new Rectangle3(mc, rm, btnr, bc));
        }

        /// <summary>
        /// Remove all children from this node
        /// </summary>
        public void Trim() {
            if (isLeaf)
                return;

            foreach (QuadNode<T> node in tree)
                node.parent = null;
            this.tree = null;
        }

    }

}
