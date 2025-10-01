
#pragma once

#pragma region "Includes"

	#include <windows.h>
	#include <stdbool.h>
	#include <stdio.h>

#pragma endregion

#pragma region "Methods"

	// Hook
    bool InstallKeyboardHook();
    void UninstallKeyboardHook();

	// Actions
    void HandleCtrl1();
    void HandleCtrl2();
    void HandleCtrl3();
    void HandleCtrl4();
    void HandleCtrl5();
	void HandleCtrlQ();

#pragma endregion
