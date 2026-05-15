@echo off
set UNITY="C:\Program Files\Unity\Hub\Editor\6000.2.6f2\Editor\Unity.exe"
set PROJECT=d:\GitHub\TestTask\TestTask
set LOG=%PROJECT%\BuildOutput\unity_build.log

echo Close Unity Editor before running this script.
echo.

%UNITY% -batchmode -nographics -quit -projectPath %PROJECT% -executeMethod Experiments.Features.Localization.Editor.Biology6ex14LocalizationBuild.BuildAll -logFile %LOG%

if %ERRORLEVEL% NEQ 0 (
    echo Build failed. See %LOG%
    exit /b %ERRORLEVEL%
)

echo.
echo Done. Output:
echo   WebGL bundles: %PROJECT%\BuildOutput\Biology_6ex14_ua_loc\WebGL
echo   unitypackage:  %PROJECT%\BuildOutput\Biology_6ex14_ua_loc\Biology_6ex14_ua_loc.unitypackage
pause
