using UnityEngine;

public class Ore : MonoBehaviour
{
    [SerializeField]
    private float health = 100;
    public float Health { get { return health; } }

    public void Init(float healtMultiplyer)
    {
        health *= healtMultiplyer;
    }
}
