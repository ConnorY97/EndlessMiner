using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton Creation
    // Singleton instance
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                // Look for an existing instance in the scene
                instance = FindObjectOfType<GameManager>();

                if (instance == null)
                {
                    // Create a new GameObject with OreManager if none exists
                    GameObject singletonObject = new GameObject("OreManager");
                    instance = singletonObject.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    // Prevent direct instantiation
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning($"Multiple instances of {nameof(GameManager)} found. Destroying the duplicate.");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Optional: Makes the instance persist across scenes
    }
    #endregion

    [SerializeField]
    private GameObject home = null;
    public GameObject Home { get { return home; } }
}
