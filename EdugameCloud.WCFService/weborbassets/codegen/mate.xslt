<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
xmlns:codegen="urn:cogegen-xslt-lib:xslt"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:import href="codegen.xslt"/>

<xsl:variable name="up" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>
  <xsl:variable name="lo" select="'abcdefghijklmnopqrstuvwxyz'"/>

    <xsl:template name="codegen.appmain">
    <file name="main.mxml">
&lt;mx:Application xmlns:mx="http://www.adobe.com/2006/mxml" layout="absolute" initialize="onLoad()" xmlns:maps = "<xsl:value-of select="//service/@namespace"/>.maps.*">
&lt;mx:Script>
&lt;![CDATA[
  <xsl:if test="count(//datatype) != 0">    
import <xsl:value-of select="//service/@namespace"/>.DataTypeInitializer;
  </xsl:if>    
public function onLoad():void
{
	<xsl:if test="count(//datatype) != 0">new DataTypeInitializer();  </xsl:if>
}
]]&gt;
&lt;/mx:Script>
&lt;maps:<xsl:value-of select="//service/@name"/>EventMap />
&lt;/mx:Application>
    </file>
  </xsl:template>

  <xsl:template name="codegen.project.custom">
    <folder name="libs">
  <file name="readme.txt">
  Project structure generated by WebORB includes all the source code and project files except the 
  library file for the Mate framework. You need to download and place the Mate_08_9.swc file 
  (or newer) in this folder. 
  </file>
	</folder>
  </xsl:template>

  <xsl:template name="codegen.service">
	  <xsl:if test="count(//datatype) != 0">
        <file name="DataTypeInitializer.as">
          <xsl:call-template name="codegen.datatypelist">
            <xsl:with-param name="namespaceName" select="@namespace" />
          </xsl:call-template>  
        </file>
      </xsl:if>   
 	  <folder name="business">
  <file name="{@name}Manager.as">
	<xsl:call-template name="codegen.description">
	  <xsl:with-param name="file-name" select="concat(@name,'Manager.as')" />
	</xsl:call-template>
	
<xsl:call-template name="manager" />
  </file>
      </folder>

<folder name="views">
  <xsl:variable name="firstLetter" select="substring(@name, 1,1)"/>
  <xsl:variable name="className" select = "concat(translate($firstLetter, $lo, $up), substring(@name, 2, 30))"/>

<file name="{$className}Panel.mxml" type="xml">
            <mx:Panel xmlns:mx="http://www.adobe.com/2006/mxml" layout="absolute" width="800" height="600">

</mx:Panel>
          </file>      
	  </folder>

<folder name="maps">	  
   <xsl:variable name="firstLetter" select="substring(@name, 1,1)"/>
   <xsl:variable name="className" select = "concat(translate($firstLetter, $lo, $up), substring(@name, 2, 30))"/>
		
<file name="{$className}EventMap.mxml" type="xml">&lt;!--
<xsl:call-template name="codegen.description">
		<xsl:with-param name="file-name" select="concat($className,'EventMap.mxml')" />
	  </xsl:call-template>-->
	  <xsl:call-template name="eventMap" />
	</file>
  </folder>
	  
<folder name="events">
	<xsl:for-each select='method'>
		<xsl:variable name="firstLetter" select="substring(@name, 1,1)"/>
		<xsl:variable name="className" select = "concat(translate($firstLetter, $lo, $up), substring(@name, 2, 30))"/>
	
	<file name="{$className}Event.as">
	  <xsl:call-template name="codegen.description">
		<xsl:with-param name="file-name" select="concat($className,'Event.as')" />
	  </xsl:call-template>
	  <xsl:call-template name="event" />
	</file>
	  
		<file name="{$className}FaultEvent.as">
		<xsl:call-template name="codegen.description">
		  <xsl:with-param name="file-name" select="concat($className,'FaultEvent.as')" />
		</xsl:call-template>
		<xsl:call-template name="faultEvent" />
	  </file>
	</xsl:for-each>
  </folder>
 
  </xsl:template>



	         <xsl:template name="manager">
package <xsl:value-of select="//service/@namespace" />.business
{   		 		 <xsl:call-template name="codegen.import.alltypes"/>

	public class <xsl:value-of select="//service/@name"/>Manager
	{		
							<xsl:for-each select="method">
		<xsl:if test="@type!='void'">		
		[Bindable]		
		public var retVal_<xsl:value-of select="@name"/>:<xsl:value-of select="@type"/>;
		</xsl:if>	
		public function <xsl:value-of select="@name"/>Store(<xsl:if test="@type!='void'">value:<xsl:value-of select="@type"/></xsl:if>):void
		{	<xsl:if test="@type!='void'">
			retVal_<xsl:value-of select="@name"/> = value;
		</xsl:if>}
	</xsl:for-each>			
	}
}
    </xsl:template>

	
 <xsl:template name="event"> 
package <xsl:value-of select="../@namespace" />.events
{
	import flash.events.Event;
<xsl:call-template name="codegen.import.alltypes"/>

<xsl:variable name="firstLetter" select="substring(@name, 1,1)"/>
		<xsl:variable name="className" select = "concat(translate($firstLetter, $lo, $up), substring(@name, 2, 30))"/>

	public class <xsl:value-of select="$className"/>Event extends Event
	{
		public static const GET:String = 'result<xsl:value-of select="@name"/>';

		public function <xsl:value-of select="$className"/>Event( type:String, <xsl:for-each select='arg'> <xsl:value-of select="@name" />:<xsl:value-of select="@type" />, </xsl:for-each> bubbles:Boolean=true, cancelable:Boolean=false)
		{
			super( type, bubbles, cancelable );		
			<xsl:for-each select='arg'>
			_<xsl:value-of select="@name" /> = <xsl:value-of select="@name" />;
			</xsl:for-each>
		}
		<xsl:for-each select='arg'>
		private var	_<xsl:value-of select="@name" />:<xsl:value-of select="@type" />;
		</xsl:for-each>
		<xsl:for-each select='arg'>
		public function get <xsl:value-of select="@name" />():<xsl:value-of select="@type" />
		{
			return _<xsl:value-of select="@name" />;
		}
		</xsl:for-each>
	}
}
 </xsl:template>

 <xsl:template name="faultEvent">  
package <xsl:value-of select="../@namespace" />.events
{
	import flash.events.Event;
	import mx.rpc.Fault;
	 <xsl:call-template name="codegen.import.alltypes"/>
	
	<xsl:variable name="firstLetter" select="substring(@name, 1,1)"/>
	<xsl:variable name="className" select = "concat(translate($firstLetter, $lo, $up), substring(@name, 2, 30))"/>

	public class <xsl:value-of select="$className" />FaultEvent extends Event
	{
		public static const FAULT_<xsl:value-of select="@name"/>:String = 'fault<xsl:value-of select="@name"/>';
	
		public function <xsl:value-of select="$className"/>FaultEvent(type:String, fault:Fault, bubbles:Boolean=true, cancelable:Boolean=false)
		{
			super(type, bubbles, cancelable);
			_fault = fault;
		}
	
		private var _fault:Fault;

		public function get fault():Fault
		{
			return _fault;
		}		
	}
}
  </xsl:template>

  
 <xsl:template name="eventMap">  

<EventMap xmlns:mx="http://www.adobe.com/2006/mxml" xmlns="http://mate.asfusion.com/">

	<mx:Script>	

	<xsl:text disable-output-escaping="yes">&lt;![CDATA[ </xsl:text>
	
	import <xsl:value-of select="@namespace" />.views.*;
	import <xsl:value-of select="@namespace" />.business.*;
	import <xsl:value-of select="@namespace" />.events.*;
	
	<xsl:text disable-output-escaping="yes"> ]]&gt;</xsl:text>
	
	</mx:Script>

	
	 <xsl:for-each select='method'>
	 
	<xsl:variable name="firstLetter" select="substring(@name, 1,1)"/>
	<xsl:variable name="className" select = "concat(translate($firstLetter, $lo, $up), substring(@name, 2, 30))"/>
	<xsl:variable name="classEventGET" select = "concat('{', $className, 'Event.GET', '}')"/>
	
	<xsl:variable name="manager" select = "concat('{', //service/@name, 'Manager', '}')"/>
	<xsl:variable name="store" select = "concat(@name, 'Store')"/>
	
	<xsl:variable name="panel" select="concat('{', //service/@name, 'Panel', '}')"/>
	<xsl:variable name="argStructure" select="concat('{', @argStructure, '}')"/>

	<xsl:variable name="fullName" select = "//service/@fullname"/>
	<xsl:variable name="resultObject" select = "concat('{', 'resultObject', '}')" />

	<EventHandlers type="{$classEventGET}" debug="true">

		<xsl:text disable-output-escaping="yes">&lt;</xsl:text>RemoteObjectInvoker destination="GenericDestination" source="<xsl:value-of select="//service/@fullname" />" method="<xsl:value-of select="@name"/>"
		<xsl:if test="count(arg) != 0">
		arguments= "{[<xsl:for-each select='arg/@name'>event.<xsl:value-of select='.'/><xsl:if test='position()!=last()'>,</xsl:if>
</xsl:for-each>]}"
		</xsl:if>
		debug="true"<xsl:text disable-output-escaping="yes">&gt;</xsl:text>
		
		<resultHandlers>
			<MethodInvoker generator="{$manager}" 
				method="{$store}" arguments="{$resultObject}"/>
		</resultHandlers>
		
	<xsl:text disable-output-escaping="yes">&lt;</xsl:text>/RemoteObjectInvoker<xsl:text disable-output-escaping="yes">&gt;</xsl:text>

</EventHandlers>

	<xsl:if test="@type!='void'">
	<Injectors target="{$panel}">
		<PropertyInjector targetKey="_retVal_{@name}" source="{$manager}" sourceKey="retVal_{@name}" />
	</Injectors>
	</xsl:if>

</xsl:for-each>

</EventMap>

  </xsl:template>


</xsl:stylesheet>
