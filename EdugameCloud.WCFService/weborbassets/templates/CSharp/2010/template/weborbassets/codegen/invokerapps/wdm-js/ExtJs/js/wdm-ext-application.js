Ext.require(['*']);

Ext.onReady(function () {
  var schemasButtons = {};
  var tableContainer = Ext.create("Ext.container.Container", {
    xtype: 'container',
    layout:'fit',
    region: 'center'
  });

  Ext.define('WdmTable', {
    extend: 'Ext.data.Model',
    fields: [
      {name: 'schemaName', type: 'string'},
      {name: 'tableName', type: 'string'}
    ]
  });

  var tableStore = Ext.create('Ext.data.Store', {
    model: 'WdmTable'
  });

  var linksContainer = Ext.create('Ext.view.View', {
    store: tableStore,
    autoScroll: true,
    layout:'fit',
    itemSelector: 'div.side-list-item',
    tpl: Ext.create('Ext.XTemplate',
      '<tpl for=".">' +
      '<div class="side-list-item">{tableName}</div>' +
      '</tpl>'
    ),
    selModel: {
      mode: 'SINGLE',
      listeners: {
        selectionchange: function (sender, selection) {
          if (selection.length) {
            var record = selection[0];
            extAdapter.selectTable(record.get('schemaName'), record.get('tableName'), tableContainer);
          }
        }
      }
    }
  });

  var schemaContainer = Ext.create("Ext.toolbar.Toolbar", {});
  Ext.create('Ext.container.Viewport', {
    layout: {
      type: 'border',
      padding: 10
    },
    defaults: {
      split: true
    },
    items: [
      tableContainer, {
        xtype: 'panel',
        width: 150,
        region: 'west',
        layout:'fit',
        items: [linksContainer]
      },
      {
        xtype: 'container',
        height: 26,
        region: 'north',
        items: [
          schemaContainer
        ]
      }
    ]
  });

  var extAdapter = new ExtAdapter();

  var tableFlag = true, schemaFlag = true;
  for (var ar in ActiveRecords) {
    if (ActiveRecords[ar] instanceof DataMapper) {
      addTable("", ar);
      if (tableFlag) {
        linksContainer.select(0);
        tableFlag = false;
      }
    } else {
      schemaContainer.add(schemasButtons[ar] = Ext.create("Ext.Button", {
        xtype: 'button',
        enableToggle: true,
        text: ar,
        toggleGroup: 'schemas',
        pressed: schemaFlag,
        handler: function (o) {
          switchSchema(o.text);
        }
      }));

      if (schemaFlag) {
        var tables = ActiveRecords[ar];
        for (var arTable in tables) {
          if (tables[arTable] instanceof DataMapper) {
            addTable(ar, arTable);
          }

          if (tableFlag) {
            linksContainer.select(0);
            tableFlag = false;
          }
        }
      }
      schemaFlag = false;
    }
  }

  function addTable(schemaName, tableName) {
    tableStore.add({
      schemaName: schemaName,
      tableName: tableName
    });
  }


  function switchSchema(schemaName) {
    tableStore.removeAll();
    var i = 0,
        firstTable = true,
        tables;

    if (schemaContainer.items.length === 1) {
      tables = ActiveRecords;
      schemaName = '';
    }
    else
      tables = ActiveRecords[schemaName];

    for (var arTable in tables) {
      if (tables[arTable] instanceof DataMapper) {
        addTable(schemaName, arTable);
        if (firstTable) {
          linksContainer.select(0);
          firstTable = false;
        }
      }
    }
  }
});


