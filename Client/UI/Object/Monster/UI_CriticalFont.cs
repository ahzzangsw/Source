using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_CriticalFont : UIBase
{
    [SerializeField] private float Yfit = 0f;
    [SerializeField] private float UpSpeed = 0f;
    [SerializeField] private float alphaSpeed = 0f; 

    private TextMeshPro text = null;
    private Color alpha = Color.clear;
    private bool bEnabled = false;

    protected override void Awake()
    {
        text = GetComponent<TextMeshPro>();
        if (text)
            alpha = text.color;
    }

    protected override void Update()
    {
        if (!bEnabled)
            return;

        transform.Translate(new Vector3(0f, UpSpeed * Time.deltaTime, 0f));

        alpha.a = Mathf.Lerp(alpha.a, 0, alphaSpeed * Time.deltaTime);
        text.color = alpha;
    }

    public void SetUp(Vector3 position, int Damage)
    {
        if (text == null)
        {
            DestroyCriticalFont();
            return;
        }

        position.y += Yfit;
        position.z = -1;
        transform.position = position;
        text.text = Oracle.ConvertNumberDigit(Damage);

        bEnabled = true;
        Invoke("DestroyCriticalFont", 1f);

        UnlockManager.Instance.AddCriticalCount();
    }

    private void DestroyCriticalFont()
    {
        Destroy(gameObject);
    }
}
