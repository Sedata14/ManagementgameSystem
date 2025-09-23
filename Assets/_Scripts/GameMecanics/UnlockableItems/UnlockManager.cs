using UnityEngine;
using System.Collections.Generic;
public class UnlockManager : MonoBehaviour
{
    [SerializeField] private List<UnlockData> UnlockedItems = new List<UnlockData>();

    public List<UnlockData> CreatedItems = new List<UnlockData>();

    public static UnlockManager Instance;

    public void Awake()
    {

        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void UnlockItem(UnlockData unlockData)
    {

        if(unlockData.DependenciesRequirement.Count > 0)
        {
            bool CanBeUnlocked = true;
            foreach (var item in unlockData.DependenciesRequirement)
            {
                //Debug.Log($"{unlockData.name} a besoin que {item.name} soit débloquer : {IsItemUnlocked(item)} et au moins créer une fois : {CreatedItems.Contains(item)}");
                 
                if(!CreatedItems.Contains(item) || !item.isUnlocked)
                {

                    CanBeUnlocked = false;
                    break;
                }

                
            }
            //Debug.Log($"Est-ce que {unlockData.name} peut être unlock ? {CanBeUnlocked}");

            if (CanBeUnlocked && !UnlockedItems.Contains(unlockData))
            {
                unlockData.isUnlocked = true;
                UnlockedItems.Add(unlockData);
            }
        }
    }

    public bool IsItemUnlocked(UnlockData unlockData)
    {
        return UnlockedItems.Contains(unlockData);
    }

    public void AddToProducedList(UnlockData unlockData)
    {
        if (!CreatedItems.Contains(unlockData))
        {
            //Debug.Log($"{unlockData.name} vient d'être produit pr la premirère fois, gg dcp on ajoute lol");
            CreatedItems.Add(unlockData);
        }
    }

}
