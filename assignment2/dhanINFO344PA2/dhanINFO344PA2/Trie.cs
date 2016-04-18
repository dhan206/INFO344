using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dhanINFO344PA2
{
    public class Trie
    {
        private TrieNode root;

        public Trie()
        {
            root = new TrieNode();
        }

        public void AddWord(string word)
        {
            if (word.Length > 0)
            {
                TrieNode currNode = this.root;
                foreach (char c in word)
                {
                    if (currNode.childNodes != null)
                    {
                        TrieNode match = currNode.childNodes.SingleOrDefault(n => n.character.Equals(c));
                        if (match != null)
                        {
                            currNode = match;
                        } else
                        {
                            TrieNode temp = new TrieNode(c, currNode);
                            currNode.AddChild(temp);
                            currNode = temp;
                        }
                    } else
                    {
                        TrieNode temp = new TrieNode(c, currNode);
                        currNode.AddChild(temp);
                        currNode = temp;
                    }

                }
                currNode.isLeaf = true; //last node is a leaf (end of word)
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
            foreach (char c in prefix)
            {
                lastNode = lastNode.childNodes.SingleOrDefault(n => n.character.Equals(c));
            }

            return lastNode.GetWords(new List<string>());
        }

       
    }
}