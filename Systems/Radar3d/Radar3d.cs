using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks.Radar {

/// <summary>
/// Represents a 3d radar display
/// </summary>
public class Radar3d : MonoBehaviour {

    /// <summary>
    /// Pool of radar instances
    /// </summary>
    private class RadarPoolInstance {
        public GameObject gameObject;
        public MeshFilter filter;
        public MeshRenderer renderer;
    }

    /// <summary>
    /// Stores all elements possibly visible on the radar
    /// </summary>
    public static RadarSceneManager scene;

    /// <summary>
    /// Size of the radar display in world coordinates
    /// </summary>
    public float displaySize = 1;
    /// <summary>
    /// Radisu of the sphere in which elements are visisble on the radar
    /// </summary>
    public float worldRange = 40;

    /// <summary>
    /// Material for radar elements
    /// </summary>
    public Material trackerMaterial;

    private Queue<RadarPoolInstance> radarPool = new Queue<RadarPoolInstance>();
    private Dictionary<Radar3dElement, RadarPoolInstance> usedFilters = new Dictionary<Radar3dElement, RadarPoolInstance>();

    /// <summary>
    /// Create a radar manager if it doesnt exist
    /// </summary>
    void Awake() {
        if (!scene) {
            GameObject go = new GameObject("3d Radar Manager");
            scene = go.AddComponent<RadarSceneManager>();
        }
    }

    /// <summary>
    /// Grab a radar display element from the pool
    /// </summary>
    /// <returns></returns>
    RadarPoolInstance GetDisplayElement() {
        if (radarPool.Count > 1) {
            return radarPool.Dequeue();
        }
        else {
            RadarPoolInstance rf = null;
            for (int i = 0; i < 4; i++) {
                GameObject go = new GameObject("Tracker");
                go.transform.SetParent(this.transform);
                MeshFilter f = go.AddComponent<MeshFilter>();
                MeshRenderer r = go.AddComponent<MeshRenderer>();
                r.material = this.trackerMaterial;

                go.SetActive(false);

                if (rf == null) {
                    RadarPoolInstance rpi = new RadarPoolInstance();
                    rpi.gameObject = go;
                    rpi.filter = f;
                    rpi.renderer = r;
                    rf = rpi;
                }
            }
            return rf;
        }
    }

    /// <summary>
    /// Update position of radar elements from their real world positions relative to this display
    /// </summary>
    void Update() {
        float scale = displaySize / worldRange;

        List<Color> colours = new List<Color>();
        List<Vector3> points = new List<Vector3>();

        foreach (Radar3dElement element in scene.elements) {
            Vector3 localPos = this.transform.InverseTransformPoint(element.transform.position);
            float distance = localPos.magnitude;

            if (distance * distance < worldRange * worldRange) {
                //Draw
                MeshFilter mf;
                if (!this.usedFilters.ContainsKey(element)) {
                    RadarPoolInstance rpi = GetDisplayElement();
                    rpi.gameObject.transform.localScale = Vector3.one * element.scale;
                    mf = rpi.filter;
                    rpi.renderer.material.color = element.color;
                    mf.gameObject.SetActive(true);
                    mf.sharedMesh = element.shape;
                    this.usedFilters[element] = rpi;
                }
                else {
                    mf = this.usedFilters[element].filter;
                }

                mf.transform.localPosition = localPos * scale;

                colours.Add(element.color);
                points.Add(mf.transform.localPosition);
                points.Add(new Vector3(mf.transform.localPosition.x, 0, mf.transform.localPosition.z));
            }
            else {
                //Hide

                if (this.usedFilters.ContainsKey(element)) {
                    RadarPoolInstance rpi = this.usedFilters[element];
                    this.usedFilters.Remove(element);
                    rpi.gameObject.SetActive(false);
                    this.radarPool.Enqueue(rpi);
                }

            }

            this.colours = colours.ToArray();
            this.points = points.ToArray();
        }
    }

    private Vector3[] points;
    private Color[] colours;
    private Material mat;
    /// <summary>
    /// Create material
    /// </summary>
    private void SetupMaterial() {
        if (!mat) {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            //Blending
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            //Backface culling
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            //Depth writes
            mat.SetInt("_ZWrite", 0);
        }
    }

    /// <summary>
    /// Draw vertial height offset lines
    /// </summary>
    public void OnRenderObject() {
        SetupMaterial();

        mat.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);

        GL.Begin(GL.LINES);
        if (points != null && points.Length > 1) {
            int k = 0;
            for (int i = 0; i < points.Length; i += 2) {
                GL.Color(colours[k++]);
                GL.Vertex3(points[i].x, points[i].y, points[i].z);
                GL.Vertex3(points[i + 1].x, points[i + 1].y, points[i + 1].z);
            }
        }

        GL.End();
        GL.PopMatrix();
    }

    /// <summary>
    /// Helper gizmos
    /// </summary>
    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(this.transform.position, this.worldRange);
        Gizmos.DrawWireSphere(this.transform.position, this.displaySize);
    }

}

}