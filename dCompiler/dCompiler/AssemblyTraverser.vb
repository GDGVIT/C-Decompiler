Imports dCompiler.SymbolProcessor
Imports dCompiler.SymbolTable


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

    Public Sub New(data As String, symProcessor As SymbolProcessor)
        rawData = data
        symProc = symProcessor
        gdb = symProcessor.GetGdbInterface
    End Sub
    Public Function GetFunction(ByVal StartAddress As String) As CFunction
        gdb.ClearBuffer()
        gdb.SendInputAndWait({$"disas {StartAddress},{StartAddress}"})
        Dim data As String = gdb.OutputBuffer
        Dim offset As Long = 1
        While GetEndOfFunction(data) = String.Empty
            gdb.ClearBuffer()
            gdb.SendInputAndWait({$"disas {ConvertLongToHex(ConvertHexToLong(StartAddress) + offset - 1)},{ConvertLongToHex(ConvertHexToLong(StartAddress) + offset)}"})
            data = gdb.OutputBuffer
            offset += 1
        End While
        Dim cfun As CFunction
        cfun.StartAddress = StartAddress
        cfun.EndAddress = GetEndOfFunction(data)
        cfun.Name = symProc.GetSymbolAtAddress(StartAddress).Name
        Return cfun
    End Function
    Public Function GetEndOfFunction(Optional ByVal func_data As String = "") As String
        If func_data = "" Then func_data = rawData
        Dim data As String() = func_data.Split(Environment.NewLine)
        For Each line In data
            If AssemblyTraverser_function_end_regex.IsMatch(line) Then
                Return AssemblyTraverser_function_end_regex.Match(line).Groups(1).Value
            End If
        Next
        Return Nothing
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
