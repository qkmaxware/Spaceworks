using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks.Position {

    /// <summary>
    /// Represents the position of an object obeying floating origin rules
    /// </summary>
    public class FloatingTransform : MonoBehaviour {

        /// <summary>
        /// Rotation
        /// </summary>
        /// <returns></returns>
        public Quaternion rotation {
            get {
                return transform.rotation;
            }
            set {
                transform.rotation = value;
            }
        }

        /// <summary>
        /// Position within the scene
        /// </summary>
        /// <returns></returns>
        public Vector3 unityPosition {
            get {
                return transform.position;

            }
            set {
                transform.position = value;
            }
        }

        /// <summary>
        /// Floating transform this one's position is relative to
        /// </summary>
        /// <returns></returns>
        public FloatingTransform parent {
            get;
            set;
        }

        private WorldPosition _local = WorldPosition.zero;
        /// <summary>
        /// Position in the world (not the scene) ignoring parent transform
        /// </summary>
        public WorldPosition localPosition {
            get {
                return _local;
            }
            set {
                _local = value;
            }
        }

        /// <summary>
        /// Position in the world (not the scene) taking into account parent position
        /// </summary>
        public WorldPosition worldPosition {
            get {
                return (parent == null) ? this.localPosition : this.localPosition + parent.worldPosition;
            }
            set {
                if (parent == null)
                    this.localPosition = value;
                else
                    this.localPosition = value - parent.worldPosition;
            }
        }

        /// <summary>
        /// Flag to disable colliders when moved
        /// </summary>
        public bool autoDisableColliders = true;

        /// <summary>
        /// List of colliders to disable
        /// </summary>
        private Collider[] monitoredColliders;

        /// <summary>
        /// Register transform with the manager and get list if attached colliders
        /// </summary>
        public void Start() {
            FloatingOrigin.Make(); //Ensure existance of floating origin manager
            FloatingOrigin.Add(this);
            this.worldPosition = new WorldPosition(this.unityPosition);
            UpdateColliderList();
        }

        /// <summary>
        /// Update the local list of attached colliders
        /// </summary>
        public void UpdateColliderList() {
            this.monitoredColliders = this.GetComponentsInChildren<Collider>();
        }

        /// <summary>
        /// Set a floating transform as the parent of this one. This effects position only
        /// </summary>
        /// <param name="parent"></param>
		public void SetParent(FloatingTransform parent) {
            this.parent = parent;
            UpdateUnityPosition();
        }

        /// <summary>
        /// Set the position of this transform with respect the the parent.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="center"></param>
		public void SetLocalPosition(WorldPosition p, WorldPosition center = null) {
            this.localPosition = p;
            if (center != null) {
                UpdateUnityPosition(center);
            }
            else {
                UpdateUnityPosition();
            }
        }

        /// <summary>
        /// Set the position of this transform with respect to the world
        /// </summary>
        /// <param name="p"></param>
        /// <param name="center"></param>
		public void SetWorldPosition(WorldPosition p, WorldPosition center = null) {
            this.worldPosition = p;
            if (center != null) {
                UpdateUnityPosition(center);
            }
            else {
                UpdateUnityPosition();
            }
        }

        /// <summary>
        /// Force update the Vector3 world position of this object by automatically aquiring the FloatingOrigin's center
        /// </summary>
		public void UpdateUnityPosition() {
            UpdateUnityPosition(FloatingOrigin.center);
        }

        /// <summary>
        /// Disable all active colliders from the local list (updated by UpdateColliderList)
        /// </summary>
        /// <returns></returns>
        public virtual List<Collider> DisableColliders() {
            List<Collider> touchedColliders = new List<Collider>();

            if(this.monitoredColliders != null)
            foreach (Collider c in this.monitoredColliders) {
                if (c.enabled) {
                    touchedColliders.Add(c);
                    c.enabled = false;
                }
            }

            return touchedColliders;
        }

        /// <summary>
        /// Enable all colliders in a list
        /// </summary>
        /// <param name="disabled"></param>
        public virtual void EnableColliders(List<Collider> disabled) {
            foreach (Collider c in disabled) {
                c.enabled = true;
            }
        }

        /// <summary>
        /// Update the Vector3 world position of this object from a given FloatingOrigin center offset
        /// </summary>
        /// <param name="sceneCenter"></param>
		public void UpdateUnityPosition(WorldPosition sceneCenter) {
            //disable colliders 
            List<Collider> touchedColliders = null;
            if (autoDisableColliders) {
                touchedColliders = DisableColliders();
            }

            unityPosition = (worldPosition - sceneCenter).vector3;

            //enable colliders
            if (autoDisableColliders && touchedColliders != null) {
                EnableColliders(touchedColliders);
            }
        }

        /// <summary>
        /// Called automatically to update positional data when the offset of the FloatingOrigin changes
        /// </summary>
        /// <param name="sceneCenter"></param>
		public virtual void OnOriginChange(WorldPosition sceneCenter) {
            UpdateUnityPosition(sceneCenter);
        }

        public override string ToString() {
            return this.name + " - " + this.worldPosition.ToString();
        }
    }

}