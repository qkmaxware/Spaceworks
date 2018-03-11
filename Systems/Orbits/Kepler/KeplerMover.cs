using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Spaceworks.Position;

namespace Spaceworks.Orbits.Kepler {

    [RequireComponent(typeof(FloatingTransform))]
    public class KeplerMover : MonoBehaviour {
        
        public double parentMass;
        public float timeScale = 1;
        public KeplerOrbitalParameters parameters;

        private KeplerOrbit orbit;
        private FloatingTransform ft;

        void Start() {
            orbit = new KeplerOrbit(new KeplerBody(parentMass, null), parameters);
            ft = this.GetComponent<FloatingTransform>();
            ft.worldPosition = new WorldPosition(orbit.GetCurrentPosition());
            ft.UpdateUnityPosition();
        }

        void Update() {
            orbit.StepTime(Time.deltaTime * timeScale);
            ft.worldPosition = new WorldPosition(orbit.GetCurrentPosition());
            ft.UpdateUnityPosition();
        }

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