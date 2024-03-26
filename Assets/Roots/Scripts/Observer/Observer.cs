using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Purchasing;

public static class Observer
{
    #region ActionPopup

    public static Action<Action> UIMove;
    public static Action<SubTask, float> ClaimTask;
    public static Action SaveCurrencyTotal;
    public static Action<SubTask> checkLastTask;
    public static Action<bool> CloseTask;
    public static Action<bool> ButtonTaskActiveNoti;
    public static Action HightLightGiftBoxCollection;
    public static Action<bool> CurrencyTotalChanged;
    public static Action CheckToActiveNotiBtnTask;
    public static Action NextMapTask;
    public static Action<bool> CheckSubTaskMove;
    public static Action<GameObject> AddFromPosiGenerationCoin;
    public static Action UpdateProcess;
    public static Action<int> UpdateBonusAdsButton;
    public static Action<GiftTask> UpdateGiftReward;
    public static Action ShowTaskToturial;
    public static Action HidePopupTutorialJigSaw;
    public static Action ShowCollectionTutorial;
    public static Action<Vector3> correctPeacePosi;

    #endregion

    #region Gameplay2

    public static Action<bool> playerEndGameplay2;
    public static Action<bool> playerDieGameplay2;
    public static Action<Transform> setPostionOfClothe;
    public static Action ShakeCamera;
    public static Action CheckWinTargetPercent;
    public static Action ActiveFxMatch;
    public static Action UnActiveWaterPosion;

    #endregion

    #region ActionIntro

    public static Action<RectTransform> nextIntroScene;
    public static Action CompleteIntro;

    #endregion

    #region GamePlayBlock

    public static Action PlayAnimWin;
    public static Action PlayAnimIdle;
    public static Action StartWinFollow;
    public static Action<List<JigsawPiece>> MoveToTracer;
    public static Action<JigsawPiece> PieceInPlace;

    #endregion

    #region cutscene

    public static Action endCutscene1;
    public static Action endCutscene2;
    public static Action endCutscene3;
    public static Action endCutscene4;

    #endregion

    #region Skin

    public static Action CurrentSkinChanged;
    public static Action<String> ShowSkin; // only use for shop
    public static Action<String, SkinType> UseSkin; // only use for shop
    public static Action<SkinData> UnlockSkin;

    public static Action CurrentSkinPinChanged;
    public static Action<String> ShowSkinPin; // only use for shop
    public static Action<String> UseSkinPin;

    #endregion

    #region Task

    public static Action ResetTaskTempValue;
    public static Action UpdateTaskValue;
    public static Action<ETaskType> UpdateTempValue;
    public static Action ContinueTutorial;
    public static Action BackToMenuInTutorual;

    #endregion

    #region Warning

    public static Action<bool> ShowHideTaskWarning;
    public static Action ShowHideShopWarning;

    #endregion

    #region MyRegion

    public static Action<bool> ShowHideButtonTutorial;
    public static Action StopCheckPinLoseCondition;

    #endregion
}