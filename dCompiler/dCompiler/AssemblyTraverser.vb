Imports dCompiler.SymbolProcessor
Imports dCompiler.SymbolTable
Imports dCompiler.PseudoCodeModel
Imports System.Text.RegularExpressions

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
