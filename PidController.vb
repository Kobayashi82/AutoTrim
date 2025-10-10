
Imports System.Threading

Public Class PIDController
    ' Parámetros del PID
    Private Kp As Double
    Private Ki As Double
    Private Kd As Double

    ' Estado interno del PID
    Private integralAccumulated As Double = 0
    Private previousVS As Double = 0
    Private previousTime As DateTime = DateTime.Now

    ' Configuración del controlador
    Private referenceSpeed As Double ' Velocidad de referencia en knots
    Private maxStepsPerCycle As Integer
    Private deadband As Double ' FPM
    Private trimMargin As Double ' Margen para límites de trim
    Private speedScalingExponent As Double = 1.5 ' Exponente para escalar por velocidad

    ' Límites del simulador
    Private ReadOnly MIN_TRIM As Double = -1600
    Private ReadOnly MAX_TRIM As Double = 1600
    Private ReadOnly MOTOR_SPEED As Integer = 30

    ' Anti-windup - detección de saturación
    Private saturationCounter As Integer = 0
    Private ReadOnly SATURATION_THRESHOLD As Integer = 5 ' Ciclos para detectar saturación
    Private previousError As Double = 0

    ' Referencias externas
    Private Motor As Object
    Private FSUIPCMgr As Object

    Public Sub New(motorInstance As Object, fsuipcInstance As Object)
        Motor = motorInstance
        FSUIPCMgr = fsuipcInstance

        LoadDefaultProfile()
    End Sub

    Public Sub LoadProfile(profileType As AircraftProfile)
        Select Case profileType
            Case AircraftProfile.LightSingleEngine
                Kp = 0.08
                Ki = 0.01
                Kd = 0.15
                referenceSpeed = 110
                maxStepsPerCycle = 40
                deadband = 50
                trimMargin = 150

            Case AircraftProfile.LightTwinEngine
                Kp = 0.06
                Ki = 0.008
                Kd = 0.12
                referenceSpeed = 150
                maxStepsPerCycle = 50
                deadband = 60
                trimMargin = 150

            Case AircraftProfile.RegionalTurboprop
                Kp = 0.05
                Ki = 0.006
                Kd = 0.1
                referenceSpeed = 200
                maxStepsPerCycle = 60
                deadband = 70
                trimMargin = 200

            Case AircraftProfile.CommercialJetSmall
                Kp = 0.04
                Ki = 0.005
                Kd = 0.08
                referenceSpeed = 250
                maxStepsPerCycle = 70
                deadband = 80
                trimMargin = 200

            Case AircraftProfile.CommercialJetLarge
                Kp = 0.03
                Ki = 0.004
                Kd = 0.06
                referenceSpeed = 280
                maxStepsPerCycle = 80
                deadband = 100
                trimMargin = 250

            Case AircraftProfile.Fighter
                Kp = 0.1
                Ki = 0.012
                Kd = 0.2
                referenceSpeed = 300
                maxStepsPerCycle = 100
                deadband = 100
                trimMargin = 200
        End Select

        ' Resetear estado al cambiar perfil
        ResetController()
    End Sub

    Private Sub LoadDefaultProfile()
        LoadProfile(AircraftProfile.CommercialJetSmall)
    End Sub

    Public Sub SetCustomParameters(kpValue As Double, kiValue As Double, kdValue As Double,
                                   refSpeed As Double, maxSteps As Integer,
                                   deadbandFpm As Double, margin As Double)
        Kp = kpValue
        Ki = kiValue
        Kd = kdValue
        referenceSpeed = refSpeed
        maxStepsPerCycle = maxSteps
        deadband = deadbandFpm
        trimMargin = margin
    End Sub

    Public Sub Update(targetVS As Double)
        ' Leer valores del simulador
        Dim currentVS As Double = FSUIPCMgr.VerticalSpeed ' Asume que está en FPM
        Dim currentTrim As Double = FSUIPCMgr.ElevatorTrim ' Valor entre -1600 y 1600
        Dim currentSpeed As Double = FSUIPCMgr.IndicatedAirspeed ' En knots

        ' Calcular tiempo transcurrido
        Dim currentTime As DateTime = DateTime.Now
        Dim deltaTime As Double = (currentTime - previousTime).TotalSeconds

        ' Evitar divisiones por cero o valores anormales
        If deltaTime <= 0 Or deltaTime > 1.0 Then
            previousTime = currentTime
            Return
        End If

        ' Calcular error
        Dim errorVS As Double = targetVS - currentVS

        ' Deadband - no actuar si estamos muy cerca del target
        If Math.Abs(errorVS) < deadband Then
            ' Opcionalmente congelar integrador en deadband
            ' integralAccumulated *= 0.9 ' Decaimiento suave
            previousTime = currentTime
            previousVS = currentVS
            Return
        End If

        ' === TÉRMINO PROPORCIONAL ===
        Dim pTerm As Double = Kp * errorVS

        ' === TÉRMINO INTEGRAL con Anti-Windup ===
        ' Solo acumular si no estamos en los límites o saturados
        Dim canIntegrate As Boolean = True

        ' Check límites del simulador
        If (currentTrim >= MAX_TRIM - trimMargin And pTerm > 0) Or
           (currentTrim <= MIN_TRIM + trimMargin And pTerm < 0) Then
            canIntegrate = False
            ' Reducir integrador existente (back-calculation)
            integralAccumulated *= 0.8
        End If

        ' Detectar saturación aerodinámica
        If DetectAeroSaturation(errorVS, currentVS) Then
            canIntegrate = False
            integralAccumulated *= 0.9
        End If

        If canIntegrate Then
            integralAccumulated += errorVS * deltaTime

            ' Limitar el integrador para evitar windup excesivo
            Dim maxIntegral As Double = 5000 ' Ajustar según necesidad
            integralAccumulated = Math.Max(-maxIntegral, Math.Min(maxIntegral, integralAccumulated))
        End If

        Dim iTerm As Double = Ki * integralAccumulated

        ' === TÉRMINO DERIVATIVO ===
        ' Calcular solo del VS, no del error (evita derivative kick)
        Dim vsRate As Double = (currentVS - previousVS) / deltaTime
        Dim dTerm As Double = -Kd * vsRate ' Negativo porque queremos frenar el cambio

        ' === SALIDA BASE DEL PID ===
        Dim pidOutput As Double = pTerm + iTerm + dTerm

        ' === ESCALADO POR VELOCIDAD ===
        Dim speedFactor As Double = 1.0
        If currentSpeed > 10 Then ' Evitar división por cero en tierra
            speedFactor = Math.Pow(referenceSpeed / currentSpeed, speedScalingExponent)
        End If

        Dim stepsToMove As Double = pidOutput * speedFactor

        ' === RATE LIMITING ===
        If Math.Abs(stepsToMove) > maxStepsPerCycle Then
            stepsToMove = Math.Sign(stepsToMove) * maxStepsPerCycle
        End If

        ' Redondear a entero
        Dim finalSteps As Integer = CInt(Math.Round(stepsToMove))

        ' === ENVIAR COMANDO AL MOTOR ===
        If finalSteps <> 0 Then
            If finalSteps > 0 Then
                Motor.TrimUp(Math.Abs(finalSteps), MOTOR_SPEED)
            Else
                Motor.TrimDown(Math.Abs(finalSteps), MOTOR_SPEED)
            End If
        End If

        ' Actualizar estado para próximo ciclo
        previousVS = currentVS
        previousError = errorVS
        previousTime = currentTime
    End Sub

    Private Function DetectAeroSaturation(currentError As Double, currentVS As Double) As Boolean
        ' Si el error mantiene el mismo signo y magnitud significativa
        ' pero el VS no está cambiando hacia el target, hay saturación
        If Math.Sign(currentError) = Math.Sign(previousError) And
           Math.Abs(currentError) > deadband * 2 Then

            ' Check si VS está cambiando en la dirección correcta
            Dim vsChange As Double = currentVS - previousVS
            Dim expectedDirection As Double = Math.Sign(currentError)

            ' Si no hay cambio significativo o va en dirección contraria
            If Math.Abs(vsChange) < 20 Or Math.Sign(vsChange) <> expectedDirection Then
                saturationCounter += 1
            Else
                saturationCounter = 0
            End If
        Else
            saturationCounter = 0
        End If

        Return saturationCounter >= SATURATION_THRESHOLD
    End Function

    Public Sub ResetController()
        integralAccumulated = 0
        previousVS = 0
        saturationCounter = 0
        previousError = 0
        previousTime = DateTime.Now
    End Sub

    Public Function GetControllerState() As ControllerState
        Return New ControllerState With {
            .IntegralValue = integralAccumulated,
            .SaturationCounter = saturationCounter,
            .LastError = previousError,
            .Kp = Me.Kp,
            .Ki = Me.Ki,
            .Kd = Me.Kd
        }
    End Function
End Class

' === ENUMERACIONES Y ESTRUCTURAS ===

Public Enum AircraftProfile
    LightSingleEngine ' Cessna 172, Piper, etc
    LightTwinEngine ' Baron, Seminole, etc
    RegionalTurboprop ' Dash-8, ATR, etc
    CommercialJetSmall ' A320, 737, etc
    CommercialJetLarge ' 777, A350, etc
    Fighter ' F-16, F-18, etc
End Enum

Public Structure ControllerState
    Public IntegralValue As Double
    Public SaturationCounter As Integer
    Public LastError As Double
    Public Kp As Double
    Public Ki As Double
    Public Kd As Double
End Structure
