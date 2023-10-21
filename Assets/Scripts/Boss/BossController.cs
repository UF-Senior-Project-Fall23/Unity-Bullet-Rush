using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public static BossController instance;

    private Boss currentBossLogic = null;
    private GameObject currentBossPrefab = null;
    private GameObject player;

    public GameObject currentBoss = null;
    public Dictionary<string, GameObject> bossPrefabs;
    public String BossName;
    
    

    private void Awake()
    {
        Debug.Log("Awakened");
        if (instance == null)
        {
            Debug.Log("Generated new instance.");
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

        if (BossName == null)
            BossName = "Cordelia";

        Debug.Log("Calling LoadBoss");
        LoadBoss(BossName);
    }

    // Update is called once per frame
    void Update()
    {
        currentBossLogic?.BossLogic(currentBoss, player.transform.position);
    }

    // Calls a function for the given boss with the name provided.
    // Useful for storing attacks in a list and properly naming the attack functions something other than "Attack1, Attack2", etc.
    public void CallAttack(Boss boss, string attack)
    {
        var bossType = boss.GetType();
        var attackFunction = bossType.GetMethod(attack);
        attackFunction?.Invoke(boss, null);
    }

    public void SummonBoss(Vector3 pos)
    {
        Debug.Log("Is this running?");

        currentBoss = Instantiate(currentBossPrefab, pos, Quaternion.identity);
    }

    public void LoadBoss(string bossName)
    {
        Debug.Log("Loading Boss: " + bossName);

        Vector3 bossPos = new Vector3(-75, 10, 0);
        
        currentBossPrefab = bossPrefabs[bossName];
        currentBossLogic = currentBossPrefab.GetComponent<Boss>();
        
        if (currentBossPrefab != null) SummonBoss(bossPos);
    }
    
    public void BossDie()
    {
        currentBossLogic = null;
        currentBossPrefab = null;
        currentBoss = null;
    }
}