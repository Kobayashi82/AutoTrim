
#pragma region "Includes"

	#include "autotrim.h"

#pragma endregion

#pragma region "Actions"

	void HandleCtrl1() {
		printf("Ctrl+1 presionado - Ejecutando accion 1\n");
	}

	void HandleCtrl2() {
		printf("Ctrl+2 presionado - Ejecutando accion 2\n");
	}

	void HandleCtrl3() {
		printf("Ctrl+3 presionado - Ejecutando accion 3\n");
	}

	void HandleCtrl4() {
		printf("Ctrl+4 presionado - Ejecutando accion 4\n");
	}

	void HandleCtrl5() {
		printf("Ctrl+5 presionado - Ejecutando accion 5\n");
	}

	void HandleCtrlQ() {
		printf("Saliendo del programa...\n");
		PostQuitMessage(0);
	}

#pragma endregion
