using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : MonoBehaviour
{
    private enum STATE
    {
        GOINGMINING,
        MINING,
        GOINGHOME,
        HOME,
        RETURNINGORE,
        SPAWNING,
        MAX
    }
    private STATE currentState = STATE.GOINGMINING;
    public float speed = 5f; // Speed at which the miner moves
    private Ore target = null;

    [SerializeField]
    private float damage = 50.0f;
    [SerializeField]
    private float homeRange = 1.0f;
    [SerializeField]
    private float miningRange = 0.25f;
    [SerializeField]
    private float returnSpeed = 0.25f;

    private float capacity = 100;

    private Timer returnOreTimer;

    private void Start()
    {
        returnOreTimer = TimerUtility.Instance.CreateTimer(returnSpeed, () =>
        {
            capacity += 10;
            GameManager.Instance.IncrementOreCout(10);
            if (capacity >= 100)
            {
                currentState = STATE.GOINGMINING;
            }
            else
            {
                Debug.Log($"Miner {gameObject.name} still retuning ore");
                TimerUtility.Instance.RegisterTimer(returnOreTimer, startImmediate: true);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        // State machine
        switch (currentState)
        {
            case STATE.GOINGMINING:
                // If there's no target, find the closest one
                if (target == null)
                {
                    target = FindClosestOre();
                    if (target == null)
                    {
                        OreManager.Instance.InstantiateGrid();
                    }
                    else
                    {
                        Debug.Log($"{gameObject.name} found a new target {target.gameObject.name}.");
                    }
                }

                MoveTowardsTarget();
                break;
            case STATE.MINING:
                Mine();
                break;
            case STATE.GOINGHOME:
                MoveTowardsHome();
                break;
            case STATE.HOME:
                TimerUtility.Instance.RegisterTimer(returnOreTimer, true);
                currentState = STATE.RETURNINGORE;
                break;
            case STATE.RETURNINGORE:
                break;
            case STATE.MAX:
                break;
            default:
                break;
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
        if (Vector2.Distance(transform.position, targetPosition) < homeRange)
        {
            Debug.Log($"{gameObject.name} reached the {target.name} position.");
            currentState = STATE.HOME;
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
        if (Vector2.Distance(transform.position, targetPosition) < miningRange)
        {
            Debug.Log($"{gameObject.name} reached the {target.gameObject.name} position.");
            currentState = STATE.MINING;
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
            if (OreManager.Instance.RemainingOre.Count == 0)
            {
                currentState = STATE.GOINGHOME;
            }
            else
            {
                currentState = STATE.GOINGMINING;
            }
        }

        if (capacity <= 0)
        {
            // We are full and need to return home
            currentState = STATE.GOINGHOME;
        }
    }
}
