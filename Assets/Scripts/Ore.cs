using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Ore : MonoBehaviour
{
    [SerializeField]
    private float health = 100;
    public float Health { get { return health; } }

    private float maxHealth = 100;

    [SerializeField]
    private GameObject minePos = null;
    public GameObject MinePos { get { return minePos; } }

    private bool targeted = false;
    public bool Targeted { get { return targeted; } set { targeted = value; } }

    private float value = 10.0f;
    public float Value { get { return value; } }

    private Ore[] neighbours = null;
    public Ore[] Neighbors { get { return neighbours; } set { neighbours = value; } }

    private SpriteRenderer spriteRenderer = null;

    [SerializeField]
    private SpriteRenderer barRenderer = null;

    public void Init(float healthMultiplier, float oreScaling, float valueMultiplier)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"Failed to retrieve the renderer on {gameObject.name}");
            return;
        }

        health *= healthMultiplier;
        gameObject.transform.localScale = new Vector3(oreScaling, oreScaling, oreScaling);
        value *= valueMultiplier;

        if (barRenderer == null)
        {
            Debug.LogError($"Please assign bar renderer to {gameObject.name}");
        }
        else
        {
            barRenderer.sprite = CreateRoundedBarTexture();
            barRenderer.color = Color.green;

            Vector3 transform = barRenderer.gameObject.transform.position;
            transform.z = -0.1f;
            barRenderer.transform.position = transform;

            barRenderer.gameObject.SetActive(false);
        }
    }

    public bool Mined(float damage)
    {
        health -= damage;

        SetHealth(health);
        if (health < 0)
        {
            NotifyNeighbours(this);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void NotifyNeighbours(Ore thisOre)
    {
        foreach (var currNeighbour in neighbours)
        {
            if (currNeighbour != null)
            {
                for (int i = 0; i < currNeighbour.Neighbors.Length; i++)
                {
                    if (currNeighbour.Neighbors[i] == thisOre)
                    {
                        currNeighbour.Neighbors[i] = null;
                    }
                }
                currNeighbour.UpdateTexture();
            }
        }
    }

    public void UpdateTexture()
    {
        string textureKey = OreManager.Instance.DetermineOrePosition(this);
        Sprite newTexture = OreManager.Instance.GetTexture(textureKey);
        spriteRenderer.sprite = newTexture;
    }

    public void DestroyOre()
    {
        Destroy(gameObject);
    }

    private Sprite CreateRoundedBarTexture()
    {
        Texture2D texture = new Texture2D(32, 8);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                float distance = Mathf.Abs(x - texture.width / 2f) / (texture.width / 2f);
                Color color = distance < 0.9f ? Color.white : new Color(1, 1, 1, 0); // Soft edges
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public void SetHealth(float health)
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        float healthPercentage = health / maxHealth;

        if (!barRenderer.gameObject.activeInHierarchy || health != maxHealth)
        {
            barRenderer.gameObject.SetActive(true);
        }

        // Interpolate color from green to red
        barRenderer.color = Color.Lerp(Color.red, Color.green, healthPercentage);
    }
}
