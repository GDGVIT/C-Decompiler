Imports dCompiler.AssemblyInterpretationModel
Imports dCompiler.PseudoCodeModel
Imports dCompiler.ExpressionModel
Imports dCompiler.SymbolTable


Public Class CodeEngine

    Public AsmParser As AssemblyParser
    Public scope As CFunction

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

            Exit Sub
        End If
        My.Computer.FileSystem.CreateDirectory(directory)

    End Sub

    Public Function ProduceCodeBlocksFromDecisionBlocks(ByVal decisionLadders As List(Of DecisionLadder))



    End Function

    Public Function ProduceExpressions(operations As List(Of Operation)) As List(Of Dictionary(Of String, Expression))
        Dim expressionControlFlow As New ExpressionControlFlow(operations, operations(0), operations(operations.Count - 1))
        expressionControlFlow.Run()
        Return expressionControlFlow.RunTimeFrames
    End Function


    Public Function ConvertAbstractToC(operations As List(Of Operation)) As String
        Dim runtimeframes = ProduceExpressions(operations)
        Dim blockString As String = String.Empty

        Dim antiRegisterIndex As Integer = 0
        For i As Integer = 0 To operations.Count - 1
            Dim operation = operations(i)
            Dim operationType = operation.Operation
            Select Case operationType
                Case OperationType.Assign
                    If IsRegister(operation.LOperand.Name) Then
                        Continue For
                    ElseIf IsRegister(operation.ROperand.Name) Then
                        Dim frame = runtimeframes(antiRegisterIndex)
                        antiRegisterIndex += 1
                        Dim exp = frame(operation.LOperand.Name)


                    Else
                        Dim cvar As CVariable = AsmParser.GetVariable(operation.CodeLine, scope)
                        If cvar.Name = Nothing Then
                            Continue For
                        End If
                        If IsValue(operation.ROperand.Name) Then
                            blockString &= Environment.NewLine & $"int {cvar.Name} = {ConvertHexToLong(operation.ROperand.Name.Trim)};"
                        Else
                            Dim cvar2 As CVariable = AsmParser.GetVariable(operation.ROperand.Name, scope)
                            blockString &= Environment.NewLine & $"int {cvar.Name} = {cvar2.Name};"
                        End If
                    End If

            End Select
        Next
        Return blockString

    End Function


End Class

Public Class CodeBlock
    Public Address As String
    Public Code As String
    Public Content As Object

    Public Function GetAddressValue() As Integer
        Return ConvertHexToLong(Address)
    End Function

    Public Shared Operator >(left As CodeBlock, right As CodeBlock)
        Return left.GetAddressValue() > right.GetAddressValue()

    End Operator

    Public Shared Operator <(left As CodeBlock, right As CodeBlock)
        Return left.GetAddressValue() < right.GetAddressValue()
    End Operator

    Public Shared Operator >=(left As CodeBlock, right As CodeBlock)
        Return (left > right) Or left = right

    End Operator

    Public Shared Operator <=(left As CodeBlock, right As CodeBlock)
        Return (left < right) Or left = right
    End Operator

    Public Shared Operator =(left As CodeBlock, right As CodeBlock)
        Return left.GetAddressValue() = right.GetAddressValue

    End Operator

    Public Shared Operator <>(left As CodeBlock, right As CodeBlock)
        Return left.GetAddressValue <> right.GetAddressValue
    End Operator
End Class
