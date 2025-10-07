<div align="center">

![MSFS 2020](https://img.shields.io/badge/MSFS-2020-blue?style=for-the-badge)
![PID Controller](https://img.shields.io/badge/PID-Controller-green?style=for-the-badge)
![Arduino Integration](https://img.shields.io/badge/Arduino-Integration-teal?style=for-the-badge)
![C Language](https://img.shields.io/badge/Language-VB.Net-red?style=for-the-badge)

*Sistema automático de control de trim para Microsoft Flight Simulator 2020*

</div>

# AutoTrim

## 🎯 Descripción

`AutoTrim` es un sistema de control automático de trim que se integra con Microsoft Flight Simulator 2020 a través de FSUIPC. El programa lee continuamente los datos de trim del simulador y utiliza un controlador PID para ajustar físicamente la rueda de trim del Honeycomb Bravo mediante un motor controlado por Arduino, proporcionando control automático de pitch y mantenimiento de velocidad vertical.

ESTE README ES UN TEMPLATE Y NO REPRESENTA EL ESTADO ACTUAL DEL PROYECTO. DE HECHO, HAY MUCHAS COSAS MAL EN EL README

## ✨ Características

### Integración con MSFS 2020
- **Conexión FSUIPC**: Lectura en tiempo real de datos de trim del simulador
- **Sincronización Continua**: Monitoreo constante del estado del avión
- **Bajo Latencia**: Respuesta rápida a cambios en las condiciones de vuelo

### Control por Teclado
- **Hook Global**: Captura de combinaciones de teclas a nivel de sistema
- **Activación/Desactivación**: Control rápido del sistema sin salir del simulador
- **Ajuste de VS**: Incremento/decremento de velocidad vertical en pasos de 100 fpm
- **Mantener VS**: Fija y mantiene la velocidad vertical actual
- **Nivelación Automática**: Ajusta el trim para mantener 0 fpm

### Controlador PID
- **Control Preciso**: Algoritmo PID optimizado para ajuste suave de trim
- **Parámetros Configurables**: Ajuste fino de constantes P, I y D
- **Respuesta Adaptativa**: Compensación automática según condiciones de vuelo

### Hardware Arduino
- **Comunicación Serie USB**: Protocolo eficiente para envío de comandos
- **Control de Motor**: Ajuste físico de la rueda de trim del Honeycomb Bravo
- **Feedback en Tiempo Real**: Confirmación de posición y estado

## 🔧 Instalación

```bash
# Clonar el repositorio
git clone https://github.com/tuusuario/autotrim.git
cd autotrim

# Compilar el proyecto (Windows)
nmake

# Ejecutable generado:
# AutoTrim.exe - Programa principal
```

## 🖥️ Requisitos del Sistema

### Software
- **Windows 10/11 (64-bit)**
- **Microsoft Flight Simulator 2020**
- **FSUIPC7**
- **Drivers Arduino**

### Hardware
- **Honeycomb Bravo Throttle Quadrant**
- **Arduino**
- **Motor Paso a Paso o Servo**
- **Driver de Motor**
- **Fuente de Alimentación**

## 🎮 Uso del Programa

### Inicio Básico

```bash
# Ejecutar AutoTrim como administrador
AutoTrim.exe

# El programa se conectará automáticamente a:
# - FSUIPC (MSFS 2020 debe estar ejecutándose)
# - Puerto serie Arduino (detección automática)
```

### Combinaciones de Teclas

| Combinación | Acción |
|------------|--------|
| **Ctrl+Alt+T** | Activar/Desactivar AutoTrim |
| **Ctrl+Alt+↑** | Incrementar VS objetivo en 100 fpm |
| **Ctrl+Alt+↓** | Decrementar VS objetivo en 100 fpm |
| **Ctrl+Alt+H** | Mantener VS actual |
| **Ctrl+Alt+L** | Nivelar avión (VS = 0 fpm) |

*Nota: Las combinaciones de teclas son configurables en el archivo de configuración*

### Flujo de Trabajo Típico

```
1. Iniciar Microsoft Flight Simulator 2020
2. Ejecutar AutoTrim.exe
3. Esperar confirmación de conexión con FSUIPC y Arduino
4. Activar el sistema con Ctrl+Alt+T
5. El sistema comenzará a ajustar el trim automáticamente
6. Usar combinaciones de teclas para control manual cuando sea necesario
```

## ⚙️ Archivo de Configuración

El archivo de configuración `config.ini` permite personalizar el comportamiento del sistema:

### Parámetros Disponibles

#### Configuración del Controlador PID

| Parámetro | Descripción | Rango | Por Defecto |
|-----------|-------------|-------|-------------|
| **Kp** | Constante proporcional | 0.0 - 10.0 | 1.5 |
| **Ki** | Constante integral | 0.0 - 5.0 | 0.3 |
| **Kd** | Constante derivativa | 0.0 - 2.0 | 0.1 |
| **update_rate** | Frecuencia de actualización (Hz) | 1 - 100 | 20 |
| **max_trim_rate** | Velocidad máxima de cambio de trim | 1 - 100 | 50 |

#### Configuración de Velocidad Vertical

| Parámetro | Descripción | Rango | Por Defecto |
|-----------|-------------|-------|-------------|
| **vs_increment** | Paso de ajuste de VS (fpm) | 50 - 500 | 100 |
| **vs_deadband** | Zona muerta de VS (fpm) | 0 - 50 | 10 |
| **vs_max** | VS máximo permitido (fpm) | 1000 - 5000 | 2000 |

#### Configuración de Arduino

| Parámetro | Descripción | Valores | Por Defecto |
|-----------|-------------|---------|-------------|
| **port** | Puerto COM del Arduino | COM1-COM99/AUTO | AUTO |
| **baudrate** | Velocidad de comunicación | 9600-115200 | 9600 |
| **timeout** | Timeout de comunicación (ms) | 100-5000 | 1000 |

#### Configuración de Teclas

| Parámetro | Descripción | Formato | Por Defecto |
|-----------|-------------|---------|-------------|
| **toggle_key** | Activar/Desactivar | Ctrl+Alt+Key | Ctrl+Alt+T |
| **vs_up_key** | Incrementar VS | Ctrl+Alt+Key | Ctrl+Alt+Up |
| **vs_down_key** | Decrementar VS | Ctrl+Alt+Key | Ctrl+Alt+Down |
| **hold_vs_key** | Mantener VS actual | Ctrl+Alt+Key | Ctrl+Alt+H |
| **level_key** | Nivelar avión | Ctrl+Alt+Key | Ctrl+Alt+L |

### Ejemplo de Archivo de Configuración

```ini
# Configuración del Controlador PID
[PID]
Kp=1.5
Ki=0.3
Kd=0.1
update_rate=20
max_trim_rate=50

# Configuración de Velocidad Vertical
[VerticalSpeed]
vs_increment=100
vs_deadband=10
vs_max=2000
vs_min=-2000

# Configuración de Arduino
[Arduino]
port=AUTO
baudrate=9600
timeout=1000
motor_steps_per_rev=200
trim_wheel_ratio=1.0

# Configuración de Teclas (Virtual Key Codes)
[Hotkeys]
toggle_key=0x54        # T
vs_up_key=0x26         # Up Arrow
vs_down_key=0x28       # Down Arrow
hold_vs_key=0x48       # H
level_key=0x4C         # L
modifiers=MOD_CONTROL|MOD_ALT

# Configuración de FSUIPC
[FSUIPC]
connection_timeout=5000
retry_interval=1000
offsets_update_rate=50
```

## 🔌 Configuración del Arduino

### Esquema de Conexión

```
Arduino Uno/Mega
├─ Pin 2  → Motor Driver IN1
├─ Pin 3  → Motor Driver IN2
├─ Pin 4  → Motor Driver IN3
├─ Pin 5  → Motor Driver IN4
├─ Pin 9  → Motor Driver Enable (PWM)
├─ 5V     → Motor Driver VCC
└─ GND    → Motor Driver GND + Motor Power GND

Motor Driver (L298N/TB6612)
├─ OUT1/OUT2 → Motor Coil A
├─ OUT3/OUT4 → Motor Coil B
└─ Motor Power → External Power Supply (9-12V)

Honeycomb Bravo
└─ Trim Wheel → Acoplamiento mecánico con motor
```

### Código Arduino

El código para el Arduino está incluido en la carpeta `arduino/`:

```cpp
// Sketch básico incluido en arduino/AutoTrim_Motor_Controller.ino
// - Recibe comandos por Serial
// - Controla motor paso a paso
// - Envía feedback de posición
```

Para cargar el código:
1. Abrir `arduino/AutoTrim_Motor_Controller.ino` en Arduino IDE
2. Seleccionar placa y puerto correctos
3. Subir sketch al Arduino

### Protocolo de Comunicación

El protocolo serie entre PC y Arduino utiliza comandos ASCII:

| Comando | Formato | Descripción | Respuesta |
|---------|---------|-------------|-----------|
| **MOVE** | `M[±]XXXX\n` | Mover motor X pasos | `OK\n` o `ERR\n` |
| **POS** | `P?\n` | Consultar posición actual | `P[±]XXXX\n` |
| **STOP** | `S\n` | Detener motor inmediatamente | `OK\n` |
| **RESET** | `R\n` | Reiniciar posición a 0 | `OK\n` |
| **STATUS** | `?\n` | Estado del sistema | `READY\n` o `BUSY\n` |

Ejemplos:
```
M+0150\n  → Mover 150 pasos adelante
M-0075\n  → Mover 75 pasos atrás
P?\n      → ¿Cuál es la posición? → Respuesta: P+0225\n
```

## 🧪 Calibración y Ajuste

### Calibración Inicial del Motor

```bash
# 1. Ejecutar modo de calibración
AutoTrim.exe --calibrate

# 2. Seguir instrucciones en pantalla:
#    - Mover trim manualmente a posición central
#    - Presionar Enter para establecer punto cero
#    - Mover trim a extremo superior
#    - Presionar Enter para registrar máximo
#    - Mover trim a extremo inferior
#    - Presionar Enter para registrar mínimo

# Los valores se guardarán automáticamente en config.ini
```

### Ajuste del Controlador PID

Para optimizar el comportamiento del controlador PID:

1. **Empezar con valores conservadores**:
   - Kp = 1.0, Ki = 0.0, Kd = 0.0

2. **Ajustar Kp (Proporcional)**:
   - Incrementar hasta que el sistema responda rápidamente
   - Si oscila, reducir ligeramente

3. **Añadir Ki (Integral)**:
   - Incrementar para eliminar error en estado estacionario
   - Valores típicos: 0.1 - 0.5

4. **Añadir Kd (Derivativo)**:
   - Incrementar para reducir sobreimpulso
   - Valores típicos: 0.05 - 0.2

### Pruebas de Vuelo Recomendadas

```bash
# Escenario 1: Crucero estable
# - Activar AutoTrim en vuelo nivelado
# - Observar estabilidad sin intervención

# Escenario 2: Cambios de velocidad
# - Cambiar potencia gradualmente
# - Verificar ajuste automático de trim

# Escenario 3: Cambios de configuración
# - Extender/retraer flaps
# - Verificar compensación correcta

# Escenario 4: Velocidades verticales
# - Establecer VS de +500 fpm
# - Verificar mantenimiento de VS
# - Cambiar a -500 fpm
# - Repetir para diferentes VS
```

## 📊 Monitoreo y Logs

### Información en Tiempo Real

El programa muestra información en consola:

```
AutoTrim v1.0 - Iniciado
[12:34:56] INFO: Conectado a FSUIPC
[12:34:57] INFO: Arduino detectado en COM3
[12:34:58] INFO: Sistema listo

[ESTADO]
  Modo: ACTIVO
  VS Actual: +450 fpm
  VS Objetivo: +500 fpm
  Trim Actual: 2.5%
  Salida PID: +0.15
  Posición Motor: +125 pasos

[PID]
  P: +0.075  I: +0.045  D: +0.030
  Error: +50 fpm
```

### Archivo de Log

Los logs detallados se guardan en `logs/autotrim.log`:

```
2024-10-01 12:34:56 [INFO] Sistema iniciado
2024-10-01 12:34:56 [INFO] FSUIPC: Conexión establecida
2024-10-01 12:34:57 [INFO] Arduino: Detectado en COM3 @ 9600 baud
2024-10-01 12:34:58 [INFO] Calibración cargada: rango ±500 pasos
2024-10-01 12:35:10 [INFO] AutoTrim activado por usuario
2024-10-01 12:35:11 [DEBUG] VS: actual=+125, objetivo=0, error=+125
2024-10-01 12:35:11 [DEBUG] PID: P=0.125, I=0.038, D=0.015, out=0.178
2024-10-01 12:35:11 [INFO] Motor: comando M-0015 enviado
2024-10-01 12:35:12 [INFO] Motor: respuesta OK, pos=-015
```

## 🛠️ Arquitectura Técnica

### Componentes Principales

#### Módulo FSUIPC
- **Conexión**: Interfaz con FSUIPC SDK
- **Offsets**: Lectura de trim, VS, actitud, etc.
- **Frecuencia**: Actualización configurable (típicamente 50 Hz)
- **Gestión de Errores**: Reconexión automática

#### Hook de Teclado
- **Nivel**: Hook global de Windows (WH_KEYBOARD_LL)
- **Filtrado**: Detección de combinaciones específicas
- **Thread Safety**: Cola de eventos thread-safe
- **Latencia**: <5ms de respuesta

#### Controlador PID
- **Algoritmo**: PID discreto con anti-windup
- **Variables**:
  - Entrada: Error de VS (VS objetivo - VS actual)
  - Salida: Cambio de trim requerido
- **Saturación**: Límites configurables
- **Reset Integral**: Al cambiar de modo

#### Comunicación Serie
- **Protocolo**: ASCII con terminación \n
- **Buffer**: Ring buffer para recepción
- **Timeout**: Detección de pérdida de conexión
- **Retry**: Reintento automático en caso de fallo

### Flujo de Datos

```
MSFS 2020
    ↓ (FSUIPC)
Lectura de Datos (VS, Trim, etc.)
    ↓
Cálculo de Error (VS objetivo - VS actual)
    ↓
Controlador PID
    ↓
Cálculo de Ajuste de Trim
    ↓
Protocolo Serie
    ↓
Arduino
    ↓
Control de Motor
    ↓
Movimiento Físico de Trim Wheel
    ↓
Honeycomb Bravo
    ↓
MSFS 2020 (cierra el bucle)
```

### Gestión de Threads

- **Thread Principal**: Interfaz de usuario y log
- **Thread FSUIPC**: Lectura continua de datos del simulador
- **Thread PID**: Cálculo y envío de comandos al Arduino
- **Thread Serie**: Gestión de comunicación con Arduino
- **Thread Keyboard**: Hook de teclado global

## 🔧 Solución de Problemas

### El programa no se conecta a FSUIPC

```
Error: No se puede conectar a FSUIPC
Solución:
1. Verificar que MSFS 2020 está ejecutándose
2. Confirmar que FSUIPC7 está instalado
3. Verificar que FSUIPC está habilitado en configuración
4. Ejecutar AutoTrim como administrador
```

### Arduino no responde

```
Error: Arduino no detectado o no responde
Solución:
1. Verificar conexión USB
2. Comprobar que el sketch está cargado correctamente
3. Verificar baudrate en config.ini coincide con Arduino
4. Revisar que el puerto COM es correcto
5. Probar con otro cable USB
```

### El trim oscila continuamente

```
Problema: El trim se mueve constantemente arriba y abajo
Solución:
1. Reducir Kp en config.ini (ejemplo: de 1.5 a 1.0)
2. Reducir Kd en config.ini
3. Incrementar vs_deadband (zona muerta)
4. Verificar que el motor no tiene holgura mecánica
```

### Respuesta lenta del sistema

```
Problema: El trim tarda mucho en ajustarse
Solución:
1. Incrementar Kp en config.ini
2. Incrementar update_rate (cuidado con sobrecarga)
3. Incrementar max_trim_rate
4. Verificar latencia de conexión con FSUIPC
```

### El motor hace ruido pero no se mueve

```
Problema: El motor vibra pero no gira
Solución:
1. Verificar conexiones del motor al driver
2. Comprobar alimentación externa del motor (9-12V)
3. Verificar secuencia de pasos en código Arduino
4. Probar con motor desacoplado del trim wheel
5. Incrementar corriente del motor (ajuste en driver)
```

## 📚 Preguntas Frecuentes

**P: ¿Funciona con todos los aviones de MSFS 2020?**
R: Sí, funciona con cualquier avión que tenga trim de pitch. Algunos aviones pueden requerir ajustes en los parámetros PID.

**P: ¿Puedo usar otro controlador en lugar de Arduino?**
R: Sí, cualquier microcontrolador que soporte comunicación serie puede usarse modificando el protocolo.

**P: ¿Afecta al trim manual del Honeycomb Bravo?**
R: No, el sistema detecta movimientos manuales y se adapta. Puedes desactivar AutoTrim en cualquier momento.

**P: ¿Funciona en modo multijugador?**
R: Sí, funciona perfectamente en sesiones multijugador y compartidas.

**P: ¿Cuánta latencia introduce el sistema?**
R: La latencia total es típicamente <100ms, imperceptible durante el vuelo normal.

**P: ¿Necesito programación para usarlo?**
R: No para uso básico. Solo necesitas cargar el sketch en Arduino y ajustar config.ini.

## 🔄 Actualizaciones Futuras

- [ ] Interfaz gráfica de configuración
- [ ] Perfiles de configuración por avión
- [ ] Integración con SimConnect (alternativa a FSUIPC)
- [ ] Modo de aprendizaje automático de parámetros PID
- [ ] Telemetría y estadísticas de vuelo

- https://youtu.be/yjuD2yaNzro?t=10

## 📄 Licencia

Este proyecto está licenciado bajo la WTFPL – [Do What the Fuck You Want to Public License](http://www.wtfpl.net/about/).

---

<div align="center">

**🛩️ Desarrollado como proyecto personal 🛩️**

*"Even virtual pilots deserve a break"*

</div>
