
Public Class ConfigManager

#Region " Variables "

    Private ConfigFilePath As String
    Private ConfigData As New Dictionary(Of String, Dictionary(Of String, String))

    ' PID
    Public Property PID_Kp As Double = 1.5
    Public Property PID_Ki As Double = 0.3
    Public Property PID_Kd As Double = 0.1
    Public Property PID_UpdateRate As Integer = 20
    Public Property PID_MaxTrimRate As Integer = 50

    ' VerticalSpeed
    Public Property VS_Increment As Integer = 100
    Public Property VS_Deadband As Integer = 10
    Public Property VS_Max As Integer = 2000
    Public Property VS_Min As Integer = -2000

    ' Arduino
    Public Property Arduino_Port As String = "AUTO"
    Public Property Arduino_Baudrate As Integer = 9600
    Public Property Arduino_Timeout As Integer = 1000
    Public Property Arduino_MotorStepsPerRev As Integer = 200
    Public Property Arduino_TrimWheelRatio As Double = 1.0

    ' Hotkeys
    Public Property Hotkey_ToggleKey As String = "0x54"
    Public Property Hotkey_VSUpKey As String = "0x26"
    Public Property Hotkey_VSDownKey As String = "0x28"
    Public Property Hotkey_HoldVSKey As String = "0x48"
    Public Property Hotkey_LevelKey As String = "0x4C"
    Public Property Hotkey_Modifiers As String = "MOD_CONTROL|MOD_ALT"

    ' FSUIPC
    Public Property FSUIPC_ConnectionTimeout As Integer = 5000
    Public Property FSUIPC_RetryInterval As Integer = 1000
    Public Property FSUIPC_OffsetsUpdateRate As Integer = 50

    ' Window
    Public Property Window_X As Integer = 272
    Public Property Window_Y As Integer = 216
    Public Property Start_Minimized As Boolean = False

#End Region

#Region " Constructor "

    Public Sub New()
        ConfigFilePath = IO.Path.Combine(Application.StartupPath, "AutoTrim.ini")
        LoadConfiguration()
    End Sub

    Public Sub New(configPath As String)
        ConfigFilePath = configPath
        LoadConfiguration()
    End Sub

#End Region

#Region " Load "

    Public Sub LoadConfiguration()
        Try
            If (Not IO.File.Exists(ConfigFilePath)) Then SaveConfiguration(Window_X, Window_Y) : Return

            ConfigData.Clear()
            Dim currentSection As String = ""

            For Each line As String In IO.File.ReadAllLines(ConfigFilePath, Text.Encoding.UTF8)
                Dim trimmedLine As String = line.Trim()

                If (String.IsNullOrEmpty(trimmedLine) OrElse trimmedLine.StartsWith("#") OrElse trimmedLine.StartsWith(";")) Then Continue For

                If (trimmedLine.StartsWith("[") AndAlso trimmedLine.EndsWith("]")) Then
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2).ToUpper()
                    If (Not ConfigData.ContainsKey(currentSection)) Then ConfigData(currentSection) = New Dictionary(Of String, String)
                    Continue For
                End If

                Dim equalIndex As Integer = trimmedLine.IndexOf("="c)
                If (equalIndex > 0 AndAlso Not String.IsNullOrEmpty(currentSection)) Then
                    Dim key As String = trimmedLine.Substring(0, equalIndex).Trim().ToUpper()
                    Dim value As String = trimmedLine.Substring(equalIndex + 1).Trim()
                    ConfigData(currentSection)(key) = value
                End If
            Next

            Try
                ' PID
                If (ConfigData.ContainsKey("PID")) Then
                    Dim pidSection = ConfigData("PID")
                    If (pidSection.ContainsKey("KP")) Then Double.TryParse(pidSection("KP"), PID_Kp)
                    If (pidSection.ContainsKey("KI")) Then Double.TryParse(pidSection("KI"), PID_Ki)
                    If (pidSection.ContainsKey("KD")) Then Double.TryParse(pidSection("KD"), PID_Kd)
                    If (pidSection.ContainsKey("UPDATE_RATE")) Then Integer.TryParse(pidSection("UPDATE_RATE"), PID_UpdateRate)
                    If (pidSection.ContainsKey("MAX_TRIM_RATE")) Then Integer.TryParse(pidSection("MAX_TRIM_RATE"), PID_MaxTrimRate)
                End If

                ' VerticalSpeed
                If (ConfigData.ContainsKey("VERTICALSPEED")) Then
                    Dim vsSection = ConfigData("VERTICALSPEED")
                    If (vsSection.ContainsKey("VS_INCREMENT")) Then Integer.TryParse(vsSection("VS_INCREMENT"), VS_Increment)
                    If (vsSection.ContainsKey("VS_DEADBAND")) Then Integer.TryParse(vsSection("VS_DEADBAND"), VS_Deadband)
                    If (vsSection.ContainsKey("VS_MAX")) Then Integer.TryParse(vsSection("VS_MAX"), VS_Max)
                    If (vsSection.ContainsKey("VS_MIN")) Then Integer.TryParse(vsSection("VS_MIN"), VS_Min)
                End If

                ' Arduino
                If (ConfigData.ContainsKey("ARDUINO")) Then
                    Dim arduinoSection = ConfigData("ARDUINO")
                    If (arduinoSection.ContainsKey("PORT")) Then Arduino_Port = arduinoSection("PORT")
                    If (arduinoSection.ContainsKey("BAUDRATE")) Then Integer.TryParse(arduinoSection("BAUDRATE"), Arduino_Baudrate)
                    If (arduinoSection.ContainsKey("TIMEOUT")) Then Integer.TryParse(arduinoSection("TIMEOUT"), Arduino_Timeout)
                    If (arduinoSection.ContainsKey("MOTOR_STEPS_PER_REV")) Then Integer.TryParse(arduinoSection("MOTOR_STEPS_PER_REV"), Arduino_MotorStepsPerRev)
                    If (arduinoSection.ContainsKey("TRIM_WHEEL_RATIO")) Then Double.TryParse(arduinoSection("TRIM_WHEEL_RATIO"), Arduino_TrimWheelRatio)
                End If

                ' Hotkeys
                If (ConfigData.ContainsKey("HOTKEYS")) Then
                    Dim hotkeysSection = ConfigData("HOTKEYS")
                    If (hotkeysSection.ContainsKey("TOGGLE_KEY")) Then Hotkey_ToggleKey = hotkeysSection("TOGGLE_KEY")
                    If (hotkeysSection.ContainsKey("VS_UP_KEY")) Then Hotkey_VSUpKey = hotkeysSection("VS_UP_KEY")
                    If (hotkeysSection.ContainsKey("VS_DOWN_KEY")) Then Hotkey_VSDownKey = hotkeysSection("VS_DOWN_KEY")
                    If (hotkeysSection.ContainsKey("HOLD_VS_KEY")) Then Hotkey_HoldVSKey = hotkeysSection("HOLD_VS_KEY")
                    If (hotkeysSection.ContainsKey("LEVEL_KEY")) Then Hotkey_LevelKey = hotkeysSection("LEVEL_KEY")
                    If (hotkeysSection.ContainsKey("MODIFIERS")) Then Hotkey_Modifiers = hotkeysSection("MODIFIERS")
                End If

                ' FSUIPC
                If (ConfigData.ContainsKey("FSUIPC")) Then
                    Dim fsuipcSection = ConfigData("FSUIPC")
                    If (fsuipcSection.ContainsKey("CONNECTION_TIMEOUT")) Then Integer.TryParse(fsuipcSection("CONNECTION_TIMEOUT"), FSUIPC_ConnectionTimeout)
                    If (fsuipcSection.ContainsKey("RETRY_INTERVAL")) Then Integer.TryParse(fsuipcSection("RETRY_INTERVAL"), FSUIPC_RetryInterval)
                    If (fsuipcSection.ContainsKey("OFFSETS_UPDATE_RATE")) Then Integer.TryParse(fsuipcSection("OFFSETS_UPDATE_RATE"), FSUIPC_OffsetsUpdateRate)
                End If

                ' Window
                If (ConfigData.ContainsKey("WINDOW")) Then
                    Dim windowSection = ConfigData("WINDOW")
                    If (windowSection.ContainsKey("X")) Then Integer.TryParse(windowSection("X"), Window_X)
                    If (windowSection.ContainsKey("Y")) Then Integer.TryParse(windowSection("Y"), Window_Y)
                    If (windowSection.ContainsKey("START_MINIMIZED")) Then Start_Minimized = windowSection("START_MINIMIZED") = "true"
                End If
            Catch : End Try
        Catch
            SaveConfiguration(Window_X, Window_Y)
        End Try
    End Sub

#End Region

#Region " Save "

    Public Sub SaveConfiguration(x As Integer, y As Integer)
        Try
            Dim configContent As New Text.StringBuilder()

            ' PID
            configContent.AppendLine("[PID]")
            configContent.AppendLine($"Kp={PID_Kp:F6}")
            configContent.AppendLine($"Ki={PID_Ki:F6}")
            configContent.AppendLine($"Kd={PID_Kd:F6}")
            configContent.AppendLine($"update_rate={PID_UpdateRate}")
            configContent.AppendLine($"max_trim_rate={PID_MaxTrimRate}")
            configContent.AppendLine()

            ' VerticalSpeed
            configContent.AppendLine("[VerticalSpeed]")
            configContent.AppendLine($"vs_increment={VS_Increment}")
            configContent.AppendLine($"vs_deadband={VS_Deadband}")
            configContent.AppendLine($"vs_max={VS_Max}")
            configContent.AppendLine($"vs_min={VS_Min}")
            configContent.AppendLine()

            ' Arduino
            configContent.AppendLine("[Arduino]")
            configContent.AppendLine($"port={Arduino_Port}")
            configContent.AppendLine($"baudrate={Arduino_Baudrate}")
            configContent.AppendLine($"timeout={Arduino_Timeout}")
            configContent.AppendLine($"motor_steps_per_rev={Arduino_MotorStepsPerRev}")
            configContent.AppendLine($"trim_wheel_ratio={Arduino_TrimWheelRatio:F1}")
            configContent.AppendLine()

            ' Hotkeys
            configContent.AppendLine("[Hotkeys]")
            configContent.AppendLine($"toggle_key={Hotkey_ToggleKey}")
            configContent.AppendLine($"vs_up_key={Hotkey_VSUpKey}")
            configContent.AppendLine($"vs_down_key={Hotkey_VSDownKey}")
            configContent.AppendLine($"hold_vs_key={Hotkey_HoldVSKey}")
            configContent.AppendLine($"level_key={Hotkey_LevelKey}")
            configContent.AppendLine($"modifiers={Hotkey_Modifiers}")
            configContent.AppendLine()

            ' FSUIPC
            configContent.AppendLine("[FSUIPC]")
            configContent.AppendLine($"connection_timeout={FSUIPC_ConnectionTimeout}")
            configContent.AppendLine($"retry_interval={FSUIPC_RetryInterval}")
            configContent.AppendLine($"offsets_update_rate={FSUIPC_OffsetsUpdateRate}")
            configContent.AppendLine()

            ' Windows
            configContent.AppendLine("[Window]")
            configContent.AppendLine($"x={x}")
            configContent.AppendLine($"y={y}")
            configContent.AppendLine($"start_minimized={Start_Minimized}")

            IO.File.WriteAllText(ConfigFilePath, configContent.ToString(), Text.Encoding.UTF8)

        Catch : End Try
    End Sub

#End Region

End Class