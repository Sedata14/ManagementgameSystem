using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FactoryStock : StockSystem
{

    [SerializeField] private ItemFactory itemFactory;


    public void Awake()
    {
        itemFactory = GetComponent<ItemFactory>();
        if (isInitated)
        {
            InitializeStock(itemFactory.AllFactoryItems.Count);
            InitalizeItemRequirement();
            isInitated = false;
        }


        itemFactory.StartCoroutine(itemFactory.ProductionCycle());
    }


    private void OnEnable()
    {
        itemFactory.OnItemValidated += ValidateItem;
    }


    private void OnDisable()
    {
        itemFactory.OnItemValidated -= ValidateItem;
    }


    // initialize chaque slot qui va stocker les ressources dont à besoin chaque produit 
    private void InitalizeItemRequirement()
    {
        for (int i = 0; i < CurrentStock.Count; i++)
        {
            if (CurrentStock[i] == null)
            {
                ItemData itemData = itemFactory.ProducibleItems[i].ItemData;


                Slot slot = CreateSlot(itemData);

                //Debug.Log($"Est ce que ce slot est vide ?? {CurrentStock[i]}");

                CurrentStock[i] = slot;
                CurrentStock[i].HiddenSlot = true;
                CurrentStock[i].StockedQuantity = 0;


                itemData.id = ItemData.ItemIds;
                ItemData.ItemIds++;
                CurrentStock[i].ProductIdHandler = itemData.id;

                Debug.Log($"Nom : {itemData._Name} | ProductIdHandler{CurrentStock[i].ProductIdHandler} | itemData.Id {itemData.id}");
                for (int j = 0; j < itemData._RessourcesNeeded.Count; j++)
                {
                    CurrentStock[i].RessourceRequirementSlot.Add(itemData.id, CreateSlot(itemData._RessourcesNeeded[i].itemData));
                }
                slotManager.DisplayAllSlots();

            }
        }
    }


    /*  public override void AddItem()
      {



          /*
          //Debug.Log($"le slot actuel {CurrentStock[i]._product.Name} a le même nom que {product.Name}");


          if (ValueToAdd > 0)
          {
              //Debug.Log($"On peut ajouter {product.Name}");




              CurrentStock[i].PendingStock += ValueToAdd;
              CurrentStock[i].ProductIdHandler = product.id;
              product.Quantity -= ValueToAdd;

              //Debug.Log($"Slot numéro : {i} à comme value : {CurrentStock[i]._product.Name} et pending de : {CurrentStock[i].PendingStock} ");



              // on update direct pr afficher en transparent pr signier que ça arrive
              slotManager.DisplayAllSlots();

              if (ValueToAdd > 0 && productFactory && !CurrentStock[i].HiddenSlot)
              {
                  //StartCoroutine(productFactory.DisplayProductionCountdown(product));

              }


              if (product.Quantity <= 0 && !productFactory)
              {
                  //Debug.Log($"{product.Name} n'a plus rien, on supprime donc");
                  break;
              }

          }

          // snn on vérif si y a un slot qui est null
          for (int i = 0; i < CurrentStock.Count; i++)
          {
              if (CurrentStock[i] == null && product.Quantity > 0)
              {
                  Slot slot = CreateSlot(product);

                  //Debug.Log($"Est ce que ce slot est vide ?? {CurrentStock[i]}");

                  CurrentStock[i] = slot;
                  CurrentStock[i].HiddenSlot = true;
                  CurrentStock[i].StockedQuantity = 0;

                  int ValueToAdd = Mathf.Min(product.Quantity, QuantityLimit - (CurrentStock[i].StockedQuantity + CurrentStock[i].PendingStock));
                  CurrentStock[i].PendingStock = ValueToAdd;

                  CurrentStock[i].ProductIdHandler = product.id;
                  slotManager.DisplayAllSlots();

                  //Debug.Log($"Ce slot vide a désormais ce produit : {slot._product.Name}");
                  if (productFactory)
                  {
                      //StartCoroutine(productFactory.DisplayProductionCountdown(product));
                  }


                  //Debug.Log($"On ajoute temporairement {slot._product.Name} avec une valeur temporaire égale à {slot.PendingStock} et un id : {slot.ProductIdHandler}");
                  break;
              }
          }

      }
          */

    public override void RemoveItem(int index, int Amount, ItemData itemData)
    {
        if (CurrentStock[index] != null)
        {

            CurrentStock[index].StockedQuantity -= Amount;
            //Debug.Log($"{CurrentStock[index]._product.Name} a un stock égale à {CurrentStock[index].StockedQuantity}");


            // si la qt = 0 et y a déjà un prod qui est entrain d'être prod exactement le mm que celui dans ce slot
            // on évite de rendre null car si y a un autre prod qui se fini plus vite bah il va lui prendre sa place
            if (CurrentStock[index].StockedQuantity <= 0 && !itemFactory.CurrentProducedItem.Any(p => p._Name == itemData._Name))
            {

                Debug.Log($"[REMOVE] Tentative de suppression de {itemData._Name} | En prod ? {itemFactory.CurrentProducedItem.Any(p => p._Name == itemData._Name)}");

                //Debug.Log($"Y a plus de stock donc on supprime {CurrentStock[index]._product.Name}");
                CurrentStock[index] = null;

            }

            slotManager.DisplayAllSlots();
        }
    }
}

   
