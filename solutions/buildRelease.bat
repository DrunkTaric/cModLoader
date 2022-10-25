MSBuild tModLoader.sln /restore /t:clean /p:Configuration=WindowsRelease
MSBuild tModLoader.sln /t:clean /p:Configuration=WindowsServerRelease
MSBuild tModLoader.sln /t:clean /p:Configuration=MacRelease
MSBuild tModLoader.sln /t:clean /p:Configuration=MacServerRelease
MSBuild tModLoader.sln /t:clean /p:Configuration=LinuxRelease
MSBuild tModLoader.sln /t:clean /p:Configuration=LinuxServerRelease
MSBuild tModLoader.sln /p:Configuration=WindowsRelease /p:GenerateDocumentation=true /p:DocumentationFile=tModLoader.xml
@IF %ERRORLEVEL% NEQ 0 (EXIT /B %ERRORLEVEL%)
MSBuild tModLoader.sln /p:Configuration=WindowsServerRelease
@IF %ERRORLEVEL% NEQ 0 (EXIT /B %ERRORLEVEL%)
MSBuild tModLoader.sln /p:Configuration=MacRelease
@IF %ERRORLEVEL% NEQ 0 (EXIT /B %ERRORLEVEL%)
MSBuild tModLoader.sln /p:Configuration=MacServerRelease
@IF %ERRORLEVEL% NEQ 0 (EXIT /B %ERRORLEVEL%)
MSBuild tModLoader.sln /p:Configuration=LinuxRelease
@IF %ERRORLEVEL% NEQ 0 (EXIT /B %ERRORLEVEL%)
MSBuild tModLoader.sln /p:Configuration=LinuxServerRelease
@IF %ERRORLEVEL% NEQ 0 (EXIT /B %ERRORLEVEL%)