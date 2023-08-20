using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCustomCursor : MonoBehaviour
{
    [SerializeField] Texture2D cursorTexture;

    void Awake()
    {
#if UNITY_WEBGL
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
#else
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
#endif
    }
}
