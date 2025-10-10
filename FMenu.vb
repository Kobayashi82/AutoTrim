Public Class FMenu

#Region " Variables "

    Private Version As String = "1.0"

    Private Config As ConfigManager
    Private WithEvents FSUIPCMgr As FSUIPC
    Private WithEvents Hook As New Hook()
    Public WithEvents Motor As MotorController

    Private PIDCtr As PIDController ' Declarar sin inicializar
    Private WithEvents PIDTimer As New Timer With {.Enabled = False, .Interval = 200}

    Dim MSFS_VerticalSpeed As Double
    Dim Final_VerticalSpeed As Double
    Dim Target_VS As Double
    Dim TrimFinal As Short

    Dim Mode As String
    Dim Iniciado As Boolean

#End Region

#Region " Constructor "

    Public Sub New()
        InitializeComponent()

        Config = New ConfigManager()
        FSUIPCMgr = New FSUIPC(Config)

        ' Configurar el timer PID
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

        ' Inicializar PIDController después de que Motor y FSUIPCMgr estén listos
        PIDCtr = New PIDController(Motor, FSUIPCMgr)

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

        Target_VS = TTexto.Text

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
            PIDTimer.Stop()
            lbl_AP_Status.Text = "AP OFF"
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

#Region " AP ON "

    Private Sub AP_ON(sender As Object, e As EventArgs) Handles Hook.AP_ON, btn_AP_Level.MouseClick, btn_AP_Current.Click, btn_AP_Toggle.Click
        If (Not FSUIPCMgr.IsConnected) Then Return

        If TypeOf sender Is Button Then
            Dim btn As Button = DirectCast(sender, Button)
            If btn Is btn_AP_Level Then
                Target_VS = 0
            ElseIf btn Is btn_AP_Current Then
                Target_VS = MSFS_VerticalSpeed
            ElseIf btn Is btn_AP_Toggle Then
                If (Iniciado) Then
                    Iniciado = False
                    PIDTimer.Stop()
                    lbl_AP_Status.Text = "AP OFF"
                    Return
                End If
            End If

        ElseIf TypeOf sender Is String Then
            Dim action As String = CStr(sender)
            Select Case action
                Case "AP_Level"
                    Target_VS = 0
                Case "AP_Current"
                    Target_VS = MSFS_VerticalSpeed
                Case "AP_Toggle"
                    If (Iniciado) Then
                        Iniciado = False
                        PIDTimer.Stop()
                        lbl_AP_Status.Text = "AP OFF"
                        Return
                    End If
            End Select
        End If

        TTexto.Text = Math.Round(Target_VS).ToString()
        If (Iniciado = False) Then StartControl()
    End Sub

#End Region

#Region " Trim Up "

    Private Sub Hook_TrimUp() Handles Hook.TrimUp
        Motor.TrimUp(0, trackVelocidad.Value)
    End Sub

#End Region

#Region " Trim Down "

    Private Sub Hook_TrimDown() Handles Hook.TrimDown
        Motor.TrimDown(0, trackVelocidad.Value)
    End Sub

#End Region

#End Region

#Region " PID "

#Region " Start "

    Private Sub StartControl()
        If (Not FSUIPCMgr.IsConnected) Then Return

        If (PIDCtr Is Nothing) Then PIDCtr = New PIDController(Motor, FSUIPCMgr)

        Iniciado = True
        lbl_AP_Status.Text = "AP ON"

        PIDCtr.LoadProfile(AircraftProfile.LightSingleEngine)
        PIDCtr.ResetController()

        PIDTimer.Start()
    End Sub

#End Region

#Region " Timer "

    Private Sub PIDTimer_Tick(sender As Object, e As EventArgs) Handles PIDTimer.Tick
        If (Not Iniciado OrElse Not FSUIPCMgr.IsConnected OrElse PIDCtr Is Nothing) Then
            Iniciado = False
            PIDTimer.Stop()
            lbl_AP_Status.Text = "AP OFF"
            Return
        End If

        Try
            PIDCtr.Update(Target_VS)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Error in PID Controller")
            Iniciado = False
            PIDTimer.Stop()
            lbl_AP_Status.Text = "AP OFF2"
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

#End Region

End Class
