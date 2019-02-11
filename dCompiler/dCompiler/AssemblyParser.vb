Imports dCompiler.SymbolTable
Imports dCompiler.PseudoCodeModel
Imports dCompiler.AssemblyInterpretationModel
Imports System.Text.RegularExpressions



Public Class AssemblyParser



    Private rawAsm As String = String.Empty
    Private asmTraverser As AssemblyTraverser
    Private symProc As SymbolProcessor

    Public Property RawAssembly As String
        Get
            Return rawAsm

        End Get
        Set(value As String)
            rawAsm = value
        End Set
    End Property


    Public Sub New()

    End Sub
    Public Sub New(rawAsm_ As String, assemblyTraverser As AssemblyTraverser)
        rawAsm = rawAsm_
        asmTraverser = assemblyTraverser
        symProc = asmTraverser.GetSymbolProcessor
    End Sub

    Public Function GetVariables(rawAsm As String, ByVal scope As CFunction) As List(Of CVariable)
        If rawAsm Is Nothing Then Return New List(Of CVariable)
        Dim cvarList As New List(Of CVariable)
        Dim data As String() = rawAsm.Split(Environment.NewLine)
        For Each line In data

            If AssemblyParser_variable_parser_regex.IsMatch(line) Then
                Dim match As Match = AssemblyParser_variable_parser_regex.Match(line)
                Dim cvar As CVariable
                Dim offdirection As Char = "m"
                If match.Groups(2).Value = "+" Then
                    offdirection = "p"
                End If
                cvar.Name = "var_" & offdirection & match.Groups(3).Value.Remove(0, 1)
                cvar.Offset = ConvertHexToLong(match.Groups(3).Value)
                cvar.Size = match.Groups(1).Value
                cvar.BaseAddress = ""
                cvar.Scope = scope
                cvarList.Add(cvar)
            End If
        Next
        Return cvarList
    End Function



    Public Function GetFunctions(rawAsm As String, Optional ByRef rootList As List(Of CFunction) = Nothing) As List(Of CFunction)
        If rawAsm = String.Empty Then Return New List(Of CFunction)
        Dim cfunList As List(Of CFunction)
        If rootList Is Nothing Then
            cfunList = New List(Of CFunction)
        Else
            cfunList = rootList
        End If

        Dim data As String() = rawAsm.Split(Environment.NewLine)
        For Each line In data
            If AssemblyParser_function_parser_regex.IsMatch(line) Then
                Dim match As Match = AssemblyParser_function_parser_regex.Match(line)
                Dim cfun As CFunction

                If match.Groups(3) IsNot "" Then
                    cfun.Name = match.Groups(3).Value
                    'Here we add an exception to prevent function capture from __main() in libc
                    If cfun.Name = "__main" Then
                        Continue For
                    End If
                Else
                    cfun.Name = "fun_" & match.Groups(1).Value
                End If
                cfun.StartAddress = match.Groups(1).Value

                Dim partialFunction As CFunction = asmTraverser.GetFunction(cfun.StartAddress)

                cfun.EndAddress = partialFunction.EndAddress
                cfun.RawAssembly = partialFunction.RawAssembly


                If Not cfun.EndAddress = "" And Not cfunList.Exists(Function(p) p.Name = cfun.Name) Then


                    cfunList.Add(cfun)


                    GetFunctions(cfun.RawAssembly, cfunList)
                End If

            End If
        Next


        Return cfunList
    End Function

    Public Function GenerateCodeLines(rawAsm As String) As List(Of CodeLine)
        Dim codeList As New List(Of CodeLine)
        Dim lines As String() = rawAsm.Split(Environment.NewLine)

        For Each line In lines
            Dim codeLine As New CodeLine
            Dim match As Match = AssemblyParser_codeline_regex.Match(line)
            If match Is Match.Empty Then
                Continue For
            End If
            codeLine.Address = symProc.GeneraliseAddress(match.Groups(1).Value)

            codeLine.Code = match.Groups(2).Value
            codeList.Add(codeLine)
        Next
        Return codeList
    End Function



    Public Function GenerateSuperficialCodeModel(codelines As List(Of CodeLine))



    End Function

    Public Function GenerateCondition(cmpStatement As CodeLine, codeLineList As List(Of CodeLine)) As Condition

    End Function





    Public Function ParseDecisionStatements(codelines As List(Of CodeLine))

        Dim stack As New Stack(Of DecisionLadder)
        Dim list As New List(Of DecisionLadder)


        For i As Integer = 0 To codelines.Count() - 1
            Dim codeline As CodeLine = codelines(i)
            If Not stack.Count() = 0 Then
                Dim dl As DecisionLadder = stack.Peek()
                If codeline = dl.EndLine And dl.Tag = -1 And dl.IsSwitchCaseLadder = False Then

                    dl.Tag = 1
                    Dim ds As New DecisionStatement
                    Dim pool As List(Of CodeLine) = codelines


                    Dim dsPrevIndex As Integer = pool.FindIndex(Function(p) p = dl.DecisionStatements.Last.EndLine)
                        ds.Content = pool.GetRange(dsPrevIndex + 2, i - dsPrevIndex - 2)
                        ds.DecisionCondition = Nothing
                        ds.StartLine = pool(dsPrevIndex + 2)
                        ds.EndLine = pool(i - 1)
                    dl.DecisionStatements.Add(ds)
                    list.Add(dl)
                    stack.Pop()





                End If
            End If
            Dim jmpConditionMatch As Match = AssemblyParser_jump_condition_regex.Match(codeline.Code)
            If jmpConditionMatch.Success Then
                Dim conditionAddress As String = symProc.GeneraliseAddress(jmpConditionMatch.Groups(2).Value)
                If ConvertHexToLong(conditionAddress) > ConvertHexToLong(codeline.Address) Then
                    Dim cmpMatch1 As Match = AssemblyParser_cmp_statement_regex.Match(codelines(i - 1).Code)
                    If cmpMatch1.Success Then
                        'This is based on the assumption that we are not dealing with switch case.
                        'Additional support is required.
                        'Otherwise we are dealing with our old friend-- the if-else ladder or
                        '                         a simple if-else.

                        'the current codeline is the jump condition statement.
                        'jne 0x40153a

                        Dim complementConditionJumpAddress As String = conditionAddress
                            Dim complementConditionJumpLineIndex As Integer = codelines.FindIndex(Function(p) p.Address = symProc.GeneraliseAddress(complementConditionJumpAddress))
                            Dim startLineOfBlock As CodeLine = codelines(i + 1)
                            Dim endLineOfBlock As CodeLine = codelines(complementConditionJumpLineIndex - 1)
                            Dim jumpOutOfBlockMatch As Match = AssemblyParser_jump_statement_regex.Match(endLineOfBlock.Code)



                            If jumpOutOfBlockMatch.Success Then   'Here it can either be if-else or if-elseif ladder 
                                Dim jumpOutOfBlockLine As CodeLine = endLineOfBlock

                                endLineOfBlock = codelines(complementConditionJumpLineIndex - 2)

                                Dim endLineOfDecisionCombo As CodeLine = codelines(codelines.FindIndex(Function(p) p.Address = symProc.GeneraliseAddress(jumpOutOfBlockMatch.Groups(1).Value)) - 1)
                                Dim decisionStatement As New DecisionStatement
                                Dim decisionLadder As New DecisionLadder
                                decisionLadder.IsSwitchCaseLadder = False

                                With decisionStatement
                                    .StartLine = startLineOfBlock
                                    .EndLine = endLineOfBlock
                                    .DecisionCondition = New Condition With {.Content = codelines(i - 1)}
                                    .Content = codelines.GetRange(i + 1, complementConditionJumpLineIndex - i - 1)

                                End With

                                If stack.Count() = 0 Then

                                    decisionLadder.DecisionStatements.Add(decisionStatement)
                                    decisionLadder.StartLine = startLineOfBlock
                                    decisionLadder.EndLine = endLineOfDecisionCombo
                                    decisionLadder.Tag = -1

                                    stack.Push(decisionLadder)

                                ElseIf stack.Peek().IsSwitchCaseLadder = True Then  'we need to create a new decision ladder.

                                    decisionLadder.DecisionStatements.Add(decisionStatement)
                                    decisionLadder.StartLine = startLineOfBlock
                                    decisionLadder.EndLine = endLineOfDecisionCombo
                                    decisionLadder.Tag = -1
                                    stack.Push(decisionLadder)

                                ElseIf stack.Peek().Tag = -1 Then    'decision-ladder is incomplete

                                    decisionLadder = stack.Peek()
                                    stack.Pop()
                                    decisionLadder.DecisionStatements.Add(decisionStatement)
                                    stack.Push(decisionLadder)
                                ElseIf stack.Peek().Tag = 1 Then  'decision ladder is complete, create a new one.
                                    decisionLadder.DecisionStatements.Add(decisionStatement)
                                    decisionLadder.StartLine = startLineOfBlock
                                    decisionLadder.EndLine = endLineOfDecisionCombo
                                    decisionLadder.Tag = -1
                                    stack.Push(decisionLadder)

                                End If


                            Else
                                MsgBox($"line={codeline.Code} address={codeline.Address} index={i.ToString()}")
                                'Now here it should definitely be a if-statement only
                                'Because there are no sequential jump statements in raw_asm
                                ' For simple if-statements we shall use a decisionLadder of order 1
                                Dim decisionStatement As New DecisionStatement
                                Dim decisionLadder As New DecisionLadder
                                decisionLadder.IsSwitchCaseLadder = False
                                With decisionStatement
                                    .StartLine = startLineOfBlock
                                    .EndLine = endLineOfBlock
                                    .DecisionCondition = New Condition With {.Content = codelines(i - 1)}
                                    .Content = codelines.GetRange(i + 1, complementConditionJumpLineIndex - i - 1)

                                End With
                                decisionLadder.Tag = 1
                                decisionLadder.DecisionStatements.Add(decisionStatement)
                                decisionLadder.StartLine = startLineOfBlock
                                decisionLadder.EndLine = endLineOfBlock
                            If stack.Count() = 0 Then
                                list.Add(decisionLadder)
                            Else
                                If Not stack.Peek().EndLine = decisionLadder.EndLine Then
                                    list.Add(decisionLadder)
                                End If
                            End If




                        End If

                        End If
                    End If
                End If

        Next
        MsgBox(stack.Count())


        Return list
    End Function


    Public Function ParseLoopStatements(codelines As List(Of CodeLine)) As List(Of LoopStatement)

        Dim loopStatementList As New List(Of LoopStatement)
        For i As Integer = 0 To codelines.Count() - 1
            Dim codeline_ As CodeLine = codelines(i)


            Dim cmpStatementMatch As Match = AssemblyParser_cmp_statement_regex.Match(codeline_.Code)
            If cmpStatementMatch.Success Then
                Dim nextcodeline As CodeLine = codelines(i + 1)

                Dim jumpConditionMatch As Match = AssemblyParser_jump_condition_regex.Match(nextcodeline.Code)
                If jumpConditionMatch.Success Then
                    If ConvertHexToLong(jumpConditionMatch.Groups(2).Value) < ConvertHexToLong(nextcodeline.Address) Then
                        Dim loopStartAddress As String = symProc.GeneraliseAddress(jumpConditionMatch.Groups(2).Value)
                        Dim loopStartIndex As Integer = codelines.FindIndex(Function(p) p.Address = loopStartAddress)
                        Dim jmpCodeLine As CodeLine = codelines(loopStartIndex - 1)
                        Dim jmpMatch As Match = AssemblyParser_jump_statement_regex.Match(jmpCodeLine.Code)
                        If jmpMatch.Success Then
                            Dim loopEndLineIndex As Integer = codelines.FindIndex(Function(p) p.Address = symProc.GeneraliseAddress(jmpMatch.Groups(1).Value)) - 1
                            Dim loopStatement As New LoopStatement With {
                            .StartLine = codelines(loopStartIndex),
                            .IsEntryControlled = True,
                            .EndLine = codelines(loopEndLineIndex),
                            .Tag = jmpCodeLine,
                            .LoopCondition = New Condition With {.Tag = codeline_},
                            .Content = codelines.GetRange(loopStartIndex, loopEndLineIndex - loopStartIndex + 1)}
                            loopStatementList.Add(loopStatement)
                        Else
                            Dim loopStatement As New LoopStatement With {
                            .StartLine = codelines(loopStartIndex),
                            .IsEntryControlled = False,
                            .EndLine = codeline_,
                            .Tag = jmpCodeLine,
                            .LoopCondition = New Condition With {.Tag = codeline_},
                            .Content = codelines.GetRange(loopStartIndex, codelines.FindIndex(Function(p) p = codeline_) - loopStartIndex + 1)}
                            loopStatementList.Add(loopStatement)
                        End If
                    End If
                End If
            End If

        Next
        Return loopStatementList
    End Function


    Private Function CheckIfDecisionLadder(obj As Object) As Boolean
        Try
            Dim dc As DecisionLadder = CType(obj, DecisionLadder)
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

End Class


Public Class PseudoCodeModel


    Public Class CodeLine
        Public Code As String
        Public Address As String
        Public CallAddress As String
        Public Tag As Object
        Public Content As Object

        Public Shared Operator =(ByVal leftVal As CodeLine, rightVal As CodeLine)
            Return leftVal.Address = rightVal.Address
        End Operator

        Public Shared Operator <>(ByVal leftVal As CodeLine, rightVal As CodeLine)
            Return Not (leftVal.Address = rightVal.Address)
        End Operator
        Public Shared Operator >(left As CodeLine, right As CodeLine)
            Return left.GetAddressValue > right.GetAddressValue
        End Operator
        Public Shared Operator <(left As CodeLine, right As CodeLine)
            Return left.GetAddressValue < right.GetAddressValue
        End Operator
        Public Shared Operator >=(left As CodeLine, right As CodeLine)
            Return left.GetAddressValue >= right.GetAddressValue
        End Operator
        Public Shared Operator <=(left As CodeLine, right As CodeLine)
            Return left.GetAddressValue <= right.GetAddressValue
        End Operator

        Public Function GetAddressValue() As Long
            Return ConvertHexToLong(Address)
        End Function
    End Class

    Public Class DecisionLadder
        Public StartLine As CodeLine
        Public EndLine As CodeLine
        Public DecisionStatements As List(Of DecisionStatement)
        Public SwitchCases As List(Of SwitchCase)
        Public IsSwitchCaseLadder As Boolean
        Public Tag As Object
        Public Content As Object
        Public Sub New()
            DecisionStatements = New List(Of DecisionStatement)
            SwitchCases = New List(Of SwitchCase)
            Tag = -1
        End Sub
    End Class

    Public Class DecisionStatement
        Public StartLine As CodeLine
        Public EndLine As CodeLine
        Public DecisionCondition As Condition
        Public Tag As Object
        Public Content As Object

    End Class

    Public Class SwitchCase
        Public StartLine As CodeLine
        Public EndLine As CodeLine
        Public SwitchValue As ValueSource
        Public Tag As Object
        Public Content As Object
    End Class

    Public Class SwitchCaseLadder
        Public StartLine As CodeLine
        Public EndLine As CodeLine
        Public SwitchCases As List(Of SwitchCase)
        Public Tag As Object
        Public Content As Object
    End Class

    Public Class ValueSource
        Public Value As String
        Public Variable As CVariable
        Public Tag As Object
        Public IsValue As Boolean
        Public Content As Object
    End Class

    Public Class Condition
        Public Line As CodeLine
        Public ConditionOperator As String
        Public LOperand As ValueSource
        Public ROperand As ValueSource
        Public Tag As Object
        Public Content As Object
    End Class

    Public Class LoopStatement
        Public StartLine As CodeLine
        Public EndLine As CodeLine
        Public IsEntryControlled As Boolean
        Public LoopCondition As Condition
        Public Tag As Object
        Public Content As Object
    End Class



End Class