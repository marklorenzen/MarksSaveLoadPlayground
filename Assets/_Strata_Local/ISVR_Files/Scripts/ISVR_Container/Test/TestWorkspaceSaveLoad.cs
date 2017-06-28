namespace Strata
{
    using UnityEngine;

    //this class is throw-away
    public class TestWorkspaceSaveLoad : MonoBehaviour
    {
        public const string TEST_METADATA_FILENAME = "MetaDataTest";

        public void Save()
        {
            EventManager.TriggerEvent(new EventWorkspaceSave(TEST_METADATA_FILENAME));//todo this is just for test
        }

        public void Load()
        {
            EventManager.TriggerEvent(new EventWorkspaceLoad(TEST_METADATA_FILENAME));//todo this is just for test
        }

        public void New()
        {
            //todo obsolete and stupid
            EventManager.TriggerEvent(new EventWorkspaceNew(WorkspaceManager.WorkspaceType.PresentationEdit));
        }
    }

    

}
