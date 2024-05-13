using System;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private int _numTriesCities, _numTriesTowns, _numTriesRural;
    private Settlement _cityPrefab, _townPrefab, _ruralPrefab;
    private GameObject _passengerEntityPrefab;
    private GridManager _grid;
    private int _numCellsX, _numCellsY;

    public static event Action OnMapGenerationFinish;

    private void Awake()
    {
        GameManager.OnMapGenerationStart += Initialize;
    }

    private void OnDestroy() {
        GameManager.OnMapGenerationStart -= Initialize;
    }

    private void Initialize()
    {
        _numTriesCities = SettingsManager.Instance.GetMapGenValue(SettingsTypes.NumCities);
        _numTriesTowns = SettingsManager.Instance.GetMapGenValue(SettingsTypes.NumTowns);
        _numTriesRural = SettingsManager.Instance.GetMapGenValue(SettingsTypes.NumRurals);

        _numCellsX = _grid.GRID_WIDTH;
        _numCellsY = _grid.GRID_HEIGHT;

        UpdateProbabilities();
        GenerateMap();

        OnMapGenerationFinish?.Invoke();
        Destroy(this);
    }

    private void GenerateMap()
    {
        for (int i = 1; i <= _numTriesCities; i++) CheckSpawnProbability(SettlementType.City);
        for (int i = 1; i <= _numTriesTowns; i++) CheckSpawnProbability(SettlementType.Town);
        for (int i = 1; i <= _numTriesTowns; i++) CheckSpawnProbability(SettlementType.Rural);
    }

    private void CheckSpawnProbability(SettlementType settlementType)
    {
        int x = UnityEngine.Random.Range(0, _numCellsX - 1);
        int y = UnityEngine.Random.Range(0, _numCellsY - 1);
        Cell currentCell = _grid.GetCellFromPosition(new Vector2Int(x, y));
        float spawnProbability = currentCell.GetProbability(settlementType);

        float r = UnityEngine.Random.Range(0, 100) / 100f;
        if (spawnProbability > r && currentCell.Settlement != null)
        {
            SpawnSettlement(currentCell, settlementType);
        }

        UpdateProbabilities();
    }

    private void SpawnSettlement(Cell currentCell, SettlementType settlementType)
    {
        Settlement prefab = settlementType switch
        {
            SettlementType.City => _cityPrefab,
            SettlementType.Town => _townPrefab,
            SettlementType.Rural => _ruralPrefab,
            _ => _cityPrefab,
        };
        Settlement settlement = Instantiate(prefab, currentCell.transform.position + (Vector3.forward * -2f), Quaternion.identity, currentCell.transform);
        settlement.Type = settlementType;
        settlement.EntityPrefab = _passengerEntityPrefab;
        settlement.Map = gameObject;
        settlement.ParentCell = currentCell;
        currentCell.Settlement = settlement;
    }

    public void UpdateProbabilities()
    {
        bool debugMessageSent = false;
        for (int x = 0; x < _numCellsX; x++)
        {
            for (int y = 0; y < _numCellsY; y++)
            {
                Cell currentCell = _grid.GetCellFromPosition(new Vector2Int(x, y));
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
                        float ruralProbability = currentCell.GetProbability(SettlementType.City) * 2 * currentCell.GetProbability(SettlementType.Town);
                        currentCell.SetProbability(SettlementType.Rural, ruralProbability);
                    }
                }

                debugMessageSent = UpdateDebugOverlay(debugMessageSent, currentCell);
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
        currentCell.SetProbability(SettlementType.City, cityProbability);

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
        currentCell.SetProbability(SettlementType.Town, townProbability);
    }

    private void CalculateTownProbability(Cell currentCell, float distance)
    {
        if (distance <= 3)
        {
            float townCalculation = Mathf.Max(0, (100f / Mathf.Pow(1f + Mathf.Exp(-0.5f * (distance - 10f)), 0.25f)) - 1f);
            float townProbability = Mathf.Min(townCalculation / 100f, currentCell.GetProbability(SettlementType.Town));
            currentCell.SetProbability(SettlementType.Town, townProbability);

            float cityCalculation = Mathf.Max(0, ((100f / Mathf.Pow(1f + Mathf.Exp(-0.5f * (distance - 10f)), 0.25f)) - 1f) / 0.75f);
            float cityProbability = Mathf.Min(cityCalculation / 100f, currentCell.GetProbability(SettlementType.City));
            currentCell.SetProbability(SettlementType.City, cityProbability);
        }
    }

    private void CalculateRuralProbability(Cell currentCell, float distance)
    {
        throw new NotImplementedException();
    }

    private bool UpdateDebugOverlay(bool debugMessageSent, Cell currentCell)
    {
        int debugMode = GameManager.Instance.DebugMode;
        float clampedProbability;
        switch (debugMode)
        {
            case 1:
                clampedProbability = currentCell.GetProbability(SettlementType.City) * 2;
                if (!debugMessageSent) Debug.Log("Debug mode: City Heatmap");
                break;
            case 2:
                clampedProbability = currentCell.GetProbability(SettlementType.Town);
                if (!debugMessageSent) Debug.Log("Debug mode: Town Heatmap");
                break;
            case 3:
                clampedProbability = currentCell.GetProbability(SettlementType.Rural);
                if (!debugMessageSent) Debug.Log("Debug mode: Rural Heatmap");
                break;
            default:
                clampedProbability = 0f;
                if (!debugMessageSent) Debug.Log("Debug mode: Off");
                break;
        }

        if (debugMode > 0)
        {
            Color cellColor = Color.Lerp(Color.red, Color.green, clampedProbability);
            currentCell.GetComponent<Renderer>().material.color = cellColor;
        }
        else currentCell.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);

        return true;
    }
}