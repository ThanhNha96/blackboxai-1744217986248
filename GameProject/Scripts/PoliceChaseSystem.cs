using UnityEngine;
using System.Collections;

public class PoliceChaseSystem : MonoBehaviour
{
    public float speedLimit = 100f; // Speed limit for the race
    public float chaseDistance = 15f; // Distance at which police will start chasing
    public GameObject policePrefab; // Reference to the police prefab
    public Transform player; // Reference to the player
    private bool isChasing = false;

    void Update()
    {
        CheckSpeed();
    }

    void CheckSpeed()
    {
        float playerSpeed = player.GetComponent<VehicleController>().currentSpeed; // Assuming VehicleController has currentSpeed property

        if (playerSpeed > speedLimit && !isChasing)
        {
            StartChase();
        }
    }

    void StartChase()
    {
        isChasing = true;
        Debug.Log("Police are chasing the player!");

        // Spawn police NPCs to chase the player
        SpawnPolice();
    }

    void SpawnPolice()
    {
        // Instantiate police NPCs at random positions around the player
        for (int i = 0; i < 3; i++) // Spawn 3 police cars
        {
            Vector3 spawnPosition = player.position + new Vector3(Random.Range(-chaseDistance, chaseDistance), 0, Random.Range(-chaseDistance, chaseDistance));
            GameObject policeCar = Instantiate(policePrefab, spawnPosition, Quaternion.identity);
            policeCar.GetComponent<PoliceAI>().SetTarget(player); // Assuming PoliceAI has a method to set the target
        }
    }
}
