

Public Class MainForm
    Dim gdbInterface As New GdbInterface()
    Dim gdbOutputBuffer As String = String.Empty
    Dim _startBuffering As Boolean = False
    Dim symProcessor As SymbolProcessor
    Dim funcl As List(Of SymbolTable.CFunction)


    Delegate Sub UpdateRTB(data As String)
    Public dlgRtbUpdate As UpdateRTB = New UpdateRTB(AddressOf rtbUpdate)
    Public Sub rtbUpdate(data As String)
        rtbAsm.Text += data + Environment.NewLine
        'rtbAsm.SelectionStart = rtbAsm.Text.Length
        'rtbAsm.ScrollToCaret()

    End Sub
    Private Sub gdbInterface_OutputReceived(sender As Object, e As DataReceivedEventArgs)
        rtbAsm.Invoke(dlgRtbUpdate, e.Data)
        If _startBuffering Then
            gdbOutputBuffer += e.Data + Environment.NewLine
        Else
            gdbOutputBuffer = ""
        End If
    End Sub
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        End
    End Sub

    Private Sub OpenBinaryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenBinaryToolStripMenuItem.Click
        Dim dialogBox As New OpenFileDialog
        dialogBox.Filter = "Executable Files (*.exe)|*.exe|All files (*.*)|*.*"
        Dim result As DialogResult = dialogBox.ShowDialog()
        If result = DialogResult.Cancel Then Exit Sub
        If dialogBox.FileName IsNot String.Empty Then
            gdbInterface.BinPath = dialogBox.FileName
        End If
        'AddHandler gdbInterface.OutputReceived, AddressOf gdbInterface_OutputReceived
        'gdbInterface.RunGdb()

    End Sub



    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not My.Computer.FileSystem.FileExists(gdb_path_default) Then
            MsgBox("gdb.exe not found..", vbCritical, "Error")
            End
        End If

        gdbInterface.GdbPath = gdb_path_default

    End Sub



    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        If gdbInterface.BinPath = "" Then gdbInterface.BinPath = "E:/main.exe"
        'AddHandler gdbInterface.OutputReceived, AddressOf gdbInterface_OutputReceived
        RemoveHandler gdbInterface.OutputReceived, AddressOf gdbInterface_OutputReceived
        gdbInterface.RunGdb()
        gdbInterface.ClearBuffer()


        Dim symProc As New SymbolProcessor(gdbInterface)
        symProc.CreateSectionCollection()
        symProc.GenerateSchema()
        Dim asmt As New AssemblyTraverser("", symProc)

        'RemoveHandler gdbInterface.OutputReceived, AddressOf gdbInterface_OutputReceived
        asmt.InitializeRawData()
        gdbInterface.RunGdb()
        gdbInterface.ClearBuffer()
        gdbInterface.SendInputAndWait({"disas main"})
        'MsgBox(gdbInterface.OutputBuffer)

        funcl = (New AssemblyParser(gdbInterface.OutputBuffer, asmt)).GetFunctions(gdbInterface.OutputBuffer)
        'MsgBox(funcl.Count)

        Dim dt As New DataTable("Function Table")
        dt.Columns.Add("Function Name")
        dt.Columns.Add("Start Address")
        dt.Columns.Add("End Address")
        For Each elem In funcl
            dt.Rows.Add({elem.Name, elem.StartAddress, elem.EndAddress})
        Next
        funcGrid.DataSource = dt
    End Sub

    Private Sub btnViewAssembly_Click(sender As Object, e As EventArgs) Handles btnViewAssembly.Click
        Dim dsc As DataGridViewSelectedRowCollection = funcGrid.SelectedRows()
        Dim row As DataGridViewRow = dsc.Item(0)
        Dim cell As DataGridViewCell = row.Cells.Item(0)
        MsgBox(cell.Value)
        Dim func As SymbolTable.CFunction = funcl.Find(Function(p) p.Name = cell.Value)
        rtbFunctionAsm.Text = func.RawAssembly

    End Sub
End Class
