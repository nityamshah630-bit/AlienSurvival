using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public int health = 3;
    public int maxHealth = 3;
    public int itemCount = 0; // Tracks collected items
    public string[] inventory = new string[5]; // Simple inventory array (max 5 items)

    void Start()
    {
        Debug.Log("PlayerCharacter initialized with Health: " + health + ", Item Count: " + itemCount);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        Debug.Log("Player took " + damage + " damage. Remaining Health: " + health);
    }

    public void Heal(int healAmount)
    {
        health = Mathf.Min(health + healAmount, maxHealth);
        Debug.Log("Player healed by " + healAmount + ". Current Health: " + health);
    }

    public void AddItem(string itemName)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (string.IsNullOrEmpty(inventory[i]))
            {
                inventory[i] = itemName;
                itemCount++;
                Debug.Log("Added " + itemName + " to inventory. Total Items: " + itemCount);
                return;
            }
        }
        Debug.LogWarning("Inventory full! Cannot add " + itemName);
    }

    public void RemoveItem(string itemName)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == itemName)
            {
                inventory[i] = null;
                itemCount--;
                Debug.Log("Removed " + itemName + " from inventory. Total Items: " + itemCount);
                return;
            }
        }
        Debug.LogWarning("Item " + itemName + " not found in inventory!");
    }

    private void Die()
    {
        Debug.Log("Player died! Lives not tracked, triggering game over logic.");
        // Notify PlayerControl or a LevelManager to handle game over (e.g., show UI or reload)
        PlayerControl playerControl = GetComponent<PlayerControl>();
        if (playerControl != null)
        {
            playerControl.GameOver();
        }
        else
        {
            Debug.LogWarning("PlayerControl component not found! Manual game over handling needed.");
        }
    }

    // Method to check inventory (for debugging or UI)
    public void DisplayInventory()
    {
        string inventoryList = "Inventory: ";
        foreach (string item in inventory)
        {
            inventoryList += item ?? "Empty, ";
        }
        Debug.Log(inventoryList.TrimEnd(',', ' '));
    }
}