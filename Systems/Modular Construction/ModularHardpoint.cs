using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    public class ModularHardpoint : MonoBehaviour {

        public bool isEmtpy {
            get {
                return transform.childCount < 1;
            }
        }

        public bool isFilled {
            get {
                return !isEmtpy;
            }
        }

        public Transform child {
            get {
                return isFilled ? transform.GetChild(0) : null;
            }
        }

        public GameObject Detach() {
            if (!isFilled)
                return null;

            Transform tr = child.transform;
            tr.SetParent(null);
            return tr.gameObject;
        }

        public void Attach(Transform go, Transform relativeTo = null) {
            go.SetParent(transform);
            go.localPosition = Vector3.zero;
            go.localRotation = Quaternion.identity;

            //https://gamedevelopment.tutsplus.com/tutorials/bake-your-own-3d-dungeons-with-procedural-recipes--gamedev-14360
            //If relative to a different hardpoint, adjust rotation and position appropriately
            if (relativeTo != null) {
                Quaternion rot = Quaternion.FromToRotation(relativeTo.forward, transform.forward);
                Vector3 pos = transform.position - relativeTo.position;

                go.rotation = go.rotation * rot;
                go.localPosition = pos;
            }

        }

        public void MoveToAttachmentPosition(Transform tr, Transform relativeTo = null) {
            tr.transform.position = this.transform.position;
            tr.transform.rotation = this.transform.rotation;

            //If relative to a different hardpoint, adjust rotation and position appropriately
            if (relativeTo != null) {
                Quaternion rot = Quaternion.FromToRotation(relativeTo.forward, transform.forward);
                Vector3 pos = transform.position - relativeTo.position;

                tr.rotation = tr.rotation * rot;
                tr.localPosition = pos;
            }
        }

    }

}