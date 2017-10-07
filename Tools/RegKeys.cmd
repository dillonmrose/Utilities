REM These keys are needed no matter what to debug
REG ADD "HKLM\SOFTWARE\Microsoft\Cortana\Testability" /v EnableTestability /t REG_DWORD /d 1 /f
REG ADD "HKLM\SOFTWARE\Microsoft\Cortana\Testability\Search" /v QueryFormulationEndPoint /t REG_SZ /f /d "https://www.bing.com/AS/API/WindowsCortanaPane/V2/Init?addfeaturesnoexpansion=AlwaysUseRafForWebTopHit,taskcompletion"
		
REM This key changes endpoint for text based cortana interaction
REM REG ADD "HKLM\Software\Microsoft\Speech\Dev" /v HalseySnrEndpointText /t REG_SZ /f /d "http://%snr%/search"
REG ADD "HKLM\Software\Microsoft\Speech\Dev" /v HalseySnrEndpointText /t REG_SZ /f /d "http://drose-desk/search"
REM This key will be appended to end of cortana text interaction query
REM REG ADD "HKLM\Software\Microsoft\Speech\Dev" /v HalseySnrEndpointParametersText /t REG_SZ /f /d "input=1&snrtrace=1&setflight=taskcompletion&p1=%5BReactivehost+endpoint%3D%22<TCP endpoint>%22%5D"

REM These keys are for changing audio endpoint.  It doesnt work unless you are also running local LU
REM REG ADD "HKLM\Software\Microsoft\Speech\Dev" /v HalseySnrEndpointParametersAudio /t REG_SZ /f /d "speech=1&input=2&snrtrace=1&setflight=taskcompletion&p1=%5BReactivehost+endpoint%3D%22<TCP endpoint>%22%5D"
REM REG ADD "HKLM\Software\Microsoft\Speech\Dev" /v HalseySnrEndpointAudio /t REG_SZ /f /d "http://%snr%/speech_render"

@ECHO Restarting Cortana
taskkill /f /im searchui.exe 