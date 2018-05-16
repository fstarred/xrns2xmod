@echo off
set output_test_path=D:\SMS_Software\xrns2xmod\test

rem PUSHD "%output_test_path%"

call :check_file "bass.dll" || goto :eof
call :check_file "bassmix.dll" || goto :eof
call :check_file "bassflac.dll" || goto :eof

VSTest.Console %output_test_path%\Xrns2XModUnitTest.dll /Platform:x64 /TestCaseFilter:"FullyQualifiedName~Xrns2XModUnitTest.UnitTestMod" /inIsolation /Settings:%output_test_path%\default.runsettings

VSTest.Console %output_test_path%\Xrns2XModUnitTest.dll /Platform:x64 /TestCaseFilter:"FullyQualifiedName~Xrns2XModUnitTest.UnitTestXM" /inIsolation /Settings:%output_test_path%\default.runsettings

:check_file
if not exist %output_test_path%\%~1 (
	echo "%~1 not found in %output_test_path%"
	exit /b 1
) else (
	exit /b 0
)