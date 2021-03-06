﻿Imports System.Text.RegularExpressions

Module dcModule
    Public gdb_path_default As String = "MinGW64\bin\gdb.exe"

    Public gdb_pid As Integer = 0
    Public output_buffer As String
    Public Function ConvertHexToLong(str As String) As Long
        Dim number As Long = 0
        Dim i As Integer = 0
        Dim new_str As String = str.Remove(0, 2)
        new_str = StrReverse(new_str)
        For Each c As Char In new_str
            If Asc(c) < 58 Then
                number += Math.Pow(16, i) * (Asc(c) - 48)
            Else
                number += Math.Pow(16, i) * (Asc(c) - Asc("a") + 10)
            End If
            i += 1
        Next
        Return number
    End Function
    Public Function IsRegister(str As String)
        Dim regex As New Regex("[\s]*?[re][abcd]x[\s]*?[\r\n]*?")
        If str = Nothing Then
            Return False
        End If
        If regex.IsMatch(str.Trim) Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function IsValue(str As String)
        If str = Nothing Then
            Return False
        End If
        If str.Trim.StartsWith("0x") Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function ConvertLongToHex(number As Long) As String
        Return "0x" & Hex(number).ToLower()
    End Function

    Public Function GetMultiple(x As String, m As Integer)
        Dim y As String = ""
        While m > 0
            y &= x
            m -= 1
        End While
        Return y
    End Function


    Public Function GiveIndentation(m As Integer) As String
        Return GetMultiple(vbTab, m)
    End Function
End Module


