<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
                  xmlns:wdm="urn:schemas-themidnightcoders-com:xml-wdm"
                  xmlns:xs="http://www.w3.org/2001/XMLSchema" 
                  xmlns:codegen="urn:cogegen-xslt-lib:xslt"
                  xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
   
  <xsl:import href="../../../../import/codegen.import.keys.xslt"/>    
   
  <xsl:template name="codegen.server.csharp.data.odbc.remove">
    <xsl:variable name="class-name" select="codegen:getClassName(@name,@wdm:Schema)"   />
    <xsl:variable name="table" select="@name"   />
	<xsl:variable name="schema" select="@wdm:Schema"   />
    <xsl:variable name="functionParam" select="codegen:FunctionParameter($class-name)" />
	<xsl:variable name="pk" select="xs:key/@name"   />    
	
    #region Delete
    private const String SqlDelete = @"Delete From <xsl:value-of select="codegen:quoteIdentifier(@wdm:Schema)" />.<xsl:value-of select="codegen:quoteIdentifier($table)" />
    <xsl:if test="count(xs:key) != 0">
      Where
      <xsl:for-each select="xs:complexType/xs:attribute[concat('@',@name) = key('pk',$table)/@xpath]">
        <xsl:value-of select="codegen:quoteIdentifier(@name)" /> = ? <xsl:if test="position() != last()"> and </xsl:if>
      </xsl:for-each>
    </xsl:if>";
    [TransactionRequired]
    public override <xsl:value-of select="$class-name" /> remove(<xsl:value-of select="$class-name" /><xsl:text> </xsl:text><xsl:value-of select="$functionParam" />, bool cascade)
    {
    using( SynchronizationScope syncScope = new SynchronizationScope( Database ) )
    {
    using (DatabaseConnectionMonitor monitor = new DatabaseConnectionMonitor(Database))
    {

    if(cascade)
    {
    <xsl:for-each select="key('dependent',current()/xs:key/@name)">
      <xsl:variable name="child-table-pk" select="xs:key/@name" />
      <xsl:for-each select="xs:keyref[@refer = $pk]">
        <xsl:variable name="fk" select="@name" />
        <xsl:for-each select="key('table',$child-table-pk)">
          <xsl:variable name="child-class-name" select="codegen:getClassName(@name,@wdm:Schema)" />
          <xsl:variable name="child-class-field" select="codegen:getClassName(@name,@wdm:Schema)" />

          {

          <xsl:choose>
            <xsl:when test="count(xs:key/xs:field[@xpath = key('fkByName',$fk)/@xpath]) = count(xs:key/xs:field)">
              // one to one relation
            </xsl:when>
            <xsl:otherwise>
              // one to many relation
            </xsl:otherwise>
          </xsl:choose>

          <xsl:if test="codegen:singleCall(concat('remove-data-field-',$table,$schema, dataMapper, $child-class-field))"><xsl:value-of select="$child-class-name"/>DataMapper </xsl:if>dataMapper<xsl:value-of select="$child-class-field"/>
          = new <xsl:value-of select="$child-class-name"/>DataMapper(Database);

          <xsl:if test="codegen:singleCall(concat('remove-data-fields-',$table,$schema, items, $child-class-field))">List&lt;<xsl:value-of select="$child-class-name"/>&gt; </xsl:if>items<xsl:value-of select="$child-class-field"/> = dataMapper.findByRelated<xsl:value-of select="$class-name"/>(
          <xsl:text> </xsl:text><xsl:value-of select="$functionParam" />,
          new QueryOptions(false));

          foreach(<xsl:value-of select="$child-class-name"/> item in items)
          dataMapper.remove(item,true);
          }
        </xsl:for-each>
      </xsl:for-each>
    </xsl:for-each>
    }
    
    using(OdbcCommand sqlCommand = Database.CreateCommand(SqlDelete))
    {
    <xsl:for-each select="xs:complexType/xs:attribute[concat('@',@name) = key('pk',$table)/@xpath]">
            <xsl:variable name="property" select="codegen:getProperty($table,$schema,@name)" />
            
            sqlCommand.Parameters.AddWithValue("?", <xsl:value-of select="$functionParam" />.<xsl:value-of select="$property" />);
          </xsl:for-each>
            sqlCommand.ExecuteNonQuery();
          }
       }
      raiseAffected(<xsl:value-of select="$functionParam" />,DataMapperOperation.delete);
      syncScope.Invoke();
      }
      return registerRecord(<xsl:value-of select="$functionParam" />);
    }
    <!--
    [TransactionRequired]
    [Synchronized]
    public override <xsl:value-of select="$class-name" /> remove(<xsl:value-of select="$class-name" /><xsl:text> </xsl:text><xsl:value-of select="$functionParam" />)
    {
      return remove(<xsl:value-of select="$functionParam" />,true);
    }
    -->
    #endregion
  </xsl:template>
  
</xsl:stylesheet>