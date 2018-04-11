using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks{

	/// <summary>
	/// Fast random integer class with algoithm based off the xorshift algorithm for 32bit integers
	/// </summary>
	public class FastRandom{

		private int state;

		/// <summary>
		/// Construct a fast random from an initial state
		/// </summary>
		/// <param name="state"></param>
		public FastRandom (int state){
			this.state = state;
		}

		/// <summary>
		/// Set/Reset the state
		/// </summary>
		/// <param name="state"></param>
		public void SetState(int state){
			this.state = state;
		}

		/// <summary>
		/// Get the next integer
		/// </summary>
		/// <returns></returns>
		public int Next(){
			int x = state;
			x ^= x << 13;
			x ^= x >> 17;
			x ^= x << 5;
			state = x;
			return x;
		}

		/// <summary>
		/// Get the next integer between a max and min
		/// </summary>
		/// <param name="max">Max value</param>
		/// <param name="min">Min value</param>
		/// <returns></returns>
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
