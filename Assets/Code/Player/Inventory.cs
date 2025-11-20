using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int coins = 0;
    [SerializeField] private TextMeshProUGUI coinsText;


    private void UpdateInventoryUI()
    {
        coinsText.text = coins.ToString() + "$";
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
}
