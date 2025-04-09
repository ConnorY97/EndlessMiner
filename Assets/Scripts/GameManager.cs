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
    private float oreCountFloat = 0;

    [SerializeField]
    private Button spawnMiner = null;

    [SerializeField]
    private Button spawnExpertMiner = null;

    [SerializeField]
    private Button spawnStripMiner = null;

    [SerializeField]
    private int minerCost = 100;

    [SerializeField]
    private float expertMinerMultiplier = 1.25f;

    [SerializeField]
    private float stripMinerMultiplier = 1.50f;

    [SerializeField]
    private GameObject minerPrefab = null;

    [SerializeField]
    private GameObject expertMinerPrefab = null;

    [SerializeField]
    private GameObject StripMinerPrefab = null;

    private List<Miner> miners = new List<Miner>();
    public List<Miner> Miners { get { return miners; } }
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

        if (spawnExpertMiner == null)
        {
            Debug.LogError("Please assign spawnExpertMiner Icon");
        }

        if (spawnStripMiner == null)
        {
            Debug.LogError("Please assign spawnStripMiner Icon");
        }

        Miner startingMiner = GameObject.FindWithTag("Miner").GetComponent<Miner>();
        if (startingMiner != null)
        {
            miners.Add(startingMiner);
        }
        else
        {
            Debug.Log("Failed to find starting miner");
        }
    }

    private void Update()
    {
        oreCountUI.text = oreCountFloat.ToString();
    }

    public void IncrementOreCount(int incrementAmount)
    {
        if (oreCountUI != null && incrementAmount > 0)
        {
            oreCountFloat += incrementAmount;
        }

        if (oreCountFloat >= minerCost)
        {
            spawnMiner.interactable = true;
        }

        if (oreCountFloat >= (minerCost * 1.25f))
        {
            spawnExpertMiner.interactable = true;
        }

        if (oreCountFloat >= (minerCost * 1.5f))
        {
            spawnStripMiner.interactable = true;
        }
    }

    public void BuyMiner()
    {
        oreCountFloat -= minerCost;

        if (oreCountFloat < minerCost)
        {
            spawnMiner.interactable = false;
        }

        miners.Add(Instantiate(minerPrefab, transform).GetComponent<Miner>());
    }

    public void BuyExpertMiner()
    {
        float expertMinerCost = minerCost * expertMinerMultiplier;
        oreCountFloat -= expertMinerCost;

        if (oreCountFloat < expertMinerCost)
        {
            spawnExpertMiner.interactable = false;
        }

        miners.Add(Instantiate(expertMinerPrefab, transform).GetComponent<Miner>());
    }

    public void BuyStripMiner()
    {
        float stripMinerCost = minerCost * stripMinerMultiplier;
        oreCountFloat -= stripMinerCost;

        if (oreCountFloat < stripMinerCost)
        {
            spawnStripMiner.interactable = false;
        }

        miners.Add(Instantiate(StripMinerPrefab, transform).GetComponent<Miner>());
    }
}
