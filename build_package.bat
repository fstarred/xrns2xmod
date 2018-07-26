@echo off
rem	Windows script to build xrns2xmod windows package, requires PowerShell
rem place package_files.txt under same directory

set source_path=D:\SMS_Software\xrns2xmod\
set files=
set version=5.0.0.0
set output_file=xrns2xmod_cli_win_x64_%version%.zip

if exist %source_path%ini\*.ini del %source_path%ini\*.ini

rem Define location of files
set "ControlFile=package_files.txt"

rem Load control file values into variables
for /f "usebackq tokens=*" %%A in ("%ControlFile%") do (
  call :concat %%A
)


set files=%files:~0,-1%"

rem echo "& Compress-Archive -LiteralPath "%files%" -CompressionLevel Optimal  -DestinationPath "%source_path%binaries\%output_file%" -Force
powershell "& Compress-Archive -LiteralPath "%files%" -CompressionLevel Optimal  -DestinationPath "%source_path%binaries\%output_file%" -Force

:concat
set files=%files% %source_path%%1,
goto :eof