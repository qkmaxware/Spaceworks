using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spaceworks.Orbits.Kepler;

namespace Spaceworks.SystemModel {

    public class SolarSystemModel : MonoBehaviour {

        [Header("Model")]
        [Range(1, 100)]
        public double percentScale = 50;

        public void BuildModel() {
            double multiplier = 100 / percentScale;

            foreach (MassBodyModel model in this.GetComponentsInChildren<MassBodyModel>()) {
                KeplerOrbitalParameters realKop = model.orbitalParameters * multiplier;
                if (!model.modelOf)
                    continue;

                model.Apply(multiplier);
            }

        }

    }
}
