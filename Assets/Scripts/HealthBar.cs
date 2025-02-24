using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Transform target; // The object this health bar follows
    public Vector2 offset = new Vector2(0, 1); // Offset from the target

    private SpriteRenderer barRenderer;
    private float maxHealth = 100f;
    private float currentHealth;
    private Vector3 initialScale;

    void Start()
    {
        // Create the health bar object
        GameObject bar = new GameObject("HealthBar");
        bar.transform.SetParent(transform);
        barRenderer = bar.AddComponent<SpriteRenderer>();
        barRenderer.sprite = CreateRoundedBarTexture();
        barRenderer.color = Color.green;

        initialScale = bar.transform.localScale;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = (Vector2)target.position + offset;
        }
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        float healthPercentage = currentHealth / maxHealth;

        // Scale the bar width based on health percentage
        barRenderer.transform.localScale = new Vector3(initialScale.x * healthPercentage, initialScale.y, 1);

        // Interpolate color from green to red
        barRenderer.color = Color.Lerp(Color.red, Color.green, healthPercentage);
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
}
