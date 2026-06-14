@echo off
title Instalador Geral

:: Verifica se o .NET Runtime 8.0 esta instalado
dotnet --list-runtimes 2>nul | findstr "WindowsDesktop 8." >nul
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] .NET 8 Runtime nao encontrado!
    echo.
    echo Instale o .NET 8 Desktop Runtime em:
    echo https://aka.ms/dotnet/8.0/windowsdesktop-runtime-win-x64.exe
    echo.
    pause
    exit /b 1
)

set APP_DIR=%~dp0bin\Release\net8.0-windows
start "" "%APP_DIR%\InstaladorGeral.exe"
