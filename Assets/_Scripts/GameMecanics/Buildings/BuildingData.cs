using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Building/BuiildingData")]
public class BuildingData : ScriptableObject
{

    public string _Name;


    public string _Description;

    public float _ConstructionDuration = 15f;

    public GameObject BuildingPrefab;

    public int _BuldingCostByDay;
}
