using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : MonoBehaviour
{
    private enum STATE
    {
        GOINGMINING,
        STARTMINING,
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
    [SerializeField]
    private float mineSpeed = 0.25f;

    private float capacity = 100;

    private Timer returnOreTimer;
    private Timer minetimer;

    private void Start()
    {
        returnOreTimer = TimerUtility.Instance.CreateTimer(returnSpeed, () =>
        {
            capacity += 10;
            GameManager.Instance.IncrementOreCount(10);
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

        minetimer = TimerUtility.Instance.CreateTimer(mineSpeed, () =>
        {
            if (!Mine())
            {
                Debug.Log($"Miner {gameObject.name} still mining ore");
                TimerUtility.Instance.RegisterTimer(minetimer, startImmediate: true);
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
                        // If something happens to the target, just go home. Don't create a new grid
                        currentState = STATE.GOINGHOME;
                    }
                    else
                    {
                        Debug.Log($"{gameObject.name} found a new target {target.gameObject.name}.");
                    }
                }

                MoveTowardsTarget();
                break;
            case STATE.STARTMINING:
                TimerUtility.Instance.RegisterTimer(minetimer, startImmediate: true);
                currentState = STATE.MINING;
                break;
            case STATE.MINING:
                if (target == null)
                {
                    currentState = STATE.GOINGMINING;
                }
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
            currentState = STATE.STARTMINING;
        }
    }

    private Ore FindClosestOre()
    {
        List<Ore> ores = OreManager.Instance.RemainingOre;
        Ore newTarget = null;
        float closestDistance = float.MaxValue;

        // Calculate the miner's current row based on its position
        float minerRow = Mathf.Floor(transform.position.y / OreManager.Instance.oreSpacing);

        // Make sure there are no ore that are behind the miner
        List<Ore> oresInPreviousRow = ores.FindAll(ore => Mathf.Floor(ore.MinePos.transform.position.y / OreManager.Instance.oreSpacing) == minerRow + 1
        );

        // Filter ores in the same row as the miner
        List<Ore> oresInCurrentRow = ores.FindAll(ore =>
            Mathf.Floor(ore.MinePos.transform.position.y / OreManager.Instance.oreSpacing) == minerRow
        );

        List<Ore> targetOres = new List<Ore>();
        // If no ores are in the current row, allow targeting from other rows
        if (oresInPreviousRow.Count > 0)
        {
            Debug.Log("Targeting ores in previous row");
            targetOres = oresInPreviousRow;
        }
        else if (oresInCurrentRow.Count > 0)
        {
            Debug.Log("Targeting ores in current row");
            targetOres = oresInCurrentRow;
        }
        else
        {
            Debug.Log("Targeting any ore");
            targetOres = ores;
        }

        // Find the closest ore from the target ores
        foreach (Ore ore in targetOres)
        {
            float currentDistance = Vector2.Distance(transform.position, ore.MinePos.transform.position);
            if (currentDistance < closestDistance && !ore.Targeted)
            {
                closestDistance = currentDistance;
                newTarget = ore;
            }
        }

        if (newTarget != null)
        {
            newTarget.Targeted = true;
            return newTarget;
        }
        return null;
    }

    private bool Mine()
    {
        if (target == null) return false;

        capacity -= target.Value;

        if (target.Mined(damage))
        {
            Debug.Log($"{gameObject.name} mined {target.gameObject.name}");
            OreManager.Instance.RemainingOre.Remove(target);
            target.DestoryOre();
            target = null;
            if (OreManager.Instance.RemainingOre.Count == 0 || capacity <= 0)
            {
                currentState = STATE.GOINGHOME;
                return true;

            }
            else
            {
                currentState = STATE.GOINGMINING;
            }
            return true;
        }

        if (capacity <= 0)
        {
            currentState = STATE.GOINGHOME;
            target.Targeted = false;
            target = null;
            return true;
        }
        return false;
    }
}
