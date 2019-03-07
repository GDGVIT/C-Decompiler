
Imports System.IO
Imports Newtonsoft.Json

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
        Dim varlist As List(Of SymbolTable.CVariable) = asmp.GetVariables(list, func)

        MsgBox($"variables: {varlist.Count()}")
        Dim exp As New ExpressionParser(list, list(0), list(list.Count() - 1))
        Dim ops = exp.GenerateOperations()



        Dim engine As New CodeEngine
        engine.AsmParser = asmp
        engine.scope = func

        engine.SetUpTemporarySourceFileDirectory("decompilation_c")
        Dim block_code As String = engine.ConvertAbstractToC(ops)
        block_code = Environment.NewLine & $"void {func.Name}" & "{" & block_code & Environment.NewLine & "}"






        My.Computer.FileSystem.WriteAllText($"decompilation_c\{func.Name}.c", block_code, False)
        engine.ViewSourceFile($"decompilation_c\{func.Name}.c")





        Dim wlist As List(Of PseudoCodeModel.LoopStatement) = asmp.ParseLoopStatements(list)
        rtbUpdate($"The function '{cell.Value}' has {wlist.Count()} Loop(s) " & vbNewLine)
        For Each lps In wlist
            If lps.IsEntryControlled Then
                rtbUpdate("WHILE/FOR LOOP: " & vbNewLine & "StartLine: " & lps.StartLine.Code & vbNewLine & "EndLine: " & lps.EndLine.Code & vbNewLine)
            Else
                rtbUpdate("DO-WHILE LOOP: " & vbNewLine & "StartLine: " & lps.StartLine.Code & vbNewLine & "EndLine: " & lps.EndLine.Code & vbNewLine)
            End If

        Next



        Dim listx As List(Of AssemblyInterpretationModel.DecisionBlock) = asmt.SortDecisionBlocks(asmt.GenerateDecisionBlocks(asmt.GenerateConditionalJumpLines(list), list))
        'listx = asmt.SortDecisionBlocks(listx)




        listx = asmt.ArrangeDecisionBlocks(listx)


        Dim listSw As List(Of PseudoCodeModel.SwitchCaseLadder) = asmp.ParseSwitchCases(listx)




        For Each block In listx
            rtbUpdate("Decision Block: ")
            rtbUpdate(block.StartLine.Code)
            rtbUpdate(block.StartLine.Address)
            rtbUpdate("managed content: ")
            rtbUpdate(block.ManagedContent.Count())
        Next


        listx = asmt.AddNonTrivialDecisionBlocks(list, listx)
        Dim mlist As List(Of PseudoCodeModel.DecisionLadder) = asmp.ParseDecisionLadders(list, listx)
        rtbUpdate("<<< ...Added non-trivial decision blocks... >>>")
        For Each block In mlist
            rtbUpdate("Decision Statement: ")
            For Each x In block.DecisionStatements
                rtbUpdate(x.StartLine.Code)
                rtbUpdate(x.StartLine.Address)
                rtbUpdate("managed and parsed content: ")
                rtbUpdate(x.Content.Count())
            Next

        Next




        lvObject.Items.Clear()
        For Each block In wlist
            Dim item As New ListViewItem

            item.BackColor = Color.MediumPurple
            item.ForeColor = Color.White
            If block.IsEntryControlled Then
                item.Text = " While / For Loop - " & block.StartLine.Address
            Else
                item.Text = " Do-while Loop - " & block.StartLine.Address
                item.BackColor = Color.OrangeRed
            End If

            lvObject.Items.Add(item)
        Next
        For Each block In listx
            Dim item As New ListViewItem
            item.BackColor = Color.Teal
            item.ForeColor = Color.White
            item.Text = "Decision Block - " & block.StartLine.Address & "   Managed Content Count - " & block.ManagedContent.Count() & "EndAddress - " & block.EndLine.Address

            lvObject.Items.Add(item)
        Next
        tree.Nodes.Clear()
        Dim rootnode As New TreeNode With {.Text = $"{func.Name} (decision-block-tree)", .ForeColor = Color.Teal}
        For Each block In listx
            Dim node As New TreeNode With {.Text = block.StartLine.Address}
            rootnode.Nodes.Add(PopulateTreeViewWithDecisionObjects(block, node).Nodes(0))
        Next
        tree.Nodes.Add(rootnode)
        tree.ShowNodeToolTips = True
    End Sub
    Public Function PopulateTreeViewWithDecisionObjects(ByVal root As AssemblyInterpretationModel.DecisionBlock, rnode As TreeNode, Optional dr As Integer = 0) As TreeNode
        Dim node As New TreeNode With {.ForeColor = Color.Green}
        For Each elem In root.ManagedContent
            PopulateTreeViewWithDecisionObjects(elem, node)
        Next
        node.Text = $"{root.StartLine.Address}({root.ManagedContent.Count()})"
        node.ToolTipText = $"Block-Type: {GetBlockType(root.BlockType)}"

        rnode.Nodes.Add(node)
        Return rnode
    End Function



    Public Function GetBlockType(blocktype As AssemblyInterpretationModel.DecisionBlockType) As String
        Select Case blocktype
            Case AssemblyInterpretationModel.DecisionBlockType.Independent
                Return "Independent"
            Case AssemblyInterpretationModel.DecisionBlockType.IndependentElse
                Return "Independent-Else"
            Case AssemblyInterpretationModel.DecisionBlockType.Member
                Return "Member"
        End Select
        Return "NaN"
    End Function

    Private Sub btnTest_Click(sender As Object, e As EventArgs) Handles btnTest.Click


    End Sub

    Private Sub lvObject_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvObject.SelectedIndexChanged

    End Sub
End Class
