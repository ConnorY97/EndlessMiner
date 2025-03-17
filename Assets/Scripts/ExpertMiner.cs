using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpertMiner : Miner
{
    protected override void Init()
    {
        base.Init();
    }

    protected override void Tick()
    {
        base.Tick();
    }

    // This miner will be able to mine ore to the left and right or their target, if they exist
    protected override bool Mine()
    {
        if (target == null) return false;

        // Adding the target-able ore
        List<Ore> ores = new List<Ore>();
        ores.Add(target);

        if (target.Neighbors[0] != null)
        {
            ores.Add(target.Neighbors[0]);
        }
        if (target.Neighbors[2] != null)
        {
            ores.Add(target.Neighbors[2]);
        }

        foreach (Ore currentTarget in ores)
        {
            capacity -= currentTarget.Value;

            if (currentTarget.Mined(damage))
            {
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
}
