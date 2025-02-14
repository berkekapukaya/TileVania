using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class AnchorToolsEditor : EditorWindow
{
    private bool _stickAnchorsToRect;

    AnchorToolsEditor()
    {
        SceneView.duringSceneGui += OnScene;
    }

    [MenuItem("Tools/Anchor Tools")]
    private static void Init()
    {
        AnchorToolsEditor editorScreenshot = GetWindow<AnchorToolsEditor>(title: "Anchor Tools");

        if (EditorPrefs.HasKey("AnchorToolsEditor.screenshotFolderPath"))
            editorScreenshot._stickAnchorsToRect = EditorPrefs.GetBool("AnchorToolsEditor.stickAnchorsToRect");
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        _stickAnchorsToRect = EditorGUILayout.Toggle("Stick Anchors to Rect", _stickAnchorsToRect);

        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetBool("AnchorToolsEditor.stickAnchorsToRect", _stickAnchorsToRect);
        }

        if (GUILayout.Button("Stick Anchors to Rect")) UpdateAnchors();
    }

    private void OnScene(SceneView sceneView)
    {
        if (_stickAnchorsToRect && Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            UpdateAnchors();
        }
    }

    public void OnDestroy()
    {
        Debug.Log("[AnchorToolsEditor] Unregistering for anchors update On Scene GUI");
        SceneView.duringSceneGui -= OnScene;
    }

    private static Rect _anchorRect;
    private static Vector2 _anchorVector;  // This is currently unused, so it's always (0, 0). It seems we can remove it.
    private static Rect _anchorRectOld;
    private static Vector2 _anchorVectorOld;
    private static RectTransform _currentRectTransform;
    private static RectTransform _parentRectTransform;
    private static Vector2 _pivotOld;
    private static Vector2 _offsetMinOld;
    private static Vector2 _offsetMaxOld;

    private static void UpdateAnchors()
    {
        TryToGetRectTransform();
        if (_currentRectTransform != null && _parentRectTransform != null && ShouldStick())
        {
            Stick();
        }
    }

    private static bool ShouldStick()
    {
        return (
            _currentRectTransform.offsetMin != _offsetMinOld ||
            _currentRectTransform.offsetMax != _offsetMaxOld ||
            _currentRectTransform.pivot != _pivotOld ||
            _anchorVector != _anchorVectorOld ||
            _anchorRect != _anchorRectOld
        );
    }

    private static void Stick()
    {
        CalculateCurrentWh();
        CalculateCurrentXY();

        CalculateCurrentXY();
        _pivotOld = _currentRectTransform.pivot;
        _anchorVectorOld = _anchorVector;

        AnchorsToCorners();
        _anchorRectOld = _anchorRect;
    }

    private static void TryToGetRectTransform()
    {
        if (Selection.activeGameObject != null)
        {
            _currentRectTransform = Selection.activeGameObject.GetComponent<RectTransform>();
            if (_currentRectTransform != null && _currentRectTransform.parent != null)
            {
                _parentRectTransform = _currentRectTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                _parentRectTransform = null;
            }
        }
        else
        {
            _currentRectTransform = null;
            _parentRectTransform = null;
        }
    }

    private static void CalculateCurrentXY()
    {
        float pivotX = _anchorRect.width * _currentRectTransform.pivot.x;
        float pivotY = _anchorRect.height * (1 - _currentRectTransform.pivot.y);
        Vector2 newXY = new Vector2(_currentRectTransform.anchorMin.x * _parentRectTransform.rect.width + _currentRectTransform.offsetMin.x + pivotX - _parentRectTransform.rect.width * _anchorVector.x,
            -(1 - _currentRectTransform.anchorMax.y) * _parentRectTransform.rect.height + _currentRectTransform.offsetMax.y - pivotY + _parentRectTransform.rect.height * (1 - _anchorVector.y));
        _anchorRect.x = newXY.x;
        _anchorRect.y = newXY.y;
        _anchorRectOld = _anchorRect;
    }

    private static void CalculateCurrentWh()
    {
        var rect = _currentRectTransform.rect;
        _anchorRect.width = rect.width;
        _anchorRect.height = rect.height;
        _anchorRectOld = _anchorRect;
    }

    private static void AnchorsToCorners()
    {
        Undo.RecordObject(_currentRectTransform, "Stick Anchors");

        float pivotX = _anchorRect.width * _currentRectTransform.pivot.x;
        float pivotY = _anchorRect.height * (1 - _currentRectTransform.pivot.y);
        _currentRectTransform.anchorMin = new Vector2(0f, 1f);
        _currentRectTransform.anchorMax = new Vector2(0f, 1f);
        _currentRectTransform.offsetMin = new Vector2(_anchorRect.x / _currentRectTransform.localScale.x, _anchorRect.y / _currentRectTransform.localScale.y - _anchorRect.height);
        _currentRectTransform.offsetMax = new Vector2(_anchorRect.x / _currentRectTransform.localScale.x + _anchorRect.width, _anchorRect.y / _currentRectTransform.localScale.y);
        _currentRectTransform.anchorMin = new Vector2(_currentRectTransform.anchorMin.x + _anchorVector.x + (_currentRectTransform.offsetMin.x - pivotX) / _parentRectTransform.rect.width * _currentRectTransform.localScale.x,
            _currentRectTransform.anchorMin.y - (1 - _anchorVector.y) + (_currentRectTransform.offsetMin.y + pivotY) / _parentRectTransform.rect.height * _currentRectTransform.localScale.y);
        _currentRectTransform.anchorMax = new Vector2(_currentRectTransform.anchorMax.x + _anchorVector.x + (_currentRectTransform.offsetMax.x - pivotX) / _parentRectTransform.rect.width * _currentRectTransform.localScale.x,
            _currentRectTransform.anchorMax.y - (1 - _anchorVector.y) + (_currentRectTransform.offsetMax.y + pivotY) / _parentRectTransform.rect.height * _currentRectTransform.localScale.y);
        _currentRectTransform.offsetMin = new Vector2((0 - _currentRectTransform.pivot.x) * _anchorRect.width * (1 - _currentRectTransform.localScale.x), (0 - _currentRectTransform.pivot.y) * _anchorRect.height * (1 - _currentRectTransform.localScale.y));
        _currentRectTransform.offsetMax = new Vector2((1 - _currentRectTransform.pivot.x) * _anchorRect.width * (1 - _currentRectTransform.localScale.x), (1 - _currentRectTransform.pivot.y) * _anchorRect.height * (1 - _currentRectTransform.localScale.y));

        _offsetMinOld = _currentRectTransform.offsetMin;
        _offsetMaxOld = _currentRectTransform.offsetMax;
    }
}

#endif