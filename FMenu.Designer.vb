<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FMenu
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FMenu))
        Me.HTexto = New System.Windows.Forms.TextBox()
        Me.TTexto = New System.Windows.Forms.TextBox()
        Me.LSFuel = New System.Windows.Forms.Label()
        Me.Texto = New System.Windows.Forms.Label()
        Me.PConection = New System.Windows.Forms.PictureBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.LDiff = New System.Windows.Forms.Label()
        Me.TDiff = New System.Windows.Forms.Label()
        Me.trackVelocidad = New System.Windows.Forms.TrackBar()
        Me.txtLog = New System.Windows.Forms.TextBox()
        Me.lblEstado = New System.Windows.Forms.Label()
        Me.cmbPuertos = New System.Windows.Forms.ComboBox()
        Me.txtControl = New System.Windows.Forms.TextBox()
        Me.txtSpeed = New System.Windows.Forms.TextBox()
        Me.btnDesconectar = New AutoTrim.BonfireButton()
        Me.btnTrimDown = New AutoTrim.BonfireButton()
        Me.btnTrimUp = New AutoTrim.BonfireButton()
        Me.btnTrimDownContinuous = New AutoTrim.BonfireButton()
        Me.btnTrimUpContinuous = New AutoTrim.BonfireButton()
        Me.btnConectar = New AutoTrim.BonfireButton()
        CType(Me.PConection, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackVelocidad, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'HTexto
        '
        Me.HTexto.Location = New System.Drawing.Point(-77, 5)
        Me.HTexto.Name = "HTexto"
        Me.HTexto.ReadOnly = True
        Me.HTexto.Size = New System.Drawing.Size(52, 20)
        Me.HTexto.TabIndex = 0
        '
        'TTexto
        '
        Me.TTexto.BackColor = System.Drawing.Color.FromArgb(CType(CType(75, Byte), Integer), CType(CType(75, Byte), Integer), CType(CType(75, Byte), Integer))
        Me.TTexto.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TTexto.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold)
        Me.TTexto.ForeColor = System.Drawing.Color.MediumTurquoise
        Me.TTexto.Location = New System.Drawing.Point(161, 39)
        Me.TTexto.MaxLength = 7
        Me.TTexto.Name = "TTexto"
        Me.TTexto.ShortcutsEnabled = False
        Me.TTexto.Size = New System.Drawing.Size(39, 15)
        Me.TTexto.TabIndex = 84
        Me.TTexto.Text = "0"
        Me.TTexto.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'LSFuel
        '
        Me.LSFuel.AutoSize = True
        Me.LSFuel.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LSFuel.ForeColor = System.Drawing.Color.MediumTurquoise
        Me.LSFuel.Location = New System.Drawing.Point(59, 14)
        Me.LSFuel.Name = "LSFuel"
        Me.LSFuel.Size = New System.Drawing.Size(95, 16)
        Me.LSFuel.TabIndex = 83
        Me.LSFuel.Text = "Vertical Speed:"
        Me.LSFuel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Texto
        '
        Me.Texto.Cursor = System.Windows.Forms.Cursors.Default
        Me.Texto.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Texto.ForeColor = System.Drawing.Color.FromArgb(CType(CType(222, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(230, Byte), Integer))
        Me.Texto.Location = New System.Drawing.Point(158, 14)
        Me.Texto.Name = "Texto"
        Me.Texto.Size = New System.Drawing.Size(47, 16)
        Me.Texto.TabIndex = 82
        Me.Texto.Text = "0"
        Me.Texto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PConection
        '
        Me.PConection.Cursor = System.Windows.Forms.Cursors.Default
        Me.PConection.Image = CType(resources.GetObject("PConection.Image"), System.Drawing.Image)
        Me.PConection.Location = New System.Drawing.Point(12, 12)
        Me.PConection.Name = "PConection"
        Me.PConection.Size = New System.Drawing.Size(12, 12)
        Me.PConection.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PConection.TabIndex = 85
        Me.PConection.TabStop = False
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(27, 64)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(207, 36)
        Me.Button1.TabIndex = 86
        Me.Button1.Text = "Trim-Zero OFF"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Label1.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(222, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(230, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(124, 117)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(71, 16)
        Me.Label1.TabIndex = 82
        Me.Label1.Text = "0,00% - (0)"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.MediumTurquoise
        Me.Label2.Location = New System.Drawing.Point(12, 38)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(142, 16)
        Me.Label2.TabIndex = 83
        Me.Label2.Text = "Vertical Speed (Target):"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.MediumTurquoise
        Me.Label3.Location = New System.Drawing.Point(45, 117)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(72, 16)
        Me.Label3.TabIndex = 83
        Me.Label3.Text = "Trim Value:"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label4.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.FromArgb(CType(CType(222, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(230, Byte), Integer))
        Me.Label4.Location = New System.Drawing.Point(205, 38)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(28, 16)
        Me.Label4.TabIndex = 82
        Me.Label4.Text = "fpm"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label5.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.FromArgb(CType(CType(222, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(230, Byte), Integer))
        Me.Label5.Location = New System.Drawing.Point(204, 14)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(28, 16)
        Me.Label5.TabIndex = 82
        Me.Label5.Text = "fpm"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LDiff
        '
        Me.LDiff.AutoSize = True
        Me.LDiff.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LDiff.ForeColor = System.Drawing.Color.MediumTurquoise
        Me.LDiff.Location = New System.Drawing.Point(88, 142)
        Me.LDiff.Name = "LDiff"
        Me.LDiff.Size = New System.Drawing.Size(29, 16)
        Me.LDiff.TabIndex = 83
        Me.LDiff.Text = "Diff:"
        Me.LDiff.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TDiff
        '
        Me.TDiff.AutoSize = True
        Me.TDiff.Cursor = System.Windows.Forms.Cursors.Hand
        Me.TDiff.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TDiff.ForeColor = System.Drawing.Color.FromArgb(CType(CType(222, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(230, Byte), Integer))
        Me.TDiff.Location = New System.Drawing.Point(124, 142)
        Me.TDiff.Name = "TDiff"
        Me.TDiff.Size = New System.Drawing.Size(14, 16)
        Me.TDiff.TabIndex = 82
        Me.TDiff.Text = "0"
        Me.TDiff.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'trackVelocidad
        '
        Me.trackVelocidad.LargeChange = 50
        Me.trackVelocidad.Location = New System.Drawing.Point(308, 143)
        Me.trackVelocidad.Maximum = 700
        Me.trackVelocidad.Minimum = 30
        Me.trackVelocidad.Name = "trackVelocidad"
        Me.trackVelocidad.Size = New System.Drawing.Size(228, 45)
        Me.trackVelocidad.SmallChange = 10
        Me.trackVelocidad.TabIndex = 88
        Me.trackVelocidad.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trackVelocidad.Value = 100
        '
        'txtLog
        '
        Me.txtLog.Location = New System.Drawing.Point(308, 175)
        Me.txtLog.Multiline = True
        Me.txtLog.Name = "txtLog"
        Me.txtLog.Size = New System.Drawing.Size(228, 89)
        Me.txtLog.TabIndex = 89
        '
        'lblEstado
        '
        Me.lblEstado.ForeColor = System.Drawing.Color.DarkRed
        Me.lblEstado.Location = New System.Drawing.Point(425, 12)
        Me.lblEstado.Name = "lblEstado"
        Me.lblEstado.Size = New System.Drawing.Size(111, 13)
        Me.lblEstado.TabIndex = 90
        Me.lblEstado.Text = "Disconnected"
        Me.lblEstado.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'cmbPuertos
        '
        Me.cmbPuertos.FormattingEnabled = True
        Me.cmbPuertos.Location = New System.Drawing.Point(308, 9)
        Me.cmbPuertos.Name = "cmbPuertos"
        Me.cmbPuertos.Size = New System.Drawing.Size(111, 21)
        Me.cmbPuertos.TabIndex = 91
        '
        'txtControl
        '
        Me.txtControl.Location = New System.Drawing.Point(384, 116)
        Me.txtControl.Name = "txtControl"
        Me.txtControl.Size = New System.Drawing.Size(76, 20)
        Me.txtControl.TabIndex = 92
        Me.txtControl.Text = "1950"
        Me.txtControl.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtSpeed
        '
        Me.txtSpeed.Location = New System.Drawing.Point(207, 175)
        Me.txtSpeed.Name = "txtSpeed"
        Me.txtSpeed.Size = New System.Drawing.Size(76, 20)
        Me.txtSpeed.TabIndex = 92
        Me.txtSpeed.Text = "30"
        Me.txtSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'btnDesconectar
        '
        Me.btnDesconectar.ButtonStyle = AutoTrim.BonfireButton.Style.Blue
        Me.btnDesconectar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnDesconectar.Font = New System.Drawing.Font("Verdana", 8.0!)
        Me.btnDesconectar.Image = Nothing
        Me.btnDesconectar.Location = New System.Drawing.Point(425, 36)
        Me.btnDesconectar.Name = "btnDesconectar"
        Me.btnDesconectar.RoundedCorners = False
        Me.btnDesconectar.Size = New System.Drawing.Size(111, 34)
        Me.btnDesconectar.TabIndex = 87
        Me.btnDesconectar.Text = "Disconnect"
        '
        'btnTrimDown
        '
        Me.btnTrimDown.ButtonStyle = AutoTrim.BonfireButton.Style.Blue
        Me.btnTrimDown.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnTrimDown.Font = New System.Drawing.Font("Verdana", 8.0!)
        Me.btnTrimDown.Image = Nothing
        Me.btnTrimDown.Location = New System.Drawing.Point(466, 116)
        Me.btnTrimDown.Name = "btnTrimDown"
        Me.btnTrimDown.RoundedCorners = False
        Me.btnTrimDown.Size = New System.Drawing.Size(70, 21)
        Me.btnTrimDown.TabIndex = 87
        Me.btnTrimDown.Text = "Trim Down"
        '
        'btnTrimUp
        '
        Me.btnTrimUp.ButtonStyle = AutoTrim.BonfireButton.Style.Blue
        Me.btnTrimUp.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnTrimUp.Font = New System.Drawing.Font("Verdana", 8.0!)
        Me.btnTrimUp.Image = Nothing
        Me.btnTrimUp.Location = New System.Drawing.Point(308, 116)
        Me.btnTrimUp.Name = "btnTrimUp"
        Me.btnTrimUp.RoundedCorners = False
        Me.btnTrimUp.Size = New System.Drawing.Size(70, 21)
        Me.btnTrimUp.TabIndex = 87
        Me.btnTrimUp.Text = "Trim Up"
        '
        'btnTrimDownContinuous
        '
        Me.btnTrimDownContinuous.ButtonStyle = AutoTrim.BonfireButton.Style.Blue
        Me.btnTrimDownContinuous.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnTrimDownContinuous.Font = New System.Drawing.Font("Verdana", 8.0!)
        Me.btnTrimDownContinuous.Image = Nothing
        Me.btnTrimDownContinuous.Location = New System.Drawing.Point(425, 76)
        Me.btnTrimDownContinuous.Name = "btnTrimDownContinuous"
        Me.btnTrimDownContinuous.RoundedCorners = False
        Me.btnTrimDownContinuous.Size = New System.Drawing.Size(111, 34)
        Me.btnTrimDownContinuous.TabIndex = 87
        Me.btnTrimDownContinuous.Text = "Trim Down Cont."
        '
        'btnTrimUpContinuous
        '
        Me.btnTrimUpContinuous.ButtonStyle = AutoTrim.BonfireButton.Style.Blue
        Me.btnTrimUpContinuous.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnTrimUpContinuous.Font = New System.Drawing.Font("Verdana", 8.0!)
        Me.btnTrimUpContinuous.Image = Nothing
        Me.btnTrimUpContinuous.Location = New System.Drawing.Point(308, 76)
        Me.btnTrimUpContinuous.Name = "btnTrimUpContinuous"
        Me.btnTrimUpContinuous.RoundedCorners = False
        Me.btnTrimUpContinuous.Size = New System.Drawing.Size(111, 34)
        Me.btnTrimUpContinuous.TabIndex = 87
        Me.btnTrimUpContinuous.Text = "Trim Up Cont."
        '
        'btnConectar
        '
        Me.btnConectar.ButtonStyle = AutoTrim.BonfireButton.Style.Blue
        Me.btnConectar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnConectar.Font = New System.Drawing.Font("Verdana", 8.0!)
        Me.btnConectar.Image = Nothing
        Me.btnConectar.Location = New System.Drawing.Point(308, 36)
        Me.btnConectar.Name = "btnConectar"
        Me.btnConectar.RoundedCorners = False
        Me.btnConectar.Size = New System.Drawing.Size(111, 34)
        Me.btnConectar.TabIndex = 87
        Me.btnConectar.Text = "Connect"
        '
        'FMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(75, Byte), Integer), CType(CType(75, Byte), Integer), CType(CType(75, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(548, 276)
        Me.Controls.Add(Me.txtSpeed)
        Me.Controls.Add(Me.txtControl)
        Me.Controls.Add(Me.cmbPuertos)
        Me.Controls.Add(Me.lblEstado)
        Me.Controls.Add(Me.txtLog)
        Me.Controls.Add(Me.trackVelocidad)
        Me.Controls.Add(Me.btnDesconectar)
        Me.Controls.Add(Me.btnTrimDown)
        Me.Controls.Add(Me.btnTrimUp)
        Me.Controls.Add(Me.btnTrimDownContinuous)
        Me.Controls.Add(Me.btnTrimUpContinuous)
        Me.Controls.Add(Me.btnConectar)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.PConection)
        Me.Controls.Add(Me.TDiff)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Texto)
        Me.Controls.Add(Me.LDiff)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.LSFuel)
        Me.Controls.Add(Me.TTexto)
        Me.Controls.Add(Me.HTexto)
        Me.Controls.Add(Me.Label2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimumSize = New System.Drawing.Size(272, 155)
        Me.Name = "FMenu"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "AutoTrim"
        Me.TopMost = True
        CType(Me.PConection, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackVelocidad, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents HTexto As TextBox
    Friend WithEvents TTexto As TextBox
    Friend WithEvents LSFuel As Label
    Friend WithEvents Texto As Label
    Friend WithEvents PConection As PictureBox
    Friend WithEvents Button1 As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents LDiff As Label
    Friend WithEvents TDiff As Label
    Friend WithEvents btnConectar As BonfireButton
    Friend WithEvents btnDesconectar As BonfireButton
    Friend WithEvents btnTrimUpContinuous As BonfireButton
    Friend WithEvents btnTrimDownContinuous As BonfireButton
    Friend WithEvents trackVelocidad As TrackBar
    Friend WithEvents txtLog As TextBox
    Friend WithEvents lblEstado As Label
    Friend WithEvents cmbPuertos As ComboBox
    Friend WithEvents btnTrimUp As BonfireButton
    Friend WithEvents txtControl As TextBox
    Friend WithEvents btnTrimDown As BonfireButton
    Friend WithEvents txtSpeed As TextBox
End Class
