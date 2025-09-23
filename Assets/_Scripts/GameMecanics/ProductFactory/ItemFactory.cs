using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using NaughtyAttributes;

public class ItemFactory : MonoBehaviour
{
    [Foldout("ItemFactory System")]
    // elle permet de créer des ProducibleItem
    public List<ItemData> AllFactoryItems = new List<ItemData>();

    // tt les prod qui peuvent être produit
    public List<ProducibleItem> ProducibleItems = new List<ProducibleItem>();

    [SerializeField] private FactoryStock factoryStock;


    public bool isCurrentlyIteming = true;

    private float TimeToProduce = 1f;

    [SerializeField] private bool isCoroutineRunning = false;


    private Dictionary<ItemData, Queue<IEnumerator>> ProduceQueue = new Dictionary<ItemData, Queue<IEnumerator>>();

    public List<ItemData> CurrentProducedItem = new List<ItemData>();



    public GameObject ItemProgressDisplay;
    public GameObject ItemProgressDisplayParent;

    public event Action<ItemData> OnItemValidated;

    [Header("Current State")]
    public ItemFactoryState CurrentFactoryState;


    private void Awake()
    {

        for (int i = 0; i < AllFactoryItems.Count; i++)
        {
            ProducibleItem producibleItem = new ProducibleItem(AllFactoryItems[i], false);
            ProducibleItems.Add(producibleItem);
        }
        //Debug.Log(ProducibleItems.Count);
        factoryStock = GetComponent<FactoryStock>();

    }


    #region Enable/Disable
    private void OnEnable()
    {
        factoryStock.AddedAnItemToStock += HandleItemionCycleCoroutines;
    }

    private void OnDisable()
    {
        factoryStock.AddedAnItemToStock -= HandleItemionCycleCoroutines;
    }
    #endregion
    public IEnumerator Produce(ItemData ItemData)
    {


        factoryStock.ItemWaitingTobeAdded.Enqueue(ItemData);

        factoryStock.AddItem();


        //StartCoroutine(DisplayItemionCountdown(ItemData));


        CurrentFactoryState = ItemFactoryState.Producing;
        yield return new WaitForSeconds(ItemData._TimeToBeProduced);

        //Debug.Log("On ajoute dans la queue " + Item.Name);

        //Debug.Log($"Taille de la queue {factoryStock.ItemWaitingTobeAdded.Count} lors de l'ajout de {Item.Name}");

        CurrentProducedItem.Remove(ItemData);

        OnItemValidated?.Invoke(ItemData);

        UnlockManager.Instance.AddToProducedList(ItemData);


        if (ProduceQueue.ContainsKey(ItemData))
        {
            //Debug.Log($"hmmm produceQueue contient {ItemData._Name}, intéressant ");
        }
        ProduceQueue.Remove(ItemData);


    }


    public void HandleItemionCycleCoroutines()
    {
        if (!isCoroutineRunning)
        {
            StartCoroutine(ItemionCycleWrapper());
        }
    }

    private IEnumerator ItemionCycleWrapper()
    {
        isCoroutineRunning = true;
        //Debug.Log("Lance la Item tmr");
        yield return StartCoroutine(ProductionCycle());
        isCoroutineRunning = false;
    }

    public IEnumerator ProductionCycle()
    {
        Debug.Log("tu veux bien produire tonton ?");
        // elle permet de vérifier si il y a pas de rssources dispo
        var TempItemList = new List<ItemData>();


        isCurrentlyIteming = true;

        if (factoryStock.CheckIfStockIsFull())
        {
            CurrentFactoryState = ItemFactoryState.Full;
        }
        Debug.Log("Juste un petit recheck en rapide");

        // tant qu'on produit et que le stock n'est pas full
        while (isCurrentlyIteming && !factoryStock.CheckIfStockIsFull())
        {
            TempItemList.Clear();

            CurrentFactoryState = ItemFactoryState.Producing;

            for (int i = 0; i < ProducibleItems.Count; i++)
            {
                //Debug.Log(UnlockManager.Instance);
                if (UnlockManager.Instance.IsItemUnlocked(ProducibleItems[i].ItemData) && ProducibleItems[i].CanProduce)
                {

                    if (ProducibleItems[i].ItemData._RessourcesNeeded.Count == 0)
                    {
                        //Debug.Log("Le produit n'a pas besoin de ressources, on produit donc");

                        StartProduction(ProducibleItems[i].ItemData);
                    }
                    else
                    {
                        //Debug.Log($"{ProducibleItems[i].ItemData} a t-il toute les ressources ?");
                        if (HasAllResources(ProducibleItems[i].ItemData))
                        {
                            //Debug.Log($"{ProducibleItems[i].ItemData._Name} à toute les ressources !!");
                            //Debug.Log("Sex ???");
                            StartProduction(ProducibleItems[i].ItemData);

                        }

                        else
                        {

                            TempItemList.Add(ProducibleItems[i].ItemData);

                        }
                    }
                    //Debug.Log($"{ProducibleItems[i].ItemData._Name}. Item needed count : {ProducibleItems[i].ItemData.RessourcesNeeded.Count} ");


                }

                else
                {
                    //Debug.Log($" {ProducibleItems[i].ItemData._Name} est pas encore unlock dcp on verif si on peut le unlock");
                    UnlockManager.Instance.UnlockItem(ProducibleItems[i].ItemData);
                }
            }

            // si la liste est égale à ProducibleItems ça veut dire qu'on a besoin de stock chef
            if (TempItemList.Count == ProducibleItems.Count)
            {
                isCurrentlyIteming = false;
                CurrentFactoryState = ItemFactoryState.WaitingForStock;
                yield break;
            }


            yield return null;
        }

        if (factoryStock.CheckIfStockIsFull())
        {
            CurrentFactoryState = ItemFactoryState.Full;
        }
    }


    public void StartProduction(ItemData Item)
    {
        if (CurrentProducedItem.Contains(Item))
        {
            if (!ProduceQueue.ContainsKey(Item))
            {
                //Debug.Log($"Il n'a pas de Produce Queue pour {Item._Name}");
                ProduceQueue[Item] = new Queue<IEnumerator>();
            }
            //Debug.Log($"Il y a un Produce Queue pour {Item._Name}");
            ProduceQueue[Item].Enqueue(Produce(Item));

            //Debug.Log($" {Item._Name} est déjà en prod dcp en attend");
        }

        else
        {

            CurrentProducedItem.Add(Item);
            StartCoroutine(Produce(Item));


            //Debug.Log($" {Item._Name} Il est pas en prod dcp on lance HAHHA");
        }

    }


    public IEnumerator DisplayItemionCountdown(ItemData ItemData)
    {
        //Debug.Log("On dit Test mais belek");
        
        // on créer un la barre de progression
        var _ItemProgressDisplay = Instantiate(ItemProgressDisplay, ItemProgressDisplayParent.transform);
        float TimeRemaining = ItemData._TimeToBeProduced;

        // la barre à un slider
        Slider slider = _ItemProgressDisplay.GetComponent<Slider>();
        slider.maxValue = ItemData._TimeToBeProduced;
        slider.value = TimeRemaining;


        _ItemProgressDisplay.GetComponentInChildren<Image>().sprite = ItemData._Icon;

        while (TimeRemaining > 0)
        {

            TimeRemaining -= Time.deltaTime;
            slider.value = TimeRemaining;
            yield return null;  // attend la prochaine frame
        }

        Destroy(_ItemProgressDisplay.gameObject);
    }

    public bool HasAllResources(ItemData ItemData)
    {
        foreach (var RessourceNeeded in ItemData._RessourcesNeeded)
        {
            bool found = false;

            foreach (var Ressource in factoryStock.CurrentStock.Values)
            {

                if (Ressource != null && !Ressource.HiddenSlot)
                {
                    //Debug.Log($"Il ce trouve que le slot n'est ni null ni masqué, peut être que {ItemData._Name} aussi Debug en rapide Ressource stp : {Ressource._Item.Name}");

                    if (Ressource.itemData._Name == RessourceNeeded.itemData._Name
                        && Ressource.StockedQuantity >= RessourceNeeded.NeededQuantity
                        && factoryStock.CheckIfItemCanBeProduced(ItemData)
                        && !CurrentProducedItem.Contains(ItemData)
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

        // Si on arrive ici, toutes les ressources sont dispo
        ConsumeAllRessources(ItemData);
        return true;
    }

    public void ConsumeAllRessources(ItemData ItemData)
    {
        foreach (var RessourceNeeded in ItemData._RessourcesNeeded)
        {

            for (int t = 0; t < factoryStock.CurrentStock.Count; t++)
            {
                if (factoryStock.CurrentStock[t] != null
                    && !factoryStock.CurrentStock[t].HiddenSlot
                    && factoryStock.CurrentStock[t].itemData._Name == RessourceNeeded.itemData._Name
                    && factoryStock.CurrentStock[t].StockedQuantity >= RessourceNeeded.NeededQuantity
                    )
                {
                    //Debug.Log($"{ItemData._Name} enlève {RessourceNeeded.NeededQuantity} à {factoryStock.CurrentStock[t]._Item.Name} qui a un stock de {factoryStock.CurrentStock[t].StockedQuantity} et qui aura desormais {factoryStock.CurrentStock[t].StockedQuantity - RessourceNeeded.NeededQuantity }");


                    ItemData RessourceData = ProducibleItems.Find(
                        p => p.ItemData._Name == factoryStock.CurrentStock[t].itemData._Name)
                        ?.ItemData;
                    factoryStock.RemoveItem(t, RessourceNeeded.NeededQuantity, RessourceData);
                    break;

                }
            }
        }
    }
}
public enum ItemFactoryState
{
    Producing,
    WaitingForStock,
    Full
}

[System.Serializable]
public class ProducibleItem
{
    public ItemData ItemData;
    public bool CanProduce;

    public ProducibleItem(ItemData _ItemData, bool _CanBeProduce)
    {
        ItemData = _ItemData;
        CanProduce = _CanBeProduce;
    }
}