using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    [SerializeField] private Transform p1;
    [SerializeField] private Transform p2;

    private GameObject objectAtP1;
    private GameObject objectAtP2;

    private Dictionary<int, GameObject> positionedObjects = new Dictionary<int, GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        EObjectType objectTypeComponent = other.GetComponent<EObjectType>();
        if (objectTypeComponent == null || objectTypeComponent.objectType == EObjectType.ESelObj.NONE)
        {
            Debug.Log("Invalid or none object type encountered.");
            return;
        }

        int instanceID = other.gameObject.GetInstanceID();
        Debug.Log("Triggered object: " + other.gameObject.name + ", Type: " + objectTypeComponent.objectType + ", Instance ID: " + instanceID);

        if (!positionedObjects.ContainsKey(instanceID))
        {
            if (objectAtP1 == null)
            {
                objectAtP1 = other.gameObject;
                positionedObjects[instanceID] = objectAtP1;
                MoveAndFixObject(objectAtP1, p1.position);
                Debug.Log("Object placed at P1: " + objectAtP1.name);
            }
            else if (objectAtP2 == null && !positionedObjects.ContainsKey(instanceID))
            {
                objectAtP2 = other.gameObject;
                positionedObjects[instanceID] = objectAtP2;
                MoveAndFixObject(objectAtP2, p2.position);
                CompareAndRemoveObjects();
                Debug.Log("Object placed at P2: " + objectAtP2.name);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        int instanceID = other.gameObject.GetInstanceID();
        if (positionedObjects.ContainsKey(instanceID))
        {
            ReleaseObject(other.gameObject);
            positionedObjects.Remove(instanceID);
        }
    }

    private void MoveAndFixObject(GameObject obj, Vector3 position)
    {
        obj.transform.position = position;
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // 물리 엔진 영향 제거
            rb.useGravity = false; // 중력 비활성화
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }


    private void ReleaseObject(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    private void CompareAndRemoveObjects()
    {
        if (positionedObjects.Count == 2)
        {
            var objects = new List<GameObject>(positionedObjects.Values);
            EObjectType type1 = objects[0].GetComponent<EObjectType>();
            EObjectType type2 = objects[1].GetComponent<EObjectType>();

            Debug.Log($"Comparing objects: {objects[0].name} with type {type1.objectType}, {objects[1].name} with type {type2.objectType}");

            if (type1.objectType == type2.objectType)
            {
                Debug.Log("Removing matched objects.");
                Destroy(objects[0]);
                Destroy(objects[1]);
                positionedObjects.Clear();
            }
            else
            {
                Debug.Log("Objects did not match.");
            }
        }
    }

}
