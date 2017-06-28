namespace Strata
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ActorPrefabStore : MonoBehaviour
    {
        public ActorPrefabCollection AnchorActorPrefabs;
        public ActorPrefabCollection StageActorPrefabs;
        public ActorPrefabCollection ShapeActorPrefabs;
        public ActorPrefabCollection BuildableActorPrefabs;

        public Actor DefaultActorPrefab;
        public Actor ActorRootPrefab;
     }
}