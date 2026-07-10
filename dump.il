
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 2:0:0:0
}
.assembly extern Microsoft.Extensions.Logging.Abstractions
{
  .publickeytoken = (AD B9 79 38 29 DD AE 60 )                         // ..y8)..`
  .ver 2:0:0:0
}
.assembly extern Atatus
{
  .publickeytoken = (42 9F 11 61 3E 2F FB C7 )                         // B..a>/..
  .ver 3:2:0:0
}
.assembly Atatus.Extensions.Logging
{
  .custom instance void [netstandard]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [netstandard]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                                63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [netstandard]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [netstandard]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .custom instance void [netstandard]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 19 2E 4E 45 54 53 74 61 6E 64 61 72 64 2C   // ....NETStandard,
                                                                                                           56 65 72 73 69 6F 6E 3D 76 32 2E 30 01 00 54 0E   // Version=v2.0..T.
                                                                                                           14 46 72 61 6D 65 77 6F 72 6B 44 69 73 70 6C 61   // .FrameworkDispla
                                                                                                           79 4E 61 6D 65 11 2E 4E 45 54 20 53 74 61 6E 64   // yName..NET Stand
                                                                                                           61 72 64 20 32 2E 30 )                            // ard 2.0
  .custom instance void [netstandard]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 06 41 74 61 74 75 73 00 00 )                // ...Atatus..
  .custom instance void [netstandard]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 07 52 65 6C 65 61 73 65 00 00 )             // ...Release..
  .custom instance void [netstandard]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 0B 32 30 32 34 20 41 74 61 74 75 73 00 00 ) // ...2024 Atatus..
  .custom instance void [netstandard]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 80 9B 41 74 61 74 75 73 20 2E 4E 45 54 20   // ....Atatus .NET 
                                                                                                       45 78 74 65 6E 73 69 6F 6E 73 20 4C 6F 67 67 69   // Extensions Loggi
                                                                                                       6E 67 20 69 6E 74 65 67 72 61 74 65 73 20 77 69   // ng integrates wi
                                                                                                       74 68 20 77 69 74 68 20 4D 69 63 72 6F 73 6F 66   // th with Microsof
                                                                                                       74 2E 45 78 74 65 6E 73 69 6F 6E 73 2E 4C 6F 67   // t.Extensions.Log
                                                                                                       67 69 6E 67 2E 43 6F 6E 66 69 67 75 72 61 74 69   // ging.Configurati
                                                                                                       6F 6E 20 74 6F 20 63 61 70 74 75 72 65 20 6C 6F   // on to capture lo
                                                                                                       67 73 20 61 73 20 65 72 72 6F 72 73 20 61 6E 64   // gs as errors and
                                                                                                       20 63 6F 72 72 65 6C 61 74 65 20 74 68 65 6D 20   //  correlate them 
                                                                                                       77 69 74 68 20 41 50 4D 2E 0D 0A 20 20 20 20 00   // with APM...    .
                                                                                                       00 ) 
  .custom instance void [netstandard]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 33 2E 32 2E 32 2E 30 00 00 )             // ...3.2.2.0..
  .custom instance void [netstandard]System.Reflection.AssemblyInformationalVersionAttribute::.ctor(string) = ( 01 00 2E 33 2E 32 2E 32 2B 32 64 36 39 38 61 66   // ...3.2.2+2d698af
                                                                                                                36 34 61 33 65 31 66 63 30 63 63 63 30 32 36 62   // 64a3e1fc0ccc026b
                                                                                                                32 32 32 65 66 35 61 37 61 32 66 33 37 36 62 39   // 222ef5a7a2f376b9
                                                                                                                33 00 00 )                                        // 3..
  .custom instance void [netstandard]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 19 41 74 61 74 75 73 2E 45 78 74 65 6E 73   // ...Atatus.Extens
                                                                                                   69 6F 6E 73 2E 4C 6F 67 67 69 6E 67 00 00 )       // ions.Logging..
  .custom instance void [netstandard]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 19 41 74 61 74 75 73 2E 45 78 74 65 6E 73   // ...Atatus.Extens
                                                                                                 69 6F 6E 73 2E 4C 6F 67 67 69 6E 67 00 00 )       // ions.Logging..
  .custom instance void [netstandard]System.Runtime.CompilerServices.InternalsVisibleToAttribute::.ctor(string) = ( 01 00 81 65 41 74 61 74 75 73 2E 45 78 74 65 6E   // ...eAtatus.Exten
                                                                                                                    73 69 6F 6E 73 2E 48 6F 73 74 69 6E 67 2C 20 50   // sions.Hosting, P
                                                                                                                    75 62 6C 69 63 4B 65 79 3D 30 30 32 34 30 30 30   // ublicKey=0024000
                                                                                                                    30 30 34 38 30 30 30 30 30 39 34 30 30 30 30 30   // 0048000009400000
                                                                                                                    30 30 36 30 32 30 30 30 30 30 30 32 34 30 30 30   // 0060200000024000
                                                                                                                    30 35 32 35 33 34 31 33 31 30 30 30 34 30 30 30   // 0525341310004000
                                                                                                                    30 30 31 30 30 30 31 30 30 31 31 63 36 39 66 61   // 00100010011c69fa
                                                                                                                    61 30 35 63 62 37 39 38 35 61 61 63 37 66 37 30   // a05cb7985aac7f70
                                                                                                                    36 39 36 66 30 64 66 36 38 64 34 34 65 62 36 65   // 696f0df68d44eb6e
                                                                                                                    62 33 63 37 38 30 62 37 32 63 63 32 61 32 62 63   // b3c780b72cc2a2bc
                                                                                                                    61 34 61 65 30 62 37 36 35 65 36 65 35 37 61 35   // a4ae0b765e6e57a5
                                                                                                                    64 64 65 36 39 31 30 38 37 30 31 39 65 39 30 37   // dde691087019e907
                                                                                                                    64 66 30 39 32 31 38 30 37 36 32 61 37 39 33 64   // df092180762a793d
                                                                                                                    34 63 61 35 66 35 39 37 37 31 65 66 32 64 35 31   // 4ca5f59771ef2d51
                                                                                                                    34 65 36 36 36 39 32 63 32 32 64 37 30 39 61 30   // 4e66692c22d709a0
                                                                                                                    35 31 61 33 39 33 33 33 37 63 36 65 61 33 39 30   // 51a393337c6ea390
                                                                                                                    65 34 34 35 65 65 34 61 36 31 62 37 39 64 38 64   // e445ee4a61b79d8d
                                                                                                                    30 37 66 39 65 61 36 31 32 38 35 61 32 38 31 62   // 07f9ea61285a281b
                                                                                                                    34 35 30 62 61 62 38 66 32 66 38 62 38 37 35 36   // 450bab8f2f8b8756
                                                                                                                    66 35 64 30 37 63 30 64 34 38 36 38 62 39 34 31   // f5d07c0d4868b941
                                                                                                                    30 36 33 37 62 62 64 39 65 38 38 38 32 31 36 37   // 0637bbd9e8882167
                                                                                                                    36 62 35 36 35 33 61 63 37 66 65 32 64 37 31 30   // 6b5653ac7fe2d710
                                                                                                                    36 66 31 34 61 66 30 61 37 00 00 )                // 6f14af0a7..
  .publickey = (00 24 00 00 04 80 00 00 94 00 00 00 06 02 00 00   // .$..............
                00 24 00 00 52 53 41 31 00 04 00 00 01 00 01 00   // .$..RSA1........
                11 C6 9F AA 05 CB 79 85 AA C7 F7 06 96 F0 DF 68   // ......y........h
                D4 4E B6 EB 3C 78 0B 72 CC 2A 2B CA 4A E0 B7 65   // .N..<x.r.*+.J..e
                E6 E5 7A 5D DE 69 10 87 01 9E 90 7D F0 92 18 07   // ..z].i.....}....
                62 A7 93 D4 CA 5F 59 77 1E F2 D5 14 E6 66 92 C2   // b...._Yw.....f..
                2D 70 9A 05 1A 39 33 37 C6 EA 39 0E 44 5E E4 A6   // -p...937..9.D^..
                1B 79 D8 D0 7F 9E A6 12 85 A2 81 B4 50 BA B8 F2   // .y..........P...
                F8 B8 75 6F 5D 07 C0 D4 86 8B 94 10 63 7B BD 9E   // ..uo].......c{..
                88 82 16 76 B5 65 3A C7 FE 2D 71 06 F1 4A F0 A7 ) // ...v.e:..-q..J..
  .hash algorithm 0x00008004
  .ver 3:2:0:0
}
.module Atatus.Extensions.Logging.dll
// MVID: {17C582C9-00CD-440E-9A79-FE321E90E22B}
.custom instance void System.Runtime.CompilerServices.RefSafetyRulesAttribute::.ctor(int32) = ( 01 00 0B 00 00 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000009    //  ILONLY
// Image base: 0x07320000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi sealed beforefieldinit Microsoft.CodeAnalysis.EmbeddedAttribute
       extends [netstandard]System.Attribute
{
  .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [netstandard]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method EmbeddedAttribute::.ctor

} // end of class Microsoft.CodeAnalysis.EmbeddedAttribute

.class private auto ansi sealed beforefieldinit System.Runtime.CompilerServices.RefSafetyRulesAttribute
       extends [netstandard]System.Attribute
{
  .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [netstandard]System.AttributeUsageAttribute::.ctor(valuetype [netstandard]System.AttributeTargets) = ( 01 00 02 00 00 00 02 00 54 02 0D 41 6C 6C 6F 77   // ........T..Allow
                                                                                                                               4D 75 6C 74 69 70 6C 65 00 54 02 09 49 6E 68 65   // Multiple.T..Inhe
                                                                                                                               72 69 74 65 64 00 )                               // rited.
  .field public initonly int32 Version
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 A_1) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [netstandard]System.Attribute::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 System.Runtime.CompilerServices.RefSafetyRulesAttribute::Version
    IL_000d:  ret
  } // end of method RefSafetyRulesAttribute::.ctor

} // end of class System.Runtime.CompilerServices.RefSafetyRulesAttribute

.class private auto ansi beforefieldinit Atatus.Extensions.Logging.ApmErrorLogger
       extends [netstandard]System.Object
       implements [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILogger
{
  .class auto ansi nested private beforefieldinit NullScope
         extends [netstandard]System.Object
         implements [netstandard]System.IDisposable
  {
    .method public hidebysig newslot virtual final 
            instance void  Dispose() cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method NullScope::Dispose

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [netstandard]System.Object::.ctor()
      IL_0006:  ret
    } // end of method NullScope::.ctor

  } // end of class NullScope

  .field private initonly class [Atatus]Atatus.IApmAgent _agent
  .field private initonly string _categoryName
  .field private initonly class Atatus.Extensions.Logging.ApmErrorLogger/NullScope _nullScope
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class [Atatus]Atatus.IApmAgent agent,
                               string categoryName) cil managed
  {
    // Code size       38 (0x26)
    .maxstack  2
    .locals init (class [Atatus]Atatus.IApmAgent V_0,
             string V_1,
             class Atatus.Extensions.Logging.ApmErrorLogger/NullScope V_2)
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [netstandard]System.Object::.ctor()
    IL_0006:  ldarg.1
    IL_0007:  stloc.0
    IL_0008:  ldarg.2
    IL_0009:  stloc.1
    IL_000a:  newobj     instance void Atatus.Extensions.Logging.ApmErrorLogger/NullScope::.ctor()
    IL_000f:  stloc.2
    IL_0010:  ldarg.0
    IL_0011:  ldloc.0
    IL_0012:  stfld      class [Atatus]Atatus.IApmAgent Atatus.Extensions.Logging.ApmErrorLogger::_agent
    IL_0017:  ldarg.0
    IL_0018:  ldloc.1
    IL_0019:  stfld      string Atatus.Extensions.Logging.ApmErrorLogger::_categoryName
    IL_001e:  ldarg.0
    IL_001f:  ldloc.2
    IL_0020:  stfld      class Atatus.Extensions.Logging.ApmErrorLogger/NullScope Atatus.Extensions.Logging.ApmErrorLogger::_nullScope
    IL_0025:  ret
  } // end of method ApmErrorLogger::.ctor

  .method public hidebysig newslot virtual final 
          instance class [netstandard]System.IDisposable 
          BeginScope<TState>(!!TState state) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Atatus.Extensions.Logging.ApmErrorLogger/NullScope Atatus.Extensions.Logging.ApmErrorLogger::_nullScope
    IL_0006:  ret
  } // end of method ApmErrorLogger::BeginScope

  .method public hidebysig newslot virtual final 
          instance bool  IsEnabled(valuetype [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.LogLevel logLevel) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.4
    IL_0002:  clt
    IL_0004:  ldc.i4.0
    IL_0005:  ceq
    IL_0007:  ret
  } // end of method ApmErrorLogger::IsEnabled

  .method public hidebysig newslot virtual final 
          instance void  Log<TState>(valuetype [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.LogLevel logLevel,
                                     valuetype [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.EventId eventId,
                                     !!TState state,
                                     class [netstandard]System.Exception exception,
                                     class [netstandard]System.Func`3<!!TState,class [netstandard]System.Exception,string> formatter) cil managed
  {
    // Code size       546 (0x222)
    .maxstack  6
    .locals init (string V_0,
             class [Atatus]Atatus.Api.ErrorLog V_1,
             class [Atatus]Atatus.ApmAgent V_2,
             class [netstandard]System.Collections.Generic.IEnumerable`1<valuetype [netstandard]System.Collections.Generic.KeyValuePair`2<string,object>> V_3,
             class [netstandard]System.Exception V_4,
             valuetype [netstandard]System.Nullable`1<valuetype [Atatus]Atatus.Logging.LoggingExtensions/MaybeLogger> V_5,
             valuetype [Atatus]Atatus.Logging.LoggingExtensions/MaybeLogger V_6,
             class [netstandard]System.Collections.Generic.IEnumerator`1<valuetype [netstandard]System.Collections.Generic.KeyValuePair`2<string,object>> V_7,
             valuetype [netstandard]System.Collections.Generic.KeyValuePair`2<string,object> V_8,
             class [netstandard]System.Exception V_9)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string Atatus.Extensions.Logging.ApmErrorLogger::_categoryName
    IL_0006:  brfalse.s  IL_001a

    IL_0008:  ldarg.0
    IL_0009:  ldfld      string Atatus.Extensions.Logging.ApmErrorLogger::_categoryName
    IL_000e:  ldstr      "Atatus"
    IL_0013:  callvirt   instance bool [netstandard]System.String::StartsWith(string)
    IL_0018:  brfalse.s  IL_001b

    IL_001a:  ret

    IL_001b:  call       bool [Atatus]Atatus.Agent::get_IsConfigured()
    IL_0020:  brtrue.s   IL_0023

    IL_0022:  ret

    IL_0023:  ldarg.0
    IL_0024:  ldfld      class [Atatus]Atatus.IApmAgent Atatus.Extensions.Logging.ApmErrorLogger::_agent
    IL_0029:  callvirt   instance class [Atatus]Atatus.Config.IConfigurationReader [Atatus]Atatus.IApmAgent::get_Configuration()
    IL_002e:  callvirt   instance bool [Atatus]Atatus.Config.IConfigurationReader::get_Enabled()
    IL_0033:  brfalse.s  IL_0047

    IL_0035:  ldarg.0
    IL_0036:  ldfld      class [Atatus]Atatus.IApmAgent Atatus.Extensions.Logging.ApmErrorLogger::_agent
    IL_003b:  callvirt   instance class [Atatus]Atatus.Config.IConfigurationReader [Atatus]Atatus.IApmAgent::get_Configuration()
    IL_0040:  callvirt   instance bool [Atatus]Atatus.Config.IConfigurationReader::get_Recording()
    IL_0045:  brtrue.s   IL_0048

    IL_0047:  ret

    IL_0048:  ldarg.s    formatter
    IL_004a:  ldarg.3
    IL_004b:  ldarg.s    exception
    IL_004d:  callvirt   instance !2 class [netstandard]System.Func`3<!!TState,class [netstandard]System.Exception,string>::Invoke(!0,
                                                                                                                                   !1)
    IL_0052:  stloc.0
    .try
    {
      IL_0053:  ldarg.0
      IL_0054:  ldfld      class [Atatus]Atatus.IApmAgent Atatus.Extensions.Logging.ApmErrorLogger::_agent
      IL_0059:  callvirt   instance class [Atatus]Atatus.Api.ITracer [Atatus]Atatus.IApmAgent::get_Tracer()
      IL_005e:  ldloc.0
      IL_005f:  ldarga.s   logLevel
      IL_0061:  constrained. [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.LogLevel
      IL_0067:  callvirt   instance string [netstandard]System.Object::ToString()
      IL_006c:  callvirt   instance void [Atatus]Atatus.Api.ITracer::CaptureLog(string,
                                                                                string)
      IL_0071:  leave.s    IL_00b0

    }  // end .try
    catch [netstandard]System.Exception 
    {
      IL_0073:  stloc.s    V_4
      IL_0075:  ldarg.0
      IL_0076:  ldfld      class [Atatus]Atatus.IApmAgent Atatus.Extensions.Logging.ApmErrorLogger::_agent
      IL_007b:  callvirt   instance class [Atatus]Atatus.Logging.IApmLogger [Atatus]Atatus.IApmAgent::get_Logger()
      IL_0080:  call       valuetype [netstandard]System.Nullable`1<valuetype [Atatus]Atatus.Logging.LoggingExtensions/MaybeLogger> [Atatus]Atatus.Logging.LoggingExtensions::Warning(class [Atatus]Atatus.Logging.IApmLogger)
      IL_0085:  stloc.s    V_5
      IL_0087:  ldloca.s   V_5
      IL_0089:  dup
      IL_008a:  call       instance bool valuetype [netstandard]System.Nullable`1<valuetype [Atatus]Atatus.Logging.LoggingExtensions/MaybeLogger>::get_HasValue()
      IL_008f:  brtrue.s   IL_0094

      IL_0091:  pop
      IL_0092:  br.s       IL_00ae

      IL_0094:  call       instance !0 valuetype [netstandard]System.Nullable`1<valuetype [Atatus]Atatus.Logging.LoggingExtensions/MaybeLogger>::GetValueOrDefault()
      IL_0099:  stloc.s    V_6
      IL_009b:  ldloca.s   V_6
      IL_009d:  ldloc.s    V_4
      IL_009f:  ldstr      "Failed capturing APM Log based on log"
      IL_00a4:  call       !!0[] [netstandard]System.Array::Empty<object>()
      IL_00a9:  call       instance void [Atatus]Atatus.Logging.LoggingExtensions/MaybeLogger::LogException(class [netstandard]System.Exception,
                                                                                                            string,
                                                                                                            object[])
      IL_00ae:  leave.s    IL_00b0

    }  // end handler
    IL_00b0:  ldarg.0
    IL_00b1:  ldarg.1
    IL_00b2:  call       instance bool Atatus.Extensions.Logging.ApmErrorLogger::IsEnabled(valuetype [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.LogLevel)
    IL_00b7:  brtrue.s   IL_00ba

    IL_00b9:  ret

    IL_00ba:  ldloc.0
    IL_00bb:  newobj     instance void [Atatus]Atatus.Api.ErrorLog::.ctor(string)
    IL_00c0:  stloc.1
    IL_00c1:  ldarg.0
    IL_00c2:  ldfld      class [Atatus]Atatus.IApmAgent Atatus.Extensions.Logging.ApmErrorLogger::_agent
    IL_00c7:  isinst     [Atatus]Atatus.ApmAgent
    IL_00cc:  stloc.2
    IL_00cd:  ldloc.2
    IL_00ce:  brfalse.s  IL_0102

    IL_00d0:  ldarg.s    exception
    IL_00d2:  brfalse.s  IL_0102

    IL_00d4:  ldloc.1
    IL_00d5:  ldarg.s    exception
    IL_00d7:  ldarg.0
    IL_00d8:  ldfld      class [Atatus]Atatus.IApmAgent Atatus.Extensions.Logging.ApmErrorLogger::_agent
    IL_00dd:  callvirt   instance class [Atatus]Atatus.Logging.IApmLogger [Atatus]Atatus.IApmAgent::get_Logger()
    IL_00e2:  ldstr      "CaptureErrorLogsAsApmError"
    IL_00e7:  ldloc.2
    IL_00e8:  callvirt   instance class [Atatus]Atatus.Config.IConfigurationReader [Atatus]Atatus.ApmAgent::get_Configuration()
    IL_00ed:  ldloc.2
    IL_00ee:  callvirt   instance class [Atatus]Atatus.AgentComponents [Atatus]Atatus.ApmAgent::get_Components()
    IL_00f3:  callvirt   instance class [Atatus]Atatus.ServerInfo.IApmServerInfo [Atatus]Atatus.AgentComponents::get_ApmServerInfo()
    IL_00f8:  call       class [netstandard]System.Collections.Generic.List`1<class [Atatus]Atatus.Api.CapturedStackFrame> [Atatus]Atatus.Helpers.StacktraceHelper::GenerateApmStackTrace(class [netstandard]System.Exception,
                                                                                                                                                                                          class [Atatus]Atatus.Logging.IApmLogger,
                                                                                                                                                                                          string,
                                                                                                                                                                                          class [Atatus]Atatus.Config.IConfigurationReader,
                                                                                                                                                                                          class [Atatus]Atatus.ServerInfo.IApmServerInfo)
    IL_00fd:  callvirt   instance void [Atatus]Atatus.Api.ErrorLog::set_StackTrace(class [netstandard]System.Collections.Generic.List`1<class [Atatus]Atatus.Api.CapturedStackFrame>)
    IL_0102:  ldloc.1
    IL_0103:  ldarga.s   logLevel
    IL_0105:  constrained. [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.LogLevel
    IL_010b:  callvirt   instance string [netstandard]System.Object::ToString()
    IL_0110:  callvirt   instance void [Atatus]Atatus.Api.ErrorLog::set_Level(string)
    IL_0115:  ldarg.3
    IL_0116:  box        !!TState
    IL_011b:  isinst     class [netstandard]System.Collections.Generic.IEnumerable`1<valuetype [netstandard]System.Collections.Generic.KeyValuePair`2<string,object>>
    IL_0120:  stloc.3
    IL_0121:  ldloc.3
    IL_0122:  brfalse.s  IL_0173

    IL_0124:  ldloc.3
    IL_0125:  callvirt   instance class [netstandard]System.Collections.Generic.IEnumerator`1<!0> class [netstandard]System.Collections.Generic.IEnumerable`1<valuetype [netstandard]System.Collections.Generic.KeyValuePair`2<string,object>>::GetEnumerator()
    IL_012a:  stloc.s    V_7
    .try
    {
      IL_012c:  br.s       IL_015c

      IL_012e:  ldloc.s    V_7
      IL_0130:  callvirt   instance !0 class [netstandard]System.Collections.Generic.IEnumerator`1<valuetype [netstandard]System.Collections.Generic.KeyValuePair`2<string,object>>::get_Current()
      IL_0135:  stloc.s    V_8
      IL_0137:  ldloca.s   V_8
      IL_0139:  call       instance !0 valuetype [netstandard]System.Collections.Generic.KeyValuePair`2<string,object>::get_Key()
      IL_013e:  ldstr      "{OriginalFormat}"
      IL_0143:  call       bool [netstandard]System.String::op_Equality(string,
                                                                        string)
      IL_0148:  brfalse.s  IL_015c

      IL_014a:  ldloc.1
      IL_014b:  ldloca.s   V_8
      IL_014d:  call       instance !1 valuetype [netstandard]System.Collections.Generic.KeyValuePair`2<string,object>::get_Value()
      IL_0152:  callvirt   instance string [netstandard]System.Object::ToString()
      IL_0157:  callvirt   instance void [Atatus]Atatus.Api.ErrorLog::set_ParamMessage(string)
      IL_015c:  ldloc.s    V_7
      IL_015e:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
      IL_0163:  brtrue.s   IL_012e

      IL_0165:  leave.s    IL_0173

    }  // end .try
    finally
    {
      IL_0167:  ldloc.s    V_7
      IL_0169:  brfalse.s  IL_0172

      IL_016b:  ldloc.s    V_7
      IL_016d:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
      IL_0172:  endfinally
    }  // end handler
    IL_0173:  nop
    .try
    {
      IL_0174:  ldarg.0
      IL_0175:  ldfld      class [Atatus]Atatus.IApmAgent Atatus.Extensions.Logging.ApmErrorLogger::_agent
      IL_017a:  callvirt   instance class [Atatus]Atatus.Api.ITracer [Atatus]Atatus.IApmAgent::get_Tracer()
      IL_017f:  callvirt   instance class [Atatus]Atatus.Api.ISpan [Atatus]Atatus.Api.ITracer::get_CurrentSpan()
      IL_0184:  brfalse.s  IL_01a1

      IL_0186:  ldarg.0
      IL_0187:  ldfld      class [Atatus]Atatus.IApmAgent Atatus.Extensions.Logging.ApmErrorLogger::_agent
      IL_018c:  callvirt   instance class [Atatus]Atatus.Api.ITracer [Atatus]Atatus.IApmAgent::get_Tracer()
      IL_0191:  callvirt   instance class [Atatus]Atatus.Api.ISpan [Atatus]Atatus.Api.ITracer::get_CurrentSpan()
      IL_0196:  ldloc.1
      IL_0197:  ldnull
      IL_0198:  ldnull
      IL_0199:  ldnull
      IL_019a:  callvirt   instance void [Atatus]Atatus.Api.IExecutionSegment::CaptureErrorLog(class [Atatus]Atatus.Api.ErrorLog,
                                                                                               string,
                                                                                               class [netstandard]System.Exception,
                                                                                               class [netstandard]System.Collections.Generic.Dictionary`2<string,class [Atatus]Atatus.Api.Label>)
      IL_019f:  br.s       IL_01e2

      IL_01a1:  ldarg.0
      IL_01a2:  ldfld      class [Atatus]Atatus.IApmAgent Atatus.Extensions.Logging.ApmErrorLogger::_agent
      IL_01a7:  callvirt   instance class [Atatus]Atatus.Api.ITracer [Atatus]Atatus.IApmAgent::get_Tracer()
      IL_01ac:  callvirt   instance class [Atatus]Atatus.Api.ITransaction [Atatus]Atatus.Api.ITracer::get_CurrentTransaction()
      IL_01b1:  brfalse.s  IL_01ce

      IL_01b3:  ldarg.0
      IL_01b4:  ldfld      class [Atatus]Atatus.IApmAgent Atatus.Extensions.Logging.ApmErrorLogger::_agent
      IL_01b9:  callvirt   instance class [Atatus]Atatus.Api.ITracer [Atatus]Atatus.IApmAgent::get_Tracer()
      IL_01be:  callvirt   instance class [Atatus]Atatus.Api.ITransaction [Atatus]Atatus.Api.ITracer::get_CurrentTransaction()
      IL_01c3:  ldloc.1
      IL_01c4:  ldnull
      IL_01c5:  ldnull
      IL_01c6:  ldnull
      IL_01c7:  callvirt   instance void [Atatus]Atatus.Api.IExecutionSegment::CaptureErrorLog(class [Atatus]Atatus.Api.ErrorLog,
                                                                                               string,
                                                                                               class [netstandard]System.Exception,
                                                                                               class [netstandard]System.Collections.Generic.Dictionary`2<string,class [Atatus]Atatus.Api.Label>)
      IL_01cc:  br.s       IL_01e2

      IL_01ce:  ldarg.0
      IL_01cf:  ldfld      class [Atatus]Atatus.IApmAgent Atatus.Extensions.Logging.ApmErrorLogger::_agent
      IL_01d4:  callvirt   instance class [Atatus]Atatus.Api.ITracer [Atatus]Atatus.IApmAgent::get_Tracer()
      IL_01d9:  ldloc.1
      IL_01da:  ldnull
      IL_01db:  ldnull
      IL_01dc:  ldnull
      IL_01dd:  callvirt   instance void [Atatus]Atatus.Api.ITracer::CaptureErrorLog(class [Atatus]Atatus.Api.ErrorLog,
                                                                                     string,
                                                                                     class [netstandard]System.Exception,
                                                                                     class [netstandard]System.Collections.Generic.Dictionary`2<string,class [Atatus]Atatus.Api.Label>)
      IL_01e2:  leave.s    IL_0221

    }  // end .try
    catch [netstandard]System.Exception 
    {
      IL_01e4:  stloc.s    V_9
      IL_01e6:  ldarg.0
      IL_01e7:  ldfld      class [Atatus]Atatus.IApmAgent Atatus.Extensions.Logging.ApmErrorLogger::_agent
      IL_01ec:  callvirt   instance class [Atatus]Atatus.Logging.IApmLogger [Atatus]Atatus.IApmAgent::get_Logger()
      IL_01f1:  call       valuetype [netstandard]System.Nullable`1<valuetype [Atatus]Atatus.Logging.LoggingExtensions/MaybeLogger> [Atatus]Atatus.Logging.LoggingExtensions::Warning(class [Atatus]Atatus.Logging.IApmLogger)
      IL_01f6:  stloc.s    V_5
      IL_01f8:  ldloca.s   V_5
      IL_01fa:  dup
      IL_01fb:  call       instance bool valuetype [netstandard]System.Nullable`1<valuetype [Atatus]Atatus.Logging.LoggingExtensions/MaybeLogger>::get_HasValue()
      IL_0200:  brtrue.s   IL_0205

      IL_0202:  pop
      IL_0203:  br.s       IL_021f

      IL_0205:  call       instance !0 valuetype [netstandard]System.Nullable`1<valuetype [Atatus]Atatus.Logging.LoggingExtensions/MaybeLogger>::GetValueOrDefault()
      IL_020a:  stloc.s    V_6
      IL_020c:  ldloca.s   V_6
      IL_020e:  ldloc.s    V_9
      IL_0210:  ldstr      "Failed capturing APM Error based on log"
      IL_0215:  call       !!0[] [netstandard]System.Array::Empty<object>()
      IL_021a:  call       instance void [Atatus]Atatus.Logging.LoggingExtensions/MaybeLogger::LogException(class [netstandard]System.Exception,
                                                                                                            string,
                                                                                                            object[])
      IL_021f:  leave.s    IL_0221

    }  // end handler
    IL_0221:  ret
  } // end of method ApmErrorLogger::Log

} // end of class Atatus.Extensions.Logging.ApmErrorLogger

.class private auto ansi beforefieldinit Atatus.Extensions.Logging.ApmErrorLoggingProvider
       extends [netstandard]System.Object
       implements [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILoggerProvider,
                  [netstandard]System.IDisposable
{
  .field private initonly class [Atatus]Atatus.IApmAgent _apmAgent
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class [Atatus]Atatus.IApmAgent apmAgent) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [netstandard]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      class [Atatus]Atatus.IApmAgent Atatus.Extensions.Logging.ApmErrorLoggingProvider::_apmAgent
    IL_000d:  ret
  } // end of method ApmErrorLoggingProvider::.ctor

  .method public hidebysig newslot virtual final 
          instance class [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILogger 
          CreateLogger(string categoryName) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [Atatus]Atatus.IApmAgent Atatus.Extensions.Logging.ApmErrorLoggingProvider::_apmAgent
    IL_0006:  ldarg.1
    IL_0007:  newobj     instance void Atatus.Extensions.Logging.ApmErrorLogger::.ctor(class [Atatus]Atatus.IApmAgent,
                                                                                       string)
    IL_000c:  ret
  } // end of method ApmErrorLoggingProvider::CreateLogger

  .method public hidebysig newslot virtual final 
          instance void  Dispose() cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method ApmErrorLoggingProvider::Dispose

} // end of class Atatus.Extensions.Logging.ApmErrorLoggingProvider

.class public abstract auto ansi sealed beforefieldinit Atatus.Extensions.Logging.AtatusLoggingProviderRegistration
       extends [netstandard]System.Object
{
  .field private static initonly class [netstandard]System.Runtime.CompilerServices.ConditionalWeakTable`2<class [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILoggerFactory,object> Processed
  .field private static initonly object Marker
  .field private static initonly object Gate
  .method public hidebysig static bool  TryAddTo(class [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILoggerFactory loggerFactory,
                                                 class [Atatus]Atatus.IApmAgent agent) cil managed
  {
    // Code size       183 (0xb7)
    .maxstack  4
    .locals init (object V_0,
             bool V_1,
             object V_2,
             bool V_3,
             class [netstandard]System.Exception V_4,
             valuetype [netstandard]System.Nullable`1<valuetype [Atatus]Atatus.Logging.LoggingExtensions/MaybeLogger> V_5,
             valuetype [Atatus]Atatus.Logging.LoggingExtensions/MaybeLogger V_6)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0006

    IL_0003:  ldarg.1
    IL_0004:  brtrue.s   IL_0008

    IL_0006:  ldc.i4.0
    IL_0007:  ret

    IL_0008:  ldsfld     object Atatus.Extensions.Logging.AtatusLoggingProviderRegistration::Gate
    IL_000d:  stloc.0
    IL_000e:  ldc.i4.0
    IL_000f:  stloc.1
    .try
    {
      IL_0010:  ldloc.0
      IL_0011:  ldloca.s   V_1
      IL_0013:  call       void [netstandard]System.Threading.Monitor::Enter(object,
                                                                             bool&)
      IL_0018:  ldsfld     class [netstandard]System.Runtime.CompilerServices.ConditionalWeakTable`2<class [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILoggerFactory,object> Atatus.Extensions.Logging.AtatusLoggingProviderRegistration::Processed
      IL_001d:  ldarg.0
      IL_001e:  ldloca.s   V_2
      IL_0020:  callvirt   instance bool class [netstandard]System.Runtime.CompilerServices.ConditionalWeakTable`2<class [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILoggerFactory,object>::TryGetValue(!0,
                                                                                                                                                                                                                                     !1&)
      IL_0025:  brfalse.s  IL_002e

      IL_0027:  ldc.i4.0
      IL_0028:  stloc.3
      IL_0029:  leave      IL_00b5

      IL_002e:  ldarg.0
      IL_002f:  call       bool Atatus.Extensions.Logging.AtatusLoggingProviderRegistration::ContainsApmProvider(class [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILoggerFactory)
      IL_0034:  brfalse.s  IL_004a

      IL_0036:  ldsfld     class [netstandard]System.Runtime.CompilerServices.ConditionalWeakTable`2<class [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILoggerFactory,object> Atatus.Extensions.Logging.AtatusLoggingProviderRegistration::Processed
      IL_003b:  ldarg.0
      IL_003c:  ldsfld     object Atatus.Extensions.Logging.AtatusLoggingProviderRegistration::Marker
      IL_0041:  callvirt   instance void class [netstandard]System.Runtime.CompilerServices.ConditionalWeakTable`2<class [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILoggerFactory,object>::Add(!0,
                                                                                                                                                                                                                             !1)
      IL_0046:  ldc.i4.0
      IL_0047:  stloc.3
      IL_0048:  leave.s    IL_00b5

      IL_004a:  nop
      .try
      {
        IL_004b:  ldarg.0
        IL_004c:  ldarg.1
        IL_004d:  newobj     instance void Atatus.Extensions.Logging.ApmErrorLoggingProvider::.ctor(class [Atatus]Atatus.IApmAgent)
        IL_0052:  callvirt   instance void [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILoggerFactory::AddProvider(class [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILoggerProvider)
        IL_0057:  ldsfld     class [netstandard]System.Runtime.CompilerServices.ConditionalWeakTable`2<class [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILoggerFactory,object> Atatus.Extensions.Logging.AtatusLoggingProviderRegistration::Processed
        IL_005c:  ldarg.0
        IL_005d:  ldsfld     object Atatus.Extensions.Logging.AtatusLoggingProviderRegistration::Marker
        IL_0062:  callvirt   instance void class [netstandard]System.Runtime.CompilerServices.ConditionalWeakTable`2<class [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILoggerFactory,object>::Add(!0,
                                                                                                                                                                                                                               !1)
        IL_0067:  ldc.i4.1
        IL_0068:  stloc.3
        IL_0069:  leave.s    IL_00b5

      }  // end .try
      catch [netstandard]System.Exception 
      {
        IL_006b:  stloc.s    V_4
        IL_006d:  ldarg.1
        IL_006e:  callvirt   instance class [Atatus]Atatus.Logging.IApmLogger [Atatus]Atatus.IApmAgent::get_Logger()
        IL_0073:  dup
        IL_0074:  brtrue.s   IL_0079

        IL_0076:  pop
        IL_0077:  br.s       IL_00a7

        IL_0079:  call       valuetype [netstandard]System.Nullable`1<valuetype [Atatus]Atatus.Logging.LoggingExtensions/MaybeLogger> [Atatus]Atatus.Logging.LoggingExtensions::Warning(class [Atatus]Atatus.Logging.IApmLogger)
        IL_007e:  stloc.s    V_5
        IL_0080:  ldloca.s   V_5
        IL_0082:  dup
        IL_0083:  call       instance bool valuetype [netstandard]System.Nullable`1<valuetype [Atatus]Atatus.Logging.LoggingExtensions/MaybeLogger>::get_HasValue()
        IL_0088:  brtrue.s   IL_008d

        IL_008a:  pop
        IL_008b:  br.s       IL_00a7

        IL_008d:  call       instance !0 valuetype [netstandard]System.Nullable`1<valuetype [Atatus]Atatus.Logging.LoggingExtensions/MaybeLogger>::GetValueOrDefault()
        IL_0092:  stloc.s    V_6
        IL_0094:  ldloca.s   V_6
        IL_0096:  ldloc.s    V_4
        IL_0098:  ldstr      "Failed adding Atatus log capture provider to the l"
        + "ogger factory"
        IL_009d:  call       !!0[] [netstandard]System.Array::Empty<object>()
        IL_00a2:  call       instance void [Atatus]Atatus.Logging.LoggingExtensions/MaybeLogger::LogException(class [netstandard]System.Exception,
                                                                                                              string,
                                                                                                              object[])
        IL_00a7:  ldc.i4.0
        IL_00a8:  stloc.3
        IL_00a9:  leave.s    IL_00b5

      }  // end handler
    }  // end .try
    finally
    {
      IL_00ab:  ldloc.1
      IL_00ac:  brfalse.s  IL_00b4

      IL_00ae:  ldloc.0
      IL_00af:  call       void [netstandard]System.Threading.Monitor::Exit(object)
      IL_00b4:  endfinally
    }  // end handler
    IL_00b5:  ldloc.3
    IL_00b6:  ret
  } // end of method AtatusLoggingProviderRegistration::TryAddTo

  .method private hidebysig static bool  ContainsApmProvider(class [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILoggerFactory loggerFactory) cil managed
  {
    // Code size       147 (0x93)
    .maxstack  3
    .locals init (class [netstandard]System.Reflection.FieldInfo V_0,
             class [netstandard]System.Collections.IEnumerable V_1,
             class [netstandard]System.Collections.IEnumerator V_2,
             object V_3,
             class [netstandard]System.Reflection.FieldInfo V_4,
             bool V_5,
             class [netstandard]System.IDisposable V_6)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance class [netstandard]System.Type [netstandard]System.Object::GetType()
      IL_0006:  ldstr      "_providerRegistrations"
      IL_000b:  ldc.i4.s   36
      IL_000d:  callvirt   instance class [netstandard]System.Reflection.FieldInfo [netstandard]System.Type::GetField(string,
                                                                                                                      valuetype [netstandard]System.Reflection.BindingFlags)
      IL_0012:  stloc.0
      IL_0013:  ldloc.0
      IL_0014:  brtrue.s   IL_0019

      IL_0016:  ldnull
      IL_0017:  br.s       IL_0020

      IL_0019:  ldloc.0
      IL_001a:  ldarg.0
      IL_001b:  callvirt   instance object [netstandard]System.Reflection.FieldInfo::GetValue(object)
      IL_0020:  isinst     [netstandard]System.Collections.IEnumerable
      IL_0025:  stloc.1
      IL_0026:  ldloc.1
      IL_0027:  brfalse.s  IL_0089

      IL_0029:  ldloc.1
      IL_002a:  callvirt   instance class [netstandard]System.Collections.IEnumerator [netstandard]System.Collections.IEnumerable::GetEnumerator()
      IL_002f:  stloc.2
      .try
      {
        IL_0030:  br.s       IL_006b

        IL_0032:  ldloc.2
        IL_0033:  callvirt   instance object [netstandard]System.Collections.IEnumerator::get_Current()
        IL_0038:  stloc.3
        IL_0039:  ldloc.3
        IL_003a:  brfalse.s  IL_006b

        IL_003c:  ldloc.3
        IL_003d:  callvirt   instance class [netstandard]System.Type [netstandard]System.Object::GetType()
        IL_0042:  ldstr      "Provider"
        IL_0047:  ldc.i4.s   20
        IL_0049:  callvirt   instance class [netstandard]System.Reflection.FieldInfo [netstandard]System.Type::GetField(string,
                                                                                                                        valuetype [netstandard]System.Reflection.BindingFlags)
        IL_004e:  stloc.s    V_4
        IL_0050:  ldloc.s    V_4
        IL_0052:  brtrue.s   IL_0057

        IL_0054:  ldnull
        IL_0055:  br.s       IL_005f

        IL_0057:  ldloc.s    V_4
        IL_0059:  ldloc.3
        IL_005a:  callvirt   instance object [netstandard]System.Reflection.FieldInfo::GetValue(object)
        IL_005f:  isinst     Atatus.Extensions.Logging.ApmErrorLoggingProvider
        IL_0064:  brfalse.s  IL_006b

        IL_0066:  ldc.i4.1
        IL_0067:  stloc.s    V_5
        IL_0069:  leave.s    IL_0090

        IL_006b:  ldloc.2
        IL_006c:  callvirt   instance bool [netstandard]System.Collections.IEnumerator::MoveNext()
        IL_0071:  brtrue.s   IL_0032

        IL_0073:  leave.s    IL_0089

      }  // end .try
      finally
      {
        IL_0075:  ldloc.2
        IL_0076:  isinst     [netstandard]System.IDisposable
        IL_007b:  stloc.s    V_6
        IL_007d:  ldloc.s    V_6
        IL_007f:  brfalse.s  IL_0088

        IL_0081:  ldloc.s    V_6
        IL_0083:  callvirt   instance void [netstandard]System.IDisposable::Dispose()
        IL_0088:  endfinally
      }  // end handler
      IL_0089:  leave.s    IL_008e

    }  // end .try
    catch [netstandard]System.Object 
    {
      IL_008b:  pop
      IL_008c:  leave.s    IL_008e

    }  // end handler
    IL_008e:  ldc.i4.0
    IL_008f:  ret

    IL_0090:  ldloc.s    V_5
    IL_0092:  ret
  } // end of method AtatusLoggingProviderRegistration::ContainsApmProvider

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  8
    IL_0000:  newobj     instance void class [netstandard]System.Runtime.CompilerServices.ConditionalWeakTable`2<class [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILoggerFactory,object>::.ctor()
    IL_0005:  stsfld     class [netstandard]System.Runtime.CompilerServices.ConditionalWeakTable`2<class [Microsoft.Extensions.Logging.Abstractions]Microsoft.Extensions.Logging.ILoggerFactory,object> Atatus.Extensions.Logging.AtatusLoggingProviderRegistration::Processed
    IL_000a:  newobj     instance void [netstandard]System.Object::.ctor()
    IL_000f:  stsfld     object Atatus.Extensions.Logging.AtatusLoggingProviderRegistration::Marker
    IL_0014:  newobj     instance void [netstandard]System.Object::.ctor()
    IL_0019:  stsfld     object Atatus.Extensions.Logging.AtatusLoggingProviderRegistration::Gate
    IL_001e:  ret
  } // end of method AtatusLoggingProviderRegistration::.cctor

} // end of class Atatus.Extensions.Logging.AtatusLoggingProviderRegistration


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file dump.res
