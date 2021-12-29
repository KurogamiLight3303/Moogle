using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoogleEngine
{
    public class SearchBase
    {
        private List<SearchNode>? Heads;
        private Stream source;
        private string? text;
        public SearchBase(Stream stream)
        {
            source = stream;
        }
        public SearchResult Search(string pattern)
        {

            SearchNode h, t, child;
            #region "Load" 
            if (Heads == null)
            {
                Heads = new List<SearchNode>();
                bool head = true;
                string last = string.Empty;
                text = string.Empty;

                while (source.Position < source.Length)
                {
                    char c = (char)source.ReadByte();
                    text += c;
                    c = char.ToLower(c);
                    if (head)
                    {
                        if ((h = Heads.FirstOrDefault(p => p.Character == c)) != null)
                            h.Positions.Add(source.Position);
                        else
                            Heads.Add(new SearchNode()
                            {
                                Character = c,
                                Positions = new List<long>() { source.Position }
                            });
                        head = false;
                        last += c;
                    }
                    else
                    {
                        if (c != ' ')
                        {
                            h = Heads.Where(p => p.Character == last[0]).FirstOrDefault();
                            foreach (char r in last[1..last.Length].ToString())
                                h = h.Next?.FirstOrDefault(p => p.Character == r);

                            if (h.Next != null && (t = h.Next.FirstOrDefault(p => p.Character == c)) != null)
                                t.Positions.Add(source.Position);
                            else if (h.Next != null)
                            {
                                h.Next.Add(new SearchNode()
                                {
                                    Character = c,
                                    Positions = new List<long>() { source.Position }
                                });
                            }
                            else
                                h.Next = new List<SearchNode>(){ new SearchNode()
                            {
                                Character = c,
                                Positions = new List<long>() { source.Position }
                            }
                                };

                            last += c;

                        }
                        else
                        {
                            head = true;
                            last = string.Empty;
                        }
                    }



                }
            }
            #endregion

            #region "Search"
            bool found;
            int n;
            List<SearchItem> items = new ();
            foreach (string word in pattern.ToLower().Split(" "))
            {
                found = true;
                n = 1;
                child = h = Heads.Where(p => p.Character == word[0]).FirstOrDefault();
                if (h != null)
                {
                    foreach (char c in word[1..word.Length])
                    {
                        n++;
                        if (child.Next == null || (child = child.Next.FirstOrDefault(p => p.Character == c)) == null)
                        {
                            found = false;
                            break;
                        }
                    }
                }
                else
                    found = false;
                

                if (found)
                    foreach(var i in child.Positions)
                        items.Add(new SearchItem(word, word, 1));
            }

            return new SearchResult(items.ToArray());
            #endregion
        }
    }
}