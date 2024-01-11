@echo off

SET mypath=%~dp0
where /q pwsh
SET exitcode=1
IF ERRORLEVEL 1 (
    powershell -ExecutionPolicy Unrestricted -File " & ""%mypath%\Open-Project.ps1" ""%*"" "
    SET exitcode=%ERRORLEVEL%
) ELSE (
    pwsh -ExecutionPolicy Unrestricted -File " & ""%mypath%\Open-Project.ps1"" ""%*"" "
    SET exitcode=%ERRORLEVEL%
)

IF "%exitcode%"=="1" (
  EXIT /B
)

powershell -executionpolicy unrestricted -File OpenProject.ps1 %*
