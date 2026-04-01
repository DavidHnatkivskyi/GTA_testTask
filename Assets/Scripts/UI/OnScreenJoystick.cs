using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class OnScreenJoystick : MonoBehaviour
    {
        [Header("Element Names")]
        [SerializeField] private string baseElementName = "joystick-base";
        [SerializeField] private string knobElementName = "joystick-knob";

        [Header("Settings")]
        [SerializeField] private float maxRadius = 54f;
        [SerializeField] private bool clampToCircle = true;
        [SerializeField] private bool logDebugInfo = false;

        public Vector2 Value { get; private set; }
        public bool IsPressed { get; private set; }

        private UIDocument _uiDocument;
        private VisualElement _baseElement;
        private VisualElement _knobElement;

        private int _activePointerId = -1;
        private Vector2 _baseCenterWorld;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
           
            
            VisualElement root = _uiDocument.rootVisualElement;
            
            root.RegisterCallback<PointerDownEvent>(evt =>
            {
                Debug.Log($"ROOT POINTER DOWN: {evt.position}");
            });

            _baseElement = root.Q<VisualElement>(baseElementName);
            _knobElement = root.Q<VisualElement>(knobElementName);
            
            _baseElement.RegisterCallback<PointerDownEvent>(evt =>
            {
                Debug.Log("POINTER DOWN");
            });

            if (_baseElement == null)
            {
                Debug.LogError($"[OnScreenJoystick] Base element '{baseElementName}' not found.", this);
                enabled = false;
                return;
            }

            if (_knobElement == null)
            {
                Debug.LogError($"[OnScreenJoystick] Knob element '{knobElementName}' not found.", this);
                enabled = false;
                return;
            }

            _baseElement.RegisterCallback<PointerDownEvent>(OnPointerDown);
            _baseElement.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            _baseElement.RegisterCallback<PointerUpEvent>(OnPointerUp);
            _baseElement.RegisterCallback<PointerCancelEvent>(OnPointerCancel);

            ResetKnobPosition();
        }

        private void OnDisable()
        {
            if (_baseElement == null)
            {
                return;
            }

            _baseElement.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            _baseElement.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            _baseElement.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            _baseElement.UnregisterCallback<PointerCancelEvent>(OnPointerCancel);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (IsPressed)
            {
                return;
            }

            IsPressed = true;
            _activePointerId = evt.pointerId;

            _baseElement.CapturePointer(_activePointerId);

            UpdateBaseCenter();
            UpdateFromPointer(evt.position);

            DebugLog($"PointerDown id={evt.pointerId}, value={Value}");
            evt.StopPropagation();
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!IsPressed || evt.pointerId != _activePointerId)
            {
                return;
            }

            UpdateFromPointer(evt.position);

            DebugLog($"PointerMove id={evt.pointerId}, value={Value}");
            evt.StopPropagation();
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (!IsPressed || evt.pointerId != _activePointerId)
            {
                return;
            }

            ReleasePointer();
            DebugLog($"PointerUp id={evt.pointerId}");
            evt.StopPropagation();
        }

        private void OnPointerCancel(PointerCancelEvent evt)
        {
            if (!IsPressed || evt.pointerId != _activePointerId)
            {
                return;
            }

            ReleasePointer();
            DebugLog($"PointerCancel id={evt.pointerId}");
            evt.StopPropagation();
        }

        private void UpdateBaseCenter()
        {
            Rect worldBound = _baseElement.worldBound;
            _baseCenterWorld = worldBound.center;
        }

        private void UpdateFromPointer(Vector2 pointerWorldPosition)
        {
            Vector2 delta = pointerWorldPosition - _baseCenterWorld;

            if (clampToCircle)
            {
                delta = Vector2.ClampMagnitude(delta, maxRadius);
            }
            else
            {
                delta.x = Mathf.Clamp(delta.x, -maxRadius, maxRadius);
                delta.y = Mathf.Clamp(delta.y, -maxRadius, maxRadius);
            }

            Value = delta / maxRadius;
            MoveKnob(delta);
        }

        private void MoveKnob(Vector2 delta)
        {
            float baseWidth = _baseElement.resolvedStyle.width;
            float baseHeight = _baseElement.resolvedStyle.height;
            float knobWidth = _knobElement.resolvedStyle.width;
            float knobHeight = _knobElement.resolvedStyle.height;

            float centerX = (baseWidth - knobWidth) * 0.5f;
            float centerY = (baseHeight - knobHeight) * 0.5f;

            _knobElement.style.left = centerX + delta.x;
            _knobElement.style.top = centerY + delta.y;
        }

        private void ReleasePointer()
        {
            if (_baseElement.HasPointerCapture(_activePointerId))
            {
                _baseElement.ReleasePointer(_activePointerId);
            }

            _activePointerId = -1;
            IsPressed = false;
            Value = Vector2.zero;

            ResetKnobPosition();
        }

        private void ResetKnobPosition()
        {
            float baseWidth = _baseElement.resolvedStyle.width;
            float baseHeight = _baseElement.resolvedStyle.height;
            float knobWidth = _knobElement.resolvedStyle.width;
            float knobHeight = _knobElement.resolvedStyle.height;

            if (baseWidth <= 0f || baseHeight <= 0f || knobWidth <= 0f || knobHeight <= 0f)
            {
                _knobElement.schedule.Execute(ResetKnobPosition).ExecuteLater(1);
                return;
            }

            _knobElement.style.left = (baseWidth - knobWidth) * 0.5f;
            _knobElement.style.top = (baseHeight - knobHeight) * 0.5f;
        }

        private void DebugLog(string message)
        {
            if (!logDebugInfo)
            {
                return;
            }

            Debug.Log($"[OnScreenJoystick] {message}", this);
        }
        
        private void Update()
        {
            if (IsPressed)
            {
                Debug.Log(Value);
            }
        }
    }
}