using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using Worldreaver.UniUI;

public class PopupTransition : UniPopupBase
{
    [SerializeField] private SkeletonGraphic transSke;
    [SerializeField, SpineAnimation(dataField = "ske")]
    private string transIn;
    [SerializeField, SpineAnimation(dataField = "ske")]
    private string transOut;
    public void Initialized()
    {

        #if !UNITY_EDITOR
        GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
        #endif
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        DoTrasnsition();
    }
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
#endif
    }

    void DoTrasnsition()
    {
        transSke.AnimationState.ClearTracks();
        transSke.AnimationState.Complete += Complete;
        transSke.AnimationState.SetAnimation(0, transIn, false);
        transSke.AnimationState.AddAnimation(0, transOut, false, 0);
    }
    void Complete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == transOut)
        {
            transform.gameObject.SetActive(false);
            transSke.AnimationState.Complete -= Complete;
        }
    }
}
