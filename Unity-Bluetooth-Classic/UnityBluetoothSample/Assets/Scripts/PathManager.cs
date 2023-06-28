using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathManager : MonoBehaviour
{
    public const int maxPercent = 100;
    public GameObject groundPrefab; // assign this in the inspector
    public GameObject obstaclePrefab; // assign this in the inspector
    public PlayerMovement player; // assign this in the inspector
    public int percentForObstacleSpawn = 1;
    public float pathWidth = 500f;

    private Queue<GameObject> grounds = new Queue<GameObject>();

    private float groundLength = 100f; // this should match the length of your ground block
    private int numInitialGrounds = 50; // the number of ground blocks to start with
    private Vector3 lastGroundEnd; // this is the new variable to keep track of the end of the last ground piece
    private System.Random random = new System.Random(); // for random obstacle creation

    void Start()
    {
        lastGroundEnd = player.transform.position - new Vector3(0, groundLength, 0);

        for (int i = 0; i < numInitialGrounds; i++)
        {
            AddGroundEnd();
        }
    }

    void Update()
    {
        if (player.transform.position.z > grounds.Peek().transform.position.z)
        {
            RemoveGroundOutsideFrustum();
            AddGroundEnd();
        }
    }

    private void RemoveGroundOutsideFrustum()
    {
        GameObject groundToRemove = grounds.Peek();
        Renderer groundRenderer = groundToRemove.GetComponent<Renderer>();

        if (groundRenderer.isVisible)
        {
            return; // The ground piece is still visible, so we don't remove it
        }

        // Remove the ground piece if it's outside the camera's view frustum
        Destroy(groundToRemove);
        grounds.Dequeue();
    }

    private void AddGroundEnd()
    {
        GameObject newGround = Instantiate(groundPrefab);
        newGround.transform.position = lastGroundEnd;
        grounds.Enqueue(newGround);

        // update lastGroundEnd to the end of the ground piece we just added
        lastGroundEnd = newGround.transform.position + new Vector3(0, 0, groundLength);

        // Instantiate obstacle with a chance
        if (random.Next(maxPercent) < percentForObstacleSpawn)
        {
            GameObject newObstacle = Instantiate(obstaclePrefab);

            // Position the obstacle on the new ground piece at a random X position
            float randomX = newGround.transform.position.x + (float)random.NextDouble() * pathWidth - pathWidth / 2;
            newObstacle.transform.position = new Vector3(randomX, newGround.transform.position.y + groundLength, newGround.transform.position.z);
        }
    }
}
