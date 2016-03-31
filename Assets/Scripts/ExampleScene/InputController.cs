using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour
{
    public Stratecam stratecam;
    public GameObject objectToGoTo;
    public GameObject objectToFollow;

    void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            stratecam.Follow(objectToFollow);
        }
        if (Input.GetKey(KeyCode.G))
        {
            stratecam.GoTo(objectToGoTo.transform.position);
        }
    }

    void SetSmoothingFactor(float x)
    {
        stratecam.smoothingFactor = x;
    }

    void SetPanSpeed(float x)
    {
        stratecam.panSpeed = x;
    }

    void SetRotationSpeed(float x)
    {
        stratecam.rotationSpeed = x;
    }

    void SetZoomSpeed(float x)
    {
        stratecam.zoomSpeed = x;
    }

    void SetGoToSpeed(float x)
    {
        stratecam.goToSpeed = x;
    }

    void SetDoubleClickMovement(bool x)
    {
        stratecam.allowDoubleClickMovement = x;
    }
}