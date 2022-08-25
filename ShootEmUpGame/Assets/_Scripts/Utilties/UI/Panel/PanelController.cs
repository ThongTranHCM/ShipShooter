using UnityEngine;
using System.Collections.Generic;
using System;

public class PanelController : MonoBehaviour
{
    public Action onPanelWillDissappear;
    public Action onPanelDidDissappear;
    public Action onPanelWillAppear;
    public Action onPanelDidAppear;

    //For other PanelControllers which content is added into this PanelController
    public List<PanelController> childPanelControllers = new List<PanelController>();

    private RectTransform _rectTransform;
    [HideInInspector]
    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = (RectTransform)transform;
            }
            return _rectTransform;
        }
    }
    private CanvasGroup _canvasGroup;
    [HideInInspector]
    public CanvasGroup canvasGroup
    {
        get
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }

            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            return _canvasGroup;
        }

    }

    // return The panel, presented by this panel with func PresentPanel
    [HideInInspector]
    public PanelController presentedPanel;

    // return The panel, present this panel with func PresentPanel
    [HideInInspector]
    public PanelController presentingPanel;

    [HideInInspector]
    public bool isVisible;

    #region Method to be Overided by SubClass
    // Methods for overiding by subclass
    // They will be invoked if the Panel is pushed/poped by PanelNavigationController or Presented/Dismissed by PanelController, or Showed by TabbarController
    public virtual void PanelWillAppear()
    {
        if (presentedPanel != null)
        {
            presentedPanel.PanelWillAppear();
        }
        gameObject.SetActive(true);

        if (childPanelControllers != null && childPanelControllers.Count > 0)
        {
            for (int i = 0; i < childPanelControllers.Count; i++)
            {
                childPanelControllers[i].PanelWillAppear();
            }
        }

        if (onPanelWillAppear != null)
        {
            onPanelWillAppear();
        }
    }

    public virtual void PanelDidAppear()
    {
        if (presentedPanel != null)
        {
            presentedPanel.PanelDidAppear();
        }

        if (childPanelControllers != null && childPanelControllers.Count > 0)
        {
            for (int i = 0; i < childPanelControllers.Count; i++)
            {
                childPanelControllers[i].PanelDidAppear();
            }
        }

        isVisible = true;

        if (onPanelDidAppear != null)
        {
            onPanelDidAppear();
        }
    }

    public virtual void PanelWillDisappear()
    {
        if (presentedPanel != null)
        {
            presentedPanel.PanelWillDisappear();
        }
        isVisible = false;

        if (childPanelControllers != null && childPanelControllers.Count > 0)
        {
            for (int i = 0; i < childPanelControllers.Count; i++)
            {
                childPanelControllers[i].PanelWillDisappear();
            }
        }

        if (onPanelWillDissappear != null)
        {
            onPanelWillDissappear();
        }
    }

    public virtual void PanelDidDisappear()
    {
        if (presentedPanel != null)
        {
            presentedPanel.PanelDidDisappear();
        }
        gameObject.SetActive(false);

        if (childPanelControllers != null && childPanelControllers.Count > 0)
        {
            for (int i = 0; i < childPanelControllers.Count; i++)
            {
                childPanelControllers[i].PanelDidDisappear();
            }
        }

        if (onPanelDidDissappear != null)
        {
            onPanelDidDissappear();
        }
    }
    #endregion

    #region Present/Show And PanelController
    // Make the panel become child of this panel, and show it
    public void PresentPanel(PanelController panel, bool animated, Action OnFinished = null)
    {
        if (this.presentedPanel != null)
            DismissPanel(false);
        panel.PanelWillAppear();
        AddChildPanel(panel);
        if (animated)
        {
            FadeIn(panel, () => {
                panel.PanelDidAppear();
                if (OnFinished != null)
                    OnFinished();
            });

        }
        else
        {
            panel.PanelDidAppear();
            panel.canvasGroup.alpha = 1;
            if (OnFinished != null)
                OnFinished();
        }
    }

    // Hide the presented panel
    public void DismissPanel(bool animated, Action OnFinished = null)
    {
        this.PanelWillAppear();
        presentedPanel.PanelWillDisappear();
        if (animated)
        {
            presentedPanel.canvasGroup.interactable = false;
            FadeOut(presentedPanel, () => {
                presentedPanel.gameObject.SetActive(false);
                presentedPanel.PanelDidDisappear();
                this.PanelDidAppear();
                presentedPanel.canvasGroup.interactable = true;
                RemoveChildPanel(presentedPanel);
                if (OnFinished != null)
                    OnFinished();
            });
        }
        else
        {
            this.PanelDidAppear();
            presentedPanel.PanelDidDisappear();
            RemoveChildPanel(presentedPanel);
            if (OnFinished != null)
                OnFinished();
        }
    }

    // Show this panel On the panel position
    public void ShowOnPanel(RectTransform panel, bool animated, Action OnFinished = null)
    {
        this.PanelWillAppear();

        this.transform.SetParent(panel, false);
        this.rectTransform.position = panel.position;
        this.rectTransform.localScale = Vector3.one;
        //this.rectTransform.SetSize(panel.GetSize());
        this.transform.SetAsLastSibling();
        this.gameObject.SetActive(true);

        if (animated)
        {
            FadeIn(this, () => {
                this.PanelDidAppear();
                if (OnFinished != null)
                    OnFinished();
            });

        }
        else
        {
            this.PanelDidAppear();
            this.canvasGroup.alpha = 1;
            if (OnFinished != null)
                OnFinished();
        }
    }

    public void Hide(bool animated, Action OnFinished = null)
    {
        if (animated)
        {
            canvasGroup.interactable = false;
            this.PanelWillDisappear();
            FadeOut(this, () => {
                canvasGroup.interactable = true;
                this.gameObject.SetActive(false);
                canvasGroup.alpha = 1;
                this.PanelDidDisappear();
                if (this.presentingPanel != null)
                    this.presentingPanel.RemoveChildPanel(this);
                if (OnFinished != null) OnFinished();
            });
        }
        else
        {
            this.PanelWillDisappear();
            this.gameObject.SetActive(false);
            this.PanelDidDisappear();
            if (this.presentingPanel != null)
                this.presentingPanel.RemoveChildPanel(this);
            if (OnFinished != null) OnFinished();
        }

    }


    void AddChildPanel(PanelController panel)
    {
        panel.transform.SetParent(transform, false);
        panel.rectTransform.position = rectTransform.position;
        panel.rectTransform.localScale = Vector3.one;
        //panel.rectTransform.SetSize(rectTransform.GetSize());
        panel.transform.SetAsLastSibling();
        panel.gameObject.SetActive(true);

        panel.presentingPanel = this;
        this.presentedPanel = panel;

    }

    void RemoveChildPanel(PanelController panel)
    {
        panel.presentingPanel = null;
        this.presentedPanel = null;
    }
    #endregion

    #region ANIMATION HELPER
    float fadeTime = 0.5f;

    Action fadeInCallback;
    CanvasGroup fadedInCanvasGroup = null;
    public void FadeIn(PanelController panel, Action callback)
    {
        fadedInCanvasGroup = panel.canvasGroup;
        //LeanTween
        //Update OnFadeInUpdate();
        fadedInCanvasGroup.alpha = 1;
        OnFadeInFinished();
        fadeInCallback = callback;
    }
    void OnFadeInUpdate(float percent)
    {
        fadedInCanvasGroup.alpha = percent;
    }
    void OnFadeInFinished()
    {
        if (fadeInCallback != null)
            fadeInCallback();
    }

    Action fadeOutCallback;
    CanvasGroup fadedOutCanvasGroup = null;
    public void FadeOut(PanelController panel, Action callback)
    {
        fadedOutCanvasGroup = panel.canvasGroup;
        //LeanTween
        //Update OnFadeOutUpdate();
        fadedInCanvasGroup.alpha = 0;
        OnFadeOutFinished();
        fadeOutCallback = callback;
    }
    void OnFadeOutUpdate(float percent)
    {
        fadedOutCanvasGroup.alpha = percent;
    }
    void OnFadeOutFinished()
    {
        if (fadeOutCallback != null)
            fadeOutCallback();
    }

    #endregion
}
