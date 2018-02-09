using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks.Position {

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

        public bool autoDisableColliders = true;

        private Collider[] monitoredColliders;

        public void Start() {
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

            unityPosition = (worldPosition - sceneCenter).ToVector3();

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