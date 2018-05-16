set output_path=D:\SMS_Software\xrns2xmod
set output_test_path=D:\SMS_Software\xrns2xmod\test

msbuild /t:rebuild /p:Configuration=Release /p:DebugSymbols=false /p:DebugType=None /p:Platform="x64" /p:OutputPath=%output_path%

msbuild Xrns2XModUnitTest\Xrns2XModUnitTest.csproj /t:rebuild /p:Configuration=Release /p:DebugSymbols=false /p:DebugType=None /p:Platform="x64" /p:OutputPath=%output_test_path%


