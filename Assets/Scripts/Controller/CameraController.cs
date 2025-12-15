using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset;

    private Transform target;
    private CameraModel cameraModel;

    void Start()
    {
        // Encontrar o primeiro objeto com a tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.transform;
            // Definir a posição inicial da câmera com base no offset
            transform.position = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
        }
        else
        {
            Debug.LogWarning("Nenhum objeto com a tag 'Player' encontrado.");
        }

        cameraModel = GetComponent<CameraModel>();
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            // Calcular a posição desejada da câmera (sem seguir o eixo Z do jogador)
            Vector3 targetPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);

            // Interpolar suavemente entre a posição atual da câmera e a posição desejada
            transform.position = Vector3.Lerp(transform.position, targetPosition, cameraModel.FollowSpeed * Time.deltaTime);
        }
        // transform.Rotate(0, 0, Mathf.Sin(Time.time * cameraModel.RotationSpeed) * cameraModel.RotationMultiplier);
    }
}
