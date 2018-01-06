using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

	[System.Serializable]
    public class SolarSystemConfig {
		[System.Serializable]
		public class SolarSystemPlanetConfig {
			public PlanetConfig planet;
			public float distanceFromSunAU;
		}

		public string name;
		public SolarSystemPlanetConfig[] planets;
    }

    public class SolarSystem {

		private SolarSystemConfig config;

		public SolarSystem(SolarSystemConfig config){
			this.config = config;
		}

		public Planet[] Render(){
			GameObject fpMgr = new GameObject ("Floating Origin Manager");
			FloatingOrigin fo = fpMgr.AddComponent<FloatingOrigin> ();

			Planet[] planets = new Planet[config.planets.Length];
			int i = 0;

			foreach (SolarSystemConfig.SolarSystemPlanetConfig conf in config.planets) {
				//Create gameobject
				Planet planet = new Planet (conf.planet);
				GameObject go = new GameObject (conf.planet.name);
				planet.RenderOn (go);

				//Add floating origin code to position object
				FloatingTransform ft = go.AddComponent<FloatingTransform> ();
				ft.worldPosition = new WorldPosition (new Vector3 (), new Long3 ());
				ft.OnOriginChange (fo.sceneCenter);

				//Update planet LODS
				planet.ForceUpdateLODs (Camera.main.transform.position);

				//Add to array
				planets[i++] = planet;
			}

			return planets;
		}

    }

}
