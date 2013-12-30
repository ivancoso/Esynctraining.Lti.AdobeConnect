<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:codegen="urn:cogegen-xslt-lib:xslt">

  <xsl:template name="replace">
    <xsl:param name="inputstr"/>
    <xsl:param name="patternstr"/>
    <xsl:param name="replacestr"/>
    <xsl:choose>
      <xsl:when test="contains($inputstr, $patternstr)">
        <xsl:value-of select="substring-before($inputstr, $patternstr)"/>
        <xsl:value-of select="$replacestr"/>
        <xsl:call-template name="replace">
          <xsl:with-param name="inputstr" select="substring-after($inputstr, $patternstr)"/>
          <xsl:with-param name="patternstr" select="$patternstr"/>
          <xsl:with-param name="replacestr" select="$replacestr"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$inputstr"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:variable name="weborbPathSlash">
    <xsl:call-template name="replace">
      <xsl:with-param name="inputstr" select="data/weborbPath"/>
      <xsl:with-param name="patternstr" select="'\'"/>
      <xsl:with-param name="replacestr" select="'/'"/>
    </xsl:call-template>
  </xsl:variable>

  <xsl:template name="codegen.project.eclipse.flex.actionscript.properties">
    <file name=".actionScriptProperties"><![CDATA[<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<actionScriptProperties analytics="false" mainApplicationPath="AirMessagingAppSample.mxml" projectUUID="25071eff-b4d2-442f-b345-e77c75239bf7" version="10">
  <compiler additionalCompilerArguments="-locale en_US -services &quot;]]><xsl:value-of select="data/weborbPath"/><![CDATA[WEB-INF\flex\weborb-services-config.xml&quot;" autoRSLOrdering="false" copyDependentFiles="true" flexSDK="Flex 3.6" fteInMXComponents="false" generateAccessible="false" htmlExpressInstall="true" htmlGenerate="false" htmlHistoryManagement="false" htmlPlayerVersionCheck="true" includeNetmonSwc="false" outputFolderLocation="]]><xsl:value-of select="data/weborbPath"/><![CDATA[AirMessagingAppSample" outputFolderPath="bin-debug" removeUnusedRSL="true" sourceFolderPath="src" strict="true" targetPlayerVersion="0.0.0" useApolloConfig="true" useDebugRSLSwfs="true" verifyDigests="true" warn="true">
    <compilerSourcePath/>
    <libraryPath defaultLinkType="0">
      <libraryPathEntry kind="4" path="">
        <excludedEntries>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/flex.swc" useDefaultLinkType="false"/>
          <libraryPathEntry kind="3" linkType="1" path="${PROJECT_FRAMEWORKS}/libs/core.swc" useDefaultLinkType="false"/>
        </excludedEntries>
      </libraryPathEntry>
      <libraryPathEntry kind="1" linkType="1" path="libs"/>
    </libraryPath>
    <sourceAttachmentPath/>
  </compiler>
  <applications>
    <application path="AirMessagingAppSample.mxml">
      <airExcludes/>
    </application>
  </applications>
  <modules/>
  <buildCSSFiles/>
  <flashCatalyst validateFlashCatalystCompatibility="false"/>
  <buildTargets>
    <buildTarget buildTargetName="default">
      <airSettings airCertificatePath="" airTimestamp="true" version="1">
        <airExcludes/>
      </airSettings>
      <actionScriptSettings version="1"/>
    </buildTarget>
  </buildTargets>
</actionScriptProperties>
    ]]>
    </file>
  </xsl:template>

  <xsl:template name="codegen.project.eclipse.flex.properties">
    <file name=".flexProperties"><![CDATA[<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<flexProperties aspUseIIS="true" enableServiceManager="false" flexServerFeatures="0" flexServerType="32" serverContextRoot="" serverRoot="]]><xsl:value-of select="data/weborbPath"/><![CDATA[" serverRootURL="]]><xsl:value-of select="data/weborbRootURL"/><![CDATA[" toolCompile="true" useServerFlexSDK="false" version="2"/>
    ]]>
    </file>
  </xsl:template>

  <xsl:template name="codegen.project.eclipse.flex">
    <file name=".project"><![CDATA[<?xml version="1.0" encoding="UTF-8"?>
<projectDescription>
	<name>AirMessagingAppSample</name>
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
		<nature>com.adobe.flexbuilder.project.apollonature</nature>
		<nature>com.adobe.flexbuilder.project.flexnature</nature>
		<nature>com.adobe.flexbuilder.project.actionscriptnature</nature>
	</natures>
	<linkedResources>
		<link>
			<name>bin-debug</name>
			<type>2</type>]]><!-- Flash Builder 4.5.1 recognizes location path only with '/' (we've replaces backslash) --><![CDATA[
			<location>]]><xsl:value-of select="$weborbPathSlash"/><![CDATA[AirMessagingAppSample</location>
		</link>
	</linkedResources>
</projectDescription>
    ]]>
    </file>
    
    <folder name="libs">
      <file path="../wdm/weborb.swc" hideContent="true"/>
    </folder>
  </xsl:template>

  <xsl:template match="/">
    <folder name="weborb-codegen">
      <info>info text</info>
      <xsl:if test="data/fullCode = 'true'">
        <folder path="messagingDestinations/airDesktop/.settings"/>
        <xsl:call-template name="codegen.project.eclipse.flex" />
        <xsl:call-template name="codegen.project.eclipse.flex.properties" />
        <xsl:call-template name="codegen.project.eclipse.flex.actionscript.properties" />
      </xsl:if>
      <folder name="src">
        <file name="AirMessagingAppSample.as"><![CDATA[import mx.messaging.Consumer;
import mx.messaging.Channel;
import mx.messaging.ChannelSet;
import mx.messaging.Consumer;
import mx.messaging.Producer;
import mx.messaging.channels.AMFChannel;
import mx.messaging.channels.SecureAMFChannel;
import mx.messaging.config.ServerConfig;
import mx.messaging.events.MessageEvent;
import mx.messaging.events.MessageFaultEvent;
import mx.messaging.messages.AsyncMessage;

import weborb.messaging.WeborbMessagingChannel;

private var consumer:Consumer = new Consumer();
private var producer:Producer = new Producer();
private var destination:String = "]]><xsl:value-of select="data/destinationId"/><![CDATA[";
private var serverRootUrl:String = "]]><xsl:value-of select="data/weborbRootURL"/><![CDATA[";

private function init():void
{
	var channelSet:ChannelSet = ServerConfig.getChannelSet(destination);
	var channelIdCount:int = channelSet.channelIds.length;
	var endpoint:String;
	var protocol:String;
	
	var newChannelSet:ChannelSet = new ChannelSet();
	
	for (var i:int = 0; i < channelIdCount; i++)
	{
		var newChannel:Channel;
		
		protocol = ServerConfig.getChannel(channelSet.channelIds[i]).protocol;
		
		switch (protocol)
		{
			case "http":
				endpoint = serverRootUrl + "/weborb.aspx";
				newChannel = new AMFChannel("amf-airdesktop", endpoint);
				break;
			case "rtmp":
				endpoint = serverRootUrl.replace("http", "rtmp");
				var index:int = endpoint.lastIndexOf("/");
				if (index != -1)
					endpoint = endpoint.substring(0, index);
				endpoint += ":2037";
				newChannel = new WeborbMessagingChannel("rtmp-airdesktop", endpoint);
				break;
			case "https":
				endpoint = serverRootUrl.replace("http", "https") + "/weborb.aspx";
				newChannel = new SecureAMFChannel("secure-amf-airdesktop", endpoint);
				break;
			default:
				throw new ArgumentError("Unsupported protocol: " + protocol);
		}
		
		newChannelSet.addChannel(newChannel);
	}
	
	consumer.destination = destination;
	consumer.channelSet = newChannelSet;
	consumer.addEventListener(MessageEvent.MESSAGE, messageReceived);
	consumer.addEventListener(MessageFaultEvent.FAULT, onFault);
	consumer.subscribe();
	
	producer.destination = destination;
	producer.channelSet = newChannelSet;
}

private function messageReceived(event:MessageEvent):void
{
	var message:AsyncMessage = AsyncMessage(event.message);
	
	var sender:String = message.headers[ "WebORBClientId" ];

    if( sender == "" )
    	sender = "Anonymous";
	
	txtArMessagesLog.text += sender +" : "+ message.body + "\n";
}

private function onFault(event:MessageFaultEvent):void
{
	trace(event.faultString);
}

private function onClick():void
{
	var message:AsyncMessage = new AsyncMessage();
	message.headers = {"WebORBClientId": txtClientId.text};
	message.body = txtMessage.text;
	producer.send(message);
}]]>
        </file>
        <file path="messagingDestinations/airDesktop/AirMessagingAppSample.mxml"/>
        <file path="messagingDestinations/airDesktop/AirMessagingAppSample-app.xml"/>
      </folder>
    </folder>
  </xsl:template>
  
</xsl:stylesheet>