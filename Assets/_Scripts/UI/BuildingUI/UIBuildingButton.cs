using UnityEngine;

public class UIBuildingButton : MonoBehaviour
{
    [SerializeField] private BuildingState buildingState;

    [SerializeField] private Building TargetBuilding;
    public void ChangeBuildingState()
    {
        if(TargetBuilding.CurrentBuildingState != BuildingState.BuildingUp)
        {
            TargetBuilding.UpdateBuldingState(buildingState);

        }

    }
}
