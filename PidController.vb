
Public Class ComplexPIDController

    Private PIDVelocity As PIDControllerS
    Private PIDRateOfChange As PIDControllerS
    Private _velocities As New Queue(Of Double)(2)
    Private _setPoint As Double

    Public Sub New(velocitySetPoint As Double)
        _setPoint = velocitySetPoint
        ' Usar valores por defecto si no se pasan parámetros
        PIDVelocity = New PIDControllerS(0.02, 0.00005, 3)
        PIDRateOfChange = New PIDControllerS(0, 0, 0.000000)
        PIDVelocity.SetPoint = _setPoint
        PIDRateOfChange.SetPoint = 0
    End Sub

    Public Sub New(velocitySetPoint As Double, kp As Double, ki As Double, kd As Double)
        _setPoint = velocitySetPoint
        ' Usar valores de configuración
        PIDVelocity = New PIDControllerS(kp, ki, kd)
        PIDRateOfChange = New PIDControllerS(0, 0, 0.000000)
        PIDVelocity.SetPoint = _setPoint
        PIDRateOfChange.SetPoint = 0
    End Sub

    Public Function Compute(velocidadVerticalActual As Double) As Double
        If (_velocities.Count >= 2) Then _velocities.Dequeue()
        _velocities.Enqueue(velocidadVerticalActual)

        If (_velocities.Count < 2) Then Return (0)

        Dim velocitiesArray = _velocities.ToArray()
        Dim velocityError = velocitiesArray(1) - velocitiesArray(0)

        ' Primero controlamos la velocidad vertical, acercándonos a la velocidad deseada
        Dim velocityControlOutput = PIDVelocity.Compute(velocidadVerticalActual)

        ' Luego controlamos la tasa de cambio, asegurándonos de que sea cero en la velocidad deseada
        Dim rateOfChangeControlOutput = PIDRateOfChange.Compute(velocityError)

        ' Ponderamos los dos controladores según la proximidad a la velocidad deseada
        Dim proximityToSetPoint As Double
        Dim safeSetPoint = If(_setPoint = 0, 1, _setPoint)

        proximityToSetPoint = Math.Abs(velocidadVerticalActual - safeSetPoint) / safeSetPoint

        Dim output = proximityToSetPoint * velocityControlOutput + (1 - proximityToSetPoint) * rateOfChangeControlOutput

        Return (output)
    End Function

End Class

Public Class PIDControllerS

    Private _Kp As Double
    Private _Ki As Double
    Private _Kd As Double
    Private _currentError As Double
    Private _previousError As Double
    Private _integral As Double
    Private _derivative As Double
    Private _setPoint As Double

    Public Sub New(Kp As Double, Ki As Double, Kd As Double)
        _Kp = Kp
        _Ki = Ki
        _Kd = Kd
    End Sub

    Public Property SetPoint As Double
        Get
            Return (_setPoint)
        End Get
        Set(value As Double)
            _setPoint = value
        End Set
    End Property

    Public Function Compute(actualValue As Double) As Double
        _currentError = _setPoint - actualValue
        _integral += _currentError
        _derivative = _currentError - _previousError
        Dim output = _Kp * _currentError + _Ki * _integral + _Kd * _derivative
        _previousError = _currentError

        Return (output)
    End Function

End Class

Public Class PIDController2

    Private _Kp As Double
    Private _Ki As Double
    Private _Kd As Double
    Private _setPoint As Double
    Private _error As Double
    Private _previousError As Double
    Private _integral As Double
    Private _derivative As Double
    Private _velocities As New Queue(Of Double)(2)

    Public Property SetPoint As Double
        Get
            Return (_setPoint)
        End Get
        Set(value As Double)
            _setPoint = value
        End Set
    End Property

    Public Sub New(Kp As Double, Ki As Double, Kd As Double)
        _Kp = Kp : _Ki = Ki : _Kd = Kd
    End Sub

    Public Function Compute(velocidadVerticalActual As Double) As Double
        If (_velocities.Count >= 2) Then _velocities.Dequeue()
        _velocities.Enqueue(velocidadVerticalActual)

        If (_velocities.Count < 2) Then Return (0)

        Dim velocitiesArray = _velocities.ToArray()
        _error = (velocitiesArray(1) - velocitiesArray(0)) + 0.01 * (velocidadVerticalActual - _setPoint)

        _integral += _error
        _derivative = _error - _previousError

        Dim output = _Kp * _error + _Ki * _integral + _Kd * _derivative
        _previousError = _error

        Return (output)
    End Function
End Class

Public Class PIDController

    Public Property SetPoint As Double
    Public Property ProcessVariable As Double
    Public Property Kp As Double
    Public Property Ki As Double
    Public Property Kd As Double
    Public popo As Boolean

    Private _lastProportional As Double
    Private _integral As Double

    Private _errorList As New List(Of Double)
    Private _maxErrorListSize As Integer = 50

    Public Sub New(Kp As Double, Ki As Double, Kd As Double)
        Me.Kp = Kp : Me.Ki = Ki : Me.Kd = Kd
    End Sub

    Public ReadOnly Property ControlOutput As Double
        Get
            Dim proportional = SetPoint - ProcessVariable : _integral += proportional
            Dim derivative = proportional - _lastProportional : _lastProportional = proportional

            If (popo = False) Then

                ' Almacenar error
                _errorList.Add(Math.Abs(proportional))
                ' Limitar tamaño de lista de error
                If (_errorList.Count > _maxErrorListSize) Then _errorList.RemoveAt(0)

                ' Ajustar Kd en función de la desviación estándar de los errores recientes
                Dim stdDev As Double = CalculateStdDev(_errorList)
                If (Not Double.IsNaN(stdDev)) Then
                    Dim deltaKd As Double = stdDev - Kd
                    Dim maxDeltaKd As Double = Kd * 0.01 ' permitir un cambio máximo del 0.1% por ciclo (0.1 = 10%)
                    If (Math.Abs(deltaKd) > maxDeltaKd) Then deltaKd = Math.Sign(deltaKd) * maxDeltaKd ' limitar el cambio a maxDeltaKd

                    Kd += deltaKd

                    ' Limitar Kd a un rango específico
                    Dim lowerLimit As Double = 0.01 ' define tu límite inferior
                    Dim upperLimit As Double = 8 ' define tu límite superior
                    If (Kd < lowerLimit) Then Kd = lowerLimit
                    If (Kd > upperLimit) Then Kd = upperLimit
                End If

            End If

            Return (Kp * proportional + Ki * _integral + Kd * derivative)
        End Get
    End Property

    Private Function CalculateStdDev(values As List(Of Double)) As Double
        Dim ret As Double = 0

        If (values.Count > 1) Then
            Dim avg As Double = values.Average()
            Dim sum As Double = values.Sum(Function(d) Math.Pow(d - avg, 2))
            If (sum > 0) Then ret = Math.Sqrt((sum) / (values.Count - 1))
        End If

        Return (ret)
    End Function

End Class
