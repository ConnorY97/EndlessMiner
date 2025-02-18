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

    private SpriteRenderer renderer = null;

    public void Init(float healtMultiplyer, float oreScaling, float valueMultiplayer)
    {
        renderer = GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            Debug.LogError($"Failed to retrieve the renderer on {gameObject.name}");
            return;
        }

        health *= healtMultiplyer;
        gameObject.transform.localScale = new Vector3(oreScaling, oreScaling, oreScaling);
        value *= valueMultiplayer;
    }

    public bool Mined(float damage)
    {
        health -= damage;

        if (health < 0)
        {
            NotifyNeighbours();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void NotifyNeighbours()
    {
        foreach (var currNeighbour in neighbours)
        {
            if (currNeighbour != null)
            {
                currNeighbour.UpdateTexture();
            }
        }
    }

    public void UpdateTexture()
    {
        string textureKey = OreManager.Instance.DeterminOrePosition(this);
        Sprite newTexture = OreManager.Instance.GetTexture(textureKey);
        renderer.sprite = newTexture;
    }

    public void DestoryOre()
    {
        Destroy(gameObject);
    }
}
