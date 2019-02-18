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
        Me.btnViewAssembly = New System.Windows.Forms.Button()
        Me.btnTest = New System.Windows.Forms.Button()
        Me.lvObject = New System.Windows.Forms.ListView()
        Me.tree = New System.Windows.Forms.TreeView()
        Me.Panel1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.funcGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rtbAsm
        '
        Me.rtbAsm.Dock = System.Windows.Forms.DockStyle.Left
        Me.rtbAsm.Font = New System.Drawing.Font("Consolas", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rtbAsm.ForeColor = System.Drawing.Color.Teal
        Me.rtbAsm.Location = New System.Drawing.Point(0, 0)
        Me.rtbAsm.Name = "rtbAsm"
        Me.rtbAsm.Size = New System.Drawing.Size(892, 155)
        Me.rtbAsm.TabIndex = 0
        Me.rtbAsm.Text = ""
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.rtbAsm)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 264)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1351, 155)
        Me.Panel1.TabIndex = 1
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FILEToolStripMenuItem, Me.ExitToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1351, 24)
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
        Me.rtbFunctionAsm.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rtbFunctionAsm.ForeColor = System.Drawing.Color.Teal
        Me.rtbFunctionAsm.Location = New System.Drawing.Point(354, 28)
        Me.rtbFunctionAsm.Name = "rtbFunctionAsm"
        Me.rtbFunctionAsm.Size = New System.Drawing.Size(643, 196)
        Me.rtbFunctionAsm.TabIndex = 6
        Me.rtbFunctionAsm.Text = ""
        '
        'btnViewAssembly
        '
        Me.btnViewAssembly.Location = New System.Drawing.Point(147, 230)
        Me.btnViewAssembly.Name = "btnViewAssembly"
        Me.btnViewAssembly.Size = New System.Drawing.Size(89, 28)
        Me.btnViewAssembly.TabIndex = 8
        Me.btnViewAssembly.Text = "View Assembly"
        Me.btnViewAssembly.UseVisualStyleBackColor = True
        '
        'btnTest
        '
        Me.btnTest.Location = New System.Drawing.Point(242, 230)
        Me.btnTest.Name = "btnTest"
        Me.btnTest.Size = New System.Drawing.Size(84, 28)
        Me.btnTest.TabIndex = 9
        Me.btnTest.Text = "test"
        Me.btnTest.UseVisualStyleBackColor = True
        '
        'lvObject
        '
        Me.lvObject.Location = New System.Drawing.Point(1003, 28)
        Me.lvObject.Name = "lvObject"
        Me.lvObject.Size = New System.Drawing.Size(165, 200)
        Me.lvObject.TabIndex = 10
        Me.lvObject.TileSize = New System.Drawing.Size(168, 30)
        Me.lvObject.UseCompatibleStateImageBehavior = False
        Me.lvObject.View = System.Windows.Forms.View.Tile
        '
        'tree
        '
        Me.tree.Location = New System.Drawing.Point(1174, 27)
        Me.tree.Name = "tree"
        Me.tree.Size = New System.Drawing.Size(165, 198)
        Me.tree.TabIndex = 11
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1351, 419)
        Me.Controls.Add(Me.tree)
        Me.Controls.Add(Me.lvObject)
        Me.Controls.Add(Me.btnTest)
        Me.Controls.Add(Me.btnViewAssembly)
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
    Friend WithEvents btnViewAssembly As Button
    Friend WithEvents btnTest As Button
    Friend WithEvents lvObject As ListView
    Friend WithEvents tree As TreeView
End Class
