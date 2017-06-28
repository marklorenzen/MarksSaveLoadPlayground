namespace Strata
{
    using System;
    using UnityEngine;

    public class ActorComponentBeACapsule : ActorComponent
    {
        public Actor _ancestor;
        public override ActorID GetActorAncestor()
        {
            return _ancestor.GetID();
        }

        [Serializable]
        public new class Data 
        {
            [SerializeField]
            public Color _color;
        }

        protected ActorComponent.Data _data;
        public override ActorComponent.Data GetData()
        {
            return _data;
        }

        protected override void Awake()
        {
            base.Awake();

            if (_data == null)
            {
                _data = new ActorComponent.Data();
                _data.BeACapsuleData = new Data[] { new Data() };
            }
            Renderer ren = GetComponent<Renderer>();
            if (ren != null)
                _data.BeACapsuleData[0]._color = ren.material.color;
        }

 
        public override void OnLoad(ISVR_MetaData.ActorRecord record)
        {
            if (record._components.Count > 0)
                _data = record._components[0]._data;//todo account for multiple components, for goodness sake


            Renderer ren = GetComponent<Renderer>();
            if (ren != null)
            {
                if (_data != null)
                {
                    var capsuleData = _data.BeACapsuleData;
                    if (capsuleData != null && capsuleData.Length > 0)
                    {
                        if (capsuleData[0] != null)
                        {
                            ren.material.color = capsuleData[0]._color;
                        }
                        else
                            Debug.LogWarning("null color at beAcapsuleData[0] in ActorComnponentBeACapsule.OnLoad " + name);

                    }
                    else
                        Debug.LogWarning("null or empty capsuleData in ActorComnponentBeACapsule.OnLoad " + name);

                }
                else
                    Debug.LogWarning("null data in ActorComnponentBeACapsule.OnLoad " + name);
            }
        }

    }
}
