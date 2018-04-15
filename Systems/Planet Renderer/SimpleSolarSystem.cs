using Spaceworks.Orbits.Kepler;
using Spaceworks.Position;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    public class SimpleSolarSystem : MonoBehaviour {

        public bool generateName = false;
        public new string name;

        [System.Serializable]
        public class Satellite {
            KeplerOrbitalParameters orbit;
        }

        [System.Serializable]
        public class Planetoid {
            public double mass;
            public PlanetConfig planetData;
            public HighLowPair mountains;
            public Material baseMaterial;
            public KeplerOrbitalParameters orbit;
            public CubemapStore textures;
            public CubemapStore heights;
            public Satellite[] satellites;
        }

        private class PlanetoidState {
            public KeplerOrbit orbit;
            public KeplerBody body;
            public Planet planet;
            public GameObject gameobject;
            public FloatingTransform transform;
        }

        public double SolarMass;
        public Planetoid[] planetoids;

        //Internalized state
        KeplerBody sun;
        PlanetoidState[] planetStates;

        void Awake() {
            InitializeSystem();
        }

        void Update() {
            UpdateSystem(Time.deltaTime);
        }

        public void InitializeSystem() {
            //Clear old data
            if (generateName)
                name = (new Spaceworks.PhenomicNameGenerator()).Generate(4, 8);

            DestorySystem();

            //Make the floating origin if it does not exist
            FloatingOrigin.Make();

            //Create the sun's reference (0,0,0) in the model (view not made)
            this.sun = new KeplerBody(this.SolarMass, null);

            //Create the initial states of all planetoids
            this.planetStates = new PlanetoidState[this.planetoids.Length];
            for (int i = 0; i < this.planetoids.Length; i++) {
                try {
                    //Init references
                    Planetoid planet = this.planetoids[i];
                    GameObject planetGO = new GameObject(string.IsNullOrEmpty(planet.planetData.name) ? this.name + " - " + i : planet.planetData.name);
                    PlanetoidState state = new PlanetoidState();
                    this.planetStates[i] = state;

                    //Init orbit model
                    state.orbit = new KeplerOrbit(this.sun, planet.orbit);
                    state.body = new KeplerBody(planet.mass, state.orbit);
                    state.gameobject = planetGO;

                    //Configure components
                    FloatingTransform transform = planetGO.AddComponent<FloatingTransform>();
                    transform.worldPosition = new WorldPosition(state.orbit.GetCurrentPosition());
                    state.transform = transform;

                    CubemapMeshGenerator meshService = planetGO.AddComponent<CubemapMeshGenerator>();
                    meshService.range = planet.mountains;
                    meshService.useSkirts = true;
                    meshService.heights = planet.heights;
                    meshService.Init();

                    CubemapTextureService textureService = planetGO.AddComponent<CubemapTextureService>();
                    textureService.top = planet.baseMaterial;
                    textureService.top.SetTexture("_MainTex", planet.textures.top);

                    textureService.bottom = planet.baseMaterial;
                    textureService.bottom.SetTexture("_MainTex", planet.textures.bottom);

                    textureService.left = planet.baseMaterial;
                    textureService.left.SetTexture("_MainTex", planet.textures.left);

                    textureService.right = planet.baseMaterial;
                    textureService.right.SetTexture("_MainTex", planet.textures.right);

                    textureService.front = planet.baseMaterial;
                    textureService.front.SetTexture("_MainTex", planet.textures.front);

                    textureService.back = planet.baseMaterial;
                    textureService.back.SetTexture("_MainTex", planet.textures.back);

                    PlanetConfig pcc = new PlanetConfig(planet.planetData);
                    pcc.generationService = meshService;
                    pcc.textureService = textureService;

                    Planet p = new Planet(pcc);
                    p.RenderOn(planetGO);
                    if(Camera.main)
                        p.ForceUpdateLODs(Camera.main.transform.position);

                    state.planet = p;
                }
                catch (Exception e){
                    Debug.Log("Failed to fully instanciate planetoid: "+i+" because");
                    Debug.Log(e.Message);
                    Debug.Log(e.StackTrace);
                }
            }
        }

        public void UpdateSystem(float dt) {
            if (this.planetStates != null)
                foreach (PlanetoidState state in this.planetStates) {
                    state.orbit.StepTime(dt);
                    state.transform.worldPosition = new WorldPosition(state.orbit.GetCurrentPosition());
                    state.transform.UpdateUnityPosition();
                }
        }

        public void DestorySystem() {
            if(this.planetStates != null)
            foreach (PlanetoidState state in this.planetStates) {
                Destroy(state.gameobject);
            }
        }

    }

}
