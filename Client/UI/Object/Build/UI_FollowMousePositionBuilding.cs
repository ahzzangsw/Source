using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FollowMousePositionBuilding : UIBase
{
    private Camera mainCamera = null;
    private SpriteRenderer spriteRenderer = null;

    protected override void Awake()
    {
        spriteRenderer.transform.localScale *= 9f;

        Vector3 newPosition = spriteRenderer.transform.localPosition;
        newPosition.y += 1f;
        spriteRenderer.transform.localPosition = newPosition;
    }

    protected override void Update()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;
    }

    public void SetBuilding(string spriteName)
    {
        if(spriteRenderer == null || mainCamera == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            mainCamera = Camera.main;
        }

        spriteRenderer.sprite = UIManager.Instance.GetSprite(spriteName);
    }
}
