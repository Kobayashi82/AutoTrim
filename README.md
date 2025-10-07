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

Ejemplo de trim automático en un [Boeing 737](https://youtu.be/yjuD2yaNzro)

## ✨ Características

### Configuración Flexible
- **Configuración Global**: Permite ajustar parámetros del controlador PID, velocidad vertical, y comunicación con Arduino
- **Administración de Perfiles**: Guardar y cargar diferentes configuraciones para distintos aviones
- **Modo Standalone**: Funciona sin necesidad de hardware

### Integración con MSFS 2020
- **Conexión FSUIPC**: Lectura en tiempo real de datos del simulador
- **Bajo Latencia**: Respuesta rápida a cambios en las condiciones de vuelo
- **Captura de Entrada**: Captura de combinaciones de teclas y botones a nivel del simulador

### Piloto Automático de Trim
- **Ajuste de VS**: Permite ajustar la velocidad vertical en pasos de 100 fpm (configurable)
- **Mantener VS**: Fija y mantiene la velocidad vertical actual
- **Nivelación Automática**: Fija y mantiene una velocidad vertical de 0 fpm

### Controlador PID
- **Control Preciso**: Algoritmo PID optimizado para ajuste suave de trim con anti-windup
- **Parámetros Configurables**: Ajuste fino de constantes P, I y D
- **Respuesta Adaptativa**: Compensación automática según condiciones de vuelo

### Hardware Arduino
- **Comunicación Serie USB**: Protocolo eficiente para envío de comandos
- **Control de Motor**: Ajuste contínuo o por pasos del motor con velocidad configurable
- **Curva de Aceleración**: Para movimientos suaves y precisos

## 🖥️ Requisitos del Sistema

### Software
- Windows 10/11 (64-bit)
- Drivers Arduino
- FSUIPC7
- Microsoft Flight Simulator 2020

### Hardware
- Honeycomb Bravo Throttle Quadrant
- Arduino Uno R3
- Nema 17 Step Motor (12V, 2A, 55N·cm bipolar, 1.81)
- TB6600 Controller (24V, 4A, microstepping hasta 1/32), configurado a 4A, 1/1
- Fuente de Alimentación (15V, 5A)
- Correa Dentada GT2 con poleas de 20 dientes

## 🔧 Instalación

- Descargar y extraer `AutoTrim` desde la sección [releases](https://github.com/Kobayashi82/AutoTrim/releases/)
- Asegurarse de tener instalado [FSUIPC7](https://www.fsuipc.com/) y los drivers de [Arduino](https://www.arduino.cc/en/software/)
- Cargar el sketch de Arduino desde la carpeta `Arduino/AutoTrim` usando Arduino IDE
- Ejecutar `AutoTrim.exe`

## ⚙️ Archivo de Configuración

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

## 🔌 Configuración del Arduino

### Esquema de Conexión

```
Arduino Uno R3
├─ Pin 8  → Controller ENA
├─ Pin 9  → Controller DIR
├─ Pin 10 → Controller PUL
└─ GND    → Controller GND

Controller (TB6600)
├─ ENA-   → Controller GND
├─ ENA+   → Arduino Pin 8
├─ DIR-   → Controller GND
├─ DIR+   → Arduino Pin 9
├─ PUL-   → Controller GND
├─ PUL+   → Arduino Pin 10
│
├─ B-     → Motor Coil B-
├─ B+     → Motor Coil B+
├─ A-     → Motor Coil A-
├─ A+     → Motor Coil A+
│
├─ GND  → Power Supply GND
└─ VCC  → Power Supply VCC (15V)

Honeycomb Bravo
└─ Trim Wheel → Acoplamiento con correas GT2 al motor
```

### Protocolo de Comunicación

El protocolo serie entre PC y Arduino utiliza comandos ASCII:

| Comando             | Formato              | Descripción                           |
|---------------------|----------------------|---------------------------------------|
| **Trim Up Cont.**   | `U:[speed]`          | Trim up continuamente a X velocidad   |
| **Trim Up Pasos**   | `US:[steps]:[speed]` | Trim up N pasos a X velocidad         |
| **Trim Down Cont.** | `D:[speed]`          | Trim down continuamente a X velocidad |
| **Trim Down Pasos** | `DS:[steps]:[speed]` | Trim down N pasos a X velocidad       |
| **Detener**         | `S`                  | Detener motor                         |

`[steps]` es un valor entero positivo.  
`[speed]` es un valor entre 100 y 700, y es el tiempo de espera usado entre pasos.

## 🧪 Controlador PID

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

## 📊 Información en Tiempo Real

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

## 🔧 Solución de Problemas

### El programa no se conecta a FSUIPC

```
Error: No se puede conectar a FSUIPC
Solución:
1. Verificar que MSFS 2020 está ejecutándose
2. Confirmar que FSUIPC7 está instalado y en ejecución
3. Ejecutar AutoTrim como administrador
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
6. Revisar micro-delays en código Arduino
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

## 📄 Licencia

Este proyecto está licenciado bajo la WTFPL – [Do What the Fuck You Want to Public License](http://www.wtfpl.net/about/).

---

<div align="center">

**🛩️ Desarrollado como proyecto personal 🛩️**

*"Even virtual pilots deserve a break"*

</div>
