using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
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

            GameObject enemyObject = ObjectPooler.Instance.SpawnFromPool("Enemy", spawnPosition, Quaternion.identity);

            if (enemyObject == null)
            {
                // 풀이 비었거나 태그 오류 처리 (예: 로그 남기기)
                Debug.LogError("Failed to spawn enemy from pool!");
            }

            // 적의 방향 설정
            Transform castleTransform = GameObject.FindGameObjectWithTag("Castle")?.transform;

            if (castleTransform != null)
            {
                Vector2 directionToCastle = (castleTransform.position - spawnPosition).normalized;
                if (directionToCastle.x > 0)
                {
                    enemyObject.transform.localScale = new Vector3(1, enemyObject.transform.localScale.y, enemyObject.transform.localScale.z); // 오른쪽
                }
                else
                {
                    enemyObject.transform.localScale = new Vector3(-1, enemyObject.transform.localScale.y, enemyObject.transform.localScale.z); // 왼쪽
                }
            }
        }
    }
}