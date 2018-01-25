using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    public class SimpleOrbit : MonoBehaviour {

        public enum FocalPoint {
            NegativeMajorAxis, PositiveMajorAxis
        }

        [Header("Ellipse Shape")]
        public float semiMajorAxis;
        [Range(0,1)]
        public float eccentricity;
        public Vector3 rotation;

        public float a {
            get {
                return semiMajorAxis;
            }
        }

        public float b {
            get {
                return a * Mathf.Sqrt(1  -eccentricity * eccentricity);
            }
        }

        [Header("Ellipse Positioning")]
        public FocalPoint focusPoint = FocalPoint.NegativeMajorAxis;
        public Transform focusObject;

        [Header("Body Parameters")]
        [Range(0, 2 * Mathf.PI)]
        public float startPosition;
        public float orbitalVelocity;

        //Internal values
        private float _position;
        private Vector3 savedRotation;
        private Quaternion _rot = Quaternion.identity;

        public Vector3 focusNegative {
            get {
                if (focusPoint == FocalPoint.NegativeMajorAxis)
                    return focusObject.position;

                Vector3 dirToCenterFromFoci = _rot * ((focusPoint == FocalPoint.NegativeMajorAxis) ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0));
                float distToCenter = Mathf.Sqrt(a * a - b * b);

                return focusObject.position + dirToCenterFromFoci * 2 * distToCenter;
            }
        }

        public Vector3 focusPositive {
            get {
                if (focusPoint == FocalPoint.PositiveMajorAxis)
                    return focusObject.position;

                Vector3 dirToCenterFromFoci = _rot * ((focusPoint == FocalPoint.NegativeMajorAxis) ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0));
                float distToCenter = Mathf.Sqrt(a * a - b * b);

                return focusObject.position + dirToCenterFromFoci * 2 * distToCenter;
            }
        }

        public Vector3 center {
            get {
                Vector3 dirToCenterFromFoci = _rot * ((focusPoint == FocalPoint.NegativeMajorAxis) ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0));
                float distToCenter = Mathf.Sqrt(a * a - b * b);

                return focusObject.position + dirToCenterFromFoci * distToCenter;
            }
        }

        void Start() {
            _position = this.startPosition;
            this.transform.position = Evaluate(_position);
            _position = Mathf.Repeat(_position, 2 * Mathf.PI);
        }

        void Update() {
            _position += this.orbitalVelocity * Time.deltaTime;
            this.transform.position = Evaluate(_position);
        }

        public Vector3 Evaluate(float f) {
            f = Mathf.Repeat(f, 2 * Mathf.PI);

            //Cache rotation
            if (savedRotation != rotation) {
                savedRotation = rotation;
                _rot = Quaternion.Euler(savedRotation);
            }

            Vector3 location = new Vector3(
                a * Mathf.Cos(f),
                0,
                b * Mathf.Sin(f)
            );

            float distToCenter = Mathf.Sqrt(a * a - b * b);

            Vector3 dirToCenterFromFoci = _rot * ((focusPoint == FocalPoint.NegativeMajorAxis) ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0));

            Vector3 worldPos = (focusObject.position + distToCenter * dirToCenterFromFoci) + _rot * location;

            return worldPos;
        }

        public Vector3[] GetPoints(int steps) {
            Vector3[] points = new Vector3[steps];

            for (int i = 0; i < steps; i++) {
                points[i] = Evaluate(i * 2 * Mathf.PI / (steps - 1));
            }

            return points;
        }

        public void OnDrawGizmos() {
            Gizmos.color = Color.white;

            Vector3[] points = GetPoints(24);
            for (int i = 0; i < points.Length; i++) {
                int start = i;
                int end = i == points.Length - 1 ? 0 : i + 1;

                Gizmos.DrawLine(points[start], points[end]);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(focusObject.position, Evaluate(0));

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Evaluate(startPosition), 0.3f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(focusNegative, 0.2f);
            Gizmos.DrawWireSphere(focusPositive, 0.2f);
            Gizmos.DrawWireSphere(center, 0.2f);
        }

    }
}