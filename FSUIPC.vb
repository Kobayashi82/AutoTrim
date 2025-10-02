
Public Class FSUIPC

#Region " Variables "

    ' Timers
    Private WithEvents FSUIPC_TConnect As New Timer With {.Enabled = False, .Interval = 50}
    Private WithEvents FSUIPC_Main As New Timer With {.Enabled = False, .Interval = 50}

    ' Offsets
    Private FSUIPC_Trim As New Global.FSUIPC.Offset(Of Short)(&HBC2)
    Private FSUIPC_TrimSet As New Global.FSUIPC.Offset(Of Short)(&HBC0)
    Private FSUIPC_VerticalSpeed As New Global.FSUIPC.Offset(Of Integer)(&H2C8)

    ' Configuración
    Private Config As ConfigManager

    ' Estados
    Private _isConnected As Boolean = False
    Private _verticalSpeed As Double = 0
    Private _trimValue As Short = 0
    Private _trimPercentage As Double = 0

    Public lowerLimit As Integer = -16383
    Public upperLimit As Integer = 16383

#End Region

#Region " Events "

    Public Event ConnectionStatusChanged(isConnected As Boolean)
    Public Event DataUpdated(verticalSpeed As Double, trimValue As Short, trimPercentage As Double)

#End Region

#Region " Properties "

    Public ReadOnly Property IsConnected As Boolean
        Get
            Return (_isConnected)
        End Get
    End Property

    Public ReadOnly Property VerticalSpeed As Double
        Get
            Return (_verticalSpeed)
        End Get
    End Property

    Public ReadOnly Property TrimValue As Short
        Get
            Return (_trimValue)
        End Get
    End Property

    Public ReadOnly Property TrimPercentage As Double
        Get
            Return (_trimPercentage)
        End Get
    End Property

#End Region

#Region " Constructor "

    Public Sub New(config As ConfigManager)
        Me.Config = config

        FSUIPC_Main.Interval = config.FSUIPC_OffsetsUpdateRate
    End Sub

    Public Sub Dispose()
        Disconnect()

        If (FSUIPC_TConnect IsNot Nothing) Then FSUIPC_TConnect.Dispose()
        If (FSUIPC_Main IsNot Nothing) Then FSUIPC_Main.Dispose()
    End Sub

#End Region

#Region " Public Methods "

#Region " Connect "

    Public Sub Connect()
        FSUIPC_TConnect.Start()
    End Sub

#End Region

#Region " Disconnect "

    Public Sub Disconnect()
        FSUIPC_TConnect.Stop()
        FSUIPC_Main.Stop()

        If (Global.FSUIPC.FSUIPCConnection.IsOpen) Then
            Try
                Call Global.FSUIPC.FSUIPCConnection.Close()
            Catch : End Try
        End If

        _isConnected = False
        RaiseEvent ConnectionStatusChanged(_isConnected)
    End Sub

#End Region

#Region " Set Trim "

    Public Sub SetTrim(value As Short)
        If (Not _isConnected) Then Throw New InvalidOperationException("FSUIPC not connected")

        If (value > upperLimit) Then
            value = upperLimit
        ElseIf (value < lowerLimit) Then
            value = lowerLimit
        End If

        FSUIPC_TrimSet.Value = value
    End Sub

#End Region

#Region " Process Data "

    Public Sub ProcessData()
        If (_isConnected) Then
            Try
                Call Global.FSUIPC.FSUIPCConnection.Process()
                Call Global.FSUIPC.FSUIPCConnection.UserInputServices.CheckForInput()

                UpdateData()
            Catch
                _isConnected = False
                FSUIPC_Main.Stop()
                RaiseEvent ConnectionStatusChanged(_isConnected)
                FSUIPC_TConnect.Start()
            End Try
        End If
    End Sub

#End Region

#End Region

#Region " Private Methods "

#Region " Update Data "

    Private Sub UpdateData()
        _verticalSpeed = (FSUIPC_VerticalSpeed.Value / 256) * 60.0 * 3.28084
        _trimValue = FSUIPC_Trim.Value

        If (_trimValue < lowerLimit) Then
            _trimPercentage = 0
        ElseIf (_trimValue > upperLimit) Then
            _trimPercentage = 100
        Else
            _trimPercentage = ((_trimValue - lowerLimit) / (upperLimit - lowerLimit)) * 100.0
        End If

        RaiseEvent DataUpdated(_verticalSpeed, _trimValue, _trimPercentage)
    End Sub

#End Region

#Region " Connection Timer "

    Private Sub FSUIPC_TConnect_Tick(sender As Object, e As EventArgs) Handles FSUIPC_TConnect.Tick
        If (FSUIPC_TConnect.Interval = 50) Then FSUIPC_TConnect.Interval = Config.FSUIPC_RetryInterval

        Try
            Call Global.FSUIPC.FSUIPCConnection.Open()

            If (Global.FSUIPC.FSUIPCConnection.IsOpen) Then
                _isConnected = True
                RaiseEvent ConnectionStatusChanged(_isConnected)
                FSUIPC_TConnect.Stop()
                FSUIPC_Main.Start()
            End If
        Catch : End Try
    End Sub

#End Region

#Region " Update Timer "

    Private Sub FSUIPC_Main_Tick(sender As Object, e As EventArgs) Handles FSUIPC_Main.Tick
        ProcessData()
    End Sub

#End Region

#End Region

End Class