
#pragma region "Includes"

	#include "autotrim.h"

#pragma endregion

#pragma region "Variables"

	static HHOOK g_keyboardHook;

#pragma endregion

#pragma region "Hook"

	static LRESULT CALLBACK KeyboardHookProc(int nCode, WPARAM wParam, LPARAM lParam) {
		if (nCode >= 0 && wParam == WM_KEYDOWN) {
			PKBDLLHOOKSTRUCT keyInfo = (PKBDLLHOOKSTRUCT)lParam;
			DWORD vkCode = keyInfo->vkCode;
			
			bool ctrlPressed = (GetAsyncKeyState(VK_CONTROL) & 0x8000);
			bool shiftPressed = (GetAsyncKeyState(VK_SHIFT) & 0x8000);

			if (ctrlPressed) {
				switch (vkCode) {
					case '1':	HandleCtrl1();	break;
					case '2':	HandleCtrl2();	break;
					case '3':	HandleCtrl3();	break;
					case '4':	HandleCtrl4();	break;
					case '5':	HandleCtrl5();	break;
					case 'Q':	HandleCtrlQ();	break;
				}
			}
		}
		
		return (CallNextHookEx(g_keyboardHook, nCode, wParam, lParam));
	}

#pragma endregion

#pragma region "Install"

	bool InstallKeyboardHook() {
		HINSTANCE hInstance = GetModuleHandle(NULL);
		if (!hInstance) return (false);

		g_keyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProc, hInstance, 0);
		if (!g_keyboardHook) return (false);

		return (true);
	}

#pragma endregion

#pragma region "Uninstall"

	void UninstallKeyboardHook() {
		if (g_keyboardHook) {
			UnhookWindowsHookEx(g_keyboardHook);
			g_keyboardHook = NULL;
		}
	}

#pragma endregion