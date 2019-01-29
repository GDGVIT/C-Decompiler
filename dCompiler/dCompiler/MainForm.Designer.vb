<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.rtbAsm = New System.Windows.Forms.RichTextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FILEToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenBinaryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveGeneratedCodeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.funcGrid = New System.Windows.Forms.DataGridView()
        Me.rtbFunctionAsm = New System.Windows.Forms.RichTextBox()
        Me.RichTextBox2 = New System.Windows.Forms.RichTextBox()
        Me.btnViewAssembly = New System.Windows.Forms.Button()
        Me.Panel1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.funcGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rtbAsm
        '
        Me.rtbAsm.Dock = System.Windows.Forms.DockStyle.Right
        Me.rtbAsm.Location = New System.Drawing.Point(3, 0)
        Me.rtbAsm.Name = "rtbAsm"
        Me.rtbAsm.Size = New System.Drawing.Size(892, 76)
        Me.rtbAsm.TabIndex = 0
        Me.rtbAsm.Text = ""
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.rtbAsm)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 264)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(895, 76)
        Me.Panel1.TabIndex = 1
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FILEToolStripMenuItem, Me.ExitToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(895, 24)
        Me.MenuStrip1.TabIndex = 2
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FILEToolStripMenuItem
        '
        Me.FILEToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenBinaryToolStripMenuItem, Me.SaveGeneratedCodeToolStripMenuItem})
        Me.FILEToolStripMenuItem.Name = "FILEToolStripMenuItem"
        Me.FILEToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.FILEToolStripMenuItem.Text = "FILE"
        '
        'OpenBinaryToolStripMenuItem
        '
        Me.OpenBinaryToolStripMenuItem.Name = "OpenBinaryToolStripMenuItem"
        Me.OpenBinaryToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.OpenBinaryToolStripMenuItem.Text = "Open Binary"
        '
        'SaveGeneratedCodeToolStripMenuItem
        '
        Me.SaveGeneratedCodeToolStripMenuItem.Name = "SaveGeneratedCodeToolStripMenuItem"
        Me.SaveGeneratedCodeToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.SaveGeneratedCodeToolStripMenuItem.Text = "Save Generated Code"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(42, 20)
        Me.ExitToolStripMenuItem.Text = "EXIT"
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(3, 230)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(138, 28)
        Me.Button2.TabIndex = 4
        Me.Button2.Text = "Generate Function Table"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'funcGrid
        '
        Me.funcGrid.AllowUserToAddRows = False
        Me.funcGrid.AllowUserToDeleteRows = False
        Me.funcGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.funcGrid.Location = New System.Drawing.Point(3, 27)
        Me.funcGrid.Name = "funcGrid"
        Me.funcGrid.ReadOnly = True
        Me.funcGrid.Size = New System.Drawing.Size(344, 197)
        Me.funcGrid.TabIndex = 5
        '
        'rtbFunctionAsm
        '
        Me.rtbFunctionAsm.Location = New System.Drawing.Point(354, 28)
        Me.rtbFunctionAsm.Name = "rtbFunctionAsm"
        Me.rtbFunctionAsm.Size = New System.Drawing.Size(299, 195)
        Me.rtbFunctionAsm.TabIndex = 6
        Me.rtbFunctionAsm.Text = ""
        '
        'RichTextBox2
        '
        Me.RichTextBox2.Location = New System.Drawing.Point(659, 27)
        Me.RichTextBox2.Name = "RichTextBox2"
        Me.RichTextBox2.Size = New System.Drawing.Size(224, 195)
        Me.RichTextBox2.TabIndex = 7
        Me.RichTextBox2.Text = ""
        '
        'btnViewAssembly
        '
        Me.btnViewAssembly.Location = New System.Drawing.Point(147, 230)
        Me.btnViewAssembly.Name = "btnViewAssembly"
        Me.btnViewAssembly.Size = New System.Drawing.Size(138, 28)
        Me.btnViewAssembly.TabIndex = 8
        Me.btnViewAssembly.Text = "View Assembly"
        Me.btnViewAssembly.UseVisualStyleBackColor = True
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(895, 340)
        Me.Controls.Add(Me.btnViewAssembly)
        Me.Controls.Add(Me.RichTextBox2)
        Me.Controls.Add(Me.rtbFunctionAsm)
        Me.Controls.Add(Me.funcGrid)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "MainForm"
        Me.Text = "dCompiler v1.0.0"
        Me.Panel1.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.funcGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents rtbAsm As RichTextBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FILEToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenBinaryToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaveGeneratedCodeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Button2 As Button
    Friend WithEvents funcGrid As DataGridView
    Friend WithEvents rtbFunctionAsm As RichTextBox
    Friend WithEvents RichTextBox2 As RichTextBox
    Friend WithEvents btnViewAssembly As Button
End Class
