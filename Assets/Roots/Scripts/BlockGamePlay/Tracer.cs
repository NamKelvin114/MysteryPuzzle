using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracer : MonoBehaviour
{
    [SerializeField] private Transform slotPrefab;
    [SerializeField] private Transform contentTransform;

    [SerializeField] private List<Transform> slotList = new List<Transform>();

    private float timeBeforeShuffle = 1.0f;

    private void OnEnable()
    {
        Observer.MoveToTracer += GameEvents_MoveToTracer;
        // Observer.ShowFinalImage += Observer_ShowFinalImage;
    }

    private void OnDisable()
    {
        Observer.MoveToTracer -= GameEvents_MoveToTracer;
        // GameEvents.MoveToTracer -= GameEvents_MoveToTracer;
        // Observer.ShowFinalImage -= Observer_ShowFinalImage;
    }

    private void GameEvents_MoveToTracer(List<JigsawPiece> pieceList)
    {
        StartCoroutine(StartMove(pieceList));
    }

    private void Observer_ShowFinalImage()
    {
        gameObject.SetActive(false);
    }
    public void DisableTutorial()
    {
        GamePopup.Instance.HidePopupTutorialJigSaw();
    }

    IEnumerator StartMove(List<JigsawPiece> pieceList)
    {
        bool _isFirstSet = true;
        SetUpSlots(pieceList.Count);

        yield return new WaitForSeconds(timeBeforeShuffle);

        for (int i = 0; i < pieceList.Count; i++)
        {
            int randomSlotIndex = Random.Range(0, slotList.Count);
            pieceList[i].SetTracerSlot(slotList[randomSlotIndex]);
            if (randomSlotIndex==0)
            {
                if (_isFirstSet)
                {
                    Observer.correctPeacePosi?.Invoke(pieceList[i].completedPosition);
                    _isFirstSet = false;
                }
            }
            slotList.RemoveAt(randomSlotIndex);
        }
    }

    public void SetUpSlots(int numberOfSlots)
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            slotList.Add(Instantiate(slotPrefab, contentTransform));
        }
    }
}
