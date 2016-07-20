using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class ControllerTracker : MonoBehaviour
{
    public bool trigger;
    public bool triggerDown;
    public bool triggerUp;
    public bool grip;
    SteamVR_TrackedObject trackedObj;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void FixedUpdate()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);
        triggerDown = device.GetHairTriggerDown() ? true : false;


        if (device.GetHairTrigger())
        {
            trigger = true;
        }
        else
        {
            trigger = false;
        }
        if (device.GetPress(Valve.VR.EVRButtonId.k_EButton_Grip))
        {
            grip = true;

        }
        else
        {
            grip = false;
        }
    }
}