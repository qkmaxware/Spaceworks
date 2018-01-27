using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    public class FloatingTransform : MonoBehaviour {

        public Quaternion rotation {
            get {
                return transform.rotation;
            }
            set {
                transform.rotation = value;
            }
        }

        public Vector3 unityPosition {
            get {
                return transform.position;
    
            }   
            set {
                transform.position = value;
            }
        }
			
		public FloatingTransform parent {
			get;
			set;
		}

        private WorldPosition _local = WorldPosition.zero;
		public WorldPosition localPosition {
            get {
                return _local;
            }
            set {
                _local = value;
            }
		}

		public WorldPosition worldPosition {
			get{ 
				return (parent == null) ? this.localPosition : this.localPosition + parent.worldPosition;
			}
			set{ 
				if (parent == null)
					this.localPosition = value;
				else
					this.localPosition = value - parent.worldPosition;
			}
		}

        public void Start() {
            FloatingOrigin.Add(this);
            this.worldPosition = new WorldPosition(this.unityPosition);
        }

		public void SetParent(FloatingTransform parent){
			this.parent = parent;
			UpdateUnityPosition ();
		}

		public void SetLocalPosition(WorldPosition p){
			this.localPosition = p;
			UpdateUnityPosition ();
		}

		public void SetWorldPosition(WorldPosition p){
			this.worldPosition = p;
			UpdateUnityPosition ();
		}

		public void UpdateUnityPosition(){
			unityPosition = (worldPosition - FloatingOrigin.center).ToVector3();
		}

		public void UpdateUnityPosition(WorldPosition sceneCenter){
			unityPosition = (worldPosition - sceneCenter).ToVector3();
		}

		public virtual void OnOriginChange(WorldPosition sceneCenter) {
			UpdateUnityPosition (sceneCenter);
        }

    }

}