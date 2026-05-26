using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{

    public EnemyData data;
    public GameObject healthBar;
    private ProgramInputManager inputManager;
    public CountElement healthNumber;

    void Start()
    {
        UpdateHealthBar();
        inputManager = FindObjectOfType<ProgramInputManager>();

        if(inputManager != null)
        {
            inputManager.OnSlowModeEnter += DisplayHealthNumber;
            inputManager.OnSlowModeExit += RemoveHealthNumber;
        }
    }

    public void DamageHealthBar(HitInfo hitInfo)
    {
        if (data.currentHealth - hitInfo.damage < 0)
        {
            data.currentHealth = 0;
        }
        else
        {
            data.currentHealth -= hitInfo.damage;
        }

        UpdateHealthBar();
    }

    public void HealHealthBar(int healing)
    {
        if (data.currentHealth + healing > data.totalHealth)
        {
            data.currentHealth = data.totalHealth;
        }
        else
        {
            data.currentHealth += healing;
        }

        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        float temp = (float) data.currentHealth / data.totalHealth;
        healthBar.transform.localScale = new Vector2(temp, healthBar.transform.localScale.y);
        healthNumber.UpdateNumber(data.currentHealth, data.totalHealth);
    }

    public void DisplayHealthNumber()
    {
        
    }

    public void RemoveHealthNumber()
    {
        
    }

}
