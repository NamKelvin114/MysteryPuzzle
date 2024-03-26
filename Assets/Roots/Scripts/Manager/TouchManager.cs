using UnityEngine;
using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour
{
    #region Trail Eff
    public float distanceFromCamera = 5;
    public LayerMask lmTouch;
    public Camera thisCamera;
    [SerializeField] private GameObject slicerPrefab;
    #endregion

    private GameObject _trail;
    private Vector3 _mousePos;

    private void Start()
    {
        if (_trail == null)
        {
            _trail = Instantiate(slicerPrefab);
        }

        _trail.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.instance.gameState != EGameState.Playing || !GameManager.instance.isTest
            && (GamePopup.Instance.popupShopHandler?.ThisGameObject.activeSelf == true
                || GameManager.instance.gPanelWin.gameObject.activeSelf
                || GamePopup.Instance.popupSkinHandler?.ThisGameObject.activeSelf == true
                || GamePopup.Instance.popupDailyRewardHandler?.ThisGameObject.activeSelf == true
                || GamePopup.Instance.currentRoom?.gameObject.activeSelf == true)) return;
        if (Input.GetMouseButtonDown(0) && GameManager.instance.isShowEvent == false)
        {
            _cutOn = true;
            _currentMouse = _oldMouse = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Ray2D ray = new Ray2D(_oldMouse, _currentMouse - _oldMouse);
            RaycastHit2D hit =
                Physics2D.Raycast(ray.origin, ray.direction, (_currentMouse - _oldMouse).magnitude, lmTouch);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == Utils.TAG_STICKBARRIE)
                {
                    StickBarrier sb = hit.collider.gameObject.GetComponent<StickBarrier>();
                    if (!sb.beginMove)
                    {
                        if (SoundManager.Instance != null)
                        {
                            if (sb._moveType == StickBarrier.MOVETYPE.FREE)
                            {
                                SoundManager.Instance.PlaySound(SoundManager.Instance.acMoveStickClick);
                                if (GameManager.instance.mapLevel.lstAllStick.Contains(sb) &&
                                    MapLevelManager.Instance.isShowTutorial == false && sb.isTutorial == false)
                                {
                                    GameManager.instance.mapLevel.lstAllStick.Remove(sb);
                                }
                            }
                            else
                                SoundManager.Instance.PlaySound(SoundManager.Instance.acMoveStick);
                        }

                        if (MapLevelManager.Instance.isShowTutorial)
                        {
                            if (sb.isTutorial && !EventSystem.current.IsPointerOverGameObject())
                            {
                                sb.beginMove = true;
                            }
                        }
                        else
                        {
                            if (!EventSystem.current.IsPointerOverGameObject())
                            {
                                sb.beginMove = true;
                            }
                        }

                        //sb.beginMove = true;
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (GameManager.instance.canUseTrail)
            {
                _trail.gameObject.SetActive(false);
            }

            _cutOn = false;
        }

        if (_cutOn)
        {
            _currentMouse = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray2D ray = new Ray2D(_oldMouse, _currentMouse - _oldMouse);
            RaycastHit2D hit =
                Physics2D.Raycast(ray.origin, ray.direction, (_currentMouse - _oldMouse).magnitude, lmTouch);
            _oldMouse = _currentMouse;
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Rope"))
                {
                    var ropeNode = hit.collider.gameObject.GetComponent<RopeNode>();
                    if (ropeNode)
                    {
                        ropeNode.CutAt(hit.point);
                        if (SoundManager.Instance != null)
                        {
                            SoundManager.Instance.PlaySound(SoundManager.Instance.acCutRope);
                        }
                    }
                }
                else if (hit.collider.gameObject.tag == Utils.TAG_STICKBARRIE)
                {
                    StickBarrier sb = hit.collider.gameObject.GetComponent<StickBarrier>();
                    if (!sb.beginMove && MapLevelManager.Instance.isShowTutorial == false)
                    {
                        if (SoundManager.Instance != null)
                        {
                            if (MapLevelManager.Instance.isShowTutorial == false && sb.isTutorial == false)
                            {
                                if (sb._moveType == StickBarrier.MOVETYPE.FREE)
                                {
                                    SoundManager.Instance.PlaySound(SoundManager.Instance.acMoveStickClick);
                                }
                                else
                                {
                                    SoundManager.Instance.PlaySound(SoundManager.Instance.acMoveStick);
                                }
                                if (GameManager.instance.mapLevel.lstAllStick.Contains(sb))
                                {
                                    GameManager.instance.mapLevel.lstAllStick.Remove(sb);
                                }
                                sb.beginMove = true;
                            }
                            // if (sb._moveType == StickBarrier.MOVETYPE.FREE)
                            // {
                            //     SoundManager.Instance.PlaySound(SoundManager.Instance.acMoveStickClick);
                            // }
                            // else if (GameManager.instance.mapLevel.lstAllStick.Contains(sb) &&
                            //          MapLevelManager.Instance.isShowTutorial == false && sb.isTutorial == false)
                            // {
                            //     GameManager.instance.mapLevel.lstAllStick.Remove(sb);
                            //     sb.beginMove = true;
                            // }
                            //
                            // SoundManager.Instance.PlaySound(SoundManager.Instance.acMoveStick);
                        }
                    }

                    if (MapLevelManager.Instance.isShowTutorial)
                    {
                        if (sb.isTutorial)
                        {
                            sb.beginMove = true;
                        }
                    }
                    // else
                    // {
                    //     if (!EventSystem.current.IsPointerOverGameObject())
                    //     {
                    //         sb.beginMove = true;
                    //     }
                    // }
                    //
                    // sb.beginMove = true;
                }
            }

            if (GameManager.instance.canUseTrail)
            {
                _trail.gameObject.SetActive(true);
                MoveTrailToCursor(Input.mousePosition);
            }
        }
    }

    void MoveTrailToCursor(
        Vector3 screenPosition)
    {
        _trail.transform.position =
            thisCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, distanceFromCamera));
    }


    private bool _cutOn;
    private Vector3 _oldMouse;
    private Vector3 _currentMouse;
}
