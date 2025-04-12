using UnityEngine;
using UnityEngine.UI; // UI 요소 사용을 위해 추가 (나중에 필요)

public class LevelManager : MonoBehaviour
{
    // --- 싱글톤 구현 시작 ---
    public static LevelManager Instance { get; private set; } // 정적 인스턴스 속성

    private void Awake()
    {
        // 인스턴스가 이미 존재하고, 그것이 자신이 아니라면
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 생성된 자신을 파괴
        }
        else
        {
            Instance = this; // 인스턴스를 자신으로 설정
            // (선택적) 씬이 로드될 때 파괴되지 않도록 설정 (게임 재시작 시 필요할 수 있음)
            // DontDestroyOnLoad(gameObject);
        }
    }
    // --- 싱글톤 구현 끝 ---


    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 10;
    public float xpMultiplier = 1.5f;

    public GameObject levelUpPanel;

    void Start()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }
        Debug.Log($"Level: {currentLevel}, XP: {currentXP}/{xpToNextLevel}");
    }

    public void GainExperience(int amount)
    {
        currentXP += amount;
        Debug.Log($"XP 획득: +{amount} / 현재: {currentXP}/{xpToNextLevel}");
        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            currentLevel++;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpMultiplier);
            Debug.Log($"레벨 업! 현재 레벨: {currentLevel}, 다음 필요 경험치: {xpToNextLevel}");
            HandleLevelUpRewards();
        }
    }

    void HandleLevelUpRewards()
    {
        Time.timeScale = 0f;
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(true);
            Debug.Log("레벨업! 게임 일시정지됨. 업그레이드를 선택하세요.");
        }
        else
        {
            Debug.LogWarning("Level Up Panel이 LevelManager에 할당되지 않았습니다!");
        }
    }

    public void ResumeGameAfterLevelUp()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }
        Time.timeScale = 1f;
        Debug.Log("업그레이드 선택 완료 (또는 임시 재개). 게임 재개됨.");
    }
}