using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{
    public class Trie
    {
        private TrieNode root;

        //value to indicate number of query suggestions
        public static int maxResults = 10;

        public Trie()
        {
            root = new TrieNode();
        }

        /// <summary>
        /// is the trie empty
        /// </summary>
        public bool isEmpty
        {
            get { return root.ChildNodes == null; }
        }

        /// <summary>
        /// adds a word to the existing trie
        /// </summary>
        /// <param name="word"></param>
        public void AddWord(string word)
        {
            if (!string.IsNullOrEmpty(word))
            {
                TrieNode currNode = this.root;
                foreach (char c in word.ToLower())
                {
                    TrieNode value;
                    if (currNode.ChildNodes == null || !currNode.ChildNodes.TryGetValue(c, out value))
                    {
                        currNode.AddChild(new TrieNode(c));
                    }
                    currNode = currNode.ChildNodes[c];
                }
                currNode.IsLeaf = true; //last node is a leaf (end of word)
            }
        }

        /// <summary>
        /// get the words in the trie with the given prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns>a list containing all words found the trie with the given prefix</returns>
        public List<string> GetWords(string prefix)
        {
            TrieNode lastNode = root;

            //find the node that represents he last letter of the prefix
            foreach (char c in prefix.ToLower().Trim())
            {
                if (lastNode.ChildNodes.ContainsKey(c))
                {
                    lastNode = lastNode.ChildNodes[c];
                }
                else
                {
                    return new List<string>();
                }
            }
            var list = new List<string>();
            return lastNode.GetWords(ref list, prefix);
        }
    }
}