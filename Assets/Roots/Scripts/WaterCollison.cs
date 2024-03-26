using System;
using System.Collections;
using UniRx;
using UnityEngine;
using Worldreaver.Timer;

public class WaterCollison : Unit
{
    private bool _flagVibrate;

    private ParticleSystem fxPoisionGlow;
    [SerializeField] private Material poisionMaterial;
    private bool isWaterPoison = false;

    void ChangePoisionWater()
    {
        var fxGlowMain = fxPoisionGlow.main;
        fxGlowMain.startColor = new Color(0.1411765f, 0.3686275f, 0.09411765f);
        sp.color = new Color(0.1411765f, 0.3686275f, 0.09411765f);
        check.gameObject.tag = "PoisonWater";
        fxPoisionGlow.GetComponent<ParticleSystemRenderer>().material = poisionMaterial;
    }

    private void Start()
    {
        isWaterPoison = check.gameObject.CompareTag("PoisonWater");
        fxPoisionGlow = effect2.GetComponent<ParticleSystem>();
        Observer.UnActiveWaterPosion += UnactiveWater;
    }

    void UnactiveWater()
    {
        if (isWaterPoison) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ice"))
        {
            var current = collision.GetComponent<ChildUnit>();
            if (current != null && current.myUnit != null)
            {
                current.myUnit.ChangeStone(true);
            }

            ChangeStone(true);
        }
        else if (collision.CompareTag("Trap_Lava"))
        {
            var current = collision.GetComponent<ChildUnit>();
            current.myUnit.ChangeStone();
            if (!MapLevelManager.Instance.isGameplay1)
            {
                var target = GetComponentInParent<Target>();
                if (target != null) MapLevelManager.Instance.OnTargetOut(target);
            }

            ChangeStone();
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Tag_Stone") && collision.gameObject.name != "BigStone" &&
                 collision.gameObject.name != "BigStoneBomb")
        {
            if (collision.CompareTag("Tag_Stone") && collision.GetComponentInParent<MineItem>())
            {
                return;
            }

            Vibrate();
            var b = false;
            var unit = collision.GetComponentInParent<Unit>();
            if (unit && unit.StoneState == EStoneState.IceStone)
            {
                b = true;
            }

            ChangeStone(b);
            if (!MapLevelManager.Instance.isGameplay1)
            {
                var target = GetComponentInParent<Target>();
                if (target != null) MapLevelManager.Instance.OnTargetOut(target);
            }
        }

        void Vibrate()
        {
            if (!_flagVibrate)
            {
                _flagVibrate = true;
                if (GameManager.instance.TimerChangeStone == null || !GameManager.instance.TimerChangeStone.IsPlaying)
                {
                    GameManager.instance.TimerChangeStone = new Timer();
                    GameManager.instance.TimerChangeStone.FinishedAsObservable.Subscribe(_ =>
                    {
                        if (GameManager.instance.TimerChangeStone != null)
                        {
                            GameManager.instance.TimerChangeStone.Stop();
                            GameManager.instance.TimerChangeStone = null;
                        }
                    });
                    GameManager.instance.TimerChangeStone.Start(0.2f);
                    GameManager.instance.RigidButton();
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PoisonWater") && !isWaterPoison)
        {
            isWaterPoison = true;
            ChangePoisionWater();
            var target = GetComponentInParent<Target>();
            if (target != null)
            {
                target.TargetType = TargetType.PoisonWater;
                target.ExpectedType = ExpectedType.Unexpected;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void PlaySoundLava()
    {
        if (SoundManager.Instance == null) return;

        StartCoroutine(PlaySoundLavaApear());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator PlaySoundLavaApear(float delay = 0.2f)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.PlaySound(SoundManager.Instance.acLavaApear);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (GameManager.instance.FlagSoundWaterStickbarrie) return;

        if (other.gameObject.CompareTag("StickBarrie") &&
            other.gameObject.GetComponent<StickBarrier>()?.beginMove == true)
        {
            GameManager.instance.FlagSoundWaterStickbarrie = true;
            PlaySoundLava();
        }
    }

    private void OnDestroy()
    {
        Observer.UnActiveWaterPosion -= UnactiveWater;
    }
}