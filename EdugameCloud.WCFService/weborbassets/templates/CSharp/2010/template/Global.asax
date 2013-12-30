<%@ Application Language="C#" %>
<%@ Import Namespace="System.Security.Principal" %>
 
<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        try
        {
           // =======================================================================
           // =======               RTMP SERVER STARTUP                   ===========
           // =======================================================================
           
           // Initialize WebORB configuration before starting messaging server
           Weborb.Config.ORBConfig config = Weborb.Config.ORBConfig.GetInstance();
           int port = 2037;
           // Create Messaging server. 2037 is the port number, 500 is connection backlog
           Weborb.Messaging.RTMPServer server = new Weborb.Messaging.RTMPServer( "default", port, 500, config );

           // Start the messaging server
           server.start();
           // Start WebORB policy socket server. This is needed for cross domain Silverlight requests
           Weborb.Messaging.PolicySocketServer.StartSocketServer();
        }
        catch( Exception )
        {
        }      
    }
    
    void Application_BeginRequest( object sender, EventArgs e )
    {
       if( Application["weborburl"] == null )
          if( Request.Url.AbsoluteUri.EndsWith( "/weborb.aspx" ) )
             Application[ "weborburl" ] = Request.Url.AbsoluteUri;
    }
    
    
    void Application_End(object sender, EventArgs e) 
    {
        Weborb.Util.ORBUtil.InvokeWeborbDisposed();
        // Retrieve the messaging server instances
        foreach ( Weborb.Messaging.BaseRTMPServer server in Weborb.Messaging.BaseRTMPServer.GetServers().Values )
        {

          // If the server is not null, stop it.
          if ( server != null )
            server.shutdown();
        }
      
        if( Application[ "weborburl" ] != null )
        try
        {
            System.Net.WebClient http = new System.Net.WebClient();
            http.DownloadStringAsync( new Uri( (String)Application[ "weborburl" ] + "?diag") );
        }
        catch (Exception ex)
        {
            Weborb.Util.Logging.Log.log( Weborb.Util.Logging.LoggingConstants.INFO, ex );
        }            
            
    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
    
    protected void Application_AuthenticateRequest(Object sender, EventArgs e)
    {
        //When application uses Forms Authentication, it creates FormsIdentity along 
        // with the FormsAuthenticationTicket. The API call below establishes WebORB 
        // principal for the current Forms Authentication request. As a result, 
        // WebORB security can leverage the same user identity created through
        // Forms Authentication
        Weborb.Security.ORBSecurity.AuthenticateRequest();
    }    
       
</script>
