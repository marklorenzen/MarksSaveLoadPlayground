namespace Strata
{
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class WorkspaceTest : WorkspaceBase
    {
        private Color _color;

        public override bool AddActor(IActor actor) { return false; } //TODO this should allow any actor, cuz we are a test class, yo
        public override bool IsRead_OnlyWorkspace() { return false; } //never read-only
        public override string GetSpecificSuffix() { return ""; } //inspecific, allows any isvr file/metadata file
        public override Color GetColor() { return _color; }

        void OnEnable()
        {
            var renderers = GetComponentsInChildren<Renderer>();

            foreach(var ren in renderers)
                ren.material.color = GetColor();
        }

        private void Awake()
        {
            //so we can easily tell the workspaces apart in the inspector :-)
            _color = Color.HSVToRGB(Random.value, Random.value, .5f + .5f * Random.value);
        }

    }
}

