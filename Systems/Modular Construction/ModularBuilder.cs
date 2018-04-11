using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks.Modular {

    /// <summary>
    /// Manage UI for snapping objects together
    /// </summary>
    public class ModularBuilder : MonoBehaviour {

        /// <summary>
        /// Object to place
        /// </summary>
        public ModularComponent objectInHand;
        /// <summary>
        /// Outline of object to place
        /// </summary>
        public GameObject objectOutline;
        /// <summary>
        /// Distance from camera to place outline if no anchor snap is determined
        /// </summary>
        public float outlineDistance = 2;

        /// <summary>
        /// Move object around scene and attach to anchor points
        /// </summary>
        void Update() {
            //Not holding anything
            if (!objectInHand) {
                if(objectOutline)
                    objectOutline.SetActive(false);
                //Try to pickup object
                TryPickup();
            } else {
                if (objectOutline)
                    objectOutline.SetActive(true);
                //Try to place object
                TryPlace();
            }
        }

        private void TryPickup() {

        }

        /// <summary>
        /// Try to place object on anchor
        /// </summary>
        private void TryPlace() {
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

            //Move to temporary position
            if (objectOutline)
                objectOutline.transform.position = ray.GetPoint(outlineDistance); //Get point 2 units down the ray

            //See if something to snap to
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                ModularHardpoint p = hit.transform.gameObject.GetComponent<ModularHardpoint>();
                if (p == null)
                    return;

                if (p.isEmtpy) {
                    //Preview
                    p.MoveToAttachmentPosition(objectOutline.transform);

                    //Place
                    if (Input.GetKeyDown(KeyCode.Mouse0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
						p.Attach(objectInHand.transform, objectInHand.connectionPoint);
                        objectInHand = null;
                    }
                }
            }
        }

    }
}