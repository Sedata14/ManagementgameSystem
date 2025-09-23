using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using NaughtyAttributes;

public abstract class StockSystem : MonoBehaviour
{
    [Foldout("Stock System")]
    public Dictionary<int, Slot> CurrentStock = new Dictionary<int, Slot>();

    [Foldout("Stock System")]
    public int QuantityLimit;

    [Foldout("Stock System")]
    public bool isInitated = true;

    [Foldout("Stock System")]
    public int StockageSize;


    public event Action AddedAnItemToStock;

    [Foldout("Stock System")]
    [Header("UiSlots")]
    public SlotManager slotManager;



    [Foldout("Stock System")]
    [SerializeField] private bool isCurrentlyAdding;

    [Foldout("Stock System")]
    [Header("Stock States")]
    public StockState CurrentStockState;

    public Queue<ItemData> ItemWaitingTobeAdded = new Queue<ItemData>();

    protected void InvokeAddItemEvent()
    {
        AddedAnItemToStock?.Invoke();
    }

    protected void InitializeStock(int _StockageSize)
    {

        for (int i = 0; i < _StockageSize; i++)
        {
            CurrentStock[i] = null;

            slotManager.InitializeUISlots();
            slotManager.DisplayAllSlots();

        }
        StockageSize = _StockageSize;
    }


    public virtual void AddItem()
    {
        Debug.Log("On ajoute");

        ItemData itemData = ItemWaitingTobeAdded.Dequeue();

        int ItemQuantity = itemData._Quantity;

        //Debug.Log($"On va tenter d'ajouter {product.Name}");

        // on vérifie d'abord si on peut ajouter un produit déjà présent
        for (int i = 0; i < CurrentStock.Count; i++)
        {
            //Debug.Log(CurrentStock[i].RessourceRequirementSlot);
            if(CurrentStock[i].RessourceRequirementSlot != null)
            {
                for (int j = 0; j < CurrentStock[i].RessourceRequirementSlot.Count; j++)
                {
                    if (CurrentStock[i].RessourceRequirementSlot[j].itemData._Name == itemData._Name && CurrentStock[i].RessourceRequirementSlot[j].StockedQuantity + CurrentStock[i].RessourceRequirementSlot[j].PendingStock < QuantityLimit)
                    {
                        int ValueToAdd = Mathf.Min(ItemQuantity, QuantityLimit - (CurrentStock[i].StockedQuantity + CurrentStock[i].PendingStock));
                        CurrentStock[i].PendingStock += ValueToAdd;
                        CurrentStock[i].HiddenSlot = true;
                        ItemQuantity -= ValueToAdd;
                        slotManager.DisplayAllSlots();
                    }

                }
            }
            else
            {
                int ValueToAdd = Mathf.Min(ItemQuantity, QuantityLimit - (CurrentStock[i].StockedQuantity + CurrentStock[i].PendingStock));
                CurrentStock[i].PendingStock += ValueToAdd;
                CurrentStock[i].HiddenSlot = true;
                ItemQuantity -= ValueToAdd;
                slotManager.DisplayAllSlots();
                Debug.Log($"On a ajouter {itemData._Name}");


                if (ItemQuantity <= 0)
                {
                    return;
                }
            }

        }
    }


    public abstract void RemoveItem(int index, int Amount, ItemData itemData);


    public void ValidateItem(ItemData itemData)
    {
        // on cherche dans tt les slot l'id du product

        Debug.Log(StockageSize);
        for (int i = 0; i < StockageSize; i++)
        {
            // si y en a un alors on fait les calculs
            if (CurrentStock[i] != null && CurrentStock[i].ProductIdHandler == itemData.id)
            {
                Debug.Log($"Id trouvé de : {CurrentStock[i].ProductIdHandler} pour le produit : {itemData._Name} qui a un ID : {itemData.id} et au passage le positionnement dans l'inventaire : {i}");
                CurrentStock[i].HiddenSlot = false;
                CurrentStock[i].StockedQuantity += CurrentStock[i].PendingStock;
                CurrentStock[i].PendingStock = 0;

                //Debug.Log($"Slot numéro : {i} VALIDER, à comme value : {CurrentStock[i]._product.Name} et une quantité de {CurrentStock[i].StockedQuantity} ");

                //Debug.Log($"Le slot qui contient : {CurrentStock[i]._product.Name} est-il masqué ? {CurrentStock[i].HiddenSlot}");
                AddedAnItemToStock?.Invoke();
                slotManager.DisplayAllSlots();

            }
        }
    }



    public Slot CreateSlot(ItemData _itemData)
    {
        var slot = new Slot(_itemData);
        slot.itemData = _itemData;
        return slot;
    }


    public bool CheckIfItemCanBeProduced(ItemData itemData)
    {
        foreach(var key in CurrentStock.Keys)
        {
            if (CurrentStock[key] != null)
            {
                var slot = CurrentStock[key];
                int ValueToAdd = Mathf.Min(itemData._Quantity, QuantityLimit - (CurrentStock[key].StockedQuantity + CurrentStock[key].PendingStock));

                // on verif si c'est le mm produit et que le slot dcp ne dépasse pas la quantité maximale
                if (itemData._Name == slot.itemData._Name && ValueToAdd > 0)
                {
                    return true;
                }
            }
        }
        foreach (var key in CurrentStock.Keys)
        {
            if (CurrentStock[key] == null)
            {

                    return true;
            }

        }


        return false;
    }
    public bool CheckIfStockIsFull()
    {
        foreach (var key in CurrentStock.Keys)
        {
            var slot = CurrentStock[key];
            if (slot == null || slot.StockedQuantity < QuantityLimit)
            {
                //Debug.Log("Le stock n'est pas full");
                CurrentStockState = StockState.isNotFull;
                return false;  // Un slot est vide ou pas rempli à fond
            }
            //Debug.Log($"{slot.itemData._Name} et  {slot.StockedQuantity}");
        }

        //Debug.Log("Le stock est full");
        CurrentStockState = StockState.isFull;
        PrintDico();
        return true; // Tous les slots sont pleins
    }

    private void PrintDico()
    {
        foreach(var key in CurrentStock.Keys)
        {
            if(CurrentStock[key] != null)
            {
                //Debug.Log($"Clé numéro : {key} avec comme produit : {CurrentStock[key]._product.Name} d'une quantité de {CurrentStock[key].StockedQuantity} et un PendingStock {CurrentStock[key].PendingStock} ET AUSSI EST CE QUE CEST HIDDEN EUH {CurrentStock[key].HiddenSlot}");

            }

        }
    }

}

public class Slot
{


    public ItemData itemData;
    public int StockedQuantity;


    public bool HiddenSlot;
    public int PendingStock;

    public int ProductIdHandler;

    public Dictionary<int, Slot> RessourceRequirementSlot;

    public Slot(ItemData _itemData)
    {
        itemData = _itemData;
        StockedQuantity = 0;

    }
}

public enum StockState
{
    isFull,
    isNotFull
}

public enum ProductFilters
{
    Name,
    Quantity,

}