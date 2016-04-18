using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dhanINFO344PA2
{
    public class TrieNode
    {
        public char character { get; private set; }
        public TrieNode parent { get; private set; }
        public List<TrieNode> childNodes { get; private set; } = new List<TrieNode>();
        public Boolean isLeaf { get; set; }

        public TrieNode()
        {
           new TrieNode('\0', null); //set character to equivalent of null, null parent
        }

        public TrieNode(char c, TrieNode parentNode)
        {
            this.character = c;
            this.parent = parentNode;
            this.isLeaf = false; //defaults to false, overriden when new words added
        }

        public void AddChild(TrieNode child)
        {
            this.childNodes.Add(child);
        }

        public List<string> GetWords(List<string> results)
        {
            if (isLeaf)
            {
                results.Add(toString());
            }
            if(childNodes.Count != 0)
            {
                childNodes.ForEach(delegate(TrieNode children) {
                    children.GetWords(results);
                });
            }
            return results;
        }

        public string toString()
        {
            if(this.parent == null)
            {
                return "";
            } else
            {
                return this.parent.toString() + this.character;
            }
        }
    }
}