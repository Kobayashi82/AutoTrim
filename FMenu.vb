
Public Class FMenu

#Region " Variables "

    Private Version As String = "1.0"

    Private Config As ConfigManager
    Private WithEvents FSUIPCMgr As FSUIPC
    Private WithEvents Hook As New Hook()

    Dim PIDController1 As PIDController
    Dim PIDController2 As PIDController2

    Private WithEvents PIDTimer As New Timer With {.Enabled = False}
    Private ComplexPIDCtrl As ComplexPIDController

    Dim MSFS_VerticalSpeed As Double
    Dim Final_VerticalSpeed As Double
    Dim TrimFinal As Short

    Dim Mode As String
    Dim Iniciado As Boolean

    Public WithEvents Motor As MotorController

#End Region

#Region " Constructor "

    Public Sub New()
        InitializeComponent()

        Config = New ConfigManager()
        FSUIPCMgr = New FSUIPC(Config)

        ' Configurar el timer PID
        PIDController1 = New PIDController(Config.PID_Kp, Config.PID_Ki, Config.PID_Kd)
        PIDTimer.Interval = Config.PID_UpdateRate

        WindowState = If(Config.Start_Minimized = "true", FormWindowState.Minimized, FormWindowState.Normal)
        StartPosition = FormStartPosition.WindowsDefaultLocation

        Dim windowRect As New Rectangle(Config.Window_X, Config.Window_Y, Width, Height)
        For Each screen As Screen In Screen.AllScreens
            If screen.WorkingArea.IntersectsWith(windowRect) Then
                StartPosition = FormStartPosition.Manual
                DesktopBounds = windowRect
                Exit For
            End If
        Next
    End Sub

#End Region

#Region " Formulario "

    Private Sub FMenu_Load(sender As Object, e As EventArgs) Handles Me.Load
        Text = "AutoTrim " + Version
        FSUIPCMgr.Connect()

        Motor = New MotorController()

        AddHandler Motor.DataReceived, AddressOf Motor_DataReceived
        AddHandler Motor.ConnectionError, AddressOf Motor_ConnectionError
        AddHandler Motor.ConnectionStatus, AddressOf Motor_ConnectionStatus

        cmbPuertos.Items.AddRange(MotorController.Ports())
        If (cmbPuertos.Items.Count = 0) Then cmbPuertos.Items.Add("No ports found")
        cmbPuertos.SelectedIndex = 0
    End Sub

    Private Sub HTexto_KeyPress(sender As Object, e As KeyPressEventArgs) Handles HTexto.KeyPress
        e.Handled = True
    End Sub

    Protected Overrides Sub OnClosed(e As EventArgs)
        Iniciado = False
        PIDTimer.Stop()
        Motor.Disconnect()
        Hook.UnregisterKeyPresses()
        FSUIPCMgr.Dispose()

        MyBase.OnClosed(e)

        Visible = False
        Config.SaveConfiguration(Left, Top)
    End Sub

#End Region

#Region " Controls "

#Region " TTexto "

    Private Sub TTexto_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TTexto.KeyPress
        Dim ch As Char = e.KeyChar

        If (Char.IsDigit(ch) OrElse ch = ControlChars.Cr Or ch = ControlChars.Back Or ch = "-"c) Then
            If (Asc(e.KeyChar) = 13) Then
                e.Handled = True
                HTexto.Focus()
            Else
                e.Handled = (TTexto.TextLength >= TTexto.MaxLength AndAlso Not Char.IsControl(e.KeyChar) AndAlso TTexto.SelectedText = "")
            End If
        Else
            e.Handled = True
        End If
    End Sub

    Private Sub TTexto_GotFocus(sender As Object, e As EventArgs) Handles TTexto.GotFocus
        Dim Texto As TextBox = DirectCast(sender, TextBox)

        If (Texto.Text = "0") Then Texto.Text = ""
    End Sub

    Private Sub TTexto_LostFocus(sender As Object, e As EventArgs) Handles TTexto.LostFocus
        If (TTexto.Text = "") Then TTexto.Text = 0

        Final_VerticalSpeed = TTexto.Text
        'PIDController2.SetPoint = Final_VerticalSpeed

        If (TTexto.Text = "0") Then
            Button1.Text = If(Iniciado = True, "Trim-Zero ON", "Trim-Zero OFF")
            Mode = "TrimZero"
        Else
            Button1.Text = If(Iniciado = True, "Trim-Auto ON", "Trim-Auto OFF")
            Mode = "TrimAuto"
        End If
    End Sub

#End Region

#Region " Button1 "

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not FSUIPCMgr.IsConnected Then Exit Sub

        If (Iniciado = False) Then
            Iniciado = True
            Button1.Text = If(TTexto.Text = "0", "Trim-Zero ON", "Trim-Auto ON")
            StartControl()
        Else
            Iniciado = False
            Button1.Text = If(TTexto.Text = "0", "Trim-Zero OFF", "Trim-Auto OFF")
            StopControl()
        End If
    End Sub

#End Region

#End Region

#Region " FSUIPC "

    Private Sub FSUIPCMgr_ConnectionStatusChanged(isConnected As Boolean) Handles FSUIPCMgr.ConnectionStatusChanged
        If (isConnected) Then
            PConection.Image = My.Resources.Green
            Hook.RegisterKeyPresses()
        Else
            PConection.Image = My.Resources.Red
            Iniciado = False
            StopControl()
            Button1.Text = Button1.Text.Replace("ON", "OFF")
            Hook.UnregisterKeyPresses()
        End If
    End Sub

    Private Sub FSUIPCMgr_DataUpdated(verticalSpeed As Double, trimValue As Short, trimPercentage As Double) Handles FSUIPCMgr.DataUpdated
        MSFS_VerticalSpeed = verticalSpeed
        Texto.Text = MSFS_VerticalSpeed.ToString("F0")
        Label1.Text = Math.Round(trimPercentage, 2).ToString + "% - (" + trimValue.ToString + ")"
    End Sub

#End Region

#Region " Hook "

#Region " Set Trim Zero "

    Public Sub SetTrimZero()
        If (Not FSUIPCMgr.IsConnected) Then Return

        Mode = "TrimZero"
        Final_VerticalSpeed = 0
        TTexto.Text = "0"
        'PIDController2.SetPoint = Final_VerticalSpeed
        Button1.Text = "Trim-Zero ON"
        If (Iniciado = False) Then
            Iniciado = True
            StartControl()
        End If
    End Sub

#End Region

#Region " Set Trim Auto "

    Public Sub SetTrimAuto()
        If (Not FSUIPCMgr.IsConnected) Then Return

        Mode = "TrimAuto"
        Final_VerticalSpeed = MSFS_VerticalSpeed
        TTexto.Text = Math.Round(Final_VerticalSpeed).ToString()
        'PIDController2.SetPoint = Final_VerticalSpeed
        Button1.Text = "Trim-Auto ON"
        If (Iniciado = False) Then
            Iniciado = True
            StartControl()
        End If
    End Sub

#End Region

#Region " Stop Trim "

    Public Sub StopTrim()
        Iniciado = False
        StopControl()
        Button1.Text = Button1.Text.Replace("ON", "OFF")
    End Sub

#End Region

#Region " Trim Zero "

    Private Sub Hook_TrimZero() Handles Hook.TrimZero
        If (Mode = "TrimZero" And Iniciado = True) Then
            StopTrim()
        Else
            SetTrimZero()
        End If
    End Sub

#End Region

#Region " Trim Auto "

    Private Sub Hook_TrimAuto() Handles Hook.TrimAuto
        If (Mode = "TrimAuto" And Iniciado = True) Then
            StopTrim()
        Else
            SetTrimAuto()
        End If
    End Sub

#End Region

#Region " Trim Zero "

    Private Sub Hook_TrimUp() Handles Hook.TrimUp
        Motor.TrimUp(0, trackVelocidad.Value)
    End Sub

#End Region

#Region " Trim Auto "

    Private Sub Hook_TrimDown() Handles Hook.TrimDown
        Motor.TrimDown(0, trackVelocidad.Value)
    End Sub

#End Region

#End Region

#Region " PID "

#Region " Start "

    Private Sub StartControl()
        If Not FSUIPCMgr.IsConnected Then Return

        Dim velocidadDeseada As Double = TTexto.Text
        ComplexPIDCtrl = New ComplexPIDController(velocidadDeseada, Config.PID_Kp, Config.PID_Ki, Config.PID_Kd)

        PIDTimer.Start()
    End Sub

#End Region

#Region " Stop "

    Private Sub StopControl()
        PIDTimer.Stop()
        ComplexPIDCtrl = Nothing
    End Sub

#End Region

#Region " Timer "

    Private Sub PIDTimer_Tick(sender As Object, e As EventArgs) Handles PIDTimer.Tick
        If (Not Iniciado OrElse Not FSUIPCMgr.IsConnected OrElse ComplexPIDCtrl Is Nothing) Then
            StopControl()
            Return
        End If

        Try
            ' Usar la velocidad vertical actual que ya está siendo actualizada por el evento DataUpdated
            Dim velocidadVerticalActual As Double = MSFS_VerticalSpeed

            ' Calcular la nueva salida del controlador PID y ajustar el trim
            Dim newTrimValue = FSUIPCMgr.TrimValue + ComplexPIDCtrl.Compute(velocidadVerticalActual)

            If (Double.IsNaN(newTrimValue) OrElse Double.IsInfinity(newTrimValue)) Then
                ' Lógica de manejo de errores, como registro o alerta
                Return
            End If

            If (newTrimValue > FSUIPCMgr.upperLimit) Then
                newTrimValue = FSUIPCMgr.upperLimit
            ElseIf (newTrimValue < FSUIPCMgr.lowerLimit) Then
                newTrimValue = FSUIPCMgr.lowerLimit
            End If

            FSUIPCMgr.SetTrim(CShort(newTrimValue))

        Catch
            Iniciado = False
            StopControl()
            Button1.Text = Button1.Text.Replace("ON", "OFF")
        End Try
    End Sub

#End Region

#End Region

#Region " Motor "

    ' Botón conectar
    Private Sub btnConectar_Click(sender As Object, e As EventArgs) Handles btnConectar.Click
        If (cmbPuertos.SelectedItem Is Nothing OrElse cmbPuertos.SelectedItem.ToString() = "No ports found") Then
            MessageBox.Show("No valid COM port selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            Motor.Connect(cmbPuertos.SelectedItem.ToString())
        End If
    End Sub

    ' Botón desconectar
    Private Sub btnDesconectar_Click(sender As Object, e As EventArgs) Handles btnDesconectar.Click
        Motor.Disconnect()
    End Sub

    ' Girar horario mientras se mantiene pulsado
    Private Sub btnTrimUpContinuous_MouseDown(sender As Object, e As MouseEventArgs) Handles btnTrimUpContinuous.MouseDown
        Motor.TrimUp(0, trackVelocidad.Value)
    End Sub

    Private Sub btnTrimUpContinuous_MouseUp(sender As Object, e As MouseEventArgs) Handles btnTrimUpContinuous.MouseUp
        Motor.DetenerMotor()
    End Sub

    ' Girar antihorario mientras se mantiene pulsado
    Private Sub btnTrimDownContinuous_MouseDown(sender As Object, e As MouseEventArgs) Handles btnTrimDownContinuous.MouseDown
        Motor.TrimDown(0, trackVelocidad.Value)
    End Sub

    Private Sub btnTrimDownContinuous_MouseUp(sender As Object, e As MouseEventArgs) Handles btnTrimDownContinuous.MouseUp
        Motor.DetenerMotor()
    End Sub

    Private Sub TrimUp_Click(sender As Object, e As EventArgs) Handles btnTrimUp.Click
        If (txtControl.Text = "") Then Return
        Dim resultado As Integer

        If Integer.TryParse(txtControl.Text, resultado) Then Motor.TrimUp(resultado, trackVelocidad.Value)
    End Sub

    Private Sub TrimDown_Click(sender As Object, e As EventArgs) Handles btnTrimDown.Click
        If (txtControl.Text = "") Then Return
        Dim resultado As Integer

        If Integer.TryParse(txtControl.Text, resultado) Then
            Motor.TrimDown(resultado, trackVelocidad.Value)
        End If
    End Sub

    ' === MANEJADORES DE EVENTOS ===

    Private Sub Motor_DataReceived(msg As String)
        If InvokeRequired Then
            Invoke(Sub() txtLog.AppendText(msg & vbCrLf))
        Else
            txtLog.AppendText(msg & vbCrLf)
        End If
    End Sub

    Private Sub Motor_ConnectionError(msg As String)
        MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Private Sub Motor_ConnectionStatus(connected As Boolean)
        lblEstado.Text = If(connected, "Connected", "Disconnected")
        lblEstado.ForeColor = If(connected, Color.Green, Color.DarkRed)
    End Sub

    Private Sub trackVelocidad_Scroll(sender As Object, e As EventArgs) Handles trackVelocidad.Scroll
        TDiff.Text = trackVelocidad.Value.ToString()
    End Sub

    Private Sub txtSpeed_TextChanged(sender As Object, e As EventArgs) Handles txtSpeed.TextChanged
        trackVelocidad.Value = If(txtSpeed.Text = "", 1, Math.Min(Math.Max(CInt(txtSpeed.Text), trackVelocidad.Minimum), trackVelocidad.Maximum))
    End Sub

    Private Sub btnTrimUpContinuous_Click_1(sender As Object, e As EventArgs)

    End Sub

#End Region

End Class
