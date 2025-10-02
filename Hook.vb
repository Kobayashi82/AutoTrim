
Public Class Hook

#Region " Variables "

    Public Event TrimZero()
    Public Event TrimAuto()

#End Region

#Region " Register "

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

#End Region

#Region " Unregister "

    Public Sub UnregisterKeyPresses()
        Try
            Dim Input As FSUIPC.UserInputServices = FSUIPC.FSUIPCConnection.UserInputServices
            RemoveHandler Input.KeyPressed, AddressOf Input_KeyPressed

            Input.RemoveKeyPress("TrimZero")
            Input.RemoveKeyPress("TrimAuto")
        Catch : End Try
    End Sub

#End Region

#Region " Input "

    Private Sub Input_KeyPressed(sender As Object, e As FSUIPC.UserInputKeyEventArgs)
        Select Case (e.ID)
            Case "TrimZero"
                RaiseEvent TrimZero()
            Case "TrimAuto"
                RaiseEvent TrimAuto()
        End Select
    End Sub

#End Region

End Class
