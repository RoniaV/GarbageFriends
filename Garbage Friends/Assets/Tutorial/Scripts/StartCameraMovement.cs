using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StartCameraMovement : MonoBehaviour
{
    public event Action OnCameraMovementEnd;

    [SerializeField] Transform camera;
    [SerializeField] GameObject gameCanvas;
    [Header("Movement Settings")]
    [SerializeField] float delayTime;
    [SerializeField] float movementTime;
    [SerializeField] Vector3 endPosition;
    [SerializeField] string moveAnimTrigger = "Move";

    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        gameCanvas.SetActive(false);
        StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        yield return new WaitForSeconds(delayTime);

        animator.SetTrigger(moveAnimTrigger);

        Vector3 startPos = camera.position;
        float t = 0;
        float timer = 0;

        while(timer < movementTime)
        {
            t = Mathf.InverseLerp(0, movementTime, timer);
            camera.position = Vector3.Lerp(startPos, endPosition, t);

            yield return null;
            timer += Time.deltaTime;
        }

        camera.position = endPosition;

        yield return null;
        gameCanvas.SetActive(true);

        OnCameraMovementEnd?.Invoke();
    }
}
