using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks
{

	/// <summary>
	/// Class represneting a pair of high and low float values
	/// </summary>
	[System.Serializable]
	public class HighLowPair
	{
		public float high;
		public float low;

		public HighLowPair ()
		{
			high = float.MaxValue;
			low = float.MinValue;
		}

		public HighLowPair (float h, float l)
		{
			this.high = h;
			this.low = l;
		}

		public HighLowPair (HighLowPair other)
		{
			this.high = other.high;
			this.low = other.low;
		}
	}

	/// <summary>
	/// Class for options for CPU noise generation algorithms
	/// </summary>
	[System.Serializable]
	public class NoiseOptions
	{
		public int seed = 0;
		public int octaves = 1;
		public float persistence = 0.8f;
		public float frequency = 1;
		public float lacunarity = 2;
		public Vector3 offset;

		public NoiseOptions ()
		{
		}

		public NoiseOptions (NoiseOptions other)
		{
			this.octaves = other.octaves;
			this.persistence = other.persistence;
			this.frequency = other.frequency;
			this.lacunarity = other.lacunarity;
			this.offset = other.offset;
		}
	}

	/// <summary>
	/// Interface all noise generators must implement
	/// </summary>
	public interface INoise
	{
		float Value (Vector3 x, float freq);
		float Sum (Vector3 x, NoiseOptions opts);
	}

}