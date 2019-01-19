Imports dCompiler.SymbolTable
Imports System.Text.RegularExpressions

Public Class AssemblyParser
    Private rawAsm As String = String.Empty
    Private asmTraverser As AssemblyTraverser

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

    End Sub

    Public Function GetVariables(rawAsm As String) As List(Of CVariable)
        If rawAsm Is Nothing Then Return New List(Of CVariable)
        Dim cvarList As New List(Of CVariable)
        Dim data As String() = rawAsm.Split(Environment.NewLine)
        For Each line In data
            If AssemblyParser_variable_parser_regex.IsMatch(line) Then
                Dim match As Match = AssemblyParser_variable_parser_regex.Match(line)
                Dim cvar As CVariable
                cvar.Name = "var_" & match.Groups(1).Value.Remove(0, 1)
                cvar.Offset = match.Groups(1).Value
                cvar.BaseAddress = ""


            End If
        Next
    End Function

    Public Function GetFunctions(rawAsm As String) As List(Of CFunction)
        If rawAsm = String.Empty Then Return New List(Of CFunction)
        Dim cfunList As New List(Of CFunction)
        Dim data As String() = rawAsm.Split(Environment.NewLine)
        For Each line In data
            If AssemblyParser_function_parser_regex.IsMatch(line) Then
                Dim match As Match = AssemblyParser_function_parser_regex.Match(line)
                Dim cfun As CFunction
                If match.Groups.Count = 2 Then
                    cfun.Name = match.Groups(2).Value
                Else
                    cfun.Name = "fun_" & match.Groups(1).Value
                End If
                cfun.StartAddress = match.Groups(1).Value
                cfun.EndAddress = asmTraverser.GetFunction(cfun.StartAddress).EndAddress
                cfunList.Add(cfun)

            End If
        Next
        Return cfunList
    End Function



End Class
