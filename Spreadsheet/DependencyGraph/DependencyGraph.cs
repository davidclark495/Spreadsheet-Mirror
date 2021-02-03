// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)
// Implementation written by David Clark, February 2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        // the number of ordered pairs in the graph
        private int p_size;

        // a dictionary storing lists of dependents for each applicable node
        private Dictionary<string, HashSet<string>> dependentsDict;
        // a dictionary storing lists of dependees for each applicable node
        private Dictionary<string, HashSet<string>> dependeesDict;


        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependentsDict = new Dictionary<string, HashSet<string>>();
            dependeesDict = new Dictionary<string, HashSet<string>>();
            p_size = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return p_size; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get { return dependeesDict[s].Count; }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            return dependentsDict.ContainsKey(s);
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            return dependeesDict.ContainsKey(s);
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if(HasDependents(s))
                return dependentsDict[s];

            return new HashSet<string>();
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (HasDependees(s))
                return dependeesDict[s];

            return new HashSet<string>();
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            // if the dependency already exists, return without doing anything
            if (HasDependents(s) && GetDependents(s).Contains(t))
                return;
            
            // update list of dependents
            if (HasDependents(s))
               dependentsDict[s].Add(t);
            else
                dependentsDict.Add(s, new HashSet<string>( new string[]{ t } ));

            // update list of dependees
            if (HasDependees(t))
                dependeesDict[t].Add(s);
            else
                dependeesDict.Add(t, new HashSet<string>( new string[] { s } ));

            p_size++;
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            // if the dependency doesn't exist, return without doing anything
            if (!HasDependents(s) || !HasDependees(t))
                return;

            dependentsDict[s].Remove(t);
            dependeesDict[t].Remove(s);
            p_size--;
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (!HasDependents(s))
                return;

            // remove old dependents
            HashSet<string> oldDependents = new HashSet<string>(dependentsDict[s]);
            foreach (string oldDep in oldDependents)
                this.RemoveDependency(s, oldDep);

            // add new dependents
            foreach (string newDep in newDependents)
                this.AddDependency(s, newDep);
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            if (!HasDependees(s))
                return;

            // remove old dependees
            HashSet<string> oldDependees = new HashSet<string>(dependeesDict[s]);
            foreach (string oldDep in oldDependees)
                this.RemoveDependency(oldDep, s);

            // add new dependees
            foreach (string newDep in newDependees)
                this.AddDependency(newDep, s);
        }

    }

}
