
Module FSUIPC

    Dim FSUIPC_VerticalSpeed As New FSUIPC.Offset(Of Integer)(&H2C8)

#Region " Connection "

    Private Sub FSUIPC_TConnect_Tick(sender As Object, e As EventArgs)
        Try : FSUIPC.FSUIPCConnection.Open() : Catch : End Try
        If FSUIPC.FSUIPCConnection.IsOpen Then
            PConection.Image = My.Resources.Green
            RegisterKeyPresses()
            FSUIPC_TConnect.Stop()
            FSUIPC_Main.Start()
        End If
    End Sub

    Private Sub FSUIPC_Main_Tick(sender As Object, e As EventArgs)
        Try
            FSUIPC.FSUIPCConnection.Process()
            FSUIPC.FSUIPCConnection.UserInputServices.CheckForInput()

            MSFS_VerticalSpeed = (FSUIPC_VerticalSpeed.Value / 256) * 60.0 * 3.28084

            Texto.Text = MSFS_VerticalSpeed.ToString("F0")
            Label1.Text = Math.Round(GetTrimPercentage(Trim.Value), 2).ToString + "% - (" + Trim.Value.ToString + ")"
        Catch
            If FSUIPC.FSUIPCConnection.IsOpen = False Then
                PConection.Image = My.Resources.Red
                Iniciado = False
                Button1.Text = Button1.Text.Replace("ON", "OFF")
                FSUIPC_Main.Stop()
                UnregisterKeyPresses()
                FSUIPC_TConnect.Start()
            End If
        End Try
    End Sub

#End Region

End Module
