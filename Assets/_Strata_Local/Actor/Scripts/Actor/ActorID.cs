namespace Strata
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct ActorID
    {
        const string INVALID = "invalid";

        [SerializeField]
        private ulong _key;

        [SerializeField]
        public string _name;

        public const ulong Invalid = 7772222777722227777UL;//1234567890

        [NonSerialized]
        private Color _color;
        public Color Color
        {
            get
            {
                if (_color.a < 1f)
                {
                    byte b = (byte)(_key & 0xff);
                    _color = Color.HSVToRGB(b/255f, .5f, 1);
                    _color.a = 1f;
                }

                return _color;
            }
        }

        public ActorID(ulong value, string name)
        {
            _key = value;
            _name = name;
            _color = Color.yellow * .8f; 
         }
        public ulong Value { get { return _key; } }

        private static ActorID _invalidID = new ActorID(Invalid, INVALID);
        public static ActorID InvalidID { get { return _invalidID; } }

        public bool IsValid { get { return _key != Invalid; } }

        public static bool operator !=(ActorID id1, ActorID id2)
        {
            return !(id1._key == id2._key);
        }

        public static bool operator ==(ActorID id1, ActorID id2)
        {
            return id1._key == id2._key;
        }

    }
}
