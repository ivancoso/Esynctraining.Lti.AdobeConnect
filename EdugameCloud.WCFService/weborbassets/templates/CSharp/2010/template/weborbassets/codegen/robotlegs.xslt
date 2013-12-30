<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:codegen="urn:cogegen-xslt-lib:xslt" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:import href="codegen.xslt"/>

  <xsl:variable name="upper" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>
  <xsl:variable name="lower" select="'abcdefghijklmnopqrstuvwxyz'"/>

  <xsl:variable name="simpleTypes" select="'Object,String,int,uint,Number,Boolean,Array,Date,void'"/>

  <xsl:variable name="serviceNamespace" select="//service/@namespace"/>
  <xsl:variable name="serviceName" select="//service/@name"/>

  <!-- Converts method name to event const name, e.g. getPerson to GET_PERSON -->
  <xsl:template name="get-event-const">
    <xsl:param name="method"/>
    <xsl:if test="$method">
      <xsl:variable name="letter" select="substring($method, 1, 1)"/>
      <xsl:if test="contains($upper, $letter)">
        <xsl:text>_</xsl:text>
      </xsl:if>
      <xsl:value-of select="translate($letter, $lower, $upper)"/>
      <xsl:call-template name="get-event-const">
        <xsl:with-param name="method" select="substring($method, 2)"/>
      </xsl:call-template>
    </xsl:if>
  </xsl:template>

  <!-- Converts method name with first letter in lower case -->
  <xsl:template name="normalize-method-name">
    <xsl:param name="method"/>
    <xsl:variable name="firstLetter" select="substring($method, 1, 1)"/>
    <xsl:value-of select="concat(translate($firstLetter, $upper, $lower), substring($method, 2))"/>
  </xsl:template>

  <!-- Converts class name with first letter in upper case -->
  <xsl:template name="get-class-name">
    <xsl:param name="input"/>
    <xsl:variable name="firstLetter" select="substring($input, 1, 1)"/>
    <xsl:value-of select="concat(translate($firstLetter, $lower, $upper), substring($input, 2))"/>
  </xsl:template>

  <!-- Returns fully qualified name of type -->
  <xsl:template name="get-fully-qualified-type">
    <xsl:param name="type"/>
    <xsl:for-each select="//datatype">
      <xsl:if test="$type = @name">
        <xsl:value-of select="@typeNamespace"/><![CDATA[.vo.]]><xsl:value-of select="@name"/>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>


  <!-- copy of codegen.xslt with little changes -> libs folder is added -->
  <xsl:template name="codegen.process.fullproject">
    <folder name="src">
      <xsl:call-template name="codegen.projectspecific" />
      <xsl:for-each select="/namespaces">
        <xsl:call-template name="codegen.process.namespace" />
      </xsl:for-each>
      <xsl:call-template name="codegen.appmain"/>
    </folder>

    <xsl:call-template name="codegen.project.eclipse.flex" />
    <xsl:call-template name="codegen.project.eclipse.flex.properties" />
    <xsl:call-template name="codegen.project.eclipse.flex.actionscript.properties" />
    <xsl:call-template name="codegen.project.eclipse.flex.htmltemplate" />

    <xsl:call-template name="codegen.project.custom"/>
  </xsl:template>

  <!-- copy of codegen.xslt with little changes -->
  <xsl:template name="codegen.project.eclipse.flex.actionscript.properties.applications">
    <application path="Main.mxml" />
  </xsl:template>

  <!-- copy of codegen.xslt with little changes -->
  <xsl:template name="codegen.project.eclipse.flex.actionscript.properties.mainApplicationPath">Main.mxml</xsl:template>

  <!-- copy of codegen.project.eclipse.flex.xslt -> libs folder added -->
  <xsl:template name="codegen.project.eclipse.flex.actionscript.properties">
    <file name=".actionScriptProperties" type="xml">
      <actionScriptProperties version="3">
        <xsl:attribute name="mainApplicationPath">
          <xsl:call-template name="codegen.project.eclipse.flex.actionscript.properties.mainApplicationPath" />
        </xsl:attribute>

        <xsl:variable name="services_config">
          <xsl:choose>
            <xsl:when test ="//runtime/@supportMessaging = 'true'">weborb-services-config.xml</xsl:when>
            <xsl:otherwise>services-config.xml</xsl:otherwise>
          </xsl:choose>
        </xsl:variable>

        <xsl:variable name="service_folder">
          <xsl:choose>
            <xsl:when test ="//runtime/@supportMessaging = 'true'"></xsl:when>
            <xsl:otherwise>
              /<xsl:call-template name="codegen.project.eclipse.flex.name" />
            </xsl:otherwise>
          </xsl:choose>
        </xsl:variable>

        <xsl:variable name="htmlGenerate">
          <xsl:choose>
            <xsl:when test ="//runtime/@supportMessaging = 'true'">false</xsl:when>
            <xsl:otherwise>true</xsl:otherwise>
          </xsl:choose>
        </xsl:variable>

        <compiler	additionalCompilerArguments="-services &quot;{//runtime/@path}WEB-INF/flex/{$services_config}&quot; -locale en_US"
					copyDependentFiles="true" genableModuleDebug="false" generateAccessible="false"
          htmlExpressInstall="true" htmlGenerate="{$htmlGenerate}"
					htmlHistoryManagement="false" htmlPlayerVersion="9.0.0"
					htmlPlayerVersionCheck="true"
					outputFolderPath="bin-debug"
					sourceFolderPath="src" strict="true" useApolloConfig="false"
					verifyDigests="true" warn="true">
          <xsl:attribute name="outputFolderLocation">
            <xsl:value-of select="//runtime/@path" />
            <xsl:call-template name="codegen.project.eclipse.flex.name" />
          </xsl:attribute>

          <compilerSourcePath />
          <libraryPath defaultLinkType="1">
            <libraryPathEntry kind="4" path="">
              <modifiedEntries>
                <libraryPathEntry kind="3" linkType="1"
									path="${PROJECT_FRAMEWORKS}/libs/framework.swc" sourcepath="${PROJECT_FRAMEWORKS}/source"
									useDefaultLinkType="true" />
              </modifiedEntries>
              <excludedEntries>
                <libraryPathEntry kind="3" linkType="1"
									path="${PROJECT_FRAMEWORKS}/libs/qtp.swc" useDefaultLinkType="false" />
                <libraryPathEntry kind="3" linkType="1"
									path="${PROJECT_FRAMEWORKS}/libs/automation.swc"
									useDefaultLinkType="false" />
                <libraryPathEntry kind="3" linkType="1"
									path="${PROJECT_FRAMEWORKS}/libs/automation_dmv.swc"
									useDefaultLinkType="false" />
                <libraryPathEntry kind="3" linkType="1"
									path="${PROJECT_FRAMEWORKS}/libs/automation_agent.swc"
									useDefaultLinkType="false" />
              </excludedEntries>
            </libraryPathEntry>

            <xsl:if test="//runtime/@supportMessaging = 'true'">
              <libraryPathEntry kind="3" linkType="1"
                path="{//runtime/@path}weborbassets/wdm/weborb.swc"
                useDefaultLinkType="false" />
            </xsl:if>

            <libraryPathEntry kind="4" path="" />
            <libraryPathEntry kind="1" linkType="1" path="libs"/>

          </libraryPath>
          <sourceAttachmentPath>
            <sourceAttachmentPathEntry kind="3"
							linkType="1" path="${PROJECT_FRAMEWORKS}/libs/datavisualization.swc"
							sourcepath="${PROJECT_FRAMEWORKS}/source" useDefaultLinkType="false" />
            <sourceAttachmentPathEntry kind="3"
							linkType="1" path="${PROJECT_FRAMEWORKS}/libs/flex.swc"
							sourcepath="${PROJECT_FRAMEWORKS}/source" useDefaultLinkType="false" />
            <sourceAttachmentPathEntry kind="3"
							linkType="1" path="${PROJECT_FRAMEWORKS}/libs/framework.swc"
							sourcepath="${PROJECT_FRAMEWORKS}/source" useDefaultLinkType="true" />
          </sourceAttachmentPath>
        </compiler>
        <applications>
          <!-- application path="{//service/@name}.mxml" /-->
          <xsl:call-template name="codegen.project.eclipse.flex.actionscript.properties.applications" />
        </applications>
        <modules />
        <buildCSSFiles />
      </actionScriptProperties>
    </file>
  </xsl:template>
  
  <xsl:template name="codegen.appmain">
    <file name="Main.mxml"><![CDATA[<?xml version="1.0" encoding="utf-8"?>
<mx:Application xmlns:mx="http://www.adobe.com/2006/mxml" layout="absolute" minWidth="955" minHeight="600" xmlns:ctx="]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.*" xmlns:vw="]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.views.*">
    <ctx:]]><xsl:value-of select="$serviceName"/><![CDATA[Context contextView="{this}" />
    <vw:]]><xsl:value-of select="$serviceName"/><![CDATA[View />
</mx:Application>]]>
    </file>
  </xsl:template>
  
  <xsl:template name="codegen.project.custom">
    <folder name="libs">
      <file name="readme.txt">
Project structure generated by WebORB includes all the source code and project files except the
library file for the Robotlegs framework.
You need to download the robotlegs library v.1.4.0 or higher and put it into this folder.
For more details visit http://www.robotlegs.org/.

Don't forget to include libs directory in Flex Build Path in project properties (if it's not included).
      </file>
    </folder>
  </xsl:template>
  
  <xsl:template name="codegen.service">
    <folder name="controller">
      <xsl:call-template name="controller.serviceinvoker.files"/>
    </folder>
    <folder name="events">
      <xsl:call-template name="events.folder"/>
    </folder>
    <folder name="model">
      <xsl:call-template name="model.events.folder"/>
      <xsl:call-template name="model.file"/>
    </folder>
    <folder name="service">
      <xsl:call-template name="service.events.folder"/>
      <xsl:call-template name="service.interface.file"/>
      <xsl:call-template name="service.file"/>
    </folder>
    <folder name="views">
      <xsl:call-template name="views.files"/>
    </folder>
    <xsl:call-template name="datatype.initializer.file"/>
    <xsl:call-template name="robotlegs.context.file"/>
  </xsl:template>

  
  <!-- CONTROLLER FOLDER -->

  <!-- commands files in controller folder -->
  <xsl:template name="controller.serviceinvoker.files">
    <xsl:for-each select="//method">
      <xsl:call-template name="controller.serviceinvoker.command.file"/>
    </xsl:for-each>
    <xsl:for-each select="//method[@type != 'void']">
      <xsl:call-template name="controller.updater.command.file"/>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="controller.serviceinvoker.command.file">
    <xsl:variable name="classNamePrefix">
      <xsl:call-template name="get-class-name">
        <xsl:with-param name="input" select="@name"/>
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="flexMethodName">
      <xsl:call-template name="normalize-method-name">
        <xsl:with-param name="method" select="@name"/>
      </xsl:call-template>
    </xsl:variable>
    <file name="{$classNamePrefix}Command.as"><xsl:call-template name="codegen.description">
      <xsl:with-param name="file-name" select="concat($classNamePrefix, 'Command.as')"/>
    </xsl:call-template><![CDATA[package ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.controller
{
    import org.robotlegs.mvcs.Command;]]>
      <xsl:if test="count(arg) != 0"><![CDATA[
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.events.]]><xsl:value-of select="$classNamePrefix"/><![CDATA[Event;]]></xsl:if><![CDATA[
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.service.I]]><xsl:value-of select="$serviceName"/><![CDATA[Service;
    
    public class ]]><xsl:value-of select="$classNamePrefix"/><![CDATA[Command extends Command
    {]]><xsl:if test="count(arg) != 0"><![CDATA[
        [Inject]
        public var event:]]><xsl:value-of select="$classNamePrefix"/><![CDATA[Event;]]>
    </xsl:if><![CDATA[
        [Inject]
        public var service:I]]><xsl:value-of select="$serviceName"/><![CDATA[Service;
        
        override public function execute():void
        {
            service.]]><xsl:value-of select="$flexMethodName"/><![CDATA[(]]><xsl:call-template name="controller.serviceinvoker.serviceparams"/><![CDATA[);
        }
    }
}]]>
    </file>
  </xsl:template>

  <xsl:template name="controller.serviceinvoker.serviceparams">
    <xsl:for-each select="arg"><![CDATA[event.]]><xsl:value-of select="@name"/><xsl:if test="position() != last()"><![CDATA[, ]]></xsl:if>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="controller.updater.command.file">
    <xsl:variable name="classNameSuffix">
      <xsl:call-template name="get-class-name">
        <xsl:with-param name="input" select="@name"/>
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="flexNamePrefix">
      <xsl:call-template name="normalize-method-name">
        <xsl:with-param name="method" select="@name"/>
      </xsl:call-template>
    </xsl:variable>
    <file name="Update{$classNameSuffix}ResultCommand.as"><xsl:call-template name="codegen.description">
      <xsl:with-param name="file-name" select="concat('Update', $classNameSuffix, 'ResultCommand.as')"/>
    </xsl:call-template><![CDATA[package ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.controller
{
    import org.robotlegs.mvcs.Command;
    
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.model.]]><xsl:value-of select="$serviceName"/><![CDATA[Model;
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.service.events.]]><xsl:value-of select="$classNameSuffix"/><![CDATA[ResultEvent;
    
    public class Update]]><xsl:value-of select="$classNameSuffix"/><![CDATA[ResultCommand extends Command
    {
        [Inject]
        public var event:]]><xsl:value-of select="$classNameSuffix"/><![CDATA[ResultEvent;
        
        [Inject]
        public var model:]]><xsl:value-of select="$serviceName"/><![CDATA[Model;
        
        override public function execute():void
        {
            model.]]><xsl:value-of select="$flexNamePrefix"/><![CDATA[Result = event.result;
        }
    }
}]]>
    </file>
  </xsl:template>

  <!-- CONTROLLER FOLDER -->
  
  
  <!-- EVENTS FOLDER -->

  <xsl:template name="events.folder">
    <xsl:call-template name="service.main.event.file"/>
    <xsl:call-template name="service.other.events"/>
  </xsl:template>
  
  <xsl:template name="service.main.event.file">
    <xsl:if test="//method[count(arg) = 0]">
    <file name="{$serviceName}Event.as"><xsl:call-template name="codegen.description">
      <xsl:with-param name="file-name" select="concat($serviceName, 'Event.as')"/>
    </xsl:call-template><![CDATA[package ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.events
{
    import flash.events.Event;
    public class ]]><xsl:value-of select="$serviceName"/><![CDATA[Event extends Event
    {]]>
      <xsl:for-each select="//method">
        <xsl:if test="not(child::arg)">
          <xsl:variable name="eventStr">
            <xsl:call-template name="normalize-method-name">
              <xsl:with-param name="method" select="@name"/>
            </xsl:call-template>
          </xsl:variable>
          <xsl:variable name="eventConst">
            <xsl:call-template name="get-event-const">
              <xsl:with-param name="method" select="$eventStr"/>
            </xsl:call-template>
          </xsl:variable>
        <![CDATA[public static const ]]><xsl:value-of select="$eventConst"/><![CDATA[:String = "]]><xsl:value-of select="$eventStr"/><![CDATA[";]]>
        </xsl:if>
      </xsl:for-each>
      <![CDATA[
        public function ]]><xsl:value-of select="$serviceName"/><![CDATA[Event(type:String, bubbles:Boolean = false, cancelable:Boolean = false)
        {
            super(type, bubbles, cancelable);
        }
		
        override public function clone():Event
        {
            return new ]]><xsl:value-of select="$serviceName"/><![CDATA[Event(type, bubbles, cancelable);
        }
    }
}]]>
    </file>
    </xsl:if>
  </xsl:template>

  <xsl:template name="service.other.events">
    <xsl:for-each select="//method">
      <xsl:if test="child::arg">
        <xsl:call-template name="service.other.event.file"/>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <!-- for each method -->
  <xsl:template name="service.other.event.file">
    <xsl:variable name="classNamePrefix">
      <xsl:call-template name="get-class-name">
        <xsl:with-param name="input" select="@name"/>
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="eventStr">
      <xsl:call-template name="normalize-method-name">
        <xsl:with-param name="method" select="@name"/>
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="eventConst">
      <xsl:call-template name="get-event-const">
        <xsl:with-param name="method" select="$eventStr"/>
      </xsl:call-template>
    </xsl:variable>
    <file name="{$classNamePrefix}Event.as"><xsl:call-template name="codegen.description">
      <xsl:with-param name="file-name" select="concat($classNamePrefix, 'Event.as')"/>
    </xsl:call-template><![CDATA[package ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.events
{
    import flash.events.Event;]]><xsl:call-template name="event.imports"/><![CDATA[
    public class ]]><xsl:value-of select="$classNamePrefix"/><![CDATA[Event extends Event
    {
        public static const ]]><xsl:value-of select="$eventConst"/><![CDATA[:String = "]]><xsl:value-of select="$eventStr"/><![CDATA[";]]><![CDATA[
        
        public function ]]><xsl:value-of select="$classNamePrefix"/><![CDATA[Event(type:String, ]]><xsl:call-template name="event.constructor.params"/><![CDATA[bubbles:Boolean = false, cancelable:Boolean = false)
        {
            super(type, bubbles, cancelable);]]><xsl:call-template name="event.constructor.body"/><![CDATA[      
        }]]>
      <xsl:for-each select="arg"><![CDATA[
        private var _]]><xsl:value-of select="@name"/><![CDATA[:]]><xsl:value-of select="@type"/><![CDATA[;
        
        public function get ]]><xsl:value-of select="@name"/><![CDATA[():]]><xsl:value-of select="@type"/><![CDATA[
        {
            return _]]><xsl:value-of select="@name"/><![CDATA[;
        }]]>
      </xsl:for-each><![CDATA[
        override public function clone():Event
        {
            return new ]]><xsl:value-of select="$classNamePrefix"/><![CDATA[Event(type, ]]><xsl:call-template name="event.clone.params"/><![CDATA[bubbles, cancelable);
        }
    }
}]]>
      
    </file>
  </xsl:template>

  <!-- for method-->
  <xsl:template name="event.imports">
    <xsl:for-each select="arg">
      <xsl:if test="not(contains($simpleTypes, @type))"><![CDATA[
    import ]]><xsl:call-template name="get-fully-qualified-type">
          <xsl:with-param name="type" select="@type"/>
        </xsl:call-template><![CDATA[;]]>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>
  
  <xsl:template name="event.constructor.params">
    <xsl:for-each select="arg"><xsl:value-of select="@name"/><![CDATA[:]]><xsl:value-of select="@type"/><![CDATA[, ]]></xsl:for-each>
  </xsl:template>

  <xsl:template name="event.constructor.body">
    <xsl:for-each select="arg"><![CDATA[
            _]]><xsl:value-of select="@name"/><![CDATA[ = ]]><xsl:value-of select="@name"/><![CDATA[;]]></xsl:for-each>
  </xsl:template>

  <xsl:template name="event.clone.params">
    <xsl:for-each select="arg"><xsl:value-of select="@name"/><![CDATA[, ]]></xsl:for-each>
  </xsl:template>

  <!-- EVENTS FOLDER -->


  <!-- MODEL FOLDER -->

  <xsl:template name="model.events.folder">
    <folder name="events">
      <file name="{$serviceName}ModelEvent.as"><xsl:call-template name="codegen.description">
      <xsl:with-param name="file-name" select="concat($serviceName, 'ModelEvent.as')"/>
    </xsl:call-template><![CDATA[package ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.model.events
{
    import flash.events.Event;
    
    public class ]]><xsl:value-of select="$serviceName"/><![CDATA[ModelEvent extends Event
    {]]>
        <xsl:for-each select="//method[@type != 'void']">
          <xsl:variable name="eventStr">
            <xsl:call-template name="normalize-method-name">
              <xsl:with-param name="method" select="@name"/>
            </xsl:call-template>
          </xsl:variable>
          <xsl:variable name="eventConst">
            <xsl:call-template name="get-event-const">
              <xsl:with-param name="method" select="$eventStr"/>
            </xsl:call-template>
          </xsl:variable><![CDATA[
        public static const ]]><xsl:value-of select="$eventConst"/><![CDATA[_RESULT_CHANGED:String = "]]><xsl:value-of select="$eventStr"/><![CDATA[ResultChanged";]]>
        </xsl:for-each><![CDATA[
        public function ]]><xsl:value-of select="$serviceName"/><![CDATA[ModelEvent(type:String, bubbles:Boolean = false, cancelable:Boolean = false)
        {
            super(type, bubbles, cancelable);
        }
        
        override public function clone():Event
        {
            return new ]]><xsl:value-of select="$serviceName"/><![CDATA[ModelEvent(type, bubbles, cancelable);
        }
    }
}]]>
      </file>
    </folder>
  </xsl:template>

  <xsl:template name="model.file">
    <file name="{$serviceName}Model.as"><xsl:call-template name="codegen.description">
      <xsl:with-param name="file-name" select="concat($serviceName, 'Model.as')"/>
    </xsl:call-template><![CDATA[package ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.model
{
    import org.robotlegs.mvcs.Actor;
    
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.model.events.]]><xsl:value-of select="$serviceName"/><![CDATA[ModelEvent;]]>
      <xsl:call-template name="file.import.all.votypes.enums"/>
      <![CDATA[
    public class ]]><xsl:value-of select="$serviceName"/><![CDATA[Model extends Actor
    {
        // Model classes for use in the model tier encapsulate and provide an API for data.
        // Models send event notifications when work has been performed on the data model. 
        // Models are generally highly portable entities.]]>
      <xsl:call-template name="model.properties"/><![CDATA[
    }
}]]>
    </file>
  </xsl:template>

  <xsl:template name="model.properties">
    <xsl:for-each select="//method[@type != 'void']">
      <xsl:variable name="nameSuffix">
        <xsl:call-template name="normalize-method-name">
          <xsl:with-param name="method" select="@name"/>
        </xsl:call-template>
      </xsl:variable>
      <xsl:variable name="eventConst">
        <xsl:call-template name="get-event-const">
          <xsl:with-param name="method" select="$nameSuffix"/>
        </xsl:call-template>
      </xsl:variable><![CDATA[
        private var _]]><xsl:value-of select="$nameSuffix"/><![CDATA[Result:]]><xsl:value-of select="@type"/><![CDATA[;
        
        public function get ]]><xsl:value-of select="$nameSuffix"/><![CDATA[Result():]]><xsl:value-of select="@type"/><![CDATA[
        {
            return _]]><xsl:value-of select="$nameSuffix"/><![CDATA[Result;
        }
        
        public function set ]]><xsl:value-of select="$nameSuffix"/><![CDATA[Result(value:]]><xsl:value-of select="@type"/><![CDATA[):void
        {
            if (_]]><xsl:value-of select="$nameSuffix"/><![CDATA[Result == value)
                return;
            
            _]]><xsl:value-of select="$nameSuffix"/><![CDATA[Result = value;
            dispatch(new ]]><xsl:value-of select="$serviceName"/><![CDATA[ModelEvent(]]><xsl:value-of select="$serviceName"/><![CDATA[ModelEvent.]]><xsl:value-of select="$eventConst"/><![CDATA[_RESULT_CHANGED));
        }]]>
    </xsl:for-each>
  </xsl:template>
  
  <!-- MODEL FOLDER -->
  

  <!-- SERVICE FOLDER -->
  
  <!-- service.events folder -->
  <xsl:template name="service.events.folder">
    <folder name="events">
      <xsl:for-each select="//method">
        <xsl:call-template name="service.event.file"/>
      </xsl:for-each>
      <xsl:call-template name="service.faultEvent.file"/>
    </folder>
  </xsl:template>

  <!-- service event files  //template's called for each method -->
  <xsl:template name="service.event.file">
    <xsl:variable name="classNamePrefix">
      <xsl:call-template name="get-class-name">
        <xsl:with-param name="input" select="@name"/>
      </xsl:call-template>
    </xsl:variable>
    <file name="{$classNamePrefix}ResultEvent.as"><xsl:call-template name="codegen.description">
      <xsl:with-param name="file-name" select="concat($classNamePrefix, 'ResultEvent.as')"/>
    </xsl:call-template><![CDATA[package ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.service.events
{
    import flash.events.Event;]]>
      <xsl:if test="not(contains($simpleTypes, @type))"><![CDATA[
    import ]]><xsl:call-template name="get-fully-qualified-type">
          <xsl:with-param name="type" select="@type"/>
        </xsl:call-template><![CDATA[;]]>
      </xsl:if><![CDATA[
    public class ]]><xsl:value-of select="$classNamePrefix"/><![CDATA[ResultEvent extends Event
    {
        public static const RESULT:String = "]]><xsl:value-of select="$serviceName"/><![CDATA[Service.]]><xsl:value-of select="@name"/><![CDATA[Result";
        
        public function ]]><xsl:value-of select="$classNamePrefix"/><![CDATA[ResultEvent(type:String, ]]><xsl:call-template name="service.event.constructor.param"/><![CDATA[bubbles:Boolean = false, cancelable:Boolean = false)
        {
            super(type, bubbles, cancelable);]]><xsl:call-template name="service.event.constructor.body"/><![CDATA[
        }]]>
      <xsl:if test="@type != 'void'"><![CDATA[
        private var _result:]]><xsl:value-of select="@type"/><![CDATA[;
        
        public function get result():]]><xsl:value-of select="@type"/><![CDATA[
        {
            return _result;
        }]]>
      </xsl:if><![CDATA[  
        override public function clone():Event
        {
            return new ]]><xsl:value-of select="$classNamePrefix"/><![CDATA[ResultEvent(type, ]]><xsl:call-template name="service.event.clone.param"/><![CDATA[bubbles, cancelable);
        }
    }
}]]>
    </file>
  </xsl:template>

  <xsl:template name="service.event.constructor.param">
    <xsl:if test="@type != 'void'"><![CDATA[result:]]><xsl:value-of select="@type"/><![CDATA[, ]]></xsl:if>
  </xsl:template>

  <xsl:template name="service.event.constructor.body">
    <xsl:if test="@type != 'void'"><![CDATA[
            _result = result;]]></xsl:if></xsl:template>

  <xsl:template name="service.event.clone.param">
    <xsl:if test="@type != 'void'"><![CDATA[result, ]]></xsl:if>
  </xsl:template>

  <xsl:template name="service.faultEvent.file">
    <file name="{$serviceName}ServiceFaultEvent.as"><xsl:call-template name="codegen.description">
      <xsl:with-param name="file-name" select="concat($serviceName, 'ServiceFaultEvent.as')"/>
    </xsl:call-template><![CDATA[package ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.service.events
{
    import flash.events.Event;
    
    public class ]]><xsl:value-of select="$serviceName"/><![CDATA[ServiceFaultEvent extends Event
    {
        public static const FAULT:String = "]]><xsl:value-of select="$serviceName"/><![CDATA[Service.fault";
        
        public function ]]><xsl:value-of select="$serviceName"/><![CDATA[ServiceFaultEvent(type:String, message:String, bubbles:Boolean = false, cancelable:Boolean = false)
        {
            super(type, bubbles, cancelable);
            
            // include any parameters your application need to know by unsuccessful remote call
            _message = message;
        }
        
        private var _message:String;
		    
        public function get message():String
        {
            return _message;
        }
		    
        override public function clone():Event
        {
            return new ]]><xsl:value-of select="$serviceName"/><![CDATA[ServiceFaultEvent(type, message, bubbles, cancelable);
        }
    }
 }]]>    
    </file>
  </xsl:template>

  
  <xsl:template name="service.interface.file">
    <file name="I{$serviceName}Service.as"><xsl:call-template name="codegen.description">
      <xsl:with-param name="file-name" select="concat('I', $serviceName, 'Service.as')"/>
    </xsl:call-template><![CDATA[package ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.service
{
    import mx.rpc.IResponder;]]>
    <xsl:call-template name="file.import.all.votypes.enums"/>
      <![CDATA[
    public interface I]]><xsl:value-of select="$serviceName"/><![CDATA[Service
    {]]>
      <xsl:for-each select="//method">
        <xsl:variable name="flexMethodName">
          <xsl:call-template name="normalize-method-name">
            <xsl:with-param name="method" select="@name"/>
          </xsl:call-template>
        </xsl:variable><![CDATA[function ]]><xsl:value-of select="$flexMethodName"/><![CDATA[(]]><xsl:call-template name="service.flex.method.params"/><![CDATA[responder:IResponder = null):void;]]>
      </xsl:for-each><![CDATA[
    }
}]]>
    </file>
  </xsl:template>
  
  <xsl:template name="service.file">
    <file name="{$serviceName}Service.as"><xsl:call-template name="codegen.description">
      <xsl:with-param name="file-name" select="concat($serviceName, 'Service.as')"/>
    </xsl:call-template><![CDATA[package ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.service
{
    import mx.rpc.AsyncToken;
    import mx.rpc.IResponder;
    import mx.rpc.events.FaultEvent;
    import mx.rpc.events.ResultEvent;
    import mx.rpc.remoting.RemoteObject;
	
    import org.robotlegs.mvcs.Actor;]]>
      <xsl:call-template name="service.file.import.eventtypes"/><![CDATA[
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.service.events.]]><xsl:value-of select="$serviceName"/><![CDATA[ServiceFaultEvent;]]>
      <xsl:call-template name="file.import.all.votypes.enums"/>
      <![CDATA[
    public class ]]><xsl:value-of select="$serviceName"/><![CDATA[Service extends Actor implements I]]><xsl:value-of select="$serviceName"/><![CDATA[Service
    {
        public function ]]><xsl:value-of select="$serviceName"/><![CDATA[Service()
        {
            _remoteObject = new RemoteObject("GenericDestination");
            _remoteObject.source = "]]><xsl:value-of select="//service/@fullname"/><![CDATA[";]]>
      <xsl:for-each select="//method">
        <xsl:variable name="handlerSuffix">
          <xsl:call-template name="get-class-name">
            <xsl:with-param name="input" select="@name"/>
          </xsl:call-template>
        </xsl:variable><![CDATA[
            _remoteObject.]]><xsl:value-of select="@name"/><![CDATA[.addEventListener(ResultEvent.RESULT, on]]><xsl:value-of select="$handlerSuffix"/><![CDATA[Result);]]></xsl:for-each><![CDATA[
            _remoteObject.addEventListener(FaultEvent.FAULT, onFault);
        }
        
        private var _remoteObject:RemoteObject;]]>
      <xsl:for-each select="//method">
        <xsl:variable name="flexMethodName">
          <xsl:call-template name="normalize-method-name">
            <xsl:with-param name="method" select="@name"/>
          </xsl:call-template>
        </xsl:variable><![CDATA[
        public function ]]><xsl:value-of select="$flexMethodName"/><![CDATA[(]]><xsl:call-template name="service.flex.method.params"/><![CDATA[responder:IResponder = null):void
        {
            var token:AsyncToken = _remoteObject.]]><xsl:value-of select="@name"/><![CDATA[(]]><xsl:call-template name="service.methodcall.params"/><![CDATA[);
            if (responder)
                token.addResponder(responder);
        }]]>
      </xsl:for-each><![CDATA[
        // Service can change model properties directly
        // e.g. model.trackingResultVar = event.result;
        //
        // But in our case the service knows nothing about the model
        // Any mediator, model or command can listen to service events]]>
      <xsl:for-each select="//method">
        <xsl:variable name="methodNameSuffix">
          <xsl:call-template name="get-class-name">
            <xsl:with-param name="input" select="@name"/>
          </xsl:call-template>
        </xsl:variable><![CDATA[
        private function on]]><xsl:value-of select="$methodNameSuffix"/><![CDATA[Result(event:ResultEvent):void
        {]]><xsl:if test="@type != 'void'"><![CDATA[
            var result:]]><xsl:value-of select="@type"/><![CDATA[ = event.result as ]]><xsl:value-of select="@type"/><![CDATA[;
            // trigger Update]]><xsl:value-of select="$methodNameSuffix"/><![CDATA[ResultCommand]]></xsl:if><![CDATA[
            dispatch(new ]]><xsl:value-of select="$methodNameSuffix"/><![CDATA[ResultEvent(]]><xsl:value-of select="$methodNameSuffix"/><![CDATA[ResultEvent.RESULT]]><xsl:call-template name="service.eventhandler.param"/><![CDATA[));
        }]]>
      </xsl:for-each><![CDATA[
        private function onFault(event:FaultEvent):void
        {
            dispatch(new ]]><xsl:value-of select="$serviceName"/><![CDATA[ServiceFaultEvent(]]><xsl:value-of select="$serviceName"/><![CDATA[ServiceFaultEvent.FAULT, event.fault.message));
        }
    }
}]]>
    </file>
  </xsl:template>

  <xsl:template name="service.file.import.eventtypes">
    <xsl:for-each select="//method">
      <xsl:variable name="classPrefix">
        <xsl:call-template name="get-class-name">
          <xsl:with-param name="input" select="@name"/>
        </xsl:call-template>
      </xsl:variable><![CDATA[
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.service.events.]]><xsl:value-of select="$classPrefix"/><![CDATA[ResultEvent;]]></xsl:for-each>
  </xsl:template>

  <!-- called for method; defining parameters in methods -->
  <xsl:template name="service.flex.method.params">
    <xsl:for-each select="arg"><xsl:value-of select="@name"/><![CDATA[:]]><xsl:value-of select="@type"/><![CDATA[, ]]></xsl:for-each>
  </xsl:template>

  <!-- calling methods with parameters -->
  <xsl:template name="service.methodcall.params">
    <xsl:for-each select="arg"><xsl:value-of select="@name"/><xsl:if test="position() != last()"><![CDATA[, ]]></xsl:if></xsl:for-each>
  </xsl:template>

  <xsl:template name="service.eventhandler.param">
    <xsl:if test="@type != 'void'"><![CDATA[, result]]></xsl:if>
  </xsl:template>
  
  <!-- SERVICE FOLDER -->
  

  <!-- VIEWS FOLDER -->

  <xsl:template name="views.files">
    <xsl:variable name="foundMethod" select="method[count(arg) = 0 and @type != 'void']"/>
    <xsl:variable name="methodNameSuffix">
      <xsl:call-template name="get-class-name">
        <xsl:with-param name="input" select="$foundMethod/@name"/>
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="eventStr">
      <xsl:call-template name="normalize-method-name">
        <xsl:with-param name="method" select="$foundMethod/@name"/>
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="eventConst">
      <xsl:call-template name="get-event-const">
        <xsl:with-param name="method" select="$eventStr"/>
      </xsl:call-template>
    </xsl:variable>
    <file name="{$serviceName}Mediator.as"><xsl:call-template name="codegen.description">
      <xsl:with-param name="file-name" select="concat($serviceName, 'Mediator.as')"/>
    </xsl:call-template><![CDATA[package ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.views
{
    import flash.events.MouseEvent;
    
    import mx.controls.Alert;
    
    import org.robotlegs.mvcs.Mediator;
    
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.model.]]><xsl:value-of select="$serviceName"/><![CDATA[Model;]]>
      <xsl:if test="$foundMethod"><![CDATA[
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.events.]]><xsl:value-of select="$serviceName"/><![CDATA[Event;
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.model.events.]]><xsl:value-of select="$serviceName"/><![CDATA[ModelEvent;
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[]]><![CDATA[.service.events.]]><xsl:value-of select="$serviceName"/><![CDATA[ServiceFaultEvent;]]>
      </xsl:if>
      <![CDATA[
    public class ]]><xsl:value-of select="$serviceName"/><![CDATA[Mediator extends Mediator
    {
        [Inject]
        public var view:]]><xsl:value-of select="$serviceName"/><![CDATA[View;
        
        [Inject]
        public var model:]]><xsl:value-of select="$serviceName"/><![CDATA[Model;
        
        override public function onRegister():void
        {
            // do mapping here, add context listeners]]>
      <xsl:if test="$foundMethod"><![CDATA[
            eventMap.mapListener(view.btnCall]]><xsl:value-of select="$methodNameSuffix"/><![CDATA[, MouseEvent.CLICK, onCall]]><xsl:value-of select="$methodNameSuffix"/><![CDATA[, MouseEvent);
            
            addContextListener(]]><xsl:value-of select="$serviceName"/><![CDATA[ServiceFaultEvent.FAULT, onFault, ]]><xsl:value-of select="$serviceName"/><![CDATA[ServiceFaultEvent);
            addContextListener(]]><xsl:value-of select="$serviceName"/><![CDATA[ModelEvent.]]><xsl:value-of select="$eventConst"/><![CDATA[_RESULT_CHANGED, on]]><xsl:value-of select="$methodNameSuffix"/><![CDATA[ResultChanged);]]>
      </xsl:if><![CDATA[
        }]]>
      <xsl:if test="$foundMethod">
        <![CDATA[
        private function onCall]]><xsl:value-of select="$methodNameSuffix"/><![CDATA[(event:MouseEvent):void
        {
            // trigger ]]><xsl:value-of select="$methodNameSuffix"/><![CDATA[Command
            dispatch(new ]]><xsl:value-of select="$serviceName"/><![CDATA[Event(]]><xsl:value-of select="$serviceName"/><![CDATA[Event.]]><xsl:value-of select="$eventConst"/><![CDATA[));
        }
        
        private function on]]><xsl:value-of select="$methodNameSuffix"/><![CDATA[ResultChanged(event:]]><xsl:value-of select="$serviceName"/><![CDATA[ModelEvent):void
        {
            // update view controls here
            
            var result:String = model.]]><xsl:value-of select="$eventStr"/><![CDATA[Result.toString();
            if (result == "")
                result = "< Empty string returned >";
            else if (result == null)
                result = "< Null string returned >";
			
            view.lblResult.text = result;
        }
        
        private function onFault(event:]]><xsl:value-of select="$serviceName"/><![CDATA[ServiceFaultEvent):void
        {
            Alert.show(event.message, "Error");
        }]]>
      </xsl:if><![CDATA[
    }
}]]>
    </file>
    <file name="{$serviceName}View.mxml"><![CDATA[<?xml version="1.0" encoding="utf-8"?>]]><![CDATA[
<!---]]>
    <xsl:value-of select="concat($serviceName, 'View.mxml')"/><![CDATA[
    Copyright (C) 2006-2012 Midnight Coders, Inc.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
-->
<mx:HBox xmlns:mx="http://www.adobe.com/2006/mxml" x="20" y="20">
    <mx:Button id="btnCall]]><xsl:value-of select="$methodNameSuffix"/><![CDATA[" label="Call ]]><xsl:value-of select="$methodNameSuffix"/><![CDATA[ Method" />
    <mx:Label id="lblResult" />
</mx:HBox>]]>
    </file>
  </xsl:template>
  
  <!-- VIEWS FOLDER -->

  <!-- TOP PACKAGE FILES -->

  <xsl:template name="datatype.initializer.file">
    <xsl:if test="count(//datatype) != 0">
      <file name="DataTypeInitializer.as"><![CDATA[/*****************************************************************
*
*  To force the compiler to include all the generated complex types
*  into the compiled application, add the following line of code
*  into the main function of your Flex application:
*
*  new ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.DataTypeInitializer();
*
*  (see ]]><xsl:value-of select="$serviceName"/><![CDATA[Context startup() method)
*
******************************************************************/
package ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[
{]]>
        <xsl:call-template name="file.import.all.vo.types"/>
        <![CDATA[
    public class DataTypeInitializer
    {
        public function DataTypeInitializer()
        {]]><xsl:for-each select="//datatype"><![CDATA[
            new ]]><xsl:value-of select="@name"/><![CDATA[();]]></xsl:for-each><![CDATA[
        }
    }
}]]>
      </file>
    </xsl:if>
  </xsl:template>

  <!-- context file -->
  <xsl:template name="robotlegs.context.file">
    <file name="{$serviceName}Context.as"><xsl:call-template name="codegen.description">
      <xsl:with-param name="file-name" select="concat($serviceName, 'Context.as')"/>
    </xsl:call-template><![CDATA[package ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[
{
    import org.robotlegs.mvcs.Context;]]>
      <xsl:call-template name="context.import.all.commands"/>
      <xsl:call-template name="context.import.events"/><![CDATA[
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.model.]]><xsl:value-of select="$serviceName"/><![CDATA[Model;
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.service.I]]><xsl:value-of select="$serviceName"/><![CDATA[Service;
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.service.]]><xsl:value-of select="$serviceName"/><![CDATA[Service;]]>
      <xsl:call-template name="context.import.service.events"/><![CDATA[
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.views.]]><xsl:value-of select="$serviceName"/><![CDATA[Mediator;
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.views.]]><xsl:value-of select="$serviceName"/><![CDATA[View;
    
    public class ]]><xsl:value-of select="$serviceName"/><![CDATA[Context extends Context
    {
        override public function startup():void
        {]]><xsl:if test="count(//datatype) != 0"><![CDATA[
            new DataTypeInitializer();]]>
        </xsl:if><![CDATA[
            // model
            injector.mapSingleton(]]><xsl:value-of select="$serviceName"/><![CDATA[Model);
            
            // service
            injector.mapSingletonOf(I]]><xsl:value-of select="$serviceName"/><![CDATA[Service, ]]><xsl:value-of select="$serviceName"/><![CDATA[Service);
            
            // views
            mediatorMap.mapView(]]><xsl:value-of select="$serviceName"/><![CDATA[View, ]]><xsl:value-of select="$serviceName"/><![CDATA[Mediator);
            
            // commands]]>
      <xsl:call-template name="context.startup.commandMap.withParams"/>
      <xsl:call-template name="context.startup.commandMap.withoutParams"/>
      <xsl:call-template name="context.statrup.commandMap.resultevents"/>
      <![CDATA[
            // go
            super.startup();
        }
    }
}]]>
    </file>
  </xsl:template>

  <xsl:template name="context.import.all.commands">
    <xsl:call-template name="context.import.all.serviceinvoker.commands"/>
    <xsl:call-template name="context.import.all.updater.commands"/>
  </xsl:template>

  <xsl:template name="context.import.all.serviceinvoker.commands">
    <xsl:for-each select="//method">
      <xsl:variable name="classNameSuffix">
        <xsl:call-template name="get-class-name">
          <xsl:with-param name="input" select="@name"/>
        </xsl:call-template>
      </xsl:variable><![CDATA[
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.controller.]]><xsl:value-of select="$classNameSuffix"/><![CDATA[Command;]]></xsl:for-each>
  </xsl:template>

  <xsl:template name="context.import.all.updater.commands">
    <xsl:for-each select="//method[@type != 'void']">
      <xsl:variable name="classNameSuffix">
        <xsl:call-template name="get-class-name">
          <xsl:with-param name="input" select="@name"/>
        </xsl:call-template>
      </xsl:variable><![CDATA[
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.controller.Update]]><xsl:value-of select="$classNameSuffix"/><![CDATA[ResultCommand;]]></xsl:for-each>
  </xsl:template>

  <xsl:template name="context.import.events">
    <xsl:for-each select="//method[count(arg) != 0]">
      <xsl:variable name="classNameSuffix">
        <xsl:call-template name="get-class-name">
          <xsl:with-param name="input" select="@name"/>
        </xsl:call-template>
      </xsl:variable><![CDATA[
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.events.]]><xsl:value-of select="$classNameSuffix"/><![CDATA[Event;]]></xsl:for-each>
    <xsl:if test="//method[count(arg) = 0]"><![CDATA[
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.events.]]><xsl:value-of select="$serviceName"/><![CDATA[Event;]]></xsl:if>
  </xsl:template>

  <xsl:template name="context.import.service.events">
    <xsl:for-each select="//method">
      <xsl:variable name="classNameSuffix">
        <xsl:call-template name="get-class-name">
          <xsl:with-param name="input" select="@name"/>
        </xsl:call-template>
      </xsl:variable><![CDATA[
    import ]]><xsl:value-of select="$serviceNamespace"/><![CDATA[.service.events.]]><xsl:value-of select="$classNameSuffix"/><![CDATA[ResultEvent;]]></xsl:for-each>
  </xsl:template>

  <!-- mapping events for methods with params -->
  <xsl:template name="context.startup.commandMap.withParams">
    <xsl:for-each select="//method[count(arg) != 0]">
      <xsl:variable name="classNameSuffix">
        <xsl:call-template name="get-class-name">
          <xsl:with-param name="input" select="@name"/>
        </xsl:call-template>
      </xsl:variable>
      <xsl:variable name="eventStr">
        <xsl:call-template name="normalize-method-name">
          <xsl:with-param name="method" select="@name"/>
        </xsl:call-template>
      </xsl:variable>
      <xsl:variable name="eventConst">
        <xsl:call-template name="get-event-const">
          <xsl:with-param name="method" select="$eventStr"/>
        </xsl:call-template>
      </xsl:variable><![CDATA[
            commandMap.mapEvent(]]><xsl:value-of select="$classNameSuffix"/><![CDATA[Event.]]><xsl:value-of select="$eventConst"/><![CDATA[, ]]><xsl:value-of select="$classNameSuffix"/><![CDATA[Command, ]]><xsl:value-of select="$classNameSuffix"/><![CDATA[Event);]]></xsl:for-each>
  </xsl:template>

  <!-- mapping events for methods without params -->
  <xsl:template name="context.startup.commandMap.withoutParams">
    <xsl:for-each select="//method[count(arg) = 0]">
      <xsl:variable name="classNameSuffix">
        <xsl:call-template name="get-class-name">
          <xsl:with-param name="input" select="@name"/>
        </xsl:call-template>
      </xsl:variable>
      <xsl:variable name="eventStr">
        <xsl:call-template name="normalize-method-name">
          <xsl:with-param name="method" select="@name"/>
        </xsl:call-template>
      </xsl:variable>
      <xsl:variable name="eventConst">
        <xsl:call-template name="get-event-const">
          <xsl:with-param name="method" select="$eventStr"/>
        </xsl:call-template>
      </xsl:variable><![CDATA[
            commandMap.mapEvent(]]><xsl:value-of select="$serviceName"/><![CDATA[Event.]]><xsl:value-of select="$eventConst"/><![CDATA[, ]]><xsl:value-of select="$classNameSuffix"/><![CDATA[Command, ]]><xsl:value-of select="$serviceName"/><![CDATA[Event);]]></xsl:for-each>
  </xsl:template>

  <!-- mapping service result events to commands -->
  <xsl:template name="context.statrup.commandMap.resultevents">
    <xsl:for-each select="//method[@type != 'void']">
      <xsl:variable name="classNameSuffix">
        <xsl:call-template name="get-class-name">
          <xsl:with-param name="input" select="@name"/>
        </xsl:call-template>
      </xsl:variable><![CDATA[
            commandMap.mapEvent(]]><xsl:value-of select="$classNameSuffix"/><![CDATA[ResultEvent.RESULT, Update]]><xsl:value-of select="$classNameSuffix"/><![CDATA[ResultCommand, ]]><xsl:value-of select="$classNameSuffix"/><![CDATA[ResultEvent);]]></xsl:for-each>
  </xsl:template>
  
  <!-- TOP PACKAGE FILES -->


  <xsl:template name="file.import.all.vo.types">
    <xsl:for-each select="//datatype"><![CDATA[
    import ]]><xsl:value-of select="@typeNamespace"/><![CDATA[.vo.]]><xsl:value-of select="@name"/><![CDATA[;]]></xsl:for-each>
  </xsl:template>

  <xsl:template name="file.import.all.enums">
    <xsl:for-each select="//enum"><![CDATA[
    import ]]><xsl:value-of select="@typeNamespace"/><![CDATA[.enum.]]><xsl:value-of select="@name"/><![CDATA[;]]></xsl:for-each>
  </xsl:template>

  <!-- import statements of all vo types and enums -->
  <xsl:template name="file.import.all.votypes.enums">
    <xsl:call-template name="file.import.all.vo.types"/>
    <xsl:call-template name="file.import.all.enums"/>
  </xsl:template>

</xsl:stylesheet>