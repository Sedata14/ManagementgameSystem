using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using NaughtyAttributes;
  
public class Building : StockSystem
{
    [Foldout("Building")]
    [Header("Building Data")]
    [SerializeField] private BuildingData buildingData;
    private string _name;
    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _name = "Constellation";
            }
            else
            {
                _name = value;
            }
        }
    }

    [Foldout("Building")]
    [Header("Building Information")]
    public string Description;

    [Foldout("Building")]
    public float ConstructionDuration;

    [Foldout("Building")]
    public int BuldingCostByDay;

    [Foldout("Building")]
    [Header("Building Construction Requirements")]
    [SerializeField] private List<Ressources> RequiredRessources = new List<Ressources>();

    [Foldout("Building")]
    [Header("Building State")]
    [SerializeField] protected bool isActive;

    [Foldout("Building")]
    public BuildingState CurrentBuildingState;


    private void Awake()
    {
        InitializeStock(RequiredRessources.Count);

        InitalizeItemRequirement();

        InitalizeBuildingInformation();

        UpdateBuldingState(BuildingState.BuildingUp);
 
    }
    public void OnEnable()
    {
        GameManager.DayCompleted += DeductFunds;
        AddedAnItemToStock += TryConstructionInitialization;
    }

    public void OnDisable()
    {
        GameManager.DayCompleted -= DeductFunds;
        AddedAnItemToStock -= TryConstructionInitialization;
    }


    public void InitalizeBuildingInformation()
    {
        Name = buildingData._Name;
        Description = buildingData._Description;
        ConstructionDuration = buildingData._ConstructionDuration;
        BuldingCostByDay = buildingData._BuldingCostByDay;

        //print(Name);
        //print(Description);
        //print(ConstructionDuration);
    }

    public override void RemoveItem(int index, int Amount, ItemData itemData)
    {
        if (CurrentStock[index] != null)
        {

            CurrentStock[index].StockedQuantity -= Amount;
            //Debug.Log($"{CurrentStock[index]._product.Name} a un stock égale à {CurrentStock[index].StockedQuantity}");


            // si la qt = 0 et y a déjà un prod qui est entrain d'être prod exactement le mm que celui dans ce slot
            // on évite de rendre null car si y a un autre prod qui se fini plus vite bah il va lui prendre sa place
            if (CurrentStock[index].StockedQuantity <= 0)
            {

                Debug.Log($"[REMOVE] Tentative de suppression de {itemData._Name}");

                //Debug.Log($"Y a plus de stock donc on supprime {CurrentStock[index]._product.Name}");
                CurrentStock[index] = null;

            }

            slotManager.DisplayAllSlots();
        }
    }



    private void InitalizeItemRequirement()
    {
        for (int i = 0; i < RequiredRessources.Count; i++)
        {
            if (CurrentStock[i] == null)
            {
                ItemData itemData = RequiredRessources[i].itemData;


                Slot slot = CreateSlot(itemData);

                //Debug.Log($"Est ce que ce slot est vide ?? {CurrentStock[i]}");

                CurrentStock[i] = slot;
                CurrentStock[i].HiddenSlot = false;
                CurrentStock[i].StockedQuantity = 0;


                itemData.id = ItemData.ItemIds;
                ItemData.ItemIds++;
                CurrentStock[i].ProductIdHandler = itemData.id;


                for (int j = 0; j < itemData._RessourcesNeeded.Count; j++)
                {
                    CurrentStock[i].RessourceRequirementSlot.Add(itemData.id, CreateSlot(itemData._RessourcesNeeded[i].itemData));
                }

                slotManager.DisplayAllSlots();

            }
        }
    }


    public void TryConstructionInitialization()
    {
        StartCoroutine(InstantiateBuilding());
    }
    
    public IEnumerator InstantiateBuilding()
    {

        if (HasConstructionRessources())
        {
            //Debug.Log("Le batiment est prêt à ETRE CONSTRUIT HAHAHAHAHAHAHAHAHA");
            //Debug.Log($"Pr construire il faut attendre {ConstructionDuration}");
            yield return new WaitForSeconds(ConstructionDuration);
            UpdateBuldingState(BuildingState.Operating);
            //Debug.Log("Le batiment est construit ! ");
            ConsumeConstructionResources();

        }
        else
        {
            //Debug.Log("Sadly the building is not ready");
        }
    }



    public IEnumerator DestroyBuilding()
    {
        yield return new WaitForSeconds(ConstructionDuration % 50);
        Destroy(transform.gameObject);
    }







    #region Construction Ressources Handler
    public bool HasConstructionRessources()
    {
        if(RequiredRessources.Count > 0)
        {
            foreach (var RessourceNeeded in RequiredRessources)
            {
                bool found = false;

                foreach (var Ressource in CurrentStock.Values)
                {

                    if (Ressource != null && !Ressource.HiddenSlot)
                    {
                        //Debug.Log($"Il ce trouve que le slot n'est ni null ni masqué, peut être que {productData._Name} aussi Debug en rapide Ressource stp : {Ressource._product.Name}");

                        if (Ressource.itemData._Name == RessourceNeeded.itemData._Name
                            && Ressource.StockedQuantity >= RessourceNeeded.NeededQuantity
                            )
                        {
                            //Debug.Log("Il a toute les ressources, GOOOO");
                            found = true;
                            break; // On a trouvé la ressource, pas besoin de continuer sur les autres slots
                        }
                    }
                }

                if (!found)
                {
                    return false; // Une ressource manquante → on peut pas produire
                }
            }

            return true;

        }
        else
        {
            return true;
        }


    }

    public void ConsumeConstructionResources()
    {
        foreach (var RessourceNeeded in RequiredRessources)
        {

            for (int t = 0; t < CurrentStock.Count; t++)
            {
                if (CurrentStock[t] != null
                    && !CurrentStock[t].HiddenSlot
                    && CurrentStock[t].itemData._Name == RessourceNeeded.itemData._Name
                    && CurrentStock[t].StockedQuantity >= RessourceNeeded.NeededQuantity
                    )
                {
                    //Debug.Log($"{productData._Name} enlève {RessourceNeeded.NeededQuantity} à {RessourcesStock.CurrentStock[t]._product.Name} qui a un stock de {RessourcesStock.CurrentStock[t].StockedQuantity} et qui aura desormais {RessourcesStock.CurrentStock[t].StockedQuantity - RessourceNeeded.NeededQuantity }");


                    ItemData ConsumeValue = RequiredRessources.Find(
                        p => p.itemData._Name == CurrentStock[t].itemData._Name)
                        ?.itemData;

                    RemoveItem(t, RessourceNeeded.NeededQuantity, ConsumeValue);
                    break;

                }
            }
        }
    }
    #endregion


    public void UpdateBuldingState(BuildingState state)
    {
        CurrentBuildingState = state;
        switch (state)
        {
            case BuildingState.BuildingUp:
                TryConstructionInitialization();
                break;
            case BuildingState.Operating:
                isActive = true;
                break;
            case BuildingState.Paused:
                isActive = false;
                break;
            case BuildingState.Upgrading:
                isActive = false;
                break;
            case BuildingState.BeingDestroyed:
                StartCoroutine(DestroyBuilding());
                isActive = false;
                break;
            default:
                break;
        }

    }

    public void DeductFunds()
    {
        GameManager.CurrentMoney -= BuldingCostByDay;
    }
}



public enum BuildingState
{
    BuildingUp,
    Operating,
    Paused,
    Upgrading,
    BeingDestroyed
}