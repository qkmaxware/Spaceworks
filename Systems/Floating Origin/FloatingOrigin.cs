using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks.Position {

    /// <summary>
    /// Represents a component that manages all objects obeying the rules of floating origin
    /// </summary>
    public class FloatingOrigin : MonoBehaviour {

        /// <summary>
        /// Static instance active in scene
        /// </summary>
        private static FloatingOrigin instance;

        /// <summary>
        /// List of Floating Transforms effected by this manager
        /// </summary>
        /// <returns></returns>
        private List<FloatingTransform> monitored = new List<FloatingTransform>();
		
        /// <summary>
        ///  Offset of the center of the world from the real center
        /// </summary>
        private WorldPosition _center = WorldPosition.zero;

        /// <summary>
        /// Offset of the center of the world from the real center
        /// </summary>
        /// <returns></returns>
		public WorldPosition sceneCenter {
			get{ 
				return _center;
			}
			private set{ 
				_center = value;
			}
		}

        /// <summary>
        /// Offset of the center of the world from the real center
        /// </summary>
        /// <returns></returns>
		public static WorldPosition center {
			get { 
				return instance ? instance.sceneCenter : WorldPosition.zero;
			}
		}

        /// <summary>
        /// Object to keep within high precision when updating the position of floating origin objects
        /// </summary>
        /// <returns></returns>
        public static Transform focus {
            get {
                if (Exists())
                    return instance.foci;
                return null;
            }
            set {
                if (Exists())
                    instance.foci = value;
            }
        }

        /// <summary>
        /// Check that the static manager instance exists
        /// </summary>
        /// <returns></returns>
        public static bool Exists() {
            return instance != null;
        }

        /// <summary>
        /// Make a static manager instance
        /// </summary>
        /// <returns></returns>
        public static FloatingOrigin Make() {
            if (Exists()) {
                return instance;
            }
            else {
                GameObject g = new GameObject("Floating Origin Manager");
                FloatingOrigin org = g.AddComponent<FloatingOrigin>();
                org.Awake();
                return org;
            }
        }

        /// <summary>
        /// Object to keep within high precision
        /// </summary>
        public Transform foci;

        /// <summary>
        /// Distance to use for partitioning space into grids
        /// </summary>
        public float bufferDistance = 1000;

        /// <summary>
        /// Set the grid size and assign static instance
        /// </summary>
        void Awake() {
            //Assign the static instance
            instance = this;
            //Set the default sector size to be the same as the buffer distance
			SetSectorSize(bufferDistance);
        }

        /// <summary>
        /// Set the grid size
        /// </summary>
        /// <param name="distance"></param>
		public void SetSectorSize(float distance){
			WorldPosition.defaultSectorSize = new Vector3(distance, distance, distance);
		}

        /// <summary>
        /// Manage a new object
        /// </summary>
        /// <param name="tr"></param>
        public static void Add(FloatingTransform tr) {
            if (instance)
                instance.AddTransform(tr);
        }

        /// <summary>
        /// Stop managing an object
        /// </summary>
        /// <param name="tr"></param>
        public static void Remove(FloatingTransform tr) {
            if (instance)
                instance.RemoveTransform(tr);
        }

        /// <summary>
        /// Manage a new object
        /// </summary>
        /// <param name="tr"></param>
        public void AddTransform(FloatingTransform tr) {
            monitored.Add(tr);
        }

        /// <summary>
        /// Stop managing an object
        /// </summary>
        /// <param name="tr"></param>
        public void RemoveTransform(FloatingTransform tr) {
            monitored.Remove(tr);
        }

        /// <summary>
        /// If the foci leaves the high precision grid cell, move all floating origin objects to compensate and reset the foci's position
        /// </summary>
        void Update() {
            if (!foci)
                return;
            
            if (Mathf.Abs(foci.transform.position.x) > bufferDistance || Mathf.Abs(foci.transform.position.y) > bufferDistance || Mathf.Abs(foci.transform.position.z) > bufferDistance) {
                WorldPosition delta = new WorldPosition(foci.transform.position).SectorOnly();
                WorldPosition old = this.sceneCenter;
                this.sceneCenter += delta;
                foci.transform.position -= delta.vector3;

                foreach (FloatingTransform tr in monitored) {
                    if(tr != null){
                        tr.OnOriginChange(this.sceneCenter, delta);
                        if(!tr.worldPosition.SameSector(old) && tr.worldPosition.SameSector(this.sceneCenter)){
                            tr.OnOriginEnter(this.sceneCenter);
                        }
                        if(tr.worldPosition.SameSector(old) && !tr.worldPosition.SameSector(this.sceneCenter)){
                            tr.OnOriginExit(this.sceneCenter);
                        }
                    }
                }
            }
        }

    }

}