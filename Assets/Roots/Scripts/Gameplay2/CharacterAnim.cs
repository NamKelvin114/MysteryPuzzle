using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pancake;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

public class CharacterAnim : MonoBehaviour
{
    [SerializeField, SpineAnimation] public string initialIdle;
    public bool mixItemOnEnd;
    [ShowIf(nameof(mixItemOnEnd))] public TargetType itemMix;
    [SerializeField] private List<EnityElementAction> actions;
    [SerializeField] private EnityDefaulAmim defauls;

    public ElementAction listAnims(TargetType targetType)
    {
        var expr =
            from c in actions
            where c.idle == initialIdle
            select c;
        var action = expr.ToArray()[0];
        var animActions =
            from a in action.elementActions
            where a.targetType == targetType
            select a;
        return animActions.ToArray()[0];
    }

    public DefaultAnims defaultAnims()
    {
        var anims = defauls.defaultAnims.FirstOrDefault(t => t.idle == initialIdle);
        return anims;
    }
}

[Serializable]
public class EnityElementAction
{
    [SpineAnimation] public string idle;
    public List<ElementAction> elementActions;
}

[Serializable]
public class ElementAction
{
    public TargetType targetType;
    [SpineAnimation] public string[] anims;
    public AudioClip sound;
    public ParticleSystem fx;
    public UnityEvent<TargetType> callBack;
}

[Serializable]
public class EnityDefaulAmim
{
    [SerializeField] public List<DefaultAnims> defaultAnims;
}

[Serializable]
public class DefaultAnims
{
    [SpineAnimation] public string idle;
    [SpineAnimation] public string[] winAnims;
    public TypeRunListAnim TypeRunListAnimWin;
    [SpineAnimation] public string loseAnims;
    public TypeRunListAnim TypeRunListAnimLose;
    public AudioClip winAudio;
    public AudioClip loseAudio;
}

[Serializable]
public enum TargetType
{
    AdventureCloses,
    Apple,
    Banana,
    Backpack,
    HatAdventure,
    Mushroom,
    Fishbone,
    Water,
    MatchBox,
    MapComplete,
    PoisonWater
}

[Serializable]
public enum ExpectedType
{
    Expected,
    Unexpected
}

[Serializable]
public enum TypeRunListAnim
{
    Random,
    List
}