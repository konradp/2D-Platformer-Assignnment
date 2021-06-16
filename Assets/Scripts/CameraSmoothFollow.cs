using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [Range(0.7f,10f)] [SerializeField] float cameraLerpSpeed;
    [Range(-15f,-5f)][SerializeField] float cameraDistance;
    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * cameraLerpSpeed);
            //fixing Z value
            transform.position = new Vector3(transform.position.x, transform.position.y, cameraDistance);
        }
    }
    public void UpdateTarget(Transform targetTransform)
    {
        target = targetTransform;
    }
}
