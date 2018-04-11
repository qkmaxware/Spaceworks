using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks.SystemModel {

    /// <summary>
    /// Special type of MassBodyModel specific to planets
    /// </summary>
    public class PlanetModel : MassBodyModel {

        public override void Apply(double scaleMultiplier) {
            base.Apply(scaleMultiplier);
            
        }

    }

}