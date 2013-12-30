<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
  xmlns:codegen="urn:cogegen-xslt-lib:xslt"
  xmlns:wdm="urn:schemas-themidnightcoders-com:xml-wdm"
  xmlns:xs="http://www.w3.org/2001/XMLSchema" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:import href="codegen.server.csharp.domain.xslt"/>
  <xsl:import href="data/codegen.server.csharp.data.xslt"/>
  <xsl:import href="codegen.server.csharp.project.xslt"/>
  <xsl:import href="codegen.server.csharp.config.xslt"/>

  <xsl:template name="codegen.server.csharp">

    <xsl:call-template name="codegen.server.csharp.config" />
	<xsl:for-each select="/xs:schema/xs:element">
        <xsl:value-of select="codegen:setCurrentDatabase(@name)" />
      <xsl:variable name="database" select="@name"/>
      <xsl:if test="codegen:singleCall(concat('database-',@name))">
        <xsl:value-of select="codegen:setCurrentSchema('')"/>
        <file name="{concat('DB','.cs')}" overwrite="true">
          namespace <xsl:value-of select="codegen:getServerNamespace()" />
          {
          using System;
          <xsl:call-template name="codegen.server.csharp.domain.enviroment" />
          <xsl:if test="codegen:singleCall('enviroment-schema-db')">
            <xsl:call-template name="codegen.server.csharp.data.enviroment">
              <xsl:with-param name="from-template" select="'db'"/>
            </xsl:call-template>
          </xsl:if>

          <xsl:call-template name="codegen.server.csharp.data.database" />
          }
        </file>
        <file name="{codegen:getClassName(@name,'')}Db.cs">
          namespace <xsl:value-of select="codegen:getServerNamespace()" />
          {
          public partial class <xsl:value-of select="codegen:getClassName(@name,'')" />Db
          {

          }
          }
        </file>
      </xsl:if>
      <xsl:value-of select="codegen:setCurrentSchema(@wdm:Schema)"/>
      <xsl:variable name="schema-name" select="@wdm:Schema"/>
      <xsl:choose>
        <xsl:when test="codegen:CountSchemas($database) = '1'">
          <xsl:call-template name="codegen.server.csharp.engine">
            <xsl:with-param name="database" select="$database"/>
            <xsl:with-param name="schema-name" select="$schema-name"/>
          </xsl:call-template>
        </xsl:when>
        <xsl:otherwise>
          <folder name="{$schema-name}">
            <xsl:call-template name="codegen.server.csharp.engine">
              <xsl:with-param name="database" select="$database"/>
              <xsl:with-param name="schema-name" select="$schema-name"/>
            </xsl:call-template>
          </folder>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:for-each>
  </xsl:template>
  <xsl:template name="codegen.server.csharp.engine">
    <xsl:param name="database"/>
    <xsl:param name="schema-name"/>
    <file name="codegen.cs" overwrite="true">
      namespace <xsl:value-of select="codegen:getServerNamespace($schema-name)" />
      {
      using System;
      <xsl:call-template name="codegen.server.csharp.domain.enviroment" />
      <xsl:if test="codegen:singleCall(concat('enviroment-schema-',@wdm:Schema))">
        <xsl:call-template name="codegen.server.csharp.data.enviroment">
          <xsl:with-param name="from-template" select="@wdm:Schema"/>
        </xsl:call-template>
      </xsl:if>

      <xsl:for-each select="xs:complexType/xs:choice/xs:element[@wdm:DatabaseObjectType='table']">
        <xsl:variable name="class-name" select="codegen:getClassName(@name,@wdm:Schema)" />

        <xsl:value-of select="codegen:Progress(concat('Generating code for table ', $class-name))" />

        <xsl:call-template name="codegen.server.csharp.domain" />
        <xsl:call-template name="codegen.server.csharp.data">

        </xsl:call-template>
      </xsl:for-each>
      }
    </file>
    <xsl:for-each select="xs:complexType/xs:choice/xs:element[@wdm:DatabaseObjectType='table']">
      <xsl:variable name="class-name" select="codegen:getClassName(@name,@wdm:Schema)"   />
      <file name="{$class-name}.cs" overwrite="false">
        namespace <xsl:value-of select="codegen:getServerNamespace($schema-name)" />
        {
        public partial class <xsl:value-of select="$class-name"/>
        {

        }
        }
      </file>
      <file name="{$class-name}DataMapper.cs" overwrite="false">
        namespace <xsl:value-of select="codegen:getServerNamespace($schema-name)" />
        {
        public class <xsl:value-of select="$class-name"/>DataMapper :_<xsl:value-of select="$class-name"/>DataMapper
        {
        public <xsl:value-of select="$class-name"/>DataMapper()
        {}
        public <xsl:value-of select="$class-name"/>DataMapper(<xsl:value-of select="codegen:getClassName(../../../@name,'')" />Db database):base(database)
        {}
        }
        }
      </file>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>