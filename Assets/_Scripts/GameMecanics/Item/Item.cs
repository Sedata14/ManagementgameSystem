using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class Item
{

    protected int _quantity;
    [Header("Main Informations")]
    public virtual int Quantity
    {
        get { return _quantity; }
        set
        {
            _quantity = 1; 
        }
    }
    public string Name;
    public string Description;
    public Sprite Icon;
    public float Price;


    private void Awake()
    {



    }
    public Item(string _Name, string _Description, Sprite _icon, int _Quantity, float _price)
    {
        Name = _Name;
        Description = _Description;
        Icon = _icon;
        Price = _price;
        _quantity = _Quantity;

    }

    public void AssignItemInformation(ItemData itemData)
    {
        Debug.Log("J'assigne les info chef mdrr");
        Quantity = itemData._Quantity;
        Name = itemData._Name;
        Description = itemData._Description;
        Icon = itemData._Icon;
        Price = itemData._Price;
        
    }

}
