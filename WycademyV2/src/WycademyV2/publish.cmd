@ECHO off

SET PUBLISH_DIR="D:\Documents\Visual Studio 2017\Projects\Git\Wycademy\WycademyV2\src\WycademyV2\bin\Release\netcoreapp2.1\publish"
SET DATA_DIR="D:\Documents\Visual Studio 2017\Projects\Git\Wycademy\WycademyV2\src\WycademyV2\bin\Release\netcoreapp2.1\publish\Data"
SET SCRAPER="D:\Documents\PyCharm\KiranicoScraper\generate.py"

IF EXIST %DATA_DIR% (GOTO clean)

:build
dotnet publish -c Release
MKDIR %DATA_DIR%
python %SCRAPER% -o %DATA_DIR%
PUSHD %PUBLISH_DIR%
7z a -tzip -y ..\release.zip *
PAUSE
EXIT

:clean
RMDIR /S %DATA_DIR%
GOTO build