/**
* ExtJsFileManagerPlugin.js
*
* Copyright 2011, Rahul Singla (http://www.rahulsingla.com)
* Released under Microsoft Public License (http://www.opensource.org/licenses/ms-pl.html).
*/

(function() {
    tinymce.PluginManager.requireLangPack('extjsfilemanager');

    var BrowserShowMode = {};
    BrowserShowMode.SelectImage = 0;
    BrowserShowMode.SelectMedia = 1;
    BrowserShowMode.UploadFiles = 2;

    tinymce.create('tinymce.plugins.ExtJsFileManagerPlugin', {
        init: function(ed, url) {
            var handlerUrl = ed.getParam('extjsfilemanager_handlerurl');
            if (!handlerUrl) {
                throw 'extjsfilemanager_handlerurl should be specified while using the ExtJsFileManagerPlugin.';
            }
            var extraParams = ed.getParam('extjsfilemanager_extraparams');
            if (!extraParams) extraParams = {};

            this._createBrowserUI(ed, handlerUrl, extraParams);

            var mediaPlugin = ed.plugins['media'];

            ed.addCommand('ExtJsFileManagerSelectImage', function() {
                ed.browserUi.show(BrowserShowMode.SelectImage);
            });
            //Add command and Button for Selecting Media only if the Media plugin for TinyMCE is available for this editor.
            if (mediaPlugin) {
                ed.addCommand('ExtJsFileManagerSelectMedia', function() {
                    ed.browserUi.show(BrowserShowMode.SelectMedia);
                });
            }
            ed.addCommand('ExtJsFileManagerUpload', function() {
                ed.browserUi.show(BrowserShowMode.UploadFiles);
            });

            ed.addButton('extjsfilemanagerselectimage', {
                title: 'extjsfilemanager.selectimage_desc',
                cmd: 'ExtJsFileManagerSelectImage',
                image: url + '/img/browseimage.png'
            });
            if (mediaPlugin) {
                ed.addButton('extjsfilemanagerselectmedia', {
                    title: 'extjsfilemanager.selectmedia_desc',
                    cmd: 'ExtJsFileManagerSelectMedia',
                    image: url + '/img/browsemedia.png'
                });
            }
            ed.addButton('extjsfilemanagerupload', {
                title: 'extjsfilemanager.upload_desc',
                cmd: 'ExtJsFileManagerUpload',
                image: url + '/img/upload.png'
            });
            ed.addButton('extjsfilemanagerinfo', {
                title: 'extjsfilemanager.info_desc',
                onclick: function() {
                    var t = ed.windowManager;

                    //WindowManager.alert encodes the content, so use raw open method.
                    var w = t.open({
                        title: 'ExtJs File Manager',
                        type: 'alert',
                        button_func: function(s) {
                            t.close(null, w.id);
                        },
                        content: 'ExtJs File Manager<br />by Rahul Singla<br /><a href="http://www.rahulsingla.com" target="_blank" style="width: auto; height: auto; top: auto;">http://www.rahulsingla.com</a>',
                        inline: 1,
                        width: 400,
                        height: 130
                    });
                },
                image: url + '/img/info.png'
            });
        },

        getInfo: function() {
            return {
                longname: 'ExtJs based File Manager plugin for TinyMCE',
                author: 'Rahul Singla',
                authorurl: 'http://www.rahulsingla.com',
                infourl: 'http://www.rahulsingla.com/blogs',
                version: "1.0"
            };
        },

        //Private
        _createBrowserUI: function(ed, handlerUrl, extraParams) {
            var ns = ed.browserUi = {};
            ns.handlerUrl = handlerUrl;
            ns.extraParams = extraParams;

            //Method for server interaction.
            Ext.apply(ed.browserUi, {
                show: function(mode) {
                    var ns = this;
                    ns.lastShowMode = mode;

                    switch (mode) {
                        case BrowserShowMode.SelectImage:
                        case BrowserShowMode.SelectMedia:
                            ns.btnSelect.show();
                            ns.btnUploadFiles.hide();
                            break;

                        case BrowserShowMode.UploadFiles:
                            ns.btnSelect.hide();
                            ns.btnUploadFiles.show();
                            break;

                        default:
                            throw 'Invalid mode for showing ExtJs FileManager Window.';
                    }

                    ns.winBrowser.show();
                    var root = ns.treeBrowser.getRootNode();
                    if (!root.data.loaded) {
                        ns.loadFolders(ns, root);
                    }
                },

                loadFolders: function(ns, folder) {
                    My.makeWebServiceCall(ns.handlerUrl,
                       ns.getParams(ns, { op: 'getFolders', path: folder.data.path }),
                        function(isValid, result) {
                            if (!isValid) {
                                Ext.Msg.alert('Error', result);
                            } else {
                                folder.appendChild(result);
                                folder.data.loaded = true;
                                folder.expand();
                            }
                        }, { jsonResult: true });
                },

                loadFiles: function(ns, folder) {
                    Ext.getBody().mask('Please wait...');

                    My.makeWebServiceCall(ns.handlerUrl,
                        ns.getParams(ns, { op: 'getFiles', path: folder.data.path }),
                        function(isValid, result) {
                            Ext.getBody().unmask();
                            if (!isValid) {
                                Ext.Msg.alert('Error', result);
                            } else {
                                ns.storeFiles.loadData(result, false);
                            }
                        }, { jsonResult: true });
                },

                createDirectory: function(ns) {
                    Ext.Msg.prompt('Enter name', 'Enter Directory name', function(btn, text) {

                        if (btn != 'ok') return;

                        var folder = ns.treeMenu.lastRecord;
                        Ext.getBody().mask('Please wait...');
                        My.makeWebServiceCall(ns.handlerUrl,
                                    ns.getParams(ns, { op: 'createFolder', path: folder.data.path, name: text }),
                                    function(isValid, result) {
                                        Ext.getBody().unmask();
                                        if (!isValid) {
                                            Ext.Msg.alert('Error', result);
                                        } else {
                                            ns.loadFolders(ns, folder);
                                        }
                                    }, { jsonResult: true });
                    });
                },

                getParams: function(ns, params) {
                    return (Ext.applyIf(params, ns.extraParams));
                }
            });

            Ext.onReady(function() {
                ns.treeMenu = new Ext.menu.Menu({
                    items: [
                        { text: 'Create Directory', ns: ns, itemId: 'mnuCreateDir', handler: function() {
                            ns.createDirectory(ns);
                        }
                        },

                        { text: 'Upload File(s)', ns: ns, itemId: 'mnuUploadFiles', handler: function() {
                            ns.winUpload.show();
                        }
                        }
                    ]
                });

                ns.winBrowser = new Ext.Window({
                    width: 600,
                    height: 500,
                    modal: true,
                    title: 'File Browser',
                    closeAction: 'hide',

                    layout: 'border',

                    buttons: [
                        { text: 'Upload File(s)', ns: ns, itemId: 'btnUploadFiles', handler: function() { ns.winUpload.show(); } },
                        { text: 'Select Image', ns: ns, itemId: 'btnSelect', handler: function() {
                            var selections = ns.grdFiles.selModel.getSelection();
                            if (selections.length == 0) {
                                Ext.Msg.alert('No file selected', 'Please select a file.');
                                return;
                            }
                            ns.winBrowser.hide();
                            var virtualPath = selections[0].data.virtualPath;

                            var commandName = null;
                            if (ns.lastShowMode == BrowserShowMode.SelectMedia) {
                                commandName = 'mceMedia';
                            } else {
                                commandName = 'mceImage';
                                var advImagePlugin = ed.plugins['advimage'];
                                if (advImagePlugin) {
                                    commandName = 'mceAdvImage';
                                }
                            }

                            ed.execCommand(commandName, true, virtualPath);

                            var fn = function(wm, wnd) {
                                ed.windowManager.onOpen.remove(fn);

                                Ext.fly(wnd).on('load', function(e, el, options) {
                                    wnd.document.getElementById('src').value = virtualPath;

                                    switch (commandName) {
                                        case 'mceMedia':
                                            //Show Preview for Advanced Image.
                                            wnd.Media.data.params.src = virtualPath;
                                            wnd.Media.preview();
                                            break;

                                        case 'mceAdvImage':
                                            //Show Preview for Advanced Image.
                                            wnd.ImageDialog.showPreviewImage(virtualPath, 1);
                                            break;
                                    }
                                });
                            };
                            ed.windowManager.onOpen.add(fn);

                            //Ext.Msg.prompt('Enter title', 'Enter title for this file (optional)',
                            //    function(btn, text) {
                            //        if (btn != 'ok') return;

                            //        var img = Ext.String.format('<img src="{0}" title="{1}" />', virtualPath, text);
                            //        ed.execCommand('mceReplaceContent', false, img);
                            //    });
                        }
                        },
                        { text: 'Close', handler: function() { ns.winBrowser.hide(); } }
                    ],

                    items: [
                        { xtype: 'treepanel',
                            ns: ns,
                            itemId: 'treeBrowser',
                            region: 'west',
                            width: 200,
                            split: true,
                            autoScroll: true,
                            root: {
                                text: 'Upload Directory',
                                path: ''
                            }
                        },
                        { xtype: 'grid',
                            ns: ns,
                            itemId: 'grdFiles',
                            region: 'center',
                            columns: [
                                { header: 'File Name', dataIndex: 'text', width: 200 },
                                { header: 'File Path', dataIndex: 'virtualPath', width: 200 }
                            ],
                            store: new Ext.data.Store({
                                ns: ns,
                                itemId: 'storeFiles',
                                autoLoad: false,
                                fields: [
                                    { name: 'text' },
                                    { name: 'virtualPath' }
                                ],
                                proxy: {
                                    type: 'memory',
                                    reader: {
                                        type: 'json',
                                        root: 'data'
                                    }
                                },
                                data: []
                            })
                        },
                        { xtype: 'window',
                            ns: ns,
                            itemId: 'winUpload',
                            title: 'Upload Files',
                            constrain: true,
                            width: 400,
                            height: 200,
                            modal: true,
                            closeAction: 'hide',
                            layout: 'fit',
                            listeners: {
                                show: function() {
                                    Ext.each(ns.pnlFiles.items.items, function(item) {
                                        item.inputEl.dom.value = '';
                                    });
                                }
                            },
                            items: [
                                { xtype: 'form',
                                    ns: ns,
                                    itemId: 'pnlFiles',
                                    bodyPadding: 5,
                                    defaults: {
                                        anchor: '95%'
                                    },
                                    items: [
                                        { xtype: 'filefield', fieldLabel: 'Select File', name: 'file1' },
                                        { xtype: 'filefield', fieldLabel: 'Select File', name: 'file2' },
                                        { xtype: 'filefield', fieldLabel: 'Select File', name: 'file3' },
                                        { xtype: 'filefield', fieldLabel: 'Select File', name: 'file4' }
                                    ]
                                }
                            ],
                            buttons: [
                                { text: 'Upload', handler: function() {
                                    var folder = ns.treeMenu.lastRecord;

                                    ns.pnlFiles.getForm().submit({
                                        clientValidation: true,
                                        url: ns.handlerUrl,
                                        waitMsg: 'Uploading files...',
                                        params: ns.getParams(ns, {
                                            op: 'uploadFiles',
                                            path: folder.data.path
                                        }),
                                        success: function(form, action) {
                                            if (action.result.errorFiles) {
                                                Ext.Msg.alert('Information', action.result.errorFiles);
                                            }

                                            ns.winUpload.hide();
                                            ns.loadFiles(ns, folder);
                                        },
                                        failure: function(form, action) {
                                            switch (action.failureType) {
                                                case Ext.form.action.Action.CLIENT_INVALID:
                                                    Ext.Msg.alert('Failure', 'Form fields may not be submitted with invalid values');
                                                    break;
                                                case Ext.form.action.Action.CONNECT_FAILURE:
                                                    Ext.Msg.alert('Failure', 'Ajax communication failed');
                                                    break;
                                                case Ext.form.action.Action.SERVER_INVALID:
                                                    Ext.Msg.alert('Failure', action.result.msg);
                                            }
                                        }
                                    });
                                }
                                },
                                { text: 'Cancel', handler: function() { ns.winUpload.hide(); } }
                            ]
                        }
                    ]
                });

                ns.treeBrowser.addManagedListener(ns.treeBrowser, 'itemcontextmenu', function(view, record, item, index, e) {
                    if (ns.lastShowMode == BrowserShowMode.UploadFiles) {
                        ns.treeMenu.lastRecord = record;

                        ns.treeMenu.showBy(item);
                        e.stopEvent();
                    }
                });

                ns.treeBrowser.addManagedListener(ns.treeBrowser, 'beforeload', function(store, operation) {
                    var node = operation.node;
                    if (!node.data.loaded) {
                        ns.loadFolders(ns, node);
                    }
                });

                ns.treeBrowser.addManagedListener(ns.treeBrowser, 'itemclick', function(view, record, item, index, e) {
                    ns.treeMenu.lastRecord = record;

                    ns.loadFiles(ns, record);

                    if (!record.data.loaded) {
                        ns.loadFolders(ns, record);
                    }
                });
            });
        }
    });

    // Register plugin
    tinymce.PluginManager.add('extjsfilemanager', tinymce.plugins.ExtJsFileManagerPlugin);
})();