using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

	public abstract class IDetailer : MonoBehaviour {

		/// <summary>
		/// Shows the chunk details.
		/// </summary>
		/// <param name="node">Node.</param>
		public abstract void ShowChunkDetails (QuadNode<ChunkData> node, Mesh m);

		/// <summary>
		/// Hides the chunk details.
		/// </summary>
		/// <param name="node">Node.</param>
		public abstract void HideChunkDetails (QuadNode<ChunkData> node);

	}

}