using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemData", menuName = "Item/ItemData")]
public class ItemData : UnlockData
{
    [SerializeField] private int _quantity = 2;


    public int _Quantity
    {
        get { return _quantity; }
        set
        {
            if (value >= 0)
            {
                _quantity = value;
            }
            else
            {
                _quantity = 0;
            }
        }
    }
    public string _Name;
    public string _Description;
    public Sprite _Icon;
    public float _Price;
    public float _TimeToBeProduced;
    public static int ItemIds = 1;
    public int id;
    [Header("Needed Ressources")]
    public List<Ressources> _RessourcesNeeded = new List<Ressources>();



}


[System.Serializable]
public class Ressources
{
    public ItemData itemData;
    public int NeededQuantity;

    
}
