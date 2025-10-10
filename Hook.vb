Public Class Hook

#Region " Variables "

    Public Event AP_ON(sender As Object, e As EventArgs)
    Public Event TrimUp()
    Public Event TrimDown()

    ' Timer necesario para el polling de FSUIPC
    Private WithEvents timerMain As New Timer With {.Enabled = False, .Interval = 50}

#End Region
    Public Sub RegisterJoystickButtons()
        Try
            ' Verificar que FSUIPC esté conectado primero
            If Not Global.FSUIPC.FSUIPCConnection.IsOpen Then
                Return
            End If

            Dim Input As Global.FSUIPC.UserInputServices = Global.FSUIPC.FSUIPCConnection.UserInputServices

            ' Registrar botones del yoke (ajusta joystick y números de botón según tu hardware)
            Input.AddJoystickButtonPress("TrimUp", 6, 4, Global.FSUIPC.StateChange.Both)
            Input.AddJoystickButtonPress("TrimDown", 6, 5, Global.FSUIPC.StateChange.Both)

            ' Registrar el evento
            AddHandler Input.ButtonPressed, AddressOf Input_ButtonPressed

            ' Iniciar el timer que hace polling
            timerMain.Start()

        Catch : End Try
    End Sub

    Private Sub Input_ButtonPressed(sender As Object, e As Global.FSUIPC.UserInputButtonEventArgs)
        Try
            ' Obtener referencia al motor desde FMenu
            Dim fmenu As FMenu = Nothing
            For Each form As Form In Application.OpenForms
                If TypeOf form Is FMenu Then
                    fmenu = DirectCast(form, FMenu)
                    Exit For
                End If
            Next

            If fmenu Is Nothing OrElse fmenu.Motor Is Nothing Then
                Return
            End If

            Dim motor As MotorController = fmenu.Motor

            Select Case e.ID
                Case "TrimUp"
                    If e.ButtonState Then
                        motor.TrimUp(0, 100) ' Velocidad por defecto
                    Else
                        motor.DetenerMotor()
                    End If

                Case "TrimDown"
                    If e.ButtonState Then
                        motor.TrimDown(0, 100) ' Velocidad por defecto
                    Else
                        motor.DetenerMotor()
                    End If
            End Select

        Catch : End Try
    End Sub

    Protected Sub timerMain_Tick(sender As Object, e As EventArgs) Handles timerMain.Tick
        Try
            ' Esto es CRÍTICO - hace el polling de FSUIPC
            If Global.FSUIPC.FSUIPCConnection.IsOpen Then
                Global.FSUIPC.FSUIPCConnection.UserInputServices.CheckForInput()
            Else
                ' Si FSUIPC se desconecta, detener el timer
                timerMain.Stop()
            End If
        Catch ex As Exception
            ' Si hay error, detener el timer para evitar spam de errores
            timerMain.Stop()
        End Try
    End Sub

#Region " Register "

    Public Sub RegisterKeyPresses()
        Try
            Dim Input As Global.FSUIPC.UserInputServices = Global.FSUIPC.FSUIPCConnection.UserInputServices

            Input.AddKeyPress("AP_Level", Global.FSUIPC.ModifierKeys.Shift, Keys.O, True)
            Input.AddKeyPress("AP_Current", Global.FSUIPC.ModifierKeys.Ctrl, Keys.O, True)
            Input.AddKeyPress("AP_Toggle", Global.FSUIPC.ModifierKeys.Ctrl, Keys.P, True)
            Input.AddKeyPress("TrimUp", Global.FSUIPC.ModifierKeys.Ctrl, Keys.T, True)
            Input.AddKeyPress("TrimDown", Global.FSUIPC.ModifierKeys.Shift, Keys.T, True)

            AddHandler Input.KeyPressed, AddressOf Input_KeyPressed

            ' También registrar los botones del joystick
            RegisterJoystickButtons()
        Catch ex As Exception
            MsgBox("Error registering key presses: " & ex.Message)
        End Try
    End Sub

#End Region

#Region " Unregister "

    Public Sub UnregisterKeyPresses()
        Try
            Dim Input As Global.FSUIPC.UserInputServices = Global.FSUIPC.FSUIPCConnection.UserInputServices

            ' Remover handlers
            RemoveHandler Input.KeyPressed, AddressOf Input_KeyPressed
            RemoveHandler Input.ButtonPressed, AddressOf Input_ButtonPressed

            ' Remover teclas
            Input.RemoveKeyPress("AP_Level")
            Input.RemoveKeyPress("AP_Current")
            Input.RemoveKeyPress("AP_Toggle")
            Input.RemoveKeyPress("TrimUp")
            Input.RemoveKeyPress("TrimDown")

            ' Detener timer
            timerMain.Stop()
        Catch : End Try
    End Sub

#End Region

#Region " Input "

    Private Sub Input_KeyPressed(sender As Object, e As Global.FSUIPC.UserInputKeyEventArgs)
        Select Case (e.ID)
            Case "AP_Level"
                RaiseEvent AP_ON("Level", Nothing)
            Case "AP_Current"
                RaiseEvent AP_ON("Current", Nothing)
            Case "AP_Toggle"
                RaiseEvent AP_ON("Toggle", Nothing)
            Case "TrimUp"
                RaiseEvent TrimUp()
            Case "TrimDown"
                RaiseEvent TrimDown()
        End Select
    End Sub

#End Region

End Class
