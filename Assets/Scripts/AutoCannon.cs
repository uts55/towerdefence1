using UnityEngine;

public class AutoCannon : MonoBehaviour
{
    public GameObject projectilePrefab; // 발사할 발사체 프리팹 (Inspector에서 연결)
    public Transform firePoint;         // 발사체가 생성될 위치 (없으면 성 위치에서 발사)
    public float fireRate = 2f;         // 발사 속도 (초당 발사 횟수)
    public float attackRange = 8f;      // 공격 사거리

    private float fireCooldown = 0f;    // 다음 발사까지 남은 시간
    private Transform targetEnemy = null; // 현재 조준 중인 적

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
        if (projectilePrefab == null || targetEnemy == null) return; // 예외 처리

        // 발사 위치 결정 (firePoint가 설정되어 있으면 거기서, 아니면 성 위치에서)
        Vector3 spawnPosition = (firePoint != null) ? firePoint.position : transform.position;

        // 발사 방향 계산 (적 방향)
        Vector2 direction = (targetEnemy.position - spawnPosition).normalized;

        // 발사체 생성
        GameObject projectileGO = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        // 발사체의 회전 설정
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // 방향 벡터를 각도로 변환
        projectileGO.transform.rotation = Quaternion.Euler(0, 0, angle); // Z축 기준으로 회전

        // 생성된 발사체의 Projectile 스크립트 가져오기
        Projectile projectileScript = projectileGO.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            // 발사체 방향 설정
            projectileScript.SetDirection(direction);
            // 발사체 데미지 설정 (필요하다면 무기 스탯에 따라 변경)
            // projectileScript.damage = weaponDamage;
        }
        else
        {
            Debug.LogError("생성된 Projectile 프리팹에 Projectile 스크립트가 없습니다!");
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
}