function ExtAdapter() {

}

ExtAdapter.prototype = {
  tableView: undefined,
  tableName: undefined,
  dataMapper: undefined,
  columnsInfo: undefined,
  record: undefined,
  gridContainer: undefined,
  reader: undefined,
  dataStore: undefined,
  pageSize: 10
};

ExtAdapter.prototype.selectTable = function (schemaName, tableName, container) {
  this.gridContainer = container;
  this.tableName = tableName;
  this.dataMapper = schemaName ? ActiveRecords[ schemaName ][ tableName ] : ActiveRecords[ tableName ];

  this.columnsInfo = this.dataMapper.createActiveRecordInstance().getColumnsInfo();
  this.reCreateTable();
};

ExtAdapter.prototype._getGridXType = function (type, isEditor) {
  var typesMap = '';
  if (isEditor)
    typesMap = { date: 'datefield', boolean: 'checkbox' };
  else
    typesMap = { date: 'datecolumn', boolean: '' };

  return typesMap[type] == undefined ? (isEditor ? 'textfield' : '') : typesMap[type];
};

ExtAdapter.prototype.reCreateTable = function (pageSize) {
  var self = this,
    columns = [],
    recordItems = [],
    emptyRecord = {};

  if (typeof pageSize !== 'undefined')
    this.pageSize = pageSize;

  for (var i = 0, len = this.columnsInfo.length; i < len; ++i) {
    var colInfo = this.columnsInfo[i];
    if (!colInfo.name) {
      continue;
    }

    var column = {
      header: colInfo.name,
      dataIndex: colInfo.name,
      flex: 1,
      xtype: this._getGridXType(colInfo.type, false)
    };
    if (!colInfo.isAutoIncremented) {
      column.editor = {
        allowBlank: !colInfo.isRequired,
        xtype: this._getGridXType(colInfo.type, true)
      }
    }

    columns.push(column);
    recordItems.push({name: colInfo.name, type: colInfo.type});
    emptyRecord[colInfo.name] = '';
  }

  var modelName = this.tableName + 'TableDataModel';

  var model = Ext.define(modelName, {
    extend: 'Ext.data.Model',
    fields: recordItems
  });
  this.data = [];
  for (var i = 0; i < 10; i++) {
    this.data.push(emptyRecord);
  }

  this.dataStore = Ext.create('Ext.data.Store', {
    // destroy the store if the grid is destroyed
    autoDestroy: true,
    autoLoad: true,
    model: modelName,
    pageSize: this.pageSize,
    proxy: Ext.create('Ext.data.proxy.Server.Weborb', {
      dataMapper: this.dataMapper,
      model: modelName,
      store: this.dataStore,
      listeners: {
        onwdmupdate: function () {
          self.syncHandler.apply(self, arguments)
        },
        onwdmdelete: function () {
          self.syncHandler.apply(self, arguments)
        },
        onwdmcreated: function () {
          self.syncHandler.apply(self, arguments)
        }
      }
    })
  });

  var rowEditing = Ext.create('Ext.grid.plugin.RowEditing', {
    clicksToMoveEditor: 1,
    autoCancel: false,
    listeners: {
      edit: function (editor, e, eOpts) {
        self.tableView.down('#applyChanges').setDisabled(false);
      }
    }
  });

  this.tableView = Ext.create('Ext.grid.Panel', {
    title: this.tableName,
    columns: columns,
    store: this.dataStore,
    frame: true,
    tbar: [
      {
        text: 'Add ' + this.tableName,
        iconCls: 'icon-add',
        handler: function () {
          rowEditing.cancelEdit();

          // Create a record instance through the ModelManager
          var r = Ext.ModelManager.create(emptyRecord, modelName);
          self.dataStore.insert(0, r);
          rowEditing.startEdit(0, 0);
        }
      },
      {
        itemId: 'removeEntity',
        text: 'Remove ' + this.tableName,
        iconCls: 'icon-remove',
        handler: function () {
          Ext.MessageBox.confirm("Confirmation", "Are you sure you want to delete record?", function (btn) {
            if (btn === 'yes') {
              var sm = self.tableView.getSelectionModel();
              rowEditing.cancelEdit();
              self.dataStore.remove(sm.getSelection());
              sm.select(0);
              self.tableView.down('#applyChanges').setDisabled(false);
            }
          })
        },
        disabled: true
      },
      {
        text: 'Apply Changes',
        itemId: 'applyChanges',
        iconCls: 'icon-save',
        handler: function () {
          self.dataStore.save();
          self.tableView.down('#applyChanges').setDisabled(true);
        },
        scope: this,
        disabled: true
      },
      {
        xtype: 'combobox',
        fieldLabel: 'Page Size',
        store: [10, 20, 30, 40, 50],
        queryMode: 'local',
        value: this.pageSize,
        listeners: {
          change: function (obj, newValue, oldValue, eOpts) {
            self.reCreateTable(newValue);
          }
        },
        width: 104,
        labelWidth: 54,
        style: {
          float: 'left',
          left: 'auto'
        }

      }
    ],
    plugins: [rowEditing],
    bbar: Ext.create('Ext.PagingToolbar', {
      store: this.dataStore,
      displayInfo: true,
      displayMsg: 'Displaying topics {0} - {1} of {2}',
      emptyMsg: "No topics to display"
    }),
    height: (typeof window.innerHeight != 'undefined' ? window.innerHeight : document.body.offsetHeight) - 51,
    listeners: {
      selectionchange: function (view, records) {
        self.tableView.down('#removeEntity').setDisabled(!records.length);
      },
      beforeedit: function (e) {
        var columns = this.columns,
            columnsInfo = self.columnsInfo;
        for (var i = 0, len = columns.length; i < len; ++i) {
          var editor = columns[i].getEditor();
          editor && editor.setDisabled(!e.record.phantom && columnsInfo[i].isPK);
        }
      }
    }
  });
  this.gridContainer.removeAll();
  this.gridContainer.add(this.tableView);
};

ExtAdapter.prototype.syncHandler = function (rec) {
  this.dataStore.load();
};

ExtAdapter.prototype.syncCreateHandler = function (activeRecord) {
  var gridRecord = Ext.ModelManager.create(activeRecord.record, this.tableName + 'TableDataModel');
  gridRecord.phantom = false;
  this.dataStore.insert(0, gridRecord);
  this.tableView.reconfigure(this.dataStore);
};

