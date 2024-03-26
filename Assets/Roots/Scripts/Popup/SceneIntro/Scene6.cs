using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Spine.Unity;
using UnityEngine;

public class Scene6 : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic mainGirl;
    [SerializeField, SpineAnimation] private string planeAnim;
    [SerializeField] private TransScene transScene;
    [SerializeField] private TransScene transScene2;
    private float value = 1f;
    private Color _color;
    private void OnEnable()
    {
        _color = new Color();
        _color = mainGirl.color;
    }
    void Start()
    {
        //SoundManager.Instance.PlaySound(SoundManager.Instance.planeSound);
        var doAnim = mainGirl.AnimationState.SetAnimation(0, planeAnim, true);
    }
    void DoneScene()
    {
        EndScene();
        transScene.DoTransScene(Done);
        transScene2.DoTransScene(null);
    }
    void EndScene()
    {
        DOTween.To(() => value, x => value = x, 0, 0.5f).OnUpdate((() =>
        {
            _color.a = value;
            mainGirl.color = _color;
        }));
    }
    void Done()
    {
        Observer.nextIntroScene?.Invoke(this.rectTransform());
    }
}
