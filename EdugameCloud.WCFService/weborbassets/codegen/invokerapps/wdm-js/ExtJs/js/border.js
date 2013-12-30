
Ext.require(['*']);
Ext.onReady(function() {
    var cw;

    Ext.create('Ext.Viewport', {
        layout: {
            type: 'border',
            padding: 5
        },
        defaults: {
            split: true
        },
        items: [{
            region: 'north',
            collapsible: true,
            title: 'North',
            split: true,
            height: 100,
            minHeight: 60,
            html: 'north'
        },{
            region: 'west',
            collapsible: true,
            title: 'Starts at width 30%',
            split: true,
            width: '30%',
            minWidth: 100,
            minHeight: 140,
            html: 'west<br>I am floatable'
        },{
            region: 'center',
            layout: 'border',
            xtype: 'panel',
            border: false,
            items: [{
                region: 'center',
                title: 'Center',
                minHeight: 80,
                id: "mainW" ,
                items: [cw = Ext.create('Ext.Window', {
                    xtype: 'window',
                    closable: false,
                    minimizable: true,
                    title: 'Constrained Window',
                    height: 200,
                    width: 400,
                    constrain: true,
                    itemId: 'center-window',
                    minimize: function() {
                        this.floatParent.down('button#toggleCw').toggle();
                        alert("asd");
                    },
                    beforerender: function(container, e){
                           alert(container.html);
                        },
                     items: [{
                         xtype: "button",
                         text     : 'Button',
                         listeners: {
                             click: function() {
                                 // this == the button, as we are in the local scope
                                 this.setText('I was clicked!');
                                 loadGrid();
                             },
                             mouseover: function() {
                                 // set a new config which says we moused over, if not already set
                                 if (!this.mousedOver) {
                                     this.mousedOver = true;
                                     alert('You moused over a button!\n\nI wont do this again.');
                                 }
                             }
                         }
                     }
                     ]

                })],
                dockedItems: [{
                    xtype: 'toolbar',
                    dock: 'bottom',
                    items: ['Text followed by a spacer',
                        ' ', {
                            itemId: 'toggleCw',
                            text: 'Constrained Window',
                            enableToggle: true,
                            toggleHandler: function() {
                            cw.setVisible(!cw.isVisible());
                        }
                    }]
                }]
            },{
                region: 'south',
                height: 100,
                split: true,
                collapsible: true,
                title: 'Splitter above me',
                minHeight: 60,
                html: 'center south'
            }]
        },{
            region: 'east',
            collapsible: true,
            floatable: true,
            split: true,
            width: 200,
            minWidth: 120,
            minHeight: 140,
            title: 'East',
            layout: {
                type: 'vbox',
                padding: 5,
                align: 'stretch'
            },
            items: [{
                xtype: 'textfield',
                labelWidth: 70,
                fieldLabel: 'Text field'
            }, {
                xtype: 'component',
                html: 'I am floatable'
            }]
        },{
            region: 'south',
            collapsible: true,
            split: true,
            height: 200,
            minHeight: 120,
            title: 'South',
            layout: {
                type: 'border',
                padding: 5
            },
            items: [{
                title: 'South Central',
                region: 'center',
                minWidth: 80,
                html: 'South Central'
            }, {
                title: 'South Eastern',
                region: 'east',
                flex: 1,
                minWidth: 80,
                html: 'South Eastern',
                split: true,
                collapsible: true
            }, {
                title: 'South Western',
                region: 'west',
                flex: 1,
                minWidth: 80,
                html: 'South Western<br>I collapse to nothing',
                split: true,
                collapsible: true,
                collapseMode: 'mini'
            }]
        }]
    });

});
function loadGrid() {
    Ext.create('Ext.data.Store', {
                storeId:'simpsonsStore',
                fields:['name', 'email', 'phone'],
                data:{'items':[
                    { 'name': 'Lisa',  "email":"lisa@simpsons.com",  "phone":"555-111-1224"  },
                    { 'name': 'Bart',  "email":"bart@simpsons.com",  "phone":"555-222-1234" },
                    { 'name': 'Homer', "email":"home@simpsons.com",  "phone":"555-222-1244"  },
                    { 'name': 'Marge', "email":"marge@simpsons.com", "phone":"555-222-1254"  }
                ]},
                proxy: {
                    type: 'memory',
                    reader: {
                        type: 'json',
                        root: 'items'
                    }
                }
            });

    Ext.create('Ext.grid.Panel', {
                title: 'Simpsons',
                store: Ext.data.StoreManager.lookup('simpsonsStore'),
                columns: [
                    { header: 'Name',  dataIndex: 'name' },
                    { header: 'Email', dataIndex: 'email', flex: 1 },
                    { header: 'Phone', dataIndex: 'phone' }
                ],
                height: 200,
                width: 400,
                renderTo: "mainW"
            });
}