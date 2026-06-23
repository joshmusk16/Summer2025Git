using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{

    public EnemyData data;
    public GameObject healthBar;
    private SpriteRenderer healthBarSprite;
    private ProgramInputManager inputManager;
    public CountElement healthNumber;

    private Color defaultColor = new Color(1,0,0,1);
    private Color instantKillColor = new Color(1,1,1,1); //health bar changes to this color when enemy can be on shot

    private const float VERTICAL_OFFSET = 10f;

    void Awake()
    {
        inputManager = FindObjectOfType<ProgramInputManager>();

        if(inputManager != null)
        {
            inputManager.OnSlowModeEnter += DisplayHealthNumber;
            inputManager.OnSlowModeExit += RemoveHealthNumber;
        }

        if(healthBar != null)
        {
            healthBarSprite = healthBar.GetComponent<SpriteRenderer>();
            healthBarSprite.color = defaultColor;
        }

        ComboBarUI.OnComboUpdate += UpdateHealthColor;
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        float temp = (float) data.currentHealth / data.totalHealth;
        healthBar.transform.localScale = new Vector2(temp, healthBar.transform.localScale.y);
        healthNumber.UpdateNumber(data.currentHealth, data.totalHealth, new Vector2(0, VERTICAL_OFFSET));
        UpdateHealthColor();
    }

    public void UpdateHealthColor()
    {
        if(data.currentHealth <= ComboBarLogic.currentCombo)
        {
            healthBarSprite.color = instantKillColor;
        }
        else
        {
            healthBarSprite.color = defaultColor;
        }
    }

    public void DisplayHealthNumber()
    {
        
    }

    public void RemoveHealthNumber()
    {
        
    }

    void OnDestroy()
    {
        ComboBarUI.OnComboUpdate -= UpdateHealthColor;
    }

}
