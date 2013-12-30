Ext.define('Ext.data.proxy.Server.Weborb', {
      extend: 'Ext.data.proxy.Proxy',
      alias : 'proxy.weborb',
      alternateClassName: 'Ext.data.ServerProxyWeborb',

      model: undefined,
      dataMapper: undefined,
      synchronized: true,

      _activeCollection: undefined,
      _columnsInfo: undefined,
      _loaded: false,
      _loading: false,
      _defaultPageSize: 10,
      _processedObjectsInQueue: {},

      INDEX_FIELD_NAME: "wdmURI",
      UPDATE_EVENT: "onwdmupdate",
      DELETE_EVENT: "onwdmdelete",
      CREATE_EVENT: "onwdmcreated",


      constructor: function(config)
      {
        var me = this;

        config = config || {};

        if (typeof config['dataMapper'] == 'undefined')
          throw "dataMapper is required config parameter";

        me.addEvents(
            this.UPDATE_EVENT,
            this.CREATE_EVENT,
            this.DELETE_EVENT
        );

        me.callParent([config]);

        me.extraParams = config.extraParams || {};

        me.api = config.api || {};

        //backwards compatibility, will be deprecated in 5.0
        me.nocache = me.noCache;

        me._columnsInfo = me.dataMapper.createActiveRecordInstance().getColumnsInfo();

        if (me.synchronized)
        {
          DataServiceClient.Instance.subscribe(__clientId__);
          DataServiceClient.Instance.addEventListener(ActiveRecordEvent.UPDATE, function(){me._onSyncUpdate.call(me, arguments)}, me);
          DataServiceClient.Instance.addEventListener(ActiveRecordEvent.CREATE, function(){me._onSyncCreate.call(me, arguments)}, me);
          DataServiceClient.Instance.addEventListener(ActiveRecordEvent.DELETE, function(){me._onSyncDelete.call(me, arguments)}, me);
        }
      },

      _onSyncUpdate: function(event)
      {
        this.fireEvent(this.UPDATE_EVENT, event);
      },
      
      _onSyncDelete: function(event)
      {
        this.fireEvent(this.DELETE_EVENT, event);
      },

      _onSyncCreate: function(event)
      {
        this._activeCollection._$arr.length++;
        this.fireEvent(this.CREATE_EVENT, event);
      },

      _onLoadSync: function(event)
      {
        var data = event.target instanceof QueryResult ? event.target.Result : event.target;
        if (data instanceof Array)
        {
          data = Collection.fromObject(data);
        }
        this._activeCollection = data;
        this._loaded = true;
      },

      _getRecord: function(data)
      {
        var rec;
        if(IdentityMap.global.exists(data[this.INDEX_FIELD_NAME]))
          rec = IdentityMap.global.extract(data[this.INDEX_FIELD_NAME]);
        else
          rec = this.dataMapper.createActiveRecordInstance();

        for (var i = 0; i < this._columnsInfo.length; ++i)
        {
          var colInfo = this._columnsInfo[ i ];
          var value = data[ colInfo.name ];

          rec[ colInfo.name ] = colInfo.type == "date" ? parseDate(value, true) : value;
        }
        return rec;
      },

      _onError: function(event)
      {
        Ext.Error.raise({
              msg: "Proxy error",
              data: event
            });
      },

      create: function(operation, responseHandler, store)
      {
        this._createOrUpdate(operation, responseHandler, store);
        this._loaded = false;
        this._loading = false;
		
      },

      read: function(operation, responseHandler, store)
      {
        var self = this;
        if (!this._loaded)
        {
          if (!this._loading)
          {
            this._loading = true;
            this._activeCollection = this.dataMapper.findAll({ PageSize: parseInt(operation.limit == undefined
                    ? this._defaultPageSize
                    : operation.limit) });
            this._activeCollection.addEventListener(DynamicLoadEvent.LOADED, this._onLoadSync, this);
            this._activeCollection.addEventListener(CollectionEvent.COLLECTION_CHANGE, this._onLoadSync, this);
            this._activeCollection.addEventListener(ErrorEvent.ERROR, this._onError, this);

            this._activeCollection.addEventListener(ActiveRecordEvent.CREATE, this._onSyncCreate, this);
            this._activeCollection.addEventListener(ActiveRecordEvent.DELETE, this._onSyncDelete, this);
          }
          setTimeout(function()
          {
            self.read(operation, responseHandler, store);
          }, 10);
          return;
        }

        var totalSize = this._activeCollection.getLength();
        var startNdx = operation.start;
        var endNdx = this.currentPage == 0
            ?
            totalSize > this._activeCollection._pageSize
                ? this._activeCollection._pageSize
                : totalSize
            : Math.min(totalSize, startNdx + this._activeCollection._pageSize);

        var records = [];

        for (var i = startNdx; i < endNdx; i++)
        {
          var record = this._activeCollection.get(i);
          if(typeof record == "undefined")
            continue;
          if (!record._isLoaded)
          {
            this._loaded = false;
            setTimeout(function()
            {
              self.read(operation, responseHandler, store);
            }, 5);
            return;
          }
          if(record.getIsDeleted())
          {
            if(endNdx < totalSize)
              endNdx++;
            continue;
          }
          record[this.INDEX_FIELD_NAME] = record.getURI();
          var modelRecord = Ext.ModelManager.create(record, this.model);
          modelRecord.phantom = false;
          records.push(modelRecord);
        }

        var recordSet = Ext.create('Ext.data.ResultSet', {
              loaded: true,
              count: endNdx - startNdx,
              success: true,
              total: this._activeCollection.getLength(),
              records: records
            });
        operation.resultSet = recordSet;

        operation.setSuccessful();
        operation.setCompleted();

        responseHandler.call(store, operation);

      },

      update: function(operation, responseHandler, store)
      {
        this._createOrUpdate(operation, responseHandler, store);
      },

      destroy: function(operation, responseHandler, store)
      {
        var self = this;
        var timestamp = new Date().getTime();
        this._processedObjectsInQueue[timestamp] = 0;
        for (var i = 0; i < operation.records.length; i++)
        {
          var record = this._getRecord(operation.records[i].data);
          var responder = this._getResponder(operation, i, store, responseHandler, timestamp);
          this._processedObjectsInQueue[timestamp]++;
          record.remove(responder);
        }
      },

      _createOrUpdate: function(operation, responseHandler, store)
      {
        var self = this;
        var timestamp = new Date().getTime();
        this._processedObjectsInQueue[timestamp] = 0;
        for (var i = 0; i < operation.records.length; i++)
        {
          var record = this._getRecord(operation.records[i].data);
          var responder = this._getResponder(operation, i, store, responseHandler, timestamp);
          this._processedObjectsInQueue[timestamp]++;
          record.save(responder);
        }
      },

      _getResponder: function(operation, index, store, responseHandler, timestamp)
      {
        var self = this;
        var param = {
          recordIndex: index,
          handleComplete: function()
          {
            self._processedObjectsInQueue[timestamp]--;
            if (self._processedObjectsInQueue[timestamp] < 1)
            {
              responseHandler.call(store, operation);
              delete self._processedObjectsInQueue[timestamp];
            }
          }}
        var responder = new Async(
            function(rec)
            {
              operation.records[this.recordIndex].commit();
              this.handleComplete();
            },
            function(err)
            {
              self._onError(err);
              this.handleComplete.call(self);
            },
            param
        );
        return responder;
      },

      onDestroy: function()
      {
        Ext.destroy(this.reader, this.writer);
      }
    });

function parseDate(value, treatAsUtc)
{
  if (value == null)
    return null;

  var date;

  if (value instanceof Date)
    date = value;
  else
  {
    var parts = value.toString().split("/");
    if (parts.length < 3)
      return null;

    date = new Date(parseInt(parts[ 2 ]), parseInt(parts[ 0 ]) - 1, parseInt(parts[ 1 ]));
    if (isNaN(date.getTime()))
      return null;
  }
  return treatAsUtc ? new Date(date.getTime() - date.getTimezoneOffset() * 60000) : date;
}