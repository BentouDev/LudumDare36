using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    public Button Exit;

    void Start()
    {
#if UNITY_WEBGL
        Exit.gameObject.SetActive(false);
#endif
    }

    public void DoExit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
#else
        Application.Quit();
#endif
    }
}
