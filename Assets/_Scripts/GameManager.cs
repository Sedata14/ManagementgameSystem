using System;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public float Hour;

    [SerializeField] private float Tick = 0.02f;

    public static event Action DayCompleted;

    public static int CurrentMoney = 250;

    public static GameManager instance;

    public TextMeshProUGUI CurrentMoneyText;

    public void Awake()
    {
        CurrentMoneyText.text = CurrentMoney.ToString();

        if (instance == null)
        {
            instance = this;
        }
    }
    public void Update()
    {
        Hour += Time.deltaTime * Tick;

        if(Hour >= 24)
        {
            Debug.Log("oh 24h ?");
            Hour = 0;
            DayCompleted?.Invoke();
            CurrentMoneyText.text = CurrentMoney.ToString();

        }

    }


}
