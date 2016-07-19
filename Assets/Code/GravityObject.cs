using UnityEngine;
using System.Collections;

public class GravityObject : MonoBehaviour {
	public GravityObject target;
	public static float G = 0.000001f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Rigidbody body = this.gameObject.GetComponent<Rigidbody> ();
		if (body == null) {
			Debug.Log ("GravityObjects need rigidbodies");
			return;
		}
		if (target != null) {
			Vector3 appliedForce = FindGravityForce (target.GetComponent<GravityObject> ());
			body.AddForce (appliedForce);
		}
	}

	Vector3 FindGravityForce(GravityObject m2) {
		return FindGravityForce (this, m2);
	}

	public static Vector3 FindGravityForce(GravityObject m1, GravityObject m2) {
		Vector3 direction = (m2.gameObject.transform.position - m1.gameObject.transform.position);
		direction.Normalize ();
		float magnitude = G * m1.gameObject.GetComponent<Rigidbody>().mass * 
							  m2.gameObject.GetComponent<Rigidbody>().mass /
		                  	 (m2.gameObject.transform.position - m1.gameObject.transform.position).sqrMagnitude;
		return magnitude * direction;
	}
}
