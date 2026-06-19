using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene;

    public Vector2 platformingRespawnPoint;

    public Vector2 respawnPoint;
    [SerializeField] Bench bench;

    [SerializeField] private FadeUI pauseMenu;
    [SerializeField] private float fadeTime;
    public bool gameIsPaused;

    public static GameManager Instance { get; private set;}
    private void Awake()
    {
        SaveData.Instance.Initialize();
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        SaveScene();
        DontDestroyOnLoad(gameObject);
        bench = FindAnyObjectByType<Bench>();
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.P))
        {
            SaveData.Instance.SavePlayerData();
        }

        if(Input.GetKey(KeyCode.Escape) && !gameIsPaused)
        {
            pauseMenu.FadeUIIn(fadeTime);
            Time.timeScale = 0;
            gameIsPaused = true;
        }
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        gameIsPaused = false;
    }

    public void SaveScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SaveData.Instance.sceneNames.Add(currentSceneName);
    }

    public void RespawnPlayer()
    {
        SaveData.Instance.LoadBench();
        
        if(SaveData.Instance.benchSceneName != null)
        {
            SceneManager.LoadScene(SaveData.Instance.benchSceneName);
        }
        if(SaveData.Instance.benchPos != null)
        {
            respawnPoint = SaveData.Instance.benchPos;
        }
        else
        {
            respawnPoint = platformingRespawnPoint;
        }
                
        PlayerControl.Instance.transform.position = respawnPoint;

        StartCoroutine(UiManager.Instance.DeactivateDeathScreen());
        PlayerControl.Instance.Respawned();
    }
}
