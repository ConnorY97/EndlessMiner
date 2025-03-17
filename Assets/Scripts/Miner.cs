using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle;
using Unity.VisualScripting;
using UnityEngine;

public class Miner : MonoBehaviour
{
    protected enum STATE
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
    protected STATE currentState = STATE.GOINGMINING;
    [SerializeField]
    protected float speed = 5f; // Speed at which the miner moves
    protected Ore target = null;

    [SerializeField]
    protected float damage = 50.0f;
    [SerializeField]
    protected float homeRange = 1.0f;
    [SerializeField]
    protected float miningRange = 0.25f;
    [SerializeField]
    protected float returnSpeed = 0.25f;
    [SerializeField]
    protected float mineSpeed = 0.25f;
    [SerializeField]
    protected float capacity = 100;

    protected Timer returnOreTimer;
    protected Timer mineTimer;

    protected bool logging = true;

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
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

        mineTimer = TimerUtility.Instance.CreateTimer(mineSpeed, () =>
        {
            if (!Mine())
            {
                Debug.Log($"Miner {gameObject.name} still mining ore");
                TimerUtility.Instance.RegisterTimer(mineTimer, startImmediate: true);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        Tick();
    }

    protected virtual void Tick()
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
                        ForceLog($"{gameObject.name} found a new target {target.gameObject.name}.");
                    }
                }

                MoveTowardsTarget();
                break;
            case STATE.STARTMINING:
                TimerUtility.Instance.RegisterTimer(mineTimer, startImmediate: true);
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
            Log($"{gameObject.name} reached the {target.name} position.");
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
            Log($"{gameObject.name} reached the {target.gameObject.name} position.");
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
            Log("Targeting ores in previous row");
            targetOres = oresInPreviousRow;
        }
        else if (oresInCurrentRow.Count > 0)
        {
            Log("Targeting ores in current row");
            targetOres = oresInCurrentRow;
        }
        else
        {
            Log("Targeting any ore");
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

    protected virtual bool Mine()
    {
        if (target == null) return false;

        capacity -= target.Value;

        if (target.Mined(damage))
        {
            TargetMined(target);
            if (OreManager.Instance.RemainingOre.Count == 0 || capacity <= 0)
            {
                currentState = STATE.GOINGHOME;
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

    protected void Log(string msg)
    {
        if (logging)
        {
            Debug.Log(msg);
        }
    }

    private void ForceLog(string msg)
    {
        Debug.Log(msg);
    }

    protected void TargetMined(Ore mined)
    {
        Log($"{gameObject.name} mined {mined.gameObject.name}");
        OreManager.Instance.RemainingOre.Remove(mined);
        mined.DestroyOre();
        if (mined == target)
            target = null;
    }
}
