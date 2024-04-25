using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    [SerializeField] private Transform p1;
    [SerializeField] private Transform p2;

    private GameObject objectP1;
    private GameObject objectP2;

    // 바구니에 담겨있는 오브젝트를 관리하는 딕셔너리
    private Dictionary<EObjectType.ESelObj, List<GameObject>> collectedObjects = new Dictionary<EObjectType.ESelObj, List<GameObject>>();

    private void OnTriggerEnter(Collider _other)
    {
        EObjectType objectTypeComponent = _other.GetComponent<EObjectType>();
        if (objectTypeComponent == null) return;

        // p1 또는 p2가 비어있는 경우, 그 위치에 오브젝트 고정 및 회전값 적용
        if (objectP1 == null)
        {
            objectP1 = _other.gameObject;
            objectP1.transform.position = p1.position;
            objectP1.transform.rotation = Quaternion.Euler(objectTypeComponent.fixedRotation);  // 지정된 회전값 적용
            Rigidbody rb = objectP1.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;  // 오브젝트를 Kinematic으로 설정하여 물리 계산 제외
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            return;
        }
        else if (objectP2 == null)
        {
            objectP2 = _other.gameObject;
            objectP2.transform.position = p2.position;
            objectP2.transform.rotation = Quaternion.Euler(objectTypeComponent.fixedRotation);  // 지정된 회전값 적용
            Rigidbody rb = objectP2.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            CompareAndRemoveIfMatched();
            return;
        }
    }


    private void OnTriggerExit(Collider _other)
    {
        // 오브젝트가 p1 또는 p2에 할당된 경우 초기화
        if (_other.gameObject == objectP1)
        {
            Rigidbody rb = objectP1.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;  // 물리 엔진이 다시 오브젝트에 영향을 미치도록 설정
            }
            objectP1 = null;  // objectP1 참조 초기화
        }
        else if (_other.gameObject == objectP2)
        {
            Rigidbody rb = objectP2.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;  // 물리 엔진이 다시 오브젝트에 영향을 미치도록 설정
            }
            objectP2 = null;  // objectP2 참조 초기화
        }
    }



    private void OnTriggerExit2(Collider _other)
    {
        // 오브젝트가 p1 또는 p2에 할당된 경우, OnTriggerExit에서 제거되지 않도록 로직을 변경
        if (_other.gameObject == objectP1 || _other.gameObject == objectP2)
        {
            return; // p1 또는 p2에 할당된 오브젝트는 무시
        }

        // 다른 오브젝트에 대한 기존 로직
        EObjectType objectTypeComponent = _other.GetComponent<EObjectType>();
        if (objectTypeComponent == null) return;
        EObjectType.ESelObj type = objectTypeComponent.objectType;

        if (collectedObjects.ContainsKey(type) && collectedObjects[type].Contains(_other.gameObject))
        {
            collectedObjects[type].Remove(_other.gameObject);
        }
    }


    private IEnumerator RemoveObjectsAfterDelay(EObjectType.ESelObj _type)
    {
        yield return new WaitForSeconds(0.1f); // 잠시 대기

        // 해당 타입의 오브젝트가 여전히 2개인지 재확인
        if (collectedObjects.TryGetValue(_type, out List<GameObject> objects) && objects.Count == 2)
        {
            foreach (GameObject obj in objects)
            {
                Destroy(obj); // 오브젝트 제거
            }
            collectedObjects[_type].Clear(); // 리스트 초기화
        }
    }

    private void CompareAndRemoveIfMatched()
    {
        if (objectP1 != null && objectP2 != null)
        {
            EObjectType objType1 = objectP1.GetComponent<EObjectType>();
            EObjectType objType2 = objectP2.GetComponent<EObjectType>();

            if (objType1.objectType == objType2.objectType)  // 타입이 같은 경우 제거
            {
                Destroy(objectP1);
                Destroy(objectP2);
                objectP1 = null;
                objectP2 = null;
            }
        }
    }
}
