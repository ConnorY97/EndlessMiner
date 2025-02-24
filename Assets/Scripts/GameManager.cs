using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    private TMP_Text oreCountUI = null;
    private int oreCountInt = 0;

    [SerializeField]
    private Button spawnMiner = null;

    [SerializeField]
    private float minerCost = 500.0f;

    [SerializeField]
    private GameObject miner = null;

    private void Start()
    {
        if (oreCountUI == null)
        {
            Debug.LogError("Please assign oreCout");
        }
        else
        {
            oreCountUI.text = "0"; 
        }

        if (spawnMiner == null)
        {
            Debug.LogError("Please assign spawnMiner Icon");
        }
    }

    public void IncrementOreCout(int incrementAmount)
    {
        if (oreCountUI != null && incrementAmount > 0)
        {
            oreCountInt += incrementAmount;
            oreCountUI.text = oreCountInt.ToString();
        }

        if (oreCountInt > minerCost)
        {
            spawnMiner.interactable = true;
        }
    }

    public void BuyMinger(int amount)
    {
        oreCountInt -= amount;
        oreCountUI.text = oreCountInt.ToString();

        if (oreCountInt < minerCost)
        {
            spawnMiner.interactable = false;
        }

        Instantiate(miner, transform);
    }
}
