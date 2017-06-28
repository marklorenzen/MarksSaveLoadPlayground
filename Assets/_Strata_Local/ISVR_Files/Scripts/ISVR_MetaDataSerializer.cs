namespace Strata
{
    using UnityEngine;

    public static class ISVR_MetaData_Serializer
    {
        public static string Serialize(ISVR_MetaData data)
        {
            string json = JsonUtility.ToJson(data, true);
            return json;
        }

        public static ISVR_MetaData Deserialize(string json)
        {
            var data = JsonUtility.FromJson<ISVR_MetaData>(json);
            return data;
        }

    }
}
