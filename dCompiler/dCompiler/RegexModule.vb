Imports System.Text.RegularExpressions


Module RegexModule
#Region "Symbol Processor"
#Region "Processing"
    Public SymbolProcessor_get_symbol_at_address_regex As New Regex("([^\n\s]*)\s?(\+\s(\d*))? in section ([^\n\s]*)")
    ' fun + 1 in section .text
    Public SymbolProcessor_get_entry_point_regex As New Regex("Entry point: (0x[a-f0-9]*)")
    'Entry point: 0x401500
    Public SymbolProcessor_function_call_regex As New Regex("(0x[a-f0-9]*)")
#End Region
#Region "Segment Collection"
    Public segmentCollection_memRangeRegex As New Regex("([^-\n]*)\s\-\s([^-]*) is (.*)")

#End Region
#Region "Symbol Table"
    Public symbolTable_NotExists As New Regex("No symbol table loaded.")
#End Region
#End Region

#Region "Assembly Parser"
#Region "ElementParsing"
    Public AssemblyParser_variable_parser_regex As New Regex("[^\n\s]*\s*?[^\:]*?\:[\s]*mov[\s]*[^\n]* \[[re]bp(.*)\],[\S]*")
    '0x000000000040153d <+13>:	mov    DWORD PTR [rbp-0x4],0xa
    Public AssemblyParser_function_parser_regex As New Regex("[\S]*\s*?[^\:]*?\:[\s]*call[\s]*(0x[a-f0-9]*)[\s]*?(\<(.*)\>)?$")
    '   0x000000000040154f <+20>:	call   0x401530 <fun>

#End Region

#End Region

#Region "AssemblyTraverser"
#Region "Superficial"
    Public AssemblyTraverser_function_end_regex As New Regex("(0x[a-f0-9]*)\s*?[^\:]*:\s*?nop")

#End Region

#End Region

End Module
