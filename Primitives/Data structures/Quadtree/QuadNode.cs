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
        public QuadNode(Zone3 area) {
            this.range = area;
        }

        /// <summary>
        /// Create node covering area with value
        /// </summary>
        /// <param name="area"></param>
        /// <param name="value"></param>
        public QuadNode(Zone3 area, T value) {
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
        public Zone3 range {
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
            Zone3 NW; Zone3 NE; Zone3 SW; Zone3 SE;

            this.range.QuadSubdivide(out NE, out NW, out SE, out SW);

            //Create 4 children, one for each quadrant
            this[Quadrant.NorthWest] = new QuadNode<T>(NW);
            this[Quadrant.NorthEast] = new QuadNode<T>(NE);
            this[Quadrant.SouthWest] = new QuadNode<T>(SW);
            this[Quadrant.SouthEast] = new QuadNode<T>(SE);
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
