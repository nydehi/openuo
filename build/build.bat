@echo off
@echo.
@echo -----------------------------------------------------
@echo -----------------------------------------------------
@echo - 
@echo - This process will build the OpenUO Ultima Online 
@echo - Client
@echo -
@echo -----------------------------------------------------
@echo -----------------------------------------------------
@echo.
pause
@echo.
@echo Building xen for XNA GameStudio 4.0
@echo This may take a minute or so...
@echo.

@echo Building...
@echo.

%SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /p:Configuration=Release /p:Optimize=true /p:DebugSymbols=false /verbosity:minimal .\..\src\client\client.sln

REM @echo.
REM @echo Merging...(this may take a few minutes)
REM @echo.

REM .\ILMerge.exe /target:winexe /out:.\..\bin\Release\OpenUO.exe .\..\bin\Release\Client.exe .\..\bin\Release\Ninject.dll .\..\bin\Release\SharpDX.Diagnostics.dll .\..\bin\Release\SharpDX.Direct3D9.dll .\..\bin\Release\SharpDX.dll
@echo.
@echo.
@echo -----------------------------------
@echo -----------------------------------
@echo - If part of the build failed,
@echo - Check the following are installed:
@echo -
@echo - .NET Framework 4.0
@echo - DirectX SDK (sometimes required)
@echo -----------------------------------
@echo -----------------------------------
pause