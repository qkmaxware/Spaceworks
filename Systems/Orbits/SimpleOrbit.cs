using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Spaceworks.Position;

namespace Spaceworks.Orbits {

    public class SimpleOrbit : FloatingTransform {

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

        private float unitB {
            get {
                return Mathf.Sqrt(1 - eccentricity * eccentricity);
            }
        }

        [Header("Ellipse Positioning")]
        public FocalPoint focusPoint = FocalPoint.NegativeMajorAxis;
        public FloatingTransform floatingFocusObject;
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

        new void Start() {
            base.Start();
            _position = this.startPosition;
            Move(0);
        }

        void Update() {
            Move(this.orbitalVelocity * Time.deltaTime);
        }

        void Move(float delta) {
            _position = Mathf.Repeat(_position + delta, 2 * Mathf.PI);
            if (floatingFocusObject) {
                this.SetWorldPosition(EvaluateFloating(_position));
            }
            else {
                this.transform.position = Evaluate(_position);
            }
        }

        /// <summary>
        /// Evaluate in higher precision for use with floating origin solution
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public WorldPosition EvaluateFloating(float f) {
            f = Mathf.Repeat(f, 2 * Mathf.PI);

            //Cache rotation
            if (savedRotation != rotation) {
                savedRotation = rotation;
                _rot = Quaternion.Euler(savedRotation);
            }

            //Get location on "unit sphere"
            Vector3 location = new Vector3(
                Mathf.Cos(f),
                0,
                unitB * Mathf.Sin(f)
            );

            //Get location in double precision
            double realX = a * System.Math.Cos(f);
            double realZ = b * System.Math.Sin(f);
            double distance = System.Math.Sqrt(realX * realX + realZ * realZ);

            //Compute distance to center of ellipse in double precision
            double centerDist = System.Math.Sqrt(a * a - b * b);

            //Create the direction to the point in space
            Vector3 dir = _rot * ((focusPoint == FocalPoint.NegativeMajorAxis) ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0));
            Vector3 localUnitPos = (_rot * location);

            //Create the world position by scaling up and moving the local precision values
            WorldPosition localPos = new WorldPosition(localUnitPos.x * distance, localUnitPos.y * distance, localUnitPos.z * distance);
            WorldPosition worldPos = (floatingFocusObject.worldPosition + new WorldPosition(centerDist * dir.x, centerDist * dir.y, centerDist * dir.z)) + localPos;

            return worldPos;
        }

        /// <summary>
        /// Evaluate in UNITY standard coordinate system
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
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
            if (focusObject == null)
                return;

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
            float radius = a * 0.1f;
            Gizmos.DrawWireSphere(Evaluate(startPosition), radius);

            Gizmos.color = Color.red;
            radius = a * 0.05f;
            Gizmos.DrawWireSphere(focusNegative, radius);
            Gizmos.DrawWireSphere(focusPositive, radius);
            Gizmos.DrawWireSphere(center, radius);
        }

    }
}