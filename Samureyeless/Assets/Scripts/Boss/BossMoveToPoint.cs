using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMoveToPoint : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Tempo")]
    [SerializeField] private float timeForMove = 2.5f;

    [Header("Chão mínimo")]
    [SerializeField] private float minimumYGround = 0f;

    [Header("Config")]
    [SerializeField] private float horizontalStopDistance = 0.1f;

    private bool movementCompleted;

    private Coroutine currentMove;

    public void Move(List<GameObject> pointsToCheck)
    {
        if (pointsToCheck == null || pointsToCheck.Count == 0)
            return;

        GameObject chosenPoint = null;

        float minDistance = Mathf.Infinity;

        // Ignora pontos abaixo do chão
        foreach (var point in pointsToCheck)
        {
            if (point.transform.position.y < minimumYGround)
                continue;

            float distance = Vector2.Distance(
                transform.position,
                point.transform.position
            );

            if (distance < minDistance)
            {
                minDistance = distance;
                chosenPoint = point;
            }
        }

        if (chosenPoint == null)
            return;

        //checa se tem movimento atual desse script, interrompe
        if (currentMove != null)
            StopCoroutine(currentMove);

        //chama corrotina de movimento
        currentMove = StartCoroutine(
            MoveRoutine(chosenPoint.transform)
        );
    }

    private IEnumerator MoveRoutine(Transform target)
    {
        // Movimento é dito concluido se um trigger pequeno dentro do boss 
        // entra em contato com o ponto desejado

        movementCompleted = false;

        float horizontalTime = timeForMove * 0.8f;
        float jumpTime = timeForMove * 0.2f;

        // Movimento Horizontal, primeiro obtem posição do alvo e atual
        // transform solto indica o transform DESSE objeto com o script
        float startX = transform.position.x; 
        float targetX = target.position.x;

        // fazer .magnitude funciona também
        // Vector3 heading = transform.position - target.position
        // float distance = heading.magnitude -> mesma coisa
        float distanceX = Mathf.Abs(targetX - startX);

        // velocidade constante mesmo
        float horizontalSpeed = distanceX / horizontalTime;

        while (
            Mathf.Abs(transform.position.x - targetX)
            > horizontalStopDistance
        )
        {
            float direction =
                Mathf.Sign(targetX - transform.position.x);

            rb.linearVelocity = new Vector2(
                direction * horizontalSpeed,
                rb.linearVelocity.y
            );

            yield return null;
        }

        // Para movimento horizontal
        rb.linearVelocity = new Vector2(
            0,
            rb.linearVelocity.y
        );

        // 2. SALTO (SE NECESSÁRIO)

        float deltaY =
            target.position.y - transform.position.y;

        if (deltaY > 0)
        {
            // Fórmula baseada em física:
            // v0 = (deltaY + 0.5 * g * t²) / t

            float gravity = Mathf.Abs(
                Physics2D.gravity.y * rb.gravityScale
            );

            float jumpVelocity =
                (deltaY + 0.5f * gravity * jumpTime * jumpTime)
                / jumpTime;

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpVelocity
            );
        }

        // 3. ESPERA TRIGGER CONFIRMAR CHEGADA
    
        while (!movementCompleted)
        {
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
    }
    
    // Trigger interno detectou ponto

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MovePoint"))
        {
            movementCompleted = true;
        }
    }
}