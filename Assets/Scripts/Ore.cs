using UnityEngine;

public class Ore : MonoBehaviour
{
    [SerializeField]
    private float health = 100;
    public float Health { get { return health; } }

    [SerializeField]
    private GameObject minePos = null;
    public GameObject MinePos { get { return minePos; } }

    public void Init(float healtMultiplyer, float oreScaling)
    {
        health *= healtMultiplyer;
        gameObject.transform.localScale = new Vector3(oreScaling, oreScaling, oreScaling);
    }
}
