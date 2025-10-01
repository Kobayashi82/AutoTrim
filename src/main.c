
#pragma region "Includes"

	#include "autotrim.h"

#pragma endregion

#pragma region "Is Admin"

	static bool IsAdmin() {
		bool isAdmin = FALSE;
		HANDLE hToken = NULL;

		if (OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY, &hToken)) {
			TOKEN_ELEVATION elevation;
			DWORD dwSize = sizeof(TOKEN_ELEVATION);
			if (GetTokenInformation(hToken, TokenElevation, &elevation, sizeof(elevation), &dwSize))
				isAdmin = elevation.TokenIsElevated;       
			CloseHandle(hToken);
		}

		return (isAdmin);
	}

#pragma endregion

#pragma region "Main"

	int main() {
		AllocConsole();
		freopen_s((FILE**)stdout, "CONOUT$", "w", stdout);
		freopen_s((FILE**)stderr, "CONOUT$", "w", stderr);
		freopen_s((FILE**)stdin, "CONIN$", "r", stdin);

		printf("==============================\n");
		printf("           AutoTrim           \n");
		printf("==============================\n");
		printf("\n");

		if (!IsAdmin()) {
			printf("Por favor, ejecuta el programa como administrador\n");
			printf("\nPresiona Enter para salir...");
			getchar();
			return (1);
		}

		if (!InstallKeyboardHook()) {
			printf("No se pudo inicializar el hook del teclado\n");
			printf("\nPresiona Enter para salir...");
			getchar();
			return (1);
		}

		printf("Combinaciones disponibles:\n");
		printf("  Ctrl+1 - Accion 1\n");
		printf("  Ctrl+2 - Accion 2\n");
		printf("  Ctrl+3 - Accion 3\n");
		printf("  Ctrl+4 - Accion 4\n");
		printf("  Ctrl+5 - Accion 5\n");
		printf("  Ctrl+Q - Salir del programa\n");
		printf("=================================================\n");

		bool g_running = true;
		while (g_running) {
			MSG msg;
			if (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE)) {
				if (msg.message == WM_QUIT) g_running = false;
				DispatchMessage(&msg);
			}

			Sleep(10);
		}

		UninstallKeyboardHook();

		return (0);
	}

#pragma endregion
