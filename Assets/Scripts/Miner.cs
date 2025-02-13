using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : MonoBehaviour
{
    public float speed = 5f; // Speed at which the miner moves
    private Ore target = null;

    // Update is called once per frame
    void Update()
    {
        // If there's no target, find the closest one
        if (target == null)
        {
            target = FindClosestOre();
            if (target == null)
            {
                Debug.Log($"{gameObject.name} cannot find a new target, dying");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"{gameObject.name} found a new target.");
            }
        }
        else
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        if (target == null) return;

        // Get the position of the target's mine position
        Vector3 targetPosition = target.MinePos.transform.position;

        // Move towards the target using Vector2.MoveTowards
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the miner has reached the target position
        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
        {
            Debug.Log($"{gameObject.name} reached the target position.");
            HandleMining(); // Call mining or other logic here
        }
    }

    private Ore FindClosestOre()
    {
        List<Ore> ores = OreManager.Instance.RemainingOre;
        Ore newTarget = null;
        float closestDistance = float.MaxValue;

        foreach (Ore ore in ores)
        {
            float currentDistance = Vector2.Distance(transform.position, ore.MinePos.transform.position);
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                newTarget = ore;
            }
        }

        return newTarget;
    }

    private void HandleMining()
    {
        // Logic for mining or interacting with the ore
        Debug.Log($"{gameObject.name} is now mining {target.gameObject.name}.");
        // Remove the target from the RemainingOre list, destroy it, or handle mining here
    }
}
