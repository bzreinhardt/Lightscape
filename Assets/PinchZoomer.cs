using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class PinchZoomer : MonoBehaviour {
    public GameObject leftController;
    public GameObject rightController;
    SteamVR_TrackedObject rightTrackedObject;
    SteamVR_TrackedObject leftTrackedObject;
    public GameObject testSphere;
    Vector3 rightAnchor;
    Vector3 leftAnchor;
    float prevDiameter;
    float epsilon = 0.01f;
    public bool debug = false;
    void Awake()
    {
        
    }

    void FixedUpdate()
    {
        if (rightTrackedObject == null)
        {
            if (rightController.GetComponent<SteamVR_TrackedObject>().isValid)
            {
                rightTrackedObject = rightController.GetComponent<SteamVR_TrackedObject>();
            }
        }
        if (leftTrackedObject == null)
        {
            if (leftController.GetComponent<SteamVR_TrackedObject>().isValid)
            {
                leftTrackedObject = leftController.GetComponent<SteamVR_TrackedObject>();
            }
        }
        if (leftTrackedObject == null ||rightTrackedObject == null)
        {
            Debug.Log("need two controllers");
            return;
        }
        var leftInput = SteamVR_Controller.Input((int)leftTrackedObject.index);
        var rightInput = SteamVR_Controller.Input((int)rightTrackedObject.index);

        // Set an anchor when one trigger is down
        if (rightInput.GetHairTrigger() && leftInput.GetHairTrigger())
        {
            Debug.Log("both triggers pressed");
            rightAnchor = rightController.transform.position;
            leftAnchor = leftController.transform.position;
            
            float d = (rightAnchor - leftAnchor).magnitude;
            bool expanding = false;
            bool contracting = false;
            if (d > prevDiameter + epsilon)
            {
                expanding = true;
                Debug.Log("expanding");
            } else if (d < prevDiameter - epsilon)
            {
                contracting = true;
                Debug.Log("contracting");
            }
            prevDiameter = d;
            if (debug)
            {
                testSphere.transform.localScale = new Vector3(d, d, d);
                testSphere.transform.position = (rightAnchor - leftAnchor) / 2.0f + leftAnchor;
            }
            
        }
     
    }
}