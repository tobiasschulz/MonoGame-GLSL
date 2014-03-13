@echo off

setlocal enabledelayedexpansion

for %%i in (*.fx) do (
	set file=%%~ni
	echo !file!
	"..\..\..\Tools\2MGFX\2mgfx.exe" !file!.fx !file!.mgfx
)
pause

