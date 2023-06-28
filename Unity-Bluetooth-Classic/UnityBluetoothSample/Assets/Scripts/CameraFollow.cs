using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player; // target to follow
    public Vector3 offset; // offset between camera and target

    void LateUpdate()
    {
        // update the camera's position based on the target's position and the offset
        transform.position = player.transform.position + offset;

        // make the camera look at the target
        transform.LookAt(player.transform);
    }
}
