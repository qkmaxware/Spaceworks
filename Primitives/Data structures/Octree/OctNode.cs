namespace Spaceworks {
    
    /// <summary>
    /// Node of an Octree
    /// </summary>
    public class OctNode<T> {
        /// <summary>
        /// Children nodes
        /// </summary>
        private OctNode<T>[] children = new OctNode<T>[8];
        /// <summary>
        /// Value stored in node
        /// </summary>
        public T value;
        /// <summary>
        /// Depth of the node
        /// </summary>
        /// <returns></returns>
        public int depth {get; private set;}
        /// <summary>
        /// Parent of this node
        /// </summary>
        /// <returns></returns>
        public OctNode<T> parent {get; private set;}
        /// <summary>
        /// Volume encompased by this node
        /// </summary>
        /// <returns></returns>
        public Box3 volume {get; private set;}

        /// <summary>
        /// Create root node covering a volume
        /// </summary>
        /// <param name="volume"></param>
        public OctNode(Box3 volume){
            this.volume = volume;
        }

        /// <summary>
        /// Create a root node convering a volume with a value
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="value"></param>
        public OctNode(Box3 volume, T value){
            this.volume = volume;
            this.value = value;
        }

        /// <summary>
        /// Get child node by quadrant
        /// </summary>
        /// <returns></returns>
        public OctNode<T> this[Octant oc]{
            get {
                return this[(int)oc];
            }
            set{
                this[(int)oc] = value;
            }
        }

        /// <summary>
        /// Get child node by index
        /// </summary>
        /// <returns></returns>
        public OctNode<T> this[int idx]{
            get {
                return children[idx];
            }
            set{
                value.parent = this;
                value.depth = this.depth + 1;
                //Disconnect child node
                if(children[idx] != null){
                    children[idx].parent = null;
                    children[idx].depth = 0;
                }
                //Connect new node
                children[idx] = value;
            }
        }

        /// <summary>
        /// Test if node has no children
        /// </summary>
        /// <returns></returns>
        public bool isLeaf {
            get{
                for(int i = 0; i < this.children.Length; i++)
                    if(children[i] != null)
                        return false;
                return true;
            }
        }

        /// <summary>
        /// Check if node does have children
        /// </summary>
        /// <returns></returns>
        public bool isBranch {
            get {
                return !isLeaf;
            }
        }

        /// <summary>
        /// Check if node has no parent node
        /// </summary>
        /// <returns></returns>
        public bool isRoot {
            get {
                return this.parent == null;
            }
        }

        /// <summary>
        /// Remove all children nodes
        /// </summary>
        public void Trim(){
            for(int i = 0; i < this.children.Length; i++){
                this[i] = null;
            }
        }

        /// <summary>
        /// Subdivide this node into 8 children
        /// </summary>
        public void Subdivide(){

        }

    }

}