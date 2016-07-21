using UnityEngine;
using System.Collections;

public class SphereGenerator : MonoBehaviour {
	public float bigRadius;
	public float tubeRadius;
	public bool randomize;
	public GameObject target;
	public static GameObject sphereTarget;
	public GameObject gravtiyCenter;


	// Use this for initialization
	void Start () {
		sphereTarget = target;
		//random torus
		GameObject torus = new GameObject();
		if (randomize) {
			for (int i = 0; i < 200; i++) {
				float theta = Random.Range (0, 2 * Mathf.PI);
				float phi = Random.Range (0, 2 * Mathf.PI);
                Vector3 torusCenter = gameObject.transform.position;
				GameObject star = Instantiate (target);
				star.gameObject.transform.SetParent (torus.gameObject.transform);
				float x = (bigRadius + tubeRadius * Mathf.Cos(theta)) * Mathf.Cos(phi) + torusCenter.x;
				float z = (bigRadius + tubeRadius * Mathf.Cos(theta)) * Mathf.Sin(phi) + torusCenter.y;
				float y = tubeRadius * Mathf.Sin (theta) + torusCenter.z;
				Vector3 r = new Vector3 (x, y, z);
				star.gameObject.transform.position = r;
				star.GetComponent<GravityObject> ().target = gravtiyCenter.GetComponent<GravityObject>();
				float speed = Mathf.Sqrt (GravityObject.G*gravtiyCenter.GetComponent<Rigidbody>().mass/
					r.magnitude);
				Vector3 direction = Vector3.Cross (new Vector3 (0, 1, 0), r);
				direction.Normalize();
				//star.GetComponent<Rigidbody> ().velocity = speed * direction;
			}
		} else {
		//non random torus
			for (int i = 0; i < 10; i++) {
				float theta = i * 2 * Mathf.PI/10;
				for (int j = 0; j < 10; j++) {
					float phi = j * 2 * Mathf.PI / 10;
					GameObject star = Instantiate (target);
					star.gameObject.transform.SetParent (torus.gameObject.transform);
					float x = (bigRadius + tubeRadius * Mathf.Cos(theta)) * Mathf.Cos(phi);
					float z = (bigRadius + tubeRadius * Mathf.Cos(theta)) * Mathf.Sin(phi);
					float y = tubeRadius * Mathf.Sin (theta);
					star.gameObject.transform.position = new Vector3 (x, y, z);
				}
			} 
		}


	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static GameObject Generate () {
		return Instantiate (sphereTarget);
	}
}
