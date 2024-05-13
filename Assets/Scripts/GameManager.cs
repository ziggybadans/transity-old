using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton references
    [SerializeReference]
    public static GameManager Instance;

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
    }

    // Main game logic
    public GameState state;
    public static event Action OnMapGenerationStart;

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
                GenerateMap();
                break;
            case GameState.Play:
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

    private void GenerateMap()
    {
        SceneManager.LoadScene("Game");
        OnMapGenerationStart?.Invoke();
    }

    public int DebugMode { get; set; }

    // Asset managing
    public Sprite CitySettlementSprite;
    public Sprite TownSettlementSprite;
    public Sprite RuralSettlementSprite;

    public Sprite TransportSprite;
    public Sprite CellSprite;

    public GameObject CitySettlementPrefab;
    public GameObject TownSettlementPrefab;
    public GameObject RuralSettlementPrefab;

    public GameObject TransportPrefab;

    public Material ConnectionMaterial;
}

// TODO: Add TimeState enum
public enum GameState {
    Menu,
    MapGeneration,
    Play,
    Pause
}