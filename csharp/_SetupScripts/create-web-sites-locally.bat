set environmentAlias=Stage
set userName="ESYNCTRAINING\Bamboo_LTI_stage"

powershell -ExecutionPolicy Unrestricted -NoLogo -File create-web-sites.ps1 -environmentAlias %environmentAlias%

powershell -ExecutionPolicy Unrestricted -NoLogo -File create-web-sites-locally.ps1 -environmentAlias %environmentAlias%

if %userName% NEQ "" net share EdugameCloud.%environmentAlias%$=c:\inetpub\EdugameCloud.%environmentAlias% /grant:%userName%,FULL


net stop was /y
ping 127.0.0.1 -n 20 > nul
net start w3svc