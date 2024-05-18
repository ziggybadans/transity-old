using System;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private int _numTriesCities, _numTriesTowns, _numTriesRural;
    private GridManager _grid;
    private int _numCellsX, _numCellsY;
    [Header("Debug")]
    [SerializeField]
    private bool debug;

    public static event Action OnMapGenerationFinish;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMapGenerationStart += Initialize;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMapGenerationStart -= Initialize;
        }
    }

    private void Initialize()
    {
        _numTriesCities = SettingsManager.Instance.GetMapGenValue(SettingsTypes.NumCities);
        _numTriesTowns = SettingsManager.Instance.GetMapGenValue(SettingsTypes.NumTowns);
        _numTriesRural = SettingsManager.Instance.GetMapGenValue(SettingsTypes.NumRurals);
        if (debug) Debug.Log("Retrieved settings: " + _numTriesCities + ", " + _numTriesTowns + ", " + _numTriesRural);

        _grid = GridManager.Instance;

        _numCellsX = _grid.GRID_WIDTH;
        _numCellsY = _grid.GRID_HEIGHT;
        if (debug) Debug.Log("Retrieved grid settings: " + _numCellsX + ", " + _numCellsY);

        UpdateProbabilities();
        GenerateMap();

        OnMapGenerationFinish?.Invoke();
        gameObject.SetActive(false);
    }

    private void GenerateMap()
    {
        if (debug) Debug.Log("Checking spawn probability for cities...");
        for (int i = 1; i <= _numTriesCities; i++) CheckSpawnProbability(SettlementType.City);
        if (debug) Debug.Log("Checking spawn probability for towns...");
        for (int i = 1; i <= _numTriesTowns; i++) CheckSpawnProbability(SettlementType.Town);
        if (debug) Debug.Log("Checking spawn probability for rural localities...");
        for (int i = 1; i <= _numTriesTowns; i++) CheckSpawnProbability(SettlementType.Rural);
    }

    private void CheckSpawnProbability(SettlementType settlementType)
    {
        int x = UnityEngine.Random.Range(0, _numCellsX - 1);
        int y = UnityEngine.Random.Range(0, _numCellsY - 1);
        Cell currentCell = _grid.GetCellFromPos(new Vector2Int(x, y));
        float spawnProbability = currentCell.GetProbability(settlementType);

        float r = UnityEngine.Random.Range(0, 100) / 100f;
        if (spawnProbability > r && currentCell.Settlement == null)
        {
            if (debug) Debug.Log("Chance succeeded. Spawning " + settlementType.ToString() + "...");
            SpawnSettlement(currentCell, settlementType);
        }
        else if (debug) Debug.Log("Probability of " + spawnProbability + " was not high enough to spawn a " + settlementType.ToString() + " against random value of " + r + ". Skipping...");

        UpdateProbabilities();
    }

    private void SpawnSettlement(Cell currentCell, SettlementType settlementType)
    {
        Sprite sprite = settlementType switch
        {
            SettlementType.City => GameManager.Instance.CitySettlementSprite,
            SettlementType.Town => GameManager.Instance.TownSettlementSprite,
            SettlementType.Rural => GameManager.Instance.RuralSettlementSprite,
            _ => GameManager.Instance.CitySettlementSprite,
        };
        GameObject settlementObj = Instantiate(GameManager.Instance.SettlementPrefab, currentCell.transform.position + (Vector3.forward * -2f), Quaternion.identity, currentCell.transform);
        Settlement settlement = settlementObj.GetComponent<Settlement>();
        settlement.Type = settlementType;
        settlement.GetComponent<SpriteRenderer>().sprite = sprite;
        settlement.ParentCell = currentCell;
        currentCell.Settlement = settlement;
    }

    public void UpdateProbabilities()
    {
        if (debug) Debug.Log("Updating probabilities...");
        for (int x = 0; x < _numCellsX; x++)
        {
            for (int y = 0; y < _numCellsY; y++)
            {
                Cell currentCell = _grid.GetCellFromPos(new Vector2Int(x, y));
                if (currentCell.Settlement != null) currentCell.SetAllProbabilities(0f);
                else
                {
                    currentCell.SetAllProbabilities(1f);

                    foreach (Cell cell in _grid.GetCells())
                    {
                        if (cell == currentCell) continue;

                        if (cell.Settlement != null)
                        {
                            float distance = CalculateDistance(currentCell, cell);

                            if (cell.Settlement.Type == SettlementType.City) CalculateCityProbability(currentCell, distance);
                            if (cell.Settlement.Type == SettlementType.Town) CalculateTownProbability(currentCell, distance);
                            if (cell.Settlement.Type == SettlementType.Rural) CalculateRuralProbability(currentCell, distance);
                        }
                    }
                }
            }
        }
    }

    private float CalculateDistance(Cell currentCell, Cell checkCell)
    {
        float distanceX = Mathf.Abs(currentCell.transform.position.x - checkCell.transform.position.x);
        float distanceY = Mathf.Abs(currentCell.transform.position.y - checkCell.transform.position.y);
        float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY) / _grid.GRID_CELL_SIZE;

        return distance;
    }

    private void CalculateCityProbability(Cell currentCell, float distance)
    {
        float cityCalculation = Mathf.Max(0, ((100f / Mathf.Pow(1f + Mathf.Exp(-0.5f * (distance - 10f)), 1f)) - 1f) / 100f);
        float cityProbability = currentCell.GetProbability(SettlementType.City) * cityCalculation / (cityCalculation > 0 ? 2 : 0);
        currentCell.SetProbability(SettlementType.City, Mathf.Round(cityProbability * 100f) / 100f);

        float townCalculation = 25f;
        if (distance < 4)
        {
            townCalculation = Mathf.Pow(distance + 1, 3);
        }
        else if (distance > 4 && distance < 15)
        {
            townCalculation = -1f * Mathf.Pow((0.5f * distance) - 2, 2) + 75;
        }

        float townProbability = Mathf.Max(0.25f, townCalculation / 100f,
            currentCell.GetProbability(SettlementType.Town) < 1f ? currentCell.GetProbability(SettlementType.Town) : 0);
        currentCell.SetProbability(SettlementType.Town, Mathf.Round(townProbability * 100f) / 100f);
    }

    private void CalculateTownProbability(Cell currentCell, float distance)
    {
        if (distance <= 3)
        {
            float townCalculation = Mathf.Max(0, (100f / Mathf.Pow(1f + Mathf.Exp(-0.5f * (distance - 10f)), 0.25f)) - 1f);
            float townProbability = Mathf.Min(townCalculation / 100f, currentCell.GetProbability(SettlementType.Town));
            currentCell.SetProbability(SettlementType.Town, Mathf.Round(townProbability * 100f) / 100f);

            float cityCalculation = Mathf.Max(0, ((100f / Mathf.Pow(1f + Mathf.Exp(-0.5f * (distance - 10f)), 0.25f)) - 1f) / 0.75f);
            float cityProbability = Mathf.Min(cityCalculation / 100f, currentCell.GetProbability(SettlementType.City));
            currentCell.SetProbability(SettlementType.City, Mathf.Round(cityProbability * 100f) / 100f);
        }
    }

    private void CalculateRuralProbability(Cell currentCell, float distance)
    {
        float ruralProbability = currentCell.GetProbability(SettlementType.City) * 2 * currentCell.GetProbability(SettlementType.Town);
        currentCell.SetProbability(SettlementType.Rural, Mathf.Round(ruralProbability * 100f) / 100f);
    }
}