using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public abstract class MenuBase : GUIBase
{
    public Selectable FirstToSelect;
    protected MenuController Controller;
    protected CanvasGroup Canvas;

    public override bool IsGameplayGUI { get { return false; } }

    public float AnimTime;
    
    public override void Hide()
    {
        base.Hide();
        gameObject.SetActive(false);
    }

    public void Init(MenuController menuController)
    {
        Controller = menuController;
        Canvas = GetComponent<CanvasGroup>();

        Canvas.interactable = false;
        Canvas.alpha = 0;
    }

    public void SwitchTo()
    {
        Controller.SwitchToMenu(this);
    }

    public virtual void OnEnd()
    {
        Canvas.interactable = false;
        Canvas.alpha = 0;

        gameObject.SetActive(false);
    }

    public virtual void OnStart()
    {
        gameObject.SetActive(true);

        StartCoroutine(AnimShow(AnimTime));
    }

    protected virtual void OnShown()
    { }

    protected virtual void OnHidden()
    { }

    protected IEnumerator AnimShow(float animDuration)
    {
        float elapsed = 0;
        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            var ratio = Mathf.Lerp(0, 1, elapsed / animDuration);
            Canvas.alpha = ratio;

            yield return null;
        }

        Canvas.interactable = true;
        Canvas.alpha = 1;

        if (FirstToSelect)
            StartCoroutine(SelectFirst(FirstToSelect));

        OnShown();
    }

    protected IEnumerator AnimHide(float animDuration)
    {
        Canvas.interactable = false;

        float elapsed = 0;
        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            var ratio = Mathf.Lerp(1, 0, elapsed / animDuration);
            Canvas.alpha = ratio;

            yield return null;
        }

        Canvas.alpha = 0;

        OnHidden();

        gameObject.SetActive(false);
    }

    protected IEnumerator SelectFirst(Selectable select)
    {
        yield return new WaitForEndOfFrame();

        EventSystem.current.SetSelectedGameObject(null);
        FirstToSelect.Select();
    }
}
