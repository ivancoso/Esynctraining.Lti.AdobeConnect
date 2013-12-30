var adapter, tableList,
    divNewRecord, divNewRecordCaption, divNewRecordContent,
    divCode, divCodeContent,
    divGridBlock,
    divPager, codeText, lstPageSize;

function Adapter(pagerId)
{
    this._pagerElement = document.getElementById( pagerId );

    this._dataMapper = null;
    this.columnsInfo = [];
    this.selectedTable = null;
    this.shownPageNo = 0;
    this._pageSize = 20;
    this._lastPageSize = 20;
    this._activeCollection = null;
    this._needReload = false;

    DataServiceClient.Instance.subscribe( __clientId__ );
    DataServiceClient.Instance.addEventListener( ActiveRecordEvent.DELETE, this._onSyncDelete, this );
    DataServiceClient.Instance.addEventListener( ActiveRecordEvent.UPDATE, this._onSyncUpdate, this );
    DataServiceClient.Instance.addEventListener( ActiveRecordEvent.CREATE, this._onSyncCreate, this );
}

Adapter.prototype._buildPager = function()
{
    var NUM_PAGES_ON_SIDE = 5;

    var s = "";
    if( this._activeCollection._pageSize > 0 && this._activeCollection._pageSize < this._activeCollection.getLength() )
    {
        var lastPageNo = Math.ceil( this._activeCollection.getLength() / this._activeCollection._pageSize );
        var needForm = lastPageNo > NUM_PAGES_ON_SIDE * 2 + 1;
        var first = Math.max( 1, this.shownPageNo - NUM_PAGES_ON_SIDE );
        var last = Math.min( lastPageNo, first + NUM_PAGES_ON_SIDE * 2 );
        if( last - this.shownPageNo < NUM_PAGES_ON_SIDE )
            first = Math.max( 1, last - NUM_PAGES_ON_SIDE * 2 );

        if( needForm )
            s = "<form onsubmit='return gotoPage( this );'>";

        if( first > 1 )
            s += "... &nbsp; ";

        for (var i = first; i <= last; ++i )
        {
            s += i == this.shownPageNo?
                "<span class=\"current-page\">" + i.toString() + "</span>"
                : "<a href='javascript:page(" + i + ")'>" + i + "</a>";
            s += " &nbsp; ";
        }

        if( last < lastPageNo )
            s += "...";

        if( needForm )
            s += "<input maxlength='5' class='gp' /></form>";
    }
    else
        s = "&nbsp;";

    this._pagerElement.innerHTML = s;
}

Adapter.prototype._onError = function( event )
{
    try { this._gridElement.innerHTML = "Error: " + event.data.description; }
    catch( e ) {}
}

Adapter.prototype._onLoadSync = function( event )
{
    this.publish( event.target );
}

Adapter.prototype.showPage = function( pageNo )
{
    if( this.shownPageNo <= 0 )
        return;

    if( this._needReload )
    {
        this._activeCollection.clear();
        this._needReload = false;
    }

    this.shownPageNo = Math.max( 1, Math.min( pageNo, Math.ceil( this._activeCollection.getLength() / this._activeCollection._pageSize ) ) );
    var rec = this._activeCollection.get( (this.shownPageNo - 1) * this._activeCollection._pageSize );
    if( rec == null || rec.getIsLoaded() )
        this.publish( this._activeCollection );
}

Adapter.prototype.addRecord = function( record, okFunc, errFunc )
{
    var activeRec = this._dataMapper.createActiveRecordInstance();
    Utils.copyValues( record, activeRec );

    activeRec.create(
        new Async(
            function( rec )
            {
                this.selectTable( this.selectedTable );
                if( okFunc instanceof Function ) okFunc();
            },
            function(err)
            {
                alert( "Error adding record: " + err.description );
                if( errFunc instanceof Function ) errFunc();
            },
            this
        )
    );
}

Adapter.prototype._onSyncCreate = function( ev )
{
    if( this._dataMapper == null )
        return;

    if( ev.record.getRemoteClassName() == this._dataMapper.createActiveRecordInstance().getRemoteClassName() )
        this._needReload = true;
}

Adapter.prototype.selectTable = function( tableName, pageSize )
{
    this.selectedTable = tableName;

    var schema = "";
    if(tableName.indexOf(".") != -1)
    {
        var arr = tableName.split(".");
        schema = arr[0];
        tableName = arr[1];
    }

    if( !pageSize )
        pageSize = this._lastPageSize;
    else
        this._lastPageSize = pageSize;

    this._pageSize = pageSize;

    if( tableName == null )
        return;

    if(schema == "")
        this._dataMapper = ActiveRecords[ tableName ];
    else
        this._dataMapper = ActiveRecords[ schema ][ tableName ];

    this.columnsInfo = this._dataMapper.createActiveRecordInstance().getColumnsInfo();

    this.shownPageNo = 1;
    this.showLoading();
    this._pagerElement.innerHTML = "";

    this._activeCollection = this._dataMapper.findAll( { PageSize: parseInt( this._pageSize ) } );
    this._activeCollection.addEventListener( DynamicLoadEvent.LOADED, this._onLoadSync, this );
    this._activeCollection.addEventListener( CollectionEvent.COLLECTION_CHANGE, this._onLoadSync, this );
    this._activeCollection.addEventListener( ErrorEvent.ERROR, this._onError, this );
}


// endof Adapter

function formatDefaultValue( value )
{
    return value instanceof Date? (value.getMonth() + 1) + "/" + value.getDate() + "/" + value.getFullYear() : value;
}

function onAddRecordClick()
{
    var row = {};
    var els = document.forms["newRec"].elements;

    for( var i = 0; i < adapter.columnsInfo.length; ++i )
    {
        var colInfo = adapter.columnsInfo[ i ];
        var input = els[ "__" + colInfo.name ];
        var value = input.value;
        var errMsg = null;

        if( colInfo.isRequired && value.length == 0 )
            errMsg = "Please set the required field [" + colInfo.name + "].";
        else if( colInfo.type == "date" && value.length > 0 && ( value = parseDate( input.value, true ) ) == null )
            errMsg = "Please set date value for field [" + colInfo.name + "] in month/day/year format.";

        if( errMsg != null )
        {
            alert( errMsg );
            input.focus();
            input.select();
            return;
        }

        row[ colInfo.name ] = value;
    }

    adapter.addRecord( row, switchToGrid, null );
}

function onCancelAddRecordClick()
{
    switchToGrid();
}

function getRandomValue( typeName )
{
    switch( typeName )
    {
        case "int": return parseInt(Math.random() * 10000);
        case "float": return Math.round( Math.random() * 1e6 ) / 100;
        case "date": return "\"06/11/2009\"";
        case "boolean": return Math.random() > 0.5;
        case "string": return "\"String Test Value\"";
    }
}

//	=============================	Creating new record and showing code functions
function addRecord()
{
    if( adapter.selectedTable == null )
    {
        alert("Select a table first.");
        return;
    }

    if( divNewRecord.style.display == "block" )
        return;

    var strHtml = "<form name='newRec' onsubmit='return false'><table class='newRec'>";
    var hiddenHtml = "";

    for( var i = 0; i < adapter.columnsInfo.length; ++i )
    {
        var colInfo = adapter.columnsInfo[ i ];

        if( !colInfo.isAutoIncremented )
        {
            strHtml += "<tr><td class='cb'>" + colInfo.name + "</td> <td><input name='__" + colInfo.name + "'";
            if( colInfo.isRequired )
                strHtml += " value='" + formatDefaultValue( ColumnInfo.defaultValues[ colInfo.type ] ) + "'";
            if( colInfo.type == "date" )
                strHtml += " onchange='if( v = parseDate( this.value ) ) this.value = getDateString( v );'";
            strHtml += ">";
            if( colInfo.isRequired )
                strHtml += " *";
            strHtml += "</td></tr>"
        }
        else
            hiddenHtml += "<input type='hidden' name='__" + colInfo.name + "' value='" + formatDefaultValue( ColumnInfo.defaultValues[ colInfo.type ] ) + "' />";
    }

    strHtml += "</table><br><button class='borderTwin margedRight ptr' onclick='onAddRecordClick()'>Add</button> <button class='borderTwin ptr' onclick='onCancelAddRecordClick()'>Cancel</button>"
        + hiddenHtml + "</form>";
    divNewRecordContent.innerHTML = strHtml;

    divNewRecordCaption.innerHTML = "New " + adapter.selectedTable;

    divGridBlock.style.display = divPager.style.display = divCode.style.display = "none";
    divNewRecord.style.display = "block";
}

function switchToGrid()
{
    divNewRecordContent.innerHTML = "";
    divNewRecord.style.display = divCode.style.display = "none";
    divGridBlock.style.display = divPager.style.display = "block";
}

function showCode()
{
    if( adapter.selectedTable == null )
    {
        alert("Select a table first.");
        return;
    }

    var tableVar = "";
    if(adapter.selectedTable.indexOf(".") == -1)
        tableVar = adapter.selectedTable;
    else
    {
        var arr = adapter.selectedTable.split(".");
        tableVar = arr[1];
    }

    tableVar = tableVar.charAt( 0 ).toLowerCase() + tableVar.substr( 1 );
    var pk, pkValue, field, fieldValue, setFields = "";

    for( var i = 0; i < adapter.columnsInfo.length; ++i )
    {
        var colInfo = adapter.columnsInfo[ i ];

        if( colInfo.isPK )
        {
            pk = colInfo.name;
            pkValue = getRandomValue( colInfo.type );
        }
        else if( field == null )
        {
            field = colInfo.name;
            fieldValue = getRandomValue( colInfo.type );
        }

        if( !colInfo.isAutoIncremented )
            setFields += tableVar + "." + colInfo.name + " = " + getRandomValue( colInfo.type ) + ";\n";
    }

    divCodeContent.innerHTML = codeText.replace( /%Table%/g, adapter.selectedTable ).replace( /%tableVar%/g, tableVar )
        .replace( /%pk%/g, pk ).replace( /%pkValue%/g, pkValue ).replace( /%field%/g, field ).replace( /%fieldValue%/g, fieldValue ).replace( /%setFields%/g, setFields );

    divGridBlock.style.display = divPager.style.display = divNewRecord.style.display = "none";
    divCode.style.display = "block";

    divCodeContent.scrollTop = 0;
}

function switchTable( tabName, div )
{
    if ( selectedTableDiv )
        selectedTableDiv.className = "";

    (selectedTableDiv = div).className = "selectedLi";

    adapter.selectTable( tabName, lstPageSize.value );
    switchToGrid();
}

function getDateString( value )
{
    if( !( value instanceof Date ) )
        return value;

    return ( value.getMonth() + 1 ) + "/" + value.getDate() + "/" + value.getFullYear();
}

function parseDate( value, treatAsUtc )
{
    if( value == null )
        return null;

    var date;

    if( value instanceof Date )
        date = value;
    else
    {
        var parts = value.toString().split( "/" );
        if( parts.length < 3 )
            return null;

        date = new Date( parseInt( parts[ 2 ] ), parseInt( parts[ 0 ] ) - 1, parseInt( parts[ 1 ] ) );
        if( isNaN( date.getTime() ) )
            return null;
    }
    return treatAsUtc? new Date( date.getTime() - date.getTimezoneOffset() * 60000 ) : date;
}

function page( pageNo )
{
    adapter.showPage( pageNo );
}

function gotoPage( form )
{
    var el = form.elements[ 0 ];
    var p = parseInt( el.value );
    if( !isNaN( p ) )
    {
        page( p );
        el.value = p;
        el.disabled = true;
    }
    else
        el.value = "";
    return false;
}

//	endof Creating new record and showing code functions