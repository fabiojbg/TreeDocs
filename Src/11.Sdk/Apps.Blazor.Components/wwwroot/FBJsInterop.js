// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API

window.FBComponents = (function () {
    return {
        zTree: [],
        treeView: {
            create: function (instance, id, settings, nodes) {
                settings.edit = settings.edit || {};
                settings.edit.showRenameBtn = function showRenameBtn(treeId, treeNode) {
                    if (treeNode.id && treeNode.id != '0')
                        return true;
                    else
                        return false;
                };
                if (instance) {
                    settings.callback = {
                        onRightClick: function (event, treeId, treeNode) {
                            instance.invokeMethod('onRightClick', {
                                TreeNode: {
                                    Id: treeNode.id,
                                    Name: treeNode.name,
                                    Open: treeNode.open,
                                    Checked: treeNode.checked,
                                    IsFirstNode: treeNode.isFirstNode,
                                    IsParent: treeNode.IsParent,
                                    ParentId: treeNode.parentId,
                                    Level: treeNode.Level
                                    },
                                    X: event.clientX + document.body.scrollLeft,
                                    Y: event.clientY + document.body.scrollTop
                                });
                        },
                        beforeRename: function (treeId, treeNode, newName, isCancel) {
                            if (isCancel) return true;
                            var oldName = treeNode.name;
                            instance.invokeMethodAsync('onBeforeRename', {
                                TreeNode: {
                                    Id: treeNode.id,
                                    Name: treeNode.name,
                                    Open: treeNode.open,
                                    Checked: treeNode.checked,
                                    IsFirstNode: treeNode.isFirstNode,
                                    IsParent: treeNode.IsParent,
                                    Level: treeNode.Level
                                },
                                NewName: newName,
                                IsCancel: isCancel
                            }).then(cancel => {
                                treeNode.name = cancel ? oldName : newName;
                                var tree = window.FBComponents.zTree[treeId];
                                tree.refresh();
                            });
                            return true;   
                        },
                        beforeClick: function (treeId, treeNode, clickFlag) {
                            var result = instance.invokeMethod('onBeforeClick', {
                                TreeNode: {
                                    Id: treeNode.id,
                                    Name: treeNode.name,
                                    Open: treeNode.open,
                                    Checked: treeNode.checked,
                                    IsFirstNode: treeNode.isFirstNode,
                                    IsParent: treeNode.IsParent,
                                    Level: treeNode.Level
                                },
                                ClickFlag: clickFlag
                            });
                            /*.then(cancel => {
                                if (cancel) {
                                    var tree = window.FBComponents.zTree[treeId];
                                    tree.refresh();
                                }
                            }); */
                            return !result;
                        },
                        onClick: function (event, treeId, treeNode, clickFlag) {
                            instance.invokeMethod('onClick', {
                                TreeNode: {
                                    Id: treeNode.id,
                                    Name: treeNode.name,
                                    Open: treeNode.open,
                                    Checked: treeNode.checked,
                                    IsFirstNode: treeNode.isFirstNode,
                                    IsParent: treeNode.IsParent,
                                    ParentId: treeNode.parentId,
                                    Level: treeNode.Level
                                },
                                ClickFlag: clickFlag,
                            });
                        },
                    };
                }
                window.FBComponents.zTree[id] = $.fn.zTree.init($("#" + id), settings, nodes);                
            },
            addNode: function (instance, treeId, parentId, nodeId, nodeName) {
                var tree = window.FBComponents.zTree[treeId];
                if (!tree) return;
                var nodes = tree.getNodesByParam("id", parentId);
                if (nodes && nodes.length > 0) {
                    var nodes = tree.addNodes(nodes[0], { id: nodeId, name: nodeName });
                    tree.editName(nodes[0]);
                }
            },
            delNode: function (instance, treeId, nodeId) {
                var tree = window.FBComponents.zTree[treeId];
                if (!tree) return;
                var nodes = tree.getNodesByParam("id", nodeId);
                if (nodes && nodes.length > 0) {
                    tree.removeNode(nodes[0]);
                }
            }
        }
    };        
    }
)();