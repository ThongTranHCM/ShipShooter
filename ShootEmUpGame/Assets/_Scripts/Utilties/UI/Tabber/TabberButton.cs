using UnityEngine;
using System.Collections;

public class TabberButton : MonoBehaviour
{
    public enum TabButtonState
    {
        Normal,
        Highlighted
    }

    public Animator anim;

    TabButtonState currentState;

    public void SetTabState(TabButtonState state)
    {
        if (currentState != state)
        {
            currentState = state;
            if (state == TabButtonState.Normal)
            {
                if (anim != null) anim.SetTrigger("Normal");
            }
            else
            {
                if (anim != null) anim.SetTrigger("Highlight");
            }
        }
    }
}
