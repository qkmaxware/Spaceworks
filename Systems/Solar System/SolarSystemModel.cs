using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spaceworks.Orbits.Kepler;
using Spaceworks.Position;

namespace Spaceworks.SystemModel {

    /// <summary>
    /// Represents a scalled down version of a solar system.
    /// </summary>
    public class SolarSystemModel : MonoBehaviour {

        [Header("Model")]
        [Tooltip("1 Model unit = X Real Units")]
        /// <summary>
        /// Scale of the model. 1 model unit = X real units.
        /// </summary>
        public double scale;

        /// <summary>
        /// Apply all children MassBodyModels
        /// </summary>
        void Start() {
            BuildModel();
        }

        /// <summary>
        /// Apply all children MassBodyModels
        /// </summary>
        public void BuildModel() {
            double multiplier = scale;

            foreach (MassBodyModel model in this.GetComponentsInChildren<MassBodyModel>()) {
                if (!model.modelOf)
                    continue;

                model.Apply(multiplier);
            }

        }

        /// <summary>
        /// Update world position of model components
        /// </summary>
        public void Update() {
            foreach (MassBodyModel model in this.GetComponentsInChildren<MassBodyModel>()) {
                if (!model.modelOf)
                    continue;

                model.modelOf.worldPosition = new WorldPosition(model.generatedOrbit.GetCurrentPosition());
            }
        }
    }
}
