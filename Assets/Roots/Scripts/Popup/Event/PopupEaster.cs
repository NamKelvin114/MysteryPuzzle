using System;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityTimer;
using Worldreaver.UniUI;

public class PopupEaster : UniPopupBase
{
    public UniButton btnClose;
    public SkeletonGraphic[] pets;
    public SkeletonGraphic[] eggs;
    public TextMeshProUGUI[] txtValueUnlocks;
    public TextMeshProUGUI[] txtCostUnlocks;
    public TextMeshProUGUI[] txtMaxs;
    public GameObject[] effectUses;
    public GameObject[] effectSelectUses;
    public UniButton[] btnUnlocks;
    public Button[] btnSelects;
    public Image[] progresses;
    public UniButton[] btnUpgrades;
    public GameObject[] groupUpgrades;
    public TextMeshProUGUI txtCurrentCoin;
    public UniButton btnToShop;
    public ParticleSystem effect;
    public GameObject petSelectPanel;
    public SkeletonGraphic[] petSelects;
    public Button[] btnSwitchPetsLevel;
    public Button btnNoPet;

    public Sprite btnHatchAvaiable;
    public Sprite btnHatchDisable;

    private Action _actionClose;
    private Action<Action, Action, Action> _actionToShop;

    public void Initialized(Action actionClose, Action<Action, Action, Action> actionToShop)
    {
        petSelectPanel.SetActive(false);
        _actionClose = actionClose;
        _actionToShop = actionToShop;
        RefeshUI();
        btnClose.onClick.RemoveListener(OnCloseButtonPressed);
        btnClose.onClick.AddListener(OnCloseButtonPressed);

        btnToShop.onClick.RemoveListener(OnTopShopButtonPressed);
        btnToShop.onClick.AddListener(OnTopShopButtonPressed);
        UpdateCurrencyDisplay();

        foreach (var effectUse in effectUses) effectUse.SetActive(false);

        if (Data.currentPet != -1)
        {
            effectUses[Data.currentPet].SetActive(true);
        }
    }

    public void RefeshUI()
    {
        for (int i = 0; i < pets.Length; i++)
        {
            if (DataController.instance.petDataController.IsUnlocked(i))
            {
                //unlocked
                groupUpgrades[i].SetActive(true);
                btnSelects[i].gameObject.SetActive(true);
                btnUnlocks[i].gameObject.SetActive(false);
                btnUnlocks[i].transform.parent.gameObject.SetActive(false);
                var (level, semgment) = DataController.instance.petDataController.GetLevelAndSegement(i);

                pets[i].gameObject.SetActive(true);
                int index2 = i;
                try
                {
                    TryChangeSkin(index2);
                }
                catch (Exception)
                {
                    Timer.Register(0.1f, () => TryChangeSkin(index2));
                }

                void TryChangeSkin(int index)
                {
                    if (index == Data.currentPet)
                    {
                        pets[index].ChangeSkin($"Level{Data.petLevel}");
                    }
                    else
                    {
                        pets[index].ChangeSkin($"Level{level}");
                    }
                }

                eggs[i].gameObject.SetActive(false);

                RefreshUIPetUpgrade();
            }
            else
            {
                eggs[i].gameObject.SetActive(true);
                pets[i].gameObject.SetActive(false);
                groupUpgrades[i].SetActive(false);
                btnSelects[i].gameObject.SetActive(false);
                btnUnlocks[i].gameObject.SetActive(true);
                btnUnlocks[i].transform.parent.gameObject.SetActive(true);
                txtValueUnlocks[i].transform.parent.gameObject.SetActive(true);
                txtValueUnlocks[i].text = $"{DataController.instance.petDataController.CurrentNumberShard(i)}/{PetCollection.GetNumberShardNeedToBreak(i)}";
                if (DataController.instance.petDataController.IsAvaiableClaim(i))
                {
                    // avaiable to claim
                    btnUnlocks[i].interactable = true;
                    btnUnlocks[i].image.sprite = btnHatchAvaiable;
                }
                else
                {
                    // not enough shard
                    btnUnlocks[i].interactable = false;
                    btnUnlocks[i].image.sprite = btnHatchDisable;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateCurrencyDisplay() { txtCurrentCoin.text = $"{Utils.currentCoin}"; }

    private void RefreshUIPetUpgrade()
    {
        UpdateCurrencyDisplay();
        for (int i = 0; i < pets.Length; i++)
        {
            int index = i;
            var (level, semgment) = DataController.instance.petDataController.GetLevelAndSegement(index);
            progresses[index].fillAmount = (float) DataController.instance.petDataController.GetSegment(index) / 10f;
            if (level >= 5)
            {
                txtMaxs[index].gameObject.SetActive(true);
                btnUpgrades[index].interactable = false;
                btnUpgrades[index].image.sprite = btnHatchAvaiable;
                groupUpgrades[index].SetActive(false);
            }
            else
            {
                txtCostUnlocks[index].text = $"{PetCollection.GetCostUpgrade(level, semgment)}";
                if (Utils.currentCoin >= PetCollection.GetCostUpgrade(level, semgment))
                {
                    btnUpgrades[index].interactable = true;
                    btnUpgrades[index].image.sprite = btnHatchAvaiable;
                }
                else
                {
                    btnUpgrades[index].interactable = false;
                    btnUpgrades[index].image.sprite = btnHatchDisable;
                }
            }
        }
    }

    public void OnUpgradeButtonPressed(int index)
    {
        var (level, semgment) = DataController.instance.petDataController.GetLevelAndSegement(index);
        int cost = PetCollection.GetCostUpgrade(level, semgment);
        bool markFull = false;
        if (Utils.currentCoin >= cost)
        {
            Utils.currentCoin -= cost;
            UpdateCurrencyDisplay();
            semgment++;
            if (semgment > 9)
            {
                markFull = true;
                semgment = 0;
                level++;
                effect.GetComponent<RectTransform>().localPosition = pets[index].transform.parent.localPosition;
                effect.gameObject.SetActive(true);
                effect.Play(true);
                pets[index].ChangeSkin($"Level{level}");
                foreach (var use in effectUses)
                {
                    use.SetActive(false);
                }

                Data.currentPet = index;
                effectUses[index].SetActive(true);
                Data.petLevel = level;
                if (level >= 5)
                {
                    level = 5;
                }

                if (GamePopup.Instance.menuRoom != null) GamePopup.Instance.menuRoom.UpdateSkinPet();
                ;
            }
        }

        DataController.instance.petDataController.SetLevelAndSegement(index, level, semgment);
        var fill = DataController.instance.petDataController.GetSegment(index) / 10f;
        if (level >= 5)
        {
            btnUpgrades[index].interactable = false;
            btnUpgrades[index].image.sprite = btnHatchAvaiable;

            txtMaxs[index].gameObject.SetActive(true);
            groupUpgrades[index].SetActive(false);
        }
        else
        {
            txtCostUnlocks[index].text = $"{PetCollection.GetCostUpgrade(level, semgment)}";
            if (Utils.currentCoin >= PetCollection.GetCostUpgrade(level, semgment))
            {
                btnUpgrades[index].interactable = true;
                btnUpgrades[index].image.sprite = btnHatchAvaiable;
            }
            else
            {
                btnUpgrades[index].interactable = false;
                btnUpgrades[index].image.sprite = btnHatchDisable;
            }
        }

        if (markFull)
        {
            progresses[index].DOFillAmount(1, 0.5f).OnComplete(() => { progresses[index].fillAmount = 0; });
        }
        else
        {
            progresses[index].DOFillAmount(fill, 0.5f).OnComplete(() => { progresses[index].fillAmount = fill; });
        }


        txtCostUnlocks[index].text = $"{PetCollection.GetCostUpgrade(level, semgment)}";
    }

    public void OnClaimButtonPressed(int index)
    {
        btnUnlocks[index].gameObject.SetActive(false);
        btnUnlocks[index].transform.parent.gameObject.SetActive(false);
        // play animation break
        eggs[index].AnimationState.SetAnimation(0, "Break", false);
        Timer.Register(0.8f,
            () =>
            {
                eggs[index].gameObject.SetActive(false);
                pets[index].gameObject.SetActive(true);
                groupUpgrades[index].SetActive(true);
                btnSelects[index].gameObject.SetActive(true);
                RefreshUIPetUpgrade();
                OnUseButtonPressed(index, 1);
            });

        DataController.instance.petDataController.UnlockPet(index);
    }

    public void OnUseButtonPressed(int index, int level)
    {
        if (!DataController.instance.petDataController.IsUnlocked(index)) return;

        foreach (var effectUse in effectUses)
        {
            effectUse.SetActive(false);
        }

        effectUses[index].SetActive(true);
        Data.currentPet = index;
        Data.petLevel = level;

        if (GamePopup.Instance.menuRoom != null) GamePopup.Instance.menuRoom.UpdateSkinPet();
        ;

        foreach (var effectUse in effectSelectUses)
        {
            effectUse.SetActive(false);
        }

        for (int i = 0; i < effectSelectUses.Length; i++)
        {
            if (i + 1 == level)
            {
                effectSelectUses[i].SetActive(true);
                break;
            }
        }
    }

    public void OnSwitchButtonPressed(int index)
    {
        petSelectPanel.SetActive(true);
        foreach (var effectUse in effectSelectUses)
        {
            effectUse.SetActive(false);
        }

        if (Data.currentPet == -1)
        {
            effectSelectUses[5].SetActive(true);
        }

        for (int i = 0; i < petSelects.Length; i++)
        {
            petSelects[i].skeletonDataAsset = pets[index].skeletonDataAsset;
            petSelects[i].Initialize(true);
            petSelects[i].ChangeSkin($"Level{i + 1}");

            if (index == Data.currentPet && Data.petLevel - 1 == i)
            {
                effectSelectUses[i].SetActive(true);
            }

            if (DataController.instance.petDataController.GetLevel(index) >= i + 1)
            {
                btnSwitchPetsLevel[i].interactable = true;
                petSelects[i].color = Color.white;
            }
            else
            {
                btnSwitchPetsLevel[i].interactable = false;
                petSelects[i].color = Color.black;
            }
        }

        for (int i = 0; i < btnSwitchPetsLevel.Length; i++)
        {
            int j = i;
            btnSwitchPetsLevel[j].onClick.RemoveAllListeners();
            btnSwitchPetsLevel[j].onClick.AddListener(() => { OnUseButtonPressed(index, j + 1); });
        }
    }

    public void ClosePopupSwitchPet()
    {
        // update effect
        petSelectPanel.SetActive(false);
        RefeshUI();
    }

    public void NoPetButtonPressed()
    {
        Data.currentPet = -1;
        Data.petLevel = 1;

        foreach (var effectUse in effectUses)
        {
            effectUse.SetActive(false);
        }

        foreach (var effectUse in effectSelectUses)
        {
            effectUse.SetActive(false);
        }

        effectSelectUses[5].SetActive(true);


        if (GamePopup.Instance.menuRoom != null)
        {
            GamePopup.Instance.menuRoom.cutePet.gameObject.SetActive(false);
        }
    }

    private void OnCloseButtonPressed()
    {
        _actionClose?.Invoke();
        RefreshUIPetUpgrade();
    }

    private void OnTopShopButtonPressed()
    {
        _actionToShop?.Invoke(UpdateCurrencyDisplay, RefreshUIPetUpgrade, null);
        ((PopupShop) GamePopup.Instance.popupShopHandler).OnClose = RefreshUIPetUpgrade;
    }

    private void OnDisable() { effect.gameObject.SetActive(false); }
}