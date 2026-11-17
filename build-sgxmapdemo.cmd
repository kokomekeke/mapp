@echo off
rem SgxMapDemo kiadasi build a Build\SgxMapDemo mappaba. Duplaklikkel is indithato.
rem Parameter: -SelfContained  (a .NET runtime-ot is a mappaba teszi)
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0build-sgxmapdemo.ps1" %*
if errorlevel 1 pause
