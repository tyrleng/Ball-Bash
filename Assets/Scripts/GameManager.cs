using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    public GameObject gameTimeUI;
    public GameObject settingsUI; 
        
    public GameObject resetButton;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveNumText;
    public Text BounceButtonText;
    
    public GameObject enemyNormalPrefab;
    public GameObject enemyLargePrefab;
    public GameObject powerupRepulsionPrefab;
    public GameObject powerupBouncePrefab;
    public bool isGameActive;

    private int waveNumber = 4;
    private int enemiesDefeated;


    public void Reset()
    {
        resetButton.SetActive(false);
        SceneManager.LoadScene(0);
    }
    
    // Called by Settings Button. Effectively pauses the game.
    public void ShowSettings()
    {
        Time.timeScale = 0;
        settingsUI.SetActive(true);
    }

    // Called by UI Button to resume the paused Game
    public void Resume()
    {
        Time.timeScale = 1;
        settingsUI.SetActive(false);
    }
    
    // Called by a UI element to start the game
    public void StartGame()
    {
        gameTimeUI.SetActive(true);
        isGameActive = true;
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
    Application.Quit();
#endif
    }

    public void GameOver()
    {
        isGameActive = false;
        resetButton.SetActive(true);
    }

    public void CollectedBouncePowerUp(int powerupBounceRemaining)
    {
        BounceButtonText.text = "Bounce: " + powerupBounceRemaining;
    }

    public void UsedBouncePowerUp(int powerupBounceRemaining)
    {
        BounceButtonText.text = "Bounce: " + powerupBounceRemaining;
    }
    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if (isGameActive)
        {
            SpawnNewWaveIfNecessary();
        }
    }

    private void SpawnNewWaveIfNecessary()
    {
        GameObject[] enemiesNormalRemaining = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] enemiesLargeRemaining = GameObject.FindGameObjectsWithTag("EnemyLarge");
        int enemiesCount = enemiesNormalRemaining.Length + enemiesLargeRemaining.Length;
        if (enemiesCount == 0)
        {
            waveNumber++;
            waveNumText.text = "Wave: " + waveNumber;
           SpawnEnemies(waveNumber);
           SpawnPowerUps(waveNumber);
        }
    }

    private void SpawnEnemies(int waveNumberLocal)
    {
        int numLargeEnemiesToSpawn = 0;
        if (waveNumberLocal >= 4 && waveNumber <= 7)
        {
            numLargeEnemiesToSpawn = Mathf.RoundToInt(0.25f * waveNumberLocal);
        } 
        else if (waveNumberLocal >= 8)
        {
            numLargeEnemiesToSpawn = Mathf.RoundToInt(0.5f * waveNumberLocal);
        }
        for (int index = 0; index < waveNumberLocal; index++)
        {
            if (numLargeEnemiesToSpawn > 0)
            {
                Instantiate(enemyLargePrefab, RandomSpawnPosition(), enemyLargePrefab.transform.rotation);
                numLargeEnemiesToSpawn--;
            }
            else
            {
                Instantiate(enemyNormalPrefab, RandomSpawnPosition(), enemyNormalPrefab.transform.rotation);
            }
        }
    }

    private void SpawnPowerUps(int waveNumberLocal)
    {
        //magic math
        float variableNumber = Mathf.FloorToInt(waveNumberLocal / 3);
        float finalSpawnNumber = variableNumber + 2;
        for (int index = 0; index < finalSpawnNumber; index++)
        {
            if (index % 2 == 0)
            {
                Instantiate(powerupBouncePrefab, RandomSpawnPosition(), powerupBouncePrefab.transform.rotation);
            }
            else
            {
                Instantiate(powerupRepulsionPrefab, RandomSpawnPosition(), powerupRepulsionPrefab.transform.rotation);
            }
        }
    }

    private Vector3 RandomSpawnPosition()
    {
        float xPos = Random.Range(-8, 8);
        float zPos = Random.Range(-8, 8);        
        return new Vector3(xPos, 0, zPos);
    }

    public void EnemyDestroyed()
    {
        enemiesDefeated += 1;
        scoreText.text = "Score: " + enemiesDefeated;
    }
    
}