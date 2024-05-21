using System;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private int _numTriesCities, _numTriesTowns, _numTriesRural;
    [Header("Debug")]
    [SerializeField]
    private bool debug;
    [SerializeField]
    private bool firstSettlement = true;

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

        GenerateMap();

        OnMapGenerationFinish?.Invoke();
        gameObject.SetActive(false);
    }

    private void GenerateMap()
    {
        if (debug) Debug.Log("Checking spawn probability for cities...");
        for (int i = 1; i < _numTriesCities; i++) CheckSpawnProbability(SettlementType.City);
        firstSettlement = true;
        if (debug) Debug.Log("Checking spawn probability for towns...");
        for (int i = 1; i <= _numTriesTowns; i++) CheckSpawnProbability(SettlementType.Town);
        if (debug) Debug.Log("Checking spawn probability for rural localities...");
        for (int i = 1; i <= _numTriesTowns; i++) CheckSpawnProbability(SettlementType.Rural);
    }

    private void CheckSpawnProbability(SettlementType settlementType)
    {
        int x = (int)UnityEngine.Random.Range(0, (GridManager.Instance.GRID_WIDTH * GridManager.Instance.GRID_CELL_SIZE) - 1);
        int y = (int)UnityEngine.Random.Range(0, (GridManager.Instance.GRID_HEIGHT * GridManager.Instance.GRID_CELL_SIZE) - 1);
        Cell currentCell = GridManager.Instance.GetCellFromPos(new Vector2Int(x, y));
        float spawnProbability = currentCell.GetProbability(settlementType);

        float r = UnityEngine.Random.Range(0, 100) / 100f;
        if (spawnProbability > r || firstSettlement)
        {
            if (debug) Debug.Log("Chance succeeded. Spawning " + settlementType.ToString() + "...");
            SpawnSettlement(currentCell, settlementType);
            if (firstSettlement) firstSettlement = false;
            UpdateProbabilities();
        }
        else if (debug) Debug.Log("Probability of " + spawnProbability + " was not high enough to spawn a " + settlementType.ToString() + " against random value of " + r + ". Skipping...");
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
        foreach (Cell currentCell in GridManager.Instance.GetCells())
        {
            if (currentCell.Settlement != null) currentCell.SetAllProbabilities(0f);
            else
            {
                // We are doing a "reverse raycast" type situation, where the current cell is the one 
                // getting the probability, while the check cell is where the radial probability
                // is propagating from, which is why we're checking for a settlement below.
                foreach (Cell checkCell in GridManager.Instance.GetCells())
                {
                    if (checkCell == currentCell) continue;

                    if (checkCell.Settlement != null)
                    {
                        float distance = CalculateDistance(currentCell, checkCell);

                        if (checkCell.Settlement.Type == SettlementType.City) CalculateCityProbability(currentCell, distance);
                        if (checkCell.Settlement.Type == SettlementType.Town) CalculateTownProbability(currentCell, distance);
                        if (checkCell.Settlement.Type == SettlementType.Rural) CalculateRuralProbability(currentCell, distance);
                    }
                }
            }
        }
    }

    private float CalculateDistance(Cell currentCell, Cell checkCell)
    {
        float distanceX = Mathf.Abs(currentCell.transform.position.x - checkCell.transform.position.x);
        float distanceY = Mathf.Abs(currentCell.transform.position.y - checkCell.transform.position.y);
        float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY) / GridManager.Instance.GRID_CELL_SIZE;

        return distance;
    }

    private void CalculateCityProbability(Cell currentCell, float distance)
    {
        float cityPreviousProbability;
        float cityCalculation;
        float cityProbability;
        if (distance == 0)
        {
            cityProbability = 0f;
        }
        else
        {
            cityPreviousProbability = currentCell.GetProbability(SettlementType.City);
            cityCalculation = distance / 25f;
            cityProbability = Mathf.Min(0.5f, cityCalculation) * cityPreviousProbability;
        }
        currentCell.SetProbability(SettlementType.City, cityProbability);

        float townPreviousProbability;
        float townCalculation;
        float townProbability;
        if (distance == 0) {
            townProbability = 0f;
        }
        else
        {
            townPreviousProbability = currentCell.GetProbability(SettlementType.Town);
            if (distance < 10) townCalculation = 3f / distance;
                else townCalculation = 0f;
            townProbability = Mathf.Max(Mathf.Min(1f, townCalculation), townPreviousProbability);
        }
        currentCell.SetProbability(SettlementType.Town, townProbability);

        float ruralPreviousProbability;
        float ruralCalculation;
        float ruralProbability;
        if (distance == 0) {
            ruralProbability = 0f;
        }
        else {
            ruralPreviousProbability = currentCell.GetProbability(SettlementType.Rural);
            ruralCalculation = distance / 25f;
            ruralProbability = Mathf.Min(Mathf.Min(1f, ruralCalculation), ruralPreviousProbability);
        }
        currentCell.SetProbability(SettlementType.Rural, ruralProbability);
    }

    private void CalculateTownProbability(Cell currentCell, float distance)
    {
        float townPreviousProbability;
        float townCalculation;
        float townProbability;
        if (distance == 0) {
            townProbability = 0f;
        }
        else
        {
            townPreviousProbability = currentCell.GetProbability(SettlementType.Town);
            townCalculation = distance / 3f;
            townProbability = Mathf.Min(Mathf.Min(1f, townCalculation), townPreviousProbability);
        }
        currentCell.SetProbability(SettlementType.Town, townProbability);

        float ruralPreviousProbability;
        float ruralCalculation;
        float ruralProbability;
        if (distance == 0) {
            ruralProbability = 0f;
        }
        else {
            ruralPreviousProbability = currentCell.GetProbability(SettlementType.Rural);
            ruralCalculation = distance / 10f;
            ruralProbability = Mathf.Min(Mathf.Min(1f, ruralCalculation), ruralPreviousProbability);
        }
        currentCell.SetProbability(SettlementType.Rural, ruralProbability);
    }

    private void CalculateRuralProbability(Cell currentCell, float distance)
    {
        float townPreviousProbability;
        float townCalculation;
        float townProbability;
        if (distance == 0) {
            townProbability = 0f;
        }
        else {
            townPreviousProbability = currentCell.GetProbability(SettlementType.Rural);
            townCalculation = distance / 5f;
            townProbability = Mathf.Min(Mathf.Min(1f, townCalculation), townPreviousProbability);
        }
        currentCell.SetProbability(SettlementType.Rural, townProbability);

        float ruralPreviousProbability;
        float ruralCalculation;
        float ruralProbability;
        if (distance == 0) {
            ruralProbability = 0f;
        }
        else {
            ruralPreviousProbability = currentCell.GetProbability(SettlementType.Rural);
            ruralCalculation = distance / 5f;
            ruralProbability = Mathf.Min(Mathf.Min(1f, ruralCalculation), ruralPreviousProbability);
        }
        currentCell.SetProbability(SettlementType.Rural, ruralProbability);
    }
}