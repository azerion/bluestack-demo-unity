using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum State
{
    Closed,
    Open
}

public abstract class PanelSlider : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler,
    IInitializePotentialDragHandler
{
    #region Fields

    // Basic Settings
    [SerializeField] private State _defaultState = State.Closed;
    [SerializeField] private float _transitionSpeed = 5f;
    [SerializeField] private GameObject _content = null;

    // Drag Settings
    [SerializeField] private float _thresholdDragSpeed = 0f;
    [SerializeField] private float _thresholdDraggedFraction = 0.5f;

    // Overlay Settings
    [SerializeField] private bool _useOverlay = true;
    [SerializeField] private Color _overlayColour = new Color(0, 0, 0, 0.25f);
    [SerializeField] private bool _useBlur = false;
    [SerializeField] private Material _blurMaterial;
    [SerializeField] private int _blurRadius = 10;

    // Events
    private UnityEvent<State> OnStateSelecting = new UnityEvent<State>();
    private UnityEvent<State> OnStateSelected = new UnityEvent<State>();
    private UnityEvent<State, State> OnStateChanging = new UnityEvent<State, State>();
    private UnityEvent<State, State> OnStateChanged = new UnityEvent<State, State>();

    private float _previousTime;
    private bool _isDragging;
    private bool _isPotentialDrag;
    private Vector2 _closedPosition, _openPosition;
    private Vector2 _startPosition, _releaseVelocity, _dragVelocity, _menuSize;
    private Vector3 _previousPosition;
    private GameObject _overlay, _blur;
    private RectTransform _rectTransform, _canvasRectTransform;
    private Image _overlayImage, _blurImage;
    private CanvasScaler _canvasScaler;
    private Canvas _canvas;

    #endregion

    #region Properties

    public GameObject Content
    {
        get => _content;
        protected set => _content = value;
    }
    public Vector2 ClosedPosition
    {
        get => _closedPosition;
        protected set => _closedPosition = value;
    }
    public Vector2 OpenPosition 
    {
        get => _openPosition;
        protected set => _openPosition = value;
    }
    public State CurrentState { get; private set; }
    public State TargetState { get; private set; }

    public float StateProgress
    {
        get
        {
            bool isLeftOrRight = true;
            return ((_rectTransform.anchoredPosition - _closedPosition).magnitude /
                    (isLeftOrRight ? _rectTransform.rect.width : _rectTransform.rect.height));
        }
    }

    private bool IsValidConfig
    {
        get
        {
            bool valid = true;

            if (_transitionSpeed <= 0)
            {
                Debug.LogError("Transition speed cannot be less than or equal to zero.", gameObject);
                valid = false;
            }

            return valid;
        }
    }

    #endregion

    #region Methods
    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        if (IsValidConfig)
        {
            Setup();
        }
        else
        {
            throw new Exception("Invalid configuration.");
        }
    }

    protected virtual void Update()
    {
        HandleState();
        HandleOverlay();
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        _isPotentialDrag = (eventData.pointerEnter == gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = _isPotentialDrag;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, eventData.position,
                _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
                out Vector2 mouseLocalPosition))
        {
            _startPosition = mouseLocalPosition;
        }

        _previousPosition = _rectTransform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDragging && RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform,
                eventData.position, _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
                out Vector2 mouseLocalPosition))
        {
            Vector2 displacement = ((TargetState == State.Closed) ? _closedPosition : _openPosition) +
                                   (mouseLocalPosition - _startPosition);
            float x = displacement.x;
            float y = _rectTransform.anchoredPosition.y;
            Vector2 min = new Vector2(Math.Min(_closedPosition.x, _openPosition.x),
                Math.Min(_closedPosition.y, _openPosition.y));
            Vector2 max = new Vector2(Math.Max(_closedPosition.x, _openPosition.x),
                Math.Max(_closedPosition.y, _openPosition.y));
            _rectTransform.anchoredPosition = new Vector2(Mathf.Clamp(x, min.x, max.x), Mathf.Clamp(y, min.y, max.y));

            OnStateSelecting.Invoke(CurrentState);
        }

        _dragVelocity = (_rectTransform.position - _previousPosition) / (Time.time - _previousTime);
        _previousPosition = _rectTransform.position;
        _previousTime = Time.time;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
        _releaseVelocity = _dragVelocity;

        if (_releaseVelocity.magnitude > _thresholdDragSpeed)
        {
            if (_releaseVelocity.x > 0)
            {
                Open();
            }
            else
            {
                Close();
            }
        }
        else
        {
            float nextStateProgress = (TargetState == State.Open) ? 1 - StateProgress : StateProgress;
            if (nextStateProgress > _thresholdDraggedFraction)
            {
                ToggleState();
            }
            else
            {
                SetState(CurrentState);
            }
        }
    }

    private void Initialize()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

        if (_canvas != null)
        {
            _canvasScaler = _canvas.GetComponent<CanvasScaler>();
            _canvasRectTransform = _canvas.GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// Method to Setup panel's close, open and anchor positions and do other stuffs at start
    /// </summary>
    protected abstract void OnSetup();
    
    private void Setup()
    {
        // Canvas and Camera
        if (_canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            _canvas.planeDistance = (_canvasRectTransform.rect.height / 2f) /
                                    Mathf.Tan((_canvas.worldCamera.fieldOfView / 2f) * Mathf.Deg2Rad);
            if (_canvas.worldCamera.farClipPlane < _canvas.planeDistance)
            {
                _canvas.worldCamera.farClipPlane = Mathf.Ceil(_canvas.planeDistance);
            }
        }
        
        var localPosition = _rectTransform.localPosition;
        _closedPosition = new Vector2(0, localPosition.y);
        _openPosition = new Vector2(_rectTransform.rect.width, localPosition.y);
        
        // Placement
        Vector2 anchorMin = Vector2.zero;
        Vector2 anchorMax = Vector2.zero;
        Vector2 pivot = Vector2.zero;

        anchorMin = new Vector2(0, 0.5f);
        anchorMax = new Vector2(0, 0.5f);
        pivot = new Vector2(1, 0.5f);

        _rectTransform.sizeDelta = _rectTransform.rect.size;
        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
        _rectTransform.pivot = pivot;
        
        OnSetup();
        
        // Default State
        SetState(CurrentState = _defaultState);
        _rectTransform.anchoredPosition = (_defaultState == State.Closed) ? _closedPosition : _openPosition;

        // Overlay
        if (_useOverlay)
        {
            _overlay = new GameObject(gameObject.name + " (Overlay)");
            _overlay.transform.parent = transform.parent;
            _overlay.transform.localScale = Vector3.one;
            _overlay.transform.SetSiblingIndex(transform.GetSiblingIndex());
            _overlay.layer = gameObject.layer;

            if (_useBlur)
            {
                _blur = new GameObject(gameObject.name + " (Blur)");
                _blur.transform.parent = transform.parent;
                _blur.transform.localScale = Vector3.one;
                _blur.transform.SetSiblingIndex(transform.GetSiblingIndex());

                RectTransform blurRectTransform = _blur.AddComponent<RectTransform>();
                blurRectTransform.anchorMin = Vector2.zero;
                blurRectTransform.anchorMax = Vector2.one;
                blurRectTransform.offsetMin = Vector2.zero;
                blurRectTransform.offsetMax = Vector2.zero;
                _blurImage = _blur.AddComponent<Image>();
                _blurImage.raycastTarget = false;
                _blurImage.material = new Material(_blurMaterial);
                _blurImage.material.SetInt("_Radius", 0);
            }

            RectTransform overlayRectTransform = _overlay.AddComponent<RectTransform>();
            overlayRectTransform.anchorMin = Vector2.zero;
            overlayRectTransform.anchorMax = Vector2.one;
            overlayRectTransform.offsetMin = Vector2.zero;
            overlayRectTransform.offsetMax = Vector2.zero;
            _overlayImage = _overlay.AddComponent<Image>();
            _overlayImage.color = (_defaultState == State.Open) ? _overlayColour : Color.clear;
            _overlayImage.raycastTarget = true; //_overlayCloseOnPressed;
            Button overlayButton = _overlay.AddComponent<Button>();
            overlayButton.transition = Selectable.Transition.None;
            overlayButton.onClick.AddListener(delegate { Close(); });
        }
    }

    private void HandleState()
    {
        if (!_isDragging)
        {
            Vector2 targetPosition = (TargetState == State.Closed) ? _closedPosition : _openPosition;
            _rectTransform.anchoredPosition = Vector2.Lerp(_rectTransform.anchoredPosition, targetPosition,
                Time.unscaledDeltaTime * _transitionSpeed);

            if (CurrentState != TargetState)
            {
                if ((_rectTransform.anchoredPosition - targetPosition).magnitude <= _rectTransform.rect.width / 10f)
                {
                    CurrentState = TargetState;
                    OnStateChanged.Invoke(CurrentState, TargetState);
                }
                else
                {
                    OnStateChanging.Invoke(CurrentState, TargetState);
                }
            }
        }
    }

    private void HandleOverlay()
    {
        if (_useOverlay)
        {
            _overlayImage.raycastTarget = (TargetState == State.Open);
            _overlayImage.color = new Color(_overlayColour.r, _overlayColour.g, _overlayColour.b,
                _overlayColour.a * StateProgress);

            if (_useBlur)
            {
                _blurImage.material.SetInt("_Radius", (int)(_blurRadius * StateProgress));
            }
        }
    }

    public void SetState(State state)
    {
        OnStateSelected.Invoke(TargetState = state);
    }

    public void ToggleState()
    {
        SetState((State)(((int)TargetState + 1) % 2));
    }

    public void Open()
    {
        SetState(State.Open);
        OnOpen();
    }

    protected abstract void OnOpen();

    public void Close()
    {
        SetState(State.Closed);
    }

    #endregion
}