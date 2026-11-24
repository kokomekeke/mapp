@echo off
rem SgxMapDemo kiadasi build a Build\SgxMapDemo mappaba. Duplaklikkel is indithato.
rem Alapertelmezes: self-contained (a .NET runtime is a mappaba kerul, telepites nelkul fut).
rem Parameter: -FrameworkDependent  (kisebb, de a celgepen kell a .NET 8 Desktop Runtime)
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0build-sgxmapdemo.ps1" %*
if errorlevel 1 pause
