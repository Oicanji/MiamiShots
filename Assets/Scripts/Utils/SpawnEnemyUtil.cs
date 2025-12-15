using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyUtil : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindFirstObjectByType<GameController>().enemies = GameObject.FindGameObjectsWithTag("Enemy");
        FindFirstObjectByType<GameController>().enemyCount = Mathf.Max(FindFirstObjectByType<GameController>().enemies.Length, 0);
    }
}
