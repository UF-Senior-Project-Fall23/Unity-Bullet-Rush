using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    // Start is called before the first frame update
    void Start()
    {
        bossPrefabs = Resources.LoadAll<GameObject>("Prefabs/Boss").ToDictionary(x => x.name, x => x);

        if (player == null)
            player = PlayerController.instance.gameObject;

        Debug.Log("Calling LoadBoss");
        LoadBoss(BossName);
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
        
        if (currentBossPrefab != null) SummonBoss(transform.position, 20f);
    }
    
    public void BossDie()
    {
        Instantiate(portalPrefab, currentBoss.transform.position, currentBoss.transform.rotation);
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
}