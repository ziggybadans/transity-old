using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton references
    public static GameManager Instance;
    public event Action OnMapGenerationStart;
    public event Action OnDebugModeChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        MapGenerator.OnMapGenerationFinish += UpdateGameStatePlay;
    }

    // Main game logic
    public GameState state;

    private void Start() {
        UpdateGameState(GameState.Menu);
    }

    public void Quit() {
        Application.Quit();
    }

    public void UpdateGameState(GameState newState) {
        state = newState;

        switch (newState) {
            case GameState.Menu:
                break;
            case GameState.MapGeneration:
                StartCoroutine(GenerateMap());
                break;
            case GameState.Play:
                MapGenerator.OnMapGenerationFinish -= UpdateGameStatePlay;
                break;
            case GameState.Pause:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    public void UpdateGameStateMenu() => UpdateGameState(GameState.Menu);
    public void UpdateGameStateMapGen() => UpdateGameState(GameState.MapGeneration);
    public void UpdateGameStatePlay() => UpdateGameState(GameState.Play);
    public void UpdateGameStatePause() => UpdateGameState(GameState.Pause);

    private IEnumerator GenerateMap()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // After the scene is loaded, invoke the event
        OnMapGenerationStart?.Invoke();
    }

    public int DebugMode { get; private set; }

    public void SetDebugMode(int value) {
        DebugMode = value;
        OnDebugModeChanged?.Invoke();
    }

    // Asset managing
    public Sprite CitySettlementSprite;
    public Sprite TownSettlementSprite;
    public Sprite RuralSettlementSprite;

    public GameObject SettlementPrefab;

    public GameObject TransportPrefab;
    public GameObject PassengerPrefab;

    public Material ConnectionMaterial;
}

// TODO: Add TimeState enum
public enum GameState {
    Menu,
    MapGeneration,
    Play,
    Pause
}