using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    private GameObject selectedObject;
    private Rigidbody selectedRigidbody;
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 dragStartMousePosition; // �巡�� ���� �� ���콺 ��ġ
    private Vector3 lastMousePosition; // ������ ���콺 ��ġ�� �����ϴ� ����
    private float dragStartTime; // �巡�� ���� �ð�

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectObject();
        }

        if (isDragging)
        {
            DragObject();
            lastMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z)); // ������ ���콺 ��ġ ������Ʈ
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

            Vector3 velocity = (lastMousePosition - dragStartMousePosition) / (Time.time - dragStartTime); // �巡�� ������ ��� �ӵ� ���

            selectedRigidbody.AddForce(velocity * 0.3f, ForceMode.VelocityChange); // ���� �ӵ��� ����� �� ����
        }
    }
    */

    private void ReleaseObject()
    {
        if (selectedRigidbody != null)
        {
            selectedRigidbody.isKinematic = false;

            Vector3 velocity = (lastMousePosition - dragStartMousePosition) / (Time.time - dragStartTime); // �巡�� ������ ��� �ӵ� ���

            selectedRigidbody.AddForce(velocity * 0.3f, ForceMode.VelocityChange); // ���� �ӵ��� ����� �� ����

            // ��ũ ������ ���� ������ ���� ���� ����
            Vector3 randomTorque = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), Random.Range(-3f, 3f));
            selectedRigidbody.AddTorque(randomTorque, ForceMode.Impulse); // ������ �������� ��ũ ����
        }
    }
}
