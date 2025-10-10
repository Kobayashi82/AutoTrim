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
    Private referenceSpeed As Double
    Private minSpeed As Integer = 200 ' Velocidad mínima del motor
    Private maxSpeed As Integer = 700 ' Velocidad máxima del motor
    Private deadband As Double
    Private trimMargin As Double
    Private speedScalingExponent As Double = 1.5

    ' Límites del simulador
    Private ReadOnly MIN_TRIM As Double = -16383
    Private ReadOnly MAX_TRIM As Double = 16383

    ' Anti-windup
    Private saturationCounter As Integer = 0
    Private ReadOnly SATURATION_THRESHOLD As Integer = 5
    Private previousError As Double = 0

    ' Control de movimiento continuo
    Private currentDirection As Integer = 0 ' -1 = down, 0 = stopped, 1 = up
    Private currentMotorSpeed As Integer = 0

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
                Kp = 0.1
                Ki = 0.008
                Kd = 0.8
                referenceSpeed = 110
                deadband = 150
                trimMargin = 150

            Case AircraftProfile.LightTwinEngine
                Kp = 0.12
                Ki = 0.008
                Kd = 0.35
                referenceSpeed = 150
                deadband = 60
                trimMargin = 150

            Case AircraftProfile.RegionalTurboprop
                Kp = 0.1
                Ki = 0.006
                Kd = 0.3
                referenceSpeed = 200
                deadband = 70
                trimMargin = 200

            Case AircraftProfile.CommercialJetSmall
                Kp = 0.08
                Ki = 0.005
                Kd = 0.25
                referenceSpeed = 250
                deadband = 80
                trimMargin = 200

            Case AircraftProfile.CommercialJetLarge
                Kp = 0.06
                Ki = 0.004
                Kd = 0.2
                referenceSpeed = 280
                deadband = 100
                trimMargin = 250

            Case AircraftProfile.Fighter
                Kp = 0.18
                Ki = 0.012
                Kd = 0.5
                referenceSpeed = 300
                deadband = 100
                trimMargin = 200
        End Select

        ResetController()
    End Sub

    Private Sub LoadDefaultProfile()
        LoadProfile(AircraftProfile.CommercialJetSmall)
    End Sub

    Public Sub SetCustomParameters(kpValue As Double, kiValue As Double, kdValue As Double,
                                   refSpeed As Double, deadbandFpm As Double, margin As Double)
        Kp = kpValue
        Ki = kiValue
        Kd = kdValue
        referenceSpeed = refSpeed
        deadband = deadbandFpm
        trimMargin = margin
    End Sub

    Public Sub Update(targetVS As Double)
        ' Leer valores del simulador
        Dim currentVS As Double = FSUIPCMgr.VerticalSpeed
        Dim currentTrim As Double = FSUIPCMgr.ElevatorTrim
        Dim currentSpeed As Double = FSUIPCMgr.IndicatedAirspeed

        ' Calcular tiempo transcurrido
        Dim currentTime As DateTime = DateTime.Now
        Dim deltaTime As Double = (currentTime - previousTime).TotalSeconds

        If deltaTime <= 0 Or deltaTime > 1.0 Then
            previousTime = currentTime
            Return
        End If

        ' Calcular error
        Dim errorVS As Double = targetVS - currentVS

        ' Deadband - DETENER si estamos cerca del target
        If Math.Abs(errorVS) < deadband Then
            If currentDirection <> 0 Then
                Motor.DetenerMotor()
                currentDirection = 0
                currentMotorSpeed = 0
                FMenu.lbl_AP_Status.Text = "HOLD (deadband)"
            End If
            previousTime = currentTime
            previousVS = currentVS
            Return
        End If

        ' === TÉRMINO PROPORCIONAL ===
        Dim pTerm As Double = Kp * errorVS

        ' === TÉRMINO INTEGRAL con Anti-Windup ===
        Dim canIntegrate As Boolean = True

        ' Límites del simulador
        If (currentTrim >= MAX_TRIM - trimMargin And pTerm > 0) Or
           (currentTrim <= MIN_TRIM + trimMargin And pTerm < 0) Then
            canIntegrate = False
            integralAccumulated *= 0.8
        End If

        ' Saturación aerodinámica
        If DetectAeroSaturation(errorVS, currentVS) Then
            canIntegrate = False
            integralAccumulated *= 0.9
        End If

        If canIntegrate Then
            integralAccumulated += errorVS * deltaTime
            Dim maxIntegral As Double = 5000
            integralAccumulated = Math.Max(-maxIntegral, Math.Min(maxIntegral, integralAccumulated))
        End If

        Dim iTerm As Double = Ki * integralAccumulated

        ' === TÉRMINO DERIVATIVO ===
        Dim vsRate As Double = (currentVS - previousVS) / deltaTime
        Dim dTerm As Double = -Kd * vsRate

        ' === SALIDA DEL PID ===
        Dim pidOutput As Double = pTerm + iTerm + dTerm

        ' === ESCALADO POR VELOCIDAD ===
        Dim speedFactor As Double = 1.0
        If currentSpeed > 10 Then
            speedFactor = Math.Pow(referenceSpeed / currentSpeed, speedScalingExponent)
        End If

        Dim adjustedOutput As Double = pidOutput * speedFactor

        ' === CONVERTIR A DIRECCIÓN Y VELOCIDAD DEL MOTOR ===
        Dim desiredDirection As Integer = Math.Sign(adjustedOutput)

        ' Mapear la magnitud de la salida a velocidad del motor (invertida: menor número = más rápido)
        Dim outputMagnitude As Double = Math.Abs(adjustedOutput)

        ' Zona de desaceleración progresiva cerca del target
        Dim errorMagnitude As Double = Math.Abs(errorVS)

        Dim desiredSpeed As Integer
        If errorMagnitude > 500 Then
            ' Lejos del target - movimiento rápido
            desiredSpeed = minSpeed
        ElseIf errorMagnitude > 200 Then
            ' Zona media - empezar a desacelerar
            desiredSpeed = CInt(minSpeed + (maxSpeed - minSpeed) * 0.3)
        ElseIf errorMagnitude > 100 Then
            ' Cerca del target - movimiento lento
            desiredSpeed = CInt(minSpeed + (maxSpeed - minSpeed) * 0.6)
        Else
            ' Muy cerca - movimiento muy lento
            desiredSpeed = maxSpeed
        End If

        ' === ENVIAR COMANDO AL MOTOR SOLO SI CAMBIA ===
        If desiredDirection <> currentDirection Or Math.Abs(desiredSpeed - currentMotorSpeed) > 50 Then
            If desiredDirection > 0 Then
                Motor.TrimUp(0, desiredSpeed)
                currentDirection = 1
                currentMotorSpeed = desiredSpeed
                FMenu.lbl_AP_Status.Text = $"↑ CONTINUO (speed: {desiredSpeed})"
            ElseIf desiredDirection < 0 Then
                Motor.TrimDown(0, desiredSpeed)
                currentDirection = -1
                currentMotorSpeed = desiredSpeed
                FMenu.lbl_AP_Status.Text = $"↓ CONTINUO (speed: {desiredSpeed})"
            Else
                Motor.DetenerMotor()
                currentDirection = 0
                currentMotorSpeed = 0
                FMenu.lbl_AP_Status.Text = "HOLD"
            End If
        End If

        ' Actualizar estado
        previousVS = currentVS
        previousError = errorVS
        previousTime = currentTime
    End Sub

    Private Function DetectAeroSaturation(currentError As Double, currentVS As Double) As Boolean
        If Math.Sign(currentError) = Math.Sign(previousError) And
           Math.Abs(currentError) > deadband * 2 Then

            Dim vsChange As Double = currentVS - previousVS
            Dim expectedDirection As Double = Math.Sign(currentError)

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
        currentDirection = 0
        currentMotorSpeed = 0
        Motor.DetenerMotor()
    End Sub

    Public Function GetControllerState() As ControllerState
        Return New ControllerState With {
            .IntegralValue = integralAccumulated,
            .SaturationCounter = saturationCounter,
            .LastError = previousError,
            .CurrentDirection = currentDirection,
            .CurrentMotorSpeed = currentMotorSpeed,
            .Kp = Me.Kp,
            .Ki = Me.Ki,
            .Kd = Me.Kd
        }
    End Function
End Class

Public Enum AircraftProfile
    LightSingleEngine
    LightTwinEngine
    RegionalTurboprop
    CommercialJetSmall
    CommercialJetLarge
    Fighter
End Enum

Public Structure ControllerState
    Public IntegralValue As Double
    Public SaturationCounter As Integer
    Public LastError As Double
    Public CurrentDirection As Integer
    Public CurrentMotorSpeed As Integer
    Public Kp As Double
    Public Ki As Double
    Public Kd As Double
End Structure