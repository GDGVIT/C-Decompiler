Imports System.Text.RegularExpressions
Imports dCompiler.SymbolTable
Public Class SymbolProcessor
    Private gdbInterface As GdbInterface
    Private outputBuffer As String = String.Empty
    Private _bufferFlag As Boolean = False
    Private symbolTableExists As Boolean = False
    Private symbolTable As New SymbolTable
    Private _generalAddressLength As Integer

    Public ReadOnly Property GetGdbInterface As GdbInterface
        Get
            Return gdbInterface
        End Get

    End Property

    Public ReadOnly Property DoesSymbolTableExist As Boolean
        Get
            Return symbolTableExists
        End Get
    End Property

    Public Property GetSymbolTable As SymbolTable
        Get
            Return symbolTable
        End Get
        Set(value As SymbolTable)
            symbolTable = value
        End Set
    End Property

    Public Function GeneraliseAddress(ByVal address As String)
        Dim hex_ As String = address.Split("x")(1)
        Return "0x" + GetMultiple("0", _generalAddressLength - 2 - hex_.Length) + hex_
    End Function
    Private Function GetAddressLength() As String
        gdbInterface.RunGdb()
        gdbInterface.ClearBuffer()
        gdbInterface.SendInputAndWait({$"disas {symbolTable.EntryPoint},{ConvertLongToHex(ConvertHexToLong(symbolTable.EntryPoint) + 1)}"})

        Dim generalisedAddress As String = AssemblyTraverser_address_line_regex.Match(gdbInterface.OutputBuffer).Groups(1).Value
        Return generalisedAddress.Length()
    End Function

    Private ReadOnly Property GeneralisedAddressLength() As String
        Get
            Return _generalAddressLength
        End Get
    End Property


    Public Function GetSymbolAtAddress(ByVal Address As String) As Symbol
        gdbInterface.RunGdb()
        gdbInterface.ClearBuffer()
        gdbInterface.SendInputAndWait({$"info symbol {Address}"})
        Dim retval As String() = {}
        retval = {SymbolProcessor_get_symbol_at_address_regex.Match(gdbInterface.OutputBuffer).Groups(1).Value,
                    SymbolProcessor_get_symbol_at_address_regex.Match(gdbInterface.OutputBuffer).Groups(3).Value,
                    SymbolProcessor_get_symbol_at_address_regex.Match(gdbInterface.OutputBuffer).Groups(4).Value}
        Dim symbol As New Symbol With {
         .Name = retval(0),
         .Section = SectionCollection.Find(Function(f) f.SectionName = retval(2))}
        If retval(1) IsNot String.Empty Then
            symbol.Offset = retval(1)
        Else
            symbol.Offset = 0
        End If

        Return symbol

    End Function


    Public SectionCollection As New List(Of Section)

    Public Sub New(gdbInterface As GdbInterface)
        Me.gdbInterface = gdbInterface
        gdbInterface.SendInputAndWait({}) 'restart gdb

        gdbInterface.RunGdb()
        gdbInterface.ClearBuffer()
        gdbInterface.SendInputAndWait({"disas main"})
        If symbolTable_NotExists.IsMatch(gdbInterface.OutputBuffer) Then
            symbolTableExists = False
        Else
            symbolTableExists = True
        End If


        'Reading symbols from E:/main.exe...(no debugging symbols found)...done.

    End Sub

    Public Sub GenerateSchema()
        _generalAddressLength = GetAddressLength()
    End Sub



    Public Sub CreateSectionCollection()
        gdbInterface.RunGdb()
        gdbInterface.ClearBuffer()
        gdbInterface.SendInputAndWait({"info file"})

        'we also capture the Entry Point of the program in the process
        symbolTable.EntryPoint = SymbolProcessor_get_entry_point_regex.Match(gdbInterface.OutputBuffer).Groups(1).Value

        Dim rawData As String = gdbInterface.OutputBuffer
        Dim lines As String() = rawData.Split(Environment.NewLine)
        For Each line In lines
            If segmentCollection_memRangeRegex.IsMatch(line) Then
                Dim memRange As New Section
                Dim match As Match = segmentCollection_memRangeRegex.Match(line)
                memRange.StartAddress = match.Groups(1).Value
                memRange.EndAddress = match.Groups(2).Value
                memRange.SectionName = match.Groups(3).Value
                SectionCollection.Add(memRange)
            End If
        Next


    End Sub

    Public Sub GenerateSymbolTable()
        If Not symbolTableExists Then Exit Sub

    End Sub





End Class

Public Class SymbolTable

    Public Structure Section
        Public SectionName As String
        Public StartAddress As String
        Public EndAddress As String
        Public Tag As Object
    End Structure

    Public Structure Symbol
        Public Name As String
        Public Section As Section
        Public Offset As Long
        Public Tag As Object
    End Structure
    Public Structure CFunction
        Public Name As String
        Public StartAddress As String
        Public EndAddress As String
        Public RawAssembly As String
        Public Tag As Object
    End Structure

    Public Structure CVariable
        Public Name As String
        Public BaseAddress As String
        Public Offset As Long
        Public Size As String
        Public Scope As CFunction
        Public IsGlobal As Boolean
        Public Tag As Object
    End Structure




    Public CFunctionCollection As New List(Of CFunction)
    Public CVariableCollection As New List(Of CVariable)
    Public EntryPoint As String = String.Empty


End Class





