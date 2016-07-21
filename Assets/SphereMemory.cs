using UnityEngine;
using System.Collections;

public class SphereMemory : MonoBehaviour {
	public Vector3 originalPosition;
	public Vector3 oldPosition;
	public bool resized = false;

	// Use this for initialization
	void Start () {
		originalPosition = gameObject.transform.position;
		oldPosition = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetOriginalPosition(Vector3 position) {
		gameObject.transform.position = position;
		originalPosition = position;
		oldPosition = position;
	}
}
