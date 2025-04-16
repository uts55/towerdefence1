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
            // DontDestroyOnLoad(gameObject); // 필요하다면 주석 해제
        }

        InitializePools(); // Awake에서 풀 초기화
    }

    // 각 풀의 정보를 담을 클래스 또는 구조체
    [System.Serializable] // Inspector에 노출시키기 위함
    public class Pool
    {
        public string tag;      // 풀을 식별할 태그 (예: "Enemy", "Projectile")
        public GameObject prefab;   // 풀링할 프리팹
        public int size;       // 초기 풀 크기
    }

    public List<Pool> pools; // Inspector에서 설정할 풀 목록

    // 실제 풀 데이터를 저장할 딕셔너리 (태그, 비활성 오브젝트 큐)
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    // 풀 초기화 함수
    void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false); // 비활성 상태로 생성
                objectQueue.Enqueue(obj); // 큐에 추가

                // (선택적) 생성된 오브젝트를 Pooler 자식으로 넣어 Hierarchy 정리
                 obj.transform.SetParent(this.transform);
            }

            poolDictionary.Add(pool.tag, objectQueue); // 딕셔너리에 풀 추가
            Debug.Log($"Pool '{pool.tag}' initialized with {pool.size} objects.");
        }
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
            //Destroy(objectToReturn); // 풀이 없으면 그냥 파괴
            return;
        }

        // 오브젝트가 이미 비활성화 상태여야 함 (보통 OnDisable에서 호출되므로)
        if (objectToReturn.activeSelf)
        {
             // 만약의 경우를 대비해 비활성화
             // objectToReturn.SetActive(false);
             // Debug.LogWarning($"Object '{objectToReturn.name}' returned to pool '{tag}' while active. Deactivating.");
        }

        // 해당 태그의 큐에 오브젝트를 다시 추가
        poolDictionary[tag].Enqueue(objectToReturn);
    }
}