using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    // 카메라 이동
    [SerializeField] public bool cameramove = true;
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] private float edgeMouseSize = 30f;
    [SerializeField] private float edgeSizeX = 1f;
    [SerializeField] private float edgeSizeY = 1f;
    // 카메라 흔들기
    [SerializeField] private Vector3 shakeOffset = Vector3.zero;
    [SerializeField] private float shakeForce = 0f;

    private Quaternion originRot = Quaternion.identity;
    public bool bProductioning { get; set; } = false;
    public bool bComplete { get; set; } = false;
    private bool bStopProductioning = false;

    private Vector3 cameraStartPos = Vector3.zero;
    private Vector3 ProductionPosition = Vector3.zero;

    private Camera mainCamera;

    private bool isCameraSet = false;       // 카메라 셋팅 상태
    private Rect limitCameraBounds;         // 카메라 최대 이동 제한 영역
    public float minSize = 5f;              // 카메라 최소 사이즈
    public float maxSize = 7f;              // 카메라 최대 사이즈
    public float zoomSpeed = 1f;

    public event Action<bool> OnCameraWalkingEvent;
    public event Action OnCameraEventEnd;

    protected override void Awake()
    {
    }

    void Start()
    {
        mainCamera = Camera.main;

        originRot = transform.rotation;

        MapBase pMapBase = MapManager.Instance.GetCurrentMapInfo();
        if (pMapBase)
        {
            if (pMapBase.spawnPoint)
                ProductionPosition = pMapBase.spawnPoint.position;
        }
    }

    void Update()
    {
        if (bComplete || bProductioning)
            return;

        if (Oracle.m_eGameType == GameDefines.MapType.ADVENTURE)
            return;

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            cameramove = !cameramove;
            OnCameraWalkingEvent?.Invoke(cameramove);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = cameraStartPos;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float newSize = mainCamera.orthographicSize - scroll * zoomSpeed;

            newSize = Mathf.Clamp(newSize, 5f, 7f);
            mainCamera.orthographicSize = newSize;
        }
    }
    void FixedUpdate()
    {
        if (bComplete)
            return;

        if (bProductioning)
        {
            if (!bStopProductioning)
            {
                Vector2 cameraPosition = Vector2.MoveTowards(transform.position, ProductionPosition, 10f * Time.deltaTime);
                Vector3 myPosition = new Vector3(cameraPosition.x, cameraPosition.y, transform.position.z);
                // 이동제한
                bool bNotMove = false;
                if (myPosition.x < limitCameraBounds.x - limitCameraBounds.width + edgeSizeX || myPosition.x > limitCameraBounds.x + limitCameraBounds.width - edgeSizeX)
                    bNotMove = true;
                if (myPosition.y < limitCameraBounds.y - limitCameraBounds.height + edgeSizeY || myPosition.y > limitCameraBounds.y + limitCameraBounds.height - edgeSizeY)
                    bNotMove = true;

                if(!bNotMove)
                    transform.position = myPosition;

                if (transform.position == ProductionPosition || bNotMove)
                {
                    bStopProductioning = true;
                    OnCameraEventEnd?.Invoke();
                }
            }

            return;
        }

        if (!cameramove)
            return;

        if (!isCameraSet || mainCamera == null)
            return;

        Vector3 newPosition = transform.position;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        newPosition.x += horizontalInput * moveSpeed * Time.deltaTime;
        newPosition.y += verticalInput * moveSpeed * Time.deltaTime;

        Vector3 mousePosition = Input.mousePosition;
        if (mousePosition.x > Screen.width - edgeMouseSize)
        {
            newPosition.x += moveSpeed * Time.deltaTime;
        }
        if (mousePosition.x < edgeMouseSize)
        {
            newPosition.x -= moveSpeed * Time.deltaTime;
        }
        if (mousePosition.y > Screen.height - edgeMouseSize)
        {
            newPosition.y += moveSpeed * Time.deltaTime;
        }
        if (mousePosition.y < edgeMouseSize)
        {
            newPosition.y -= moveSpeed * Time.deltaTime;
        }

        // 이동제한
        if (newPosition.x < limitCameraBounds.x - limitCameraBounds.width + edgeSizeX || newPosition.x > limitCameraBounds.x + limitCameraBounds.width - edgeSizeX)
            newPosition.x = transform.position.x;
        if (newPosition.y < limitCameraBounds.y - limitCameraBounds.height + edgeSizeY || newPosition.y > limitCameraBounds.y + limitCameraBounds.height - edgeSizeY)
            newPosition.y = transform.position.y;

        transform.position = newPosition;
    }

    public void Move(Vector3 position)
    {
        transform.position = position;
    }

    public void SetCameraRect(Rect limitCameraRect)
    {
        isCameraSet = true;
        limitCameraBounds = limitCameraRect;
    }

    public void CameraShake(float fTime, bool bResetControl)
    {
        StartCoroutine(ShakeCoroutine(fTime, bResetControl));
    }
    private void ResetCameraShake()
    {
        transform.rotation = Quaternion.identity;
        mainCamera.transform.rotation = Quaternion.identity;
        transform.eulerAngles = Vector3.zero;
    }
    public void EndCameraShake()
    {
        bProductioning = false;
        bStopProductioning = false;
    }
    IEnumerator ShakeCoroutine(float fTime, bool bResetControl)
    {
        Vector3 originEuler = transform.eulerAngles;
        while (fTime > 0.0f)
        {
            float rotX = Oracle.RandomDice(-shakeOffset.x, shakeOffset.x);
            float rotY = Oracle.RandomDice(-shakeOffset.y, shakeOffset.y);
            float rotZ = Oracle.RandomDice(-shakeOffset.z, shakeOffset.z);

            Vector3 randomRot = originEuler + new Vector3(rotX, rotY, rotZ);
            Quaternion rot = Quaternion.Euler(randomRot);

            while (Quaternion.Angle(transform.rotation, rot) > 0.1f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, shakeForce * Time.deltaTime);
                fTime -= Time.deltaTime;
                yield return null;
            }
            
            yield return null;
        }

        ResetCameraShake();
        if (!bResetControl)
            EndCameraShake();
    }

    public void StartCameraPosition(float x, float y)
    {
        cameraStartPos = new Vector3(x, y, transform.position.z);
        transform.position = cameraStartPos;
    }
}
