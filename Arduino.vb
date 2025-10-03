Imports System.IO.Ports

Public Class ControladorMotor
    Private WithEvents SerialPort As SerialPort
    Private _conectado As Boolean = False

    ' Eventos para notificar al formulario
    Public Event MensajeRecibido(mensaje As String)
    Public Event ErrorConexion(mensaje As String)
    Public Event EstadoConexion(conectado As Boolean)

    ' Propiedades
    Public ReadOnly Property Conectado As Boolean
        Get
            Return _conectado AndAlso SerialPort IsNot Nothing AndAlso SerialPort.IsOpen
        End Get
    End Property

    Public Sub New()
        SerialPort = New SerialPort()
        SerialPort.BaudRate = 9600
        SerialPort.DataBits = 8
        SerialPort.Parity = Parity.None
        SerialPort.StopBits = StopBits.One
        SerialPort.ReadTimeout = 500
        SerialPort.WriteTimeout = 500
    End Sub

    ''' <summary>
    ''' Conecta con el Arduino en el puerto especificado
    ''' </summary>
    Public Function Conectar(nombrePuerto As String) As Boolean
        Try
            If SerialPort.IsOpen Then
                SerialPort.Close()
            End If

            SerialPort.PortName = nombrePuerto
            SerialPort.Open()
            _conectado = True
            RaiseEvent EstadoConexion(True)
            Return True

        Catch ex As Exception
            _conectado = False
            RaiseEvent ErrorConexion("Error al conectar: " & ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Desconecta del Arduino
    ''' </summary>
    Public Sub Desconectar()
        Try
            If SerialPort IsNot Nothing AndAlso SerialPort.IsOpen Then
                Try
                    ' Vaciar buffers
                    SerialPort.DiscardInBuffer()
                    SerialPort.DiscardOutBuffer()

                    ' Enviar stop
                    SerialPort.Write("S")
                    System.Threading.Thread.Sleep(100)
                Catch
                    ' Ignorar errores
                End Try

                ' Deshabilitar eventos ANTES de cerrar
                RemoveHandler SerialPort.DataReceived, AddressOf SerialPort_DataReceived

                ' Cerrar en un hilo separado con timeout
                Dim closeTask = Task.Run(Sub()
                                             Try
                                                 SerialPort.Close()
                                             Catch
                                             End Try
                                         End Sub)

                ' Esperar máximo 1 segundo
                If Not closeTask.Wait(1000) Then
                    ' Si no cierra, forzar dispose
                    SerialPort.Dispose()
                    SerialPort = Nothing
                End If
            End If
        Catch
        Finally
            _conectado = False
            RaiseEvent EstadoConexion(False)
        End Try
    End Sub

    ''' <summary>
    ''' Envía un comando al Arduino
    ''' </summary>
    Private Function EnviarComando(comando As String) As Boolean
        Try
            If Not Conectado Then
                RaiseEvent ErrorConexion("No hay conexión con el Arduino")
                Return False
            End If

            SerialPort.WriteLine(comando)
            Return True

        Catch ex As Exception
            RaiseEvent ErrorConexion("Error al enviar comando: " & ex.Message)
            Return False
        End Try
    End Function

    ' === MÉTODOS PÚBLICOS PARA CONTROL DEL MOTOR ===

    ''' <summary>
    ''' Enciende el motor (lo habilita)
    ''' </summary>
    Public Function EncenderMotor() As Boolean
        Return EnviarComando("ON")
    End Function

    ''' <summary>
    ''' Apaga el motor (lo deshabilita, deja de consumir corriente)
    ''' </summary>
    Public Function ApagarMotor() As Boolean
        Return EnviarComando("S")
    End Function

    ''' <summary>
    ''' Gira el motor en sentido horario
    ''' </summary>
    Public Function GirarHorario() As Boolean
        Return EnviarComando("H")
    End Function

    ''' <summary>
    ''' Gira el motor en sentido antihorario
    ''' </summary>
    Public Function GirarAntihorario() As Boolean
        Return EnviarComando("A")
    End Function

    ''' <summary>
    ''' Detiene el movimiento del motor (pero lo mantiene habilitado)
    ''' </summary>
    Public Function DetenerMotor() As Boolean
        Return EnviarComando("S")
    End Function
    Public Function PasosMotor(ByVal comando As String) As Boolean
        Return EnviarComando(comando)
    End Function

    ''' <summary>
    ''' Cambia la velocidad del motor (100-5000 microsegundos)
    ''' Valores más bajos = más rápido, valores más altos = más lento
    ''' </summary>
    Public Function CambiarVelocidad(microsegundos As Integer) As Boolean
        If microsegundos < 100 OrElse microsegundos > 5000 Then
            RaiseEvent ErrorConexion("Velocidad fuera de rango (100-5000)")
            Return False
        End If
        Return EnviarComando("SPEED:" & microsegundos.ToString())
    End Function

    ''' <summary>
    ''' Obtiene lista de puertos COM disponibles
    ''' </summary>
    Public Shared Function ObtenerPuertosDisponibles() As String()
        Return SerialPort.GetPortNames()
    End Function

    ' Evento cuando llegan datos del Arduino
    Private Sub SerialPort_DataReceived(sender As Object, e As SerialDataReceivedEventArgs) Handles SerialPort.DataReceived
        Try
            Dim mensaje As String = SerialPort.ReadLine().Trim()
            RaiseEvent MensajeRecibido(mensaje)
        Catch ex As Exception
            ' Ignorar errores de lectura (timeout, etc)
        End Try
    End Sub

    ' Liberar recursos
    Public Sub Dispose()
        Desconectar()
        If SerialPort IsNot Nothing Then
            SerialPort.Dispose()
        End If
    End Sub
End Class