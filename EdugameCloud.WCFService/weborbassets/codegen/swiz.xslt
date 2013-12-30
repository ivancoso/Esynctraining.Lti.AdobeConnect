<?xml version="1.0"?>
<xsl:stylesheet version="2.0"
  xmlns:codegen="urn:cogegen-xslt-lib:xslt"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:import href="codegen.xslt"/>

<xsl:variable name="up" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>
<xsl:variable name="lo" select="'abcdefghijklmnopqrstuvwxyz'"/>

<xsl:template name="codegen.appmain">
<file name="main.mxml"><![CDATA[<?xml version="1.0" encoding="utf-8"?>]]>
&lt;mx:Application xmlns:mx="http://www.adobe.com/2006/mxml" xmlns:swiz="http://swiz.swizframework.org" xmlns:config="<xsl:value-of select="//service/@namespace"/>.config.*" layout="absolute" initialize="onLoad()">

  &lt;mx:Script>
    &lt;![CDATA[
<xsl:if test="count(//datatype) != 0">
      import <xsl:value-of select="//service/@namespace"/>.DataTypeInitializer;
</xsl:if>
  <xsl:for-each select="//service">
      import <xsl:value-of select="@namespace" />.event.<xsl:value-of select="@name"/>Event;</xsl:for-each>

      // These variables are included to enable runtime without error
      // Remove the code below when you dispatch real events
      // Pay attention: if you mediate event (see controller class [Mediate] metatag) 
      // you have to dispatch event or make dummy variable to include event type into compilation
      // otherwise you'll get runtime error
      <xsl:for-each select="//service">
      private var dummy<xsl:value-of select="@name"/>Event:<xsl:value-of select="@name"/>Event;</xsl:for-each>

      public function onLoad():void
      {<xsl:if test="count(//datatype) != 0">
        new DataTypeInitializer();
      </xsl:if>}
    ]]&gt;
  &lt;/mx:Script>

  &lt;swiz:Swiz>
    &lt;swiz:beanProviders>
      &lt;config:Beans />
    &lt;/swiz:beanProviders>

    &lt;swiz:config>
  &lt;swiz:SwizConfig
	     eventPackages="<xsl:value-of select="//service/@namespace"/>.event.*" />
    &lt;/swiz:config>
  &lt;/swiz:Swiz>
&lt;/mx:Application>
</file>
</xsl:template>

<xsl:template name="codegen.project.custom">
    <folder name="libs">
      <file name="readme.txt">
	  Project structure generated by WebORB includes all the source code and project files except the
	  library file for the Swiz framework. You need to download and place the swiz-framework-1.0.0-RC1.swc
	  file (or newer) in this folder.
        </file>
    </folder>
</xsl:template>

<xsl:template name="codegen.service">
<xsl:call-template name="codegen.datatypeinitializer"/>
<folder name="controller">
		<xsl:variable name="firstLetter" select="substring(@name, 1,1)"/>
		<xsl:variable name="className" select = "concat(translate($firstLetter, $lo, $up), substring(@name, 2, 30))"/>

		<file name="{$className}Controller.as">
			<xsl:call-template name="codegen.description">
				<xsl:with-param name="file-name" select="concat($className,'Controller.as')" />
			</xsl:call-template>
			<xsl:call-template name="controllers" />
		</file>
</folder>

<folder name="config">
	<file name="Beans.mxml" type="xml">
	  <xsl:call-template name="beans" />
	</file>
</folder>

<folder name="event">
    <xsl:for-each select="../service">
    <file name="{@name}Event.as">
    <xsl:call-template name="codegen.description">
        <xsl:with-param name="file-name" select="concat(@name,'Event.as')" />
    </xsl:call-template>
	
package <xsl:value-of select="@namespace" />.event
{
    import flash.events.Event;
	<xsl:call-template name="codegen.import.alltypes"/>	

    public class <xsl:value-of select="@name" />Event extends Event
    {<xsl:for-each select="method">
		<xsl:variable name="evtName" select = "translate(@name, $lo, $up)"/>
		<xsl:for-each select='arg'><xsl:variable name="argName" select="@name"/>
		public var <xsl:choose><xsl:when test="count(../../method/arg[@name=$argName]) = 1"><xsl:value-of select="$argName" /></xsl:when><xsl:otherwise><xsl:value-of select="../@name"/>_<xsl:value-of select="$argName" /></xsl:otherwise></xsl:choose>:<xsl:value-of select="@type" />;</xsl:for-each>

		public static const <xsl:value-of select="$evtName" />_REQUESTED : String = "<xsl:value-of select="@name" />";</xsl:for-each>

		public function <xsl:value-of select="@name"/>Event( type:String )
        {
			super( type, true );
        }
    }
}
</file>
</xsl:for-each>
</folder>

<xsl:if test="count(../enum) != 0">
  <folder name="enum">
	<xsl:for-each select="../enum">
	  <xsl:call-template name="codegen.enum"/>							  
	</xsl:for-each>        
  </folder>
</xsl:if>

<folder name="service">
<file name="{@name}Service.as">
<xsl:call-template name="codegen.description">
	<xsl:with-param name="file-name" select="concat(@name,'Event.as')" />
</xsl:call-template>
package <xsl:value-of select="@namespace" />.service
{
	import flash.events.IEventDispatcher;
	import mx.rpc.AsyncToken;
	import mx.rpc.IResponder;
	import mx.rpc.remoting.RemoteObject;
	import mx.rpc.events.ResultEvent;
	import mx.rpc.events.FaultEvent;
	import mx.collections.ArrayCollection;
	<xsl:call-template name="codegen.import.alltypes"/>

	public class <xsl:value-of select="@name"/>Service
	{
		/**
		 * The [Dispatcher] metadata tag instructs Swiz to inject an event dispatcher.
		 * Event's dispatched via this dispatcher can trigger event mediators.
		 */
		[Dispatcher]
		public var dispatcher : IEventDispatcher;
		private var remoteObject:RemoteObject;

	    public function <xsl:value-of select="@name"/>Service()
		{
			remoteObject  = new RemoteObject("GenericDestination");
			remoteObject.source = "<xsl:value-of select='@fullname'/>";
		}
		<xsl:for-each select="method">
		public function <xsl:value-of select="@name"/>(<xsl:for-each select="arg"><xsl:value-of select="@name"/>:<xsl:value-of select="@type" />, </xsl:for-each>responder:IResponder = null ):AsyncToken
		{
			var asyncToken:AsyncToken = remoteObject.<xsl:value-of select="@name"/>(<xsl:for-each select="arg">
			<xsl:if test="position() != 1">,</xsl:if>
			<xsl:value-of select="@name"/>
			</xsl:for-each>);

			if( responder != null )
				asyncToken.addResponder( responder );

			return asyncToken;
		}
		</xsl:for-each>
	}
}
</file>
</folder>
</xsl:template>

<xsl:template name="beans">
	<xsl:variable name="firstLetter" select="substring(@name, 1,1)"/>
	<xsl:variable name="className" select = "concat(translate($firstLetter, $up, $lo), substring(@name, 2, 30))"/>
	
&lt;!-- <xsl:call-template name="codegen.description">
	<xsl:with-param name="file-name" select="concat(@name,'Beans.mxml')" />
</xsl:call-template> -->

<xsl:text disable-output-escaping="yes">&lt;</xsl:text>swiz:BeanProvider
           xmlns:swiz="http://swiz.swizframework.org"
           xmlns:mx="http://www.adobe.com/2006/mxml"
           xmlns:service="<xsl:value-of select="//service/@namespace" />.service.*"
           xmlns:controller="<xsl:value-of select="//service/@namespace" />.controller.*"<xsl:text disable-output-escaping="yes">&gt;</xsl:text>
<xsl:text disable-output-escaping="yes">&lt;</xsl:text>service:<xsl:value-of select="//service/@name" />Service id="<xsl:value-of select="$className"/>Service"/<xsl:text disable-output-escaping="yes">&gt;</xsl:text>
<xsl:text disable-output-escaping="yes">
&lt;</xsl:text>controller:<xsl:value-of select="//service/@name" />Controller id="<xsl:value-of select="$className"/>Controller"/<xsl:text disable-output-escaping="yes">&gt;</xsl:text>
<xsl:text disable-output-escaping="yes">&lt;</xsl:text>swiz:ServiceHelper id="serviceHelper" /<xsl:text disable-output-escaping="yes">&gt;</xsl:text>
<xsl:text disable-output-escaping="yes">
&lt;</xsl:text>/swiz:BeanProvider<xsl:text disable-output-escaping="yes">
&gt;</xsl:text>
</xsl:template>

<xsl:template name="controllers">
package <xsl:value-of select="//service/@namespace" />.controller
{
	import mx.rpc.events.ResultEvent;
	import mx.rpc.events.FaultEvent;
	import mx.utils.ObjectUtil;
	import mx.controls.Alert;
	import org.swizframework.utils.services.ServiceHelper;
	import <xsl:value-of select="//service/@namespace" />.service.<xsl:value-of select="@name"/>Service;<xsl:call-template name="codegen.import.alltypes"/>

	<xsl:variable name="firstLetter" select="substring(//service/@name, 1,1)"/>
	<xsl:variable name="className" select = "concat(translate($firstLetter, $up, $lo), substring(//service/@name, 2, 30))"/>

    public class <xsl:value-of select="//service/@name"/>Controller
    {
		[Inject]
        public var <xsl:value-of select="$className"/>Service:<xsl:value-of select="@name"/>Service;

		[Inject]
		public var serviceHelper:ServiceHelper;
		<xsl:for-each select="//method"><xsl:variable name="evtName" select = "translate(@name, $lo, $up)"/><xsl:variable name="firstLetterMethodName" select="substring(@name, 1,1)"/>
		<xsl:variable name="methodNamePascal" select = "concat(translate($firstLetterMethodName, $lo, $up), substring(@name, 2, 30))"/>
		[Mediate( event="<xsl:value-of select="//service/@name"/>Event.<xsl:value-of select="$evtName"/>_REQUESTED" <xsl:if test="count(arg) != 0">, properties= "<xsl:for-each select='arg'><xsl:value-of select='@name'/><xsl:if test='position()!=last()'>,</xsl:if></xsl:for-each>" </xsl:if>)]
        public function <xsl:value-of select='@name'/>( <xsl:for-each select='arg'><xsl:value-of select='@name'/>:<xsl:value-of select='@type'/><xsl:if test='position()!=last()'>,</xsl:if></xsl:for-each>):void
		{
			serviceHelper.executeServiceCall( <xsl:value-of select="$className"/>Service.<xsl:value-of select='@name'/>(<xsl:for-each select='arg'><xsl:value-of select='@name'/><xsl:if test='position()!=last()'>, </xsl:if></xsl:for-each>), handle<xsl:value-of select='$methodNamePascal'/>Result );
		}

		public function handle<xsl:value-of select='$methodNamePascal'/>Result( event:ResultEvent ):void
		{
			trace( "received result for <xsl:value-of select='@name'/> method invocation" );
		}
		</xsl:for-each>
    }
}
    </xsl:template>
</xsl:stylesheet>