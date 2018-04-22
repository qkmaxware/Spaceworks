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
        /// Scale of the model. 1 model unit = X real units. 1Au = 149600000 scale
        /// </summary>
        public double scale;

        /// <summary>
        /// Animate orbits of model
        /// </summary>
        public bool animate = false;

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
            if(!animate)
                return;
            double timestep = Time.deltaTime;
            foreach (MassBodyModel model in this.GetComponentsInChildren<MassBodyModel>()) {
                //Nothing generated, skip
                if(model.generatedScaledOrbit == null)
                    continue;

                //Step the orbit in time
                model.generatedScaledOrbit.StepTime(timestep);
                
                //If there is no real world element to this model, skip it
                if(!model.modelOf)
                    continue;

                //Set the position of the real object that this is the model of
                Vector3d pos =  model.generatedScaledOrbit.GetCurrentPosition();
                model.modelOf.worldPosition = new WorldPosition(pos);
                model.modelOf.UpdateUnityPosition();
            }
        }
    }
}
