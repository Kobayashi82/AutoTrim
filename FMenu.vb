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
    Dim FSUIPC_VerticalSpeed As New FSUIPC.Offset(Of Integer)(&H2C8)

    Dim MSFS_VerticalSpeed As Double
    Dim Final_VerticalSpeed As Double
    Dim TrimFinal As Short
    Dim Iniciado As Boolean

    Dim PIDController As New PIDController(0.02, 0.00005, 3)
    Dim PIDController2 As New PIDController2(0.5, 0.5, 3)

    ' Instancia de Hook para manejar las teclas
    Dim WithEvents keyHook As New Hook()

#End Region

#Region " Formulario "

    Private Sub FMenu_Load(sender As Object, e As EventArgs) Handles Me.Load
        Text = "Trimmer " + Version

        ' Verificar si se está ejecutando como administrador
        CheckAdministratorPrivileges()

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
        keyHook.UnregisterKeyPresses()
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

#Region " FSUIPC Connection "

    Private Sub FSUIPC_TConnect_Tick(sender As Object, e As EventArgs)
        Try
            FSUIPC.FSUIPCConnection.Open()

            If FSUIPC.FSUIPCConnection.IsOpen Then
                PConection.Image = My.Resources.Green
                keyHook.RegisterKeyPresses()
                FSUIPC_TConnect.Stop()
                FSUIPC_Main.Start()

                ' Mostrar mensaje de conexión exitosa
                Console.WriteLine("FSUIPC conectado exitosamente")
            End If

        Catch ex As FSUIPC.FSUIPCException
            ' Manejo específico de errores de FSUIPC
            Select Case ex.FSUIPCErrorCode
                Case FSUIPC.FSUIPCError.FSUIPC_ERR_OPEN
                    Console.WriteLine("FSUIPC no está ejecutándose")
                Case FSUIPC.FSUIPCError.FSUIPC_ERR_NOFS
                    Console.WriteLine("Simulador no está conectado a FSUIPC")
                Case FSUIPC.FSUIPCError.FSUIPC_ERR_REGMSG
                    Console.WriteLine("Error registrando mensaje con FSUIPC")
                Case FSUIPC.FSUIPCError.FSUIPC_ERR_ATOM
                    Console.WriteLine("Error creando átomo para FSUIPC")
                Case FSUIPC.FSUIPCError.FSUIPC_ERR_MAP
                    Console.WriteLine("Error mapeando vista de archivo de FSUIPC")
                Case FSUIPC.FSUIPCError.FSUIPC_ERR_VIEW
                    Console.WriteLine("Error mapeando vista de memoria de FSUIPC")
                Case FSUIPC.FSUIPCError.FSUIPC_ERR_VERSION
                    Console.WriteLine("Versión incompatible de FSUIPC")
                Case FSUIPC.FSUIPCError.FSUIPC_ERR_WRONGFS
                    Console.WriteLine("Simulador incorrecto conectado a FSUIPC")
                Case FSUIPC.FSUIPCError.FSUIPC_ERR_NOTOPEN
                    Console.WriteLine("FSUIPC no está abierto")
                Case FSUIPC.FSUIPCError.FSUIPC_ERR_NODATA
                    Console.WriteLine("No hay datos disponibles en FSUIPC")
                Case FSUIPC.FSUIPCError.FSUIPC_ERR_TIMEOUT
                    Console.WriteLine("Timeout conectando a FSUIPC")
                Case FSUIPC.FSUIPCError.FSUIPC_ERR_SENDMSG
                    Console.WriteLine("Error enviando mensaje a FSUIPC - Verifica que FSUIPC esté ejecutándose y MSFS conectado")
                Case FSUIPC.FSUIPCError.FSUIPC_ERR_DATA
                    Console.WriteLine("Error en datos de FSUIPC")
                Case FSUIPC.FSUIPCError.FSUIPC_ERR_RUNNING
                    Console.WriteLine("FSUIPC ya está en ejecución")
                Case FSUIPC.FSUIPCError.FSUIPC_ERR_SIZE
                    Console.WriteLine("Error de tamaño en datos de FSUIPC")
                Case Else
                    Console.WriteLine($"Error FSUIPC desconocido: {ex.FSUIPCErrorCode} - {ex.Message}")
            End Select

            ' Asegurar que el indicador visual muestre desconectado
            PConection.Image = My.Resources.Red

        Catch ex As Exception
            ' Otros errores no relacionados con FSUIPC
            Console.WriteLine($"Error general conectando: {ex.Message}")
            PConection.Image = My.Resources.Red
        End Try
    End Sub

    Private Sub FSUIPC_Main_Tick(sender As Object, e As EventArgs)
        Try
            ' Verificar que la conexión sigue activa antes de procesar
            If Not FSUIPC.FSUIPCConnection.IsOpen Then
                Throw New Exception("Conexión FSUIPC perdida")
            End If

            FSUIPC.FSUIPCConnection.Process()
            FSUIPC.FSUIPCConnection.UserInputServices.CheckForInput()

            MSFS_VerticalSpeed = (FSUIPC_VerticalSpeed.Value / 256) * 60.0 * 3.28084

            Texto.Text = MSFS_VerticalSpeed.ToString("F0")
            Label1.Text = Math.Round(GetTrimPercentage(Trim.Value), 2).ToString + "% - (" + Trim.Value.ToString + ")"

        Catch ex As FSUIPC.FSUIPCException
            Console.WriteLine($"Error FSUIPC en bucle principal: {ex.FSUIPCErrorCode} - {ex.Message}")
            HandleFSUIPCDisconnection()

        Catch ex As Exception
            Console.WriteLine($"Error general en bucle principal: {ex.Message}")
            HandleFSUIPCDisconnection()
        End Try
    End Sub

    Private Sub HandleFSUIPCDisconnection()
        ' Manejar desconexión de FSUIPC
        PConection.Image = My.Resources.Red
        Iniciado = False
        Button1.Text = Button1.Text.Replace("ON", "OFF")
        FSUIPC_Main.Stop()
        keyHook.UnregisterKeyPresses()
        FSUIPC_TConnect.Start()

        Console.WriteLine("Conexión FSUIPC perdida - Reintentando...")
    End Sub

#End Region

#Region " Controls "

#Region " PConection Click "

    Private Sub PConection_Click(sender As Object, e As EventArgs) Handles PConection.Click
        ' Al hacer click en el indicador de conexión, forzar reconexión
        If Not FSUIPC.FSUIPCConnection.IsOpen Then
            Console.WriteLine("Intentando reconectar manualmente...")
            ForceReconnect()
        Else
            Console.WriteLine("FSUIPC ya está conectado")
        End If
    End Sub

#End Region

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
        If FSUIPC.FSUIPCConnection.IsOpen = False Then
            Console.WriteLine("No se puede activar: FSUIPC no conectado")
            Exit Sub
        End If

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

#Region " Check Administrator Privileges "

    Private Sub CheckAdministratorPrivileges()
        Try
            Dim identity As Security.Principal.WindowsIdentity = Security.Principal.WindowsIdentity.GetCurrent()
            Dim principal As New Security.Principal.WindowsPrincipal(identity)

            If principal.IsInRole(Security.Principal.WindowsBuiltInRole.Administrator) Then
                Console.WriteLine("✓ Aplicación ejecutándose como Administrador")
            Else
                Console.WriteLine("⚠ ADVERTENCIA: La aplicación NO se está ejecutando como Administrador")
                Console.WriteLine("  Esto puede causar problemas de conexión con FSUIPC")
                Console.WriteLine("  Reinicia la aplicación como Administrador")

                ' Opcional: Mostrar un mensaje al usuario
                MessageBox.Show("ADVERTENCIA: La aplicación no se está ejecutando como Administrador." & vbCrLf &
                              "Esto puede causar problemas de conexión con FSUIPC." & vbCrLf & vbCrLf &
                              "Se recomienda reiniciar la aplicación como Administrador.",
                              "Permisos Insuficientes",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning)
            End If
        Catch ex As Exception
            Console.WriteLine($"Error verificando permisos de administrador: {ex.Message}")
        End Try
    End Sub

#End Region

#Region " Public Methods for Hook "

    Public Sub SetTrimZero()
        If Not FSUIPC.FSUIPCConnection.IsOpen Then
            Console.WriteLine("No se puede activar Trim-Zero: FSUIPC no conectado")
            Return
        End If

        Mode = "TrimZero"
        Final_VerticalSpeed = 0
        TTexto.Text = "0"
        PIDController2.SetPoint = Final_VerticalSpeed
        Button1.Text = "Trim-Zero ON"
        If Iniciado = False Then Iniciado = True : Iniciar()
    End Sub

    Public Sub SetTrimAuto()
        If Not FSUIPC.FSUIPCConnection.IsOpen Then
            Console.WriteLine("No se puede activar Trim-Auto: FSUIPC no conectado")
            Return
        End If

        Mode = "TrimAuto"
        Final_VerticalSpeed = MSFS_VerticalSpeed
        TTexto.Text = Math.Round(Final_VerticalSpeed).ToString()
        PIDController2.SetPoint = Final_VerticalSpeed
        Button1.Text = "Trim-Auto ON"
        If Iniciado = False Then Iniciado = True : Iniciar()
    End Sub

    Public Sub StopTrim()
        Iniciado = False
        If Mode = "TrimZero" Then
            Button1.Text = "Trim-Zero OFF"
        Else
            Button1.Text = "Trim-Auto OFF"
        End If
    End Sub

    Public ReadOnly Property IsStarted() As Boolean
        Get
            Return Iniciado
        End Get
    End Property

    Public ReadOnly Property CurrentMode() As String
        Get
            Return Mode
        End Get
    End Property

    Public ReadOnly Property IsConnectedToFSUIPC() As Boolean
        Get
            Return FSUIPC.FSUIPCConnection.IsOpen
        End Get
    End Property

    Public Sub ForceReconnect()
        ' Forzar reconexión a FSUIPC
        Console.WriteLine("Forzando reconexión a FSUIPC...")
        FSUIPC_Main.Stop()
        keyHook.UnregisterKeyPresses()
        FSUIPC_TConnect.Start()
    End Sub

#End Region

#Region " Iniciar "

    Private Sub Iniciar2()
        FSUIPC.FSUIPCConnection.Process()
        TrimFinal = Trim.Value
        PIDController.SetPoint = Final_VerticalSpeed
        PIDController.ProcessVariable = MSFS_VerticalSpeed

        While Iniciado = True
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
