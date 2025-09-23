using System.Collections.Generic;
using UnityEngine;

public class Product : Item
{
    public float TimeToBeProduce;

    public override int Quantity {
    get => base.Quantity;
    set 
    {
        if(value > 0)
        {
            _quantity = value;
        }
        else
        {
            _quantity = 0;
        }

    } 
    }

    [Header("Needed Ressources")]
    public List<Ressources> _RessourcesNeeded = new List<Ressources>();

    public Product(string _Name, string _Description, Sprite _icon, int _Quantity, float _price, float _TimeToBeProduce)
    : base(_Name, _Description, _icon, _Quantity, _price)
    {
        TimeToBeProduce = _TimeToBeProduce;
    }



}
