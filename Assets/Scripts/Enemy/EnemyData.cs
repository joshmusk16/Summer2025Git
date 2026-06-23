using UnityEngine;

public class EnemyData : MonoBehaviour
{
    public int totalHealth;
    public int currentHealth;

    private HurtBox hurtBox;
    public EnemyHealthBar health;

    void Awake()
    {
        hurtBox = gameObject.GetComponent<HurtBox>();

        if(health != null)
        {
            hurtBox.OnHit += RemoveHealth;
        }
    }

    public void AddHealth(int healing)
    {   
        if (currentHealth + healing > totalHealth)
        {
            currentHealth = totalHealth;
        }
        else
        {
            currentHealth += healing;
        }

        health.UpdateHealthBar();
    }


    public void RemoveHealth(HitInfo hitInfo)
    {   
        if (currentHealth - hitInfo.damage <= 0)
        {
            currentHealth = 0;
            Destroy(gameObject); //temporary line of code to represent death of enemy
        }
        else
        {
            currentHealth -= hitInfo.damage;
        }

        health.UpdateHealthBar();
    }

    void OnDestroy()
    {
        hurtBox.OnHit -= RemoveHealth;
    }

}
