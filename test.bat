@echo off
set output_test_path=D:\SMS_Software\xrns2xmod\test
set nunit_console_exe=.\packages\NUnit.ConsoleRunner.3.8.0\tools\nunit3-console.exe

rem PUSHD "%output_test_path%"

call :check_file "bass.dll" || goto :eof
call :check_file "bassmix.dll" || goto :eof
call :check_file "bassflac.dll" || goto :eof

%nunit_console_exe% %output_test_path%\Xrns2XModUnitTest.dll

:check_file
if not exist %output_test_path%\%~1 (
	echo "%~1 not found in %output_test_path%"
	exit /b 1
) else (
	exit /b 0
)