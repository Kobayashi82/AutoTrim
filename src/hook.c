#include "autotrim.h"

t_WinkeyState g_winkeyState = {0};

// Detects if the active window has changed
static BOOL HasWindowChanged(void) {
    HWND currentWindow;

    currentWindow = GetForegroundWindow();
    if (currentWindow != g_winkeyState.lastWindow) {
        g_winkeyState.lastWindow = currentWindow;
        return (TRUE);
    }
    return (FALSE);
}

// Low level keyboard hook callback
LRESULT CALLBACK LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam) {
    PKBDLLHOOKSTRUCT keyInfo;

    // Only process if code is valid and it's a key press
    if (nCode >= 0 && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN)) {
        keyInfo = (PKBDLLHOOKSTRUCT)lParam;
    }

    // Pass to next hook in the chain
    return (CallNextHookEx(g_winkeyState.keyboardHook, nCode, wParam, lParam));
}
