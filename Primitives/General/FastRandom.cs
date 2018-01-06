using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks{

	/// <summary>
	/// Fast random integer class with algoithm based off the xorshift algorithm for 32bit integers
	/// </summary>
	public class FastRandom{

		private int state;

		public FastRandom (int state){
			this.state = state;
		}

		public void SetState(int state){
			this.state = state;
		}

		public int Next(){
			int x = state;
			x ^= x << 13;
			x ^= x >> 17;
			x ^= x << 5;
			state = x;
			return x;
		}

		public int Next(int max, int min){
			int x = state;
			x ^= x << 13;
			x ^= x >> 17;
			x ^= x << 5;
			state = x;

			return x > max ? max : (x < min ? min : x);
		}
	}

}
