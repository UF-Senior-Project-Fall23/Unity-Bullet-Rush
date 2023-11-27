using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public static BossController instance;

    private GameObject currentBossPrefab = null;
    private GameObject player;
    private List<GameObject> indicators = new();

    public GameObject currentBoss = null;
    public Dictionary<string, GameObject> bossPrefabs;
    public String BossName;
    public GameObject portalPrefab;
    public GameObject indicatorPrefab;
    public GameObject CircleIndicatorPrefab;
    
    public GameObject inidcatorSmallPrefab;

    public List<string> runBosses; // The list of bosses for the currently generated run, in order.

    // Establishes singleton instance
    private void Awake()
    {
        Debug.Log("Awakened Boss Controller");
        if (instance == null)
        {
            Debug.Log("Generated new instance of Boss Controller");
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Generates boss prefab dictionary and gets player instance
    void Start()
    {
        bossPrefabs = Resources.LoadAll<GameObject>("Prefabs/Boss").ToDictionary(x => x.name, x => x);
        string[] actualBosses = {"Cordelia", "Blagthoroth", "Onyx"};
        foreach (var key in bossPrefabs.Keys.ToList())
        {
            if (!actualBosses.Contains(key))
            {
                bossPrefabs.Remove(key);
            }
        }

        if (player == null)
            player = PlayerController.instance.gameObject;
    }

    public void SummonBoss(Vector3 pos, float health)
    {
        currentBoss = Instantiate(currentBossPrefab, pos, Quaternion.identity);
        currentBoss.GetComponent<IHealth>().MaxHealth = health;
        StartCoroutine(currentBoss.GetComponent<Boss>().StartPhase());
    }

    public void LoadBoss(string bossName)
    {
        Debug.Log("Loading Boss: " + bossName);
        
        currentBossPrefab = bossPrefabs[bossName];
        
        FindObjectOfType<MusicManager>()?.LoadBossMusic(bossName);
    }
    
    public void BossDie(Vector3 deathPos, Quaternion deathAng)
    {
        Debug.Log("Boss Died");

        GameObject portal;
        
        if (currentBoss is null)
        {
            portal = Instantiate(portalPrefab, deathPos, deathAng);
        }
        else
        {
            portal = Instantiate(portalPrefab, currentBoss.transform.position, currentBoss.transform.rotation);
        }
        
        Debug.LogWarning($"Current Level is {GameManager.instance.getCurrentLevel()}");
        
        if (GameManager.instance.getCurrentLevel() == 3)
        {
            portal.GetComponent<Portal>().destination = "Start";
            Debug.Log("You won, generating start portal!");
        }
        else
        {
            portal.GetComponent<Portal>().destination = "Loot Room"; 
        }
        

        currentBossPrefab = null;
        currentBoss = null;
    }

    public void BossDie()
    {
        BossDie(currentBoss.transform.position, currentBoss.transform.rotation);
    }

    public void MinionDie()
    {
        Debug.Log("Minion Died");
        currentBossPrefab = null;
        currentBoss = null;

        removeAllIndicators();

        indicators.Clear();
    }

    public void removeAllIndicators()
    {
        foreach (var i in indicators)
            Destroy(i);
    }

    public GameObject Indicate(Vector3 position, Quaternion rotation)
    {
        GameObject indicator = Instantiate(indicatorPrefab, position, rotation);
        indicators.Add(indicator);
        return indicator;
    }
    public GameObject IndicateCircle(Vector3 position, Quaternion rotation)
    {
        GameObject indicator = Instantiate(CircleIndicatorPrefab, position, rotation);
        indicators.Add(indicator);
        return indicator;
    }

    //Returns the predicted position of the player
    //Strength is how strong the prediction is
    public Vector3 GetPredictedPos(float strength)
    {
        Vector3 playerPos = PlayerController.instance.transform.position;
        return (Vector2)playerPos + Vector2.ClampMagnitude(PlayerController.instance.GetComponent<Rigidbody2D>().velocity, strength);
    }

    public void RemoveIndicator(GameObject indicator)
    {
        indicators.Remove(indicator);
        Destroy(indicator);
    }

    public void GenerateRun()
    {
        runBosses = bossPrefabs.Keys.OrderBy(x => new System.Random().Next()).Take(3).ToList();
        GameManager.instance.setLevel(0);
        
        Debug.Log($"Generated new run. Bosses: {string.Join(", ", runBosses)}");
    }

    public void StartBoss(int index)
    {
        string boss = runBosses[index];
        LoadBoss(boss);
        SummonBoss(new Vector3(-75, 25), 40); // TODO: Make this automatically adjust based on the boss
    }
}