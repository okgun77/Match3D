using System.Collections;
using UnityEngine;

public class Basket : MonoBehaviour
{
    [SerializeField] private Transform p1;
    [SerializeField] private Transform p2;

    private GameObject objectP1;
    private GameObject objectP2;

    // 바구니에 담겨있는 오브젝트를 관리하는 딕셔너리
    // private Dictionary<EObjectType.ESelObj, List<GameObject>> collectedObjects = new Dictionary<EObjectType.ESelObj, List<GameObject>>();

    private void OnTriggerEnter(Collider _other)
    {
        EObjectType objectTypeComponent = _other.GetComponent<EObjectType>();

        if (objectTypeComponent == null) return;

        // p1 또는 p2가 비어있는 경우, 그 위치에 오브젝트 고정 및 회전값 적용
        if (objectP1 == null)
        {
            // objectP1 = _other.gameObject;
            // objectP1.transform.position = p1.position;
            // SetRigidbodyState(objectP1, true);
            // objectP1.transform.rotation = Quaternion.Euler(objectTypeComponent.fixedRotation);  // 지정된 회전값 적용
            // return;

            SetObjectAtPosition(_other.gameObject, p1, out objectP1, objectTypeComponent.fixedRotation);
        }
        else if (objectP2 == null)
        {
            // objectP2 = _other.gameObject;
            // objectP2.transform.position = p2.position;
            // SetRigidbodyState(objectP2, true);
            // objectP2.transform.rotation = Quaternion.Euler(objectTypeComponent.fixedRotation);  // 지정된 회전값 적용
            // CompareAndRemove();
            SetObjectAtPosition(_other.gameObject, p2, out objectP2, objectTypeComponent.fixedRotation);
            StartCoroutine("CompareAndRemove");
            // return;
        }
    }


    private void OnTriggerExit(Collider _other)
    {
        // 오브젝트가 p1 또는 p2에 할당된 경우 초기화
        if (_other.gameObject == objectP1)
        {
            SetRigidbodyState(objectP1, false);
            objectP1 = null;  // objectP1 참조 초기화
        }
        else if (_other.gameObject == objectP2)
        {
            SetRigidbodyState(objectP2, false);
            objectP2 = null;  // objectP2 참조 초기화
        }
    }


    private IEnumerator CompareAndRemove()
    {
        // yield return new WaitForSeconds(0.5f);

        if (objectP1 != null && objectP2 != null)
        {
            EObjectType objType1 = objectP1.GetComponent<EObjectType>();
            EObjectType objType2 = objectP2.GetComponent<EObjectType>();

            if (objType1.objectType == objType2.objectType)  // 타입이 같은 경우 제거
            {
                Debug.Log("before Destroy P1 : " + objectP1.name);
                Debug.Log("before Destroy P1 : " + objectP2.name);

                yield return new WaitForSeconds(0.3f);

                Destroy(objectP1);
                Destroy(objectP2);

                Debug.Log("after Destroy P1 : " + objectP1.name);
                Debug.Log("after Destroy P1 : " + objectP2.name);

                objectP1 = null;
                objectP2 = null;

            }
        }
    }


    private void SetObjectAtPosition(GameObject _obj, Transform _position, out GameObject _objRef, Vector3 _rotation)
    {
        _obj.transform.position = _position.position;
        _obj.transform.rotation = Quaternion.Euler(_rotation);
        SetRigidbodyState(_obj, true);
        _objRef = _obj;
    }
    
    private void SetRigidbodyState(GameObject _obj, bool _isKinematic)
    {
        Rigidbody rb = _obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // rb.isKinematic = _isKinematic;
            rb.constraints = _isKinematic ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;
        }
    }
}