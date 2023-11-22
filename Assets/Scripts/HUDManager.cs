using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI weaponText;
    public TextMeshProUGUI heatText;
    public GameObject tooltip;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI difficultyText;

    private void Awake()
    {
        if (instance == null)
        {
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
        weaponText.text = "Weapon: None";
        UpdateScoreText();
        UpdateHealthText();
        UpdateDifficultyText();
        UpdateLevelText();

        PlayerController.instance.health.PlayerHealthChanged.AddListener(UpdateHealthText);
        GameManager.instance.ScoreChanged.AddListener(UpdateScoreText);
        GameManager.instance.DifficultyChanged.AddListener(UpdateDifficultyText);
        GameManager.instance.LevelChanged.AddListener(UpdateLevelText);
    }

    void FixedUpdate()
    {
        timeText.text = "Time Elapsed: " + Mathf.Floor(GameManager.instance.gameTime).ToString();
    }

    void UpdateHealthText()
    {
        healthText.text = "Health: " + PlayerController.instance.health.CurrentHealth.ToString();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + GameManager.instance.score.ToString();
    }

    void UpdateDifficultyText()
    {
        difficultyText.text = "Difficulty: " + GameManager.instance.difficulty.ToString();
    }

    void UpdateLevelText()
    {
        levelText.text = "Level: " + GameManager.instance.currentLevel.ToString();
    }

    public void ShowTooltip(string name, string description, Vector3 pos)
    {
        tooltip.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = name;
        tooltip.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = description;
        tooltip.transform.position = pos;
        tooltip.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltip.SetActive(false);
    }
}