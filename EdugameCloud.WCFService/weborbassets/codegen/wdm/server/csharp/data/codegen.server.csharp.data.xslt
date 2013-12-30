<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
                  xmlns:xs="http://www.w3.org/2001/XMLSchema"
                  xmlns:codegen="urn:cogegen-xslt-lib:xslt"
                  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                  xmlns:wdm="urn:schemas-themidnightcoders-com:xml-wdm">

  <xsl:import href="mssql/codegen.server.csharp.data.mssql.xslt"/>
  <xsl:import href="mysql/codegen.server.csharp.data.mysql.xslt"/>
  <xsl:import href="oracle/codegen.server.csharp.data.oracle.xslt"/>
  <xsl:import href="odbc/codegen.server.csharp.data.odbc.xslt"/>

  <xsl:template name="codegen.server.csharp.data.enviroment">
    <xsl:param name="from-template"/>
    using System.Data;
    using System.Collections;
    using System.Collections.Generic;
    using Weborb.Data.Management;

    <xsl:for-each select="/xs:schema/xs:element">
    <xsl:variable name="database" select="codegen:getDatabaseServerType(@name)" />
    <xsl:value-of select="codegen:setCurrentDatabase(@name)"/>

    <xsl:value-of select="codegen:Progress(concat('Database is ', $database))" />
      <xsl:if test="codegen:singleCall(concat('enviroment-schema-inner-', $from-template))">
        <xsl:choose>
          <xsl:when test="$database = 'mssql'">
            <xsl:call-template name="codegen.server.csharp.data.mssql.enviroment" />
          </xsl:when>
          <xsl:when test="$database = 'mysql'">
            <xsl:call-template name="codegen.server.csharp.data.mysql.enviroment" />
          </xsl:when>
          <xsl:when test="$database = 'oracle'">
            <xsl:call-template name="codegen.server.csharp.data.oracle.enviroment" />
          </xsl:when>
          <xsl:when test="$database = 'odbc'">
            <xsl:call-template name="codegen.server.csharp.data.odbc.enviroment" />
          </xsl:when>
        </xsl:choose>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="codegen.server.csharp.data">
    <xsl:value-of select="codegen:Progress(concat('Generating class ', @name,'DataMapper'))" />
    <xsl:variable name="database" select="codegen:getDatabaseServerType(current()/../../../@name)" />
    <xsl:choose>
      <xsl:when test="$database = 'mssql'">
        <xsl:call-template name="codegen.server.csharp.data.mssql" />
      </xsl:when>
      <xsl:when test="$database = 'mysql'">
        <xsl:call-template name="codegen.server.csharp.data.mysql" />
      </xsl:when>
      <xsl:when test="$database = 'oracle'">
        <xsl:call-template name="codegen.server.csharp.data.oracle" />
      </xsl:when>
      <xsl:when test="$database = 'odbc'">
        <xsl:call-template name="codegen.server.csharp.data.odbc" />
      </xsl:when>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="codegen.server.csharp.data.database">
    <xsl:variable name="database" select="codegen:getDatabaseServerType(@name)" />
    <xsl:value-of select="codegen:setCurrentDatabase(@name)"/>
    <xsl:if test="codegen:CountSchemas(codegen:getCurrentDatabase()) = 1">
      <xsl:value-of select="codegen:setCurrentSchema(@wdm:Schema)"/>
    </xsl:if>
    <xsl:choose>
      <xsl:when test="$database = 'mssql'">
        <xsl:call-template name="codegen.server.csharp.data.mssql.database" />
      </xsl:when>
      <xsl:when test="$database = 'mysql'">
        <xsl:call-template name="codegen.server.csharp.data.mysql.database" />
      </xsl:when>
      <xsl:when test="$database = 'oracle'">
        <xsl:call-template name="codegen.server.csharp.data.oracle.database" />
      </xsl:when>
      <xsl:when test="$database = 'odbc'">
        <xsl:call-template name="codegen.server.csharp.data.odbc.database" />
      </xsl:when>
    </xsl:choose>
  </xsl:template>

</xsl:stylesheet>