using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BulletView : MonoBehaviour
{
    private BulletController bulletController;
    private int totalCollision;
    public Collider2D tilemapCollider; // Referência ao Collider do Tilemap

    void Start()
    {
        bulletController = GetComponent<BulletController>();

        // Configura o Collider secundário da bala para interagir apenas com o Tilemap
        if (tilemapCollider != null)
        {
            tilemapCollider.isTrigger = true; // Define o segundo Collider como trigger
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        totalCollision++;
        bulletController.bulletModel.Speed *= 1.4f; // TODO: Permitir ajuste de velocidade de fragmentação

        // Tocar efeito de fragmentação
        if (bulletController.bulletModel.FragmentEffect != null)
        {
            AudioSource.PlayClipAtPoint(bulletController.bulletModel.FragmentEffect, transform.position);
        }

        HandleCollision(collision);

        // Limite de fragmentação atingido, destruir a bala
        if (bulletController.bulletModel.LimitFragmentation <= totalCollision)
        {
            AudioSource.PlayClipAtPoint(bulletController.bulletModel.FragmentEndEffect, transform.position);
            bulletController.DestroyAfterTrail();
        }
    }

    void HandleCollision(Collision2D collision)
    {
        if (collision.gameObject == null) return;

        // Se colidir com outras superfícies, reflete
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall"))
        {
            bulletController.ReflectBullet(collision);
        }
    }

    // Colisão com o Tilemap (detectada pelo Collider trigger)
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("BreakableWall"))
        {
            Tilemap tilemap = collider.GetComponent<Tilemap>();
            if (tilemap != null)
            {
                // Usar a posição da bala para determinar onde remover o tile
                Vector3 contactPoint = transform.position;
                RemoveTilesAlongPath(tilemap, contactPoint);
            }
        }
    }

    // Método para remover tiles ao longo do caminho da bala
    void RemoveTilesAlongPath(Tilemap tilemap, Vector3 startPoint)
    {
        float distance = bulletController.bulletModel.Speed * Time.deltaTime; // Distância que a bala viaja por frame
        Vector2 direction = bulletController.bulletModel.Direction; // Direção da bala

        // Raycast ao longo da direção da bala
        RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint, direction, distance);

        Debug.DrawRay(startPoint, direction * distance, Color.red, 1.0f);
        Debug.Log($"Raycast hits: {hits.Length}");

        HashSet<Vector3Int> cellsToRemove = new HashSet<Vector3Int>();

        foreach (RaycastHit2D hit in hits)
        {
            // Verificar se o hit collidiu com um Tilemap
            if (hit.collider != null)
            {
                Vector3Int cellPosition = tilemap.WorldToCell(hit.point);

                // Adicionar a célula ao conjunto se ainda não estiver presente
                cellsToRemove.Add(cellPosition);

                // Log para verificação
                TileBase tile = tilemap.GetTile(cellPosition);
                Debug.Log($"Tile at {cellPosition} before removal: {tile}");
            }
        }

        // Remover todos os tiles nas células detectadas
        foreach (Vector3Int cell in cellsToRemove)
        {
            TileBase tile = tilemap.GetTile(cell);
            if (tile != null)
            {
                // Remover o tile na célula de contato
                tilemap.SetTile(cell, null);

                // Forçar atualização do Tilemap
                tilemap.RefreshTile(cell);

                Debug.Log($"Removed tile at: {cell}");

                // Log para verificar se o tile foi removido
                tile = tilemap.GetTile(cell);
                Debug.Log($"Tile at {cell} after removal: {tile}");
            }
        }
    }
}
