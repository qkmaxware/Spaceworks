using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks
{

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

	[System.Serializable]
	public struct NoiseSample
	{
		public float value;
		public Vector3 derivative;

		public static NoiseSample operator + (NoiseSample a, NoiseSample b)
		{
			a.value += b.value;
			a.derivative += b.derivative;
			return a;
		}

		public static NoiseSample operator + (NoiseSample a, float b)
		{
			a.value += b;
			return a;
		}

		public static NoiseSample operator + (float a, NoiseSample b)
		{
			b.value += a;
			return b;
		}

		public static NoiseSample operator - (NoiseSample a, float b)
		{
			a.value -= b;
			return a;
		}

		public static NoiseSample operator - (float a, NoiseSample b)
		{
			b.value = a - b.value;
			b.derivative = -b.derivative;
			return b;
		}

		public static NoiseSample operator - (NoiseSample a, NoiseSample b)
		{
			a.value -= b.value;
			a.derivative -= b.derivative;
			return a;
		}
               
		//Apply multiplication to value and derivative (cf)' = cf'
		public static NoiseSample operator * (NoiseSample a, float b)
		{
			a.value *= b;
			a.derivative *= b;
			return a;
		}

		public static NoiseSample operator * (float a, NoiseSample b)
		{
			b.value *= a;
			b.derivative *= a;
			return b;
		}
               
		//Apply product rule to derivative
		public static NoiseSample operator * (NoiseSample a, NoiseSample b)
		{
			a.derivative = a.derivative * b.value + b.derivative * a.value;
			a.value *= b.value;
			return a;
		}
               
	}


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

	public interface INoise
	{
		NoiseSample Value (Vector3 x, float freq);
		NoiseSample Sum (Vector3 x, NoiseOptions opts);
	}

}