<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
	xmlns:codegen="urn:cogegen-xslt-lib:xslt" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:import href="codegen.xslt" />
	<xsl:import href="codegen.invoke.ajax.xslt" />  

	<xsl:template name="codegen.flex.properties" />
	
	<xsl:template name="codegen.actionscript.properties" />

	<xsl:template name="codegen.invoke.method.name">
		<xsl:value-of select="@name" />
	</xsl:template>
	
	<xsl:template name="codegen.process.fullproject">
		<xsl:for-each select="/namespaces">
			<xsl:call-template name="codegen.process.namespace" />
		</xsl:for-each>

		<xsl:call-template name="codegen.appmain" />
	</xsl:template>
	
	<xsl:template name="codegen.appmain">
		<xsl:call-template name="codegen.service.invoker" />		
	</xsl:template>
	
	<xsl:template name="codegen.process.namespace">
    <xsl:variable name="includes">
      <xsl:call-template name="codegen.include.js.namespace"></xsl:call-template>
    </xsl:variable>
	<folder name="scripts">
		<file path="invokerapps/js/invokerappdemo.js" />	
		<file path="../scripts/WebORB.js" />
    <xsl:call-template name="codegen.datatype.registry"/>
		<xsl:call-template name="codegen.process.js.namespace" />	
	</folder>
	</xsl:template>
	
	<xsl:template name="codegen.process.js.namespace">
		<xsl:for-each select="namespace">
      <xsl:if test="count(current()//service) &gt; 0 or count(current()//datatype) &gt; 0">
        <folder name="{@name}">
          <xsl:call-template name="codegen.process.js.namespace" />
          <xsl:if test="count(current()/service) &gt; 0 or count(current()/datatype) &gt; 0">
            <file name="{@name}.js">
              <xsl:for-each select="service">
                <xsl:call-template name="codegen.service" />
              </xsl:for-each>
              <xsl:call-template name="codegen.datatype" />
            </file>
          </xsl:if>
        </folder>
      </xsl:if>
		</xsl:for-each>
	</xsl:template>

  <xsl:template name="codegen.includes.js.namespace">
    <xsl:for-each select="/namespaces">
      <xsl:call-template name="codegen.include.js.namespace"/>
    </xsl:for-each>
  </xsl:template>
  
  <xsl:template name="codegen.include.js.namespace">
    <xsl:for-each select="namespace">
      <xsl:if test="count(current()//datatype) &gt; 0">
        <xsl:call-template name="codegen.include.js.namespace"/>
        <xsl:if test="count(current()/datatype) &gt; 0">
          <xsl:variable name="nameWithSlashes">
            <xsl:call-template name="string-replace-all">
              <xsl:with-param name="text" select="@fullname" />
              <xsl:with-param name="replace" select="'.'" />
              <xsl:with-param name="by" select="'/'" />
            </xsl:call-template>
          </xsl:variable>
          <![CDATA[<script type="text/javascript" src="scripts/]]><xsl:value-of select="concat($nameWithSlashes, '/', @name, '.js')" /><![CDATA[" ></script>]]>
        </xsl:if>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="codegen.service">
    <xsl:call-template name="codegen.description">
      <xsl:with-param name="file-name" select="concat(@name,'.js')" />
    </xsl:call-template>
    <xsl:call-template name="codegen.code" />
  </xsl:template>
  
  <xsl:template name="codegen.datatype">
    <xsl:for-each select="datatype">
      function <xsl:value-of select="@name" />()
      {
      }

      <xsl:value-of select="@name" />.prototype =
      {
      <xsl:call-template name="codegen.datatype.loadfields"/>
      }

      function <xsl:call-template name='codegen.datatype.factory.name'/>( obj )
      {
        var new<xsl:value-of select="@name" />Obj = new <xsl:value-of select="@name" />();
      <xsl:call-template name="codegen.datatype.factory">
        <xsl:with-param name="prefix" select="@name"/>
      </xsl:call-template>
        return new<xsl:value-of select="@name" />Obj;
      }
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="codegen.datatype.factory">
    <xsl:param name="prefix"/>
    <xsl:param name="processedFields" select="''"/>
    <xsl:for-each select="field">
      <xsl:if test="not(contains($processedFields,concat(@name,',')))">
        new<xsl:value-of select="$prefix" />Obj.<xsl:value-of select="@name" /> = obj.<xsl:value-of select="@name" />;
      </xsl:if>
    </xsl:for-each>
    
    <xsl:variable name="fields">
      <xsl:for-each select="field">
        <xsl:value-of select="@name"/>,
      </xsl:for-each>
      <xsl:value-of select="$processedFields"/>,
    </xsl:variable>
    
    <xsl:if test="@parentName">
      <xsl:text>  
        </xsl:text>//Inherited fields from <xsl:value-of select="@parentNamespace"/>.<xsl:value-of select="@parentName"/>
      <xsl:text>
      </xsl:text>
      <xsl:variable name="parentNamespace" select="@parentNamespace"/>
      <xsl:variable name="parentName" select="@parentName"/>
      <xsl:for-each select="//datatype[@typeNamespace=$parentNamespace and @name=$parentName]">
        <xsl:call-template name="codegen.datatype.factory">
          <xsl:with-param name="prefix" select="$prefix"/>
          <xsl:with-param name="processedFields" select="$fields"/>
        </xsl:call-template>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>

  <xsl:template name="codegen.datatype.loadfields">
    <xsl:param name="processedFields" select="''"/>
    <xsl:for-each select="field">
      <xsl:if test="not(contains($processedFields,concat(@name,',')))">
        <xsl:call-template name="codegen.datatype.field"/>
      </xsl:if>
    </xsl:for-each>
    <xsl:variable name="fields">
      <xsl:for-each select="field">
        <xsl:value-of select="@name"/>,
      </xsl:for-each>
      <xsl:value-of select="$processedFields"/>,
    </xsl:variable>
    <xsl:if test="@parentName">
      <xsl:text>  
        </xsl:text>//Inherited fields from <xsl:value-of select="@parentNamespace"/>.<xsl:value-of select="@parentName"/>
      <xsl:text>
      </xsl:text>
      <xsl:variable name="parentNamespace" select="@parentNamespace"/>
      <xsl:variable name="parentName" select="@parentName"/>
      <xsl:for-each select="//datatype[@typeNamespace=$parentNamespace and @name=$parentName]">
        <xsl:call-template name="codegen.datatype.loadfields">
          <xsl:with-param name="processedFields" select="$fields"/>
        </xsl:call-template>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>
  
  <xsl:template name="codegen.datatype.field">
      <xsl:text>  </xsl:text><xsl:value-of select="@name" />:<xsl:choose>
      <xsl:when test="@type='String'">''</xsl:when>
      <xsl:when test="@type='Boolean'">false</xsl:when>
      <xsl:when test="@type='Date'">new Date()</xsl:when>
      <xsl:when test="@type='int' or @type = 'Number'">0</xsl:when>
      <xsl:when test="@type='Array'">
        [<xsl:call-template name="codegen.get.typeprocessor">
          <xsl:with-param name="datatype-name" select="@elementType" />
        </xsl:call-template>]
      </xsl:when>
      <xsl:when test="@type='Object'">null</xsl:when>
      <xsl:otherwise>
        <xsl:call-template name="codegen.get.typeprocessor">
          <xsl:with-param name="datatype-name" select="@type" />
        </xsl:call-template>
      </xsl:otherwise>
      </xsl:choose><xsl:text>,
      </xsl:text>
  </xsl:template>
	
	<xsl:template name="codegen.service.invoker">
<xsl:for-each select="//service">	
   <xsl:variable name="fullpath" select = "concat( @fullname,'.js')" />
   <xsl:variable name="nameWithSlashes">
    <xsl:call-template name="string-replace-all">
      <xsl:with-param name="text" select="../@fullname" />
      <xsl:with-param name="replace" select="'.'" />
      <xsl:with-param name="by" select="'/'" />
    </xsl:call-template>
  </xsl:variable>
		<file name="{@name}Invoker.html">
    <![CDATA[<html><head>
  <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
  <title>WebORB AJAX Invoker</title>
  <script type="text/javascript" src="scripts/WebORB.js" ></script>
  <script type="text/javascript" src="scripts/invokerappdemo.js" ></script>
  <script type="text/javascript" src="scripts/DatatypeRegistry.js" ></script>
  <script type="text/javascript" src="scripts/]]><xsl:value-of select="concat($nameWithSlashes, '/', ../@name, '.js')" /><![CDATA[" ></script>]]>
      <xsl:call-template name="codegen.includes.js.namespace"/>
  <![CDATA[  
  
		<style type="text/css">
body {font-family: Helvetica, Arial, sans-serif;margin: 0;background-color: #CCC;}
html[high-color-depth] body {background: -webkit-gradient(linear, 0% 0%, 0% 100%, from(#CCC), to(#AAA));background-attachment: fixed;}
#mainTable {margin:20px;}
#mainTable td {padding:5px;}
#sourcecode {font-family: "Courier New", Courier, monospace;font-size: 8pt;line-height:95%;}
#intro {background-color:#FFF;border:1px solid #9c9c9c;padding:10px;font-size:10pt;}
.box {background-color: white;color: black;font-size: 10pt;line-height: 18px; -webkit-box-shadow: 2px 5px 12px #555;border-radius: 5px;margin: auto;padding: 20px;}
.box .header {background-color: #ddd;margin-bottom:10px;padding:3px;}
div p {padding:1px;margin:5px 0px 0px 0px;}

.calendar-box {display:none;background-color:#fff;border:1px solid #444;position:absolute;width:250px;padding: 0 5px;font-family: 'Lucida Sans Unicode', 'Lucida Grande', sans-serif;font-size: 12px;  }
.calendar-box select.calendar-month {width:90px;}
.calendar-box select.calendar-year {width:70px;}
.calendar-box .calendar-cancel {width:100%;}
.calendar-box table td {width:14%;}
.calendar-box .calendar-title { text-align:center; }
.calendar-box a { text-decoration:none; }
.calendar-box .today a {padding:0 5px; margin-left:-5px;background-color:#ffe9c6; } 
.calendar-box .selected a { padding:0 5px; margin-left:-5px; background-color:#c9ff8b; }
.header #classname {font-weight:bold;}
.dataTable { border: 1px solid #69C; border-width: 1px; border-style:solid; border-color: #69C; border-collapse: collapse; font-family: 'Lucida Sans Unicode', 'Lucida Grande', sans-serif; font-size: 12px; margin: 10px; text-align: left; width: 100%; }
.dataTable th { color: #039; font-size: 12px; font-weight: normal; padding: 1px 3px 1px 3px; }
.dataTable tbody { background: #E8EDFF; }
.dataTable td {  border-top: 1px dashed white; border-top-width: 1px;  border-top-style: dashed;  border-top-color: white;  color: #669;  padding: 1px 5px 1px 5px;}

#acc {list-style:none; color:#033; margin:0px;padding:0px}
#acc li {border:1px solid #ddd;margin-bottom:5px;}
#acc h3 {padding:3px; margin-top:0px; margin-bottom:5px; cursor:pointer;background-color: #ddd;font-weight:normal;font-size:10pt}
#acc .acc-section {overflow:hidden; background:#fff}
#acc .acc-content {padding:5px; background:#fff}
#invhistory .historyline:hover{ background-color:#ddd;}
#invhistory .ts{ padding-right:20px;}
#invhistory .method{padding-right:20px;width:70%}
#invhistory .duration{float:right}
</style>
</head>
<body>
<table id="mainTable">
<tbody>
<tr><td colspan="2"><div id="intro">This is a sample JavaScript application demonstrating remote invocations of the ']]><xsl:value-of select="@fullname" /><![CDATA[' class using the WebORB JavaScript/AJAX Client API. To invoke a remote method, select a method name from the dropdown list, enter argument values and click the 'Invoke method' button. The area on the right contains JavaScript/HTML source code which will perform the same method invocation.</div></td></tr>
<tr>
<td valign="top">
<div class="box">
<div class="header">Method Invoker for <span id="classname"></span></div>
		<div id="outputWrapper">
			<label>Select a method to invoke:</label><br/>
			<select onChange="methodSelected();" id="availableSignatures"></select>
		</div>
		<span id="methodInfo"></span>
		<div style="display: none" id="requestedParametersWrapper">
			<p>Method arguments:</p>
			<div id="argsWrapper"></div>
			<button onclick="onInvoke();" id="invokeMethod">Invoke method</button>
		</div>
		<div style="display: none" id="resultWrapper">
			<p>Result:</p>
			<div id="resultTable"></div>
		</div>
</div>		
</td><td valign="top">
<div class="box">
<ul class="acc" id="acc">
	<li>						
		<h3>AJAX invocation source code:</h3>
		<div class="acc-section"><div class="acc-content"><div id="sourcecode"></div></div></div>
	</li>
	<li>
		<h3>Invocation history:</h3>
		<div class="acc-section"><div class="acc-content"><div id="invhistory"></div><a href="javascript:clearInvHistory()">clear</a></div></div>
	</li>
</ul>	
</div></td></tr>
</tbody>
</table>
<script type="text/javascript">
var parentAccordion=new TINY.accordion.slider("parentAccordion");
parentAccordion.init("acc","h3",0,0);
var depth = 0;
var counter = 0;
var nameCounter = 0;
var model = new Array();		
var className = "]]><xsl:value-of select="@fullname" /><![CDATA[";
var serviceScriptPath = "]]><xsl:value-of select="concat($nameWithSlashes,'.js')" /><![CDATA[";
var serviceClass = "]]><xsl:value-of select="@name" /><![CDATA[";
var service = new ]]><xsl:value-of select="@name" /><![CDATA[();
var methodInfo = []]><xsl:for-each select="method">
        {name:'<xsl:value-of select="@name" />', args:[<xsl:for-each select="arg"><xsl:if test="position() != 1">, </xsl:if>
<xsl:call-template name="codegen.get.typeprocessor"><xsl:with-param name="datatype-name" select="@type" /></xsl:call-template>
</xsl:for-each>]}<xsl:if test="position() != last()">, </xsl:if>
			</xsl:for-each><![CDATA[
			];
initDisplay();
</script>
</body>
</html>	]]>			
		</file>
	</xsl:for-each>	
	</xsl:template>	
	
	<xsl:template name="codegen.get.complextype.struct">
	   <xsl:param name="datatype-name" />
			<xsl:for-each select="//datatype[@name = $datatype-name]">
				<xsl:for-each select="field"><xsl:if test="position() != 1">, </xsl:if><xsl:value-of select="@name" />:<xsl:call-template name="codegen.get.typeprocessor"><xsl:with-param name="datatype-name" select="@type" /></xsl:call-template>
</xsl:for-each>
			</xsl:for-each>	   				
	</xsl:template>

  <xsl:template name="codegen.get.typeprocessor">
    <xsl:param name="datatype-name" />
    <xsl:param name="isElementTypePassed" select="'0'" />
    <xsl:choose>
      <xsl:when test="$datatype-name='String'">'string'</xsl:when>
      <xsl:when test="$datatype-name='Boolean'">'boolean'</xsl:when>
      <xsl:when test="$datatype-name='Date'">'date'</xsl:when>
      <xsl:when test="$datatype-name='int' or $datatype-name = 'Number'">'number'</xsl:when>
      <xsl:when test="$datatype-name='Object'">{}</xsl:when>
      <xsl:when test="$datatype-name='Array'">
        <xsl:text>[</xsl:text><xsl:choose>
          <xsl:when test="$isElementTypePassed = '1'">
            <xsl:call-template name="codegen.get.typeprocessor">
              <xsl:with-param name="datatype-name" select="'Object'" />
            </xsl:call-template>
          </xsl:when>
          <xsl:otherwise>
            <xsl:call-template name="codegen.get.typeprocessor">
              <xsl:with-param name="datatype-name" select="@elementType" />
              <xsl:with-param name="isElementTypePassed" select="'1'" />
            </xsl:call-template>
          </xsl:otherwise>
      </xsl:choose><xsl:text>]</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        {__woproto__:"<xsl:value-of select='$datatype-name' />"}
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name='codegen.datatype.factory.name'>
    <xsl:variable name='name' select='@name'/>
    <xsl:choose>
      <xsl:when test='count(//datatype[@name=$name]) > 1'>
        <xsl:call-template name='string-replace-all'>
          <xsl:with-param name='text' select='@typeNamespace'/>
          <xsl:with-param name='by' select='"_"'/>
          <xsl:with-param name='replace' select='"."'/>
        </xsl:call-template>_<xsl:value-of select='@name'/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select='@name'/>
      </xsl:otherwise>
    </xsl:choose>Factory</xsl:template>
		
<xsl:template name="codegen.code">
	function <xsl:value-of select='@name' />()
	{
		this.proxy = webORB.bind( "<xsl:value-of select='@fullname' />", "<xsl:value-of select='@url' />" );
    
    registryDatatype();
    		
	  // initialize the member function references for the class prototype
	  if (typeof(_<xsl:value-of select='@name' />_prototype_called) == 'undefined')
	  {
	    _<xsl:value-of select='@name' />_prototype_called = true;
	    <xsl:variable name="servicename" select = "@name" />
	    <xsl:for-each select='method'>
	    <xsl:value-of select='$servicename' />.prototype.<xsl:value-of select='@name' /> = <xsl:value-of select='@name' />;
	    </xsl:for-each> 
	  }		
	}

	<xsl:for-each select='method'>
	function <xsl:value-of select='@name' />( <xsl:for-each select="arg"><xsl:if test="position() != 1">, </xsl:if><xsl:value-of select="@name" /></xsl:for-each><xsl:if test="count(arg) != 0">,</xsl:if> asyncObj )
	{
	  if( asyncObj )
	    this.proxy.<xsl:value-of select='@name' />( <xsl:for-each select="arg"><xsl:if test="position() != 1">, </xsl:if><xsl:value-of select="@name" /></xsl:for-each>
			<xsl:if test="count(arg) != 0">,</xsl:if> asyncObj );
	  else
	    return this.proxy.<xsl:value-of select='@name' />( <xsl:for-each select="arg"><xsl:if test="position() != 1">, </xsl:if><xsl:value-of select="@name" /></xsl:for-each> );
	}
		</xsl:for-each>
	
	<xsl:for-each select="method[@containsvalues=1]">
	function TestDrive()
	{
	  <xsl:call-template name="codegen.invoke.method" />
	}
	</xsl:for-each>
</xsl:template>
  <xsl:template name="codegen.datatype.registry">
    <file name="DatatypeRegistry.js">
      <xsl:call-template name="codegen.description">
        <xsl:with-param name="file-name" select="'DatatypeRegestry.js'" />
      </xsl:call-template>
      function registryDatatype()
      {
      <xsl:if test='count(//datatype)>0'>
        <xsl:for-each select='//datatype'>
        webORB.registerTypeFactory( "<xsl:value-of select='@typeNamespace'/>.<xsl:value-of select='@name'/>", <xsl:call-template name='codegen.datatype.factory.name'/> );
        </xsl:for-each>
      </xsl:if>
      }
    </file>
  </xsl:template>
</xsl:stylesheet>