Imports dCompiler.SymbolProcessor
Imports dCompiler.SymbolTable
Imports dCompiler.PseudoCodeModel
Imports System.Text.RegularExpressions
Imports dCompiler.AssemblyInterpretationModel

''' <summary>
''' This Class has been designed for stripped binaries.
''' Using this Class we will be able to traverse through assembly code in a hierarchical manner and use these results for the
''' AssemblyParser Class.
''' </summary>

Public Class AssemblyTraverser
    Private rawData As String
    Private symProc As SymbolProcessor
    Private gdb As GdbInterface
    Public Property Data As String
        Get
            Return rawData
        End Get
        Set(value As String)
            rawData = value
        End Set
    End Property

    Public Property GetSymbolProcessor As SymbolProcessor
        Get
            Return symProc
        End Get
        Set(value As SymbolProcessor)
            symProc = value
        End Set
    End Property


    Public Sub New(data As String, symProcessor As SymbolProcessor)
        rawData = data
        symProc = symProcessor
        gdb = symProcessor.GetGdbInterface

    End Sub





    Public Sub InitializeRawData()
        rawData = GetRawCode()
    End Sub
    Public Function GetRawCode() As String
        gdb.RunGdb()
        gdb.ClearBuffer()
        gdb.SendInputAndWait({$"disas {symProc.GetSymbolTable.EntryPoint},{symProc.SectionCollection.Find(Function(p) p.SectionName = ".text").EndAddress}"})

        Return gdb.OutputBuffer
    End Function

    Public Function GoToAddress(rawData As String, Address As String) As Integer 'Returns Line index
        Dim data As String() = rawData.Split(Environment.NewLine)
        Dim i As Integer = 0
        For Each line In data
            If AssemblyTraverser_address_line_regex.Match(line).Groups(1).Value = Address Then
                Return i
            End If
            i = i + 1
        Next
        Return -1
    End Function

    Public Function CropBeforeAddress(rawData As String, Address As String) As String
        Dim index As Integer = GoToAddress(rawData, symProc.GeneraliseAddress(Address))
        Dim lines As String() = rawData.Split(Environment.NewLine)


        Dim cropString As String = ""
        For i As Integer = index To lines.Count - 1
            cropString &= lines(i) & Environment.NewLine
        Next
        Return cropString
    End Function


    Public Function GetFunction(ByVal StartAddress As String) As CFunction
        Dim data As String = CropBeforeAddress(rawData, symProc.GeneraliseAddress(StartAddress))

        Dim cfun As CFunction
        cfun.StartAddress = StartAddress
        cfun.EndAddress = GetEndOfFunction(data)
        If Not cfun.EndAddress = "" Then
            gdb.RunGdb()

            gdb.ClearBuffer()
            gdb.SendInputAndWait({$"disas {cfun.StartAddress},{ConvertLongToHex(ConvertHexToLong(cfun.EndAddress) + 1)}"})
            Dim asmData As String = gdb.OutputBuffer

            asmData = AssemblyTraverser_raw_asm_capture_regex.Match(asmData).Groups(1).Value
            cfun.RawAssembly = asmData
        End If
        cfun.Name = symProc.GetSymbolAtAddress(StartAddress).Name
        Return cfun
    End Function
    Public Function GetEndOfFunction(Optional ByVal func_data As String = "") As String
        If func_data = "" Then func_data = rawData
        Dim match As Match = AssemblyTraverser_function_end_regex.Match(func_data)
        Return match.Groups(1).Value

    End Function


    Public Function SeekValueSource(codeLines As List(Of CodeLine), registerName As String, EndAddress As String) As ValueSource
        Dim codelines_range As List(Of CodeLine) = codeLines.GetRange(0, codeLines.FindIndex(Function(p) p.Address = EndAddress))
        Dim source As New ValueSource
        For Each line In codelines_range
            Dim matchVal As Match = AssemblyParser_mov_val_to_register_regex.Match(line.Code)
            Dim matchVar As Match = AssemblyParser_mov_var_to_register_regex.Match(line.Code)
            If matchVal.Success Then
                If matchVal.Groups(1).Value = registerName Then
                    source.Value = matchVal.Groups(2).Value
                    source.Tag = line
                    source.IsValue = True
                End If
            End If
            If matchVar.Success Then
                If matchVar.Groups(1).Value = registerName Then
                    source.Variable = New CVariable With {.Tag = line}
                End If
            End If

        Next
        Return source

    End Function





    Public Function GenerateDecisionBlocks(ByVal jumplines As List(Of JumpLine), codelines As List(Of CodeLine)) As List(Of DecisionBlock)
        Dim decisionBlocks As New List(Of DecisionBlock)
        If jumplines.Count() = 0 Then
            Return decisionBlocks
        End If
        For i As Integer = 0 To codelines.Count() - 1
            Dim codeline As CodeLine = codelines(i)
            If jumplines.Exists(Function(p) p.Line = codeline) Then  'We are checking if the line is a jumpline or not
                Dim jumpline As JumpLine = jumplines.Find(Function(p) p.Line = codeline)
                If Not jumpline.JumpCondition = JumpConditionFlag.NoJumpCondition Then 'jumpline is a conditional JL
                    Dim decisionBlock As New DecisionBlock
                    decisionBlock.DecisionJumpLine = jumpline
                    decisionBlock.StartLine = codelines(i - 1)
                    decisionBlock.EndLine = codelines.Find(Function(p) p.Address = symProc.GeneraliseAddress(jumpline.JumpAddress))
                    decisionBlock.Content = codelines.GetRange(i - 1, codelines.FindIndex(Function(p) p = decisionBlock.EndLine) - i + 2)
                    decisionBlock.BlockType = DecisionBlockType.Independent
                    Dim potentialJL As CodeLine = codelines(codelines.FindIndex(Function(q) q = decisionBlock.EndLine) - 1)
                    decisionBlock.TerminalJumpLine = New JumpLine With {.Line = New CodeLine With {.Address = "0x0"}}
                    If jumplines.Exists(Function(p) p.Line = potentialJL) Then
                        Dim terminalJumpLine As JumpLine = jumplines.Find(Function(p) p.Line = potentialJL)
                        If terminalJumpLine.JumpCondition = JumpConditionFlag.NoJumpCondition Then
                            decisionBlock.TerminalJumpLine = terminalJumpLine
                            decisionBlock.BlockType = DecisionBlockType.Member
                        End If
                    End If
                    decisionBlock.ManagedContent = New List(Of DecisionBlock)
                    decisionBlocks.Add(decisionBlock)
                End If


            End If
        Next

        Return decisionBlocks

    End Function

    Public Function GetLastTrivialDecisionBlock(memberDecisionBlock As DecisionBlock, decisionBlocks As List(Of DecisionBlock)) As DecisionBlock
        Dim startIndex As Integer = decisionBlocks.FindIndex(Function(p) p = memberDecisionBlock)
        For i As Integer = startIndex To decisionBlocks.Count() - 1
            If Not decisionBlocks(i).TerminalJumpLine.JumpAddress = memberDecisionBlock.TerminalJumpLine.JumpAddress Then
                Return decisionBlocks(i - 1)
            End If
            If i = decisionBlocks.Count() - 1 And decisionBlocks(i).TerminalJumpLine.JumpAddress = memberDecisionBlock.TerminalJumpLine.JumpAddress Then
                Return decisionBlocks(i)
            End If
        Next
        Return memberDecisionBlock
    End Function
    ''' <summary>
    ''' Returns a list of DecisionBlocks after adding Else-blocks to the given list of Arranged DecisionBlocks
    ''' </summary>
    ''' <param name="codelines"></param>
    ''' <param name="decisionBlocks"></param>
    ''' <returns></returns>
    Public Function AddNonTrivialDecisionBlocks(codelines As List(Of CodeLine), ByRef decisionBlocks As List(Of DecisionBlock)) As List(Of DecisionBlock)

        Dim insertBlocks As New Dictionary(Of Integer, DecisionBlock)
        Dim ignorableBlocks As New List(Of DecisionBlock)
        For i As Integer = 0 To decisionBlocks.Count() - 1
            Dim decisionBlock As DecisionBlock = decisionBlocks(i)
            decisionBlock.ManagedContent = AddNonTrivialDecisionBlocks(codelines, decisionBlock.ManagedContent)
            If decisionBlock.BlockType = DecisionBlockType.Member Then
                Dim lastTrivialDecisionBlock As DecisionBlock = GetLastTrivialDecisionBlock(decisionBlock, decisionBlocks) '
                'decisionBlocks.FindLast(Function(p) p.EndLine.GetAddressValue < decisionBlock.TerminalJumpLine.GetAddressValue And p.BlockType = DecisionBlockType.Member And p.TerminalJumpLine.Line = decisionBlock.TerminalJumpLine.Line)


                'This means that there is an Else Block
                Dim elseDecisionBlock As New DecisionBlock
                elseDecisionBlock.StartLine = lastTrivialDecisionBlock.EndLine

                elseDecisionBlock.EndLine = codelines.Find(Function(p) p.Address = symProc.GeneraliseAddress(lastTrivialDecisionBlock.TerminalJumpLine.JumpAddress))
                Dim startIndex = codelines.FindIndex(Function(p) p = elseDecisionBlock.StartLine)
                elseDecisionBlock.Content = codelines.GetRange(startIndex, codelines.FindIndex(Function(p) p = elseDecisionBlock.EndLine) - startIndex + 1)
                elseDecisionBlock.ManagedContent = New List(Of DecisionBlock)
                elseDecisionBlock.BlockType = DecisionBlockType.IndependentElse
                elseDecisionBlock.TerminalJumpLine = New JumpLine With {.JumpCondition = JumpConditionFlag.NoJumpCondition}

                For Each potentialChild In decisionBlocks
                    If (potentialChild.StartLine >= elseDecisionBlock.StartLine) And (potentialChild.EndLine <= elseDecisionBlock.EndLine) Then
                        elseDecisionBlock.ManagedContent.Add(potentialChild)
                        ignorableBlocks.Add(potentialChild)
                    End If

                Next

                'elseDecisionBlock.ManagedContent = ArrangeDecisionBlocks(elseDecisionBlock.ManagedContent)

                'Recursive addition of nontrivial decision blocks via depths of managed content

                elseDecisionBlock.ManagedContent = AddNonTrivialDecisionBlocks(codelines, elseDecisionBlock.ManagedContent)

                insertBlocks.Add(decisionBlocks.FindLastIndex(Function(p) p.EndLine <= elseDecisionBlock.StartLine) + 1, elseDecisionBlock)
                i = decisionBlocks.FindIndex(Function(p) p.StartLine.GetAddressValue > lastTrivialDecisionBlock.TerminalJumpLine.GetAddressValue)
                If i = -1 Then
                    Exit For
                End If
            End If
        Next
        For Each block In insertBlocks
            decisionBlocks.Insert(block.Key, block.Value)
        Next
        For Each block In ignorableBlocks
            decisionBlocks.Remove(block)
        Next
        SortDecisionBlocks(decisionBlocks)
        Return decisionBlocks
    End Function
    Public Function SortDecisionBlocks(ByRef decisionBlocks As List(Of DecisionBlock), Optional ByVal IsRecursive As Boolean = False) As List(Of DecisionBlock)
        decisionBlocks.Sort(Function(x, y)
                                If x.StartLine < y.StartLine Then
                                    Return -1
                                ElseIf x.StartLine = y.StartLine Then
                                    Return 0
                                Else
                                    Return 1
                                End If
                            End Function)

        If IsRecursive Then
            For Each block In decisionBlocks
                SortDecisionBlocks(block.ManagedContent)
            Next
        End If
        Return decisionBlocks
    End Function

    Public Function ArrangeDecisionBlocks(ByRef decisionBlocks As List(Of DecisionBlock)) As List(Of DecisionBlock)
        Dim arrangedDecisionBlocks As New List(Of DecisionBlock)
        If decisionBlocks.Count() = 0 Then
            Return arrangedDecisionBlocks
        End If

        Dim tempDecisionBlocks As List(Of DecisionBlock) = decisionBlocks
        tempDecisionBlocks.Reverse()


        Dim i As Integer = 0
        Dim ignorableIndices As New List(Of Integer)
        While (True)
            Dim j As Integer = 0
            Dim decisionBlock As DecisionBlock = tempDecisionBlocks(i)
            Dim length As Integer = tempDecisionBlocks.Count()

            For j = 0 To length - 1
                Dim potentialChildDecisionBlock As DecisionBlock = tempDecisionBlocks(j)
                If potentialChildDecisionBlock = decisionBlock Or ignorableIndices.Exists(Function(p) p = j) Then
                    Continue For
                End If
                If (decisionBlock.StartLine < potentialChildDecisionBlock.StartLine) And (decisionBlock.EndLine >= potentialChildDecisionBlock.EndLine) Then
                    'decisionBlock.ManagedContent.Add(potentialChildDecisionBlock)
                    'tempDecisionBlocks(i) = decisionBlock
                    ignorableIndices.Add(j)
                    'i -= 1
                End If
            Next

            i += 1
            If i >= length Then Exit While

        End While


        'For i As Integer = 0 To length
        'Dim decisionBlock As DecisionBlock = tempDecisionBlocks(i)
        'For Each potentialChildDecisionBlock In tempDecisionBlocks
        'If potentialChildDecisionBlock = DecisionBlock Then
        '     Continue For
        '  End If
        '   If (DecisionBlock.StartLine < potentialChildDecisionBlock.StartLine) And (DecisionBlock.EndLine >= potentialChildDecisionBlock.EndLine) Then
        '        DecisionBlock.ManagedContent.Add(potentialChildDecisionBlock)
        '         tempDecisionBlocks.Remove(potentialChildDecisionBlock)
        '          i -= 1
        '       End If
        '    Next
        'decisionBlock.ManagedContent.Reverse()
        'decisionBlock.ManagedContent = ArrangeDecisionBlocks(decisionBlock.ManagedContent)
        'arrangedDecisionBlocks.Add(decisionBlock)
        ' Next
        For x As Integer = 0 To tempDecisionBlocks.Count() - 1
            If Not ignorableIndices.Exists(Function(p) p = x) Then
                If Not arrangedDecisionBlocks.Exists(Function(p) p = tempDecisionBlocks(x)) Then
                    Dim block As DecisionBlock = tempDecisionBlocks(x)
                    block.ManagedContent = ArrangeDecisionBlocks(decisionBlocks.FindAll(Function(p) p.StartLine > block.StartLine And p.EndLine <= block.EndLine))
                    arrangedDecisionBlocks.Add(block)

                End If
            End If
        Next
        'tempDecisionBlocks.RemoveAll(Function(p) ignorableIndices.Exists(Function(q) tempDecisionBlocks(q) = p))
        'tempDecisionBlocks.Reverse()
        SortDecisionBlocks(arrangedDecisionBlocks)
        'arrangedDecisionBlocks.Reverse()
        Return arrangedDecisionBlocks
    End Function



    Public Function GenerateConditionalJumpLines(codelines As List(Of CodeLine)) As List(Of JumpLine)
        Dim cjumpLineList As New List(Of JumpLine)
        For i As Integer = 0 To codelines.Count() - 1
            Dim codeline As CodeLine = codelines(i)
            Dim match As Match = AssemblyParser_jump_condition_regex.Match(codeline.Code)
            Dim match2 As Match = AssemblyParser_jump_statement_regex.Match(codeline.Code)
            If match.Success Then
                Dim cjLine As New JumpLine
                cjLine.Line = codeline
                cjLine.JumpAddress = symProc.GeneraliseAddress(match.Groups(2).Value)
                If cjLine.GetAddressValue < cjLine.Line.GetAddressValue Then
                    Continue For
                End If
                Dim rawCondition As String = match.Groups(1).Value
                Select Case rawCondition
                    Case "je"
                        cjLine.JumpCondition = JumpConditionFlag.JumpIfEqual
                    Case "jne"
                        cjLine.JumpCondition = JumpConditionFlag.JumpIfNotEqual
                    Case "jge"
                        cjLine.JumpCondition = JumpConditionFlag.JumpIfGreaterThanOrEqualTo
                    Case "jle"
                        cjLine.JumpCondition = JumpConditionFlag.JumpIfLessThanOrEqualTo
                End Select
                cjumpLineList.Add(cjLine)
            ElseIf match2.Success Then

                Dim jl As New JumpLine
                jl.Line = codeline
                jl.JumpCondition = JumpConditionFlag.NoJumpCondition
                jl.JumpAddress = symProc.GeneraliseAddress(match2.Groups(1).Value)
                cjumpLineList.Add(jl)

            End If

        Next
        Return cjumpLineList

    End Function







    Public Function GetProgramMainAddress() As String
        'Here we must refer to a binary pool manager but for the prototype version
        'we are refering to a known address of main() with respect to F(Machine,Compiler)

        Dim libMainAddress As String = My.Resources.prototype_main_address
        Dim entryPoint As String = symProc.GetSymbolTable.EntryPoint
        Dim entry_point_long As Long = ConvertHexToLong(entryPoint)
        Dim tempData As String = ""
        Dim offset As Long = 1
        While tempData = ""
            gdb.ClearBuffer()
            gdb.SendInputAndWait({$"disas {ConvertLongToHex(entry_point_long + offset - 1)},{ConvertLongToHex(entry_point_long + offset)}"})
            offset += 1
            If gdb.OutputBuffer.Contains(libMainAddress) Then

            End If
        End While

    End Function






End Class


''' <summary>
''' Built on the PseudoCodeModel
''' <para>
''' It is a Full-duplex integration medium between the PseudoCodeModel and the AssemblyTraversal Class
''' </para>
''' 
''' 
''' </summary>
Public Class AssemblyInterpretationModel

    Public Class Register
        Public Value As Integer
        Public Address As String
        Public IsValue As Boolean
        Public Tag As Object
    End Class

    Public Class JumpLine
        Public Line As CodeLine
        Public JumpAddress As String
        Public JumpCondition As JumpConditionFlag
        Public Function GetAddressValue() As Long
            Return ConvertHexToLong(JumpAddress)
        End Function


    End Class

    Public Enum JumpConditionFlag
        NoJumpCondition = 0
        JumpIfEqual = 1
        JumpIfNotEqual = 2
        JumpIfLessThanOrEqualTo = 3
        JumpIfGreaterThanOrEqualTo = 4
        JumpIfZero = 5
        JumpIfNotZero = 6
    End Enum

    Public Enum DecisionBlockType
        Member = 0
        Independent = 1
        IndependentElse = 2

    End Enum

    Public Class DecisionBlock
        Public StartLine As CodeLine
        Public EndLine As CodeLine
        Public BlockType As DecisionBlockType
        Public DecisionJumpLine As JumpLine
        Public TerminalJumpLine As JumpLine
        Public Content As Object
        Public Tag As Object

        Public ManagedContent As List(Of DecisionBlock)

        Public Shared Operator =(ByVal left As DecisionBlock, right As DecisionBlock)
            Return left.StartLine = right.StartLine
        End Operator
        Public Shared Operator <>(ByVal left As DecisionBlock, right As DecisionBlock)
            Return left.StartLine <> right.StartLine
        End Operator

    End Class



End Class
