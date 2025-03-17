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
    private Button spawnExpertMiner = null;

    [SerializeField]
    private float minerCost = 500.0f;

    [SerializeField]
    private GameObject miner = null;

    [SerializeField]
    private GameObject expertMiner = null;

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

    public void IncrementOreCount(int incrementAmount)
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

        if (oreCountInt > (minerCost * 1.25f))
        {
            spawnExpertMiner.interactable = true;
        }
    }

    public void BuyMiner(int amount)
    {
        oreCountInt -= amount;
        oreCountUI.text = oreCountInt.ToString();

        if (oreCountInt < minerCost)
        {
            spawnMiner.interactable = false;
        }

        miners.Add(Instantiate(miner, transform).GetComponent<Miner>());
    }

    public void BuyExpertMiner(int amount)
    {
        oreCountInt -= amount;

        if (oreCountInt < (minerCost * 1.25f))
        {
            spawnExpertMiner.interactable = false;
        }

        miners.Add(Instantiate(expertMiner, transform).GetComponent<Miner>());
    }
}
