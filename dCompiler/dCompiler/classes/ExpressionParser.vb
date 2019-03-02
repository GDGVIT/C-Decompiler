Imports System.Text.RegularExpressions
Imports dCompiler.ExpressionModel
Imports dCompiler.PseudoCodeModel

Imports dCompiler.SymbolTable




Public Class ExpressionParser
    Public Function ScanForVariableDeclarations(codelines As List(Of CodeLine)) As List(Of CVariable)

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

                Dim operation As New Operation With {.CodeLine = codeline, .Operation = OperationType.Multiply, .LOperand = left, .ROperand = right}
                operations.Add(operation)
                Continue For
            End If


        Next





        Return operations

    End Function







End Class

Public Class ExpressionModel

    Public Enum OperationType
        Add = 1
        Subtract = 2
        Multiply = 3

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
        Public CodePool As List(Of CodeLine)

        Public Function GetInlineExpression() As String 'Now we have dived into actual C Code Parsing.

        End Function
    End Class
End Class
