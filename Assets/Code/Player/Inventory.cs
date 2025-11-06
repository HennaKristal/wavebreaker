using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI metalText;

    private int coins = 0;
    private int woodDebris = 0;
    private int metalDebris = 0;


    private void UpdateInventoryUI()
    {
        coinsText.text = coins.ToString();
        woodText.text = woodDebris.ToString();
        metalText.text = metalDebris.ToString();
    }


    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateInventoryUI();
    }

    public void RemoveCoins(int amount)
    {
        coins -= amount;
        UpdateInventoryUI();
    }

    public bool HasCoins(int amount)
    {
        return coins >= amount;
    }



    public void AddWood(int amount)
    {
        woodDebris += amount;
        UpdateInventoryUI();
    }

    public void RemoveWood(int amount)
    {
        woodDebris -= amount;
        UpdateInventoryUI();
    }

    public bool HasWood(int amount)
    {
        return woodDebris >= amount;
    }



    public void AddMetal(int amount)
    {
        metalDebris += amount;
        UpdateInventoryUI();
    }

    public void RemoveMetal(int amount)
    {
        metalDebris -= amount;
        UpdateInventoryUI();
    }

    public bool HasMetal(int amount)
    {
        return metalDebris >= amount;
    }
}
