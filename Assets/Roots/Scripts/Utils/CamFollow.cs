using System;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class CamFollow : MonoBehaviour
{
    public GameObject objectToFollow /*,group*/;
    public Camera _myCam;
    public float speed = 2.0f;
    public float camSize = 2.5f;
    public float camSizeGameplay1 = 1.5f;
    private bool _isShow = true;
    public bool beginFollow;

    Vector3 position;
    float interpolation;

    public GameObject PanelWin;
    private bool startFollow = false;
    [SerializeField] private GameObject BtnTask;
    [SerializeField] private SkeletonGraphic handTutorial;
    [SerializeField] private Image blackBG;
    [SerializeField] private float durationMoveCam = 1f;
    [SerializeField] private float minDistanceToFollow = .3f;
    [SerializeField] private float distanceWhenGameplayHasGlass = .18f;
    [SerializeField] private float durationMoveCamToFollow = .2f;
    private Sequence _sequence;
    private bool endZoomCam;

    private void Start()
    {
        _isShow = true;
        blackBG.gameObject.SetActive(false);
        endZoomCam = false;
    }

    private void OnEnable()
    {
        Observer.ShakeCamera += Shake;
    }

    private void OnDisable()
    {
        Observer.ShakeCamera -= Shake;
    }

    void Shake()
    {
        gameObject.GetComponent<CameraShake>().Shake();
    }

    void Update()
    {
        if (beginFollow)
        {
            if (MapLevelManager.Instance.isGameplay1) camSize = camSizeGameplay1;
            interpolation = speed * Time.deltaTime;

            var scale = PanelWin.transform.localScale;
            scale.x = scale.y = Mathf.Lerp(scale.x, 1, interpolation * 3);
            PanelWin.transform.localScale = scale;

            position = this.transform.position;
            if (!(objectToFollow is null))
            {
                try
                {
                    if (MapLevelManager.Instance.isGameplay1)
                        position.y = objectToFollow.transform.position.y + .5f;
                    else
                        position.y = objectToFollow.transform.position.y + .7f;
                    position.x = objectToFollow.transform.position.x;
                }
                catch (Exception e)
                {
                    //
                    position = this.transform.position;
                }
            }

            // if (objectToFollow != null)
            // position = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y + .7f,
            //     this.transform.position.z);
            if (objectToFollow == null) return;
            float newCamSize = _myCam.orthographicSize / 5 * camSize;
            MeshRenderer temp = objectToFollow.gameObject.GetComponentInChildren<MeshRenderer>();
            if (temp == null)
            {
                return;
            }

            var bounds = objectToFollow.gameObject.GetComponentInChildren<MeshRenderer>().bounds;
            if (!MapLevelManager.Instance.isGameplay1)
                bounds = objectToFollow.gameObject.GetComponentsInChildren<MeshRenderer>()[1].bounds;
            float objectHeight = bounds.size.y;
            float screenHeightInWorldSpace = _myCam.orthographicSize * 2.0f;
            float objectHeightInScreenSpace = objectHeight / screenHeightInWorldSpace * Screen.safeArea.height;
            float targetHeightInScreenSpace = Screen.safeArea.height * 0.3f;
            float newOrthographicSize = _myCam.orthographicSize / targetHeightInScreenSpace * objectHeightInScreenSpace;
            newCamSize = newOrthographicSize;
            if (MapLevelManager.Instance.GetComponent<LevelMap>().isBlockGameplay) camSize = newOrthographicSize;
            position = new Vector3(
                bounds.center.x + (MapLevelManager.Instance.isNotPullAllPins && MapLevelManager.Instance.gameStateWin
                    ? distanceWhenGameplayHasGlass * (objectToFollow.gameObject.transform.localScale.x > 0 ? 1f : -1f)
                    : 0f),
                bounds.center.y, _myCam.transform.position.z);
            if (!MapLevelManager.Instance.gameStateWin && MapLevelManager.Instance.isGameplay1) newCamSize = camSize;
            if (!startFollow)
            {
                startFollow = true;
                //this.transform.position = position;
                //position.z += 10f;
                if (_sequence != null) DOTween.Kill(_sequence);
                _sequence = DOTween.Sequence();
                _sequence.Append(this.transform.DOMove(position, durationMoveCam))
                    .Join(DOTween.To(() => _myCam.orthographicSize, x => _myCam.orthographicSize = x, newCamSize,
                        durationMoveCam)).OnComplete(() => endZoomCam = true);
                //.OnComplete(() => GamePopup.Instance.ShowPopupMoney());
            }
            else
            {
                bounds = objectToFollow.gameObject.GetComponentInChildren<MeshRenderer>().bounds;
                if (!MapLevelManager.Instance.isGameplay1)
                    bounds = objectToFollow.gameObject.GetComponentsInChildren<MeshRenderer>()[1].bounds;
                if (endZoomCam && MapLevelManager.Instance.isGameplay1)
                    if (Mathf.Abs(bounds.center.x - transform.position.x) > minDistanceToFollow ||
                        Mathf.Abs(bounds.center.y - transform.position.y) > minDistanceToFollow)
                    {
                        var newPos = new Vector3(bounds.center.x, bounds.center.y, _myCam.transform.position.z);
                        DOTween.Kill(this.transform);
                        transform.DOMove(newPos, durationMoveCamToFollow);
                    }
            }

            // Debug.LogError(Vector2.Distance(transform.position, objectToFollow.transform.position));
            if (GameManager.instance.bouderCoinFly.activeSelf)
                return;
            if (PanelWin.transform.localScale.x <= 1.1f)
            {
                //GameManager.instance.bouderCoinFly.SetActive(true);
                // if (_isShow)
                // {
                //     _isShow = false;
                //     if (MapLevelManager.Instance.isShowTask)
                //     {
                //         blackBG.gameObject.SetActive(true);
                //         handTutorial.gameObject.SetActive(true);
                //     }
                //     BtnTask.gameObject.SetActive(true);
                // }
            }
        }
    }

    public void ShowTask()
    {
        GamePopup.Instance.ShowTaskdPopup(null, false);
        if (MapLevelManager.Instance.isShowTask)
        {
            blackBG.gameObject.SetActive(false);
            handTutorial.gameObject.SetActive(false);
            if (MapLevelManager.Instance.isEndChapter)
            {
                Observer.CloseTask?.Invoke(false);
            }
        }
    }
}