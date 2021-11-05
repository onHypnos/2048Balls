using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//using UnityEditor;

namespace Dreamteck
{
    public class Octree
    {
        public class Node
        {
            private TS_Bounds _bounds = new TS_Bounds();
            private Node _parent = null;
            private Node _root = null;
            private Node[] _children = new Node[0];

            public TS_Bounds bounds
            {
                get { return _bounds; }
            }

            public Node topBackLeft
            {
                get { return _children[0]; }
            }

            public Node topFrontLeft
            {
                get { return _children[1]; }
            }

            public Node topFrontRight
            {
                get { return _children[2]; }
            }

            public Node topBackRight
            {
                get { return _children[3]; }
            }

            public Node bottomBackLeft
            {
                get { return _children[4]; }
            }

            public Node bottomFrontLeft
            {
                get { return _children[5]; }
            }

            public Node bottomFrontRight
            {
                get { return _children[6]; }
            }

            public Node bottomBackRight
            {
                get { return _children[7]; }
            }

            public bool isLeaf
            {
                get
                {
                    return _children.Length == 0;
                }
            }

            public Node parent
            {
                get
                {
                    return _parent;
                }
            }

            public Node root
            {
                get
                {
                    return _root;
                }
            }

            public Node[] children
            {
                get { return _children; }
            }

            internal Node(Bounds b)
            {
                _parent = null;
                _root = null;
                _bounds = new TS_Bounds(b);
            }

            internal Node(Node p)
            {
                _parent = p;
                Node par = _parent;
                while(par != null)
                {
                    if(par.parent == null)
                    {
                        _root = par;
                        break;
                    }
                    par = par.parent;
                }
            }

            internal void Append()
            {
                _children = new Node[8];
                TS_Bounds current = _bounds;
                current.size /= 2f;
                for (int i = 0; i < _children.Length; i++)
                {
                    switch (i)
                    {
                        //Top 4 cells
                        case 0: current.center = _bounds.center + new Vector3(-1f * _bounds.size.x / 4f, 1f * _bounds.size.y / 4f, -1f * _bounds.size.z / 4f); break;
                        case 1: current.center = _bounds.center + new Vector3(-1f * _bounds.size.x / 4f, 1f * _bounds.size.y / 4f, 1f * _bounds.size.z / 4f); break;
                        case 2: current.center = _bounds.center + new Vector3(1f * _bounds.size.x / 4f, 1f * _bounds.size.y / 4f, 1f * _bounds.size.z / 4f); break;
                        case 3: current.center = _bounds.center + new Vector3(1f * _bounds.size.x / 4f, 1f * _bounds.size.y / 4f, -1f * _bounds.size.z / 4f); break;
                        //Bottom 4 cells
                        case 4: current.center = _bounds.center + new Vector3(-1f * _bounds.size.x / 4f, -1f * _bounds.size.y / 4f, -1f * _bounds.size.z / 4f); break;
                        case 5: current.center = _bounds.center + new Vector3(-1f * _bounds.size.x / 4f, -1f * _bounds.size.y / 4f, 1f * _bounds.size.z / 4f); break;
                        case 6: current.center = _bounds.center + new Vector3(1f * _bounds.size.x / 4f, -1f * _bounds.size.y / 4f, 1f * _bounds.size.z / 4f); break;
                        case 7: current.center = _bounds.center + new Vector3(1f * _bounds.size.x / 4f, -1f * _bounds.size.y / 4f, -1f * _bounds.size.z / 4f); break;
                    }
                    _children[i] = new Node(this);
                    _children[i]._bounds = current;
                }
            }

            internal void Truncate()
            {
                if (isLeaf) return;
                if (!_children[0].isLeaf) return;
                _children = new Node[0];
            }



            public bool Contains(Vector3 point)
            {
                if (point.x < _bounds.min.x || point.x > _bounds.max.x) return false;
                if (point.y < _bounds.min.y || point.y > _bounds.max.y) return false;
                if (point.z < _bounds.min.z || point.z > _bounds.max.z) return false;
                return true;
            }

            public bool Intersects(TS_Bounds check)
            {
                bool intersects = true;
                if (_bounds.max.x < check.min.x) intersects = false;
                if (_bounds.max.y < check.min.y) intersects = false;
                if (_bounds.max.z < check.min.z) intersects = false;
                if (intersects) return true;
                intersects = true;
                if (check.max.x < _bounds.min.x) intersects = false;
                if (check.max.y < _bounds.min.y) intersects = false;
                if (check.max.z < _bounds.min.z) intersects = false;
                return intersects;
            }


            //Drawing methods

            public void DrawPath()
            {
                DrawBounds(false);
                Node p = parent;
                while (p != null)
                {
                    p.DrawBounds(false);
                    p = p.parent;
                }
            }

            public void DrawBounds(bool drawChildren)
            {
                Vector3 v3Center = bounds.center;
                Vector3 v3Extents = bounds.extents;
                Vector3 v3FrontTopLeft;
                Vector3 v3FrontTopRight;
                Vector3 v3FrontBottomLeft;
                Vector3 v3FrontBottomRight;
                Vector3 v3BackTopLeft;
                Vector3 v3BackTopRight;
                Vector3 v3BackBottomLeft;
                Vector3 v3BackBottomRight;

                v3FrontTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top left corner
                v3FrontTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top right corner
                v3FrontBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom left corner
                v3FrontBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom right corner
                v3BackTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top left corner
                v3BackTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top right corner
                v3BackBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom left corner
                v3BackBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom right corner

                Color color = Color.red;

                Debug.DrawLine(v3FrontTopLeft, v3FrontTopRight, color);
                Debug.DrawLine(v3FrontTopRight, v3FrontBottomRight, color);
                Debug.DrawLine(v3FrontBottomRight, v3FrontBottomLeft, color);
                Debug.DrawLine(v3FrontBottomLeft, v3FrontTopLeft, color);

                Debug.DrawLine(v3BackTopLeft, v3BackTopRight, color);
                Debug.DrawLine(v3BackTopRight, v3BackBottomRight, color);
                Debug.DrawLine(v3BackBottomRight, v3BackBottomLeft, color);
                Debug.DrawLine(v3BackBottomLeft, v3BackTopLeft, color);

                Debug.DrawLine(v3FrontTopLeft, v3BackTopLeft, color);
                Debug.DrawLine(v3FrontTopRight, v3BackTopRight, color);
                Debug.DrawLine(v3FrontBottomRight, v3BackBottomRight, color);
                Debug.DrawLine(v3FrontBottomLeft, v3BackBottomLeft, color);

                if (!drawChildren) return;
                if (!isLeaf)
                {
                    for (int i = 0; i < children.Length; i++)
                    {
                        children[i].DrawBounds(true);
                    }
                }
            }

            public void DrawCenter()
            {
                Debug.DrawRay(bounds.center, Vector3.up / 50f, Color.cyan);
                if (!isLeaf)
                {
                    for (int i = 0; i < children.Length; i++)
                    {
                        children[i].DrawCenter();
                    }
                }
            }

        }

        private int _depth = 1;
        public int depth
        {
            get { return _depth; }
        }
        private Node root;

        public Octree(Bounds b, int d)
        {
            root = new Node(b);
            SetDepth(d);
        }

        public void DrawRoot()
        {
            root.DrawBounds(false);
        }

        public void SetDepth(int targetDepth)
        {
            int currentDepth = 0;
            Node current = root;
            while (!current.isLeaf)
            {
                currentDepth++;
                current = current.children[0];
            }
            if (targetDepth == _depth) return;
            int delta = Mathf.Abs(_depth - targetDepth);
            for(int i = 0; i < delta; i++)
            {
                if (targetDepth < _depth) Truncate(root);
                else Append(root);
            }
            _depth = targetDepth;
        }

        private void Append(Node node)
        {
            if (node.isLeaf) node.Append();
            else
            {
                for (int i = 0; i < node.children.Length; i++) Append(node.children[i]);
            }
        }

        private void Truncate(Node node)
        {
            if (!node.isLeaf && node.children[0].isLeaf) node.Truncate();
            else
            {
                for (int i = 0; i < node.children.Length; i++) Truncate(node.children[i]);
            }
        }

        public Node FindContainingNode(Vector3 point, int maxSearchLevel = 0)
        {
            return FindContainingNodeLogic(root, point, maxSearchLevel);
        }

        private Node FindContainingNodeLogic(Node start, Vector3 point, int maxSearchLevel)
        {
            Node child = start;
            int level = 0;
            while (!child.isLeaf)
            {
                bool found = false;
                for (int i = 0; i < child.children.Length; i++)
                {
                    if (child.children[i].Contains(point))
                    {
                        child = child.children[i];
                        level++;
                        found = true;
                        if (maxSearchLevel > 0 && level >= maxSearchLevel) return child;
                        break;
                    }
                }
                if (!found)
                {
                    child = null;
                    break;
                }
            }
            return child;
        }

        public List<Node> FindIntersectingNodes(TS_Bounds check, int maxSearchLevel = 0)
        {
            List<Node> found = new List<Node>();
            int level = 0;
            FindIntersectingNodes(root, ref found, check, level, maxSearchLevel);
            return found;
        }

        private void FindIntersectingNodes(Node start, ref List<Node> found, TS_Bounds check, int currentLevel, int maxSearchLevel = 0)
        {
            if (!start.Intersects(check)) return;
            if (start.isLeaf || (maxSearchLevel > 0 && maxSearchLevel >= currentLevel)) found.Add(start);
            else
            {
                currentLevel++;
                for (int i = 0; i < start.children.Length; i++)
                {
                    FindIntersectingNodes(start.children[i], ref found, check, currentLevel, maxSearchLevel);
                }
            }
        }

        public List<Node> GetNeighbours(Node node, int maxSearchLevel = 0)
        {
            List<Node> neighbours = new List<Node>();
            Node found = FindContainingNode(node.bounds.center + Vector3.back * node.bounds.size.z + Vector3.up * node.bounds.size.y, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.back * node.bounds.size.z + Vector3.up * node.bounds.size.y + Vector3.left * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.up * node.bounds.size.y + Vector3.left * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.forward * node.bounds.size.z + Vector3.up * node.bounds.size.y + Vector3.left * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.forward * node.bounds.size.z + Vector3.up * node.bounds.size.y, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.forward * node.bounds.size.z + Vector3.up * node.bounds.size.y + Vector3.right * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.up * node.bounds.size.y + Vector3.right * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.back * node.bounds.size.z + Vector3.up * node.bounds.size.y + Vector3.right * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center +  Vector3.up * node.bounds.size.y, maxSearchLevel);
            if (found != null) neighbours.Add(found);

            found = FindContainingNode(node.bounds.center + Vector3.back * node.bounds.size.z, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.back * node.bounds.size.z + Vector3.left * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.left * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.forward * node.bounds.size.z + Vector3.left * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.forward * node.bounds.size.z, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.forward * node.bounds.size.z + Vector3.right * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.right * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.back * node.bounds.size.z + Vector3.right * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);

            found = FindContainingNode(node.bounds.center + Vector3.back * node.bounds.size.z + Vector3.down * node.bounds.size.y, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.back * node.bounds.size.z + Vector3.down * node.bounds.size.y + Vector3.left * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.down * node.bounds.size.y + Vector3.left * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.forward * node.bounds.size.z + Vector3.down * node.bounds.size.y + Vector3.left * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.forward * node.bounds.size.z + Vector3.down * node.bounds.size.y, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.forward * node.bounds.size.z + Vector3.down * node.bounds.size.y + Vector3.right * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.down * node.bounds.size.y + Vector3.right * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.back * node.bounds.size.z + Vector3.down * node.bounds.size.y + Vector3.right * node.bounds.size.x, maxSearchLevel);
            if (found != null) neighbours.Add(found);
            found = FindContainingNode(node.bounds.center + Vector3.down * node.bounds.size.y, maxSearchLevel);
            if (found != null) neighbours.Add(found);

            return neighbours;
        }

        public int[] GetPointsInRange(Vector3 point, Vector3[] points, bool[] inRange, float range, int maxDepth = 0)
        {
            TS_Bounds rangeBounds = new TS_Bounds(point, Vector3.one * range * 2f);
            if (inRange.Length != points.Length) inRange = new bool[points.Length];
            int containCount = 0;
            for (int i = 0; i < points.Length; i++)
            {
                inRange[i] = false;
                if (rangeBounds.Contains(points[i]))
                {
                    if(Vector3.Distance(point, points[i]) <= range)
                    {
                        inRange[i] = true;
                        containCount++;
                    }
                }
            }
            int[] result = new int[containCount];
            int index = 0;
            for (int i = 0; i < inRange.Length; i++)
            {
                if (inRange[i])
                {
                    result[index] = i;
                    index++;
                }
            }
            return result;
            /*
            int maxSearchLevel = 0;
            Node current = root;
            while (!current.isLeaf)
            {
                maxSearchLevel++;
                current = current.children[0];
                if (current.bounds.size.x/2f < range || current.bounds.size.y/2f < range || current.bounds.size.z/2f < range)
                {
                    break;
                }
            }
            current = FindContainingNode(point, maxSearchLevel);
            if (current == null) return new int[0];
            List<Node> all = GetNeighbours(current, maxSearchLevel);
            for (int i = 0; i < all.Count; i++)
            {
                float xDelta = Mathf.Abs(point.x - all[i].bounds.center.x)-all[i].bounds.size.x/2f;
                float yDelta = Mathf.Abs(point.y - all[i].bounds.center.y) - all[i].bounds.size.y / 2f;
                float zDelta = Mathf.Abs(point.z - all[i].bounds.center.z) - all[i].bounds.size.z / 2f;
                if (i < all.Count-1 && xDelta > range || yDelta > range || zDelta > range)
                {
                    all.RemoveAt(i);
                    i--;
                    continue;
                }
            }
            all.Add(current);
            List<int> inRange = new List<int>();
            for(int i = 0; i < points.Length; i++)
            {
                for(int n = 0; n < all.Count; n++)
                {
                    if (all[n].Contains(points[i]))
                    {
                        if (Vector3.Distance(point, points[i]) <= range) inRange.Add(i);
                        break;
                    }
                }
            }
            return inRange.ToArray();
            */
            
        }

    }
}
