Imports System.Diagnostics.Process
Imports System.IO


Public Class GdbInterface
    Private gdb_path As String
    Private bin_path As String
    Public gdbProc As Process
    Public OutputBuffer As String = String.Empty
    Public Event OutputReceived(sender As Object, e As DataReceivedEventArgs)

    Public Sub New()

    End Sub
    Public Sub New(gdbPath As String, binPath As String)
        gdb_path = gdbPath
        bin_path = binPath
    End Sub
    Public Property GdbPath As String
        Get
            Return gdb_path
        End Get
        Set(value As String)
            gdb_path = value
        End Set
    End Property

    Public Property BinPath As String
        Get
            Return bin_path
        End Get
        Set(value As String)
            bin_path = value
        End Set
    End Property

    Public Sub GdbOutputReceived(sender As Object, e As DataReceivedEventArgs)
        RaiseEvent OutputReceived(sender, e)
        OutputBuffer += e.Data + vbCrLf

    End Sub

    Public Sub ClearBuffer()
        OutputBuffer = ""
    End Sub
    Public Function RunGdb(Optional ByVal sync As Boolean = False) As String
        Try

            gdbProc = New Process
            With gdbProc
                .StartInfo.FileName = gdb_path
                .StartInfo.Arguments = bin_path.Replace("\", "/")
                .StartInfo.UseShellExecute = False
                .StartInfo.RedirectStandardInput = True
                .StartInfo.RedirectStandardOutput = True
                .StartInfo.CreateNoWindow = True

            End With
            AddHandler gdbProc.OutputDataReceived, AddressOf GdbOutputReceived

            gdbProc.Start()

            If Not sync Then
                gdbProc.BeginOutputReadLine()
            End If

            gdb_pid = gdbProc.Id

        Catch ex As Exception
            MsgBox(ex.Message)
            Return ex.Message
        End Try
        Threading.Thread.Sleep(1)
        ClearBuffer()
    End Function

    Public Sub SendInput(ByVal data As String)

        Dim inputStream As StreamWriter = gdbProc.StandardInput
        inputStream.WriteLine(data)
    End Sub

    Public Function SendCommand(ByVal data As String) As String
        Dim inputStream As StreamWriter = gdbProc.StandardInput
        inputStream.WriteLine(data)
        gdbProc.CancelOutputRead()
        Dim output As String = String.Empty
        While gdbProc.StandardOutput.Peek() > -1
            output += gdbProc.StandardOutput.ReadLine()
        End While
        Return output
    End Function

    Public Sub SendInputAndWait(ByVal commandList As String())
        SendInput("set disassembly-flavor intel")
        For Each command_ In commandList
            SendInput(command_)
        Next
        SendInput("quit")
        While gdbProc.HasExited = False
            Application.DoEvents()
        End While
        System.Threading.Thread.Sleep(10)
    End Sub
End Class
