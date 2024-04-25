using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    private GameObject  selectedObject;
    private Rigidbody   selectedRigidbody;
    private bool        isHold = false;
    private Vector3     offset;                  
    private Vector3     holdStartMousePosition; // 잡기 시작 시 마우스 위치
    private Vector3     lastMousePosition;      // 마지막 마우스 위치를 추적하는 변수
    private float       holdStartTime;          // 잡기 시작 시간

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectObject();
        }

        if (isHold && selectedObject != null)
        {
            HoldObject();
            lastMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                Camera.main.WorldToScreenPoint(selectedObject.transform.position).z
                )); // 마지막 마우스 위치 업데이트
        }

        if (Input.GetMouseButtonUp(0) && isHold)
        {
            ReleaseObject();
            isHold = false;
        }
    }

    private void SelectObject()
    {
        Ray          ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.CompareTag("NotSelectable"))
            {
                EObjectType objectTypeComponent = hit.collider.GetComponent<EObjectType>();

                if (objectTypeComponent != null && objectTypeComponent.objectType != EObjectType.ESelObj.NONE)
                {
                    selectedObject = hit.transform.gameObject;
                    selectedRigidbody = selectedObject.GetComponent<Rigidbody>();

                    if (selectedRigidbody != null)
                    {
                        selectedRigidbody.isKinematic = true;
                    }

                    isHold = true;
                    holdStartMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(
                        Input.mousePosition.x,
                        Input.mousePosition.y,
                        Camera.main.WorldToScreenPoint(selectedObject.transform.position).z));
                    holdStartTime = Time.time;
                    Debug.Log("Selected object: " + selectedObject.name);
                    break;  // 첫 번째 선택 가능한 오브젝트를 선택하고 반복 중지
                }
            }
        }
    }


    private void HoldObject()
    {
        if (selectedObject != null)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                Camera.main.WorldToScreenPoint(selectedObject.transform.position).z));

            float desiredHeight = 1.5f;

            selectedObject.transform.position = new Vector3(
                mouseWorldPosition.x + offset.x,
                desiredHeight,
                mouseWorldPosition.z + offset.z
            );
        }
    }

    private void ReleaseObject()
    {
        if (selectedRigidbody != null)
        {
            selectedRigidbody.isKinematic = false;

            Vector3 velocity = (lastMousePosition - holdStartMousePosition) / (Time.time - holdStartTime); // 드래그 동안의 평균 속도 계산
            selectedRigidbody.AddForce(velocity * 0.3f, ForceMode.VelocityChange); // 계산된 속도에 기반한 힘 적용

            // 토크 적용을 위한 무작위 방향 벡터 생성
            Vector3 randomTorque = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), Random.Range(-3f, 3f));
            selectedRigidbody.AddTorque(randomTorque, ForceMode.Impulse); // 무작위 방향으로 토크 적용
        }

        // 오브젝트 참조 초기화.
        selectedObject      = null;
        selectedRigidbody   = null;
    }
}
