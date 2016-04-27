using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace dhanINFO344PA2
{
    public class TrieNode
    {
        public char Character { get; private set; }
        public Dictionary<char, TrieNode> ChildNodes { get; set; } = null;
        public bool IsLeaf { get; set; } //indicates the last letter of a word

        public TrieNode()
        {
           new TrieNode('\0'); //set character to equivalent of null, null parent
        }

        public TrieNode(char c)
        {
            this.Character = c;
            this.IsLeaf = false; //defaults to false, overriden when new word added
        }

        /// <summary>
        /// adds a child to this node
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(TrieNode child)
        {
            if(this.ChildNodes == null)
            {
                this.ChildNodes = new Dictionary<char, TrieNode>();
            }
            this.ChildNodes.Add(child.Character, child);
        }

        /// <summary>
        /// gets the words that result from this node or any of its children
        /// </summary>
        /// <param name="results"></param>
        /// <returns>list of words with Trie.maxResults amount of words</returns>
        public List<string> GetWords(ref List<string> results, string s)
        {
            if (this.IsLeaf)
            {
                results.Add(s);
            }

            if(this.ChildNodes != null)
            {
                foreach(var key in this.ChildNodes.Keys)
                {
                    if (results.Count == Trie.maxResults)
                    {
                        return results;
                    }
                    this.ChildNodes[key].GetWords(ref results, s + key);
                }
            }
            return results;
        }
    }
}