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

    Public Function ProduceExpressions(operations As List(Of Operation)) As Dictionary(Of String, Dictionary(Of String, Expression))
        Dim expressionControlFlow As New ExpressionControlFlow(operations, operations(0), operations(operations.Count - 1))
        expressionControlFlow.Run()
        Return expressionControlFlow.RunTimeFrames
    End Function

    Public Function EvaluateOperationType(opType As OperationType) As String
        Select Case opType
            Case OperationType.Add
                Return "+"
            Case OperationType.Subtract
                Return "-"
            Case OperationType.Multiply
                Return "*"
            Case OperationType.Divide
                Return "/"
        End Select
    End Function


    Public Function EvaluateExpression(exp As Expression) As String




        If exp.Operands.Count = 0 And exp.OperationTypeList.Count = 0 Then
            Return ""
        End If
        If exp.Operands.Count = 1 And exp.OperationTypeList.Count = 1 Then
            Dim cvar = AsmParser.GetVariable(exp.Operands(0).Name, scope)
            If cvar.Name = Nothing Then
                cvar.Name = exp.Operands(0).Name
            End If
            Return $"{EvaluateOperationType(exp.OperationTypeList(0))}={cvar.Name}"


        Else

            Dim evalStr As String = ""
            For i As Integer = 0 To exp.Operands.Count - 1
                Dim varx_ = exp.Operands(i).Name
                Dim varx = AsmParser.GetVariable(varx_, scope).Name
                If varx = "" Then
                    varx = varx_
                End If
                evalStr = $"({evalStr}{varx})"
                If i < exp.OperationTypeList.Count Then
                    evalStr &= EvaluateOperationType(exp.OperationTypeList(i))
                End If


            Next
            Return evalStr


        End If

    End Function


    Public Function ConvertAbstractToC(operations As List(Of Operation)) As String
        Dim runtimeframes = ProduceExpressions(operations)
        Dim blockString As String = String.Empty


        For i As Integer = 0 To operations.Count - 1
            Dim operation = operations(i)
            Dim operationType = operation.Operation
            Dim assignedVars As New List(Of CVariable)

            Select Case operationType
                Case OperationType.Assign
                    If IsRegister(operation.LOperand.Name) Then
                        Continue For
                    ElseIf IsRegister(operation.ROperand.Name) Then
                        Dim frame = runtimeframes(operation.CodeLine.Address)
                        MsgBox(operation.CodeLine.Address)

                        For Each key In frame
                            MsgBox($"{key.Key}--{key.Value.Operands.Count}--{key.Value.OperationTypeList.Count}")
                        Next





                        Dim cvar As CVariable = AsmParser.GetVariable(operation.LOperand.Name, scope)


                        Dim exp = frame(operation.LOperand.Name)



                        blockString &= Environment.NewLine & $"{cvar.Name} = {EvaluateExpression(exp)};"

                    Else

                        Dim cvar As CVariable = AsmParser.GetVariable(operation.CodeLine, scope)
                        If cvar.Name = Nothing Then
                            Continue For
                        End If
                        assignedVars.Add(cvar)
                        If IsValue(operation.ROperand.Name) Then
                            blockString &= Environment.NewLine & $"{cvar.Name} = {ConvertHexToLong(operation.ROperand.Name.Trim)};"
                        Else
                            Dim cvar2 As CVariable = AsmParser.GetVariable(operation.ROperand.Name, scope)
                            blockString &= Environment.NewLine & $"{cvar.Name} = {cvar2.Name};"
                        End If
                    End If
                Case OperationType.Add
                    If IsRegister(operation.LOperand.Name) Then
                        Continue For
                    ElseIf IsRegister(operation.ROperand.Name) Then
                        Dim frame = runtimeframes(operation.CodeLine.Address)
                        MsgBox(operation.CodeLine.Address)

                        For Each key In frame
                            MsgBox($"{key.Key}--{key.Value.Operands.Count}--{key.Value.OperationTypeList.Count}")
                        Next





                        Dim cvar As CVariable = AsmParser.GetVariable(operation.LOperand.Name, scope)


                        Dim exp = frame(operation.LOperand.Name)



                        blockString &= Environment.NewLine & $"{cvar.Name} = {EvaluateExpression(exp)};"

                    Else

                        Dim cvar As CVariable = AsmParser.GetVariable(operation.CodeLine, scope)
                        If cvar.Name = Nothing Then
                            Continue For
                        End If
                        assignedVars.Add(cvar)
                        If IsValue(operation.ROperand.Name) Then
                            blockString &= Environment.NewLine & $"{cvar.Name} += {ConvertHexToLong(operation.ROperand.Name.Trim)};"
                        Else
                            Dim cvar2 As CVariable = AsmParser.GetVariable(operation.ROperand.Name, scope)
                            blockString &= Environment.NewLine & $"{cvar.Name} += {cvar2.Name};"
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
