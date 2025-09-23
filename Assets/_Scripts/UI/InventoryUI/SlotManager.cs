
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
public class SlotManager : MonoBehaviour
{
    [SerializeField] private List<UiSlot> uiSlots = new List<UiSlot>();

    [SerializeField] private GameObject SlotPrefab;
    [SerializeField] private GameObject Slot_parent;

    [SerializeField] private StockSystem stockManager;


    [Header("For debug ")]
    [SerializeField] private ProductFilters debugFilter;
    [Header("Current Filter For All Factories")]
    public static ProductFilters CurrentProductFilter;

    // fonction qui trie le stock visuellement et qui affiche aussi dcp mdrr
    public void DisplayAllSlots()
    {
        CurrentProductFilter = debugFilter;
        var SortedSlots = stockManager.CurrentStock.Values.ToList();

        Debug.Log(SortedSlots);
        switch (CurrentProductFilter)
        {
            case ProductFilters.Name:
                SortedSlots = SortedSlots
                    .OrderBy(slot => slot == null)
                    .ThenBy(slot => slot?.itemData._Name)
                    .ToList();

                break;


            case ProductFilters.Quantity:
                Debug.Log("On fit par qantité");
                SortedSlots = SortedSlots
                    .OrderBy(slot => slot == null)
                    .ThenByDescending(slot => slot?.StockedQuantity)
                    .ToList();
                  
                break;

        }
        

        for (int i = 0; i < SortedSlots.Count; i++)
        {
            UpdateUiSlot(i, SortedSlots[i]);
        }
    }
    // permet de update chaque slot
    public void UpdateUiSlot(int PlaceInInventory, Slot slot)
    {
       
        if(slot == null)
        {
            uiSlots[PlaceInInventory].NameText.text = "";
            uiSlots[PlaceInInventory].ItemIcon.sprite = null;
            uiSlots[PlaceInInventory].QuantityText.text = "";

        }

        else
        {
            Debug.Log(slot);
            uiSlots[PlaceInInventory].NameText.text = slot.itemData._Name;
            uiSlots[PlaceInInventory].ItemIcon.sprite = slot.itemData._Icon;

            //Debug.Log(uiSlots[PlaceInInventory].ItemIcon);
            if (slot.StockedQuantity <= 0)
            {
                Debug.Log("La quantité est inférieur à 0 par conséquent, je réduis l'opacité");
                AdjustIconOpacity(uiSlots[PlaceInInventory].ItemIcon, 0.5f);
            }
            else
            {
                Debug.Log("gneugneu la quantité est sup à 0 dcp j'augm");
                AdjustIconOpacity(uiSlots[PlaceInInventory].ItemIcon, 1f);
            }

            string PendingStockDisplay = slot.PendingStock > 0 ? $"+ ({slot.PendingStock})" : "";
            uiSlots[PlaceInInventory].QuantityText.text = $"{slot.StockedQuantity} {PendingStockDisplay}";
        }
        

    }

    public void InitializeUISlots()
    {
        var Slot = Instantiate(SlotPrefab, Slot_parent.transform);
        uiSlots.Add(Slot.GetComponent<UiSlot>());
    }

    public void AdjustIconOpacity(Image image, float AlphaValue)
    {
        //Debug.Log($"On ajuste l'opacité bordel, la valeur c'est ça btw : {AlphaValue}");
        Color CurrentColor = image.color;
        Color newColor = new Color(CurrentColor.r, CurrentColor.g, CurrentColor.b, AlphaValue);
        image.color = newColor;
    }
}


