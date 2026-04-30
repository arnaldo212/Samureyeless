using UnityEngine;

public class BossHitBox : MonoBehaviour
{
    public float damage = 20f;
    private BoxCollider2D col;
    private BossController boss;

    private void Awake() {
        col = GetComponent<BoxCollider2D>();
        boss = GetComponentInParent<BossController>();
        DisableHitbox();
    }

    public void EnableHitbox() {
        col.enabled = true;
    }

    public void DisableHitbox() {
        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // ignora colisão com o próprio boss
        if (other.transform.IsChildOf(boss.transform) || other.gameObject == boss.gameObject) return;

        Debug.Log("Trigger com: " + other.gameObject.name);
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            DisableHitbox(); //evita acertar mais de uma vez por ataque
        }
        else
        {
            Debug.Log("PlayerHealth não encontrado em: " + other.gameObject.name);
        }
    }
}