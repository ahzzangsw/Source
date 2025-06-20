using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 커서는 UI지만 UIBase를 상속받지않고 별도로 동작하자
public class UI_Cursor : MonoBehaviour
{
    void Awake()
    {
    }

    public void SetCursor()
    {
        Sprite cursorSprite = ResourceAgent.Instance.GetCursorSprite(OptionManager.Instance.iCursorIndex);
        if (cursorSprite == null)
        {
            cursorSprite = ResourceAgent.Instance.GetCursorSprite(0);
        }

        Texture2D texture = cursorSprite.texture;
        //Texture2D texture = new Texture2D((int)cursorSprite.rect.width, (int)cursorSprite.rect.height);
        //Color[] newColors = cursorSprite.texture.GetPixels((int)cursorSprite.textureRect.x,
        //                                                        (int)cursorSprite.textureRect.y,
        //                                                        (int)cursorSprite.textureRect.width,
        //                                                        (int)cursorSprite.textureRect.height);
        //texture.SetPixels(newColors);
        //texture.Apply();

        Cursor.SetCursor(texture, Vector2.zero, CursorMode.ForceSoftware);
    }

    void Update()
    {
    }
}
