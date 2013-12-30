function init() {
  adapter = new JqAdapter("grid", "divPager");

  divNewRecord = document.getElementById("divNewRecord");
  divNewRecordCaption = document.getElementById("divNewRecordCaption");
  divNewRecordContent = document.getElementById("divNewRecordContent");

  divCode = document.getElementById("divCode");
  divCodeContent = document.getElementById("divCodeContent");

  divGridBlock = document.getElementById("divGrid");

  codeText = divCodeContent.innerHTML;
  divPager = document.getElementById("divPager");
  lstPageSize = document.getElementById("lstPageSize");

  var listHtml = '',
      tables = [],
      i = 0;
  for (var n in ActiveRecords) {
    if (ActiveRecords[n] instanceof DataMapper) {
      tables.push(n);
      listHtml += '<div class="table-item" data-item-num="' + i + '">' + n + '</div>';
      i++;
    } else {
      for (var j in ActiveRecords[n]) {
        if (ActiveRecords[n][j] instanceof DataMapper) {
          var str = n + '.' + j;
          tables.push(str);
          listHtml += '<div class="table-item" data-item-num="' + i + '">' + str + '</div>';
          i++;
        }
      }
    }
  }
  $('#tableList').html(listHtml);
  $('#tableList div.table-item').click(function () {

    $('#tableList div.table-item').removeClass('selectedLi');
    var num = $(this).addClass('selectedLi').attr('data-item-num');

    adapter.selectTable(tables[num], lstPageSize.value);
    switchToGrid();
  });
  $('#tableList div.table-item:first').click();

}

var selectedTableDiv = null;

//	=============================	JqAdapter
function JqAdapter(gridId, pagerId) {
  Adapter.call(this, pagerId);

  this.dateFormat = "m/d/Y";

  this.gridId = "#" + gridId;

  this.lastEditedId = null;
  this.editedRowsData = {};

}

Utils.setInheritance(JqAdapter, Adapter);

JqAdapter.prototype.publish = function (dataObject) {
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

  //	build columns model
  var colNames = [ '', '' ];
  var colModel = [
    { name: "_save_", index: "_save_", formatter: JqAdapter._formatterSaveButton, editable: false, sortable: false, width: 56, align: "center", fixed: true },
    { name: "_delete_", index: "_delete_", formatter: JqAdapter._formatterDeleteButton, editable: false, sortable: false, width: 71, align: "center", fixed: true }
  ];
  var gridData = [];

  for (var i = 0; i < this.columnsInfo.length; ++i) {
    var colInfo = this.columnsInfo[ i ];
    colNames.push(colInfo.name);

    var cm = {
      name: colInfo.name,
      index: colInfo.name,
      editable: !colInfo.isPK,
      align: colInfo.type == "int" || colInfo.type == "float" ? "right" : "left",
      width: 80,
      sortable: false,
      formatoptions: { }
    };

    this._setColModel(cm, colInfo);
    colModel.push(cm);
  }

  //	fill gridData array
  for (var i = startNdx; i < endNdx; ++i){
    gridData.push(this._getRowObject(this._activeCollection.get(i)));
  }
  this.maxId = endNdx;
  //	recreate grid
  var pThis = this;

  $(this.gridId).GridUnload();
  $(this.gridId).jqGrid({
    datatype: "local",
    data: gridData,
    colNames: colNames,
    colModel: colModel,
    rowNum: endNdx - startNdx,
    altRows: false,
    caption: this.selectedTable,

    cellEdit: false,
    editurl: "clientArray",
    cellsubmit: "clientArray",

    width: 780,
    height: 300,

    onSelectRow: this._createMethodPointer("editRecord"),
    onPaging: this._createMethodPointer("_onPaging")
  });
};

JqAdapter.prototype._getRowObject = function (activeRecord) {
  var result = {};
  var self = this;
  this.columnsInfo.forEach(function (colInfo) {
    result[ colInfo.name ] = self._formatValue(activeRecord[ colInfo.name ]);
  }, this);

  return result;
};

JqAdapter.prototype._formatValue = function (value) {
  if (value == null)
    return "&nbsp;"
  if (typeof( value ) == "string")
    return value.length == 0 ? "&nbsp;" : value.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
  return value;
};

JqAdapter.prototype._setColModel = function (colModel, colInfo) {
  colModel.edittype = "text";
  colModel.sorttype = colInfo.type;
  colModel.editrules =
  {
    required: colInfo.isRequired,
    integer: colInfo.type == "int",
    number: colInfo.type == "float",
    date: colInfo.type == "date",
    align: "left"
  };

  switch (colInfo.type) {
    case "string":
      colModel.sorttype = "text";
      break;
    case "int":
      colModel.formatter = "integer";
      colModel.align = "right";
      break;
    case "float":
      colModel.formatter = "number";
      colModel.sorttype = "number";
      colModel.align = "right";
      break;
    case "date":
      colModel.formatter = "date";
      colModel.datefmt = this.dateFormat;
      colModel.formatoptions.newformat = this.dateFormat;
      break;
    case "boolean":
      colModel.edittype = "checkbox";
      colModel.formatter = "checkbox";
      colModel.editoptions = { value: "true:false" };
      colModel.align = "center";
      break;
  }
};

JqAdapter._formatterSaveButton = function (cellvalue, options, rowObject) {
  return cellvalue || ( "<button id='_save_" + options.rowId + "' class='ui-state-default cellButton' disabled style='color:#ccc' onclick='adapter._onClickSaveButton( " + options.rowId + ", event );'>Save</button>" );
};

JqAdapter._formatterDeleteButton = function (cellvalue, options, rowObject) {
  return "<button class='ui-state-default cellButton' onclick='adapter.deleteRecord( " + options.rowId + ", event ); adapter._dontEdit = true;'>Delete</button>";
};

JqAdapter.prototype._onClickSaveButton = function (rowId, event) {
  if (event) {
    if (event.stopPropagation instanceof Function){
      event.stopPropagation();
    }
    event.cancelBubble = true;
  }

  if ($(this.gridId).saveRow(rowId))
    this.updateRecord(rowId);
};

JqAdapter.prototype._disableBtn = function (btn) {
  btn.disabled = true;
  btn.style.color = "#ccc";
};

JqAdapter.prototype._enableBtn = function (btn) {
  btn.disabled = false;
  btn.style.color = "#2e6e9e";
};

JqAdapter.prototype._createMethodPointer = function (funcName) {
  var pThis = this;
  return function () {
    pThis[funcName].apply(pThis, arguments);
  };
};

JqAdapter.prototype._onUpdateRecordFailed = function (rowId, err) {
  $(this.gridId).setRowData(rowId, this.editedRowsData[ rowId ]);
  delete this.editedRowsData[ rowId ];
  alert("Error saving record: " + err.description);
};

JqAdapter.prototype.showLoading = function () {
  this.publish(new ActiveCollection());
  try {
    $(this.gridId)[0].innerHTML = "Loading...";
  }
  catch (e) {
  }
};

JqAdapter.prototype._onAfterRestoreRow = function (rowId) {
  var self = this;
  setTimeout(function () {
    self._resetRow(rowId)
  }, 50);
};

JqAdapter.prototype.editRecord = function (rowId) {
  if (this._dontEdit) {
    this._dontEdit = false;
    return;
  }

  if (rowId && rowId != this.lastEditedId) {
    if (this.lastEditedId != null)
      $(this.gridId).restoreRow(this.lastEditedId, this._createMethodPointer("_onAfterRestoreRow"));

    this.lastEditedId = rowId;
  }

  if (this.editedRowsData[ rowId ] == null)
    this.editedRowsData[ rowId ] = $(this.gridId).getRowData(rowId);

  $(this.gridId).editRow(rowId, true, null,
      null, "clientArray", null,
      this._createMethodPointer("updateRecord"),
      null, this._createMethodPointer("_onAfterRestoreRow")
  );

  this._enableBtn(document.getElementById("_save_" + this.lastEditedId));
};

JqAdapter.prototype.getRecord = function (rowId) {
  var cells = $(this.gridId).getRowData(rowId);
  var rec = this._dataMapper.createActiveRecordInstance();

  for (var i = 0; i < this.columnsInfo.length; ++i) {
    var colInfo = this.columnsInfo[ i ];
    var value = cells[ colInfo.name ];

    rec[ colInfo.name ] = colInfo.type == "date" ? parseDate(value, true) : value;
  }
  return rec;
};

JqAdapter.prototype.updateRecord = function (rowId) {
  var record = this.getRecord(rowId);
  var trueRecord = IdentityMap.global.extract(record.getURI());

  record.save(
      new Async(
          function () {
            delete this.editedRowsData[ rowId ];
            Utils.copyValues(record, trueRecord);
            this._resetRow(rowId);
          },
          function (err) {
            this._onUpdateRecordFailed(rowId, err);
          },
          this
      )
  );

  this._disableBtn(document.getElementById("_save_" + rowId));
};

JqAdapter.prototype.deleteRecord = function (rowId, event) {
  if (event) {
    if (event.stopPropagation instanceof Function){
      event.stopPropagation();
    }
    event.cancelBubble = true;
  }

  $(this.gridId).restoreRow(rowId);

  var record = this.getRecord(rowId);
  record.remove(true,
      new Async(
          function () {
            $(this.gridId).delRowData(rowId);
            this.selectTable(this.selectedTable);
          },
          function (err) {
            alert("Error deleting record: " + err.description);
          },
          this
      )
  );
};

JqAdapter.prototype._resetRow = function (rowId) {
  this._disableBtn(document.getElementById("_save_" + rowId));

  var gridRec = this.getRecord(rowId);
  var curRec = IdentityMap.global.extract(gridRec.getURI());

  if (curRec == null || gridRec.equals(curRec))
    return;

  for (var i = 0; i < this.columnsInfo.length; ++i) {
    var value = curRec[ this.columnsInfo[ i ].name ];
    if (this.columnsInfo[ i ].type == "date" && value != null) {
      var mf = {};
      Utils.copyMembers(jQuery.jgrid.formatter.date, mf);
      mf.masks = { myFormat: this.dateFormat };
      value = $.fmatter.util.DateFormat(jQuery.jgrid.formatter.date, value, "myFormat", mf);
    }

    $(this.gridId).setCell(rowId, i + 2, value, null, null, true);
  }
};


//	synchronization handlers
JqAdapter.prototype._onSyncDelete = function (ev) {
  this._needReload = true;
  this.showPage(this.shownPageNo);
};

JqAdapter.prototype._onSyncUpdate = function (ev) {
  if (!this._dataMapper) {
    return;
  }

  var evRecordURI = ev.record.getURI();
  for (var i = 1, len = $(this.gridId).getDataIDs().length; i <= len; ++i) {
    if (this.getRecord(i).getURI() == evRecordURI) {
      if (document.getElementById("_save_" + i).disabled)
        this._resetRow(i);
    }
  }
};

JqAdapter.prototype._onSyncCreate = function(ev){
  this._needReload = true;
  this.showPage(this.shownPageNo);
};

//	endof JqAdapter
