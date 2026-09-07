# KW 싱글톤 패턴

`KW.Managers` 네임스페이스에 4종의 제네릭 싱글톤 베이스 클래스가 있습니다. 하나의 `KWSingleton<T>`가 아니라 용도별로 나뉘어 있으니 새 매니저를 작성할 때는 아래 표로 골라 씁니다.

| 타입 | 베이스 | 자동 생성 | 씬 전환 시 유지 | 대표 사용처 |
|---|---|---|---|---|
| `KWGlobalSingleton<T>` | `MonoBehaviour` | O (없으면 `FindFirstObjectByType` 후 GameObject 생성) | O (`DontDestroyOnLoad`) | `GameManager` |
| `KWLocalSingleton<T>` | `MonoBehaviour` | X (씬에 직접 배치해야 함, 없으면 `Instance == null`) | X (씬 한정) | `GameplayManager`, `HintManager`, `StatisticsManager` |
| `KWPureSingleton<T>` | 순수 C# (`new()` 제약) | O (`Lazy<T>`로 최초 접근 시) | 해당 없음 | Unity 비의존 서비스/데이터 클래스 |
| `KWSingletonScriptable<T>` | `ScriptableObject` | X (Resources에 asset 필요) | 해당 없음(에셋) | `GameplaySettings`, `UILayoutSettings` |

파일 위치: `Assets/Scripts/Runtime/Managers/{KWGlobalSingleton,KWLocalSingleton,KWPureSingleton,KWSingletonScriptable}.cs`

## KWGlobalSingleton\<T\>

씬을 넘나들며 항상 하나만 존재해야 하는 전역 매니저용입니다.

```csharp
namespace KW.Core
{
    public class GameManager : KWGlobalSingleton<GameManager>
    {
        protected override void OnAwake()
        {
            // Awake() 대신 이걸 오버라이드
            LoadData();
        }
    }
}

// 다른 스크립트에서
GameManager.Instance.SaveData(...);
if (GameManager.Exists) { ... }
```

- `Instance`에 처음 접근하면 `FindFirstObjectByType<T>()`로 씬에서 찾고, 없으면 새 GameObject를 만들어 붙입니다.
- `DontDestroyOnLoad`가 자동 적용됩니다.
- `Awake()`를 직접 오버라이드하지 마세요 — 인스턴스 등록/중복 제거 로직이 거기 있습니다. 초기화 코드는 `OnAwake()`에 작성합니다.
- 종료 시점(`OnApplicationQuit`) 이후 `Instance`에 접근하면 경고 로그와 함께 `null`을 반환합니다(오브젝트 파괴 순서 문제 방지).

## KWLocalSingleton\<T\>

씬 안에서만 유효한 세션 상태(현재 게임플레이 씬의 매니저 등)에 사용합니다.

```csharp
namespace KW.Managers
{
    public sealed class GameplayManager : KWLocalSingleton<GameplayManager>
    {
        protected override void OnAwake() { ... }
    }
}
```

- **자동 생성하지 않습니다.** 씬에 GameObject로 미리 배치해야 하며, 없으면 `Instance`가 `null`입니다 — 접근 전 `Exists` 체크 권장.
- 씬 전환으로 오브젝트가 파괴되면 `OnDestroy()`에서 정적 참조가 자동으로 `null`로 정리됩니다.
- `OnAwake()`(초기화), `Init()`(외부에서 명시적으로 호출하는 2단계 초기화), `OnSceneDestroy()`(정리 로직) 3개의 훅을 제공합니다.

## KWPureSingleton\<T\>

Unity API에 의존하지 않는 순수 C# 서비스/데이터 클래스용입니다.

```csharp
public class WordValidator : KWPureSingleton<WordValidator>
{
    protected override void OnInitialize() { /* 최초 접근 시 1회 */ }
}

WordValidator.Instance.Validate(word);
```

- `Lazy<T>`로 스레드 안전하게 최초 접근 시 생성됩니다.
- `new T()`가 가능해야 하므로 `T`는 `new()` 제약을 만족해야 합니다.
- `IsInitialized`로 인스턴스 생성 여부를 확인할 수 있습니다(`KWGlobalSingleton`/`KWLocalSingleton`의 `Exists`와 동일한 역할이지만 이름이 다릅니다).
- 생성자에서 중복 생성을 막지만(두 번째 `new T()` 시도 시 예외), 정상적인 사용에서는 `Instance`를 통해서만 접근하므로 문제되지 않습니다.

## KWSingletonScriptable\<T\>

ScriptableObject 기반 설정값에 사용합니다 (`GameplaySettings`, `UILayoutSettings`).

```csharp
[CreateAssetMenu(fileName = "MySettings", menuName = "KW/Settings/My Settings")]
public class MySettings : KWSingletonScriptable<MySettings>
{
    public int someValue;
}
```

- `Instance`에 처음 접근하면 `Resources.Load<T>(typeof(T).Name)`으로 로드합니다. 즉 **에셋 파일명이 클래스명과 정확히 일치**해야 하고, `Assets/Resources/` 바로 아래(또는 커스텀 경로) 있어야 합니다.
- 커스텀 경로가 필요하면 `GetResourcePath()`를 오버라이드합니다(`protected static new string GetResourcePath()`).
- `Reload()`를 호출하면 캐시를 비우고 다시 로드합니다. 런타임에 값을 바꿔 테스트할 때 사용.
- 에셋을 못 찾으면 `KWDebug.LogError`로 실패 사실을 로그에 남기고 `Instance`는 `null`을 반환합니다 — 이후 `Instance.someValue` 접근 시 NullReferenceException이 나므로 asset이 `Assets/Resources/`에 실제로 있는지 먼저 확인하세요.

## 언제 무엇을 쓸까

- 게임 전역에서 씬을 넘어 유지되어야 하는 매니저 → `KWGlobalSingleton<T>`
- 특정 씬(게임플레이 세션 등) 안에서만 유효한 매니저 → `KWLocalSingleton<T>`
- Unity 라이프사이클이 필요 없는 서비스/헬퍼 → `KWPureSingleton<T>`
- 에디터에서 값을 조정하는 설정/데이터 → `KWSingletonScriptable<T>`
- 그 외 상태 없는 헬퍼는 그냥 `static class` (예: `KWDebug`, `CoroutineUtility`)
