using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : MonoBehaviour
{
    public float speed = 5f; // Speed at which the miner moves
    private Ore target = null;

    private bool withinRange = false;
    [SerializeField]
    private float damage = 50.0f;

    private float capacity = 100;
    private bool returnHome = false;

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
                Debug.Log($"{gameObject.name} found a new target {target.gameObject.name}.");
            }
        }
        else if (returnHome)
        {
            MoveTowardsHome();
        }
        else
        {
            MoveTowardsTarget();
        }

        if (withinRange)
        {
            if (returnHome)
            {
                capacity = 100;
                returnHome = false;
            }
            else
            {
                Mine();
            }
        }
    }

    private void MoveTowardsHome()
    {
        GameObject target = GameManager.Instance.Home;
        if (target == null) return;

        // Get the position of the target's mine position
        Vector3 targetPosition = target.transform.position;

        // Move towards the target using Vector2.MoveTowards
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the miner has reached the target position
        if (Vector2.Distance(transform.position, targetPosition) < 1.01f)
        {
            Debug.Log($"{gameObject.name} reached the {target.name} position.");
            withinRange = true;
        }
        else
        {
            withinRange = false;
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
            Debug.Log($"{gameObject.name} reached the {target.gameObject.name} position.");
            withinRange = true;
        }
        else
        {
            withinRange = false;
        }
    }

    private Ore FindClosestOre()
    {
        List<Ore> ores = OreManager.Instance.RemainingOre;
        Ore newTarget = null;
        float closestDistance = float.MaxValue;

        // Calculate the miner's current row based on its position
        float minerRow = Mathf.Floor(transform.position.y / OreManager.Instance.oreSpacing);

        // Filter ores in the same row as the miner
        List<Ore> oresInCurrentRow = ores.FindAll(ore =>
            Mathf.Floor(ore.MinePos.transform.position.y / OreManager.Instance.oreSpacing) == minerRow
        );

        // If no ores are in the current row, allow targeting from other rows
        List<Ore> targetOres = oresInCurrentRow.Count > 0 ? oresInCurrentRow : ores;

        // Find the closest ore from the target ores
        foreach (Ore ore in targetOres)
        {
            float currentDistance = Vector2.Distance(transform.position, ore.MinePos.transform.position);
            if (currentDistance < closestDistance && !ore.Targeted)
            {
                closestDistance = currentDistance;
                newTarget = ore;
                // Ensure that only one miner targets an ore at a time
                ore.Targeted = true;
            }
        }

        return newTarget;
    }


    private void Mine()
    {
        if (target == null) return;

        capacity -= target.Value;

        if (target.Mined(damage))
        {
            Debug.Log($"{gameObject.name} mined {target.gameObject.name}");
            OreManager.Instance.RemainingOre.Remove(target);
            target.DestoryOre();
            target = null;
            withinRange = false;
        }

        if (capacity <= 0)
        {
            // We are full and need to return home
            returnHome = true;
        }
    }
}
