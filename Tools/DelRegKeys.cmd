
REM Add registry keys to enable all tasks behind the fli

REG DELETE "HKLM\SOFTWARE\Microsoft\Cortana\Testability" /v EnableTestability /f
REG DELETE "HKLM\SOFTWARE\Microsoft\Cortana\Testability" /v QueryFormulationEndPoint /f
REG DELETE "HKLM\SOFTWARE\Microsoft\Cortana\Testability\Search" /v QueryFormulationEndPoint /f
		
REM Add registry keys to enable all tasks behind the flight

REG DELETE "HKLM\Software\Microsoft\Speech\Dev" /v HalseySnrEndpointParametersText /f
REG DELETE "HKLM\Software\Microsoft\Speech\Dev" /v HalseySnrEndpointParametersAudio /f
REG DELETE "HKLM\Software\Microsoft\Speech\Dev" /v HalseySnrEndpointText /f
REG DELETE "HKLM\Software\Microsoft\Speech\Dev" /v HalseySnrEndpointAudio /f

@ECHO Restarting Cortana
taskkill /f /im searchui.exe 