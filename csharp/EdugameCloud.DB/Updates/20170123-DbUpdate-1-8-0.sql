update LmsCompanySetting set name='IsOAuthSandbox' where name = 'IsD2LSandbox';
update LmsCompanySetting set name='OAuthAppId'     where name = 'D2LAppId'    ;
update LmsCompanySetting set name='OAuthAppKey'    where name = 'D2LAppKey'   ;
