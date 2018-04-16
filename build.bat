set output_path=D:\SMS_Software\xrns2xmod

msbuild /t:rebuild /p:Configuration=Release /p:DebugSymbols=false /p:DebugType=None /p:Platform="x64" /p:OutputPath=%output_path%
