<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
                  xmlns:wdm="urn:schemas-themidnightcoders-com:xml-wdm"
                  xmlns:xs="http://www.w3.org/2001/XMLSchema" 
                  xmlns:codegen="urn:cogegen-xslt-lib:xslt"
                  xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template name="codegen.server.csharp.data.oracle.findByPrimaryKey">
    <xsl:variable name="class-name" select="codegen:getClassName(@name,@wdm:Schema)"   />
    <xsl:variable name="table" select="@name"   />
	<xsl:variable name="schema" select="@wdm:Schema"   />
    <xsl:variable name="functionParam" select="codegen:FunctionParameter($class-name)" />

    private const String SqlSelectByPk = @"Select "+
    <xsl:for-each select="xs:complexType/xs:attribute">
      "\"<xsl:value-of select="@name" />\" <xsl:if test="position() != last()">,</xsl:if> "+
    </xsl:for-each>
      "From \"<xsl:value-of select="@wdm:Schema" />\".\"<xsl:value-of select="$table" />\""
    <xsl:if test="count(xs:key) != 0">
       +"Where "
      <xsl:for-each select="xs:complexType/xs:attribute[concat('@',@name) = key('pk',$table)/@xpath]">
         + "\"<xsl:value-of select="@name" />\" = :<xsl:value-of select="codegen:getFunctionParameter(@name)" /> <xsl:if test="position() != last()"> and </xsl:if> "
      </xsl:for-each>
    </xsl:if>
    ;

    public <xsl:value-of select="$class-name" /> findByPrimaryKey(
    <xsl:for-each select="xs:complexType/xs:attribute[concat('@',@name) = key('pk',$table)/@xpath]">
      <xsl:value-of select="codegen:CSharpDataType(@type)" />
      <xsl:text> </xsl:text>
      <xsl:value-of select="codegen:FunctionParameter(@name)" />
      <xsl:if test="position() != last()">,</xsl:if>
    </xsl:for-each>
    )
    {
    using (DatabaseConnectionMonitor monitor = new DatabaseConnectionMonitor(Database))
    {
    using(OracleCommand sqlCommand = Database.CreateCommand(SqlSelectByPk))
    {
        sqlCommand.BindByName = true;
        <xsl:for-each select="xs:complexType/xs:attribute[concat('@',@name) = key('pk',$table)/@xpath]">
          sqlCommand.Parameters.Add("<xsl:value-of select="codegen:getFunctionParameter(@name)"/>", <xsl:value-of select="codegen:FunctionParameter(@name)" />);
        </xsl:for-each>

        using(IDataReader dataReader = sqlCommand.ExecuteReader())
        {
          if(dataReader.Read())
            return doLoad(dataReader);
        }
      }
     }      
      throw new DataNotFoundException("<xsl:value-of select="$table" /> not found, search by primary key");
 

    }


    public bool exists(<xsl:value-of select="$class-name" /> <xsl:text> </xsl:text><xsl:value-of select="$functionParam" />)
    {
      using (DatabaseConnectionMonitor monitor = new DatabaseConnectionMonitor(Database))
      {
          using(OracleCommand sqlCommand = Database.CreateCommand(SqlSelectByPk))
          {
              sqlCommand.BindByName = true;
              <xsl:for-each select="xs:complexType/xs:attribute[concat('@',@name) = key('pk',$table)/@xpath]">
                sqlCommand.Parameters.Add("<xsl:value-of select="@name"/>", <xsl:value-of select="$functionParam" />.<xsl:value-of select="codegen:getPropertyName($table,$schema,@name)" />);
              </xsl:for-each>

              using(IDataReader dataReader = sqlCommand.ExecuteReader())
              {
              return dataReader.Read();
              }
          }
      }
    }

    private const string CheckInSql = @"
    <xsl:for-each select="xs:complexType/xs:attribute[concat('@',@name) = key('pk',$table)/@xpath]">
      '<xsl:value-of select="$table"/>'.'<xsl:value-of select="@name" />' = @CheckIn<xsl:value-of select="@name" /> <xsl:if test="position() != last()"> and </xsl:if>
    </xsl:for-each>";

    protected override IDbCommand prepareCheckInCommand(DomainObject domainObject, string sqlQuery)
    {
      <xsl:value-of select="$class-name"/> _<xsl:value-of select="$class-name"/> = (<xsl:value-of select="$class-name"/>)domainObject;

      OracleCommand sqlCommand = Database.CreateCommand(modifyQueryForCheckIn(sqlQuery,CheckInSql));
      sqlCommand.BindByName = true;
      <xsl:for-each select="xs:complexType/xs:attribute[concat('@',@name) = key('pk',$table)/@xpath]">
        sqlCommand.Parameters.Add("CheckIn<xsl:value-of select="@name"/>", _<xsl:value-of select="$class-name"/>.<xsl:value-of select="codegen:getPropertyName($table,$schema,@name)" />);
      </xsl:for-each>

      return sqlCommand;
    }

  </xsl:template>

  
  </xsl:stylesheet>