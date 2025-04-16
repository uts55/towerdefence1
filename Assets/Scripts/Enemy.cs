using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f; // 적의 이동 속도
    public float health = 10f;   // 적의 체력
    public float damage = 1f;    // 적이 성에 주는 피해량
    public int experienceValue = 1; // 이 적을 처치했을 때 주는 경험치
    public float initialHealth = 10f; // 초기 체력값 저장 (재사용 시 필요)

    private Transform targetCastle; // 추적할 성의 Transform

    void Start()
    {
        GameObject castleObject = GameObject.FindGameObjectWithTag("Castle");
        if (castleObject != null)
        {
            targetCastle = castleObject.transform;
        }
        else
        {
            Debug.LogError("성을 찾을 수 없습니다! Castle 오브젝트에 'Castle' 태그가 지정되었는지 확인하세요.");
            enabled = false;
        }
    }

    void Update()
    {
        if (targetCastle != null)
        {
            //Vector2 direction = (targetCastle.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, targetCastle.position, moveSpeed * Time.deltaTime);
        }
    }

    // 다른 Collider2D (Is Trigger가 체크된) 영역에 들어갔을 때 호출되는 함수
    void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한(영역에 들어온) 오브젝트가 "Castle" 태그를 가지고 있는지 확인
        // Collision2D 대신 Collider2D 타입의 'other' 매개변수를 사용합니다.
        if (other.CompareTag("Castle"))
        {
            // 성의 CastleHealth 스크립트에 접근하여 피해를 줌
            CastleHealth castleHealth = other.GetComponent<CastleHealth>();
            if (castleHealth != null)
            {
                 castleHealth.TakeDamage(damage); // CastleHealth 스크립트에 TakeDamage 함수 구현 필요 (다음 단계)
                 Debug.Log("적과 성이 충돌 (트리거)!"); // 임시 로그: 충돌 확인용
            }

            // 성과 충돌하면 스스로 파괴
            Destroy(gameObject);
        }

        // TODO: 나중에 여기에 발사체(Projectile)와의 충돌 처리 로직도 추가할 수 있습니다.
        // if (other.CompareTag("Projectile")) { ... }
    }

    void OnEnable()
    {
        // 상태 초기화 (재사용될 때마다 호출됨)
        health = initialHealth; // 체력을 초기값으로 리셋!
        // TODO: 필요한 다른 상태들도 여기서 초기화 (예: 이동 속도 변화가 있었다면 등)
        // targetCastle은 Start에서 찾으므로 보통은 괜찮음
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            if (gameObject.activeSelf) // 이미 비활성화 중이면 Die() 중복 호출 방지
            {
               Die();
            }
        }
    }

    void Die()
    {
        // LevelManager 경험치 지급은 그대로 유지
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.GainExperience(experienceValue);
        }

        // Destroy(gameObject) 대신 오브젝트 비활성화
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        // 풀러에게 오브젝트 반환 요청
        if (ObjectPooler.Instance != null)
        {
            ObjectPooler.Instance.ReturnToPool("Enemy", gameObject);
        }
    }
}