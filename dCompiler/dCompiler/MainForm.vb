

Public Class MainForm
    Dim gdbInterface As New GdbInterface()
    Dim gdbOutputBuffer As String = String.Empty
    Dim _startBuffering As Boolean = False
    Dim symProcessor As SymbolProcessor
    Dim funcl As List(Of SymbolTable.CFunction)
    Dim symProc As SymbolProcessor
    Dim asmt As AssemblyTraverser
    Dim asmp As AssemblyParser


    Delegate Sub UpdateRTB(data As String)
    Public dlgRtbUpdate As UpdateRTB = New UpdateRTB(AddressOf rtbUpdate)
    Public Sub rtbUpdate(data As String)
        rtbAsm.Text += data + Environment.NewLine
        rtbAsm.SelectionStart = rtbAsm.Text.Length
        rtbAsm.ScrollToCaret()

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
            gdbInterface.BinPath = """" & dialogBox.FileName & """"
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


        symProc = New SymbolProcessor(gdbInterface)
        symProc.CreateSectionCollection()
        symProc.GenerateSchema()
        asmt = New AssemblyTraverser("", symProc)

        'RemoveHandler gdbInterface.OutputReceived, AddressOf gdbInterface_OutputReceived
        asmt.InitializeRawData()
        gdbInterface.RunGdb()
        gdbInterface.ClearBuffer()
        gdbInterface.SendInputAndWait({"disas main"})
        asmp = New AssemblyParser(gdbInterface.OutputBuffer, asmt)

        funcl = asmp.GetFunctions(gdbInterface.OutputBuffer)







        'Here we are updating the functions to the symbol table
        symProc.GetSymbolTable.CFunctionCollection = funcl
        Dim dt As New DataTable("Function Table")
        dt.Columns.Add("Function Name")
        dt.Columns.Add("Start Address")
        dt.Columns.Add("End Address")
        For Each elem In funcl
            dt.Rows.Add({elem.Name, elem.StartAddress, elem.EndAddress})
            rtbUpdate($"Added function : '{elem.Name}' to DataGrid" & vbNewLine)
        Next
        funcGrid.DataSource = dt
    End Sub

    Private Sub btnViewAssembly_Click(sender As Object, e As EventArgs) Handles btnViewAssembly.Click
        Dim dsc As DataGridViewSelectedRowCollection = funcGrid.SelectedRows()
        Dim row As DataGridViewRow
        Try
            row = dsc.Item(0)
        Catch ex As Exception
            Exit Sub
        End Try

        Dim cell As DataGridViewCell = row.Cells.Item(0)

        Dim func As SymbolTable.CFunction = funcl.Find(Function(p) p.Name = cell.Value)
        rtbFunctionAsm.Text = func.RawAssembly
        Dim list As List(Of PseudoCodeModel.CodeLine) = asmp.GenerateCodeLines(func.RawAssembly)
        Dim wlist As List(Of PseudoCodeModel.LoopStatement) = asmp.ParseLoopStatements(list)
        rtbUpdate($"The function '{cell.Value}' has {wlist.Count()} Loop(s) " & vbNewLine)
        For Each lps In wlist
            If lps.IsEntryControlled Then
                rtbUpdate("WHILE/FOR LOOP: " & vbNewLine & "StartLine: " & lps.StartLine.Code & vbNewLine & "EndLine: " & lps.EndLine.Code & vbNewLine)
            Else
                rtbUpdate("DO-WHILE LOOP: " & vbNewLine & "StartLine: " & lps.StartLine.Code & vbNewLine & "EndLine: " & lps.EndLine.Code & vbNewLine)
            End If

        Next

        Dim ls As List(Of PseudoCodeModel.DecisionLadder) = asmp.ParseDecisionStatements(list)
        For Each dsl In ls
            If dsl.DecisionStatements.Count = 1 Then
                rtbUpdate($"Found if-statement in function '{cell.Value}' : " & vbNewLine & $":::{dsl.StartLine.Code}" & vbNewLine & $":::{dsl.EndLine.Code}")
            Else
                rtbUpdate($"Found if-else / if-else ladder in function '{cell.Value}' : " & vbNewLine & $":::{dsl.StartLine.Code}" & vbNewLine & $":::{dsl.EndLine.Code}" & vbNewLine & $"#Order={dsl.DecisionStatements.Count}")
                For Each ds In dsl.DecisionStatements
                    rtbUpdate($":-->{ds.StartLine.Code}" & vbNewLine & $":-->{ds.EndLine.Code}" & vbNewLine)
                Next
            End If

        Next

        Dim listx As List(Of AssemblyInterpretationModel.DecisionBlock) = asmt.ArrangeDecisionBlocks(asmt.GenerateDecisionBlocks(asmt.GenerateConditionalJumpLines(list), list))
        MsgBox(listx.Count())
    End Sub

    Private Sub btnTest_Click(sender As Object, e As EventArgs) Handles btnTest.Click


    End Sub
End Class
