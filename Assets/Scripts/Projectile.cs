using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;      // 발사체 속도
    public float damage = 5f;       // 발사체 공격력
    public float lifetime = 3f;     // 발사체 생존 시간 (초)

    private Vector2 direction;     // 이동 방향
    private float lifeTimer; // 남은 생존 시간 타이머

    void Start()
    {
        // lifetime 이후에 자동으로 파괴되도록 예약
        // Destroy(gameObject, lifetime);
    }

    void OnEnable()
    {
        // 생존 시간 타이머 초기화
        lifeTimer = lifetime;
        // SetDirection은 AutoCannon의 Fire에서 호출됨
    }
    
    // 외부(무기 스크립트)에서 호출하여 이동 방향을 설정하는 함수
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized; // 방향 벡터 정규화 (크기를 1로 만듦)
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // 생존 시간 감소 및 체크
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            // 시간 다 되면 비활성화 (풀로 돌아감)
             gameObject.SetActive(false);
        }
    }

    // 다른 트리거 콜라이더와 충돌했을 때 호출됨
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // 데미지는 OnEnable에서 리셋할 필요 없음 (Cannon이 설정)
            }
            // 적과 충돌하면 즉시 비활성화 (풀로 돌아감)
             gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        // 풀러에게 오브젝트 반환 요청
         if (ObjectPooler.Instance != null)
        {
            ObjectPooler.Instance.ReturnToPool("Projectile", gameObject);
        }
    }
}