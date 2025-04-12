using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;      // 발사체 속도
    public float damage = 5f;       // 발사체 공격력
    public float lifetime = 3f;     // 발사체 생존 시간 (초)

    private Vector2 direction;     // 이동 방향

    void Start()
    {
        // lifetime 이후에 자동으로 파괴되도록 예약
        Destroy(gameObject, lifetime);
    }

    // 외부(무기 스크립트)에서 호출하여 이동 방향을 설정하는 함수
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized; // 방향 벡터 정규화 (크기를 1로 만듦)
    }

    void Update()
    {
        // 설정된 방향으로 매 프레임 이동
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        // Space.World: 월드 좌표 기준으로 이동 (오브젝트의 회전과 상관없이)
    }

    // 다른 트리거 콜라이더와 충돌했을 때 호출됨
    void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 대상이 "Enemy" 태그를 가졌는지 확인
        if (other.CompareTag("Enemy"))
        {
            // 적의 Enemy 스크립트 컴포넌트 가져오기
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 적에게 데미지 주기
                enemy.TakeDamage(damage);
            }
            else
            {
                Debug.LogError("충돌한 Enemy 오브젝트에 Enemy 스크립트가 없습니다!");
            }

            // 적과 충돌하면 발사체 즉시 파괴
            Destroy(gameObject);
        }
    }
}