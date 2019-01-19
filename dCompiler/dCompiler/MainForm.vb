

Public Class MainForm
    Dim gdbInterface As New GdbInterface()
    Dim gdbOutputBuffer As String = String.Empty
    Dim _startBuffering As Boolean = False
    Dim symProcessor As SymbolProcessor



    Delegate Sub UpdateRTB(data As String)
    Public dlgRtbUpdate As UpdateRTB = New UpdateRTB(AddressOf rtbUpdate)
    Public Sub rtbUpdate(data As String)
        rtbAsm.Text += data + Environment.NewLine
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
        AddHandler gdbInterface.OutputReceived, AddressOf gdbInterface_OutputReceived
        gdbInterface.RunGdb()

    End Sub



    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not My.Computer.FileSystem.FileExists(gdb_path_default) Then
            MsgBox("gdb.exe not found..", vbCritical, "Error")
            End
        End If

        gdbInterface.GdbPath = gdb_path_default

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        symProcessor = New SymbolProcessor(gdbInterface)
        symProcessor.CreateSectionCollection()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        gdbInterface.BinPath = "E:/main.exe"
        AddHandler gdbInterface.OutputReceived, AddressOf gdbInterface_OutputReceived

        gdbInterface.RunGdb()
        gdbInterface.SendInputAndWait({"disas 0x401530,0x401550"})

        'Dim symProc As New SymbolProcessor(gdbInterface)
        'symProc.CreateSectionCollection()

    End Sub
End Class
