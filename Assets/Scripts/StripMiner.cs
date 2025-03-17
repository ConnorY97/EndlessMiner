using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mines ore in a straight line, fast with low capacity
/// </summary>
public class StripMiner : Miner
{
    protected override bool Mine()
    {
        if (target == null) return false;

        List<Ore> ores = new List<Ore>();
        ores.Add(target);
        Ore oreExists = FindOreBehing(target);
        // Work down the ore in a line till we find the end
        while (oreExists != null)
        {
            ores.Add(oreExists);
                oreExists = FindOreBehing(oreExists);
        }
        // IT WORKS

        foreach (Ore currentTarget in ores)
        {
            if (currentTarget.Mined(damage))
            {
                capacity -= currentTarget.Value;
                TargetMined(currentTarget);
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
        }

        return false;
    }

    private Ore FindOreBehing(Ore firstore)
    {
        return firstore.Neighbors[1] ? firstore.Neighbors[1] : null;
    }
}
