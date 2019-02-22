# dCompiler

The **dCompiler** project aims to produce user-friendly C source code from unstripped and stripped binaries.

In this project, we will be using GDB (GNU Debugger) to extract the raw assembly from Portable Executable Files.
We use a GdbInterface to accomplish this task. We need to specify the path of the binary and the path to gdb.exe.

**VB**
```VB
Dim gdbInterface As New GdbInterface
gdbInterface.GdbPath="PATH_TO_GDB"
gdbInterface.BinPath="PATH_TO_BINARY"
```

**C#**
```C#
GdbInterface gdbInterface=new GdbInterface();
gdbInterface.GdbPath="PATH_TO_GDB";
gdbInterface.BinPath="PATH_TO_BINARY";
```

Once we have set up our GdbInterface, we can proceed with setting up our SymbolProcessor.
The SymbolProcessor is dedicated to capture symbols from the raw binary, generating generalised addresses, parsing sections etc.
The `CreateSectionCollection()` method is used to manage the sections of the raw binary.
The `GenerateSchema()` method is used to set up the generalised address parsing functionality, etc.

**Notice** that we need to pass the GdbInterface that we created earlier as an argument to the SymbolProcessor. 

**VB**
```VB
Dim symProc = New SymbolProcessor(gdbInterface)
symProc.CreateSectionCollection()
symProc.GenerateSchema()
```

**C#**
```C#
SymbolProcessor symProc=new SymbolProcessor(gdbInterface);
symProc.CreateSectionCollection();
symProc.GenerateSchema();
```


