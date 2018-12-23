using System;
using System.Collections.Generic;
using System.Text;

namespace KiranicoScraper
{
    /// <summary>
    /// A class used to keep track of Kiranico weapon trees for purposes of parent IDs.
    /// </summary>
    class WeaponStack
    {
        private int _depth;
        private Stack<int> _stack;

        public WeaponStack()
        {
            _stack = new Stack<int>();
        }

        /// <summary>
        /// Resets the stack with a new weapon as the root.
        /// </summary>
        /// <param name="id">The database id of the root weapon of the tree.</param>
        public void Reset(int id)
        {
            _stack.Clear();
            _stack.Push(id);
            // The root of a tree should always have a depth of zero.
            _depth = 0;
        }

        /// <summary>
        /// Adds a weapon to the stack.
        /// </summary>
        /// <param name="id">The database id of the weapon to push onto the stack.</param>
        public void Push(int id)
        {
            _stack.Push(id);
            _depth++;
        }

        /// <summary>
        /// Gets the parent id of a weapon with a specified depth, unwinding the stack if necessary.
        /// </summary>
        /// <param name="depth">The depth of the weapon whose parent id should be retrieved.</param>
        /// <returns>The database id of the weapon's parent.</returns>
        public int PopToParent(int depth)
        {
            // The same depth as the stack head means that the weapon's sibling is at the top of the stack, so we pop it to access its parent.
            if (depth == _depth)
            {
                Pop();
                return _stack.Peek();
            }
            // If the weapon's depth is one greater than the head of the stack, it means that it's a direct child and the parent is at the top of the stack.
            else if (depth - _depth == 1)
            {
                return _stack.Peek();
            }
            // If the supplied depth is less than the depth of the head it means that we jumped to another branch of the tree, so the stack must be unwound to one _before_ the supplied depth, in other words its parent.
            else if (depth < _depth)
            {
                while (_depth > depth - 1)
                {
                    Pop();
                }
                return _stack.Peek();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(depth), depth, "Supplied depth should never be 0 or more than 1 greater than the stack head.");
            }
        }

        private void Pop()
        {
            _stack.Pop();
            _depth--;
        }
    }
}
