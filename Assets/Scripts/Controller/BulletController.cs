using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public BulletModel bulletModel;
    private Rigidbody2D rigidbodyBall;
    private BulletTrailManager trailManager;
    private bool isActive = true;

    void Start()
    {
        bulletModel = GetComponent<BulletModel>();
        rigidbodyBall = GetComponent<Rigidbody2D>();
        trailManager = GetComponent<BulletTrailManager>();

        // Definir direção e aplicar rotação
        Vector2 directionToMouse = GetDirectionToMouse();
        bulletModel.Direction = directionToMouse.normalized;
        RotateTowardsMouse(directionToMouse);

        // Aplicar impulso inicial
        ApplyInitialImpulse();
    }

    void Update()
    {
        if (isActive)
        {
            // Atualizar rastro enquanto a bala estiver ativa
            trailManager?.UpdateTrail();
        }
    }

    private Vector2 GetDirectionToMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return (mousePosition - transform.position).normalized;
    }

    private void RotateTowardsMouse(Vector2 directionToMouse)
    {
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private void ApplyInitialImpulse()
    {
        rigidbodyBall.AddForce(bulletModel.Direction * bulletModel.Speed, ForceMode2D.Impulse);
    }

    public void ReflectBullet(Collision2D collision)
    {
        if (!isActive) return;  // Se a bala estiver desativada, não refletir
        Vector2 reflectedDirection = Vector2.Reflect(bulletModel.Direction, collision.contacts[0].normal);
        bulletModel.Direction = reflectedDirection;
        rigidbodyBall.linearVelocity = bulletModel.Direction * bulletModel.Speed;

        float angle = Mathf.Atan2(bulletModel.Direction.y, bulletModel.Direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    // Desativar a bala e aguardar o fim do rastro
    public void DestroyAfterTrail()
    {
        // Desativar lógica física e visual da bala
        isActive = false;
        rigidbodyBall.linearVelocity = Vector2.zero; // Parar o movimento
        rigidbodyBall.isKinematic = true;      // Desativar física
        GetComponent<Collider2D>().enabled = false; // Desativar colisões
        GetComponent<SpriteRenderer>().enabled = false; // Esconder o sprite da bala

        // Iniciar corrotina para destruir após o rastro terminar
        StartCoroutine(WaitForTrailToFinishAndDestroy());
    }

    private IEnumerator WaitForTrailToFinishAndDestroy()
    {
        // Esperar até que o rastro acabe de ser desenhado
        yield return StartCoroutine(trailManager.WaitForTrailToFinish());

        // Destruir a bala após o rastro desaparecer
        Destroy(gameObject);
    }
}
