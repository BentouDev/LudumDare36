using UnityEngine;
using System.Collections.Generic;

public class HealthBar : MonoBehaviour
{
    public Pawn Reference;

    private float BaseWidth;
    private RectTransform Rect;

    void Start()
    {
        Rect = GetComponent<RectTransform>();
        BaseWidth = Rect.offsetMax.x;
    }

    void Update()
    {
        float ratio = 0;

        if (Reference)
        {
            ratio = Reference.Data.Health / 2.0f;
        }

        Rect.offsetMax = new Vector2(BaseWidth * ratio, Rect.offsetMax.y);
    }
}
