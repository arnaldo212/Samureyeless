using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Invencibilidade após tomar dano")]
    public float invincibleTime = 1f;
    public float invincibleTimer = 0f;
    public bool isInvincible = false;

    public static PlayerHealth Instance;

    private void Awake() {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start() {
        currentHealth = maxHealth;
    }

    private void Update() {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
                isInvincible = false;
        }
    }

    public void TakeDamage(float amount) {
        if (isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Jogador tomou dano! HP: " + currentHealth);

        //ativa invencibilidade temporaria
        isInvincible = true;
        invincibleTimer = invincibleTime;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die() {
        Debug.Log("Jogador morreu!");
        //chamar animação de morte
    }
}