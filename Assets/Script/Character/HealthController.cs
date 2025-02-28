using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float maxHealth;

    [HideInInspector] public bool isDie = false;
    [HideInInspector] public bool isHurt = false;

    public HealthBar healthBar;
    private void Start()
    {
        if (healthBar != null)
        {
            healthBar.maxHealth=maxHealth;
            UpdateHealth();
        }
    }

    public void UpdateHealth()
    {
        //healthBar.value = currentHealth / maxHealth;


        healthBar.currentHealth = currentHealth;
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth - damage <= 0)
        {
            currentHealth = 0;
        }
        else
            currentHealth -= damage;
        if(healthBar != null) 
            UpdateHealth();
    }

    public void Heal(float heal)
    {
        if (currentHealth + heal > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
            currentHealth += heal;
        if (healthBar != null)
            UpdateHealth();
    }
}
