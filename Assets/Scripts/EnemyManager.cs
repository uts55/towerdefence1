using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;     // 생성할 적 프리팹 (Inspector에서 연결)
    public float spawnInterval = 1.5f; // 적 생성 간격 (초)
    public float spawnDistance = 30f;  // 성(중심)으로부터 적이 생성될 거리

    void Start()
    {
        // 게임 시작 시 적 생성 코루틴 시작
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // 랜덤 각도 생성 (0 ~ 360도)
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector2 spawnDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector3 spawnPosition = spawnDirection * spawnDistance;

            // 적 프리팹 생성
            if (enemyPrefab != null)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                // 적의 방향 설정
                Transform castleTransform = GameObject.FindGameObjectWithTag("Castle")?.transform;
                if (castleTransform != null)
                {
                    Vector2 directionToCastle = (castleTransform.position - spawnPosition).normalized;
                    if (directionToCastle.x > 0)
                    {
                        enemy.transform.localScale = new Vector3(1, enemy.transform.localScale.y, enemy.transform.localScale.z); // 오른쪽
                    }
                    else
                    {
                        enemy.transform.localScale = new Vector3(-1, enemy.transform.localScale.y, enemy.transform.localScale.z); // 왼쪽
                    }
                }
            }
            else
            {
                Debug.LogError("Enemy Prefab이 EnemyManager에 할당되지 않았습니다!");
            }
        }
    }
}