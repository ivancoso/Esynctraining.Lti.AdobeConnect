powershell -ExecutionPolicy Unrestricted -NoLogo -File create-web-sites.ps1

net share EdugameCloud.Stage$=c:\inetpub\EdugameCloud.Stage /grant:ESYNCTRAINING\Bamboo_LTI_stage,FULL

net stop was /y
ping 127.0.0.1 -n 20 > nul
net start w3svc