using System.Collections.Generic;
using UnityEngine;

public class UI_Interrupt : UIBase
{
    enum InterruptProcessState
    {
        APPEAR0,
        APPEAR1,
        APPEAR2,
        MOVE,
        GROW0,
        GROW1,
        ROTATE,
    }
    private class InterruptUIInfo
    {
        public InterruptProcessState eCurrentState = InterruptProcessState.APPEAR0;
        public InterruptProcessState eState = InterruptProcessState.APPEAR0;
        public GameObject UIObject = null;
        public RectTransform Rect = null;

        public bool bEndAppear = false;
        public byte EndMoveStep = 0;
        public Vector2 vStartMovePosition = Vector2.zero;
        public float fJourneyMoveLength = 0f;
        public float fStartTime = 0f;
        public bool bGrow = false;
        public float fInitialScale = 0f;

        public Vector2 LeaveLookVector = Vector2.zero;
    }

    private RectTransform canvasRect = null;
    private int maxInterruptCount = 0;
    private List<Vector2> InterruptProcessPositionList = null;
    private List<Vector2> InterruptAppearPositionList = null;
    private Dictionary<int, InterruptUIInfo> CreatedUIObjectList = null;
    private int iProcessCurrentindex = 0;

    private bool bEnd = false;
    private bool bDestory = false; 
    private float fEndTime = 0f;

    protected override void Awake()
    {
        // 860, 370
        InterruptProcessPositionList = new List<Vector2>();
        InterruptProcessPositionList.Add(new Vector2(0f, 0f));
        InterruptProcessPositionList.Add(new Vector2(-430f, 185f));
        InterruptProcessPositionList.Add(new Vector2(-430f, -185f));
        InterruptProcessPositionList.Add(new Vector2(430f, 185f));
        InterruptProcessPositionList.Add(new Vector2(430f, -185f));
        int n = maxInterruptCount = InterruptProcessPositionList.Count;
        while (n > 1)
        {
            int i = Oracle.RandomDice(0, n--);
            var value = InterruptProcessPositionList[i];
            InterruptProcessPositionList[i] = InterruptProcessPositionList[n];
            InterruptProcessPositionList[n] = value;
        }

        canvasRect = GetComponent<RectTransform>();
        float width = canvasRect.rect.width / 2f;
        float height = canvasRect.rect.height / 2f;
        InterruptAppearPositionList = new List<Vector2>();
        InterruptAppearPositionList.Add(new Vector2(width, height));  //start

        CreatedUIObjectList = new Dictionary<int, InterruptUIInfo>();
    }

    protected override void Update()
    {
        for (int i = 0; i < CreatedUIObjectList.Count; ++i)
        {
            InterruptUIInfo Info = CreatedUIObjectList[i];
            if (Info == null)
                continue;

            if (Info.UIObject == null)
                continue;

            if (bEnd)
            {
                if (Info.LeaveLookVector == Vector2.zero)
                {
                    Info.LeaveLookVector = Info.UIObject.transform.right;
                }

                Info.Rect.anchoredPosition += Info.LeaveLookVector * 1000f * Time.deltaTime;
                continue;
            }

            if (Info.eCurrentState == InterruptProcessState.ROTATE)
            {
                Info.UIObject.transform.Rotate(Vector3.forward * 2f);
                if (i >= maxInterruptCount - 1)
                {
                    bEnd = true;
                    fEndTime = Time.time + 2f;
                }
            }
            else if (Info.eCurrentState == InterruptProcessState.APPEAR0 || Info.eCurrentState == InterruptProcessState.APPEAR1 || Info.eCurrentState == InterruptProcessState.APPEAR2)
            {
                Bbaekkom(Info.eCurrentState, Info);
            }
            else if (Info.eCurrentState == InterruptProcessState.MOVE)
            {
                Move(i, Info);
            }
            else if (Info.eCurrentState == InterruptProcessState.GROW0 || Info.eCurrentState == InterruptProcessState.GROW1)
            {
                GROW(Info.eCurrentState, Info);
            }
        }

        if (bEnd && bDestory == false)
        {
            if (fEndTime < Time.time)
            {
                MonsterPool.Instance.UpdateBossPatternEvent();
                bDestory = true;
            }
        }
    }

    protected override void PreHide()
    {
    }

    public void Process()
    {
        if (CreatedUIObjectList.Count > iProcessCurrentindex)
        {
            InterruptUIInfo outInfo = CreatedUIObjectList[iProcessCurrentindex];
            if (outInfo.eState != InterruptProcessState.ROTATE)
            {
                outInfo.eState++;
                return;
            }
            else
            {
                ++iProcessCurrentindex;
            }
        }
         
        GameObject imagePrefab = ResourceAgent.Instance.GetRandomBossPrefab();
        GameObject newImage = Instantiate(imagePrefab, transform);
        CameraManager.Instance.bProductioning = false;

        Component[] components = newImage.GetComponents<Component>();
        foreach (Component component in components)
        {
            Destroy(component);
        }
        newImage.SetActive(true);
        newImage.transform.SetSiblingIndex(999);

        Character character = newImage.GetComponent<Character>();
        if (character)
            character.ChangeRanderOrder(100);

        RectTransform imageRect = newImage.AddComponent<RectTransform>();
        imageRect.anchoredPosition = new Vector2(InterruptAppearPositionList[0].x + imageRect.rect.width / 2f, InterruptAppearPositionList[0].y + imageRect.rect.height / 2f);
        imageRect.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 135f));
        imageRect.localScale *= 500f;

        if (InterruptAppearPositionList.Count == 1)
        {
            InterruptAppearPositionList.Add(new Vector2(InterruptAppearPositionList[0].x - imageRect.rect.width / 2f, InterruptAppearPositionList[0].y - imageRect.rect.height / 2f));
            InterruptAppearPositionList.Add(new Vector2(InterruptAppearPositionList[1].x - imageRect.rect.width / 2f, InterruptAppearPositionList[1].y - imageRect.rect.height / 2f));
        }
        
        InterruptUIInfo Info = new InterruptUIInfo();
        Info.UIObject = newImage;
        Info.Rect = imageRect;
        CreatedUIObjectList.Add(iProcessCurrentindex, Info);
    }

    public void Clear()
    {
        if (CreatedUIObjectList == null)
            return;

        foreach (var data in CreatedUIObjectList)
        {
            Destroy(data.Value.UIObject);
        }
        CreatedUIObjectList.Clear();

        for (int i = InterruptAppearPositionList.Count-1; i >= 1; --i)
        {
            InterruptAppearPositionList.RemoveAt(i);
        }
        iProcessCurrentindex = 0;
        fEndTime = 0f;
        bEnd = false;
    }

    private void Bbaekkom(InterruptProcessState eState, InterruptUIInfo Info)
    {
        if (Info.bEndAppear)
        {
            if (Info.eCurrentState < Info.eState)
            {
                Info.eCurrentState++;
                Info.bEndAppear = false;

                if(Info.eCurrentState == InterruptProcessState.MOVE)
                {
                    Info.Rect.localScale /= 5f;
                    Info.vStartMovePosition = new Vector2(Info.Rect.anchoredPosition.x, -(InterruptAppearPositionList[0].y));
                }
            }

            return;
        }

        Vector2 targetPosition = InterruptAppearPositionList[(int)eState];
        Info.Rect.anchoredPosition = Vector2.MoveTowards(Info.Rect.anchoredPosition, targetPosition, 80f * Time.deltaTime);
        if (Info.Rect.anchoredPosition == targetPosition)
        {
            Info.bEndAppear = true;
        }
    }

    private void Move(int posIndex, InterruptUIInfo Info)
    {
        if (posIndex < 0 || posIndex >= InterruptProcessPositionList.Count)
            return;

        if (Info.EndMoveStep == 2)
        {
            if (Info.eCurrentState < Info.eState)
            {
                Info.eCurrentState++;
                Info.fInitialScale = Info.Rect.localScale.x;
            }

            return;
        }

        Info.UIObject.transform.Rotate(Vector3.forward * 180f * Time.deltaTime);
        if (Info.EndMoveStep == 0)
        {
            Info.Rect.anchoredPosition = Vector2.MoveTowards(Info.Rect.anchoredPosition, Info.vStartMovePosition, 500f * Time.deltaTime);
            if (Info.Rect.anchoredPosition == Info.vStartMovePosition)
            {
                Info.EndMoveStep = 1;
                Info.vStartMovePosition = Info.Rect.anchoredPosition;
                Info.fJourneyMoveLength = Vector3.Distance(Info.vStartMovePosition, InterruptProcessPositionList[posIndex]);
                Info.fStartTime = Time.time;
            }
        }
        else if (Info.EndMoveStep == 1)
        {
            float distCovered = (Time.time - Info.fStartTime) * 500f;
            float fractionOfJourney = distCovered / Info.fJourneyMoveLength;

            Info.Rect.anchoredPosition = Vector2.Lerp(Info.vStartMovePosition, InterruptProcessPositionList[posIndex], fractionOfJourney) + Vector2.up * Mathf.Sin(fractionOfJourney * Mathf.PI) * 50f;
            if (fractionOfJourney >= 1.0f)
            {
                Info.EndMoveStep = 2;
            }
        }
    }

    private void GROW(InterruptProcessState eState, InterruptUIInfo Info)
    {
        if (Info.bGrow)
        {
            if (Info.eCurrentState < Info.eState)
            {
                Info.eCurrentState++;
                Info.bGrow = false;
            }
                
            return;
        }

        Info.Rect.localScale += Info.Rect.localScale * Time.deltaTime * 1.5f;
        if (Info.Rect.localScale.x >= Info.fInitialScale * 3f)
        {
            Info.bGrow = true;
            Info.fInitialScale = Info.Rect.localScale.x;
        }
    }
}
