
Public Class MotorController

#Region " Variables "

    Private WithEvents SerialPort As IO.Ports.SerialPort
    Private _connected As Boolean = False

    Public Event DataReceived(msg As String)
    Public Event ConnectionError(msg As String)
    Public Event ConnectionStatus(connected As Boolean)

#End Region

#Region " Constructor "

    Public Sub New()
        SerialPort = New IO.Ports.SerialPort()
        SerialPort.BaudRate = 9600
        SerialPort.DataBits = 8
        SerialPort.Parity = IO.Ports.Parity.None
        SerialPort.StopBits = IO.Ports.StopBits.One
        SerialPort.ReadTimeout = 500
        SerialPort.WriteTimeout = 500
    End Sub

    Public Sub Dispose()
        Disconnect()
        If (SerialPort IsNot Nothing) Then SerialPort.Dispose()
    End Sub

#End Region

#Region " Connection "

#Region " Ports "

    Public Shared Function Ports() As String()
        Return (IO.Ports.SerialPort.GetPortNames())
    End Function

#End Region

#Region " Connect "

    Public Function Connect(port As String) As Boolean
        Try
            If (SerialPort.IsOpen) Then SerialPort.Close()

            SerialPort.PortName = port
            SerialPort.Open()
            _connected = True
            RaiseEvent ConnectionStatus(True)
            Return (True)

        Catch ex As Exception
            _connected = False
            RaiseEvent ConnectionError("Connecting error: " & ex.Message)
            Return (False)
        End Try
    End Function

#End Region

#Region " Disconnect "

    Public Sub Disconnect()
        Try
            If (SerialPort IsNot Nothing AndAlso SerialPort.IsOpen) Then
                Try
                    SerialPort.DiscardInBuffer()
                    SerialPort.DiscardOutBuffer()

                    SerialPort.Write("S")
                    Threading.Thread.Sleep(100)
                Catch : End Try

                RemoveHandler SerialPort.DataReceived, AddressOf SerialPort_DataReceived

                Dim closeTask = Task.Run(Sub() SerialPort.Close())
                If (Not closeTask.Wait(1000)) Then
                    SerialPort.Dispose()
                    SerialPort = Nothing
                End If
            End If
        Catch : Finally
            _connected = False
            RaiseEvent ConnectionStatus(False)
        End Try
    End Sub

#End Region

#Region " Data Received "

    Private Sub SerialPort_DataReceived(sender As Object, e As IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort.DataReceived
        Try
            RaiseEvent DataReceived(SerialPort.ReadLine().Trim())
        Catch : End Try
    End Sub

#End Region

#Region " Send Data "

    Private Function SendData(command As String) As Boolean
        Try
            If (Not (_connected AndAlso SerialPort IsNot Nothing AndAlso SerialPort.IsOpen)) Then
                RaiseEvent ConnectionError("Not connected")
                Return (False)
            End If

            SerialPort.WriteLine(command)
            Return (True)

        Catch ex As Exception
            RaiseEvent ConnectionError("Sending error: " & ex.Message)
            Return (False)
        End Try
    End Function

#End Region

#End Region

#Region " Trim "

#Region " Up "

    Public Function TrimUpContinuous(speed As Integer) As Boolean
        If (speed < 50 OrElse speed > 5000) Then Return (False)

        Return (SendData($"U:{speed}"))
    End Function

    Public Function TrimUp(steps As Integer, speed As Integer) As Boolean
        If (steps <= 0 OrElse speed < 50 OrElse speed > 5000) Then Return (False)

        Return SendData($"US:{steps}:{speed}")
    End Function

#End Region

#Region " Down "

    Public Function TrimDownContinuous(speed As Integer) As Boolean
        If (speed < 50 OrElse speed > 5000) Then Return (False)

        Return (SendData($"D:{speed}"))
    End Function

    Public Function TrimDown(steps As Integer, speed As Integer) As Boolean
        If (steps <= 0 OrElse speed < 50 OrElse speed > 5000) Then Return (False)

        Return SendData($"DS:{steps}:{speed}")
    End Function

#End Region

#Region " Stop "

    Public Function DetenerMotor() As Boolean
        Return (SendData("S"))
    End Function

#End Region

#End Region

End Class
