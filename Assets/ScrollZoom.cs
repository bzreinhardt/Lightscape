using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScrollZoom : MonoBehaviour {
	public int zoomLevel;
	public GameObject zoomSphere;
	public float zoomRadius;
	public int minZoomSpheres = 0;
	public int maxZoomSpheres = 4;
	Dictionary<int, List<GameObject>> newSpheres;


	// Use this for initialization
	void Start () {
		zoomLevel = 0;
		zoomRadius = 0.7f;
		newSpheres = new Dictionary<int, List<GameObject>> ();
	}
	
	// Update is called once per frame
	void Update () {
		float scroll = Input.GetAxis ("Mouse ScrollWheel");

		if (scroll != 0) {
			int change = scroll > 0f ? 1 : -1;
			zoomLevel = zoomLevel + change;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			RaycastHit hit;
			bool success = Physics.Raycast (ray, out hit, 10000f);
//			Debug.Log ("hit success = " + success);
			Vector3 center = hit.point;
			float scale = (float)zoomLevel * 0.01f;

			//zoomSphere.SetActive (true);
			//zoomSphere.transform.position = center;

			//zoomSphere.transform.localScale = new Vector3 (scale, scale, scale);

			// Generate some spheres near the hit point
			if (change > 0) {
				for (int i = 0; i < Random.Range (minZoomSpheres, maxZoomSpheres); i++) {
					GameObject newSphere = SphereGenerator.Generate ();
					Vector3 delta = new Vector3 (Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f);
					delta = 0.01f * delta;
					newSphere.GetComponent<SphereMemory>().SetOriginalPosition(center + delta);

					if (!newSpheres.ContainsKey (zoomLevel)) {
						newSpheres.Add (zoomLevel, new List<GameObject> ());
					}
					newSpheres [zoomLevel].Add (newSphere);
				}
			} 

			else {
				if (newSpheres.ContainsKey(zoomLevel)) {
					foreach (GameObject sphere in newSpheres[zoomLevel]) {
						sphere.SetActive(false);
					}
				}
			}


			// Check what's inside the zoom sphere

			var objects = GameObject.FindGameObjectsWithTag("zoomable");
			foreach (var obj in objects) {
				Vector3 rtp = EuclideanToSpherical (obj.GetComponent<SphereMemory> ().oldPosition - center);
				rtp.x = rtp.x * (1.0f + (float)zoomLevel * 0.1f);
				obj.transform.position = SphericalToEuclidean (rtp) + center;
			}
			 

		} 
	}

	Vector3 EuclideanToSpherical(Vector3 xyz) {
		float r = xyz.magnitude;
		float theta = Mathf.Acos (xyz.y / r);

		float phi = Mathf.Atan2 (xyz.z, xyz.x);
		return new Vector3 (r, theta, phi);
	}

	Vector3 SphericalToEuclidean(Vector3 rtp) {
		float x = rtp.x * Mathf.Sin(rtp.y) * Mathf.Cos(rtp.z);
		float y = rtp.x * Mathf.Cos(rtp.y);
		float z = rtp.x * Mathf.Sin(rtp.y)* Mathf.Sin(rtp.z);
		return new Vector3 (x, y, z);
	}
}
