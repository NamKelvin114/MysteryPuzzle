using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
#if UNITY_ANDROID
using Google.Play.Review;
#endif
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;
using Pancake;
public class PopupRate : UniPopupBase
{
    [SerializeField] private UniButton btnBack;
    [SerializeField] private UniButton btnOk;
    [SerializeField] private Toggle toggleDontShow;

    private Action _actionBack;
    private Action _actionOk;

#if UNITY_ANDROID
    private ReviewManager _reviewManager = null;
    private PlayReviewInfo _playReviewInfo = null;
#endif

    private bool _flag;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionOk"></param>
    public void Initialized(Action actionBack, Action actionOk)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        GameManager.instance.Source = new CancellationTokenSource();
        Observable.FromCoroutine(IeCallInitReview).Subscribe().AddTo(GameManager.instance.gameObject);
#endif

        _actionBack = actionBack;
        _actionOk = actionOk;

        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(OnBackButtonPressed);

        btnOk.onClick.RemoveAllListeners();
        btnOk.onClick.AddListener(OnOkButtonPressed);

        Data.LastLevelShowRate = Utils.CurrentLevel + 1;
        Data.CountShowRate += 1;
        _flag = false;
    }

    /// <summary>
    /// back button pressed
    /// </summary>
    private void OnBackButtonPressed()
    {
        if (toggleDontShow.isOn)
        {
            Data.UserRated = true;
        }

        _actionBack?.Invoke();
    }

    /// <summary>
    /// ok button pressed
    /// </summary>
    private async void OnOkButtonPressed()
    {
        if (_flag)
        {
            return;
        }

        if (toggleDontShow.isOn)
        {
            Data.UserRated = true;
        }

        _flag = true;
        _actionOk?.Invoke();
// #if UNITY_ANDROID && !UNITY_EDITOR
//         try
//         {
//             await UniTask.WaitUntil(() => _playReviewInfo != null, cancellationToken: GameManager.instance.Source.Token);
//             Observable.FromCoroutine(IeCallReview).Subscribe().AddTo(GameManager.instance.gameObject);
//         }
//         catch (Exception)
//         {
//             GameManager.instance.Source?.Cancel();
//             GameManager.instance.Source?.Dispose();
//             RateUs();
//         }
// #elif UNITY_IOS && !UNITY_EDITOR
//         RateUs();
// #endif
    }

    /// <summary>
    /// 
    /// </summary>
    private void RateUs()
    {
        var url = "";

        if (Application.platform == RuntimePlatform.Android)
        {
            url = "market://details?id=" + Application.identifier;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            //url = "itms-apps://itunes.apple.com/app/id" + Application.identifier;
            url = "https://apps.apple.com/us/app/id1556567754";
        }

#if UNITY_EDITOR
        if (string.IsNullOrEmpty(url))
        {
            url = "market://details?id=" + Application.identifier;
        }
#endif

        Application.OpenURL(url);
        Data.UserRated = true;
    }

#if UNITY_ANDROID
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator IeCallInitReview()
    {
        if (_reviewManager == null)
        {
            _reviewManager = new ReviewManager();
        }

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }

        _playReviewInfo = requestFlowOperation.GetResult();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator IeCallReview()
    {
        if (_playReviewInfo == null)
        {
            yield return StartCoroutine(IeCallInitReview());
        }

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }

        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
    }
#endif
}