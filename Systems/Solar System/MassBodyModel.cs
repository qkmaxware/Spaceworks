using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spaceworks.Orbits.Kepler;
using Spaceworks.Position;

namespace Spaceworks.SystemModel {

    public class MassBodyModel : MonoBehaviour {

        public double mass;
        public KeplerOrbitalParameters orbitalParameters;
        
        public MassBodyModel parent;
        public FloatingTransform modelOf;

        public virtual void Apply(double scaleMultiplier) {
            KeplerOrbitalParameters realKop = orbitalParameters * scaleMultiplier;
            if (parent == null) {
                modelOf.worldPosition = new WorldPosition(Vector3d.zero);
            }
            else {
                KeplerBody body = new KeplerBody(parent.mass, null);
                KeplerOrbit ko = new KeplerOrbit(body, realKop);
                modelOf.worldPosition = new WorldPosition(ko.GetCurrentPosition());
            }
        }

        public void OnDrawGizmos() {
            if (parent == null || orbitalParameters == null)
                return;

            KeplerOrbit orbit = new KeplerOrbit(new KeplerBody(parent.mass, null), orbitalParameters);

            //Draw start position
            Gizmos.color = Color.white;
            float radius = (float)orbitalParameters.semiMajorLength * 0.1f;
            Vector3 pos = orbit.GetCurrentPosition().vector3;
            Gizmos.DrawWireSphere(parent.transform.position + pos, radius);

            //Draw path
            bool first = true;
            Vector3 p = Vector3.zero;
            foreach (Vector3d position in orbit.GetPositions(24)) {
                if (!first) {
                    Vector3 p2 = parent.transform.position + position.vector3;
                    Gizmos.DrawLine(p, p2);
                    p = p2;
                }
                else {
                    first = false;
                    p = parent.transform.position + position.vector3;
                }
            }

            //Draw velocity
            Gizmos.color = Color.yellow;
            Vector3 vel = orbit.GetCurrentVelocity().vector3;
            Gizmos.DrawLine(pos, pos + vel);
        }

    }

}