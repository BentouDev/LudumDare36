using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageMenu : MenuBase
{
    public Text MessageBox;
    public Button ConfirmButton;
    public Button CancelButton;

    private System.Action CancelAction;
    private System.Action ConfirmAction;

    private bool ConfirmClicked;
    private bool CancelClicked;

    public bool HasCancelAction { get; protected set; }

    private void SetMessage(string message)
    {
        if (MessageBox)
        {
            MessageBox.text = message;
        }
    }

    private void SetConfirmAction(System.Action action)
    {
        ConfirmClicked = false;
        ConfirmAction = action;

        if (ConfirmButton)
        {
            ConfirmButton.onClick.RemoveAllListeners();
            ConfirmButton.onClick.AddListener(OnConfrimButtonClicked);
        }
    }

    private void SetCancelAction(System.Action action)
    {
        HasCancelAction = action != null;

        CancelClicked = false;
        CancelAction = action;

        if (HasCancelAction && CancelButton)
        {
            CancelButton.onClick.RemoveAllListeners();
            CancelButton.onClick.AddListener(OnCancelButtonClicked);
        }
    }

    public void SwitchTo(string message, System.Action confirm, System.Action cancel = null)
    {
        SetMessage(message);
        SetConfirmAction(confirm);
        SetCancelAction(cancel);

        SwitchTo();
    }

    public override void OnStart()
    {
        base.OnStart();

        CancelButton.gameObject.SetActive(HasCancelAction);
    }

    protected void OnConfrimButtonClicked()
    {
        ConfirmClicked = true;
        StartCoroutine(AnimHide(AnimTime));
    }

    protected void OnCancelButtonClicked()
    {
        CancelClicked = true;
        StartCoroutine(AnimHide(AnimTime));
    }

    protected override void OnHidden()
    {
        if(CancelClicked)
        {
            if (HasCancelAction && CancelAction != null)
            {
                CancelAction();
            }
        }
        else if (ConfirmClicked)
        {
            if (ConfirmAction != null)
            {
                ConfirmAction();
            }
        }
    }
}
