using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mines ore in a straight line, fast with low capacity
/// </summary>
public class StripMiner : Miner
{
    private Ore nextTarget = null;
    protected override bool Mine()
    {
        if (target == null) return false;

        List<Ore> ores = new List<Ore>();
        ores.Add(target);

        Ore oreExists = FindOreBehind(target);

        // Work down the ore in a line till we find the end
        while (oreExists != null)
        {
            ores.Add(oreExists);
            oreExists = FindOreBehind(oreExists);
        }
        // IT WORKS

        // TODO: This is currently broken. If the first block is mined, damage is not passed down the line. I have to implement a way to mine the first block but still apply that damage to all remaining ore in the strip. Maybe this miner will be allowed to go over specified capacity?
        float oresMined = 0;
        foreach (Ore currentTarget in ores)
        {
            if (currentTarget.Mined(damage))
            {
                TargetMined(currentTarget);
                oresMined += currentTarget.Value;
            }
        }

        capacity -= oresMined;

        if (OreManager.Instance.RemainingOre.Count == 0 || capacity <= 0)
        {
            currentState = STATE.GOINGHOME;
            return true;
        }
        else
        {
            currentState = STATE.GOINGMINING;
        }
        return false;
    }

    /// <summary>
    /// The strip miner will continue to work in a straight line rather than moving to other columns
    /// </summary>
    protected override Ore FindClosestOre()
    {
        // We have the ore behind our previous target, set it as the new target and find the next
        if (nextTarget != null)
        {
            Ore previous = nextTarget;
            // Here we should be able to work all the way down a strip we find the end
            nextTarget = previous.Neighbors[1] ? previous.Neighbors[1] : null;
            return previous;
        }

        // We will simply find the closest ore
        List<Ore> ores = OreManager.Instance.RemainingOre;
        Ore newTarget = null;
        float closest = float.MaxValue;
        foreach (Ore currentTarget in ores)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (distance < closest)
            {
                newTarget = currentTarget;
                closest = distance;
            }
        }

        // Find the next target behind the current
        nextTarget = newTarget.Neighbors[1] ? newTarget.Neighbors[1] : null;

        // We want to check if next is null, then that would imply we have made it to the end of the row
        if (nextTarget == null)
        {
            return null;
        }
        else
        {
            return newTarget;
        }
    }

    private Ore FindOreBehind(Ore firstore)
    {
        return firstore.Neighbors[1] ? firstore.Neighbors[1] : null;
    }

    private void OnDrawGizmos()
    {
        if (debugDraw)
        {
            if (nextTarget != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(nextTarget.transform.position, debugRadius);
            }

            if (target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(target.transform.position, debugRadius);
            }
        }
    }
}
