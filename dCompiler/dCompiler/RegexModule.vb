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
    Public AssemblyParser_variable_parser_regex As New Regex("[^\n\s]*\s*?[^\:]*?\:[\s]*mov[\s]*(DWORD|QWORD|BYTE|WORD|TBYTE) PTR\s*\[[re]bp([+-])(.*)\],[\S]*")
    '0x000000000040153d <+13>:	mov    DWORD PTR [rbp-0x4],0xa
    Public AssemblyParser_function_parser_regex As New Regex("[\S]*\s*?[^\:]*?\:[\s]*call[\s]*(0x[a-f0-9]*)[\s]*?(\<(.*)\>)?$")
    '   0x000000000040154f <+20>:	call   0x401530 <fun>
    Public AssemblyParser_codeline_regex As New Regex("(0x[0-9a-f]*)\s*?[^\:]*?\s*?:(.*)")

    Public AssemblyParser_jump_condition_regex As New Regex("\s*?((?:je)|(?:jz)|(?:jne)|(?:jnz)|(?:jg)|(?:jl)|(?:jnle)|(?:jge)|(?:jnl)|(?:je)|(?:jnge)|(?:jle)|(?:jng))\s*?(0x[0-9a-f]*)\s*?(?:\<(.*)\>)?$")

    Public AssemblyParser_jump_statement_regex As New Regex("\s*?jmp\s*?(0x[0-9a-f]*)\s*?(?:\<(.*)\>)?$")

    Public AssemblyParser_cmp_statement_regex As New Regex("\s*?cmp\s*?(.*)")

    Public AssemblyParser_mov_var_to_register_regex As New Regex("mov\s*([re][abcd]x)\s*?,\s*?(DWORD|QWORD|BYTE|WORD|TBYTE) PTR\s*\[[re]bp([+-])(.*)\]")

    Public AssemblyParser_mov_val_to_register_regex As New Regex("mov\s*([re][abcd]x)\s*?,\s*?(0x[a-f0-9]*)")

#End Region

#End Region

#Region "AssemblyTraverser"
#Region "Superficial"
    Public AssemblyTraverser_function_end_regex As New Regex("(?:0x[a-f0-9]*)\s*?[^\:\n]*:\s*?(?:pop\s*[re]bp\s*\r?\n\s*(0x[a-f0-9]*)\s*?[^\:\n]*:\s*?ret)")
    '0x0000000000401530 <+0>:	push   rbp
    '0x0000000000401531 <+1>:	mov    rbp, rsp
    '0x0000000000401534 <+4>:	mov    eax,0x0
    '0x0000000000401539 <+9>:	pop    rbp
    '0x000000000040153a <+10>:	ret    
    Public AssemblyTraverser_address_line_regex As New Regex("\n\s*?(0x[a-f0-9]*)\s*?[^\:\n]*:\s*?")
    Public AssemblyTraverser_raw_asm_capture_regex As New Regex("(?:(?:\(gdb\))? (?:\(gdb\))? Dump of assembler code from 0x[a-f0-9]* to 0x[a-f0-9]*\s*?:\s*?)+\r?\n?([\s\S]*)\r?\n\s*?(?:End of assembler dump.)\r?\n?\s*?(?:\(gdb\))?")
    '(gdb) (gdb) Dump of assembler code from 0x401530 to 0x40153b
    '0x0000000000401530 <fun+0>:	push   rbp
    '0x0000000000401531 <fun+1>:	mov    rbp, rsp
    '0x0000000000401534 <fun+4>:	mov    eax,0x0
    '0x0000000000401539 <fun+9>:	pop    rbp
    '0x000000000040153a <fun+10>:	ret    
    'End Of assembler dump.
    '(gdb) 


#End Region

#End Region

End Module
