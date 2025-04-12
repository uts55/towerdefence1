using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가 (게임 오버 후 재시작 등)

public class CastleHealth : MonoBehaviour
{
    public float maxHealth = 100f; // 성의 최대 체력 (Inspector에서 조절 가능)
    public float currentHealth;    // 현재 체력

    // 게임 시작 시 호출되는 함수
    void Start()
    {
        // 현재 체력을 최대 체력으로 초기화
        currentHealth = maxHealth;
        Debug.Log("성 체력 초기화: " + currentHealth + "/" + maxHealth); // 초기 상태 확인용 로그
    }

    // 외부에서 호출하여 성에 피해를 주는 함수
    public void TakeDamage(float amount)
    {
        // 받은 피해량만큼 현재 체력 감소
        currentHealth -= amount;
        Debug.Log("성이 피해를 입음! 현재 체력: " + currentHealth + "/" + maxHealth); // 피해 확인용 로그

        // TODO: 여기에 체력이 감소했을 때 시각적 효과 추가 (예: 성 색상 깜빡임)

        // 체력이 0 이하가 되었는지 확인
        if (currentHealth <= 0)
        {
            Die(); // 성 파괴 처리 함수 호출
        }
    }

    // 성이 파괴되었을 때 처리
    void Die()
    {
        currentHealth = 0; // 체력이 음수가 되지 않도록 0으로 고정
        Debug.LogError("게임 오버! 성이 파괴되었습니다."); // 게임 오버 로그

        // TODO: 여기에 게임 오버 UI 표시, 점수 기록 등 로직 추가

        // 임시 게임 오버 처리: 게임 시간을 멈춤 (더 이상 진행 안 됨)
        Time.timeScale = 0f;

        // 또는 현재 씬을 다시 로드하여 게임 재시작 (주석 처리됨)
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // (선택 사항) 나중에 체력 회복 아이템 등을 위해 추가할 수 있는 함수
    public void Heal(float amount)
    {
        currentHealth += amount;
        // 현재 체력이 최대 체력을 넘지 않도록 제한
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log("성이 회복됨! 현재 체력: " + currentHealth + "/" + maxHealth);
    }
}