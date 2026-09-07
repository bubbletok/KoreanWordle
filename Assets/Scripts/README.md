# Scripts 폴더 구조

Korean Wordle 프로젝트의 스크립트 폴더 구조입니다. (2026-08 기준, 실제 코드 스캔 결과)

## 전체 구조

```
Assets/Scripts/
├── Runtime/                    # KW.Runtime.asmdef
│   ├── Core/                   # KW.Core / KW.Core.Constants / KW.Core.Settings
│   ├── Managers/                # KW.Managers
│   ├── Input/                   # KW.Input
│   ├── Gameplay/                 # KW.Gameplay (일부 파일은 KW.Managers/KW.UI — 아래 "알려진 예외" 참조)
│   ├── UI/                       # KW.UI / KW.UI.Popup / KW.UI.Animation / KW.UI.Loading / KW.UI.Scene
│   └── Utility/                  # KW.Utility
├── Editor/                     # KW.Editor.asmdef (Editor/README.md 참조)
├── .Deprecated/                 # 사용 중단, 컴파일 제외
├── NAMING_CONVENTIONS.md
└── README.md                   # 이 문서
```

`.Deprecated`는 점(`.`)으로 시작하는 폴더명입니다 — Unity가 이런 폴더를 컴파일 대상에서 자동 제외합니다. 리네임하면 안에 있는 코드가 다시 컴파일되어 빌드가 깨질 수 있습니다.

## Runtime/Core (KW.Core, KW.Core.Constants, KW.Core.Settings)

```
Core/
├── GameManager.cs               # 영구 게임 상태, 단어 리스트 로딩/캐싱, 세이브 관리 (KWGlobalSingleton)
├── GameData.cs                  # GameData_V1(레거시)/GameData_V2(현재) 저장 데이터 구조
├── Constants/
│   ├── KoreanInputConstants.cs  # 초/중/종성 자모 배열, 키보드 레이아웃 (static)
│   ├── ResourcePathsConstants.cs # Resources 경로 상수 (static)
│   └── SceneNames.cs            # 씬 이름 상수 (static)
└── Settings/
    ├── GameplayEnums.cs          # CellState, HangulComponentPosition
    ├── GameplaySettings.cs       # ScriptableObject (MaxAttempts 등 게임 규칙)
    ├── UILayoutSettings.cs       # ScriptableObject (해상도, 셀 크기 등)
    └── SettingsManager.cs        # GameplaySettings/UILayoutSettings 접근 파사드
```

`KoreanInputSettings`/`ResourcePathsSettings`/`SceneConfiguration`이라는 이름의 ScriptableObject는 존재하지 않습니다 — 과거 그렇게 설계되었으나 현재는 `Constants/` 아래의 static 클래스로 대체되었습니다.

## Runtime/Managers (KW.Managers)

```
Managers/
├── KWGlobalSingleton.cs         # MonoBehaviour 싱글톤 (씬 전환 유지, 자동 생성)
├── KWLocalSingleton.cs          # MonoBehaviour 싱글톤 (씬 한정, 수동 배치 필요)
├── KWPureSingleton.cs           # 순수 C# 싱글톤 (Lazy<T>)
├── KWSingletonScriptable.cs     # ScriptableObject 싱글톤 (Resources 자동 로드)
├── README_KWSingleton.md        # 4종 상세 비교 및 사용법
├── HintManager.cs               # 초성/품사 힌트 (KWLocalSingleton)
├── StatisticsManager.cs         # 게임 통계 계산
└── GameplayManager.cs*          # (파일은 Runtime/Gameplay/에 위치, 아래 "알려진 예외" 참조)
```

`KWSingleton<T>`라는 단일 클래스는 없습니다 — 위 4종으로 분화되어 있으며 자세한 사용법은 `Runtime/Managers/README_KWSingleton.md`를 참조하세요.

## Runtime/Input (KW.Input)

```
Input/
├── KoreanInputHandler.cs            # abstract 베이스 (KWLocalSingleton<KoreanInputHandler>)
├── KoreanInputHandler.Blocked.cs    # KoreanInputHandlerBlocked : KoreanInputHandler
├── KoreanInputHandler.Deblocked.cs  # KoreanInputHandlerDeblocked : KoreanInputHandler
├── HangulComposer.cs                # 한글 Unicode 조합/분해 static 유틸리티
├── KoreanCompositionState.cs        # 셀별 조합 상태(초/중/종 인덱스) 관리
├── KeyboardCell.cs                  # 가상 키보드 셀 (UI Toolkit 기반, POCO가 Button을 감쌈)
└── .Deprecated/                     # EnterCharacter, EnterCharBlocked, EnterCharDeblocked
```

`KoreanInputHandler.Blocked.cs`/`.Deblocked.cs`는 파일명이 `partial` 스타일이지만 실제로는 **partial class가 아니라 `KoreanInputHandler`를 상속하는 별개의 파생 클래스**(`KoreanInputHandlerBlocked`, `KoreanInputHandlerDeblocked`)입니다.

## Runtime/Gameplay (KW.Gameplay)

```
Gameplay/
├── GameWordSelector.cs              # 정답 선택 & 암호화(offset 214743673)
├── GameplayCell.cs                  # 게임플레이 셀 베이스 (UI Toolkit 기반, POCO가 VisualElement를 감쌈)
├── GameplayCellBlocked.cs           # Blocked 모드 셀(모아쓰기, 초/중/종 3개 자식 상태 엘리먼트)
├── GameplayCellDeblocked.cs         # Deblocked 모드 셀(풀어쓰기, 셀 자체가 상태 표시)
├── GameplayCellManager.cs           # 셀 그리드 관리 베이스(KWLocalSingleton, UIDocument, VisualTreeAsset 템플릿을 CloneTree)
├── GameplayCellManagerBlocked.cs*   # (네임스페이스 KW.UI, 아래 참조)
├── GameplayCellManagerDeblocked.cs* # (네임스페이스 KW.UI, 아래 참조)
├── GameplayManager.cs*              # (네임스페이스 KW.Managers, 아래 참조) - GameType/GameStageType enum 포함
├── GameplaySetupManager.cs          # 게임플레이 씬 초기화 시퀀스
├── TODO/
│   └── TodayWord.cs                 # 일일 단어 기능 스텁 (미구현, 빌드 씬 목록에 없음)
└── .Deprecated/                     # CharStageBase, CharBlockedStage, CharDeblockedStage
```

## Runtime/UI (KW.UI, KW.UI.Popup, KW.UI.Animation, KW.UI.Loading, KW.UI.Scene)

```
UI/
├── GameplayUIManager.cs, KeyboardView.cs (UI Toolkit 기반, UIDocument), StageCellManager.cs (UI Toolkit 기반, UIDocument),
│   LoadSceneButton.cs, ShowTutorialButton.cs, MainMenuUIManager.cs (UI Toolkit 기반, UIDocument),
│   FontApplier.cs, StatisticsUIManager.cs (UI Toolkit 기반, UIDocument)
├── Animation/       # UIAnimator(abstract), UIElementAnimator
├── Loading/         # LoadingScreen(KWLocalSingleton, UI Toolkit 기반, UIDocument)
├── Scene/           # StartSceneController
├── Popup/           # UI Toolkit 기반. BasePopup(abstract, POCO), PopupManager(KWGlobalSingleton, UIDocument),
│                     # GameResultPopup, ConfirmPopup, MessagePopup, TutorialPopup
│                     # UXML/USS: Assets/UIToolkit/Popup/
└── .Deprecated/     # ChooseMenu, StatisticsInformation, UserStats
```

## Runtime/Utility (KW.Utility)

```
Utility/
├── KWDebug.cs           # Debug 래퍼 (Runtime/Utility/KWDEBUG_USAGE.md 참조)
└── CoroutineUtility.cs  # 코루틴 헬퍼
```

## 알려진 예외 (폴더-네임스페이스 불일치, 이번 정리 범위 밖)

리팩터링 이력의 잔재로, 폴더 위치와 네임스페이스가 어긋난 파일이 3개 있습니다. 동작에는 문제가 없지만 새 코드를 작성할 때 참고하세요.

| 파일 | 폴더 | 실제 네임스페이스 |
|---|---|---|
| `Runtime/Gameplay/GameplayManager.cs` | Gameplay | `KW.Managers` |
| `Runtime/Gameplay/GameplayCellManagerBlocked.cs` | Gameplay | `KW.UI` |
| `Runtime/Gameplay/GameplayCellManagerDeblocked.cs` | Gameplay | `KW.UI` |

## `.Deprecated/` 폴더 (사용 중단, 컴파일 제외)

```
Scripts/.Deprecated/          AcceptCeritificates, UsingAPI, CameraResolution, CheckList, Vibration
Runtime/Input/.Deprecated/    EnterCharacter, EnterCharBlocked, EnterCharDeblocked
Runtime/Gameplay/.Deprecated/ CharStageBase, CharBlockedStage, CharDeblockedStage
Runtime/UI/.Deprecated/       ChooseMenu, StatisticsInformation, UserStats
```

각각 `KoreanInputHandler`, `GameWordSelector`, `LoadSceneButton`로 대체되었습니다.

## Assembly Definition

이 프로젝트는 **단일 Runtime 어셈블리 + 별도 Editor/Tests 어셈블리** 구조입니다(순환 참조를 피하고 관리를 단순화하기 위함). 어셈블리는 3개뿐입니다:

| 어셈블리 | 위치 | 역할 |
|---|---|---|
| `KW.Runtime` | `Assets/Scripts/Runtime/KW.Runtime.asmdef` | 모든 런타임 네임스페이스(KW.Core, KW.Managers, KW.Input, KW.Gameplay, KW.UI, KW.Utility 등) |
| `KW.Editor` | `Assets/Scripts/Editor/KW.Editor.asmdef` | Editor 전용, `KW.Runtime` 참조, 빌드 자동 제외 |
| `KW.Tests` | `Assets/Tests/KW.Tests.asmdef` | PlayMode 테스트, `KW.Runtime` 참조, `autoReferenced: false` + `UNITY_INCLUDE_TESTS` 제약으로 빌드 제외 |

새 스크립트를 추가할 때 asmdef를 새로 만들 필요는 없습니다 — `Runtime/` 아래 어디에 두든 `KW.Runtime`에 자동 포함되고, 모든 KW 네임스페이스가 같은 어셈블리 안에 있어 자유롭게 서로 참조할 수 있습니다.

프로젝트 루트에 `KW.Core.csproj` 등 개별 csproj 파일이 남아 있는 경우가 있는데, 이는 과거 다중 어셈블리 구조의 잔재이며 `.gitignore`로 제외되고 실제 `.sln`/`.slnx`가 참조하지 않는 stale 파일입니다.

## 새 스크립트 작성 시

- 폴더 구조와 네임스페이스를 일치시킵니다: `Runtime/UI/` → `namespace KW.UI`
- 네이밍 규칙은 `NAMING_CONVENTIONS.md` 참조
- 하드코딩 대신 `GameplaySettings.Instance`/`UILayoutSettings.Instance`(ScriptableObject 설정) 또는 `KW.Core.Constants`의 static 클래스(자모 배열, 씬 이름, 리소스 경로)를 사용
- 싱글톤이 필요하면 `Runtime/Managers/README_KWSingleton.md`에서 4종 중 알맞은 것을 선택
- Editor 스크립트는 `Assets/Scripts/Editor/`에 대응 하위 폴더로 배치(`Editor/README.md` 참조)

## 관련 문서

- `NAMING_CONVENTIONS.md` — 네이밍 규칙 및 주석 정책
- `Runtime/Managers/README_KWSingleton.md` — 싱글톤 4종 사용법
- `Runtime/Utility/KWDEBUG_USAGE.md` — KWDebug 사용법
- `Editor/README.md` — Editor 스크립트 목록 및 작성 가이드
- 프로젝트 루트 `CLAUDE.md` — 전체 아키텍처 개요
