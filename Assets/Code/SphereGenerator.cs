using UnityEngine;
using System.Collections;

public class SphereGenerator : MonoBehaviour {
	public float bigRadius;
	public float tubeRadius;
	public bool randomize;
	public GameObject target;
	public static GameObject sphereTarget;
	public GameObject gravtiyCenter;
    public GameObject head;
    public int numSpheres = 200;
    bool initialized;

	// Use this for initialization
	void Start () {
        initialized = false;
	}
	
	// Update is called once per frame
	void Update () {
	    if (!initialized && head.GetComponent<SteamVR_TrackedObject>().isValid)
        {
            Initialize();
        }
	}

	public static GameObject Generate () {
		return Instantiate (sphereTarget);
	}

    void Initialize()
    {
        sphereTarget = target;
        //random torus
        GameObject torus = new GameObject();
        torus.transform.position = head.transform.position + head.transform.forward*0.7f + head.transform.up * -0.4f;
        torus.name = "torus";
        gravtiyCenter.transform.position = torus.transform.position;
        if (randomize)
        {
            for (int i = 0; i < numSpheres; i++)
            {
                float theta = Random.Range(0, 2 * Mathf.PI);
                float phi = Random.Range(0, 2 * Mathf.PI);
                // Start 0.5 meters in front of the headest
                GameObject star = Instantiate(target);
                star.gameObject.transform.SetParent(torus.gameObject.transform);
                float x = (bigRadius + tubeRadius * Mathf.Cos(theta)) * Mathf.Cos(phi);
                float z = (bigRadius + tubeRadius * Mathf.Cos(theta)) * Mathf.Sin(phi);
                float y = tubeRadius * Mathf.Sin(theta);
                Vector3 r = new Vector3(x, y, z);
                star.gameObject.transform.localPosition = r;

                star.GetComponent<GravityObject>().target = gravtiyCenter.GetComponent<GravityObject>();
                float speed = Mathf.Sqrt(GravityObject.G * gravtiyCenter.GetComponent<Rigidbody>().mass /
                    r.magnitude);
                Vector3 direction = Vector3.Cross(new Vector3(0, 1, 0), r);
                direction.Normalize();
                //star.GetComponent<Rigidbody> ().velocity = speed * direction;
            }
        }
        initialized = true;
    }
}
