@echo off
rem Run this batch from Developer Command Prompt for VS 2xxx

msbuild -version 2>NUL || call :no_command_found

if errorlevel  1 (
	exit /B 1	
)

set output_path=D:\SMS_Software\xrns2xmod
set output_test_path=D:\SMS_Software\xrns2xmod\test

msbuild Xrns2XModCmd/Xrns2XModCmd.csproj /t:rebuild /p:Configuration=Release /p:DebugSymbols=false /p:DebugType=None /p:Platform="x64" /p:OutputPath=%output_path%

msbuild Xrns2XModUnitTest\Xrns2XModUnitTest.csproj /t:rebuild /p:Configuration=Release /p:DebugSymbols=false /p:DebugType=None /p:Platform="x64" /p:OutputPath=%output_test_path%

:no_command_found
echo This batch must be launched from Developer Command Prompt for VS 2xxx
exit /B 1
goto :eof