using System;
using System.Windows.Forms;

namespace FlashDevelop.Controls
{
    public class ExtTreeNode : TreeNode
    {
        #region sorting

        /// <summary>
        /// Inserts this node into the specified <paramref name="parentNode"/>
        /// at the position determined by the comparer
        /// of the TreeView which contains the <paramref name="parentNode"/>,
        /// assuming that all other immediate child nodes of the <paramref name="parentNode"/>
        /// are in sorted order.
        /// </summary>
        public void InsertSorted(TreeNode parentNode, Boolean onItemAdd = true)
        {
            DragDropTreeView etv = parentNode.TreeView as DragDropTreeView;
            if (etv.enableSorting)
                this.Insert(this.GetInsertionIndex(parentNode.Nodes, parentNode.TreeView), parentNode, onItemAdd);
            else
                this.Insert(parentNode.Nodes.Count, parentNode, onItemAdd);

        }

        /// <summary>
        /// Obtains the insertion index into parent node
        /// </summary>
        int GetInsertionIndex(TreeNodeCollection nodes, TreeView treeView)
        {
            if (treeView == null)
                return nodes.Count;

            Comparison<TreeNode> comparison = null;

            DragDropTreeView etv = treeView as DragDropTreeView;
            if (etv == null)
            {
                if (treeView.TreeViewNodeSorter != null)
                    comparison = treeView.TreeViewNodeSorter.Compare;
            }
            else
            {
                if (etv.NodeSorter != null)
                    comparison = etv.NodeSorter.Compare;
            }

            if (comparison == null) return nodes.Count;
            for (int i = 0; i < nodes.Count; ++i)
            {
                if (comparison(this, nodes[i]) < 0)
                    return i;
            }

            return nodes.Count;
        }

        /// <summary>
        /// Inserts into parent node based on index
        /// </summary>
        public void Insert(int index, TreeNode parentNode, Boolean onItemAddEvent = true)
        {
            parentNode.Nodes.Insert(index, this);

            if (onItemAddEvent)
            {
                DragDropTreeView etv = parentNode.TreeView as DragDropTreeView;
                etv.OnItemAdd(this);
            }
        }
        #endregion

    }
}
