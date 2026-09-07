# Korean Wordle - 네이밍 컨벤션

이 문서는 `.editorconfig`(프로젝트 루트, authoritative)에 정의된 네이밍 규칙의 해설입니다. `.editorconfig`와 이 문서가 어긋나면 **`.editorconfig`가 우선**입니다.

## 규칙 요약

| 대상 | 스타일 | 예시 |
|---|---|---|
| public / internal 멤버 (클래스, 구조체, 열거형, 인터페이스, 메서드, 프로퍼티, 이벤트, 델리게이트, 필드) | `PascalCase` | `public int CurrentStage;` |
| private / protected 필드 | `camelCase` (언더스코어 접두사 없음) | `private bool isWordListsLoaded;` |
| const 필드 | `ALL_UPPER` (`_` 구분자) | `public const string WORD_LIST_PATH = "...";` |
| local 변수 | `camelCase` | `int startIndex = (curPage - 1) * stagesPerPage;` |
| interface | `IPascalCase` | (본 코드베이스에는 아직 인터페이스가 없음. 도입 시 이 규칙 적용) |
| 제네릭 타입 매개변수 | `TPascalCase` | `KWLocalSingleton<T> where T : KWLocalSingleton<T>` |
| 네임스페이스 | `PascalCase` 계층 (`KW.`으로 시작) | `namespace KW.Managers` |
| Boolean | `Is`/`Has`/`Can`/`Should` 접두사 권장 | `public bool IsTodayCompleted;` |
| 컬렉션 | 복수형 권장 | `public List<string> NounList;` |

실제 코드 예시:

```csharp
// Assets/Scripts/Runtime/Core/GameManager.cs
namespace KW.Core
{
    public class GameManager : KWGlobalSingleton<GameManager>
    {
        public GameData_V2 Data;
        public int CurrentStage;
        public bool IsTodayCompleted = false;

        private bool isWordListsLoaded = false;
        private Dictionary<string, List<string>> wordListCache = new Dictionary<string, List<string>>();

        public bool IsWordListsLoaded => isWordListsLoaded;
    }
}
```

```csharp
// Assets/Scripts/Runtime/Core/Constants/ResourcePathsConstants.cs 스타일
public const string WORD_LIST_PATH = "WordList/Default";
```

## Unity 특수 규칙

- `[SerializeField]`가 붙는 필드도 private/protected 규칙(camelCase, 언더스코어 없음)을 그대로 따릅니다.
  ```csharp
  [SerializeField] private TMP_Text labelText;
  ```
- 기존에 `_camelCase`로 선언되어 있던 필드를 현재 규칙으로 리네임할 때는 인스펙터에 이미 할당된 값/참조가 끊기지 않도록 `[FormerlySerializedAs("_이전이름")]`을 함께 붙입니다. 실제 사례: `Runtime/UI/BarGraphItem.cs`, `Runtime/UI/BarGraphContainer.cs`.
  ```csharp
  [SerializeField, FormerlySerializedAs("_labelText")] private TMP_Text labelText;
  ```
- MonoBehaviour 생명주기 메서드(`Awake`, `Start`, `Update` 등)는 Unity 규약을 그대로 따릅니다. 싱글톤 계열(`KWGlobalSingleton<T>`, `KWLocalSingleton<T>`)에서는 `Awake()`를 직접 오버라이드하지 말고 `OnAwake()`를 사용합니다 (자세한 내용은 `Runtime/Managers/README_KWSingleton.md`).
- Coroutine 메서드는 동사로 시작 (`StartCoroutine(FadeOut())`).

## 네임스페이스 구조

실제 코드베이스에 존재하는 네임스페이스(2026-08 기준):

- `KW.Core` — GameManager, GameData
- `KW.Core.Constants` — KoreanInputConstants, ResourcePathsConstants, SceneNames (static 클래스, 이전에는 ScriptableObject 설정이었으나 이관됨)
- `KW.Core.Settings` — GameplaySettings, UILayoutSettings (ScriptableObject), SettingsManager, GameplayEnums
- `KW.Managers` — KWGlobalSingleton, KWLocalSingleton, KWPureSingleton, KWSingletonScriptable, HintManager, StatisticsManager, GameplayManager(GameType/GameStageType 포함)
- `KW.Input` — KoreanInputHandler(+Blocked/Deblocked), HangulComposer, KoreanCompositionState, KeyboardCell
- `KW.Gameplay` — GameWordSelector, GameplayCell(+Blocked/Deblocked), GameplayCellManager(+Blocked/Deblocked), GameplaySetupManager
- `KW.UI` / `KW.UI.Popup` / `KW.UI.Animation` / `KW.UI.Loading` / `KW.UI.Scene` — UI 컴포넌트 일반
- `KW.Utility` — KWDebug, CoroutineUtility
- `KW.Editor` / `KW.Editor.Core` / `KW.Editor.UI` / `KW.Editor.UI.Popup` — 에디터 확장(`KW.Editor.asmdef`, 별도 어셈블리)

> **알려진 예외 (구조적 불일치, 이번 문서 정리 범위 밖):** `Runtime/Gameplay/GameplayManager.cs`는 폴더상 Gameplay이지만 네임스페이스는 `KW.Managers`이고, `Runtime/Gameplay/GameplayCellManagerBlocked.cs`/`GameplayCellManagerDeblocked.cs`는 폴더상 Gameplay이지만 네임스페이스는 `KW.UI`입니다. 리팩터링 이력의 잔재이며 향후 정리 대상입니다.

`.Deprecated/`로 시작하는 폴더(점 접두사)는 Unity가 컴파일 대상에서 자동 제외하는 폴더입니다. 리네임하면 해당 폴더의 코드가 다시 컴파일되므로 주의합니다.

## `#region` 사용 정책

새로 작성하는 코드에서 `#region`을 쓸 경우 **기능 단위**로 나눕니다(예: `Initialization`, `Word Selection`, `Answer Management`). `Public Fields`/`Private Fields`처럼 접근제한자 축으로 나누는 방식은 지양합니다. 기존 파일의 `#region`을 이 정책에 맞춰 일괄 재편하는 작업은 하지 않습니다.

## 주석 정책

- 기본 언어는 영어. 초성/중성/종성, 곁받침, 모아쓰기/풀어쓰기 등 **한글 도메인 용어**를 설명할 때는 한국어 병기를 허용합니다.
- 자명한 코드(`// increment counter` 류)에는 주석을 달지 않습니다. 코드만으로 드러나지 않는 의도·제약·이유를 남깁니다.
- 로그 메시지의 태그(`[ClassName]`)는 실제 클래스명과 반드시 일치시킵니다.
