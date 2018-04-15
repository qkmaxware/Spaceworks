using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spaceworks.Orbits.Kepler;
using Spaceworks.Position;

namespace Spaceworks.SystemModel {

    /// <summary>
    /// Body with mass and orbit in a solar system. Represents scalled down version of a real object
    /// </summary>
    public class MassBodyModel : MonoBehaviour {

        public double mass;
        public KeplerOrbitalParameters orbitalParameters;
        public KeplerOrbit generatedScaledOrbit { get; private set; }
        public KeplerOrbit modelOrbit { get; private set; }

        public MassBodyModel parent;
        public FloatingTransform modelOf;

        /// <summary>
        /// Apply this model to the KeplerBody it represents, and scale up the model.
        /// </summary>
        /// <param name="scaleMultiplier"></param>
        public virtual void Apply(double scaleMultiplier) {
            KeplerOrbitalParameters realKop = orbitalParameters * scaleMultiplier;
            if (parent == null) {
                modelOf.worldPosition = new WorldPosition(new Vector3d(this.transform.position) * scaleMultiplier);
            }
            else {
                KeplerBody body = new KeplerBody(parent.mass, null);
                KeplerOrbit ko = new KeplerOrbit(body, realKop);
                generatedScaledOrbit = ko;
                modelOrbit = new KeplerOrbit(body,orbitalParameters);
                modelOf.worldPosition = new WorldPosition(ko.GetCurrentPosition());
            }
        }

        /// <summary>
        /// Editor drawing help
        /// </summary>
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
            Vector3 center = Vector3.zero;

            if (parent.parent) {
                KeplerOrbit parentOrbit = new KeplerOrbit(new KeplerBody(parent.parent.mass, null), parent.orbitalParameters);
                center = parentOrbit.GetCurrentPosition().vector3;
            }
            foreach (Vector3d position in orbit.GetPositions(24)) {
                if (!first) {
                    Vector3 p2 =  center + position.vector3;
                    Gizmos.DrawLine(p, p2);
                    p = p2;
                }
                else {
                    first = false;
                    p = center + position.vector3;
                }
            }

            //Draw velocity
            Gizmos.color = Color.yellow;
            Vector3 vel = orbit.GetCurrentVelocity().vector3.normalized * (float)this.orbitalParameters.semiMajorLength;
            Gizmos.DrawLine(center + pos, center + pos + vel);
        }

    }

}