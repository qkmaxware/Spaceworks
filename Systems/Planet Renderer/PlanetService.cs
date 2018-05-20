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

        public override List<Collider> DisableColliders() {
            List<Collider> colliders = new List<Collider>();

            if (this.generatedPlanet != null) {
                System.Action<MeshCollider> fn = (x) => {
                    if (x.enabled) {
                        colliders.Add(x);
                        x.enabled = false;
                    }
                };

                this.generatedPlanet.topFace.ForEachActiveCollider(fn);
                this.generatedPlanet.bottomFace.ForEachActiveCollider(fn);
                this.generatedPlanet.leftFace.ForEachActiveCollider(fn);
                this.generatedPlanet.rightFace.ForEachActiveCollider(fn);
                this.generatedPlanet.frontFace.ForEachActiveCollider(fn);
                this.generatedPlanet.backFace.ForEachActiveCollider(fn);
            }

            return colliders;
        }

        public override void OnOriginChange (WorldPosition sceneCenter, WorldPosition delta){
			base.OnOriginChange (sceneCenter, delta);
			if (generatedPlanet != null)
				generatedPlanet.UpdateMaterial (this.gameObject);
		}

    }

}
