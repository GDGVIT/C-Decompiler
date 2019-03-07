Imports System.Text.RegularExpressions
Imports dCompiler.ExpressionModel
Imports dCompiler.PseudoCodeModel
Imports dCompiler.AssemblyInterpretationModel
Imports dCompiler.SymbolTable




Public Class ExpressionParser
    Private codelines As List(Of CodeLine)
    Private startLine As CodeLine
    Private endLine As CodeLine


    Public Sub New(codelines As List(Of CodeLine), startLine As CodeLine, endLine As CodeLine)
        Me.codelines = codelines
        Me.startLine = startLine
        Me.endLine = endLine

    End Sub

    Public Function ParseAssignmentOperationsInScope(codelines As List(Of CodeLine), startLine As CodeLine, endLine As CodeLine) As List(Of Operation)
        Dim operations As New List(Of Operation)

        For i As Integer = codelines.IndexOf(startLine) To codelines.IndexOf(endLine)
            Dim codeline = codelines(i)
            Dim match As Match = ExpressionParsing_mov_statement.Match(codeline.Code)
            If match.Success Then
                Dim operation As New Operation With {.LOperand = New Operand With {.Line = codeline, .Name = match.Groups(1).Value}, .ROperand = New Operand With {.Line = codeline, .Name = match.Groups(2).Value}}
                operation.Operation = OperationType.Assign
                operation.CodeLine = codeline

                operations.Add(operation)
            End If
        Next
        Return operations
    End Function

    Public Function ParseFunctionCallOperationsInScope(codelines As List(Of CodeLine), startLine As CodeLine, endLine As CodeLine) As List(Of Operation)
        Dim operations As New List(Of Operation)

        For i As Integer = codelines.IndexOf(startLine) To codelines.IndexOf(endLine)
            Dim codeline = codelines(i)
            Dim match As Match = ExpressionParsing_call_regex.Match(codeline.Code)
            If match.Success Then


                'To-Do: add code to determine what register is being used in the function

                Dim operation As New Operation With {.LOperand = New Operand With {.Line = codeline, .Name = ""}, .ROperand = New Operand With {.Line = codeline, .Name = match.Groups(1).Value}}
                operation.Operation = OperationType.Assign
                operation.CodeLine = codeline
                operations.Add(operation)
            End If
        Next
        Return operations
    End Function



    Public Function ParseMathOperationsInScope(ByVal codelines As List(Of CodeLine), startLine As CodeLine, endLine As CodeLine) As List(Of Operation)
        Dim operations As New List(Of Operation)
        Dim startIndex As Integer = codelines.IndexOf(startLine)
        For Each codeline In codelines.GetRange(startIndex, codelines.IndexOf(endLine) - startIndex + 1)
            Dim match As Match = ExpressionParsing_add_regex.Match(codeline.Code)
            If match.Success Then
                Dim left As New Operand With {.Line = codeline, .Name = match.Groups(1).Value}
                Dim right As New Operand With {.Line = codeline, .Name = match.Groups(2).Value}
                Dim operation As New Operation With {.CodeLine = codeline, .Operation = OperationType.Add, .LOperand = left, .ROperand = right}
                operations.Add(operation)
                Continue For
            End If
            match = ExpressionParsing_sub_regex.Match(codeline.Code)
            If match.Success Then
                Dim left As New Operand With {.Line = codeline, .Name = match.Groups(1).Value}
                Dim right As New Operand With {.Line = codeline, .Name = match.Groups(2).Value}
                Dim operation As New Operation With {.CodeLine = codeline, .Operation = OperationType.Subtract, .LOperand = left, .ROperand = right}
                operations.Add(operation)
                Continue For
            End If

            match = ExpressionParsing_imul_double_regex.Match(codeline.Code)
            If match.Success Then
                Dim left As New Operand With {.Line = codeline, .Name = match.Groups(1).Value}
                Dim right As New Operand With {.Line = codeline, .Name = match.Groups(2).Value}
                Dim operation As New Operation With {.CodeLine = codeline, .Operation = OperationType.Multiply, .LOperand = left, .ROperand = right}
                operations.Add(operation)
                Continue For
            End If

            match = ExpressionParsing_idiv_single_regex.Match(codeline.Code)
            If match.Success Then
                Dim right As New Operand With {.Line = codeline, .Name = match.Groups(1).Value}
                Dim left As New Operand
                Dim line As CodeLine = codelines(codelines.IndexOf(codeline) - 2)
                left.Name = ExpressionParsing_mov_statement.Match(line.Code).Value
                left.Line = line

                Dim operation As New Operation With {.CodeLine = codeline, .Operation = OperationType.Divide, .LOperand = left, .ROperand = right}
                operations.Add(operation)
                Continue For
            End If


        Next





        Return operations

    End Function
    Public Sub SortOperations(ByRef operations As List(Of Operation))
        operations.Sort(Function(a, b)
                            If a.CodeLine < b.CodeLine Then
                                Return -1
                            ElseIf a.CodeLine = b.CodeLine Then
                                Return 0
                            Else Return 1
                            End If
                        End Function)
    End Sub


    Public Function GenerateOperations() As List(Of Operation)
        Dim operations As New List(Of Operation)
        operations.AddRange(ParseAssignmentOperationsInScope(codelines, startLine, endLine))
        operations.AddRange(ParseMathOperationsInScope(codelines, startLine, endLine))
        SortOperations(operations)


        Return operations
    End Function






End Class

Public Class ExpressionModel

    Public Enum OperationType
        Add = 4
        Subtract = 2
        Multiply = 3
        Assign = 1
        Divide = 5

    End Enum

    Public Class Operand
        Public Name As String
        Public Line As CodeLine

    End Class


    Public Class Operation
        Public LOperand As Operand
        Public ROperand As Operand
        Public Operation As OperationType
        Public CodeLine As CodeLine
    End Class

    Public Class Expression
        Public Source As ValueSource
        Public OperationTypeList As New List(Of OperationType)
        Public Operands As New List(Of Operand)


    End Class
End Class


Public Class ExpressionControlFlow
    Public Expressions As List(Of Expression)
    Private operations As List(Of Operation)
    Private startoperation As Operation
    Private endoperation As Operation
    Public RunTimeValues As New Dictionary(Of String, Expression)
    Public RunTimeFrames As New List(Of Dictionary(Of String, Expression))

    Public Sub New(operations As List(Of Operation), startOperation As Operation, endOperation As Operation)
        Me.operations = operations
        Me.startoperation = startOperation
        Me.endoperation = endOperation


    End Sub

    Public Sub Run()
        For i As Integer = operations.FindIndex(Function(p) p.CodeLine = startoperation.CodeLine) To operations.FindIndex(Function(p) p.CodeLine = endoperation.CodeLine)
            Dim operation = operations(i)
            Dim operationType = operation.Operation


            If operationType = OperationType.Assign Then
                Dim operationKey = operation.LOperand.Name
                Dim operationValue = operation.ROperand.Name
                Dim expression As New Expression
                If IsRegister(operationValue) Then
                    expression.Operands.AddRange(RunTimeValues(operationValue).Operands)
                    expression.OperationTypeList.AddRange(RunTimeValues(operationValue).OperationTypeList)
                Else
                    expression.Operands.Add(New Operand With {.Name = operationValue, .Line = operation.CodeLine})
                End If

                'expression.OperationTypeList.Add(OperationType.Assign)
                If RunTimeValues.Keys.Contains(operationKey) = False Then
                    'MsgBox(operation.CodeLine.Code)
                    'MsgBox($"LNAME: {operation.LOperand.Name},RNAME: {operation.ROperand.Name}")
                    RunTimeValues.Add(operationKey, expression)

                Else
                    'MsgBox(operation.CodeLine.Code)
                    RunTimeValues(operationKey) = expression

                End If


            ElseIf (operationType > 0 And operationType < 5) Then

                Dim operationKey = operation.LOperand.Name
                Dim operationValue = operation.ROperand.Name

                Dim expression As New Expression
                If RunTimeValues.Keys.Contains(operationKey) = True Then
                    expression = RunTimeValues(operationKey)
                End If
                expression.OperationTypeList.Add(operationType)
                If IsValue(operationValue) Then
                    expression.Operands.Add(New Operand With {.Name = operationValue, .Line = operation.CodeLine})

                Else

                    If IsRegister(operationValue) Then
                        expression.Operands.AddRange(RunTimeValues(operationValue).Operands)
                        expression.OperationTypeList.AddRange(RunTimeValues(operationValue).OperationTypeList)
                    Else
                        expression.Operands.Add(New Operand With {.Name = operationValue, .Line = operation.CodeLine})
                    End If



                End If
                RunTimeValues(operationKey) = expression

            End If

            If IsRegister(operation.ROperand.Name) Then

                RunTimeFrames.Add(RunTimeValues)
                '  MsgBox($"#:{RunTimeFrames.Count}" + vbCrLf + operation.CodeLine.Code + vbCrLf + operation.CodeLine.Address)
                ' For Each key In RunTimeValues
                'Dim s As String = ""
                'Dim im As Integer = 0
                'For Each op In key.Value.Operands
                's += op.Name + " "
                'If im < key.Value.OperationTypeList.Count Then
                's &= key.Value.OperationTypeList(im).ToString() & " "
                'im += 1
                'End If
                '
                'Next
                '       MsgBox($"ValueSource: {key.Key} " + vbCrLf + $"Expression: {s}")
                'Next
            End If

        Next
    End Sub
End Class

