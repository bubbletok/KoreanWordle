# KWDebug Usage Guide

KWDebug is a wrapper around Unity's Debug class with additional features for the Korean Wordle project.

## Features

✅ **Automatic [KW] prefix** - All logs are tagged with `[KW]` for easy filtering
✅ **Namespace-specific logging** - Tag logs by namespace (e.g., `[KW].Input`)
✅ **Conditional compilation** - Log/Assert calls removed in Release builds
✅ **Color support** - Rich text formatting for better visibility
✅ **Unity format compatible** - Same API as Unity Debug
✅ **Performance logging** - Built-in performance measurement tools

## Basic Usage

### Simple Logging

```csharp
using KW.Utility;

// Basic log
KWDebug.Log("Game started");
// Output: [KW] Game started

// Log with namespace tag
KWDebug.Log("Key pressed: Space", "Input");
// Output: [KW].Input Key pressed: Space

// Log with context (clickable in Unity Console)
KWDebug.Log("Cell initialized", gameObject);
// Output: [KW] Cell initialized (clicking will highlight the GameObject)
```

### Warning and Error Logging

```csharp
// Warning (always active)
KWDebug.LogWarning("Save file not found, using defaults");
// Output: [KW] Save file not found, using defaults

// Error (always active)
KWDebug.LogError("Failed to load word list!");
// Output: [KW] Failed to load word list!

// Exception logging
try
{
    // ... code ...
}
catch (Exception e)
{
    KWDebug.LogException(e);
}
```

### Colored Logging

```csharp
// Success message (green)
KWDebug.LogSuccess("Level completed!");
// Output: [KW] Level completed! (in green)

// Custom color
KWDebug.LogColored("Special event triggered", "#FF00FF");
// Output: [KW] Special event triggered (in purple)
```

### Formatted Logging

```csharp
int currentHP = 80;
int maxHP = 100;

KWDebug.LogFormat("Player HP: {0}/{1}", currentHP, maxHP);
// Output: [KW] Player HP: 80/100

KWDebug.LogWarningFormat("Only {0} attempts remaining!", attemptsLeft);
KWDebug.LogErrorFormat("Invalid word length: {0}, expected: {1}", actual, expected);
```

## Namespace-Specific Logger

For classes that log frequently, create a namespace logger:

```csharp
using KW.Utility;

namespace KW.Input
{
    public class KoreanInputHandler
    {
        // Create logger once
        private static readonly KWDebug.NamespaceLogger _logger = KWDebug.GetLogger("Input");

        public void ProcessInput(string input)
        {
            _logger.Log($"Processing input: {input}");
            // Output: [KW.Input] Processing input: ㄱ

            if (string.IsNullOrEmpty(input))
            {
                _logger.LogWarning("Empty input received");
                // Output: [KW.Input] Empty input received
            }
        }
    }
}
```

### Recommended Namespace Tags

- `Input` - Korean input handling
- `Gameplay` - Game logic and mechanics
- `UI` - UI managers and components
- `Core` - Core systems (GameManager, GameData)
- `Save` - Save/load operations
- `Audio` - Audio management
- `Network` - Network operations (if applicable)

## Assertions

```csharp
// Assert condition (removed in Release builds)
KWDebug.Assert(wordLength > 0, "Word length must be positive!");

// Assert with context
KWDebug.Assert(cellArray != null, "Cell array is null!", gameObject);
```

## Performance Logging

### Simple Performance Message

```csharp
KWDebug.LogPerformance("Word list loaded: 713K entries");
// Output: [KW] [PERF] Word list loaded: 713K entries
```

### Measuring Execution Time

```csharp
using System.Diagnostics;

var stopwatch = Stopwatch.StartNew();

// ... expensive operation ...
LoadWordList();

KWDebug.LogExecutionTime("LoadWordList", stopwatch);
// Output: [KW] [PERF] LoadWordList took 245ms
```

## Scene View Debug Drawing

```csharp
// Draw a ray (only in Editor)
KWDebug.DrawRay(transform.position, transform.forward, Color.red, 2f);

// Draw a line (only in Editor)
KWDebug.DrawLine(startPos, endPos, Color.green, 1f);
```

## Build Behavior

### Development Build / Unity Editor
- `KWDebug.Log()` - ✅ Active
- `KWDebug.LogWarning()` - ✅ Active
- `KWDebug.LogError()` - ✅ Active
- `KWDebug.Assert()` - ✅ Active
- Performance logs - ✅ Active

### Release Build
- `KWDebug.Log()` - ❌ Removed (no performance cost)
- `KWDebug.LogWarning()` - ✅ Active (important warnings still logged)
- `KWDebug.LogError()` - ✅ Active (errors always logged)
- `KWDebug.Assert()` - ❌ Removed (no performance cost)
- Performance logs - ❌ Removed

## Migration from Unity Debug

Replace Unity Debug calls with KWDebug:

```csharp
// Before
Debug.Log("Message");
Debug.LogWarning("Warning");
Debug.LogError("Error");

// After
KWDebug.Log("Message");
KWDebug.LogWarning("Warning");
KWDebug.LogError("Error");
```

## Best Practices

### ✅ DO

```csharp
// Use namespace loggers for frequently logging classes
private static readonly KWDebug.NamespaceLogger _logger = KWDebug.GetLogger("Input");

// Use formatted logging for complex messages
KWDebug.LogFormat("Cell [{0}, {1}] state: {2}", row, col, state);

// Add context for GameObject-related logs
KWDebug.Log("Initialization complete", gameObject);

// Use appropriate log levels
KWDebug.Log("User clicked button");           // Info
KWDebug.LogWarning("Unexpected state");        // Warning
KWDebug.LogError("Critical failure");          // Error
```

### ❌ DON'T

```csharp
// Don't use string concatenation in hot paths
for (int i = 0; i < 10000; i++)
{
    KWDebug.Log("Processing " + i); // ❌ Bad (string allocation even if removed)
}

// Don't log in Update() without guards
void Update()
{
    KWDebug.Log("Update called"); // ❌ Bad (spam)
}

// Don't use Log for errors
KWDebug.Log("Error: File not found"); // ❌ Bad (use LogError)
KWDebug.LogError("File not found");   // ✅ Good
```

## Unity Console Filtering

Filter logs in Unity Console using the search box:

- `[KW]` - Show all KW logs
- `[KW.Input]` - Show only Input namespace logs
- `[KW.Gameplay]` - Show only Gameplay namespace logs
- `[PERF]` - Show only performance logs

## Color Reference

Built-in colors used by KWDebug:

- **Info**: `#00BFFF` (Deep Sky Blue)
- **Warning**: `#FFA500` (Orange) - Unity default yellow
- **Error**: `#FF4500` (Orange Red) - Unity default red
- **Success**: `#32CD32` (Lime Green)
- **Performance**: `#00FFFF` (Cyan)

## Example: Complete Class

```csharp
using UnityEngine;
using KW.Utility;
using static KW.Core.Settings.GameplayEnums;

namespace KW.Gameplay
{
    public class GameplayCell : MonoBehaviour
    {
        private static readonly KWDebug.NamespaceLogger _logger = KWDebug.GetLogger("Gameplay");

        private void Awake()
        {
            _logger.Log("GameplayCell Awake", this);
        }

        public void SetCellState(CellState state, HangulComponentPosition position)
        {
            KWDebug.Assert(state != null, "CellState cannot be null!", this);

            if (position < 0 || position > HangulComponentPosition.Jongsung)
            {
                _logger.LogWarning($"Invalid position: {position}");
                return;
            }

            _logger.LogFormat("Setting cell state: {0} at position {1}", state, position);

            // ... implementation ...
        }
    }
}
```

## Notes

- All KWDebug methods use Unity's Debug internally, so stack traces work correctly
- Logs appear in Unity Console with proper file/line clickability
- No performance impact in Release builds (Log/Assert calls are completely removed)
- Color formatting uses Unity's rich text tags
