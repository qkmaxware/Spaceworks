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
			
        public WorldPosition worldPosition {
			get;
			set;
        }

        public void Start() {
            FloatingOrigin.Add(this);
            this.worldPosition = new WorldPosition(this.unityPosition);
        }

		public virtual void OnOriginChange(WorldPosition sceneCenter) {
            unityPosition = (worldPosition - sceneCenter).ToVector3();
        }

    }

}