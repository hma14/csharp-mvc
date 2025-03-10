@if "%SCM_TRACE_LEVEL%" NEQ "4" @echo off


echo %WEBSITE_SITE_NAME% | findstr /C:"omnae-coreapi-stg" > nul && (
    SET FEATURES=STAGING
) || (
    SET FEATURES=
)
echo %WEBSITE_SITE_NAME% | findstr /C:"omnae-coreapi" > nul && (
    echo Compiling WebAPI
    call deploy.OmnaeWebApi.cmd 
) || (
    echo Compiling Web
    call deploy.OmnaeWeb.cmd
)