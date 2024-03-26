using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class CharacterGamePlayBlock : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private SkeletonGraphic ske;

    [SerializeField, SpineAnimation(dataField = "ske")]
    private string idle;

    [SerializeField, SpineAnimation(dataField = "ske")]
    private string win;

    public GameObject zoomPos;

    void Start()
    {
        GameManager.instance.gTargetFollow = gameObject;
    }

    private void OnEnable()
    {
        Observer.PlayAnimWin += PlayAnimWin;
        Observer.PlayAnimIdle += PlayAnimIdle;
    }

    private void OnDisable()
    {
        Observer.PlayAnimWin -= PlayAnimWin;
        Observer.PlayAnimIdle -= PlayAnimIdle;
    }

    public void PlayAnimIdle()
    {
        ske.AnimationState.SetAnimation(0, idle, true);
    }

    public void PlayAnimWin()
    {
        if (!(SoundManager.Instance.heroJumpWin is null))
            SoundManager.Instance.PlaySound(SoundManager.Instance.heroJumpWin);
        ske.AnimationState.SetAnimation(0, win, true);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
