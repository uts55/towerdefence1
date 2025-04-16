using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // 스크립트 실행 순서 설정이 여전히 중요!
            // 코드에서 풀을 초기화하더라도 Awake가 먼저 완료되어야 함
            InitializePools(); // Awake에서 풀 초기화
        }
    }

    // --- Inspector에서 설정할 프리팹 참조 ---
    [Header("Prefabs to Pool")] // Inspector에서 보기 좋게 그룹화
    public GameObject enemyPrefab;
    public GameObject projectilePrefab;
    // 필요하다면 다른 프리팹들도 추가 (예: public GameObject experienceGemPrefab;)

    [Header("Pool Sizes")]
    public int enemyPoolSize = 50;
    public int projectilePoolSize = 100;
    // 다른 프리팹 풀 크기 추가

    // 실제 풀 데이터를 저장할 딕셔너리 (태그, 비활성 오브젝트 큐)
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    // 풀 초기화 함수
    void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // --- 코드에서 직접 풀 정보 정의 및 생성 ---

        // 1. Enemy Pool
        if (enemyPrefab != null) // 프리팹이 할당되었는지 확인
        {
            CreatePool("Enemy", enemyPrefab, enemyPoolSize);
        }
        else Debug.LogError("[ObjectPooler] Enemy Prefab is not assigned in the Inspector!");

        // 2. Projectile Pool
        if (projectilePrefab != null)
        {
            CreatePool("Projectile", projectilePrefab, projectilePoolSize);
        }
        else Debug.LogError("[ObjectPooler] Projectile Prefab is not assigned in the Inspector!");

        // 3. 다른 풀이 있다면 여기에 추가
        // if (experienceGemPrefab != null) { CreatePool("ExperienceGem", experienceGemPrefab, experienceGemPoolSize); }

        // --- 코드 정의 끝 ---

        Debug.Log("[ObjectPooler] All pools initialized via code.");
    }

    // 풀 생성 로직을 별도 함수로 분리 (코드 가독성 향상)
    void CreatePool(string tag, GameObject prefab, int size)
    {
        Queue<GameObject> objectQueue = new Queue<GameObject>();

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false); // 비활성 상태로 생성
            objectQueue.Enqueue(obj); // 큐에 추가
            obj.transform.SetParent(this.transform); // Hierarchy 정리
        }

        poolDictionary.Add(tag, objectQueue); // 딕셔너리에 풀 추가
        Debug.Log($"[ObjectPooler] Pool '{tag}' initialized with {size} objects (Prefab: {prefab.name}).");
    }

    // 풀에서 오브젝트를 가져오는 함수 (활성화)
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag '{tag}' doesn't exist.");
            return null;
        }

        Queue<GameObject> objectQueue = poolDictionary[tag];

        // 풀에 사용 가능한 오브젝트가 있는지 확인
        if (objectQueue.Count > 0)
        {
            GameObject objectToSpawn = objectQueue.Dequeue(); // 큐에서 하나 꺼냄

            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.SetActive(true); // 활성화!

            // TODO: 여기에 IPooledObject 인터페이스 같은 것을 구현해서
            // 오브젝트가 활성화될 때 초기화하는 로직(OnObjectSpawn)을 호출할 수 있음

            return objectToSpawn;
        }
        else
        {
            // 풀이 비었을 경우 처리 (옵션)
            // 1. null 반환 (생성 실패)
             Debug.LogWarning($"Pool '{tag}' is empty. Consider increasing pool size.");
             return null;

            // 2. 새로 생성 (풀 크기 동적 확장 - Instantiate 비용 발생)
            /*
            Pool poolToExpand = pools.Find(p => p.tag == tag);
            if (poolToExpand != null) {
                 GameObject obj = Instantiate(poolToExpand.prefab);
                 // obj.transform.SetParent(this.transform); // 필요 시 부모 설정
                 obj.transform.position = position;
                 obj.transform.rotation = rotation;
                 obj.SetActive(true);
                 Debug.LogWarning($"Pool '{tag}' was empty. Instantiated a new object.");
                 // 이 오브젝트는 ReturnToPool될 때 해당 큐에 추가될 것임
                 return obj;
            } else {
                 return null; // 프리팹 정보도 없으면 생성 불가
            }
            */
        }
    }

    // 오브젝트를 풀로 돌려보내는 함수 (비활성화)
    // 주의: 이 함수는 보통 비활성화되는 오브젝트 자체의 OnDisable() 에서 호출됨
    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log($"ReturnToPool called with tag: {tag}");
            // foreach (var key in poolDictionary.Keys)
            // {
            //     Debug.Log($"Existing pool tag: {key}");
            // }
            Destroy(objectToReturn); // 풀이 없으면 그냥 파괴
            return;
        }

        // 오브젝트가 이미 비활성화 상태여야 함 (보통 OnDisable에서 호출되므로)
        // if (objectToReturn.activeSelf)
        // {
             // 만약의 경우를 대비해 비활성화
             // objectToReturn.SetActive(false);
             // Debug.LogWarning($"Object '{objectToReturn.name}' returned to pool '{tag}' while active. Deactivating.");
        // }

        // 해당 태그의 큐에 오브젝트를 다시 추가
        poolDictionary[tag].Enqueue(objectToReturn);
    }
}