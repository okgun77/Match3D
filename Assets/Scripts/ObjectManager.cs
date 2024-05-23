using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] private GameObject[] objectPrefabs;
    [SerializeField] private GameObject disableObject; // 비활성화할 오브젝트를 지정할 수 있는 시리얼라이즈 필드
    [SerializeField] private float disableDelay = 5.0f; // 오브젝트 비활성화 딜레이 시간 (초)

    
    private void Start()
    {
        CreatePairObjects();
        
        if (disableObject != null)
        {
            StartCoroutine(DisableObjectAfterDelay(disableObject, disableDelay));
        }
    }

    private void CreatePairObjects()
    {
        for (int i = 0; i < objectPrefabs.Length; ++i)
        {
            // 각 오브젝트 타입별로 2개씩 생성
            for (int j = 0; j < 2; ++j)
            {
                GameObject instance = Instantiate(objectPrefabs[i], RandomPosition(), Quaternion.identity);
                instance.GetComponent<EObjectType>().objectType = (EObjectType.ESelObj)i; // 타입 번호 부여
            }
        }
    }
    
    private Vector3 RandomPosition()
    {
        // 범위 내 무작위 위치 생성
        return new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-5f, 2f));
    }
    
    private IEnumerator DisableObjectAfterDelay(GameObject _obj, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.SetActive(false); // 오브젝트 비활성화
    }
}
