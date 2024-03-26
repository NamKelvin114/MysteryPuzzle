using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum EStoneState
    {
        None,
        IceStone,
        Stone,
    }

    public Sprite[] spriteGem;
    public MapLevelManager.SPAWNTYPE spawnType;
    public GameObject effect, effect2;
    public SpriteRenderer sp;
    public Rigidbody2D rid;
    public bool activeChangeStone;
    public GameObject check;
    public Collider2D colliderBegin;
    public SpriteRenderer stoneRenderer;
    public Sprite stoneSprite;
    public Sprite iceStoneSprite;

    public EStoneState StoneState { get; private set; } = EStoneState.None;

    public void DisableSprite()
    {
        if (spriteGem.Length > 0)
        {
            _randomDisplayEffect = Random.Range(0, spriteGem.Length);
            sp.sprite = spriteGem[_randomDisplayEffect];
        }
    }

    public virtual void ChangeStone(bool isIce = false)
    {
        if (!activeChangeStone)
        {
            colliderBegin.gameObject.layer = 9;
            rid.sharedMaterial = GameManager.instance.matStone;
            colliderBegin.sharedMaterial = GameManager.instance.matStone;
            activeChangeStone = true;
            StoneState = isIce ? EStoneState.IceStone : EStoneState.Stone;
            StartCoroutine(DelayChangeStone(isIce));
        }
    }

    protected virtual IEnumerator DelayChangeStone(bool isIce = false)
    {
        yield return new WaitForSeconds(0.1f);

        if (check != null) check.SetActive(false);
        stoneRenderer.gameObject.SetActive(true);
        ChangeStoneType(isIce);
        sp.gameObject.SetActive(false);
        if (effect != null) effect.SetActive(false);
        if (effect2 != null) effect2.SetActive(false);
        int randomeffect = Random.Range(0, 100);
        if (randomeffect < 10)
        {
            if (ObjectPoolerManager.Instance != null)
            {
                GameManager.instance.PlaySoundLavaOnWater();
                GameObject g = ObjectPoolerManager.Instance.effectWaterFirePooler.GetPooledObject();
                g.transform.position = transform.position;
                g.SetActive(true);
            }
        }
    }

    protected virtual void ChangeStoneType(bool isIce = false) { stoneRenderer.sprite = isIce ? iceStoneSprite : stoneSprite; }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (sp == null) sp = GetComponent<SpriteRenderer>();
        if (rid == null) rid = GetComponent<Rigidbody2D>();
    }
#endif


    private int _randomDisplayEffect;


    public float speedMove;
    public bool isGravity;
    public float timeFly = 0;

    private float _ranGas;

    // Start is called before the first frame update
    public void BeginCreateGas()
    {
        _ranGas = Random.Range(0.5f, 1);
        if (isGravity) rid.gravityScale = 0;
        else rid.gravityScale = 0.1f;
        transform.localScale = new Vector3(_ranGas, _ranGas, _ranGas);
        speedMove = Random.Range(1f, 1.5f);
    }

    public virtual void OnUpdate(float deltaTime) { }
}