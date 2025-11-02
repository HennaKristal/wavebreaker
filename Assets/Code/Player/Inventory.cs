using UnityEngine;

public class Inventory : MonoBehaviour
{
    private int coins = 0;
    private int materials = 0;


    public void AddCoins(int amount)
    {
        coins += amount;
    }

    public void RemoveCoins(int amount)
    {
        coins -= amount;
    }

    public bool HasCoins(int amount)
    {
        return coins >= amount;
    }


    public void AddMaterials(int amount)
    {
        materials += amount;
    }

    public void RemoveMaterials(int amount)
    {
        materials -= amount;
    }

    public bool HasMaterials(int amount)
    {
        return materials >= amount;
    }
}
