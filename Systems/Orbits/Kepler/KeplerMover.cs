using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Spaceworks.Position;

namespace Spaceworks.Orbits.Kepler {

    /// <summary>
    /// Moves an object on an keplerian orbital path
    /// </summary>
    [RequireComponent(typeof(FloatingTransform))]
    public class KeplerMover : MonoBehaviour {
        
        /// <summary>
        /// Mass of orbiting object
        /// </summary>
        public double parentMass;
        /// <summary>
        /// Timescale
        /// </summary>
        public float timeScale = 1;
        /// <summary>
        /// Orbital parameters
        /// </summary>
        public KeplerOrbitalParameters parameters;

        private KeplerOrbit orbit;
        private FloatingTransform ft;

        /// <summary>
        /// Create and start orbit
        /// </summary>
        void Start() {
            orbit = new KeplerOrbit(new KeplerBody(parentMass, null), parameters);
            ft = this.GetComponent<FloatingTransform>();
            ft.worldPosition = new WorldPosition(orbit.GetCurrentPosition());
            ft.UpdateUnityPosition();
        }

        /// <summary>
        /// Update orbital position
        /// </summary>
        void Update() {
            orbit.StepTime(Time.deltaTime * timeScale);
            ft.worldPosition = new WorldPosition(orbit.GetCurrentPosition());
            ft.UpdateUnityPosition();
        }

        /// <summary>
        /// Helper Gizmos
        /// </summary>
        void OnDrawGizmos() {
            KeplerOrbit orbit = new KeplerOrbit(new KeplerBody(parentMass, null), parameters);

            //Draw start position
            Gizmos.color = Color.white;
            float radius = (float)parameters.semiMajorLength * 0.1f;
            Vector3 pos = orbit.GetCurrentPosition().vector3;
            Gizmos.DrawWireSphere(pos, radius);

            //Draw path
            bool first = true;
            Vector3 p = Vector3.zero;
            foreach(Vector3d position in orbit.GetPositions(24)) {
                if (!first) {
                    Vector3 p2 = position.vector3;
                    Gizmos.DrawLine(p, p2);
                    p = p2;
                }
                else {
                    first = false;
                    p = position.vector3;
                }
            }

            //Draw velocity
            Gizmos.color = Color.yellow;
            Vector3 vel = orbit.GetCurrentVelocity().vector3;
            Gizmos.DrawLine(pos, pos + vel);
        }

    }
}