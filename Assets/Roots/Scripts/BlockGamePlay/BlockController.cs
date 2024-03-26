using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    protected Camera gameCamera;
    [SerializeField] private BellowTray bellowTray;
    [SerializeField] private float offsetY;
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private float localScaleBlockPieceOnTray = 0.5f;
    [SerializeField] private PolygonCollider2D containerBox;
    [SerializeField] private SpriteRenderer containerImage;
    [SerializeField] private GameObject moveBackground;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private GameObject gameplayObject;

    public List<BlockPiece> ListPiece;
    private List<List<TrayCellState>> trayCellStates = new List<List<TrayCellState>>();
    private List<List<BlockPiece>> takePlaceMap = new List<List<BlockPiece>>();
    private BlockPiece currentPiece = null;

    public void Init(Camera _camera)
    {
        Observer.PlayAnimIdle?.Invoke();
        gameCamera = _camera;
        SetupCamForGameplay();
        bellowTray.Init(gameCamera);
        SetupBg();
        Input.multiTouchEnabled = false;
        for (int i = 0; i < mapSize.x; i++)
        {
            trayCellStates.Add(new List<TrayCellState>());
            takePlaceMap.Add(new List<BlockPiece>());
            for (int j = 0; j < mapSize.y; j++)
            {
                trayCellStates[i].Add(TrayCellState.None);
                takePlaceMap[i].Add(null);
            }
        }

        foreach (var piece in ListPiece)
        {
            piece.CalcStartPosition();
            foreach (var cell in piece.ListCell)
            {
                var position = cell.transform.position;
                int x = (int)position.x;
                int y = (int)position.y;
                trayCellStates[x][y] = TrayCellState.Empty;
            }

            piece.PieceTrigger.enabled = false;
        }

        containerBox.enabled = false;
        ArrangePieces();
    }

    private void SetupBg()
    {
        float scale = background.transform.localScale.x;
        float newScale = gameCamera.orthographicSize / 5 * scale;
        moveBackground.transform.localScale = Vector3.one * newScale;
        Vector3 position = background.transform.position;
        moveBackground.transform.position = new Vector3(position.x + containerImage.transform.position.x,
            position.y + containerImage.transform.position.y, 0);
    }

    private void SetupCamForGameplay()
    {
        var bounds = containerImage.bounds;
        float objectHeight = bounds.size.y;
        float objectWidth = bounds.size.x;
        float screenHeightInWorldSpace = gameCamera.orthographicSize * 2.0f;
        float screenWidthInWorldSpace = screenHeightInWorldSpace * gameCamera.aspect;
        float objectHeightInScreenSpace = objectHeight / screenHeightInWorldSpace * Screen.safeArea.height;
        float objectWidthInScreenSpace = objectWidth / screenWidthInWorldSpace * Screen.safeArea.width;
        float targetHeightInScreenSpace = Screen.safeArea.height * 0.4f;
        float targetWidthInScreenSpace = Screen.safeArea.width * 0.75f;
        float newOrthographicSize = gameCamera.orthographicSize / targetHeightInScreenSpace * objectHeightInScreenSpace;
        float newOrthographicSize1 = gameCamera.orthographicSize / targetWidthInScreenSpace * objectWidthInScreenSpace;
        float newOrtho = Mathf.Max(newOrthographicSize, newOrthographicSize1);
        gameCamera.orthographicSize = newOrtho;
        float worldDistance = 0.1f * Screen.safeArea.height * 2 * gameCamera.orthographicSize / Screen.safeArea.height;
        gameCamera.transform.position = new Vector3(containerImage.bounds.center.x, bounds.center.y - worldDistance,
            gameCamera.transform.position.z);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleFingerDown();
        }

        if (Input.GetMouseButtonUp(0))
        {
            HandleFingerUp();
        }

        if (_isFingerDown)
        {
            HandleFingerUpdate();
        }
    }

    //----------------------------------------------------------------
    // Main Game Cycle
    private bool _isFingerDown = false;
    private bool isDuringPieceDrag = false;
    private bool isDuringTraySlide = false;
    private Vector2 _startTraySlidePos = Vector2.zero;
    private Vector2 _currentTraySlidePos = Vector2.zero;
    private BlockPiece _firstPieceCurrent;
    private BlockPiece _lastPieceCurrent;
    private int _remainPieceNumber;
    private Vector3 _oldSlidePos;
    private float _swipeXDir;
    private bool _isDragPieceFromTray;

    private void HandleFingerDown()
    {
        if (_isFingerDown)
            return;
        Vector3 cameraPos = GetPointerPosition(gameCamera);

        _isFingerDown = true;

        RaycastHit2D hit = Physics2D.Raycast(cameraPos, Vector2.zero);
        if (hit.collider != null) // hit block
        {
            Transform objectHit = hit.transform;
            var piece = objectHit.GetComponentInParent<BlockPiece>();
            if (piece)
            {
                isDuringPieceDrag = true;

                currentPiece = piece;
                _isDragPieceFromTray = currentPiece.PieceState == PieceState.NotPlaced;
                currentPiece.BeginDrag();
                // remove takeplace at map
                RemovePieceInTakePlaceMap(currentPiece);

                // make that piece
                currentPiece.transform.localScale = Vector3.one;
            }
        }
        else if (bellowTray.Tray.bounds.Contains(cameraPos))
        {
            _startTraySlidePos = cameraPos;

            var remainPieceArray = ListPiece.Where(piece => piece.PieceState == PieceState.NotPlaced).ToArray();

            _firstPieceCurrent = remainPieceArray[0];
            _lastPieceCurrent = remainPieceArray[^1];
            _remainPieceNumber = remainPieceArray.Length;

            _oldSlidePos = _startTraySlidePos;

            if (!CheckCanNotMoveTray())
                isDuringTraySlide = true;
        }
    }

    void HandleFingerUp()
    {
        _isFingerDown = false;
        if (isDuringPieceDrag)
        {
            ReleasePiece();
            isDuringPieceDrag = false;
        }

        if (isDuringTraySlide)
        {
            float deltaX = 0;
            Vector3 localScale = bellowTray.transform.localScale;
            if (_firstPieceCurrent.Center.x > bellowTray.StartPos)
            {
                float prePos = bellowTray.Tray.bounds.min.x -
                               0.25f * bellowTray.PointDistance * bellowTray.transform.localScale.x;
                float leftRange = _firstPieceCurrent.LeftRange;
                float thisPos = prePos + bellowTray.PointDistance * localScale.x + leftRange;
                deltaX = thisPos - _firstPieceCurrent.transform.position.x;
            }
            else if (_lastPieceCurrent.Center.x < bellowTray.LastPos)
            {
                float prePos = bellowTray.Tray.bounds.max.x +
                               0.25f * bellowTray.PointDistance * bellowTray.transform.localScale.x;
                float rightRange = _lastPieceCurrent.RightRange;
                float thisPos = prePos - bellowTray.PointDistance * localScale.x - rightRange;
                deltaX = thisPos - _lastPieceCurrent.transform.position.x;
            }


            if (deltaX != 0)
            {
                foreach (var piece in ListPiece)
                {
                    if (piece.PieceState == PieceState.NotPlaced)
                    {
                        piece.transform.DOMoveX(piece.transform.position.x + deltaX, 0.2f).SetEase(Ease.OutBack);
                    }
                }
            }
        }

        isDuringTraySlide = false;
    }

    void HandleFingerUpdate()
    {
        if (!_isFingerDown) return;

        Vector3 cameraPos = GetPointerPosition(gameCamera);

        if (isDuringPieceDrag)
        {
            currentPiece.transform.position =
                new Vector3(cameraPos.x, cameraPos.y + currentPiece.OffsetDragY + offsetY);
            int x = Mathf.RoundToInt(currentPiece.transform.position.x);
            int y = Mathf.RoundToInt(currentPiece.transform.position.y);

            bool canPlaceCurrenPiece = CanPlaceCurrentPiece();
            Vector3 shadowPos = new Vector3(x, y);

            currentPiece.Dragging(canPlaceCurrenPiece, shadowPos);
        }

        if (isDuringTraySlide)
        {
            _currentTraySlidePos = cameraPos;

            _swipeXDir = _currentTraySlidePos.x - _oldSlidePos.x;
            bool isLeft = _swipeXDir < 0;

            _oldSlidePos = _currentTraySlidePos;
            float ratio = 1;
            if (!isLeft && _firstPieceCurrent.transform.position.x > bellowTray.Tray.bounds.min.x)
            {
                ratio = 1 - (_firstPieceCurrent.transform.position.x - bellowTray.Tray.bounds.min.x) /
                    (bellowTray.Tray.bounds.size.x * 0.5f);
            }
            else if (isLeft && _lastPieceCurrent.transform.position.x < bellowTray.Tray.bounds.max.x)
            {
                ratio = 1 - (bellowTray.Tray.bounds.max.x - _lastPieceCurrent.transform.position.x) /
                    (bellowTray.Tray.bounds.size.x * 0.5f);
            }

            foreach (BlockPiece piece in ListPiece)
            {
                if (piece.PieceState == PieceState.NotPlaced)
                {
                    var piecePos = piece.transform.position;
                    piecePos.x += _swipeXDir * ratio;
                    piece.transform.position = piecePos;
                }
            }
        }
    }

    public void ReleasePiece()
    {
        // round the position
        var position = currentPiece.transform.position;
        int posX = Mathf.RoundToInt(position.x);
        int posY = Mathf.RoundToInt(position.y);
        position = new Vector2(posX, posY);
        currentPiece.transform.position = position;

        currentPiece.EndDrag();

        bool canBePlaced = CanPlaceCurrentPiece() && CheckInsideBox();
        // if (autoFalse)
        //     canBePlaced = false;

        if (canBePlaced)
        {
            // Observer.PlaceBlock?.Invoke(); 
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acPlaceBlock);
            if (_isDragPieceFromTray)
            {
                PushPiecesAfterPlaced();
            }

            currentPiece.PlacePiece();
            // remove other piece if they already take place
            foreach (Cell cell in currentPiece.ListCell)
            {
                var position1 = cell.transform.position;
                int x = (int)position1.x;
                int y = (int)position1.y;

                if (takePlaceMap[x][y] != null)
                {
                    RemovePieceInTakePlaceMap(takePlaceMap[x][y]);
                }

                takePlaceMap[x][y] = currentPiece;
            }

            CheckWin();
        }
        else
        {
            // if(autoFalse == false)
            //     Observer.PlaceBlockFalse?.Invoke();
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acPlaceBlockFalse);
            currentPiece.ReturnPiece();
            PushPiecesAfterNotPlaced();
            //ArrangePieces(0.2f);
        }

        currentPiece = null;
    }

    private bool CanPlaceCurrentPiece()
    {
        foreach (Cell cell in currentPiece.ListCell)
        {
            var position = cell.transform.position;
            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.y);

            if ((x >= mapSize.x || x < 0)
                || (y >= mapSize.y || y < 0)
                || trayCellStates[x][y] == TrayCellState.None
                || takePlaceMap[x][y] != null) // NOT REPLACE CONCEPT, REMOVE THIS LINE TO ROLLBACK
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckInsideBox()
    {
        if (currentPiece == null)
            return false;
        currentPiece.PieceTrigger.enabled = true;
        Vector2[] points = currentPiece.PieceTrigger.points;
        float scale = 1.1f;
        containerBox.enabled = true;
        foreach (var point in points)
        {
            if (containerBox.OverlapPoint(point * scale + (Vector2)currentPiece.transform.position) == false)
            {
                containerBox.enabled = false;
                currentPiece.PieceTrigger.enabled = false;
                return false;
            }
        }

        for (int i = 0; i < containerBox.pathCount; i++)
        {
            foreach (var point in containerBox.GetPath(i))
            {
                if (currentPiece.PieceTrigger.OverlapPoint(point * scale + (Vector2)containerBox.transform.position) ==
                    true)
                {
                    containerBox.enabled = false;
                    currentPiece.PieceTrigger.enabled = false;
                    return false;
                }
            }
        }

        currentPiece.PieceTrigger.enabled = false;
        containerBox.enabled = false;
        return true;
    }

    // ----------------------------------------------------------------
    // do something with Piece
    public void RemovePieceInTakePlaceMap(BlockPiece piece)
    {
        BlockPiece tempPiece = piece;

        //tempPiece.PieceState = PieceState.NotPlaced;
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                if (takePlaceMap[i][j] == tempPiece)
                {
                    takePlaceMap[i][j] = null;
                }
            }
        }
    }

    void PushPiecesAfterPlaced()
    {
        if (currentPiece)
        {
            var remainPieceArray = ListPiece.Where(piece => piece.PieceState == PieceState.NotPlaced).ToArray();

            int currentPieceIndex = 0;
            for (int i = 0; i < remainPieceArray.Length; i++)
                if (remainPieceArray[i] == currentPiece)
                {
                    currentPieceIndex = i;
                    break;
                }

            // currentPiece.pieceState = PieceState.Placed;
            currentPiece.PlacePiece();
            if (CheckCanNotMoveTray())
            {
                ArrangePieces(0.2f);
                return;
            }

            if (currentPieceIndex == remainPieceArray.Length - 1)
            {
                float range = bellowTray.PointDistance * bellowTray.transform.localScale.x;
                float yPos = bellowTray.transform.position.y;
                float prePos = bellowTray.LastPos;
                for (int i = currentPieceIndex - 1; i >= 0; i--)
                {
                    var piece = remainPieceArray[i];
                    float leftRange = piece.LeftRange;
                    float rightRange = piece.RightRange;
                    float thisPos = prePos - range - rightRange;
                    prePos = thisPos - leftRange;
                    piece.transform.DOMove(new Vector3(thisPos, yPos, 0), 0.2f);
                }
            }
            else
            {
                float range = bellowTray.PointDistance * bellowTray.transform.localScale.x;
                float yPos = bellowTray.transform.position.y;
                float prePos = (currentPieceIndex == 0)
                    ? bellowTray.Tray.bounds.min.x -
                      0.25f * bellowTray.PointDistance * bellowTray.transform.localScale.x
                    : remainPieceArray[currentPieceIndex - 1].PieceImage.bounds.max.x;
                for (int i = currentPieceIndex + 1; i < remainPieceArray.Length; i++)
                {
                    var piece = remainPieceArray[i];
                    piece.transform.localScale = Vector3.one * localScaleBlockPieceOnTray;
                    float leftRange = piece.LeftRange;
                    float rightRange = piece.RightRange;
                    float thisPos = prePos + range + leftRange;
                    prePos = thisPos + rightRange;
                    piece.transform.DOMove(new Vector3(thisPos, yPos, 0), 0.2f);
                }
            }
        }
    }

    void PushPiecesAfterNotPlaced()
    {
        if (CheckCanNotMoveTray())
        {
            ArrangePieces(0.2f);
            return;
        }

        if (currentPiece)
        {
            var remainPieceArray = ListPiece.Where(piece => piece.PieceState == PieceState.NotPlaced).ToArray();

            int currentPieceIndex = 0;
            for (int i = 0; i < remainPieceArray.Length; i++)
                if (remainPieceArray[i] == currentPiece)
                {
                    currentPieceIndex = i;
                    break;
                }

            currentPiece.transform.localScale = Vector3.one * localScaleBlockPieceOnTray;
            float range = bellowTray.PointDistance * bellowTray.transform.localScale.x;
            float yPos = bellowTray.transform.position.y;
            float prePos = (currentPieceIndex == 0)
                ? bellowTray.Tray.bounds.min.x - 0.25f * bellowTray.PointDistance * bellowTray.transform.localScale.x
                : remainPieceArray[currentPieceIndex - 1].PieceImage.bounds.max.x;
            for (int i = currentPieceIndex; i < remainPieceArray.Length; i++)
            {
                var piece = remainPieceArray[i];
                piece.transform.localScale = Vector3.one * localScaleBlockPieceOnTray;
                float leftRange = piece.LeftRange;
                float rightRange = piece.RightRange;
                float thisPos = prePos + range + leftRange;
                prePos = thisPos + rightRange;
                piece.transform.DOMove(new Vector3(thisPos, yPos, 0), 0.2f);
            }
        }
    }

    bool CheckCanNotMoveTray()
    {
        var remainPieceArray = ListPiece.Where(piece => piece.PieceState == PieceState.NotPlaced).ToArray();
        float prePos = bellowTray.Tray.bounds.min.x -
                       0.25f * bellowTray.PointDistance * bellowTray.transform.localScale.x;
        float range = bellowTray.PointDistance * bellowTray.transform.localScale.x;

        for (int i = 0; i < remainPieceArray.Length; i++)
        {
            var piece = remainPieceArray[i];
            piece.transform.localScale = Vector3.one * localScaleBlockPieceOnTray;
            float leftRange = piece.LeftRange;
            float rightRange = piece.RightRange;
            float thisPos = prePos + range + leftRange;
            prePos = thisPos + rightRange;
        }

        return prePos < bellowTray.Tray.bounds.max.x;
    }

    private void ArrangePieces(float moveTime = 0f)
    {
        var localScale = bellowTray.transform.localScale;
        float range = bellowTray.PointDistance * localScale.x;
        var remainPieceArray = ListPiece.Where(piece => piece.PieceState == PieceState.NotPlaced).ToArray();
        float yPos = bellowTray.transform.position.y;
        float prePos = bellowTray.Tray.bounds.min.x - 0.25f * bellowTray.PointDistance * localScale.x;
        for (int i = 0; i < remainPieceArray.Length; i++)
        {
            var piece = remainPieceArray[i];
            piece.transform.localScale = Vector3.one * localScaleBlockPieceOnTray;
            float leftRange = piece.LeftRange;
            float rightRange = piece.RightRange;
            float thisPos = prePos + range + leftRange;
            prePos = thisPos + rightRange;
            piece.transform.DOMove(new Vector3(thisPos, yPos, 0), moveTime);
        }
    }

    private void CheckWin()
    {
        bool isWin = true;
        foreach (BlockPiece piece in ListPiece)
        {
            if (piece.PieceState != PieceState.Placed || piece.transform.position != piece.StartPosition)
            {
                isWin = false;
                break;
            }
        }

        if (isWin)
        {
            gameplayObject.SetActive(false);
            background.color = new Color(0.5f, 0.5f, 0.5f, 1);
            Observer.PlayAnimWin?.Invoke();
            MapLevelManager.Instance.OnWin();
        }
    }

    private static Vector3 GetPointerPosition(Camera _camera)
    {
        Vector3 pointerPosition;
        // For mobile/desktop
        if (Input.touchCount > 0)
            pointerPosition = _camera.ScreenToWorldPoint(Input.GetTouch(0).position);
        else
            pointerPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        pointerPosition.z = 0;
        return pointerPosition;
    }
#if UNITY_EDITOR
    public void SetupBox(PolygonCollider2D box)
    {
        containerBox = box;
    }

    public void UpdatePiece()
    {
        BlockPiece[] listBlockPieces = GetComponentsInChildren<BlockPiece>(true);
        ListPiece = new List<BlockPiece>();
        foreach (BlockPiece blockPiece in listBlockPieces)
        {
            ListPiece.Add(blockPiece);
        }

        Debug.Log(ListPiece.Count);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}

public enum TrayCellState
{
    None, // don't have draw in it
    Empty, // have draw but not filled
    Filled // done as filled
}

public enum PieceState
{
    NotPlaced,
    Placed,
    Locked,
}