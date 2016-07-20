using UnityEngine;
using System.Collections;

public class InitializeToTarget : MonoBehaviour {
    public GameObject target;
	// Use this for initialization
	void Start () {
        gameObject.transform.position = target.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
