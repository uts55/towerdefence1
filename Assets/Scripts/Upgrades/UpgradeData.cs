using UnityEngine;

// 업그레이드 종류를 나타내는 열거형 (필요에 따라 추가)
public enum UpgradeType
{
    WeaponDamage,
    WeaponFireRate,
    WeaponProjectileSpeed, // 예시 추가
    CastleMaxHealth,
    MovementSpeed, // 플레이어 이동 속도 (만약 플레이어가 있다면)
    PickupRadius,  // 아이템 획득 반경 (만약 젬 방식이라면)
    // ... 기타 등등
}

// 프로젝트 메뉴에서 생성할 수 있도록 설정
[CreateAssetMenu(fileName = "NewUpgrade", menuName = "CastleSurvival/Upgrade Data")]
public class UpgradeData : ScriptableObject // MonoBehaviour 대신 ScriptableObject 상속
{
    public UpgradeType type;      // 업그레이드 종류
    public string upgradeName;    // UI에 표시될 이름 (예: "공격력 증가")
    [TextArea] // 여러 줄 입력 가능하도록
    public string description;    // UI에 표시될 설명
    public float value;           // 업그레이드 수치 (예: 공격력 증가량, 체력 증가량)
    public Sprite icon;           // UI에 표시될 아이콘 (선택 사항)

    // TODO: 업그레이드 레벨, 요구 조건 등 추가 가능
}