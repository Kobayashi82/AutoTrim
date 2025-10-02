Public Class Hook

#Region " Key Input "

    Private mainForm As FMenu

    Public Sub New()
        ' Encontrar la instancia del formulario principal
        For Each form As Form In Application.OpenForms
            If TypeOf form Is FMenu Then
                mainForm = DirectCast(form, FMenu)
                Exit For
            End If
        Next
    End Sub

    Public Sub RegisterKeyPresses()
        Try
            Dim Input As FSUIPC.UserInputServices = FSUIPC.FSUIPCConnection.UserInputServices
            Input.AddKeyPress("TrimZero", FSUIPC.ModifierKeys.Ctrl, Keys.T, True)
            Input.AddKeyPress("TrimAuto", FSUIPC.ModifierKeys.Shift, Keys.T, True)
            AddHandler Input.KeyPressed, AddressOf Input_KeyPressed
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Public Sub UnregisterKeyPresses()
        Try
            Dim Input As FSUIPC.UserInputServices = FSUIPC.FSUIPCConnection.UserInputServices
            RemoveHandler Input.KeyPressed, AddressOf Input_KeyPressed
            Input.RemoveKeyPress("TrimZero")
            Input.RemoveKeyPress("TrimAuto")
        Catch
        End Try
    End Sub

    Private Sub Input_KeyPressed(sender As Object, e As FSUIPC.UserInputKeyEventArgs)
        If mainForm Is Nothing Then
            ' Intentar encontrar el formulario nuevamente si no se encontró antes
            For Each form As Form In Application.OpenForms
                If TypeOf form Is FMenu Then
                    mainForm = DirectCast(form, FMenu)
                    Exit For
                End If
            Next
        End If

        If mainForm IsNot Nothing Then
            Select Case e.ID
                Case "TrimZero"
                    If mainForm.CurrentMode = "TrimZero" And mainForm.IsStarted = True Then
                        mainForm.StopTrim()
                    Else
                        mainForm.SetTrimZero()
                    End If
                Case "TrimAuto"
                    If mainForm.CurrentMode = "TrimAuto" And mainForm.IsStarted = True Then
                        mainForm.StopTrim()
                    Else
                        mainForm.SetTrimAuto()
                    End If
            End Select
        End If
    End Sub

#End Region

End Class
