using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    private GameObject selectedObject;
    private Rigidbody selectedRigidbody;
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 dragStartMousePosition; // 드래그 시작 시 마우스 위치
    private Vector3 lastMousePosition; // 마지막 마우스 위치를 추적하는 변수
    private float dragStartTime; // 드래그 시작 시간

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectObject();
        }

        if (isDragging)
        {
            DragObject();
            lastMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z)); // 마지막 마우스 위치 업데이트
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            ReleaseObject();
            isDragging = false;
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
                if (selectedObject != null)
                {
                    selectedObject.GetComponent<Renderer>().material.color = Color.white;
                }

                selectedObject = hit.transform.gameObject;
                selectedObject.GetComponent<Renderer>().material.color = Color.red;
                selectedRigidbody = selectedObject.GetComponent<Rigidbody>();

                if (selectedRigidbody != null)
                {
                    selectedRigidbody.isKinematic = true;
                }

                isDragging = true;

                dragStartMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(
                    Input.mousePosition.x,
                    Input.mousePosition.y,
                    Camera.main.WorldToScreenPoint(selectedObject.transform.position).z));
                dragStartTime = Time.time;
            }
        }
    }

    private void DragObject()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.WorldToScreenPoint(selectedObject.transform.position).z));

        float desiredHeight = 1.0f;

        selectedObject.transform.position = new Vector3(
            mouseWorldPosition.x + offset.x,
            desiredHeight,
            mouseWorldPosition.z + offset.z
        );

        // selectedObject.transform.position = mouseWorldPosition + offset;
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

            Vector3 velocity = (lastMousePosition - dragStartMousePosition) / (Time.time - dragStartTime); // 드래그 동안의 평균 속도 계산

            selectedRigidbody.AddForce(velocity * 0.3f, ForceMode.VelocityChange); // 계산된 속도에 기반한 힘 적용

            // 토크 적용을 위한 무작위 방향 벡터 생성
            Vector3 randomTorque = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), Random.Range(-3f, 3f));
            selectedRigidbody.AddTorque(randomTorque, ForceMode.Impulse); // 무작위 방향으로 토크 적용
        }
    }
}
