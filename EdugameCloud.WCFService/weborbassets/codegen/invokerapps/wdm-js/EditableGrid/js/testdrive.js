var divGrid, selectedTableDiv;

function init() {
  adapter = new EgAdapter("divGrid", "divPager", "divGridCaption");
  tableList = document.getElementById("tableList");

  divNewRecord = document.getElementById("divNewRecord");
  divNewRecordCaption = document.getElementById("divNewRecordCaption");
  divNewRecordContent = document.getElementById("divNewRecordContent");

  divCode = document.getElementById("divCode");
  divCodeContent = document.getElementById("divCodeContent");

  divGridBlock = document.getElementById("divGridBlock");
  divGrid = document.getElementById("divGridBlock");
  divPager = document.getElementById("divPager");
  codeText = divCodeContent.innerHTML;
  lstPageSize = document.getElementById("lstPageSize");

  var listHtml = "";
  for (var n in ActiveRecords) {
    if (ActiveRecords[n] instanceof DataMapper)
      listHtml += "<div onclick=\"switchTable('" + n + "', this);\">" + n + "</div>";
    else for (var i in ActiveRecords[n]) {
      if (ActiveRecords[n][i] instanceof DataMapper)
        listHtml += "<div onclick=\"switchTable('" + n + "." + i + "', this);\">" + n + "." + i + "</div>";
    }
  }

  tableList.innerHTML = listHtml;
  tableList.children[0].click();
}

//	=============================	Renderer classes
function USDateCellRenderer(config) {
  DateCellRenderer.call(this, config);
}
Utils.setInheritance(USDateCellRenderer, DateCellRenderer);

USDateCellRenderer.prototype.render = function (cell, value) {
  if (value instanceof Date)
    cell.innerHTML = getDateString(value);
  else {
    var date = this.editablegrid.checkDate(value);
    if (typeof date == "object")
      cell.innerHTML = getDateString(new Date(date.sortDate));
    else
      cell.innerHTML = value;
  }
};

function SaveCellRenderer() {
  CellRenderer.call(this);
}
Utils.setInheritance(SaveCellRenderer, CellRenderer);
SaveCellRenderer.prototype.render = function (cell, value) {
  CellRenderer.prototype.render.call(this, cell, value);
  cell.style.width = "1%";
  cell.innerHTML = "<button class=\"cellButton\" style=\"color:#ccc\" id=\"_save_" + cell.rowIndex + "\" onclick=\"this.disabled = true; this.style.color = '#ccc'; adapter.updateRecord(" + cell.rowIndex + ");\" disabled>Save</button>";
}

function DeleteCellRenderer() {
  CellRenderer.call(this);
}
Utils.setInheritance(DeleteCellRenderer, CellRenderer);
DeleteCellRenderer.prototype.render = function (cell, value) {
  CellRenderer.prototype.render.call(this, cell, value);
  cell.style.width = "1%";
  cell.innerHTML = "<button class=\"cellButton\" onclick=\"adapter.deleteRecord(" + cell.rowIndex + ");\">Delete</button>";
}
//	endof Renderer classes


//	=============================	Editor classes
function DateCellEditor(size, maxlen, config) {
  TextCellEditor.call(size, maxlen, config);
}
Utils.setInheritance(DateCellEditor, TextCellEditor);

DateCellEditor.prototype.editorValue = DateCellEditor.prototype.formatValue = function (value) {
  return value instanceof Date ? getDateString(value) : value.toString();
}
//	endof Editor classes


//	=============================	extend several EditableGrid functions
var extendEditableGridFunctions = function () {
  //	extend CellEditor.applyEditing
  var pf1 = CellEditor.prototype.applyEditing;
  CellEditor.prototype.applyEditing = function (element, newValue) {
    var isError = false;

    if (element && element.isEditing && !this.column.isValid(newValue)) {
      alert("Invalid value: " + newValue);
      this.cancelEditing(element);
      this._clearEditor(element);
      isError = true;
    }

    if (!isError)
      pf1.call(this, element, newValue);

    var isDirty = adapter.isRowDirty(element.rowIndex);
    ( btn = document.getElementById("_save_" + element.rowIndex) ).disabled = !isDirty;
    btn.style.color = isDirty ? "#2e6e9e" : "#ccc";

    if (isError)
      return false;
  }
}();
//	endof extend several EditableGrid functions



//	=============================	EgAdapter
function EgAdapter(gridId, pagerId, captionId) {
  Adapter.call(this, pagerId);

  this._gridElement = document.getElementById(gridId);
  this._captionElement = document.getElementById(captionId);
}

Utils.setInheritance(EgAdapter, Adapter);

EgAdapter.prototype.publish = function (dataObject) {
  this._activeCollection = null;

  var data = dataObject instanceof QueryResult ? dataObject.Result : dataObject;
  if (data instanceof Array) {
    data = Collection.fromObject(data);
    data._pageSize = this._pageSize;
  }

  this._activeCollection = data;
  this._buildPager();

  //	check if page exists
  var startNdx = this.shownPageNo == 0 ? 0 : (this.shownPageNo - 1 ) * this._activeCollection._pageSize;
  var endNdx = this.shownPageNo == 0 ? this._activeCollection.getLength() : Math.min(this._activeCollection.getLength(), startNdx + this._activeCollection._pageSize);
  var rec = data.get(startNdx);

  if (rec == null)
    this._activeCollection.removeItemAt(startNdx);

  if (this.shownPageNo > 1 && ( rec == null || endNdx - startNdx == 0 )) {
    this.showPage(this.shownPageNo - 1);
    return;
  }

  //	build grid
  this.grid = new EditableGrid(
      "DemoGrid",
      {
        enableSort: false, // true is the default, set it to false if you don't want sorting to be enabled
        editmode: "absolute", // change this to "fixed" to test out editorzone, and to "static" to get the old-school mode
        editorzoneid: "edition", // will be used only if editmode is set to "fixed"
        checkDate: function (strDate, strDatestyle) {
          strDatestyle = strDatestyle || "US";
          if (strDate instanceof Date)
            strDate = getDateString(strDate);

          return EditableGrid.prototype.checkDate.call(this, strDate, strDatestyle);
        }
      }
  );

  this._addColumn("_save_", "", "html", false);
  this._addColumn("_delete_", "", "html", false);
  this.grid.setCellRenderer("_save_", new SaveCellRenderer());
  this.grid.setCellRenderer("_delete_", new DeleteCellRenderer());

  for (var i = 0; i < this.columnsInfo.length; ++i)
    this._addColumn(this.columnsInfo[ i ]);

  this.grid.renderGrid(this._gridElement.id, "tabGrid", "tabGrid");

  //	fill grid
  for (var i = startNdx; i < endNdx; ++i)
    this.grid.addRow(i, this._getRowObject(this._activeCollection.get(i)), true);
}

EgAdapter.prototype._getRowObject = function (activeRecord) {
  var result = {};
  var cols = activeRecord.getColumnsInfo();
  cols.forEach(function (colInfo) {
    result[ colInfo.name ] = activeRecord[ colInfo.name ] != null ? activeRecord[ colInfo.name ] : "";
  });

  return result;
}

EgAdapter.prototype.restoreRow = function (rowIndex, grid) {
  //	if the grid was recreated during asynchronous call, do nothing
  if (this.grid != grid)
    return false;

  var rec = this._getCollectionRecord(rowIndex);

  for (var i = 0; i < this.columnsInfo.length; ++i) {
    var cell = rec[ this.columnsInfo[ i ].name ];
    this.grid.setValueAt(rowIndex, i + 2, cell != null ? cell : "");
  }

  return true;
}

EgAdapter.prototype._addColumn = function (colInfoOrName, label, dataType, editable) {
  var col = new Column();

  if (colInfoOrName instanceof ColumnInfo) {
    col.name = col.label = colInfoOrName.name;
    col.datatype = this._getDataType(colInfoOrName);
    col.editable = !colInfoOrName.isPK;
  }
  else {
    col.name = colInfoOrName;
    col.label = label;
    col.datatype = dataType;
    col.editable = editable;
  }

  this.grid.parseColumnType(col);

  // create suited cell renderer
  this.grid._createCellRenderer(col);
  this.grid._createHeaderRenderer(col);

  // create suited cell editor
  this.grid._createCellEditor(col);
  this.grid._createHeaderEditor(col);

  if (col.datatype == "date") {
    col.cellRenderer = new USDateCellRenderer();
    col.cellRenderer.editablegrid = this.grid;
    col.cellRenderer.column = col;

    col.cellEditor = new DateCellEditor();
    col.cellEditor.editablegrid = this.grid;
    col.cellEditor.column = col;
  }
  // add default cell validators based on the column type
  this.grid._addDefaultCellValidators(col);

  col.editablegrid = this.grid;
  this.grid.columns.push(col);
}

EgAdapter.prototype._getDataType = function (colInfo) {
  if (colInfo.type == "int")
    return "integer";
  else if (colInfo.type == "float")
    return "double";
  return colInfo.type;
}

EgAdapter.prototype._onUpdateRecordFailed = function (rowId, err, grid) {
  this.restoreRow(rowId, grid);
  alert("Error saving record: " + err.description);
}

EgAdapter.prototype.showLoading = function () {
  this._captionElement.innerHTML = this.selectedTable;
  this._gridElement.innerHTML = "Loading...";
}

EgAdapter.prototype.getRecord = function (rowId) {
  var rec = this._dataMapper.createActiveRecordInstance();

  for (var i = 0; i < this.columnsInfo.length; ++i) {
    var colInfo = this.columnsInfo[ i ];
    var value = this.grid.getValueAt(rowId, i + 2);

    rec[ colInfo.name ] = colInfo.type == "date" ? parseDate(value, true) : value;
  }
  return rec;
}

EgAdapter.prototype.isRowDirty = function (rowId) {
  var actRec = this._getCollectionRecord(rowId);

  for (var i = 0; i < this.columnsInfo.length; ++i) {
    var colInfo = this.columnsInfo[ i ];
    var value = this.grid.getValueAt(rowId, i + 2);
    var recValue = actRec[ colInfo.name ];

    if (colInfo.type == "date")
      value = getDateString(parseDate(value));
    if (recValue instanceof Date)
      recValue = getDateString(recValue);

    if (value != recValue)
      return true;
  }

  return false;
}

EgAdapter.prototype._getCollectionRecord = function (rowId) {
  var ndx = ( this.shownPageNo - 1 ) * parseInt(this._activeCollection._pageSize) + parseInt(rowId);
  return this._activeCollection.get(ndx);
}

EgAdapter.prototype.updateRecord = function (rowId) {
  var record = this.getRecord(rowId);
  var collectionRecord = this._getCollectionRecord(rowId);

  record.save(
      new Async(
          function () {
            Utils.copyValues(record, collectionRecord);
            this.restoreRow(rowId, this.grid);
          },
          function (err) {
            this._onUpdateRecordFailed(rowId, err, this.grid);
          },
          this
      )
  );

  return true;
}

EgAdapter.prototype.deleteRecord = function (rowId) {
  var record = this._getCollectionRecord(rowId);
  record.remove(true,
      new Async(
          function () {
            this.selectTable(this.selectedTable);
          },
          function (err) {
            alert("Error deleting record: " + err.description);
          },
          this
      )
  );
}

//	synchronization handlers
EgAdapter.prototype._onSyncDelete = function (ev) {
  this._needReload = true;
  this.showPage(this.shownPageNo);
};

EgAdapter.prototype._onSyncUpdate = function (ev) {
  if (this._dataMapper == null)
    return;
  var evRecordURL = ev.record.getURI();
  for (var i = 0; i < this.grid.getRowCount(); ++i) {
    if (this.getRecord(i).getURI() == evRecordURL) {
      if (document.getElementById("_save_" + i).disabled) {
        for (var j = 0; j < this.columnsInfo.length; ++j) {
          if (this.grid.getCell(i, j + 2).isEditing)
            return;
        }

        this.restoreRow(i, this.grid);
      }
    }
  }
};

EgAdapter.prototype._onSyncCreate = function (ev) {
  this._needReload = true;
  this.showPage(this.shownPageNo);
};


//	endof EgAdapter
