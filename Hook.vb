Public Class Hook

#Region " Key Input "

    Private Sub RegisterKeyPresses()
        Try : Dim Input As FSUIPC.UserInputServices = FSUIPC.FSUIPCConnection.UserInputServices
            Input.AddKeyPress("TrimZero", FSUIPC.ModifierKeys.Ctrl, Keys.T, True)
            Input.AddKeyPress("TrimAuto", FSUIPC.ModifierKeys.Shift, Keys.T, True)
            AddHandler Input.KeyPressed, AddressOf Input_KeyPressed
        Catch ex As Exception : MsgBox(ex.Message) : End Try
    End Sub

    Private Sub UnregisterKeyPresses()
        Try : Dim Input As FSUIPC.UserInputServices = FSUIPC.FSUIPCConnection.UserInputServices
            RemoveHandler Input.KeyPressed, AddressOf Input_KeyPressed
            Input.RemoveKeyPress("TrimZero")
            Input.RemoveKeyPress("TrimAuto")
        Catch : End Try
    End Sub

    Private Sub Input_KeyPressed(sender As Object, e As FSUIPC.UserInputKeyEventArgs)
        Select Case e.ID
            Case "TrimZero"
                If Mode = "TrimZero" And Iniciado = True Then
                    Iniciado = False
                    Button1.Text = "Trim-Zero OFF"
                    Exit Select
                Else
                    Mode = "TrimZero"
                    Final_VerticalSpeed = 0
                    TTexto.Text = "0"
                    PIDController2.SetPoint = Final_VerticalSpeed
                    'PIDController.ProcessVariable = MSFS_VerticalSpeed
                    Button1.Text = "Trim-Zero ON"
                    If Iniciado = False Then Iniciado = True : Iniciar()
                End If
            Case "TrimAuto"
                If Mode = "TrimAuto" And Iniciado = True Then
                    Iniciado = False
                    Button1.Text = "Trim-Auto OFF"
                    Exit Select
                Else
                    Mode = "TrimAuto"
                    Final_VerticalSpeed = MSFS_VerticalSpeed
                    TTexto.Text = Math.Round(Final_VerticalSpeed)
                    PIDController2.SetPoint = Final_VerticalSpeed
                    'PIDController.ProcessVariable = MSFS_VerticalSpeed
                    Button1.Text = "Trim-Auto ON"
                    If Iniciado = False Then Iniciado = True : Iniciar()
                End If
        End Select
    End Sub

#End Region

End Class
