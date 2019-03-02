Imports dCompiler.AssemblyInterpretationModel
Imports dCompiler.PseudoCodeModel
Imports dCompiler.ExpressionModel
Imports dCompiler.SymbolTable


Public Class CodeEngine

    Public Sub ViewSourceFile(ByVal filename As String)
        If My.Computer.FileSystem.FileExists(filename) = False Then
            Throw New InvalidOperationException("The file doesn't exist.")
            Exit Sub
        End If
        If Not filename.EndsWith(".c") Then
            Exit Sub
        End If
        Dim proc As New Process
        proc.StartInfo.FileName = filename
        proc.StartInfo.WindowStyle = ProcessWindowStyle.Maximized
        proc.Start()

    End Sub


    Public Sub SetUpTemporarySourceFileDirectory(ByVal directory As String)
        If My.Computer.FileSystem.DirectoryExists(directory) Then
            Throw New InvalidOperationException("The directory provided to the CodeEngine already exists")
            Exit Sub
        End If
        My.Computer.FileSystem.CreateDirectory(directory)

    End Sub
End Class
