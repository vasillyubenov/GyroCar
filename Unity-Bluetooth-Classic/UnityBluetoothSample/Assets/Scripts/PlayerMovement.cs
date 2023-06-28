using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 80f; // adjust this to control the speed of movement
    public float speedForward = 100f;
    public float speedSteering = 3f;

    [HideInInspector]
    public float rotationAngle;

    private void Update()
    {
        transform.position += new Vector3(rotationAngle * speedSteering, 0f, speedForward) * speed * Time.deltaTime;
    }

    public void UpdatePlayerMovement(float sensorData) 
    {
        rotationAngle = sensorData;
    }

    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}