using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks.Radar {

/// <summary>
/// Represents an object when on the 3d radar
/// </summary>
public class Radar3dElement : MonoBehaviour {

    /// <summary>
    /// Shape of object on the radar
    /// </summary>
    public Mesh shape;
    /// <summary>
    /// Size of object on the radar
    /// </summary>
    public float scale;
    /// <summary>
    /// Color of object on the radar
    /// </summary>
    public Color color;

    /// <summary>
    /// Add myself to the radar manager
    /// </summary>
    void Start () {
        if(Radar3d.scene)
            Radar3d.scene.elements.Add(this);
	}

    /// <summary>
    /// Remove myself from the radar manager
    /// </summary>
    void OnDestroy() {
        if (Radar3d.scene)
            Radar3d.scene.elements.Remove(this);
    }

	// Update is called once per frame
	void Update () {
		
	}
}

}