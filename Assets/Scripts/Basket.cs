using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    // 바구니에 담겨있는 오브젝트를 관리하는 딕셔너리
    private Dictionary<EObjectType.ESelObj, List<GameObject>> collectedObjects = new Dictionary<EObjectType.ESelObj, List<GameObject>>();

    private void OnTriggerEnter(Collider _other)
    {
        // 오브젝트에서 EObjectType 컴포넌트를 가져옵니다.
        EObjectType objectTypeComponent = _other.GetComponent<EObjectType>();
        if (objectTypeComponent == null) return;

        EObjectType.ESelObj type = objectTypeComponent.objectType;

        // 해당 타입의 오브젝트 리스트가 없으면 새로 생성
        if (!collectedObjects.ContainsKey(type))
        {
            collectedObjects[type] = new List<GameObject>();
        }
        else
        {
            // 이미 같은 인스턴스의 오브젝트가 리스트에 있다면 추가하지 않음
            if (collectedObjects[type].Contains(_other.gameObject))
            {
                return;
            }
        }

        collectedObjects[type].Add(_other.gameObject);

        // 바구니 내 동일한 타입의 오브젝트가 정확히 2개인 경우만 제거 로직 실행
        if (collectedObjects[type].Count == 2)
        {
            StartCoroutine(RemoveObjectsAfterDelay(type)); // 짧은 딜레이 후 오브젝트 제거
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        EObjectType objectTypeComponent = _other.GetComponent<EObjectType>();

        if (objectTypeComponent == null) return;

        EObjectType.ESelObj type = objectTypeComponent.objectType;

        if (collectedObjects.ContainsKey(type) && collectedObjects[type].Contains(_other.gameObject))
        {
            collectedObjects[type].Remove(_other.gameObject);
        }
    }

    private IEnumerator RemoveObjectsAfterDelay(EObjectType.ESelObj type)
    {
        yield return new WaitForSeconds(0.1f); // 잠시 대기

        // 해당 타입의 오브젝트가 여전히 2개인지 재확인
        if (collectedObjects.TryGetValue(type, out List<GameObject> objects) && objects.Count == 2)
        {
            foreach (GameObject obj in objects)
            {
                Destroy(obj); // 오브젝트 제거
            }
            collectedObjects[type].Clear(); // 리스트 초기화
        }
    }
}
