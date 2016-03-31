using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public Terrain terrain;

    private void Update()
    {
        transform.Translate(Vector3.forward*Time.deltaTime*10.0f);

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + Time.deltaTime*5.0f, transform.localEulerAngles.z);
        transform.position = new Vector3(transform.position.x, terrain.SampleHeight(transform.position) + 5.0f,
            transform.position.z);
    }
}