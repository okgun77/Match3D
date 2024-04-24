using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    private GameObject selectedObject;
    private Rigidbody selectedRigidbody;
    private bool isHold = false;
    private Vector3 offset;
    private Vector3 holdStartMousePosition; // 잡기 시작 시 마우스 위치
    private Vector3 lastMousePosition; // 마지막 마우스 위치를 추적하는 변수
    private float holdStartTime; // 잡기 시작 시간

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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            EObjectType objectTypeComponent = hit.collider.GetComponent<EObjectType>();

            if (objectTypeComponent != null && objectTypeComponent.objectType != EObjectType.ESelObj.NONE)
            {
                //if (selectedObject != null)
                //{
                //    selectedObject.GetComponent<Renderer>().material.color = Color.white;
                //}

                selectedObject = hit.transform.gameObject;
                // selectedObject.GetComponent<Renderer>().material.color = Color.red;
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
            }
        }
    }

    private void HoldObject()
    {
        // selectedObject가 null이 아닌지 확인
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

            // selectedObject.transform.position = mouseWorldPosition + offset;
        }
    }

    /*
    private void ReleaseObject()
    {
        if (selectedRigidbody != null)
        {
            selectedRigidbody.isKinematic = false;

            Vector3 velocity = (lastMousePosition - dragStartMousePosition) / (Time.time - dragStartTime); // 드래그 동안의 평균 속도 계산

            selectedRigidbody.AddForce(velocity * 0.3f, ForceMode.VelocityChange); // 계산된 속도에 기반한 힘 적용
        }
    }
    */

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

        // 오브젝트 참조를 초기화합니다.
        selectedObject = null;
        selectedRigidbody = null;
    }

}
