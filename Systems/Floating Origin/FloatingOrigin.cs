using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    public class FloatingOrigin : MonoBehaviour {

        private static FloatingOrigin instance;

        private List<FloatingTransform> monitored = new List<FloatingTransform>();
		private WorldPosition _center = WorldPosition.zero;
		public WorldPosition sceneCenter {
			get{ 
				return _center;
			}
			private set{ 
				_center = value;
			}
		}

		public static WorldPosition center {
			get { 
				return instance ? instance.sceneCenter : WorldPosition.zero;
			}
		}

        public Transform foci;
        public float bufferDistance = 1000;

        void Awake() {
            //Assign the static instance
            instance = this;
            //Set the default sector size to be the same as the buffer distance
			SetSectorSize(bufferDistance);
        }

		public void SetSectorSize(float distance){
			WorldPosition.defaultSectorSize = new Vector3(distance, distance, distance);
		}

        public static void Add(FloatingTransform tr) {
            if (instance)
                instance.AddTransform(tr);
        }

        public static void Remove(FloatingTransform tr) {
            if (instance)
                instance.RemoveTransform(tr);
        }

        public void AddTransform(FloatingTransform tr) {
            monitored.Add(tr);
        }

        public void RemoveTransform(FloatingTransform tr) {
            monitored.Remove(tr);
        }

        void Update() {
            if (!foci)
                return;

            if (foci.transform.position.sqrMagnitude > bufferDistance * bufferDistance) {
                WorldPosition delta = new WorldPosition(foci.transform.position).SectorOnly();
                sceneCenter += delta;
                foci.transform.position -= delta.ToVector3();

                foreach (FloatingTransform tr in monitored) {
                    tr.OnOriginChange(sceneCenter);
                }
            }

        }

    }

}