using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks { 

	public class SolarSystemService : MonoBehaviour {

		public SolarSystemConfig config;

		public SolarSystem generatedSystem { get; private set;}
		public Planet[] generatedPlanets { get; private set;}

		void Start(){
			SolarSystem s = new SolarSystem (config);
			generatedPlanets = s.Render ();
			generatedSystem = s;
		}

		void Update(){
			
		}
	}

}