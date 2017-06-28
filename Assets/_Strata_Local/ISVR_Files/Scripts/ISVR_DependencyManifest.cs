namespace Strata
{
    using System.Collections.Generic;
    using System;
    using UnityEngine;

    /// <summary>
    /// track all the dependencies in a workspace
    /// keyed by actorID (each actorcomponent is assumed to have zero or one dependency)
    /// each dependency is a URL of a file 
    /// an asset bundle (e.g. a buildable)
    /// or an importable file (e.g. a collada) 
    /// or another ISVR file (e.g. a stage)
    /// </summary>
    [Serializable]
    public class ISVR_DependencyManifest 
    {
        public ISVR_DependencyManifest()
        {

        }

        public ISVR_DependencyManifest( string[] URLs)
        {
            foreach (var url in URLs)
                AddDependency(ActorID.InvalidID, url);
        }

        public ISVR_DependencyManifest( List <ISVR_MetaData.ActorRecord> actorRecordList )
        {
            foreach (var ar in actorRecordList)
            {
                if (ar._actor != null)
                    AddDependency(ar._id, ar._actor.GetDependencyURL());
            }
        }


        [Serializable]
        public class Dependency
        {
            [SerializeField]
            public ActorID _id;

            [SerializeField]
            public string _url;

            public Dependency(ActorID id, string url)
            {
                _id = id;
                _url = url;
            }
        }

        [SerializeField]
        private List<Dependency> _dependencies = new List<Dependency>();

        public List<Dependency> Dependencies
        {
            get { return _dependencies; }
        }

        public void AddDependency(ActorID actorComponent, string url)
        {
            Dependencies.Add( new Dependency(actorComponent, url) );
        }


    }
}