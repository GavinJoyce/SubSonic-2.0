@echo off
set "ReleasePath=C:\dev\3rdParty\SubSonic-2.0\SubCommander\bin\Release"
set "Destination1Path=C:\dev\MyHome.Common\Dependencies\SubSonic2.2"
set "Destination2Path=C:\dev\MyHomeV3\Dependencies\SubSonic2.2"

copy "%ReleasePath%\sonic.exe" "%Destination1Path%\SubCommander"
copy "%ReleasePath%\SubSonic.dll" "%Destination1Path%\SubCommander"
copy "%ReleasePath%\SubSonic.Migrations.dll" "%Destination1Path%\SubCommander"
copy "%ReleasePath%\SubSonic.dll" "%Destination1Path%"
copy "%ReleasePath%\SubSonic.Migrations.dll" "%Destination1Path%"

copy "%ReleasePath%\sonic.exe" "%Destination2Path%\SubCommander"
copy "%ReleasePath%\SubSonic.dll" "%Destination2Path%\SubCommander"
copy "%ReleasePath%\SubSonic.Migrations.dll" "%Destination2Path%\SubCommander"
copy "%ReleasePath%\SubSonic.dll" "%Destination2Path%"
copy "%ReleasePath%\SubSonic.Migrations.dll" "%Destination2Path%"
pause