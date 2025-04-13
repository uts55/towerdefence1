using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
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

    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 10;
    public float xpMultiplier = 1.5f;

    public GameObject levelUpPanel;

    // UI 참조 변수
    public TextMeshProUGUI levelText;
    public Slider xpSlider;
    public TextMeshProUGUI timerText;

    // 타이머 변수
    private float survivalTime = 0f;

    public System.Collections.Generic.List<UpgradeData> availableUpgrades; // 보유 중인 모든 업그레이드 데이터 리스트 (Inspector에서 연결)
    public UpgradeData[] currentUpgradeOptions = new UpgradeData[3]; // 현재 제시된 3가지 옵션 저장용 배열

    // UI 요소 참조 (업그레이드 버튼 및 내용 표시용)
    public Button[] upgradeButtons; // 업그레이드 버튼 3개 (Inspector에서 연결)
    public TextMeshProUGUI[] upgradeButtonTitles; // 버튼 제목 텍스트 (Inspector에서 연결)
    public TextMeshProUGUI[] upgradeButtonDescriptions; // 버튼 설명 텍스트 (Inspector에서 연결)
    public Image[] upgradeButtonIcons; // 아이콘 이미지

    void Start()
    {
        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        UpdateLevelUI(); // 초기 UI 업데이트
        UpdateXPUI();    // 초기 UI 업데이트
        UpdateTimerUI(); // 초기 UI 업데이트
    }

    void Update()
    {
        // 게임 시간이 흐를 때만 (레벨업 중 아닐 때)
        if (Time.timeScale > 0)
        {
            survivalTime += Time.deltaTime; // 생존 시간 증가
            UpdateTimerUI(); // 매 프레임 타이머 UI 업데이트
        }
    }

    public void GainExperience(int amount)
    {
        currentXP += amount;
        UpdateXPUI(); // UI 업데이트
        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            currentLevel++;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpMultiplier);
            UpdateLevelUI(); // UI 업데이트
            UpdateXPUI();    // UI 업데이트 (최대값, 현재값 모두 변경됨)
            HandleLevelUpRewards();
        }
    }

    // --- UI 업데이트 함수들 ---
    void UpdateLevelUI()
    {
        if (levelText != null) levelText.text = "Level: " + currentLevel;
    }

    void UpdateXPUI()
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = xpToNextLevel;
            xpSlider.value = currentXP;
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            // 시간을 분:초 형식으로 변환
            int minutes = Mathf.FloorToInt(survivalTime / 60f);
            int seconds = Mathf.FloorToInt(survivalTime % 60f);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    void HandleLevelUpRewards()
    {
        Time.timeScale = 0f;

        if (levelUpPanel != null && availableUpgrades.Count > 0 && upgradeButtons.Length >= 3)
        {
            levelUpPanel.SetActive(true);
            // 3개의 랜덤 업그레이드 선택 (중복 없이)
            System.Collections.Generic.List<UpgradeData> tempList = new System.Collections.Generic.List<UpgradeData>(availableUpgrades);
            for (int i = 0; i < 3; i++)
            {
                if (tempList.Count == 0) // 선택할 업그레이드가 더 없으면
                {
                    currentUpgradeOptions[i] = null; // 옵션 비우기
                    upgradeButtons[i].gameObject.SetActive(false); // 버튼 비활성화
                    continue;
                }
                int randomIndex = Random.Range(0, tempList.Count);
                currentUpgradeOptions[i] = tempList[randomIndex]; // 옵션 저장
                tempList.RemoveAt(randomIndex); // 중복 선택 방지

                // UI 업데이트
                upgradeButtons[i].gameObject.SetActive(true); // 버튼 활성화
                if (upgradeButtonTitles.Length > i && upgradeButtonTitles[i] != null)
                    upgradeButtonTitles[i].text = currentUpgradeOptions[i].upgradeName;
                if (upgradeButtonDescriptions.Length > i && upgradeButtonDescriptions[i] != null)
                    upgradeButtonDescriptions[i].text = currentUpgradeOptions[i].description;
                if (upgradeButtonIcons.Length > i && upgradeButtonIcons[i] != null && currentUpgradeOptions[i].icon != null)
                    upgradeButtonIcons[i].sprite = currentUpgradeOptions[i].icon;

                // 버튼 클릭 이벤트 설정 (중요!) - 각 버튼이 어떤 인덱스를 선택했는지 알려주도록 설정
                int buttonIndex = i; // 람다식/델리게이트에서 클로저 문제를 피하기 위해 지역 변수 사용
                upgradeButtons[i].onClick.RemoveAllListeners(); // 기존 리스너 제거
                upgradeButtons[i].onClick.AddListener(() => SelectUpgrade(buttonIndex));
            }
        }
        else
        {
            Debug.LogWarning("Level Up Panel, 업그레이드 목록 또는 버튼 설정이 부족합니다!");
            // ResumeGameAfterLevelUp(); // 선택지 없으면 바로 재개할 수도 있음
        }
    }
    // 업그레이드 버튼 클릭 시 호출될 함수
    public void SelectUpgrade(int index)
    {
        if (index < 0 || index >= currentUpgradeOptions.Length || currentUpgradeOptions[index] == null)
        {
            Debug.LogError("잘못된 업그레이드 선택 인덱스입니다.");
            ResumeGameAfterLevelUp(); // 문제가 있으면 그냥 재개
            return;
        }

        UpgradeData selectedUpgrade = currentUpgradeOptions[index];
        ApplyUpgrade(selectedUpgrade); // 선택된 업그레이드 적용 함수 호출

        ResumeGameAfterLevelUp(); // 게임 재개
    }

    // 선택된 업그레이드를 실제로 적용하는 함수
    void ApplyUpgrade(UpgradeData upgrade)
    {
        Debug.Log($"업그레이드 적용: {upgrade.upgradeName}");
        switch (upgrade.type)
        {
            case UpgradeType.WeaponDamage:
                // TODO: AutoCannon 또는 WeaponManager에 공격력 증가 함수 호출
                // 예: GetComponent<AutoCannon>().IncreaseDamage(upgrade.value);
                AutoCannon cannon = GetComponent<AutoCannon>(); // LevelManager가 Castle에 붙어있다고 가정
                if (cannon != null) cannon.IncreaseDamageMultiplier(upgrade.value); // 곱연산 방식 예시
                else Debug.LogWarning("AutoCannon 컴포넌트를 찾을 수 없습니다.");
                break;
            case UpgradeType.WeaponFireRate:
                // TODO: AutoCannon 또는 WeaponManager에 발사 속도 증가 함수 호출
                AutoCannon cannonFR = GetComponent<AutoCannon>();
                if (cannonFR != null) cannonFR.IncreaseFireRateMultiplier(upgrade.value);
                else Debug.LogWarning("AutoCannon 컴포넌트를 찾을 수 없습니다.");
                break;
            case UpgradeType.CastleMaxHealth:
                CastleHealth health = GetComponent<CastleHealth>(); // LevelManager가 Castle에 붙어있다고 가정
                if (health != null) health.IncreaseMaxHealth(upgrade.value);
                else Debug.LogWarning("CastleHealth 컴포넌트를 찾을 수 없습니다.");
                break;
            // ... 다른 종류의 업그레이드 처리 ...
            default:
                Debug.LogWarning($"아직 처리되지 않은 업그레이드 타입: {upgrade.type}");
                break;
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