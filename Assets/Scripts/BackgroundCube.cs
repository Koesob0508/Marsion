using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCube : MonoBehaviour
{
    [SerializeField] private float RotationSpeed = 10.0f;

    void Update()
    {
        // x���� �������� rotationSpeed��ŭ �ʴ� ȸ����Ŵ
        transform.rotation *= Quaternion.Euler(0f, 0f, RotationSpeed * Time.deltaTime);
    }
}
