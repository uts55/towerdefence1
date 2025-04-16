using UnityEngine;

public class AutoCannon : MonoBehaviour
{
    public Transform firePoint;         // 발사체가 생성될 위치 (없으면 성 위치에서 발사)
    public float fireRate = 2f;         // 발사 속도 (초당 발사 횟수)
    public float attackRange = 8f;      // 공격 사거리

    private float fireCooldown = 0f;    // 다음 발사까지 남은 시간
    private Transform targetEnemy = null; // 현재 조준 중인 적

    public float baseDamage = 5f; // 발사체 기본 데미지 (Projectile.cs에서 가져와도 됨)
    public float baseFireRate = 2f; // 기본 발사 속도

    private float currentDamageMultiplier = 1f; // 현재 공격력 배율
    private float currentFireRateMultiplier = 1f; // 현재 발사 속도 배율

    void Update()
    {
        // 쿨다운 감소
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
        }

        // 쿨다운이 0 이하이고, 조준할 적이 없거나 죽었다면 새로운 적 탐색
        if (fireCooldown <= 0 && (targetEnemy == null || !targetEnemy.gameObject.activeInHierarchy)) // activeInHierarchy: 적이 파괴되었는지 확인
        {
            FindNearestEnemy();
        }

        // 조준할 적이 있고 쿨다운이 0 이하면 발사
        if (targetEnemy != null && fireCooldown <= 0)
        {
            Fire();
            // 쿨다운 초기화 (1 나누기 초당 발사 횟수)
            fireCooldown = 1f / fireRate;
            // 발사 후 즉시 새로운 적을 찾도록 targetEnemy를 null로 설정 (선택적: 계속 같은 적을 쏠 수도 있음)
            // targetEnemy = null;
        }

        if (targetEnemy != null && fireCooldown <= 0)
        {
            Fire();
            fireCooldown = 1f / (baseFireRate * currentFireRateMultiplier); // 배율 적용
        }
    }

    void FindNearestEnemy()
    {
        targetEnemy = null; // 일단 초기화
        float closestDistanceSqr = attackRange * attackRange; // 거리 제곱으로 비교 (루트 연산 방지)
        Vector3 currentPosition = transform.position; // 성의 위치

        // "Enemy" 태그를 가진 모든 활성 상태의 적 오브젝트를 찾음
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemyObject in enemies)
        {
            Vector3 directionToEnemy = enemyObject.transform.position - currentPosition;
            float dSqrToEnemy = directionToEnemy.sqrMagnitude; // 적까지 거리의 제곱 계산

            // 사거리 내에 있고, 현재까지 가장 가까운 적인 경우
            if (dSqrToEnemy < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToEnemy;
                targetEnemy = enemyObject.transform; // 가장 가까운 적을 타겟으로 설정
            }
        }
        // Debug.Log(targetEnemy != null ? "가장 가까운 적 찾음: " + targetEnemy.name : "사거리 내 적 없음");
    }

    void Fire()
    {
        if (targetEnemy == null) return; // 예외 처리

        Vector3 spawnPosition = (firePoint != null) ? firePoint.position : transform.position;
        Vector2 direction = (targetEnemy.position - spawnPosition).normalized;

        // 발사체 생성
        GameObject projectileGO = ObjectPooler.Instance.SpawnFromPool("Projectile", spawnPosition, Quaternion.identity);

        // 발사체의 회전 설정
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // 방향 벡터를 각도로 변환
        projectileGO.transform.rotation = Quaternion.Euler(0, 0, angle); // Z축 기준으로 회전

        if (projectileGO != null)
        {
            Projectile projectileScript = projectileGO.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetDirection(direction);
                projectileScript.damage = baseDamage * currentDamageMultiplier;
            }
            else Debug.LogError("Spawned Projectile is missing Projectile script!");
        }
        else
        {
            // 풀 비었거나 오류 처리
            // Debug.LogError("Failed to spawn projectile from pool!");
        }
        
        // (선택적) 발사 효과음 재생
        // AudioSource.PlayClipAtPoint(fireSound, spawnPosition);
    }

    // (선택적) Scene 뷰에서 공격 사거리를 시각적으로 표시
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // 공격력 배율 증가 함수 (LevelManager에서 호출)
    public void IncreaseDamageMultiplier(float amount)
    {
        currentDamageMultiplier += amount; // 0.1 (10%) 같은 값이 더해짐
        Debug.Log($"무기 공격력 배율 증가! 현재: {currentDamageMultiplier * 100}%");
    }

    // 발사 속도 배율 증가 함수 (LevelManager에서 호출)
    public void IncreaseFireRateMultiplier(float amount)
    {
        currentFireRateMultiplier += amount;
        Debug.Log($"무기 발사 속도 배율 증가! 현재: {currentFireRateMultiplier * 100}%");
    }
}