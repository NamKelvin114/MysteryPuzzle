using System;
using System.Collections.Generic;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityTimer;
using Random = UnityEngine.Random;

public abstract class BaseRoom : MonoBehaviour
{
    [SerializeField] protected int roomId;
    [SerializeField] protected GameObject effectUnlock;
    [SerializeField] protected SkeletonGraphic skeleton;
    [SerializeField] protected GameObject[] itemLocks;
    [SerializeField] protected OpenItem openItem;
    public SkeletonGraphic cutePet;
    [SerializeField] protected Button btnContinue;
    [SerializeField, SpineAnimation] protected string idle;
    [SerializeField, SpineAnimation] protected string dance;
    [SerializeField] private List<AnimInHome> idleHomeAnimList;
    public SkeletonGraphic pet;

    public SkeletonGraphic Skeleton => skeleton;
    public Action ActionActiveCameraEffect { get; set; }
    public Button BtnContinue => btnContinue;
    public int RoomId => roomId;

    protected virtual void ShowLight(int i)
    {
    }

    public void UpdateSkinPet()
    {
        if (Data.currentPet == -1)
        {
            cutePet.gameObject.SetActive(false);
        }
        else
        {
            cutePet.skeletonDataAsset = PetCollection.GetSkeletonAsset(Data.currentPet);
            cutePet.gameObject.SetActive(true);
            cutePet.Initialize(true);
            cutePet.ChangeSkin($"Level{Data.petLevel}");
        }
    }

    public void Initialized(bool isDance)
    {
        btnContinue.onClick.RemoveAllListeners();
        btnContinue.onClick.AddListener(OnContinueButtonPressed);

        for (int i = 0; i < 8; i++)
        {
            if (DataController.instance.SaveItems[i + 8 * roomId].unlock)
            {
                itemLocks[i].SetActive(true);
                ShowLight(i);
            }
            else
            {
                itemLocks[i].SetActive(false);
            }
        }

        skeleton.startingLoop = true;

        if (isDance)
        {
            int animRan = Random.Range(0, idleHomeAnimList.Count);
            var item = idleHomeAnimList[animRan];
            skeleton.AnimationState.SetAnimation(0, item.anim, true);
            // if (item.isMoving)
            // {
            //     skeleton.GetComponent<MoveInHome>().DoMove();
            // }
            // else skeleton.GetComponent<MoveInHome>().Stop();
        }
        else
        {
            skeleton.AnimationState.SetAnimation(0, idle, true);
        }


        //skeleton.Initialize(true);

        //skeleton.Initialize(true);

        // if (skeleton.skeletonDataAsset != null) {
        //     foreach (AtlasAssetBase aa in skeleton.skeletonDataAsset.atlasAssets) {
        //         if (aa != null) aa.Clear();
        //     }
        //     skeleton.skeletonDataAsset.Clear();
        // }
        // skeleton.skeletonDataAsset.GetSkeletonData(true);
        //
        // skeleton.LateUpdate();

        //skeleton.GetComponent<SkeletonUtility>().CollectBones();
        //skeleton.Initialize(true);
        //ChangeSkin(HeroData.SkinHeroByIndex(Data.currentHero).Item1);
        if (isDance)
        {
            UpdateSkinPet();
        }
        else
        {
            cutePet.gameObject.SetActive(false);
        }
    }


    public void ActiveDance()
    {
        skeleton.startingLoop = true;
        skeleton.startingAnimation = "Home";
        skeleton.Initialize(true);
        //ChangeSkin(HeroData.SkinHeroByIndex(Data.currentHero).Item1);
        UpdateSkinPet();
    }

    private void OnContinueButtonPressed()
    {
        GamePopup.Instance.ShowPopupMoney();
        gameObject.SetActive(false);
        GamePopup.Instance.CanvasRoom.worldCamera.depth = 0;
        ActionActiveCameraEffect?.Invoke();
        SoundClickButton();
    }

    public void SoundClickButton()
    {
        var sound = SoundManager.Instance;
        if (sound != null)
        {
            sound.StopSound();
            sound.audioSouceBG.volume = 1f;
            sound.PlaySound(SoundManager.Instance.acClick);
        }
    }

    public virtual void UnlockItem(int index, Action onCompleted = null, Action actionActiveCameraEffect = null)
    {
        ActionActiveCameraEffect = actionActiveCameraEffect;
        if (index < 0 || index >= 8)
        {
            return;
        }

        openItem.gameObject.SetActive(true);
        openItem.Initialized(Unlocked,
            ((ERoomItem)(index + roomId * 8)).NameSkin(),
            ItemCollection.Instance.infos[index + roomId * 8],
            itemLocks[index].transform.position);

        itemLocks[index].SetActive(false);

        void Unlocked()
        {
            effectUnlock.transform.position = itemLocks[index].transform.position;
            effectUnlock.SetActive(true);
            Observable.Timer(TimeSpan.FromSeconds(1.5f)).Subscribe(_ => effectUnlock.SetActive(false)).AddTo(this);

            itemLocks[index].SetActive(true);
            Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
                itemLocks[index].transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 1f, 5, 0.5f)).AddTo(this);

            // if (Utils.showNewWorld)
            // {
            //     Observable.Timer(TimeSpan.FromSeconds(3f))
            //         .Subscribe(_ =>
            //         {
            //             // GameManager.instance.FadeIn2(0.5f,
            //             //     () =>
            //             //     {
            //             //         Utils.isHardMode = false;
            //             //         if (ObjectPoolerManager.Instance != null) ObjectPoolerManager.Instance.ClearAllPool();
            //             //
            //             // if (onCompleted != null) // skip by video
            //             // {
            //             //     Utils.CurrentLevel += 1;
            //             // }
            //             //
            //             //         GamePopup.Instance.ShowChooseLevelPopup(null);
            //             //         (GamePopup.Instance.popupChooseLevelHandler as PopupChooseLevel).ActiveCutScene(true,
            //             //             () =>
            //             //             {
            //             //                 GamePopup.Instance.ShowRoomGameplay(hide: true, force: true);
            //             //                 GamePopup.Instance.CanvasRoom.worldCamera.depth = 0;
            //             //                 UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(Constants.CUTSCENE_SCENE_NAME);
            //             //             });
            //             //     });
            //         })
            //         .AddTo(this);
            // }
            // else
            // {
            //     onCompleted?.Invoke();
            //     Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ => btnContinue.gameObject.SetActive(true));
            // }

            Observable.Timer(TimeSpan.FromSeconds(1f))
                .Subscribe(_ =>
                {
                    if (Utils.showNewWorld)
                    {
                        GamePopup.Instance.ShowRoomGameplay(force: true, action: ActionActiveCameraEffect);

                        Observable.Timer(TimeSpan.FromSeconds(1.5f))
                            .Subscribe(__ => GamePopup.Instance.currentRoom.btnContinue.gameObject.SetActive(true))
                            .AddTo(GamePopup.Instance.currentRoom);
                    }
                    else
                    {
                        btnContinue.gameObject.SetActive(true);
                    }

                    onCompleted?.Invoke();
                })
                .AddTo(this);

            //skeleton.AnimationState.SetAnimation(0, dance, true);

            DataController.instance.SaveItems[index + 8 * roomId].unlock = true;
            DataController.instance.SaveItem();
        }
    }

    private void OnDisable()
    {
        btnContinue.gameObject.SetActive(false);
        try
        {
            //skeleton.AnimationState.SetAnimation(0, "Idle", true);
            skeleton.startingLoop = true;
            skeleton.startingAnimation = idle;
            skeleton.Initialize(true);
        }
        catch (Exception)
        {
            //
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="skinName"></param>
    public void ChangeSkin(string skinName)
    {
        try
        {
            if (skeleton != null) skeleton.ChangeSkin(skinName);

            var resutl = HeroData.HeroHasPetByIndex(Data.currentHero);
            if (resutl.Item1)
            {
                pet.gameObject.SetActive(true);
                pet.ChangeSkin(resutl.Item2);
            }
            else
            {
                pet.gameObject.SetActive(false);
            }
        }
        catch (Exception)
        {
            Timer.Register(0.1f, () => ChangeSkin(skinName));
        }
    }
}

[Serializable]
public class AnimInHome
{
    [SpineAnimation] public string anim;
    public bool isMoving;
}