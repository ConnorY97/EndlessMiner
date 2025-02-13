using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Miner : MonoBehaviour
{
    public float speed = 5f;
    public float maxSpeed = 7f;
    private Ore target = null;
    private Rigidbody2D rb = null;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError($"Failed to find RB2D on {gameObject.name}.");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
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
                Debug.Log($"{gameObject.name} found new target");
            }
        }
        else
        {
            // Move towards the target using forces
            MoveTowardsTarget();
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }

    private void MoveTowardsTarget()
    {
        if (target == null || rb == null) return;

        // Calculate the direction to the target
        Vector2 direction = (target.gameObject.transform.position - transform.position).normalized;

        // Apply force towards the target
        rb.AddForce(direction * speed);
    }

    private Ore FindClosestOre()
    {
        List<Ore> ores = OreManager.Instance.RemainingOre;
        Ore newTarget = null;
        float closestOre = float.MaxValue;
        foreach (Ore ore in ores)
        {
            float currentDistance = Vector2.Distance(transform.position, ore.transform.position);
            if (currentDistance < closestOre)
            {
                closestOre = currentDistance;
                newTarget = ore;
            }
        }
        return newTarget;
    }
}
