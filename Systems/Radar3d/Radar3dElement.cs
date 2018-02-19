using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar3dElement : MonoBehaviour {

    public Mesh shape;
    public float scale;
    public Color color;

    // Use this for initialization
    void Start () {
        if(Radar3d.scene)
            Radar3d.scene.elements.Add(this);
	}

    void OnDestroy() {
        if (Radar3d.scene)
            Radar3d.scene.elements.Remove(this);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
