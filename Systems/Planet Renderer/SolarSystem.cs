using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Spaceworks.Orbits;
using Spaceworks.Position;

namespace Spaceworks {

    [System.Serializable]
    public class SolarSystemConfig {

        [System.Serializable]
        public class SolarSystemPlanetConfig {
            
            public SolarSystemConfigOrbitalParameters orbit;
            public PlanetConfig planetConfig;
        }

        [System.Serializable]
        public class SolarSystemConfigOrbitalParameters {
            [Header("Ellipse Shape")]
            public float semiMajorAxis;
            [Range(0, 1)]
            public float eccentricity;
            public Vector3 rotation;

            [Range(0, 2 * Mathf.PI)]
            public float startPosition;
        }

        [Header("General")]
        public bool generateSystemName = true;
        public string name;

        [Header("Sun")]
        public GameObject sunObject;

        [Header("Planets")]
        public bool generatePlanetNames = true;
        public SolarSystemPlanetConfig[] planets;

    }

    public class SolarSystem {

		private SolarSystemConfig config;

		public SolarSystem(SolarSystemConfig config){
			this.config = config;

            PhenomicNameGenerator gen = new PhenomicNameGenerator();
            if (config.generateSystemName) {
                config.name = gen.Generate(4, 12);
            }

            if (config.generatePlanetNames) {
                int i = 0;
                foreach (SolarSystemConfig.SolarSystemPlanetConfig conf in config.planets) {
                    conf.planetConfig.name = config.name + "-" + (i++);
                }
            }
		}

		public Planet[] RenderOn(GameObject parent){
            //Set sun as child
            FloatingTransform sunFloat;
            if (config.sunObject != parent) {
                config.sunObject.transform.SetParent(parent.transform);
                config.sunObject.transform.localPosition = Vector3.zero;
            }
            sunFloat = config.sunObject.GetComponent<FloatingTransform>();
            if (sunFloat != null) {
                sunFloat.worldPosition = WorldPosition.zero;
            }

            List<Planet> generatedAstralBodies = new List<Planet>();

            //Deal with children
            for (int i = 0; i < this.config.planets.Length; i++) {
                //Get references
                PlanetConfig conf = this.config.planets[i].planetConfig;
                SolarSystemConfig.SolarSystemConfigOrbitalParameters orbit = this.config.planets[i].orbit;

                //Create gameobject
                GameObject go = new GameObject();
                go.name = "Planet: " + conf.name;
                go.transform.SetParent(parent.transform);

                //Create planet
                Planet planet = new Planet(conf);
                generatedAstralBodies.Add(planet);

                //Create-Configure orbit prefab
                SimpleOrbit orb = go.AddComponent<SimpleOrbit>();
                orb.semiMajorAxis = orbit.semiMajorAxis;
                orb.eccentricity = orbit.eccentricity;
                orb.rotation = orbit.rotation;

                orb.focusPoint = SimpleOrbit.FocalPoint.NegativeMajorAxis;
                orb.focusObject = config.sunObject.transform;

                orb.startPosition = orbit.startPosition;

                //Create-Config floating transform (if using this system)
                FloatingTransform floater = go.AddComponent<FloatingTransform>();

                if (sunFloat != null)
                    orb.floatingFocusObject = sunFloat;

                floater.unityPosition = orb.Evaluate(orb.startPosition);

                if(sunFloat != null)
                    floater.worldPosition = orb.EvaluateFloating(orb.startPosition);

                //Render the planet
                planet.RenderOn(go);

                //for (int j = 0; j < this.config.planets[i].moons.Length; i++) {
                //
                //}
            }

            return generatedAstralBodies.ToArray();
		}

    }

}
