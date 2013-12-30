<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:codegen="urn:cogegen-xslt-lib:xslt">
  <xsl:template match="/">
    <folder name="weborb-codegen">
      <info>info text</info>
      <folder name="src">
        <file path="messagingApplications/androidAir/src/MessagingCodegenAndroid.mxml" />
        <file path="messagingApplications/androidAir/src/MessagingCodegenAndroid-app.xml" />
        <folder name="ui">
          <xsl:for-each select="data/features/feature">
            <xsl:choose>
              <xsl:when test="id = 0">
                <folder path="messagingApplications/androidAir/src/ui/broadcasting" />
              </xsl:when>
              <xsl:when test="id = 1">
                <folder path="messagingApplications/androidAir/src/ui/recording" />
              </xsl:when>
              <xsl:when test="id = 2">
                <folder path="messagingApplications/androidAir/src/ui/sharedObj" />
              </xsl:when>
              <xsl:when test="id = 3">
                <folder path="messagingApplications/androidAir/src/ui/dataPush" />
              </xsl:when>
              <xsl:when test="id = 4">
                <folder path="messagingApplications/androidAir/src/ui/methodCall" />
              </xsl:when>
            </xsl:choose>
          </xsl:for-each>
          <folder path="messagingApplications/androidAir/src/ui/controls" />
          <folder path="messagingApplications/androidAir/src/ui/enums" />
          <folder path="messagingApplications/androidAir/src/ui/events" />
          <folder name="home">
            <file path="messagingApplications/androidAir/src/ui/home/HomeController.as" />
            <file name="HomeView.mxml" type="xml" addxmlversion="true">
              <![CDATA[
<s:View xmlns:fx="http://ns.adobe.com/mxml/2009" 
    xmlns:s="library://ns.adobe.com/flex/spark"
    xmlns:controls="ui.controls.*"
    title="RTMP application sample" initialize="onInitialize()">
                
    <fx:Script>]]>
        <xsl:text disable-output-escaping="yes">&lt;![CDATA[</xsl:text>
        <![CDATA[
        import spark.events.IndexChangeEvent;]]>
  <xsl:for-each select="data/features/feature">
    <xsl:choose>
      <xsl:when test="id = 0">
        import ui.broadcasting.VideoBroadcastView;
      </xsl:when>
      <xsl:when test="id = 1">
        import ui.recording.VideoRecordingView;
      </xsl:when>
      <xsl:when test="id = 2">
        import ui.sharedObj.RemoteSharedObjectView;
      </xsl:when>
      <xsl:when test="id = 3">
        import ui.dataPush.ServerDataPushView;
      </xsl:when>
      <xsl:when test="id = 4">
        import ui.methodCall.ServerMethodCallView;
      </xsl:when>
    </xsl:choose>
  </xsl:for-each>
        <![CDATA[import ui.managers.ConnectionManager;
                    
        private var controller:HomeController;
			
        private function onInitialize():void
        {
	        controller = new HomeController(this);
				
	        var manager:ConnectionManager = ConnectionManager.instance;
				
	        if (manager.connectionUri != manager.defaultConnectionUri)
		        txtConnectionUrl.text = manager.connectionUri;
				
	        if (manager.uriIsValid == true)
		        imgTestUri.source = dpiBtmpSrcGood;
	        else if (manager.uriIsValid == false)
		        imgTestUri.source = dpiBtmpSrcBad;
	        else
		        imgTestUri.source = dpiBtmpSrcTest;
        }
                    
        protected function onChange(event:IndexChangeEvent):void
        {]]>
      <xsl:choose>
        <xsl:when test="count(data/features/feature) = 2">
          <xsl:for-each select="data/features/feature">
            <xsl:choose>
              <xsl:when test="id = 0">
                navigator.pushView(VideoBroadcastView);
              </xsl:when>
              <xsl:when test="id = 1">
                navigator.pushView(VideoRecordingView);
              </xsl:when>
              <xsl:when test="id = 2">
                navigator.pushView(RemoteSharedObjectView);
              </xsl:when>
              <xsl:when test="id = 3">
                navigator.pushView(ServerDataPushView);
              </xsl:when>
              <xsl:when test="id = 4">
                navigator.pushView(ServerMethodCallView);
              </xsl:when>
            </xsl:choose>
          </xsl:for-each>
        </xsl:when>
        <xsl:otherwise><![CDATA[switch(event.newIndex)
    {]]>
        <xsl:for-each select="data/features/feature">
          <xsl:if test="id != 5"><![CDATA[case ]]><xsl:value-of select="position() - 1"/><![CDATA[:]]> 
          </xsl:if>
          <xsl:choose>
            <xsl:when test="id = 0">
              navigator.pushView(VideoBroadcastView);
            </xsl:when>
            <xsl:when test="id = 1">
              navigator.pushView(VideoRecordingView);
            </xsl:when>
            <xsl:when test="id = 2">
              navigator.pushView(RemoteSharedObjectView);
            </xsl:when>
            <xsl:when test="id = 3">
              navigator.pushView(ServerDataPushView);
            </xsl:when>
            <xsl:when test="id = 4">
              navigator.pushView(ServerMethodCallView);
            </xsl:when>
          </xsl:choose>
          <xsl:if test="id != 5"><![CDATA[break;]]>
          </xsl:if>
        </xsl:for-each><![CDATA[}]]>
          </xsl:otherwise>
        </xsl:choose><![CDATA[}]]>
      <xsl:text disable-output-escaping="yes">]]&gt;</xsl:text>
    <![CDATA[
    </fx:Script>
                
    <fx:Declarations>
	    <s:MultiDPIBitmapSource id="dpiBtmpSrcGood" 
		    source160dpi="@Embed('assets/TestGood_160.png')" 
		    source240dpi="@Embed('assets/TestGood_240.png')" 
		    source320dpi="@Embed('assets/TestGood_320.png')"/>
	    <s:MultiDPIBitmapSource id="dpiBtmpSrcTest" 
		    source160dpi="@Embed('assets/Test_160.png')" 
		    source240dpi="@Embed('assets/Test_240.png')" 
		    source320dpi="@Embed('assets/Test_320.png')"/>
	    <s:MultiDPIBitmapSource id="dpiBtmpSrcBad" 
		    source160dpi="@Embed('assets/TestBad_160.png')" 
		    source240dpi="@Embed('assets/TestBad_240.png')" 
		    source320dpi="@Embed('assets/TestBad_320.png')"/>
    </fx:Declarations>
	
	  <s:layout>
		  <s:VerticalLayout gap="4" paddingBottom="4" paddingTop="4" paddingLeft="4" paddingRight="4" />
	  </s:layout>
	
	  <s:navigationContent>
		  <s:Button id="btnExit" click="{NativeApplication.nativeApplication.exit()}">
			  <s:icon>
				  <s:MultiDPIBitmapSource 
					  source160dpi="@Embed('assets/Exit_160.png')" 
					  source240dpi="@Embed('assets/Exit_240.png')" 
					  source320dpi="@Embed('assets/Exit_320.png')"/>
			  </s:icon>
		  </s:Button>
	  </s:navigationContent>
	
	  <s:Group width="100%" height="100%">
		  <s:BusyIndicator id="busyIndicator" visible="false"
			  verticalCenter="0" horizontalCenter="0" />
		  <s:VGroup id="grContent" left="0" right="0" top="0" bottom="0">
			  <s:HGroup gap="0" width="100%" verticalAlign="middle" paddingLeft="4">
				  <s:TextInput id="txtConnectionUrl" prompt="Input connection URL" width="100%" 
					  change="{controller.onUriChange()}" />
				  <s:Image id="imgTestUri" click="{controller.onTestUri()}" />
			  </s:HGroup>
			  <s:List width="100%" change="onChange(event)">
				  <s:dataProvider>
					  <s:ArrayCollection source="[]]><xsl:text disable-output-escaping="yes"></xsl:text>
                <xsl:for-each select="data/features/feature">
                  <xsl:choose>
                    <xsl:when test="id = 0">
                      <xsl:text disable-output-escaping="yes">Video Broadcast</xsl:text>
                    </xsl:when>
                    <xsl:when test="id = 1">
                      <xsl:text disable-output-escaping="yes">Video Recording</xsl:text>
                    </xsl:when>
                    <xsl:when test="id = 2">
                      <xsl:text disable-output-escaping="yes">Remote Shared Object</xsl:text>
                    </xsl:when>
                    <xsl:when test="id = 3">
                      <xsl:text disable-output-escaping="yes">Data Push From Server</xsl:text>
                    </xsl:when>
                    <xsl:when test="id = 4">
                      <xsl:text disable-output-escaping="yes">Server Method Invocation</xsl:text>
                    </xsl:when>
                  </xsl:choose>
                  <xsl:if test="position() &lt; last() - 1">
                    <xsl:text disable-output-escaping="yes">,</xsl:text>
                  </xsl:if>
                </xsl:for-each><![CDATA[]" />
				  </s:dataProvider>
			  </s:List>
		  </s:VGroup>
	  </s:Group>
	  <controls:HRule width="100%" />
	  <s:Label id="lblCurrentUri" width="100%" color="#838383" 
		  text="{ConnectionManager.instance.connectionUri}" />
</s:View>]]>
            </file>
          </folder>
          <folder name="managers">
            <file name="ConnectionManager.as">
              <![CDATA[
package ui.managers
{
	import flash.events.AsyncErrorEvent;
	import flash.events.EventDispatcher;
	import flash.events.IOErrorEvent;
	import flash.events.NetStatusEvent;
	import flash.events.SecurityErrorEvent;
	import flash.events.TimerEvent;
	import flash.net.NetConnection;
	import flash.utils.Timer;
	
	import mx.messaging.config.ServerConfig;
	
	import ui.enums.ConnectionTestResultType;
	import ui.events.ConnectionTestEvent;
	
	[Event(name="testResult", type="ui.events.ConnectionTestEvent")]
	
	/**
	  * Helper class.<br/>
	  * Stores URI string that is either default URI string
	  * or user defined one. Tests given URI and
	  * dispatches appropriate event.<br/>
	  * ConnectionManager is singleton class.
	  */
	public class ConnectionManager extends EventDispatcher
	{
		/**
		  * Timeout to test URI in ms
		  */
		public static var TIMEOUT_MS:Number = 5000;
		
		/**
		  * @private
		  */
		private static var _instance:ConnectionManager;
		
		/**
		  * Instance of ConnectionManager
		  */
		public static function get instance():ConnectionManager
		{
			if (!_instance)
				_instance = new ConnectionManager(new PrivateConnectionManager());
			return _instance;
		}
		
		/**
		  * Constructor
		  */
		public function ConnectionManager(prvt:PrivateConnectionManager)
		{
			if (prvt == null)
				throw new Error("ConnectionManager is singleton. Use ConnectionManager.instance property instead");
			
			_tmrTimeout = new Timer(TIMEOUT_MS, 1);
		}
		
		/**
		  * @private
		  * Timer to define timeout by testing URI
		  */
		private var _tmrTimeout:Timer;
		
		/**
		  * Default connection URI.
		  */
		public function get defaultConnectionUri():String
		{]]>
    <xsl:for-each select="data/features/feature">
      <xsl:choose>
        <xsl:when test="id = 5 and included != 'true'">
          return ServerConfig.getChannel("air-nohttp").endpoint + "/<xsl:value-of select="/data/applicationName"/>";
        </xsl:when>
        <xsl:when test="id = 5 and included = 'true'"> <!-- tunneling -->
          return "rtmpt://localhost" + "/<xsl:value-of select="/data/applicationName"/>";
      </xsl:when>
      </xsl:choose>
    </xsl:for-each>
    <![CDATA[}
   /**
		 * @private
		 */
		private var _connectionUri:String = defaultConnectionUri;
		
		[Bindable]
		/**
		 * Connection URI (either user defined or default)
		 */
		public function get connectionUri():String
		{
			return _connectionUri;
		}
		
		/**
		 * @private
		 */
		public function set connectionUri(value:String):void
		{
			if (_connectionUri == value)
				return;
			
			_uriIsValid = null;
			
			// if user clears uri text box
			// then default uri is used
			if (value == null || value == "")
				_connectionUri = defaultConnectionUri;
			else
				_connectionUri = value;
		}
		
		/**
		 * @private
		 */
		private var _uriIsValid:Object = null;
		
		/**
		 * True if URI is valid connection string,
		 * false if URI is not valid and null
		 * if we got timeout and could not connect
		 * with this URI.
		 */
		public function get uriIsValid():Object
		{
			return _uriIsValid;
		}
		
		/**
		 * @private
		 * A two-way connection between a client and a server
		 */
		private var _netConnection:NetConnection;
		
		/**
		 * Tests URI string
		 */
		public function testConnectionUri(uri:String = null):void
		{
			if (uri == null || uri == "")
				return;
			
			connect(uri);
		}
		
		/**
		 * @private
		 * Establish connection between the application and the server
		 * with the given URI
		 */
		private function connect(uri:String):void
		{
			disconnect();
			
			_netConnection = new NetConnection();
			_netConnection.addEventListener(NetStatusEvent.NET_STATUS, onNetStatus);
			_netConnection.addEventListener(SecurityErrorEvent.SECURITY_ERROR, onSecurityError);
			
			// connect
			_netConnection.connect(uri);
			
			// start timer to check timeout
			startTimeoutTimer();
		}
		
		/**
		 * @private
		 * Closes connection
		 */
		private function disconnect():void
		{
			if (!_netConnection)
				return;
			
			// remove listeners
			_netConnection.removeEventListener(NetStatusEvent.NET_STATUS, onNetStatus);
			_netConnection.removeEventListener(SecurityErrorEvent.SECURITY_ERROR, onSecurityError);
			
			if (!_netConnection.connected)
				return;
			
			// we cannot close the NetConnection immediately after connection
			// therefore we wait a little before closing;
			// pay attention of using 'useWeakReference = true' in addEventListener
			var closeTimer:Timer = new Timer(200, 1);
			closeTimer.addEventListener(
				TimerEvent.TIMER, 
				function (event:TimerEvent):void {
					_netConnection.close();
				}, 
				false, 0, true);
			closeTimer.start();
		}
		
		/**
		 * @private
		 * Starts timer
		 */
		private function startTimeoutTimer():void
		{
			_tmrTimeout.addEventListener(TimerEvent.TIMER, onTimer);
			_tmrTimeout.start();
		}
		
		/**
		 * @private
		 * Stops timer
		 */
		private function stopTimeoutTimer():void
		{
			_tmrTimeout.stop();
			_tmrTimeout.removeEventListener(TimerEvent.TIMER, onTimer);
		}
		
		/**
		 * @private
		 * Handles timer tick
		 */
		private function onTimer(event:TimerEvent):void
		{
			_uriIsValid = null;
			
			stopTimeoutTimer();
			disconnect();
			
			dispatchEvent(new ConnectionTestEvent(
				ConnectionTestEvent.TEST_RESULT, ConnectionTestResultType.TIMEOUT, "Timeout. Cannot test URI"));
		}
		
		/**
		 * @private
		 * Handles connection results
		 */
		private function onNetStatus(event:NetStatusEvent):void
		{
			stopTimeoutTimer();
			disconnect();
			
			if (event.info.code == "NetConnection.Connect.Success")
				dispatchGoodTest();
			else
				dispatchBadTest(event.info.code);
		}
		
		/**
		 * @private
		 * Handles connection results
		 */
		private function onSecurityError(event:SecurityErrorEvent):void
		{
			stopTimeoutTimer();
			disconnect();
			dispatchBadTest("Security error.");
		}
		
		/**
		 * @private
		 */
		private function dispatchGoodTest():void
		{
			_uriIsValid = true;
			
			dispatchEvent(new ConnectionTestEvent(
				ConnectionTestEvent.TEST_RESULT, ConnectionTestResultType.GOOD));
		}
		
		/**
		 * @private
		 */
		private function dispatchBadTest(message:String):void
		{
			_uriIsValid = false;
			
			dispatchEvent(new ConnectionTestEvent(
				ConnectionTestEvent.TEST_RESULT, ConnectionTestResultType.BAD, message));
		}
	}
}

/**
 * Helper class to prevent using constuctor of ConnectionManager
 */
class PrivateConnectionManager
{
}
              ]]>
            </file>
          </folder>
          <file path="messagingApplications/androidAir/src/ui/Defaults.as" />
        </folder>
        <folder path="messagingApplications/androidAir/src/assets" />
      </folder>
      <folder name="libs">
        <file path="../wdm/weborb.swc" />
      </folder>
      <file name=".actionScriptProperties"><![CDATA[<?xml version="1.0" encoding="UTF-8" standalone="no"?>
  <actionScriptProperties analytics="false" mainApplicationPath="MessagingCodegenAndroid.mxml" projectUUID="54277c98-2a1b-4e6f-b737-dcd9565c11a9" version="10">
    <compiler additionalCompilerArguments="-locale en_US -services &quot;]]><xsl:value-of select="data/weborbPath"/><![CDATA[WEB-INF\flex\weborb-services-config.xml&quot;" autoRSLOrdering="true" copyDependentFiles="true" fteInMXComponents="false" generateAccessible="false" htmlExpressInstall="true" htmlGenerate="false" htmlHistoryManagement="false" htmlPlayerVersionCheck="true" includeNetmonSwc="false" outputFolderPath="bin-debug" removeUnusedRSL="true" sourceFolderPath="src" strict="true" targetPlayerVersion="0.0.0" useApolloConfig="true" useDebugRSLSwfs="true" verifyDigests="true" warn="true">
    <compilerSourcePath/>
    <libraryPath defaultLinkType="0">
      <libraryPathEntry kind="4" path="">
        <excludedEntries>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/qtp.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/advancedgrids.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/automation_air.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/air/airspark.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/netmon.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/mx/mx.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/air/applicationupdater.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/utilities.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/flex.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/sparkskins.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/qtp_air.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/datavisualization.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/core.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/spark_dmv.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/automation.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/automation_dmv.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/air/airframework.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/air/applicationupdater_ui.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/automation_flashflexkit.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/automation_agent.swc" useDefaultLinkType="false"/>
        </excludedEntries>
      </libraryPathEntry>
      <libraryPathEntry kind="1" linkType="1" path="libs"/>
    </libraryPath>
    <sourceAttachmentPath/>
  </compiler>
  <applications>
    <application path="MessagingCodegenAndroid.mxml">
      <airExcludes/>
    </application>
  </applications>
  <modules/>
  <buildCSSFiles/>
  <flashCatalyst validateFlashCatalystCompatibility="false"/>
  <buildTargets>
    <buildTarget androidSettingsVersion="1" buildTargetName="com.adobe.flexide.multiplatform.android.platform">
      <airSettings airCertificatePath="" airTimestamp="true" version="1">
        <airExcludes/>
      </airSettings>
      <multiPlatformSettings enabled="true" includePlatformLibs="false" platformID="com.adobe.flexide.multiplatform.android.platform" version="2"/>
      <actionScriptSettings version="1"/>
    </buildTarget>
    <buildTarget buildTargetName="default">
      <airSettings airCertificatePath="" airTimestamp="true" version="1">
        <airExcludes/>
      </airSettings>
      <multiPlatformSettings enabled="false" includePlatformLibs="false" platformID="default" version="2"/>
      <actionScriptSettings version="1"/>
    </buildTarget>
  </buildTargets>
</actionScriptProperties>]]>
      </file>
      <file path="messagingApplications/androidAir/.flexProperties" />
      <file name=".project" type="xml" addxmlversion="true"><![CDATA[<projectDescription>
	<name>]]><xsl:value-of select="data/applicationName"/>DemoApp<![CDATA[</name>
	<comment></comment>
	<projects>
	</projects>
	<buildSpec>
		<buildCommand>
			<name>com.adobe.flexbuilder.project.flexbuilder</name>
			<arguments>
			</arguments>
		</buildCommand>
		<buildCommand>
			<name>com.adobe.flexbuilder.project.apollobuilder</name>
			<arguments>
			</arguments>
		</buildCommand>
	</buildSpec>
	<natures>
		<nature>com.adobe.flexide.project.multiplatform.multiplatformnature</nature>
		<nature>com.adobe.flexbuilder.project.apollonature</nature>
		<nature>com.adobe.flexbuilder.project.flexnature</nature>
		<nature>com.adobe.flexbuilder.project.actionscriptnature</nature>
	</natures>
</projectDescription>]]>
    </file>
    </folder>
  </xsl:template>
</xsl:stylesheet>