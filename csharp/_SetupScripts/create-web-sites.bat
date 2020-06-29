set environmentAlias=Prod
set userName=""

powershell -ExecutionPolicy Unrestricted -NoLogo -File create-web-sites.ps1 -environmentAlias %environmentAlias%

net stop was /y
ping 127.0.0.1 -n 20 > nul
net start w3svc