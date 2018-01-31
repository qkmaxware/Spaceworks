using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Spaceworks.Position;

namespace Spaceworks {

	public class PlanetService : FloatingTransform {

        public PlanetConfig config;

        public Planet generatedPlanet { get; private set; }

        // Use this for initialization
        new void Start() {
			base.Start ();

            //Create the planet
            Planet p = new Planet(config);
            this.generatedPlanet = p;

            //Instanciate the planet using this gameobject as an encapsilating object
            p.RenderOn(this.gameObject);

            //Render the levels of details for the initial camera position (slow)
            p.ForceUpdateLODs(Camera.main.transform.position);

            this.name = this.name + ": " + this.config.name;
        }

        // Update is called once per frame
        void Update() {
            generatedPlanet.UpdateLODs(Camera.main.transform.position);
        }

		public override void OnOriginChange (WorldPosition sceneCenter){
			base.OnOriginChange (sceneCenter);
			if (generatedPlanet != null)
				generatedPlanet.UpdateMaterial (this.gameObject);
		}

    }

}
