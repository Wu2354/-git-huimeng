using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepStaticIconTextAreaToWorld : MonoBehaviour
{
    private Vector3 worldPosition;
    private Quaternion worldRotation;
    private Vector3 worldScale;

    void Update()
    {
        worldPosition = transform.position;
        worldRotation = transform.rotation;
        worldScale = transform.lossyScale;
    }

    void LateUpdate()
    {
        transform.position = worldPosition;
        transform.rotation = worldRotation;
        transform.localScale = Vector3.Scale(worldScale, InverseScale(transform.parent));
    }

    Vector3 InverseScale(Transform parentTransform)
    {
        if (parentTransform == null) return Vector3.one;
        return new Vector3(1 / parentTransform.lossyScale.x, 1 / parentTransform.lossyScale.y, 1 / parentTransform.lossyScale.z);
    }
}
