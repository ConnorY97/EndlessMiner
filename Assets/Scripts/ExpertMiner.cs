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
        bool minedTarget = false;
        if (target == null) return false;

        capacity -= target.Value * 3;

        if (target.Mined(damage))
        {
            TargetMined(target);
            minedTarget = true;
        }

        if (target.Neighbors[0] != null)
        {
            Ore leftNeighbor = target.Neighbors[0];
            if (leftNeighbor.Mined(damage))
            {
                TargetMined(leftNeighbor);
                Log("Left neighbor mined");
            }
        }
        if (target.Neighbors[2] != null)
        {
            Ore rightNeighbor = target.Neighbors[2];
            if (rightNeighbor.Mined(damage))
            {
                TargetMined(rightNeighbor);
                Log("Right neighbor mined");
            }
        }

        if (minedTarget)
        {
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

        return false;
    }
}
