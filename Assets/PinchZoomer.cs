using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public float epsilon = 0.0001f;
    public int zoomLevel;
    public GameObject zoomSphere;
    public float zoomRadius;
    public int minZoomSpheres = 0;
    public int maxZoomSpheres = 4;
    Dictionary<int, List<GameObject>> newSpheres;
    bool continuousPress;
    float zoomScale;
    Vector3 center;

    void Start()
    {
        zoomLevel = 0;
        zoomRadius = 0.7f;
        newSpheres = new Dictionary<int, List<GameObject>>();
        continuousPress = false;
        center = new Vector3();
    }

    void FixedUpdate()
    {
        // Set up Controller inputs.
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
        // reset everything
        if (rightInput.GetPress(Valve.VR.EVRButtonId.k_EButton_Grip) || leftInput.GetPress(Valve.VR.EVRButtonId.k_EButton_Grip))
        {
            var objects = GameObject.FindGameObjectsWithTag("zoomable");
            foreach (var obj in objects)
            {
                obj.transform.position = obj.GetComponent<SphereMemory>().originalPosition;
            }
            foreach (int key in newSpheres.Keys)
            {
                DestroySpheres(key);
            }
        }

        // Zoom when both triggers are pulled
        if (rightInput.GetHairTrigger() && leftInput.GetHairTrigger())
        {
            rightAnchor = rightController.transform.position;
            leftAnchor = leftController.transform.position;
            
            float d = (rightAnchor - leftAnchor).magnitude;
            int change = 0;
            if (!continuousPress)
            {
                prevDiameter = d;
                center = (rightAnchor - leftAnchor) / 2.0f + leftAnchor;
            }
            float delta = d - prevDiameter;
            if (delta > epsilon)
            {
                change = 1;
            } else if (delta < -epsilon)
            {
                change = -1;
            }
            // reset the zoom level if you had let go of the controllers
            
            zoomLevel = zoomLevel + change;
            prevDiameter = d;
            // Generate some spheres near the hit point
            if (change > 0)
            {
                GenerateSpheres(center);
            } else
            {
                DestroySpheres(zoomLevel);
            }

            var objects = GameObject.FindGameObjectsWithTag("zoomable");
            foreach (var obj in objects)
            {
                Vector3 rtp = EuclideanToSpherical(obj.GetComponent<SphereMemory>().oldPosition - center);
                // move radially towards or away from the gesture center
                rtp.x = ZoomFunction(change, rtp.x);
                obj.transform.position = SphericalToEuclidean(rtp) + center;
                obj.GetComponent<SphereMemory>().oldPosition = obj.transform.position;
            }
            continuousPress = true;
        } else
        {
            var objects = GameObject.FindGameObjectsWithTag("zoomable");
            foreach (var obj in objects)
            {
                obj.GetComponent<SphereMemory>().oldPosition = obj.transform.position;
            }
            // record whether you've been continuously dragging the controllers
            continuousPress = false;
        }
     
    }

    float ZoomFunction(int change, float r)
    {
        // adjust how much faster small distances expand than large ones
        float smallExplosionAdjust = 0.2f;
        // scale the rate of expansion
        float scale = 0.01f;
        return r * Mathf.Exp(change * scale / (smallExplosionAdjust + r));
    }

    Vector3 EuclideanToSpherical(Vector3 xyz)
    {
        float r = xyz.magnitude;
        float theta = Mathf.Acos(xyz.y / r);

        float phi = Mathf.Atan2(xyz.z, xyz.x);
        return new Vector3(r, theta, phi);
    }

    Vector3 SphericalToEuclidean(Vector3 rtp)
    {
        float x = rtp.x * Mathf.Sin(rtp.y) * Mathf.Cos(rtp.z);
        float y = rtp.x * Mathf.Cos(rtp.y);
        float z = rtp.x * Mathf.Sin(rtp.y) * Mathf.Sin(rtp.z);
        return new Vector3(x, y, z);
    }

    void GenerateSpheres(Vector3 center)
    {
        for (int i = 0; i < Random.Range(minZoomSpheres, maxZoomSpheres); i++)
        {
            GameObject newSphere = SphereGenerator.Generate();
            Vector3 delta = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f);
            delta = 0.01f * delta;
            newSphere.GetComponent<SphereMemory>().SetOriginalPosition(center + delta);

            if (!newSpheres.ContainsKey(zoomLevel))
            {
                newSpheres.Add(zoomLevel, new List<GameObject>());
            }
            newSpheres[zoomLevel].Add(newSphere);
        }
    }

    void DestroySpheres(int level)
    {
        // turn off spheres in the order they were created as you zoom back out
        if (newSpheres.ContainsKey(level))
        {
            foreach (GameObject sphere in newSpheres[level])
            {
                sphere.SetActive(false);
            }
        }
    }
}