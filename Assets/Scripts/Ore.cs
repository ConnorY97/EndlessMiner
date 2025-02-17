using UnityEngine;

public class Ore : MonoBehaviour
{
    [SerializeField]
    private float health = 100;
    public float Health { get { return health; } }

    [SerializeField]
    private GameObject minePos = null;
    public GameObject MinePos { get { return minePos; } }

    private bool targeted = false;
    public bool Targeted { get { return targeted; } set { } }

    private float value = 10.0f;
    public float Value { get { return value; } }

    private Ore[] neighbours = null;
    public Ore[] Neighbours { get { return neighbours; } set { neighbours = value; } }

    public void Init(float healtMultiplyer, float oreScaling, float valueMultiplayer)
    {
        health *= healtMultiplyer;
        gameObject.transform.localScale = new Vector3(oreScaling, oreScaling, oreScaling);
        value *= valueMultiplayer;
    }

    public bool Mined(float damage)
    {
        health -= damage;

        if (health < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DestoryOre()
    {
        Destroy(gameObject);
    }
}
