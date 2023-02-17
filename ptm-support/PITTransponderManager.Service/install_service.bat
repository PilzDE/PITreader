@ECHO OFF

SET SCRIPTDIR=%~dp0
SET SCNAME=Pilz.PTM

sc query | find "SERVICE_NAME: %SCNAME%"
IF %ERRORLEVEL% EQU 0 (
	sc stop "%SCNAME%"
	sc delete "%SCNAME%"
)

sc create "%SCNAME%" binpath="%SCRIPTDIR%PtmServiceWrapper.exe" DisplayName="Pilz PIT Transponder Manager" start=demand

sc description "%SCNAME%" "Starts PIT Transponder Manager in the background to be accessible using 127.0.0.1:8090 in a local browser."

sc sdset "%SCNAME%" D:(A;;CCLCSWRPWPDTLOCRRC;;;SY)(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA)(A;;CCLCSWLOCRRCRPWP;;;IU)(A;;CCLCSWLOCRRC;;;SU)S:(AU;FA;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;WD)