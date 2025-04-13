using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // 추가

public class CastleHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider; // 추가: 체력바 슬라이더 참조

    void Start()
    {
        currentHealth = maxHealth;
        // 슬라이더 초기 설정
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        else Debug.LogWarning("Health Slider가 CastleHealth에 할당되지 않았습니다.");
        // Debug.Log("성 체력 초기화..."); // 로그는 이제 UI로 확인 가능하므로 줄여도 됨
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthUI(); // UI 업데이트 호출
        // Debug.Log("성이 피해를 입음...");

        if (currentHealth <= 0) Die();
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

    
    public void Heal(float amount) // 있다면
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHealthUI(); // UI 업데이트 호출
        // Debug.Log("성이 회복됨...");
    }

    // 체력 UI 업데이트 함수
    void UpdateHealthUI()
    {
         if (healthSlider != null)
         {
            healthSlider.value = currentHealth;
         }
    }

    // (선택적) 최대 체력 증가 업그레이드를 위한 함수
    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        currentHealth += amount; // 최대 체력 증가 시 현재 체력도 같이 증가시켜줌
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth; // 슬라이더 최대값 업데이트
        }
        UpdateHealthUI(); // UI 업데이트
        Debug.Log($"최대 체력 증가! 현재: {currentHealth}/{maxHealth}");
    }
}