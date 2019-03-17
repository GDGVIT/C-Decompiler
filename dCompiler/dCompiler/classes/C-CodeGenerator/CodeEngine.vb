Imports dCompiler.AssemblyInterpretationModel
Imports dCompiler.PseudoCodeModel
Imports dCompiler.ExpressionModel
Imports dCompiler.SymbolTable


Public Class CodeEngine

    Public AsmParser As AssemblyParser
    Public scope As CFunction
    Public AssemblyTraverser As AssemblyTraverser
    Private blockLevel As Integer = 1

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

    Public Function ParseDecisionJumpLineToOperator(ByVal jumpLine As JumpLine) As String
        Select Case jumpLine.JumpCondition
            Case JumpConditionFlag.JumpIfEqual
                Return "!="
            Case JumpConditionFlag.JumpIfGreaterThanOrEqualTo
                Return "<"
            Case JumpConditionFlag.JumpIfLessThanOrEqualTo
                Return ">"
            Case JumpConditionFlag.JumpIfNotEqual
                Return "=="
            Case JumpConditionFlag.JumpIfNotZero
                Return "=="
            Case JumpConditionFlag.JumpIfZero
                Return "!="
            Case JumpConditionFlag.JumpIfGreaterThan
                Return "<="
            Case JumpConditionFlag.JumpIfLesserThan
                Return ">="
            Case Else
                Return String.Empty
        End Select
    End Function


    Public Function ConvertAbstractToC(operations As List(Of Operation)) As String
        Dim runtimeframes = ProduceExpressions(operations)
        Dim blockString As String = String.Empty



        For i As Integer = 0 To operations.Count - 1
            Dim operation = operations(i)
            If operation.LOperand.Name = Nothing Or operation.ROperand.Name = Nothing Then
                MsgBox(operation.CodeLine.Code)
            End If
            Dim operationType = operation.Operation
            Dim assignedVars As New List(Of CVariable)

            If operationType = OperationType.Assign Then

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



                    blockString &= Environment.NewLine & GiveIndentation(blockLevel) & $"{cvar.Name} = {EvaluateExpression(exp)};"

                Else

                    Dim cvar As CVariable = AsmParser.GetVariable(operation.CodeLine, scope)
                    If cvar.Name = Nothing Then
                        Continue For
                    End If
                    assignedVars.Add(cvar)
                    If IsValue(operation.ROperand.Name) Then
                        blockString &= Environment.NewLine & GiveIndentation(blockLevel) & $"{cvar.Name} = {ConvertHexToLong(operation.ROperand.Name.Trim)};"
                    Else
                        Dim cvar2 As CVariable = AsmParser.GetVariable(operation.ROperand.Name, scope)
                        blockString &= Environment.NewLine & GiveIndentation(blockLevel) & $"{cvar.Name} = {cvar2.Name};"
                    End If
                End If
            ElseIf operationType > 0 And operationType < 5 Then
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



                    blockString &= Environment.NewLine & GiveIndentation(blockLevel) & $"{cvar.Name} = {EvaluateExpression(exp)};"

                Else

                    Dim cvar As CVariable = AsmParser.GetVariable(operation.CodeLine, scope)
                    If cvar.Name = Nothing Then
                        Continue For
                    End If
                    assignedVars.Add(cvar)
                    Dim charOperator As Char
                    Select Case operationType
                        Case OperationType.Add
                            charOperator = "+"
                        Case OperationType.Subtract
                            charOperator = "-"
                        Case OperationType.Multiply
                            charOperator = "*"
                        Case OperationType.Divide
                            charOperator = "/"
                    End Select
                    If IsValue(operation.ROperand.Name) Then
                        blockString &= Environment.NewLine & GiveIndentation(blockLevel) & $"{cvar.Name} {charOperator}= {ConvertHexToLong(operation.ROperand.Name.Trim)};"
                    Else
                        Dim cvar2 As CVariable = AsmParser.GetVariable(operation.ROperand.Name, scope)
                        blockString &= Environment.NewLine & GiveIndentation(blockLevel) & $"{cvar.Name} {charOperator}= {cvar2.Name};"
                    End If
                End If


            ElseIf operationType = OperationType.DecideIf Or operationType = OperationType.DecideElseIf Or operationType = OperationType.DecideElse Then
                Dim leftOp, rightOp As String

                leftOp = String.Empty
                rightOp = String.Empty

                If Not operationType = OperationType.DecideElse Then
                    leftOp = AsmParser.GetVariable(operation.LOperand.Name, scope).Name
                    rightOp = AsmParser.GetVariable(operation.ROperand.Name, scope).Name
                    If IsRegister(operation.LOperand.Name) Or IsRegister(operation.ROperand.Name) Then
                        Dim frame = runtimeframes(runtimeframes.Keys.ToList.FindLast(Function(p) ConvertHexToLong(p) <= operation.CodeLine.GetAddressValue()))

                        If IsRegister(operation.LOperand.Name) Then
                            leftOp = EvaluateExpression(frame(operation.LOperand.Name))
                        End If
                        If IsRegister(operation.ROperand.Name) Then
                            rightOp = EvaluateExpression(frame(operation.ROperand.Name))
                        End If
                    End If

                    If IsValue(operation.LOperand.Name) Then
                        leftOp = operation.LOperand.Name
                    End If
                    If IsValue(operation.ROperand.Name) Then
                        rightOp = operation.ROperand.Name
                    End If
                End If



                Dim decisionBlock = DirectCast(operation.ContentBlock, DecisionBlock)

                Dim jumpline = decisionBlock.DecisionJumpLine
                Dim if_statement As String
                If operationType = OperationType.DecideIf Then

                    if_statement = $"if({leftOp}{ParseDecisionJumpLineToOperator(jumpline)}{rightOp})"
                ElseIf operationType = OperationType.DecideElseIf Then
                    if_statement = $"else if({leftOp}{ParseDecisionJumpLineToOperator(jumpline)}{rightOp})"
                Else

                    if_statement = "else"
                End If

                blockString &= Environment.NewLine & GiveIndentation(blockLevel) & if_statement & "{"


                Dim codelines As List(Of CodeLine) = decisionBlock.Content

                Dim expParser As ExpressionParser
                If operationType = OperationType.DecideElse Then
                    expParser = New ExpressionParser(codelines, codelines(0), codelines(codelines.Count - 1))
                Else
                    expParser = New ExpressionParser(codelines, codelines(2), codelines(codelines.Count - 1))

                End If


                expParser.SetupAssemblyTraverser(AssemblyTraverser)

                Dim auxOperations = expParser.GenerateOperations()

                blockLevel += 1
                blockString &= ConvertAbstractToC(auxOperations) & Environment.NewLine & GiveIndentation(blockLevel - 1) & "}"
                blockLevel -= 1





            End If

        Next
        Return blockString

    End Function


End Class


