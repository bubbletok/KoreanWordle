# Editor Scripts

Korean Wordle 프로젝트의 Unity Editor 전용 스크립트입니다. 별도 어셈블리(`KW.Editor.asmdef`)로 분리되어 있어 빌드에 포함되지 않습니다.

## 폴더 구조

```
Assets/Scripts/Editor/
├── KWEditor.cs
├── GameWordSelectorEditor.cs
├── GameplaySetupManagerEditor.cs
├── Core/
│   ├── GameSettingsCreator.cs
│   └── GameDataDebugger.cs
├── UI/
│   ├── LoadSceneButtonEditor.cs
│   └── FontApplierEditor.cs
├── UI/Popup/
│   └── GameResultPopupEditor.cs
└── KW.Editor.asmdef
```

## 네임스페이스

원칙적으로는 `KW.Editor`이며, `Core/`·`UI/`·`UI/Popup/` 하위 폴더에서는 `KW.Editor.Core` / `KW.Editor.UI` / `KW.Editor.UI.Popup`처럼 폴더를 반영한 세부 네임스페이스도 쓰입니다. 다만 아래 표에 ⚠로 표시한 3개 파일은 폴더와 네임스페이스가 일치하지 않는 상태로 남아 있습니다(향후 정리 대상, 이번 정리 범위 밖).

## 스크립트 목록

| 파일 | 네임스페이스 | 대상 컴포넌트 |
|---|---|---|
| `KWEditor.cs` | `KW.Editor` | 제네릭 Custom Inspector 베이스 (`KWEditor<T> : Editor where T : MonoBehaviour`) |
| `GameWordSelectorEditor.cs` | `KW.Editor` | `GameWordSelector` |
| `GameplaySetupManagerEditor.cs` | `KW.Editor` | `GameplaySetupManager` |
| `Core/GameSettingsCreator.cs` | `KW.Editor` ⚠ (폴더는 Core) | ScriptableObject 설정 생성 도구 (컴포넌트 아님) |
| `Core/GameDataDebugger.cs` | `KW.Editor.Core` | `GameManager` (세이브 데이터 디버깅용) |
| `UI/LoadSceneButtonEditor.cs` | `KW.Editor` ⚠ (폴더는 UI) | `LoadSceneButton`, `KWEditor<LoadSceneButton>` 상속 |
| `UI/FontApplierEditor.cs` | `KW.Editor.UI` | `FontApplier` + `BulkFontApplierTool`(EditorWindow) |
| `UI/Popup/GameResultPopupEditor.cs` | `KW.Editor.UI.Popup` | `GameResultPopup` |

## GameSettingsCreator 메뉴

`Core/GameSettingsCreator.cs`가 제공하는 Unity 메뉴 항목(실제 코드 기준, 2026-08):

- `KW/Create Game Settings (Complete Setup)` — `UILayoutSettings.asset`, `GameplaySettings.asset`을 `Assets/Resources/`에 한 번에 생성(이미 있으면 건너뜀)
- `KW/Settings/Create UI Layout Settings` — `UILayoutSettings`만 생성
- `KW/Settings/Create Gameplay Settings` — `GameplaySettings`만 생성

`KoreanInputSettings`/`SceneConfiguration`/`ResourcePathsSettings`는 더 이상 ScriptableObject가 아니라 `KW.Core.Constants`의 static 클래스이므로 이 도구가 생성할 대상이 아닙니다.

## Assembly Definition

`Assets/Scripts/Editor/KW.Editor.asmdef` (실제 내용):

```json
{
    "name": "KW.Editor",
    "rootNamespace": "KW.Editor",
    "references": ["KW.Runtime", "Unity.TextMeshPro"],
    "includePlatforms": ["Editor"],
    "autoReferenced": true
}
```

- `KW.Runtime` 어셈블리(런타임 스크립트 전체)를 참조하므로 모든 Runtime 타입에 접근 가능합니다.
- `includePlatforms: ["Editor"]`로 인해 빌드에서 자동 제외됩니다.

## 새 Editor 스크립트 작성 가이드

### Custom Inspector (KWEditor 베이스 사용)

```csharp
using UnityEditor;
using KW.Gameplay;

namespace KW.Editor
{
    [CustomEditor(typeof(MyComponent))]
    public class MyComponentEditor : KWEditor<MyComponent>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI(); // serializedObject.Update() 자동 호출
            DrawDefaultInspector();
        }
    }
}
```

### Editor Window

```csharp
using UnityEditor;
using UnityEngine;

namespace KW.Editor
{
    public class MyEditorWindow : EditorWindow
    {
        [MenuItem("KW/Tools/My Tool")]
        public static void ShowWindow() => GetWindow<MyEditorWindow>("My Tool");

        private void OnGUI()
        {
            GUILayout.Label("My Editor Window", EditorStyles.boldLabel);
        }
    }
}
```

## 폴더 구성 규칙

Runtime 스크립트와 대응하는 하위 폴더에 배치합니다(`Runtime/UI/` → `Editor/UI/` 등). Runtime 폴더 안에 Editor 스크립트를 두지 않습니다(`Assets/Scripts/Runtime/*/Editor/` 금지).

파일 네이밍: Custom Inspector는 `{ComponentName}Editor.cs`, Editor Window는 `{FeatureName}Window.cs`, Property Drawer는 `{TypeName}Drawer.cs`, 유틸리티 도구는 `{FeatureName}Creator.cs`/`{FeatureName}Validator.cs`.

## 참고 자료

- [Unity Custom Inspectors](https://docs.unity3d.com/Manual/editor-CustomEditors.html)
- [Unity Editor Windows](https://docs.unity3d.com/Manual/editor-EditorWindows.html)
- `Assets/Scripts/README.md` — 전체 스크립트 구조
