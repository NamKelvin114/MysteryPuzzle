using System;
using DG.Tweening;
using RoboRyanTron.SearchableEnum;
using UniRx;
using UnityEngine;
using Worldreaver.Root.Attribute;
using Spine.Unity;
#if UNITY_EDITOR
using UnityEditor;

// ReSharper disable InconsistentNaming

#endif

public class RoomItem : MonoBehaviour, IPickUpItem,IPushAway
{
    [SerializeField] private GameObject chest;

    [ReadOnly, SerializeField] private int roomId;
    [ReadOnly, SerializeField] private int id;
    [SpineAnimation, SerializeField] private string dieAnimation;
    [SpineAnimation, SerializeField] private string idleAnimation;
    [SerializeField] private bool falling;
    [SerializeField] private GameObject flashEffect;
    [SerializeField] AnimationCurve animationCurve;
    private float _startValueCurve = 0;
    [SerializeField] private float durationCurve;
    [SerializeField] private GameObject smokeEffect;
    [SerializeField] private Rigidbody2D rigid;
    private bool _isCollected = true;

    private bool _detected;
    [SerializeField] private SkeletonAnimation skeleton;

    internal SkeletonAnimation Skeleton => skeleton;
#if UNITY_EDITOR
    [SerializeField, SearchableEnum] private ERoomItem roomItem;

    internal ERoomItem RoomItem1 => roomItem;

#endif

    public int RoomId
    {
        get => roomId;
        internal set => roomId = value;
    }

    public int ID
    {
        get => id;
        internal set => id = value;
    }

    private void Awake()
    {
        Skeleton.initialSkinName = "default";
    }

    private void Start()
    {
        if (MapLevelManager.Instance.eQuestType == EQuestType.CollectRoomItem)
        {
            if (!DataController.instance.SaveItems[ID + RoomId * Constants.NUMBER_ITEM_IN_ROOM].unlock &&
                (ID + RoomId * Constants.NUMBER_ITEM_IN_ROOM) <= Config.MaxIdRoomItem)
            {
                GameManager.instance.LevelContainRoomItem = true;
                Utils.roomId = RoomId;
                Utils.roomItemId = ID;
                if (!falling)
                {
                    MapLevelManager.Instance.trTarget = transform;
                }

                // if(Data.TimeToRescueParty.Milliseconds>0)
                // {
                //     var letterVlt=Instantiate(letter, transform.parent, false);
                //     letterVlt.transform.position = transform.position+new Vector3(0.1f,0.01f,0);

                // }
            }
            else
            {
                MapLevelManager.Instance.eQuestType = EQuestType.OpenChest;
                var go = Instantiate(chest, transform.parent, false);
                go.transform.position = transform.position;
                MapLevelManager.Instance.trTarget = go.transform;
                gameObject.SetActive(false);
                GameManager.instance.OnInitQuestText();

                // if(Data.TimeToRescueParty.Milliseconds>0)
                // {
                //     var letterVlt=Instantiate(letter, transform.parent, false);
                //     letterVlt.transform.position = transform.position+new Vector3(0.1f,0.01f,0);
                //     gameObject.SetActive(false);
                // }
            }
        }
    }
    public void GetPushed(SpringItem spring)
    {
        //skeleton.skeleton.ScaleX *= -1;
        Vector2 speed = new Vector2();
        if (transform.position.x < spring.transform.position.x)
        {
            speed = Vector2.left;
        }
        else
        {
            speed = Vector2.right;
        }

        DOTween.To(() => _startValueCurve, x => _startValueCurve = x,
                animationCurve.keys[animationCurve.length - 1].time, durationCurve).OnStart((() =>
            {
                //target = null;
            }))
            .OnUpdate((() =>
            {
                if (GameManager.instance.gameState != EGameState.Win &&
                    GameManager.instance.gameState != EGameState.Lose)
                {
                    rigid.velocity = new Vector2(speed.x * animationCurve.Evaluate(_startValueCurve), rigid.velocity.y);
                }
            })).OnComplete((() =>
            {
                _startValueCurve = 0;
            }));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Utils.TAG_LAVA))
        {
            if (!_detected)
            {
                if (PlayerManager.instance.state == EUnitState.Playing)
                {
                    if (GameManager.instance.gameState != EGameState.Win)
                    {
                        //gameObject.SetActive(false);
                        skeleton.AnimationState.SetAnimation(0, dieAnimation, false);
                        smokeEffect.SetActive(true);
                        PlayerManager.instance.OnPlayerDie(EDieReason.Despair);

                        if (ObjectPoolerManager.Instance != null)
                        {
                            GameObject destroyEffect =
                                ObjectPoolerManager.Instance.effectDestroyPooler.GetPooledObject();
                            destroyEffect.transform.position = other.transform.position;
                            destroyEffect.SetActive(true);
                        }
                    }
                }

                _detected = true;
            }
        }

        if (other.CompareTag("BodyPlayer"))
        {
            if (GameManager.instance.gameState != EGameState.Lose && PlayerManager.instance.state != EUnitState.Die)
            {
                // Debug.Log("athere");
                if (skeleton.AnimationState.Equals(dieAnimation)) return;
                GameManager.instance.FlagGetRoomItem = true;
                //GameManager.instance.gameState = EGameState.Win;
                if (_isCollected)
                {
                    //Utils.SetTaskProcess(ETaskType.CollectBox, Utils.GetTaskProcess(ETaskType.CollectBox) + 1);
                    _isCollected = false;
                }

                //rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
                PlayerManager.instance.OnWin(true, true);
                if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acOpenChest);
                Observable.Timer(TimeSpan.FromSeconds(0.3f))
                    .Subscribe(_ =>
                    {
                        if (PlayerManager.instance.state != EUnitState.Die &&
                            !skeleton.AnimationState.Equals(dieAnimation))
                        {
                            flashEffect.SetActive(true);
                            skeleton.AnimationState.SetAnimation(0, idleAnimation, false);
                        }
                    });
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        IDieByFallingStone obj = other.gameObject.GetComponentInParent<IDieByFallingStone>();

        if (obj != null)
        {
            var dragon = (BaseCannon)obj;
            if (!dragon.IsDisable)
            {
                dragon.PlayDeathAnimation();
                dragon.DoBreak();
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RoomItem))]
public class RoomItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var item = (RoomItem)target;

        int value = (int)item.RoomItem1;
        var roomId = (int)(value / Constants.NUMBER_ITEM_IN_ROOM);
        var id = value % Constants.NUMBER_ITEM_IN_ROOM;
        var skinName = ((ERoomItem)value).NameSkin();

        if (item.RoomId != roomId) item.RoomId = roomId;

        if (item.ID != id) item.ID = id;

        if (item != null)
        {
            //item.Skeleton.GetComponent<SkeletonRenderer>().initialSkinName = skinName;
            if (!item.Skeleton.enabled)
            {
                item.Skeleton.enabled = true;
            }
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Unpack"))
        {
            PrefabUtility.UnpackPrefabInstance(item.gameObject, PrefabUnpackMode.Completely,
                InteractionMode.UserAction);
        }

        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        EditorUtility.SetDirty(UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(item.gameObject)
            .prefabContentsRoot);
    }
}
#endif


public enum ERoomItem
{
    I1_1 = 0,
    I1_2 = 1,
    I1_3 = 2,
    I1_4 = 3,
    I1_5 = 4,
    I1_6 = 5,
    I1_7 = 6,
    I1_8 = 7,

    I2_1 = 8,
    I2_2 = 9,
    I2_3 = 10,
    I2_4 = 11,
    I2_5 = 12,
    I2_6 = 13,
    I2_7 = 14,
    I2_8 = 15,

    I3_1 = 16,
    I3_2 = 17,
    I3_3 = 18,
    I3_4 = 19,
    I3_5 = 20,
    I3_6 = 21,
    I3_7 = 22,
    I3_8 = 23,

    I4_1 = 24,
    I4_2 = 25,
    I4_3 = 26,
    I4_4 = 27,
    I4_5 = 28,
    I4_6 = 29,
    I4_7 = 30,
    I4_8 = 31,

    I5_1 = 32,
    I5_2 = 33,
    I5_3 = 34,
    I5_4 = 35,
    I5_5 = 36,
    I5_6 = 37,
    I5_7 = 38,
    I5_8 = 39,

    I6_1 = 40,
    I6_2 = 41,
    I6_3 = 42,
    I6_4 = 43,
    I6_5 = 44,
    I6_6 = 45,
    I6_7 = 46,
    I6_8 = 47,

    I7_1 = 48,
    I7_2 = 49,
    I7_3 = 50,
    I7_4 = 51,
    I7_5 = 52,
    I7_6 = 53,
    I7_7 = 54,
    I7_8 = 55,

    I8_1 = 56,
    I8_2 = 57,
    I8_3 = 58,
    I8_4 = 59,
    I8_5 = 60,
    I8_6 = 61,
    I8_7 = 62,
    I8_8 = 63,

    I9_1 = 64,
    I9_2 = 65,
    I9_3 = 66,
    I9_4 = 67,
    I9_5 = 68,
    I9_6 = 69,
    I9_7 = 70,
    I9_8 = 71,

    I10_1 = 72,
    I10_2 = 73,
    I10_3 = 74,
    I10_4 = 75,
    I10_5 = 76,
    I10_6 = 77,
    I10_7 = 78,
    I10_8 = 79,

    I11_1 = 80,
    I11_2 = 81,
    I11_3 = 82,
    I11_4 = 83,
    I11_5 = 84,
    I11_6 = 85,
    I11_7 = 86,
    I11_8 = 87,

    I12_1 = 88,
    I12_2 = 89,
    I12_3 = 90,
    I12_4 = 91,
    I12_5 = 92,
    I12_6 = 93,
    I12_7 = 94,
    I12_8 = 95,
}