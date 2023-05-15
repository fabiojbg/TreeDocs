using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apps.Blazor.Components.Navigation
{
    internal interface ITreeViewInterop
    {
        Task CreateTreeViewAsync(string treeId, object settings, TreeViewNode[] nodes);
        Task AddNode(string treeId, string parentNodeId, string nodeId, string nodeName);
        Task DeleteNode(string treeId, string nodeId);
        Action<RightNodeClickEventArgs> OnRightClick { get; set; }
        Action<BeforeNodeRenameEventArgs> OnBeforeRename { get; set; }
        Action<NodeClickEventArgs> OnClick { get; set; }
        Action<BeforeNodeClickEventArgs> OnBeforeClick { get; set; }
    }
    public class TreeNode
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Open { get; set; }
        public bool Checked { get; set; }
        public bool IsFirstNode { get; set; }
        public bool IsParent { get; set; }
        public string ParentId { get; set; }
        public int Level { get; set; }
    }

    public class RightNodeClickEventArgs
    {
        public TreeNode TreeNode { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class NodeClickEventArgs
    {
        public TreeNode TreeNode { get; set; }
        public int clickFlag { get; set; }
    }

    public class BeforeNodeClickEventArgs
    {
        public TreeNode TreeNode { get; set; }
        public int clickFlag { get; set; }
        public bool? Cancel { get; set; }
    }

    public class BeforeNodeRenameEventArgs
    {
        public TreeNode TreeNode { get; set; }
        public string NewName { get; set; }
        public bool? Cancel { get; set; }
    }

    internal class TreeViewInterop : ITreeViewInterop, IDisposable
    {
        private IJSRuntime _jsRuntime { get; }
        private DotNetObjectReference<TreeViewInterop> _thisObjRef;
        public Action<RightNodeClickEventArgs> OnRightClick { get; set; }
        public Action<BeforeNodeRenameEventArgs> OnBeforeRename { get; set; }
        public Action<NodeClickEventArgs> OnClick { get; set; }
        public Action<BeforeNodeClickEventArgs> OnBeforeClick { get; set; }


        private DotNetObjectReference<TreeViewInterop> objRef => _thisObjRef ??= DotNetObjectReference.Create(this);

        public TreeViewInterop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
        public async Task CreateTreeViewAsync(string treeId, object settings, TreeViewNode[] nodes)
        {
            await _jsRuntime.InvokeAsync<string>("FBComponents.treeView.create", objRef, treeId, settings, nodes);
        }

        public async Task AddNode(string treeId, string parentNodeId, string nodeId, string nodeName)
        {
            await _jsRuntime.InvokeAsync<string>("FBComponents.treeView.addNode", objRef, treeId, parentNodeId, nodeId, nodeName);
        }

        public async Task DeleteNode(string treeId, string nodeId)
        {
            await _jsRuntime.InvokeAsync<string>("FBComponents.treeView.delNode", objRef, treeId, nodeId);
        }

        [JSInvokable]
        public void onRightClick(RightNodeClickEventArgs args)
        {
            OnRightClick?.Invoke(args);
        }

        [JSInvokable]
        public async Task<bool> onBeforeRename(BeforeNodeRenameEventArgs args)
        {
            OnBeforeRename?.Invoke(args);
            while( !args.Cancel.HasValue) // horrible implementation but for some strange reason, the invoke is returning after any await is call in the calling action
            {
                await Task.Delay(500);
            }

            return args.Cancel ?? true;
        }

        [JSInvokable]
        public void onClick(NodeClickEventArgs args)
        {
            OnClick?.Invoke(args);
        }

        [JSInvokable]
        public bool onBeforeClick(BeforeNodeClickEventArgs args)
        {
            OnBeforeClick?.Invoke(args);

            return args.Cancel ?? true;
        }

        public void Dispose()
        {
            _thisObjRef.Dispose();
        }
    }
}
