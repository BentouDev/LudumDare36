using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MenuBase
{
    public Animator Anim;
    public string ShowAnim;
    public string HideAnim;
    
    public float Delay = 4;

    public override void OnStart()
    {
        base.OnStart();

        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        Anim.Play(ShowAnim);

        yield return new WaitForSeconds(Delay);

        Anim.Play(HideAnim);

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(0);
    }
}
