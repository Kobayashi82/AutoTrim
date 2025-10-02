
'Trim Wheel Step 79

#Region " Trimmer "

Public Class FMenu

#Region " Variables "

    Dim Version As String = "1.0"
    Dim Mode As String
    Dim Loaded As Boolean

    Dim FSUIPC_TConnect As New Timer With {.Enabled = True, .Interval = 1000}
    Dim FSUIPC_Main As New Timer With {.Enabled = False, .Interval = 50}

    Dim Trim As New FSUIPC.Offset(Of Short)(&HBC2)
    Dim TrimSet As New FSUIPC.Offset(Of Short)(&HBC0)

    Dim MSFS_VerticalSpeed As Double
    Dim Final_VerticalSpeed As Double
    Dim TrimFinal As Short
    Dim Iniciado As Boolean

    'Dim PIDController As New PIDController(0.1, 0.0005, 8)
    'Dim PIDController As New PIDController(0.00008, 0.00005, 6)  ' afinado
    'Este Dim PIDController As New PIDController(0.05, 0.0005, 8)  'pretty good


    'Dim PIDController As New PIDController(0.05, 0.0005, 3)
    'Dim PIDController As New PIDController(0.06, 0.04, 0.0225)

    Dim PIDController As New PIDController(0.02, 0.00005, 3)

    Dim PIDController2 As New PIDController2(0.5, 0.5, 3)
#End Region

#Region " Formulario "

    Private Sub FMenu_Load(sender As Object, e As EventArgs) Handles Me.Load
        Text = "Trimmer " + Version

        AddHandler FSUIPC_TConnect.Tick, AddressOf FSUIPC_TConnect_Tick
        AddHandler FSUIPC_Main.Tick, AddressOf FSUIPC_Main_Tick
        Height = 216
    End Sub

    Private Sub HTexto_KeyPress(sender As Object, e As KeyPressEventArgs) Handles HTexto.KeyPress
        e.Handled = True
    End Sub

#End Region

#Region " Functions "

#Region " Save Position "

    Public Sub New()
        InitializeComponent()

        WindowState = FormWindowState.Normal
        StartPosition = FormStartPosition.WindowsDefaultBounds

        If My.Settings.WindowPosition <> Rectangle.Empty AndAlso IsVisibleOnAnyScreen(My.Settings.WindowPosition) Then
            StartPosition = FormStartPosition.Manual
            DesktopBounds = My.Settings.WindowPosition

            WindowState = My.Settings.WindowState
        Else
            StartPosition = FormStartPosition.WindowsDefaultLocation
            Size = My.Settings.WindowPosition.Size
        End If
    End Sub

    Private Function IsVisibleOnAnyScreen(rect As Rectangle) As Boolean
        For Each screen As Screen In Screen.AllScreens
            If screen.WorkingArea.IntersectsWith(rect) Then
                Return True
            End If
        Next
        Return False
    End Function

    Protected Overrides Sub OnClosed(e As EventArgs)
        Iniciado = False
        UnregisterKeyPresses()
        FSUIPC_TConnect.Stop()
        FSUIPC_Main.Stop()

        MyBase.OnClosed(e)

        Select Case WindowState
            Case FormWindowState.Normal, FormWindowState.Maximized
                My.Settings.WindowState = WindowState
            Case Else
                My.Settings.WindowState = FormWindowState.Normal
        End Select

        Visible = False : WindowState = FormWindowState.Normal
        My.Settings.WindowPosition = New Rectangle(DesktopBounds.X, DesktopBounds.Y, Width, Height) : My.Settings.Save()
    End Sub

#End Region

#Region " Get Trim Percentage "

    Private Function GetTrimPercentage(TrimValue As Integer) As Double
        Dim LowerLimit As Integer = -16383
        Dim UpperLimit As Integer = 16383

        If TrimValue < LowerLimit Then Return 0
        If TrimValue > UpperLimit Then Return 100

        Return ((TrimValue - LowerLimit) / (UpperLimit - LowerLimit)) * 100.0
    End Function

#End Region

#End Region

#Region " Controls "

#Region " TTexto "

    Private Sub TTexto_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TTexto.KeyPress
        Dim ch As Char = e.KeyChar
        If Char.IsDigit(ch) OrElse ch = ControlChars.Cr Or ch = ControlChars.Back Or ch = "-"c Then
            If Asc(e.KeyChar) = 13 Then
                e.Handled = True : HTexto.Focus()
            Else
                If TTexto.TextLength >= TTexto.MaxLength AndAlso Not Char.IsControl(e.KeyChar) AndAlso TTexto.SelectedText = "" Then e.Handled = True Else e.Handled = False
            End If
        Else
            e.Handled = True
        End If
    End Sub

    Private Sub TTexto_GotFocus(sender As Object, e As EventArgs) Handles TTexto.GotFocus
        Dim Texto As TextBox = DirectCast(sender, TextBox)
        If Texto.Text = "0" Then Texto.Text = ""
    End Sub

    Private Sub TTexto_LostFocus(sender As Object, e As EventArgs) Handles TTexto.LostFocus
        If TTexto.Text = "" Then TTexto.Text = 0
        Final_VerticalSpeed = TTexto.Text
        PIDController2.SetPoint = Final_VerticalSpeed
        'PIDController.ProcessVariable = MSFS_VerticalSpeed
        If TTexto.Text = "0" Then
            If Iniciado = True Then Button1.Text = "Trim-Zero ON" Else Button1.Text = "Trim-Zero OFF"
            Mode = "TrimZero"
        Else
            If Iniciado = True Then Button1.Text = "Trim-Auto ON" Else Button1.Text = "Trim-Auto OFF"
            Mode = "TrimAuto"
        End If
    End Sub

#End Region

#Region " Button1 "

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If FSUIPC.FSUIPCConnection.IsOpen = False Then Exit Sub
        If Iniciado = False Then
            Iniciado = True
            If TTexto.Text = "0" Then Button1.Text = "Trim-Zero ON" Else Button1.Text = "Trim-Auto ON"
            Iniciar()
        Else
            Iniciado = False
            If TTexto.Text = "0" Then Button1.Text = "Trim-Zero OFF" Else Button1.Text = "Trim-Auto OFF"
        End If
    End Sub

#End Region

#End Region

#End Region

#Region " Iniciar "

    Private Sub Iniciar2()
        FSUIPC.FSUIPCConnection.Process()
        TrimFinal = Trim.Value
        PIDController.SetPoint = Final_VerticalSpeed
        PIDController.ProcessVariable = MSFS_VerticalSpeed

        While Iniciado = True

            '79 Steps

            'Dim ControlOutput As Double
            'Dim ControlOutputRounded As Short

            'controlOutput = PIDController.ControlOutput
            'Dim controlOutputRounded As Int16 = CShort(Math.Round(controlOutput / 79.0) * 79)
            'TrimFinal += controlOutputRounded

            Dim ControlOutput As Double = PIDController.ControlOutput

            Dim PotentialControlOutput As Integer = CInt(Math.Round(ControlOutput))

            If PotentialControlOutput > 16383 Then
                PotentialControlOutput = 16383
            ElseIf PotentialControlOutput < -16383 Then
                PotentialControlOutput = -16383
            End If

            Dim ShortControlOutput As Short = CShort(PotentialControlOutput)
            Dim PotentialTrimFinal As Integer = TrimFinal + ShortControlOutput

            If PotentialTrimFinal > 16383 Then
                TrimFinal = 16383
            ElseIf PotentialTrimFinal < -16383 Then
                TrimFinal = -16383
            Else
                TrimFinal = PotentialTrimFinal
            End If

            PIDController.ProcessVariable = MSFS_VerticalSpeed
            TrimSet.Value = Short.Parse(TrimFinal)
            Application.DoEvents()
            Threading.Thread.Sleep(50)
        End While
    End Sub

#End Region

#Region " Iniciar "

    Private Sub Iniciar()
        ' Ajustar la velocidad vertical deseada
        Dim VelocidadDeseada As Double = TTexto.Text
        Dim pidController As New ComplexPIDController(VelocidadDeseada)

        ' Bucle que se ejecuta mientras Iniciado es True:
        While Iniciado
            ' Leer la velocidad vertical actual
            FSUIPC.FSUIPCConnection.Process()
            Dim velocidadVerticalActual As Double = MSFS_VerticalSpeed

            ' Calcular la nueva salida del controlador PID y ajustar el trim:
            Dim newTrimValue = Trim.Value + pidController.Compute(velocidadVerticalActual)
            If Double.IsNaN(newTrimValue) OrElse Double.IsInfinity(newTrimValue) Then
                ' Lógica de manejo de errores, como registro o alerta
            Else
                If newTrimValue > 16383 Then
                    newTrimValue = 16383
                ElseIf newTrimValue < -16383 Then
                    newTrimValue = -16383
                End If

                TrimSet.Value = newTrimValue
            End If

            ' Dormir durante 50 ms antes de la siguiente actualización:
            Application.DoEvents()
            Threading.Thread.Sleep(50)
        End While
    End Sub

#End Region

End Class

#End Region
